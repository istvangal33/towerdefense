using TMPro; 
using System.Collections;
using UnityEngine.UI;
using UnityEngine;

public class MoneyUI : MonoBehaviour
{
    
    public TextMeshProUGUI moneyText;

    void Update()
    {
        
        moneyText.text = PlayerStats.Money.ToString();
    }
}
