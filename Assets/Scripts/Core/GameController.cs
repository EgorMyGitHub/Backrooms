using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    [SerializeField] private GameObject gameLoseOverlay;
    [SerializeField] private GameObject gameWinOverlay;

    [SerializeField] private Text timerDisplay;
    
    private Timer timer;

    private Leadearboard leadearboard;
    
    private void Awake()
    {
        gameLoseOverlay.SetActive(false);
        gameWinOverlay.SetActive(false);
    }

    private void Start()
    {
        leadearboard = new Leadearboard();
        
        timer = ComponentRoot.Resolve<Timer>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F4))
            SetGameOver(true);
        
        if (Input.GetKeyDown(KeyCode.F5))
            SetGameOver(false);
    }

    public void SetGameOver(bool isWin)
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        
        if(isWin)
            Win();
        else
            Lose();
    }
    
    private void Win()
    {
        timerDisplay.text = $"Your Timer: {timer.second:00}.{timer.millisecond:000} Sec";
        
        StartCoroutine(leadearboard.SubmitResult(timer));
        
        gameWinOverlay.SetActive(true);
    }

    private void Lose() =>
        gameLoseOverlay.SetActive(true);
}
