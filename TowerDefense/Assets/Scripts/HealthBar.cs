using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Image fillImage;  // Az eg�szs�gs�v kit�lt�se (el�tte be kell h�zni az inspectorban)

    public void SetHealth(float currentHealth, float maxHealth)
    {
        // Az eg�szs�g ar�ny�nak kisz�m�t�sa, �s az Image kit�lt�se ennek megfelel�en
        fillImage.fillAmount = currentHealth / maxHealth;
    }
}
