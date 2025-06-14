using System.Collections.Generic;
using UnityEngine;

public class GameCanvas : MonoBehaviour
{
    public static GameCanvas Instance;
    
    [SerializeField]
    List<RocketUI> rocketsUI;
    
    [SerializeField]
    float recoverDelay = .1f;

    private void Awake()
    {
        if (!Instance)
        {
            Instance = this;
            return;
        }

        Destroy(gameObject);
    }

    public void UpdateRocketsUI(int newRocketsAmount)
    {
        float currentRecoverDelay = 0;

        for (int i = 0; i < rocketsUI.Count; i++)
        {
            RocketUI rocketUI = rocketsUI[i];

            if (!rocketUI.IsShown && i < newRocketsAmount)
            {
                rocketUI.OnRocketRecovered(currentRecoverDelay);
                currentRecoverDelay += recoverDelay;
            }
            else if (rocketUI.IsShown && i >= newRocketsAmount)
            {
                rocketUI.OnRocketFired();
            }
        }
    }
}
