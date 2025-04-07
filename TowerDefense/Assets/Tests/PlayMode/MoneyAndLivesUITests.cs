using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;

public class MoneyAndLivesUITests
{
    [UnitySetUp]
    public IEnumerator SetUp()
    {
        if (SceneManager.GetActiveScene().name != "LVL1")
        {
            SceneManager.LoadScene("LVL1");
            yield return new WaitForSeconds(2f);
        }
    }

    [UnityTest]
    public IEnumerator MoneyAndLivesUI_Matches_PlayerStats()
    {
        
        yield return new WaitForSeconds(1f);

        var moneyUI = GameObject.FindObjectOfType<MoneyUI>();
        var livesUI = GameObject.FindObjectOfType<LivesUI>();

        Assert.IsNotNull(moneyUI, "MoneyUI nem talalhato a jelenetben!");
        Assert.IsNotNull(livesUI, "LivesUI nem talalhato a jelenetben!");

        yield return null;

        string moneyText = moneyUI.moneyText.text;
        string livesText = livesUI.livesText.text;

        Assert.AreEqual(PlayerStats.Money.ToString(), moneyText, "MoneyUI nem egyezik a PlayerStats-szal!");
        Assert.AreEqual(PlayerStats.Lives.ToString(), livesText, "LivesUI nem egyezik a PlayerStats-szal!");
    }
}
