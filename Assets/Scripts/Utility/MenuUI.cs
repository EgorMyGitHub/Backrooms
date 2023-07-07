using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuUI : MonoBehaviour
{
    [SerializeField] private Button start;
    [SerializeField] private Button leaderboard;

    private void Awake()
    {
        start.onClick.AddListener(() => SceneManager.LoadScene("Game"));
        leaderboard.onClick.AddListener(() => SceneManager.LoadScene("Leaderboard"));
    }
}
