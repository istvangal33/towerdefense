import onnxruntime as ort
import numpy as np
import pandas as pd
import joblib

# Betöltjük az ONNX modellt
onnx_model_path = "tower_defense_model.onnx"
session = ort.InferenceSession(onnx_model_path)

# Betöltjük a skálázót a bemeneti adatok normalizálásához
scaler = joblib.load("scaler.pkl")


test_data = np.array([
    [1.0, 6.0, 15, 60.0],
    [1.0, 2.0, 15, 34.8],
    [1.0, 5.0, 15, 52.0],        
    [1.0, 2.0, 12, 35.0],
    [1.0, 3.0, 12, 60.0],
    [1.0, 4.0, 6, 60.0],
    [1.0, 10.0, 6, 100.0],
    [1.0, 5.0, 10, 80.0],
    [2.0, 3.0, 8, 75.0],
    [3.0, 5.0, 2, 100.0],
    [3.0, 6.0, 5, 100.0],
    [3.0, 6.0, 8, 70.0],
    [3.0, 4.0, 6, 70.0],
    [4.0, 10.0, 13, 100.0]
], dtype=np.float32)

# Normalizáljuk az adatokat
test_data_scaled = scaler.transform(test_data)

# ONNX modell bemenetének előkészítése
input_name = session.get_inputs()[0].name
output_name = session.get_outputs()[0].name

# Modell futtatása (előrejelzés)
predictions = session.run([output_name], {input_name: test_data_scaled})[0]

# Eredmények kiírása
for i, pred in enumerate(predictions):
    hp_multiplier = np.clip(pred[0], 0.8, 2.3)
    speed_multiplier = np.clip(pred[1], 0.8, 1.4)
    print(f"Adat {i+1}: HP Multiplier = {hp_multiplier:.2f}, Speed Multiplier = {speed_multiplier:.2f}")
