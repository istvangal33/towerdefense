using TMPro; // Hozz�adod a TextMeshPro haszn�lat�hoz
using System.Collections;
using UnityEngine.UI;
using UnityEngine;

public class MoneyUI : MonoBehaviour
{
    // A moneyText v�ltoz�nak TextMeshProUGUI t�pust kell adni
    public TextMeshProUGUI moneyText;

    void Update()
    {
        // Friss�ti a sz�veget a PlayerStats.Money �rt�k�vel
        moneyText.text = PlayerStats.Money.ToString();
    }
}
