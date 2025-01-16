using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public static int startMoney = 20;
    public static int Money = 20;
    private static int previousMoney;
    private static int previousLives;
    public static int Lives = 15;

    void Start()
    {
        Money = startMoney;
        previousMoney = Money;
    }

    void Update()
    {
        if (Lives != previousLives)
        {
            GameManager.Instance.LogPlayerLivesChange(previousLives, Lives);
            previousLives = Lives;
        }
    }
}
