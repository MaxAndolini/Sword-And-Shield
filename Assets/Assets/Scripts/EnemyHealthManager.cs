using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthManager : MonoBehaviour
{
    public float maxHealth = 100f;
    public float currentHealth;
    public GameObject[] blood;
    public GameObject health;
    private readonly int activeBlood = 0;

    private EnemyAI enemyControll;
    private Slider healthSlider;

    private void Awake()
    {
        enemyControll = GetComponent<EnemyAI>();
        healthSlider = health.GetComponent<Slider>();
    }

    private void Start()
    {
        currentHealth = maxHealth;
        healthSlider.maxValue = maxHealth;
        healthSlider.value = currentHealth;
    }

    private void Update()
    {
        if (currentHealth >= maxHealth)
        {
            currentHealth = maxHealth;
            healthSlider.value = currentHealth;
        }
    }

    public IEnumerator HurtEnemy(float damageToGive)
    {
        yield return new WaitForSeconds(0.5f);
        currentHealth -= damageToGive;
        healthSlider.value = currentHealth;

        var pos = Random.Range(0, 1);
        var hurtAudio = Audio.Audios.EnemyHurt1;

        if (pos == 1) hurtAudio = Audio.Audios.EnemyHurt2;

        FindObjectOfType<Audio>().PlayEnemy(gameObject, hurtAudio);

        if (activeBlood < 3)
        {
            pos = Random.Range(0, blood.Length);
            blood[pos].SetActive(true);
        }

        if (currentHealth <= 0 && !enemyControll.isDead)
        {
            FindObjectOfType<Audio>().PlayEnemy(gameObject, Audio.Audios.EnemyDie);

            foreach (var t in blood) t.SetActive(false);
            health.SetActive(false);
            enemyControll.Death();
        }
    }

    public void SetMaxHealth()
    {
        currentHealth = maxHealth;
        healthSlider.value = currentHealth;
    }
}