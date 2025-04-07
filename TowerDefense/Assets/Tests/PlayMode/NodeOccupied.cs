using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public class NodeOccupied
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
    public IEnumerator CannotBuildOnOccupiedNode()
    {
        Node node = GameObject.FindObjectOfType<Node>();
        Assert.IsNotNull(node, "Nem talalhato a Node!");

        PlayerStats.Money = 100;

        var buildManager = BuildManager.instance;
        Assert.IsNotNull(buildManager, "BuildManager nem talalhato!");

        var blueprint = GameObject.FindObjectOfType<Shop>()?.Machinegun;
        Assert.IsNotNull(blueprint, "Machinegun blueprint nem található!");

        buildManager.SelectTurretToBuild(blueprint);
        node.BuildTurret(blueprint);

        yield return null;

        Assert.IsTrue(node.isOccupied, "Node nem lett foglaltnak jelolve!");

       
        if (node.turret != null && node.turret.tag != "Tower")
        {
            node.turret.tag = "Tower";
        }

        
        buildManager.SelectTurretToBuild(blueprint);
        node.BuildTurret(blueprint);

        yield return null;

        Assert.AreEqual(1, GameObject.FindGameObjectsWithTag("Tower").Length, "tobbszoros toronyepites megtortént egy node-ra!");
    }
}
