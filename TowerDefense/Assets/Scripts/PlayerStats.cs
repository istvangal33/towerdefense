using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public static int startMoney = 20;
    public static int Money;
    private static int previousMoney;
    private static int previousLives;
    public static int Lives = 15;

    private static bool isFirstLevel = true; 

    void Start()
    {
        if (isFirstLevel)
        {
            Money = startMoney;
            isFirstLevel = false;
        }

        previousMoney = Money;
        previousLives = Lives;
    }

    void Update()
    {
       
        if (Lives != previousLives)
        {
            GameManager.Instance.LogPlayerLivesChange(previousLives, Lives);
            previousLives = Lives;
        }

        
        if (Money != previousMoney)
        {
            GameManager.Instance.LogPlayerMoneyChange(previousMoney, Money);
            previousMoney = Money;
        }
    }

    public static void ResetStats() 
    {
        isFirstLevel = true;
        Money = startMoney;
        Lives = 15;
    }
}
