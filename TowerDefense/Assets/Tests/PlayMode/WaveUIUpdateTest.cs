using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using TMPro;

public class WaveUIUpdateTest
{
    [UnityTest]
    public IEnumerator WaveNumber_UI_UpdatesCorrectly()
    {
        var waveSpawner = GameObject.FindObjectOfType<WaveSpawner>();
        Assert.IsNotNull(waveSpawner);

        yield return new WaitForSeconds(2f);

        var waveText = waveSpawner.waveNumberText;
        Assert.IsNotNull(waveText);

        string[] split = waveText.text.Split('/');
        int currentWave = int.Parse(split[0]);
        int maxWave = int.Parse(split[1]);

        Assert.AreEqual(waveSpawner.waveNumber, currentWave);
        Assert.AreEqual(waveSpawner.maxWaves, maxWave);
    }
}
