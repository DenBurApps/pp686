using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(ScreenVisabilityHandler))]
public class PauseScreen : MonoBehaviour
{
    [SerializeField] private Button _againButton;
    [SerializeField] private Button _exitButton;
    [SerializeField] private Button _continueButton;
    
    private ScreenVisabilityHandler _screenVisabilityHandler;
    
    public event Action AgainClicked;
    public event Action ExitClicked;
    public event Action ContinueClicked;

    private void Awake()
    {
        _screenVisabilityHandler = GetComponent<ScreenVisabilityHandler>();
    }

    private void OnEnable()
    {
        _againButton.onClick.AddListener(OnAgainClicked);
        _exitButton.onClick.AddListener(OnExitClicked);
        _continueButton.onClick.AddListener(OnContinueClicked);
    }

    private void OnDisable()
    {
        _againButton.onClick.RemoveListener(OnAgainClicked);
        _exitButton.onClick.RemoveListener(OnExitClicked);
        _continueButton.onClick.RemoveListener(OnContinueClicked);
    }
    
    private void Start()
    {
        Disable();
    }

    public void Enable()
    {
        _screenVisabilityHandler.EnableScreen();
    }

    public void Disable()
    {
        _screenVisabilityHandler.DisableScreen();
    }

    private void OnAgainClicked() => AgainClicked?.Invoke();
    private void OnExitClicked() => ExitClicked?.Invoke();
    private void OnContinueClicked() => ContinueClicked?.Invoke();
}
