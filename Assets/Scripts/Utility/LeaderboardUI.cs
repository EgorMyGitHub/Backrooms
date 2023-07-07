using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LeaderboardUI : MonoBehaviour
{
    [SerializeField] private TMP_Text leaderboard;

    [SerializeField] private Button back;
    
    private void Awake()
    {
        back.onClick.AddListener(() => SceneManager.LoadScene("Menu"));
        
        StartCoroutine(new Leadearboard().SetSubmit(leaderboard));
    }
}
