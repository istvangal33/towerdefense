using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Image fillImage; 

    public void SetHealth(float currentHealth, float maxHealth)
    {
        
        fillImage.fillAmount = currentHealth / maxHealth;
    }
}
