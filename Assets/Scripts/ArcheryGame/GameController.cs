using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace ArcheryGame
{
    public class GameController : MonoBehaviour
    {
        private const string FileName = "A_game_scores.json";
        
        [Header("UI Screens")]
        [SerializeField] private StartScreen _startScreen;
        [SerializeField] private PauseScreen _pauseScreen;
        [SerializeField] private FailScreen _failScreen;
        [SerializeField] private WinScreen _winScreen;
        [SerializeField] private ScoreScreen _scoreScreen;

        [Header("Gameplay Components")]
        [SerializeField] private Arrow _arrow;
        [SerializeField] private Target _target;
        [SerializeField] private HitTarget _hitTarget;

        [Header("UI Elements")]
        [SerializeField] private TMP_Text _scoreText;
        [SerializeField] private Button _stopButton;
        [SerializeField] private GameObject _splashScreen;

        [Header("Camera Controller")]
        [SerializeField] private CameraCloseUpController _closeUpController;

        private int _levelUpScore = 20;
        private int _score;
        private int _missCount;

        private enum GameState
        {
            Starting,
            Playing,
            Paused,
            Won,
            Lost
        }

        private GameState _currentGameState;

        private void Start()
        {
            _startScreen.Enable();
            _splashScreen.gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            _startScreen.StartCompleted += StartNewGame;
            
            _scoreScreen.NextClicked += HandleNextRound;
            _winScreen.NextClicked += HandleLevelUp;
            _winScreen.ExitClicked += GoToMainScene;

            _stopButton.onClick.AddListener(SendArrow);

            _failScreen.AgainClicked += StartNewGame;
            _failScreen.ExitClicked += GoToMainScene;

            _pauseScreen.ContinueClicked += ContinueGame;
            _pauseScreen.AgainClicked += StartNewGame;
            _pauseScreen.ExitClicked += GoToMainScene;
            
            _arrow.TargetHit += ArrowHitTarget;
            _arrow.Missed += ArrowMissed;
        }

        private void OnDisable()
        {
            _startScreen.StartCompleted -= StartNewGame;
            
            _scoreScreen.NextClicked -= HandleNextRound;
            _winScreen.NextClicked -= HandleLevelUp;
            _winScreen.ExitClicked -= GoToMainScene;

            _stopButton.onClick.RemoveListener(SendArrow);

            _failScreen.AgainClicked -= StartNewGame;
            _failScreen.ExitClicked -= GoToMainScene;

            _pauseScreen.ContinueClicked -= ContinueGame;
            _pauseScreen.AgainClicked -= StartNewGame;
            _pauseScreen.ExitClicked -= GoToMainScene;
            
            _arrow.TargetHit -= ArrowHitTarget;
            _arrow.Missed -= ArrowMissed;
        }
        
        public void PauseGame()
        {
            if (_currentGameState != GameState.Playing) return;

            _currentGameState = GameState.Paused;
            
            if (_target.isActiveAndEnabled)
                _target.StopRandomMovement();

            if (_arrow.isActiveAndEnabled)
                _arrow.StopMovement();
            
            _stopButton.gameObject.SetActive(false);

            _pauseScreen.Enable();
        }

        private void ContinueGame()
        {
            if (_currentGameState != GameState.Paused) return;

            _currentGameState = GameState.Playing;
            
            if (_target.isActiveAndEnabled)
                _target.StartRandomMovement();

            _stopButton.gameObject.SetActive(true);
            
            _pauseScreen.Disable();
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
                    _startScreen.Disable();
                    break;

                case GameState.Won:
                    ShowScoreOrWinScreen();
                    break;

                case GameState.Lost:
                    GameLost();
                    break;
            }
        }

        public void StartNewGame()
        {
            _winScreen.Disable();
            _failScreen.Disable();
            _pauseScreen.Disable();
            _scoreScreen.Disable();

            _score = 0;
            _missCount = 0;

            _scoreText.text = _score.ToString();

            if (_arrow.isActiveAndEnabled)
            {
                _arrow.ReturnToDefaultPosition();
                _arrow.gameObject.SetActive(false);
            }

            _target.ReturnToDefaultPosition();
            _target.gameObject.SetActive(true);
            _target.StartRandomMovement();

            _stopButton.gameObject.SetActive(true);

            _closeUpController.ShowDefaultView();
            
            _levelUpScore = 20;
            
            SetGameState(GameState.Playing);
        }

        public void SendArrow()
        {
            _target.StopRandomMovement();
            Vector2 targetPosition = _target.GetCurrentPosition();

            StartCoroutine(ShowSplashScreen(0.5f));

            _target.gameObject.SetActive(false);
            _arrow.gameObject.SetActive(true);
            _arrow.StartMovement(targetPosition);
            _closeUpController.ShowCloseUp();
            _stopButton.gameObject.SetActive(false);
            _hitTarget.CircleCollider2D.isTrigger = true;
        }

        private IEnumerator ShowSplashScreen(float duration)
        {
            _splashScreen.SetActive(true);
            yield return new WaitForSeconds(duration);
            _splashScreen.SetActive(false);
        }

        private void ArrowHitTarget()
        {
            _arrow.StopMovement();
            //_arrow.ReturnToDefaultPosition();
            //_arrow.gameObject.SetActive(false);

            _score++;
            _scoreText.text = _score.ToString();

            SetGameState(GameState.Won);
        }

        private void ArrowMissed()
        {
            _arrow.StopMovement();
            _missCount++;

            if (_missCount >= 3)
            {
                SetGameState(GameState.Lost);
                return;
            }
            
            
            StartCoroutine(PauseBeforeNextRound());
        }
        
        private IEnumerator PauseBeforeNextRound()
        {
            yield return new WaitForSeconds(1f);
            _arrow.ReturnToDefaultPosition();
            _arrow.gameObject.SetActive(false);
            HandleNextRound();
        }
        
        private void ShowScoreOrWinScreen()
        {
            if (_score >= _levelUpScore)
            {
                _winScreen.Enable(_score.ToString());
            }
            else
            {
                _scoreScreen.Enable(_score.ToString());
            }
        }

        private void HandleNextRound()
        {
            ResetRound();
            SetGameState(GameState.Playing);
        }

        private void HandleLevelUp()
        {
            _levelUpScore += 20;
            _target.IncreaseSpeed();
            ResetRound();
            SetGameState(GameState.Playing);
        }

        private void ResetRound()
        {
            _arrow.ReturnToDefaultPosition();
            _arrow.gameObject.SetActive(false);

            _target.ReturnToDefaultPosition();
            _target.gameObject.SetActive(true);
            _target.StartRandomMovement();

            _stopButton.gameObject.SetActive(true);
            _closeUpController.ShowDefaultView();

            _hitTarget.CircleCollider2D.isTrigger = false;
        }

        private void GameLost()
        {
            _stopButton.gameObject.SetActive(false);

            if (_target.isActiveAndEnabled)
                _target.StopRandomMovement();

            if (_arrow.isActiveAndEnabled)
                _arrow.StopMovement();

            _failScreen.Enable("Game Over!");
        }

        private void ResetAllValues()
        {
            _missCount = 0;
            _score = 0;
            
            _arrow.ReturnToDefaultPosition();
            _arrow.gameObject.SetActive(false);

            _target.ReturnToDefaultPosition();
            _target.gameObject.SetActive(true);
            _target.StartRandomMovement();

            _stopButton.gameObject.SetActive(true);
            _closeUpController.ShowDefaultView();
            
            _hitTarget.CircleCollider2D.isTrigger = false;

            _scoreText.text = _score.ToString();
            _stopButton.gameObject.SetActive(true);
        }

        public void GoToMainScene()
        {
            SaveScores();
            SceneManager.LoadScene("MainScene");
        }
        
        private void SaveScores()
        {
            if(_score <= 0 || _missCount <= 0)
                return;
            
            GameScores scores = new GameScores
            {
                PlayerScore = _score,
                EnemyScore = _missCount
            };

            SaveLoadManager.SaveScores(FileName,scores);
        }
    }
}
