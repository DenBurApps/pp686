using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WinScreen : MonoBehaviour
{
    [SerializeField] private TMP_Text _score;
    [SerializeField] private Button _nextButton;
    [SerializeField] private Button _exitButton;

    private ScreenVisabilityHandler _screenVisabilityHandler;

    public event Action NextClicked;
    public event Action ExitClicked;

    private void Awake()
    {
        _screenVisabilityHandler = GetComponent<ScreenVisabilityHandler>();
    }

    private void Start()
    {
        Disable();
    }

    private void OnEnable()
    {
        _nextButton.onClick.AddListener(OnAgainClicked);
        _exitButton.onClick.AddListener(OnExitClicked);
    }

    private void OnDisable()
    {
        _nextButton.onClick.RemoveListener(OnAgainClicked);
        _exitButton.onClick.RemoveListener(OnExitClicked);
    }

    public void Enable(string score)
    {
        if (_score != null)
            _score.text = score;
        
        _screenVisabilityHandler.EnableScreen();
    }

    public void Disable()
    {
        _screenVisabilityHandler.DisableScreen();
    }

    private void OnAgainClicked() => NextClicked?.Invoke();
    private void OnExitClicked() => ExitClicked?.Invoke();
}
