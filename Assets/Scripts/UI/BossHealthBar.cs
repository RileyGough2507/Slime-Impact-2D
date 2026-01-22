using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BossHealthBar : MonoBehaviour
{
    public Slider fillBar;
    public TextMeshProUGUI bossName;

    private float maxHealth;
    private float currentHealth;


    private void Start()
    {
        fillBar= GetComponent<Slider>();
        Initialize(bossName.text, maxHealth);
        SetHealth(maxHealth);
    }
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
            fillBar.value = currentHealth / maxHealth;
    }
}
