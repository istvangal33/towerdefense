using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;

public class NodeUIValueTests
{
    [UnitySetUp]
    public IEnumerator SetUp()
    {
        if (SceneManager.GetActiveScene().name != "LVL1")
        {
            SceneManager.LoadScene("LVL1");
            yield return new WaitForSeconds(2f);
        }

        yield return new WaitForSeconds(1f);
    }

    [UnityTest]
    public IEnumerator NodeUI_ShowsCorrectUpgradeAndSellValues()
    {
        
        Node targetNode = null;
        Node[] nodes = GameObject.FindObjectsOfType<Node>();

        foreach (var node in nodes)
        {
            if (node != null && node.turret == null && !node.isOccupied)
            {
                targetNode = node;
                break;
            }
        }

        Assert.IsNotNull(targetNode, "Nincs ures node!");

        
        PlayerStats.Money = 999;

        var buildManager = BuildManager.instance;
        Assert.IsNotNull(buildManager, "BuildManager nem talalhato!");

        var blueprint = GameObject.FindObjectOfType<Shop>()?.Machinegun;
        Assert.IsNotNull(blueprint, "Machinegun blueprint nem talalhato!");

        buildManager.SelectTurretToBuild(blueprint);
        targetNode.BuildTurret(blueprint);

        yield return new WaitForSeconds(0.5f);

        Assert.IsNotNull(targetNode.turret, "Nem sikerult tornyot epiteni a node-ra!");

        
        buildManager.SelectNode(targetNode);
        yield return null;

        string upgradeText = buildManager.nodeUI.upgradeCostText.text;
        string sellText = buildManager.nodeUI.sellAmountText.text;

        int expectedUpgrade = targetNode.upgradeLevel == 0
            ? blueprint.upgradeCost
            : blueprint.upgradeCost2;

        int expectedSell = blueprint.GetSellAmount(targetNode.upgradeLevel);

        StringAssert.Contains(expectedUpgrade.ToString(), upgradeText, "fejlesztesi koltseg nem megfelelo!");
        StringAssert.Contains(expectedSell.ToString(), sellText, "eladasi ertek nem megfelelo!");
    }
}
