using System;
using TMPro;
using UnityEngine;

public class LeaderboardElement : MonoBehaviour
{
    private const string DefaultScore = "-";
    private const string DefaultStatus = "-";
    
    [SerializeField] private Color _winColor;
    [SerializeField] private Color _failColor;
    [SerializeField] private Color _defaultColor;
    
    [SerializeField] private TMP_Text _scoreText;
    [SerializeField] private TMP_Text _winStatusText;

    private LeaderboardElementData _data;

    public LeaderboardElementData Data => _data;

    private void Start()
    {
        if (_data == null)
        {
            _scoreText.text = DefaultScore;
            _winStatusText.text = DefaultStatus;
            SetColor(_defaultColor);
        }
        else
        {
            SetData(_data);
        }
    }

    public void SetData(LeaderboardElementData data)
    {
        _data = data;

        _scoreText.text = _data.Score;

        if (_data.Status == WinStatus.Fail)
        {
            _winStatusText.text = "fail";
            SetColor(_failColor);
        }
        else
        {
            _winStatusText.text = "win";
            SetColor(_winColor);
        }
    }

    private void SetColor(Color color)
    {
        _scoreText.color = color;
        _winStatusText.color = color;
    }
}

[Serializable]
public class LeaderboardElementData
{
    public string Score;
    public WinStatus Status;

    public LeaderboardElementData(string score, WinStatus status)
    {
        Score = score;
        Status = status;
    }
}

[Serializable]
public enum WinStatus
{
    Win,
    Fail,
    None
}
