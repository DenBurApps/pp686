using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace FlightGame
{
    public class GameController : MonoBehaviour
    {
        private const string FileName = "F_game_scores.json";
        
        private const int SpawnInterval = 1;

        [SerializeField] private FailScreen _failScreen;
        [SerializeField] private WinScreen _winScreen;
        [SerializeField] private PauseScreen _pauseScreen;
        [SerializeField] private TMP_Text _scoreText;
        [SerializeField] private Player _player;
        [SerializeField] private StartScreen _startScreen;
        [SerializeField] private InteractableObjectSpawner _spawner;

        private int _score;
        private int _virusHitCount;

        private IEnumerator _spawnCoroutine;

        private enum GameState { Starting, Playing, Paused, Won, Lost }
        private GameState _currentGameState;

        private void Start()
        {
            _startScreen.Enable();
        }

        private void OnEnable()
        {
            _startScreen.StartCompleted += StartNewGame;

            _player.VirusCatched += ProcessVirusCatched;
            _player.BallCatched += ProcessBallCatched;

            _failScreen.AgainClicked += ProcessGameRestart;
            _failScreen.ExitClicked += ProcessGoToMainScene;

            _winScreen.NextClicked += ProcessNextClicked;
            _winScreen.ExitClicked += ProcessGoToMainScene;

            _pauseScreen.ContinueClicked += ProcessContinue;
            _pauseScreen.ExitClicked += ProcessGoToMainScene;
            _pauseScreen.AgainClicked += ProcessGameRestart;
        }

        private void OnDisable()
        {
            _startScreen.StartCompleted -= StartNewGame;

            _player.VirusCatched -= ProcessVirusCatched;
            _player.BallCatched -= ProcessBallCatched;

            _failScreen.AgainClicked -= ProcessGameRestart;
            _failScreen.ExitClicked -= ProcessGoToMainScene;

            _winScreen.NextClicked -= ProcessNextClicked;
            _winScreen.ExitClicked -= ProcessGoToMainScene;

            _pauseScreen.ContinueClicked -= ProcessContinue;
            _pauseScreen.ExitClicked -= ProcessGoToMainScene;
            _pauseScreen.AgainClicked -= ProcessGameRestart;
        }

        private void StartNewGame()
        {
            _winScreen.Disable();
            _failScreen.Disable();
            _pauseScreen.Disable();
            SetGameState(GameState.Playing);
        }

        private void SetGameState(GameState newState)
        {
            _currentGameState = newState;

            switch (_currentGameState)
            {
                case GameState.Starting:
                    ResetAllValues();
                    _startScreen.Enable();
                    break;

                case GameState.Playing:
                    ResetAllValues();
                    _startScreen.Disable();
                    if (_spawnCoroutine == null)
                    {
                        _spawnCoroutine = StartSpawning();
                        StartCoroutine(_spawnCoroutine);
                    }
                    break;

                case GameState.Paused:
                    ProcessPause();
                    break;

                case GameState.Won:
                    ProcessGameWin();
                    break;

                case GameState.Lost:
                    ProcessGameLost();
                    break;
            }
        }

        private IEnumerator StartSpawning()
        {
            WaitForSeconds interval = new WaitForSeconds(SpawnInterval);

            while (true)
            {
                _spawner.Spawn();
                yield return interval;
            }
        }

        public void ProcessPause()
        {
            if (_spawnCoroutine != null)
                StopCoroutine(_spawnCoroutine);

            _pauseScreen.Enable();
            _spawner.ReturnAllObjectsToPool();
        }

        private void ProcessContinue()
        {
            _pauseScreen.Disable();

            if (_spawnCoroutine != null)
                StartCoroutine(_spawnCoroutine);
        }

        private void ProcessGoToMainScene()
        {
            SaveScores();
            SceneManager.LoadScene("MainScene");
        }

        private void ProcessGameRestart()
        {
            ResetAllValues();
            StartNewGame();
        }

        private void ProcessBallCatched(MovingObject @object)
        {
            if (@object is Virus)
            {
                ProcessGameLost();
                return;
            }

            _score++;
            _spawner.ReturnToPool(@object);
            _scoreText.text = _score.ToString();

            if (_score >= 10)
            {
                SetGameState(GameState.Won);
            }
        }

        private void ProcessGameWin()
        {
            if (_spawnCoroutine != null)
                StopCoroutine(_spawnCoroutine);

            _winScreen.Enable(string.Empty);
            _spawner.ReturnAllObjectsToPool();
        }

        private void ProcessVirusCatched(MovingObject movingObject)
        {
            _virusHitCount++;
            _spawner.ReturnToPool(movingObject);

            if (_virusHitCount >= 3)
            {
                SetGameState(GameState.Lost);
            }
        }

        private void ProcessGameLost()
        {
            if (_spawnCoroutine != null)
                StopCoroutine(_spawnCoroutine);

            _spawner.ReturnAllObjectsToPool();
            _failScreen.Enable(string.Empty);
        }

        private void ProcessNextClicked()
        {
            _spawner.IncreaceSpeed();
            StartNewGame();
        }

        private void ResetAllValues()
        {
            _virusHitCount = 0;
            _score = 0;

            if (_spawnCoroutine != null)
                StopCoroutine(_spawnCoroutine);

            _spawnCoroutine = null;
            _scoreText.text = _score.ToString();
            _spawner.ReturnAllObjectsToPool();
        }
        
        private void SaveScores()
        {
            if(_score <= 0 || _virusHitCount <= 0)
                return;
            
            GameScores scores = new GameScores
            {
                PlayerScore = _score,
                EnemyScore = _virusHitCount
            };

            SaveLoadManager.SaveScores(FileName,scores);
        }
    }
}
