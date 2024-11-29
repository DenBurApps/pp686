using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace VolleyballGame
{
    public class GameController : MonoBehaviour
    {
        private const string FileName = "V_game_scores.json";
        
        [SerializeField] private Player _player;
        [SerializeField] private Enemy _enemy;
        [SerializeField] private Field _playerField;
        [SerializeField] private Field _enemyField;
        [SerializeField] private VolleyballBall _ball;
        [SerializeField] private PauseScreen _pauseScreen;
        [SerializeField] private FailScreen _failScreen;
        [SerializeField] private WinScreen _winScreen;
        [SerializeField] private StartScreen _startScreen;
        [SerializeField] private TMP_Text _playerScoreText;
        [SerializeField] private TMP_Text _enemyScoreText;

        private int _playerScore;
        private int _enemyScore;
        private readonly int _maxRoundScore = 24;

        private enum GameState { Starting, Playing, Paused, Won, Lost }
        private GameState _currentGameState;

        private void OnEnable()
        {
            SubscribeToEvents();
        }

        private void OnDisable()
        {
            UnsubscribeFromEvents();
        }

        private void Start()
        {
            SetGameState(GameState.Starting);
        }

        private void SubscribeToEvents()
        {
            _pauseScreen.ContinueClicked += ProcessContinue;
            _pauseScreen.ExitClicked += GoToMainScene;
            _pauseScreen.AgainClicked += StartNewGame;

            _startScreen.StartCompleted += StartNewGame;

            _winScreen.NextClicked += StartNewGameAfterWin;
            _winScreen.ExitClicked += GoToMainScene;

            _failScreen.AgainClicked += StartNewGame;
            _failScreen.ExitClicked += GoToMainScene;

            _enemyField.BallHit += RoundWin;
            _playerField.BallHit += RoundLost;
        }

        private void UnsubscribeFromEvents()
        {
            _pauseScreen.ContinueClicked -= ProcessContinue;
            _pauseScreen.ExitClicked -= GoToMainScene;
            _pauseScreen.AgainClicked -= StartNewGame;

            _startScreen.StartCompleted -= StartNewGame;

            _winScreen.NextClicked -= StartNewGameAfterWin;
            _winScreen.ExitClicked -= GoToMainScene;

            _failScreen.AgainClicked -= StartNewGame;
            _failScreen.ExitClicked -= GoToMainScene;
            
            _enemyField.BallHit -= RoundWin;;
            _playerField.BallHit -= RoundLost;
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
                    StartNewRound();
                    break;

                case GameState.Paused:
                    ProcessPause();
                    break;

                case GameState.Won:
                    GameWin();
                    break;

                case GameState.Lost:
                    GameLost();
                    break;
            }
        }

        private void StartNewGame()
        {
            SetGameState(GameState.Playing);
        }

        private void StartNewGameAfterWin()
        {
            _enemy.IncreaseSpeed();
            SetGameState(GameState.Playing);
        }

        public void ProcessPause()
        {
            StopAllGameplay();
            _pauseScreen.Enable();
        }

        private void ProcessContinue()
        {
            _pauseScreen.Disable();
            ResumeAllGameplay();
        }

        private void RoundWin()
        {
            _playerScore++;
            StopAllGameplay();

            _playerScoreText.text = _playerScore.ToString();
            if (_playerScore >= _maxRoundScore)
            {
                SetGameState(GameState.Won);
                return;
            }

            StartNewRound();
        }

        private void RoundLost()
        {
            _enemyScore++;
            StopAllGameplay();

            _enemyScoreText.text = _enemyScore.ToString();
            if (_enemyScore >= _maxRoundScore)
            {
                SetGameState(GameState.Lost);
                return;
            }

            StartNewRound();
        }

        private void GameWin()
        {
            StopAllGameplay();
            _winScreen.Enable($"{_playerScore} : {_enemyScore}");
        }

        private void GameLost()
        {
            StopAllGameplay();
            _failScreen.Enable($"{_playerScore} : {_enemyScore}");
        }

        private void StartNewRound()
        {
            SaveScores();
            ResumeAllGameplay();
            
            _enemy.StartFollowing();
            _enemy.ResetPosition();
            _player.ResetPosition();
            _ball.ResetBall();
            _ball.LaunchBall();
        }

        private void ResetAllValues()
        {
            _playerScore = 0;
            _enemyScore = 0;

            _playerScoreText.text = _playerScore.ToString();
            _enemyScoreText.text = _enemyScore.ToString();
            _enemy.ResetPosition();
            _player.ResetPosition();
            
            _pauseScreen.Disable();
            _winScreen.Disable();
            _failScreen.Disable();

            StopAllGameplay();
        }

        private void StopAllGameplay()
        {
            _player.ResetPosition();
            _enemy.StopFollowing();
            _ball.StopBall();
        }

        private void ResumeAllGameplay()
        {
            _enemy.StartFollowing();
            _ball.StartBall();
        }

        private void GoToMainScene()
        {
            SceneManager.LoadScene("MainScene");
        }
        
        private void SaveScores()
        {
            if(_playerScore <= 0 || _enemyScore <= 0)
                return;
            
            GameScores scores = new GameScores
            {
                PlayerScore = _playerScore,
                EnemyScore = _enemyScore
            };

            SaveLoadManager.SaveScores(FileName,scores);
        }
    }
}
