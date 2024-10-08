using TMPro; // Hozzáadod a TextMeshPro használatához
using System.Collections;
using UnityEngine.UI;
using UnityEngine;

public class MoneyUI : MonoBehaviour
{
    // A moneyText változónak TextMeshProUGUI típust kell adni
    public TextMeshProUGUI moneyText;

    void Update()
    {
        // Frissíti a szöveget a PlayerStats.Money értékével
        moneyText.text = PlayerStats.Money.ToString();
    }
}
