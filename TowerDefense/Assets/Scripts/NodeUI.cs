using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NodeUI : MonoBehaviour
{
    public GameObject ui;
    public TMP_Text upgradeCostText; 
    public Button upgradeButton;
    public TMP_Text sellAmountText;  
    public Button sellButton;        

    private Node target;

    
    private string GetIcon()
    {
        return "<sprite=0>"; 
    }

    public void SetTarget(Node _target)
    {
        target = _target;

        
        if (target.upgradeLevel == 0)
        {
            upgradeCostText.text = "Upgrade\n$" + target.turretBlueprint.upgradeCost.ToString();
            upgradeButton.interactable = true;
        }
        else if (target.upgradeLevel == 1) 
        {
            upgradeCostText.text = "Upgrade\n$" + target.turretBlueprint.upgradeCost2.ToString(); 
            upgradeButton.interactable = true;
        }
        else if (target.upgradeLevel == 2) 
        {
            upgradeCostText.text = "MAX"; 
            upgradeButton.interactable = false; 
        }

        
        sellAmountText.text = "Sell \n$" + target.turretBlueprint.GetSellAmount(target.upgradeLevel).ToString();

        
        ui.SetActive(true);
    }

    public void Hide()
    {
        ui.SetActive(false);
    }

    public void Upgrade()
    {
        target.UpgradeTurret();
        BuildManager.instance.DeselectNode();
    }

    public void Sell()
    {
        target.SellTurret();
        BuildManager.instance.DeselectNode();
    }
}
