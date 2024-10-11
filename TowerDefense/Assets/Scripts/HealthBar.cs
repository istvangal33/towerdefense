using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Image fillImage;  // Az egészségsáv kitöltése (elõtte be kell húzni az inspectorban)

    public void SetHealth(float currentHealth, float maxHealth)
    {
        // Az egészség arányának kiszámítása, és az Image kitöltése ennek megfelelõen
        fillImage.fillAmount = currentHealth / maxHealth;
    }
}
