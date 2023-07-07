using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] private Button[] menu;
    [SerializeField] private Button[] restart;

    private void Awake()
    {
        foreach (var item in menu)
        {
            item.onClick.AddListener(() => SceneManager.LoadScene("Menu"));
        }
        
        foreach (var item in restart)
        {
            item.onClick.AddListener(() => SceneManager.LoadScene("Game"));
        }
    }
}
