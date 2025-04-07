using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class TowerOccupancyTests
{
    [Test]
    public void CalculatesCorrectTowerOccupancyPercentage()
    {
        // Arrange
        int totalTowerSpots = 20;
        int placedTowers = 5;

        // Act
        float occupancy = CalculateOccupancy(placedTowers, totalTowerSpots);

        // Assert
        Assert.AreEqual(25f, occupancy, 0.01f, "Occupancy percentage should be 25%");
    }

    private float CalculateOccupancy(int placed, int total)
    {
        if (total == 0) return 0f;
        return (float)placed / total * 100f;
    }
}
