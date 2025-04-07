using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class TurretBlueprintTests
{
    [Test]
    public void TurretBlueprint_UpgradeCostsCorrect()
    {
        var blueprint = new TurretBlueprint();
        blueprint.upgradeCost = 10;
        blueprint.upgradeCost2 = 20;

        Assert.AreEqual(10, blueprint.upgradeCost);
        Assert.AreEqual(20, blueprint.upgradeCost2);
    }

    [Test]
    public void TurretBlueprint_SellAmountCalculatesCorrectly()
    {
        var blueprint = new TurretBlueprint();
        blueprint.cost = 10;
        blueprint.upgradeCost = 8;
        blueprint.upgradeCost2 = 12;

        Assert.AreEqual(5, blueprint.GetSellAmount(0));  
        Assert.AreEqual(9, blueprint.GetSellAmount(1));  
        Assert.AreEqual(15, blueprint.GetSellAmount(2)); 
    }

    [Test]
    public void TurretBlueprint_StatGettersReturnCorrectValues()
    {
        var blueprint = new TurretBlueprint();
        blueprint.rangeByLevel = new float[] { 3f, 4f, 5f };
        blueprint.fireRateByLevel = new float[] { 1f, 1.5f, 2f };
        blueprint.damageByLevel = new int[] { 10, 20, 30 };

        Assert.AreEqual(3f, blueprint.GetRange(0));
        Assert.AreEqual(4f, blueprint.GetRange(1));
        Assert.AreEqual(5f, blueprint.GetRange(2));

        Assert.AreEqual(1f, blueprint.GetFireRate(0));
        Assert.AreEqual(1.5f, blueprint.GetFireRate(1));
        Assert.AreEqual(2f, blueprint.GetFireRate(2));

        Assert.AreEqual(10, blueprint.GetDamage(0));
        Assert.AreEqual(20, blueprint.GetDamage(1));
        Assert.AreEqual(30, blueprint.GetDamage(2));
    }

    [Test]
    public void TurretBlueprint_GetStats_ReturnsCorrectFormattedString()
    {
        var blueprint = new TurretBlueprint
        {
            cost = 20,
            upgradeCost = 40,
            upgradeCost2 = 60,
            rangeByLevel = new float[] { 3f, 4f, 5f },
            fireRateByLevel = new float[] { 1f, 2f, 3f },
            damageByLevel = new int[] { 10, 20, 30 }
        };

        string stats = blueprint.GetStats(1);
        StringAssert.Contains("Cost: 20", stats);
        StringAssert.Contains("UpgradeCost1: 40", stats);
        StringAssert.Contains("UpgradeCost2: 60", stats);
        StringAssert.Contains("Range: 4", stats);
        StringAssert.Contains("FireRate: 2", stats);
        StringAssert.Contains("Damage: 20", stats);
    }
}
