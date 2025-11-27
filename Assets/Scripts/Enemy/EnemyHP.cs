using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.AI;

public class EnemyHP : MonoBehaviour
{
    public enum SpellType { None, Ice, Fire, Poison }
    public Slider healthBar;
    public float maxHealth = 200f;
    public float currentHealth;

    public float originalSpeed;
    private EnemyAi ai;

    [Header("Status Effects")]
    private bool isPoisoned = false;
    private bool isBurning = false;
    private bool isSlowed = false;

    [Header("Health Bar Animation")]
    public float barSmoothTime = 0.1f;
    private float barTimer = 0f;
    private float startValue;
    private float targetValue;

    public LevelClearController levelClearController;

    void Start()
    {
        ai = GetComponent<EnemyAi>();
        originalSpeed = ai.nav.speed;

        currentHealth = maxHealth;

        if (healthBar != null)
        {
            healthBar.maxValue = maxHealth;
            healthBar.minValue = 0;
            healthBar.value = currentHealth;
        }

        startValue = currentHealth;
        targetValue = currentHealth;
    }

    void Update()
    {
        if (barTimer < barSmoothTime)
        {
            barTimer += Time.deltaTime;
            float n = barTimer / barSmoothTime;
            n = 1f - Mathf.Pow(1f - n, 3f);
            healthBar.value = Mathf.Lerp(startValue, targetValue, n);
        }
    }

    public void ApplySpellEffect(SpellType effect)
    {
        if (effect == SpellType.Ice)
        {
            if (!isSlowed)
                StartCoroutine(SlowRoutine(0.5f, 5f, 50f));
        }
        else if (effect == SpellType.Fire)
        {
            if (!isBurning)
                StartCoroutine(DamageOverTimeRoutine(20f, 3, 2f, () => isBurning = false, () => isBurning = true));
        }
        else if (effect == SpellType.Poison)
        {
            if (!isPoisoned)
                StartCoroutine(DamageOverTimeRoutine(5f, 15, 1f, () => isPoisoned = false, () => isPoisoned = true));
        }
    }

    IEnumerator DamageOverTimeRoutine(float dmgPerTick, int ticks, float tickRate, System.Action endStatus, System.Action startStatus)
    {
        startStatus.Invoke();

        for (int i = 0; i < ticks; i++)
        {
            TakeDamage(dmgPerTick);
            yield return new WaitForSeconds(tickRate);
        }

        endStatus.Invoke();
    }

    IEnumerator SlowRoutine(float slowPercent, float duration, float slowDamage)
    {
        isSlowed = true;
        ai.nav.speed = originalSpeed * (1f - slowPercent);
        TakeDamage(slowDamage);

        yield return new WaitForSeconds(duration);

        ai.nav.speed = originalSpeed;
        isSlowed = false;
    }

    public void TakeDamage(float damageAmount)
    {
        currentHealth -= damageAmount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        startValue = healthBar.value;
        targetValue = currentHealth;
        barTimer = 0f;

        if (GetComponentInChildren<ParticleSystem>() != null)
        {
            GetComponentInChildren<ParticleSystem>().Play();
        }

        CheckAlive();
    }

    void CheckAlive()
    {
        if (currentHealth <= 0)
        {
            if (GetComponent<Animator>() != null)
            {
                GetComponent<Animator>().SetTrigger("Die");
            }

            if (ai != null)
            {
                ai.enabled = false;
            }

            GetComponent<NavMeshAgent>().enabled = false;
            GetComponent<Collider>().enabled = false;

            if (levelClearController != null)
            {
                levelClearController.EnemyKilled();
            }

            Destroy(gameObject, 5f);
        }
    }
}
