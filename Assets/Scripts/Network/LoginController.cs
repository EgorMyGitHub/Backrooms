using System.Collections;
using System.Collections.Generic;
using LootLocker.Requests;
using UnityEngine;

public class LoginController
{
    public static int playerId { get; private set; }
    
    public IEnumerator Login(string Name)
    {
        bool isDone = false;
        
        LootLockerSDKManager.StartGuestSession((response) =>
        {
            if (response.success)
            {
                Debug.Log("IsLogin");
                isDone = true;

                playerId = response.player_id;
                
                LootLockerSDKManager.SetPlayerName(Name, (response) =>
                {
                    if (response.success)
                        Debug.Log($"Your Nick: {response.name}");
                });
            }
        });

        yield return new WaitWhile(() => isDone);
    }
}
