using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BossHealthBar : MonoBehaviour
{
    public Image fillBar;
    public TextMeshProUGUI bossName;

    private float maxHealth;
    private float currentHealth;

    public void Initialize(string name, float maxHP)
    {
        bossName.text = name;
        maxHealth = maxHP;
        currentHealth = maxHP;
        UpdateBar();
    }

    public void SetHealth(float hp)
    {
        currentHealth = Mathf.Clamp(hp, 0, maxHealth);
        UpdateBar();
    }

    void UpdateBar()
    {
        if (fillBar != null)
            fillBar.fillAmount = currentHealth / maxHealth;
    }
}
