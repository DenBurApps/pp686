using System;
using System.Collections.Generic;
using System.Linq;
using DanielLochner.Assets.SimpleScrollSnap;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(ScreenVisabilityHandler))]
public class MainScreenGameUpdater : MonoBehaviour
{
    private const string ArcheryFileName = "A_game_scores.json";
    private const string FlightFileName = "F_game_scores.json";
    private const string VolleyballFileName = "V_game_scores.json";
    
    [SerializeField] private SimpleScrollSnap _scrollSnap;
    [SerializeField] private List<GameObject> _panels;
    [SerializeField] private List<LeaderboardElement> _volleyballElements;
    [SerializeField] private List<LeaderboardElement> _archeryElements;
    [SerializeField] private List<LeaderboardElement> _flightElements;
    [SerializeField] private Button _volleyballGameButton;
    [SerializeField] private Button _archeryGameButton;
    [SerializeField] private Button _flightGameButton;
    [SerializeField] private Button _settingsButton;
    [SerializeField] private Settings _settingsScreen;

    private ScreenVisabilityHandler _screenVisabilityHandler;
    
    private void Awake()
    {
        _scrollSnap.OnPanelCentered.AddListener(UpdateInfo);
        _screenVisabilityHandler = GetComponent<ScreenVisabilityHandler>();
        UpdateInfo(0, 0);
    }

    private void OnEnable()
    {
        _volleyballGameButton.onClick.AddListener(RunVolleyball);
        _archeryGameButton.onClick.AddListener(RunArchery);
        _flightGameButton.onClick.AddListener(RunFlight);
        _settingsButton.onClick.AddListener(OnSettingsClicked);

        _settingsScreen.BackClicked += _screenVisabilityHandler.EnableScreen;
    }

    private void OnDisable()
    {
        _volleyballGameButton.onClick.RemoveListener(RunVolleyball);
        _archeryGameButton.onClick.RemoveListener(RunArchery);
        _flightGameButton.onClick.RemoveListener(RunFlight);
        _settingsButton.onClick.RemoveListener(OnSettingsClicked);
        
       _settingsScreen.BackClicked -= _screenVisabilityHandler.EnableScreen;
    }

    private void Start()
    {
       LoadAndUpdateLeaderboard(ArcheryFileName, _archeryElements);
       LoadAndUpdateLeaderboard(FlightFileName, _flightElements);
       LoadAndUpdateLeaderboard(VolleyballFileName, _volleyballElements);
    }

    private void UpdateInfo(int start, int end)
    {
        foreach (var item in _panels)
        {
            item.SetActive(false);
        }

        _panels[start].SetActive(true);
    }

    private void LoadAndUpdateLeaderboard(string fileName, List<LeaderboardElement> elements)
    {
        // Load scores from SaveLoadManager
        List<GameScores> scores = SaveLoadManager.LoadScores(fileName);

        // Convert scores to LeaderboardElementData
        List<LeaderboardElementData> data = scores
            .OrderByDescending(score => score.PlayerScore)
            .Select(score =>
            {
                int playerScore = score.PlayerScore;
                int enemyScore = score.EnemyScore;
                
                WinStatus winStatus = playerScore > enemyScore ? WinStatus.Win : WinStatus.Fail;

                return new LeaderboardElementData($"{playerScore} / {enemyScore}", winStatus);
            })
            .ToList();
        
        UpdateScores(data, elements);
    }
    
    private void UpdateScores(List<LeaderboardElementData> data, List<LeaderboardElement> elements)
    {
        foreach (var score in data)
        {
            var selectedElement = elements.FirstOrDefault(e => e.Data == null);

            if (selectedElement != null)
            {
                selectedElement.SetData(score);
            }
        }
    }

    private void RunFlight() => SceneManager.LoadScene("FlightScene");
    private void RunVolleyball() => SceneManager.LoadScene("Volleyball");
    private void RunArchery() => SceneManager.LoadScene("ArcheryScene");

    private void OnSettingsClicked()
    {
        _settingsScreen.ShowSettings();
        _screenVisabilityHandler.DisableScreen();
    }
}