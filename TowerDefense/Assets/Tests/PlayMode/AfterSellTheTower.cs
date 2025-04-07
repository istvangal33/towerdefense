using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;

public class AfterSellTheTower
{
    Node node;
    TurretBlueprint blueprint;
    BuildManager buildManager;

    [UnitySetUp]
    public IEnumerator SetUp()
    {
        if (SceneManager.GetActiveScene().name != "LVL1")
        {
            SceneManager.LoadScene("LVL1");
            yield return new WaitForSeconds(2f);
        }

        yield return new WaitForSeconds(1f);

        node = GameObject.FindObjectOfType<Node>();
        Assert.IsNotNull(node, "Nem talalhato a Node!");

        blueprint = GameObject.FindObjectOfType<Shop>()?.Machinegun;
        Assert.IsNotNull(blueprint, "Machinegun blueprint nem talalhato!");

        buildManager = BuildManager.instance;
        Assert.IsNotNull(buildManager, "BuildManager.instance null!");

        PlayerStats.Money = 999;
    }

    private bool IsUnityNull(Object obj)
    {
        return obj == null || !obj;
    }

    [UnityTest]
    public IEnumerator Node_Becomes_Free_After_Sell()
    {
        buildManager.SelectTurretToBuild(blueprint);
        node.BuildTurret(blueprint);

        yield return new WaitForSeconds(0.5f);

        Assert.IsTrue(node.isOccupied, "A Node nem lett foglalt az epites utan!");
        Assert.IsFalse(IsUnityNull(node.turret), "A Node-on nincs torony az epites utan!");

        buildManager.SelectNode(node);
        node.SellTurret();

        yield return new WaitForSeconds(0.5f); 

        Assert.IsFalse(node.isOccupied, "sell utan a Node nem szabadult fel!");
        Assert.IsTrue(IsUnityNull(node.turret), "sell utan meg mindig van torony a Node-on!");
    }
}