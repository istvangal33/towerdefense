using System;
using System.Linq;
using UnityEngine;
using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
using System.Collections.Generic;

public class ONNXModelLoader : MonoBehaviour
{
    private InferenceSession session;

    [SerializeField] private float[] mean;
    [SerializeField] private float[] std;  

    void Start()
    {
        
        string modelPath = Application.streamingAssetsPath + "/tower_defense_model.onnx";
        session = new InferenceSession(modelPath);

        
        float[] inputData = new float[] { 1.0f, 1.0f, 15.0f, 0.0f };

        if (inputData == null || inputData.Length == 0)
        {
            Debug.LogError(" Input data is empty or null!");
            return;
        }

        int[] inputDimensions = new int[] { 1, inputData.Length };
        Debug.Log($" Input Dimensions: [{inputDimensions[0]}, {inputDimensions[1]}]");

        
        var inputTensor = new DenseTensor<float>(inputData, inputDimensions);
        var input = new List<NamedOnnxValue>
        {
            NamedOnnxValue.CreateFromTensor("dense_input", inputTensor)
        };

        
        using (var results = session.Run(input))
        {
            var output = results.First(v => v.Name == "dense_3").AsTensor<float>().ToArray();
            Debug.Log($" ONNX Output: {string.Join(", ", output)}");
        }
    }

    public float[] NormalizeInput(float[] input)
    {
        if (mean == null || std == null || mean.Length != input.Length || std.Length != input.Length)
        {
            Debug.LogError("Mean vagy STD nincs inicializalva, vagy a meretuk nem megfelelo!");
            return input;
        }

        float[] normalizedInput = new float[input.Length];
        for (int i = 0; i < input.Length; i++)
        {
            normalizedInput[i] = (input[i] - mean[i]) / std[i];
        }

        return normalizedInput;
    }

    public float[] Predict(float[] inputData)
    {
        if (inputData == null || inputData.Length == 0)
        {
            Debug.LogError(" Predict input data is empty or null!");
            return new float[0];
        }

        // Normalizálás
        float[] normalizedInput = NormalizeInput(inputData);

        int[] inputDimensions = new int[] { 1, normalizedInput.Length };
        var inputTensor = new DenseTensor<float>(normalizedInput, inputDimensions);
        var input = new List<NamedOnnxValue>
        {
            NamedOnnxValue.CreateFromTensor("dense_input", inputTensor)
        };

        using (var results = session.Run(input))
        {
            return results.First(v => v.Name == "dense_3").AsTensor<float>().ToArray();
        }
    }

    void OnDestroy()
    {
        session?.Dispose();
    }
}
