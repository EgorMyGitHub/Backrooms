using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DataUpdate : MonoBehaviour
{
    [SerializeField] private Text time;
    [SerializeField] private Text data;

    private List<string> months = new List<string>()
    {
        "Jan",
        "Feb",
        "Mar",
        "Apr",
        "May",
        "Jun",
        "Jul",
        "Aug",
        "Sep",
        "Oct",
        "Nov",
        "Dec"
    };
    
    private void Awake()
    {
        StartCoroutine(UpdateRoutine());
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }

    private IEnumerator UpdateRoutine()
    {
        while (true)
        {
            UpdateAll();
            
            yield return new WaitForSeconds(1f);
        }
    }

    private void UpdateAll()
    {
        var data = DateTime.Now;

        var dataSystem = data.Hour > 12 ? "PM" : "AM";
        var hour = dataSystem == "PM" ? data.Hour - 12 : data.Hour;
        
        UpdateTime($"{dataSystem} {hour:00}:{data.Minute:00}:{data.Second:00}");
        UpdateData($"{months[data.Month - 1]} {data.Day:00} {data.Year}");
    }

    private void UpdateTime(string time) =>
        this.time.text = time;

    private void UpdateData(string data) =>
        this.data.text = data;
}
