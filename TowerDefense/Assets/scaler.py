import joblib

# Betöltjük a skálázót (StandardScaler)
scaler = joblib.load("scaler.pkl")

# Kiírjuk az átlagot és szórást
print("Mean values:", scaler.mean_)
print("Standard deviation values:", scaler.scale_)
