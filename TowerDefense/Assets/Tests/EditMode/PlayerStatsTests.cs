using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class PlayerStatsTests
{
    [Test]
    public void Money_StartsWithCorrectAmount()
    {
        PlayerStats.ResetStats();
        Assert.AreEqual(20, PlayerStats.Money);
    }

    [Test]
    public void Money_DecreasesAfterTowerPurchase()
    {
        PlayerStats.ResetStats();
        PlayerStats.Money -= 10;
        Assert.AreEqual(10, PlayerStats.Money);
    }

    [Test]
    public void PlayerStats_ResetStats_SetsDefaultValues()
    {
        PlayerStats.Money = 999;
        PlayerStats.Lives = 1;

        PlayerStats.ResetStats();

        Assert.AreEqual(20, PlayerStats.Money);
        Assert.AreEqual(15, PlayerStats.Lives);
    }

    [Test]
    public void TurretBlueprint_GetSellAmount_Level0_ReturnsHalfCost()
    {
        var blueprint = new TurretBlueprint { cost = 10 };
        Assert.AreEqual(5, blueprint.GetSellAmount(0));
    }

}


