using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : MonoBehaviour
{
    [Header("Health Settings")]
    public float maxHealth = 100f;
    public float currentHealth;

    [Header("Stamina Settings")]
    public float maxStamina = 100f;
    public float currentStamina;
    public float staminaRegenRate = 10f; // za sekundu
    public float staminaRegenDelay = 2f; // čakanie pred regeneráciou

    [Header("UI References")]
    public Slider healthBar;
    public Slider staminaBar;

    private float staminaRegenTimer = 0f;
    private bool isRegeneratingStamina = true;

    void Start()
    {
        currentHealth = maxHealth;
        currentStamina = maxStamina;
        UpdateUI();
    }

    void Update()
    {
        // Regenerácia staminy
        if (isRegeneratingStamina)
        {
            staminaRegenTimer += Time.deltaTime;

            if (staminaRegenTimer >= staminaRegenDelay)
            {
                RegenerateStamina(staminaRegenRate * Time.deltaTime);
            }
        }

        UpdateUI();
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(float amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
    }

    public bool UseStamina(float amount)
    {
        if (currentStamina >= amount)
        {
            currentStamina -= amount;
            staminaRegenTimer = 0f; // Reset regenerácie
            return true;
        }
        return false;
    }

    private void RegenerateStamina(float amount)
    {
        if (currentStamina < maxStamina)
        {
            currentStamina += amount;
            currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);
        }
    }

    public void StopStaminaRegen()
    {
        isRegeneratingStamina = false;
        staminaRegenTimer = 0f;
    }

    public void StartStaminaRegen()
    {
        isRegeneratingStamina = true;
    }

    private void UpdateUI()
    {
        if (healthBar != null)
        {
            healthBar.value = currentHealth / maxHealth;
        }

        if (staminaBar != null)
        {
            staminaBar.value = currentStamina / maxStamina;
        }
    }

    private void Die()
    {
        Debug.Log("Hráč zomrel!");
        // Tu pridaj logiku smrti
    }
}