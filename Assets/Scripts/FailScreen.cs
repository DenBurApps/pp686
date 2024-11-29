using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(ScreenVisabilityHandler))]
public class FailScreen : MonoBehaviour
{
    [SerializeField] private TMP_Text _score;
    [SerializeField] private Button _againButton;
    [SerializeField] private Button _exitButton;

    private ScreenVisabilityHandler _screenVisabilityHandler;

    public event Action AgainClicked;
    public event Action ExitClicked;

    private void Awake()
    {
        _screenVisabilityHandler = GetComponent<ScreenVisabilityHandler>();
    }

    private void OnEnable()
    {
        _againButton.onClick.AddListener(OnAgainClicked);
        _exitButton.onClick.AddListener(OnExitClicked);
    }

    private void OnDisable()
    {
        _againButton.onClick.RemoveListener(OnAgainClicked);
        _exitButton.onClick.RemoveListener(OnExitClicked);
    }
    
    private void Start()
    {
        Disable();
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

    private void OnAgainClicked() => AgainClicked?.Invoke();
    private void OnExitClicked() => ExitClicked?.Invoke();
}