import onnxruntime as ort
import numpy as np
import joblib

# Betöltjük a modellt és a scaler-t
session = ort.InferenceSession("tower_defense_model.onnx")
scaler = joblib.load("scaler.pkl")

# Teszt bemenetek
test_data = np.array([
    [1.0, 1.0, 15, 0],
    [1.0, 2.0, 15, 30],
    [1.0, 2.0, 12, 30],
    [1.0, 3.0, 12, 45],
    [1.0, 3.0, 13, 55],
    [1.0, 6.0, 13, 70],
    [1.0, 8.0, 13, 85],
    [2.0, 6.0, 13, 70],
    [3.0, 6.0, 13, 70],
    [1.0, 2.0, 6, 26],
    [1.0, 2.0, 10, 30],
    [1.0, 4.0, 8, 30],
    [1.0, 7.0, 12, 100],
    [1.0, 8.0, 12, 100],
    [1.0, 9.0, 12, 100],
    [1.0, 10.0, 12, 100],
    [1.0, 5.0, 15, 52],
    [1.0, 5.0, 15, 60],
    [1.0, 5.0, 15, 75],
    [1.0, 2.0, 12, 35],
    [1.0, 3.0, 12, 60],
    [1.0, 4.0, 6, 60],
    [1.0, 10.0, 6, 100],
    [1.0, 5.0, 10, 80],
    [2.0, 3.0, 8, 75],
    [3.0, 5.0, 2, 100],
    [3.0, 6.0, 5, 100],
    [3.0, 6.0, 8, 70],
    [3.0, 4.0, 6, 70],
    [4.0, 2.0, 12, 30],
    [4.0, 10.0, 13, 100],
    [4.0, 10.0, 15, 100]
], dtype=np.float32)

# Normalizálás
normalized_data = scaler.transform(test_data).astype(np.float32)

# ONNX modell futtatása Pythonban
input_name = session.get_inputs()[0].name
output_name = session.get_outputs()[0].name
results = session.run([output_name], {input_name: normalized_data})[0]

# Kiírás
print("Python ONNX Outputs:")
for i, result in enumerate(results):
    print(f"Input {i+1}: {test_data[i]} -> Output: {result}")

print("\nPython ONNX Runtime version:", ort.__version__)
