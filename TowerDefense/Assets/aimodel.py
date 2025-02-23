import pandas as pd
import numpy as np
import joblib
from sklearn.model_selection import train_test_split
from sklearn.preprocessing import StandardScaler
from keras.models import Sequential
from keras.layers import Dense, Dropout
from keras.callbacks import EarlyStopping
import tensorflow as tf
import tf2onnx

# GPU használatának letiltása
try:
    tf.config.set_visible_devices([], 'GPU')
    print("GPU letiltva, csak CPU használat.")
except RuntimeError as e:
    print(e)

# CSV fájl betöltése és előkészítése
file_path = 'GameData.csv'

try:
    data = pd.read_csv(file_path, delimiter=';', header=0, encoding='utf-8')
    print("Adatok beolvasva.")

    # Típusátalakítás: minden szám numerikussá alakítása
    for col in ['CurrentLevel', 'WaveNumber', 'CurrentLives', 'CoverageByTower']:
        data[col] = pd.to_numeric(data[col], errors='coerce')

    # NaN értékek cseréje az oszlopok átlagával
    data.fillna(data.mean(), inplace=True)

    # Célváltozók számítása
    def calculate_enemy_stats(row):
        try:
            # currentlevel
            level_effect_hp = (float(row['CurrentLevel']) ** 1.3) * 0.06  # Erősebb hatás
            level_effect_speed = float(row['CurrentLevel']) * 0.04  # Erősebb hatás

            # wavenum 
            wave_effect = (float(row['WaveNumber']) ** 1.2) * 0.02  # Kissé növelt hatás

            # lives
            lives_effect = np.clip((1 / (float(row['CurrentLives']) + 0.28)) * 4.0, 0, 3.0)  # Marad erős

            # coverage
            coverage_effect = (float(row['CoverageByTower']) / 100) ** 0.5 * 0.9  # Kicsit növelt hatás

        except (ValueError, TypeError) as e:
            print(f"Hiba az adatok konvertálásánál egy sorban: {row}\nHiba: {e}")
            return 1.0, 1.0

        # HP 
        hp_multiplier = np.clip(
            0.8 + level_effect_hp * 1.2 + wave_effect * 0.6 - lives_effect * 1.5 + coverage_effect * 1.4,
            0.7,  # min
            2.5   # max
        )

        # Speed
        speed_multiplier = np.clip(
            0.9 + level_effect_speed * 1.0 + wave_effect * 0.4 - lives_effect * 1.2 + coverage_effect * 1.1,
            0.8,  #min
            1.6   #mmax
        )

        return hp_multiplier, speed_multiplier




    # Bemeneti és kimeneti változók előkészítése
    X = data[['CurrentLevel', 'WaveNumber', 'CurrentLives', 'CoverageByTower']]
    y = data.apply(calculate_enemy_stats, axis=1, result_type='expand')
    y.columns = ['EnemyHPMultiplier', 'EnemySpeedMultiplier']

    # Adatok szétválasztása
    X_train, X_test, y_train, y_test = train_test_split(X, y, test_size=0.3, random_state=42)

    # Adatok normalizálása
    scaler = StandardScaler()
    X_train = scaler.fit_transform(X_train)
    X_test = scaler.transform(X_test)
    joblib.dump(scaler, 'scaler.pkl')

    # Modell építése
    model = Sequential([
        Dense(64, input_dim=4, activation='relu'),
        Dropout(0.2),  # Dropout hozzáadva a túlilleszkedés csökkentésére
        Dense(32, activation='relu'),
        Dropout(0.2),  # Dropout hozzáadva
        Dense(16, activation='relu'),
        Dense(2, activation='linear')
])

    # Modell fordítása
    model.compile(optimizer='adam', loss='mse', metrics=['mae'])

    # Korai leállítás
    early_stopping = EarlyStopping(monitor='val_loss', patience=5, restore_best_weights=True)

    # Modell tanítása
    model.fit(X_train, y_train, epochs=50, batch_size=32, validation_split=0.2, callbacks=[early_stopping])

    # Modell mentése
    model.save('tower_defense_model.h5')

    # ONNX konverzió
    onnx_model, _ = tf2onnx.convert.from_keras(model, output_path='tower_defense_model.onnx')

    print("Modell sikeresen betanítva és elmentve.")

except Exception as e:
    print(f"Hiba történt: {e}")
