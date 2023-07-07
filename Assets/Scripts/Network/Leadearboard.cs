using System.Collections;
using System.Collections.Generic;
using LootLocker.Requests;
using TMPro;
using UnityEngine;

public class Leadearboard
{
    private const int leaderboardId = 15827;
    
    public IEnumerator SubmitResult(Timer timer)
    {
        bool isDone = false;

        LootLockerSDKManager.SubmitScore(
            LoginController.playerId.ToString(),
            timer.second, leaderboardId,
            timer.millisecond.ToString(),
            (response) =>
        {
            if (response.success)
            {
                Debug.Log("IsComplete");
                isDone = true;
            }
            
        });

        yield return new WaitWhile(() => isDone);
    }

    public IEnumerator SetSubmit(TMP_Text text)
    {
        bool isDone = false;

        LootLockerSDKManager.GetScoreList(leaderboardId.ToString(), 10, (response) =>
        {
            if (response.success)
            {
                var rank = 1;
                
                for (int i = response.items.Length - 1; i >= 0; i--)
                {
                    text.text += $"#{rank} {response.items[i].player.name} - {response.items[i].score}.{float.Parse(response.items[i].metadata):0} \n";
                    rank++;
                }
            }
        });

        yield return new WaitWhile(() => isDone);
    }
}
