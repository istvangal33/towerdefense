using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public class AIOutputChangesWithDifferentInputs
{
    [UnityTest]
    public IEnumerator AI_Predicts_DifferentOutputs_For_DifferentInputs()
    {
        
        if (SceneManager.GetActiveScene().name != "LVL1")
        {
            SceneManager.LoadScene("LVL1");
            yield return new WaitForSeconds(2f);
        }

        yield return new WaitForSeconds(1f); 

        var model = GameObject.FindObjectOfType<ONNXModelLoader>();
        Assert.IsNotNull(model, "ONNXModelLoader nem található a jelenetben!");

        
        float[] input1 = new float[] { 1, 1, 1, 0 };
        float[] input2 = new float[] { 5, 3, 10, 70 };
        float[] input3 = new float[] { 4, 10, 15, 100 };

        float[] output1 = model.Predict(input1);
        float[] output2 = model.Predict(input2);
        float[] output3 = model.Predict(input3);


        Assert.AreEqual(output1.Length, output2.Length, "Az output tömb hossza nem egyezik!");

        
        bool outputsDiffer = false;
        for (int i = 0; i < output1.Length; i++)
        {
            if (Mathf.Abs(output1[i] - output2[i]) > 0.01f)
            {
                outputsDiffer = true;
                break;
            }
        }

        Assert.IsTrue(outputsDiffer, $"A két predikció ugyanazt az outputot adta! Output1: [{string.Join(", ", output1)}], Output2: [{string.Join(", ", output2)}]");

        Assert.That(output1[0], Is.InRange(0.7f, 2.5f), "Health multiplier kívül van a tartományon (output1)!");
        Assert.That(output1[1], Is.InRange(0.8f, 1.6f), "Speed multiplier kívül van a tartományon (output1)!");
        Assert.That(output2[0], Is.InRange(0.7f, 2.5f), "Health multiplier kívül van a tartományon (output2)!");
        Assert.That(output2[1], Is.InRange(0.8f, 1.6f), "Speed multiplier kívül van a tartományon (output2)!");
    }
}
