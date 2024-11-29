using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreScreen : MonoBehaviour
{
    [SerializeField] private TMP_Text _score;
    
    public event Action NextClicked;

    private void Start()
    {
        Disable();
    }

    public void Enable(string text)
    {
        gameObject.SetActive(true);
        _score.text = text + " points";
    }

    public void Disable()
    {
        gameObject.SetActive(false);
    }

    public void OnNextClicked()
    {
        NextClicked?.Invoke();
        Disable();
    }
}
