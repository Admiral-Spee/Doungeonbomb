using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    public int maxHealth = 1;
    public bool isAlive = true;
    public int currentHealth;

    private AudioSource audioSource;

    [Header("Use for Enemy")]
    public float disappearTime = 2f; // Fading time 逐渐消失的时间
    public AudioClip damageSound;
    public GameObject damageEffect;
    public GameObject dieSound;
    public GameObject dieEffect;

    private Collider enemyCollider; 

    [Header("Use for Player")]
    
    public Slider healthSlider;
    public SceneManagment sceneManagment;
    public AudioClip playerDamageSound;
    
    public GameObject addEffect;

    [Header("Use for FinalSpot")]
    public AudioClip finalSpotSound;


    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth; 
        audioSource = GetComponent<AudioSource>();

        enemyCollider = GetComponent<Collider>();

        // 自动寻找场景中的SceneManagment对象
        GameObject sceneManagmentObject = GameObject.FindGameObjectWithTag("SceneManagment");
        if (sceneManagmentObject != null)
        {
            sceneManagment = sceneManagmentObject.GetComponent<SceneManagment>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (gameObject.CompareTag("Player"))
        {
            // Update the value of the health slider 更新血量条的值
            healthSlider.value = (float) currentHealth / maxHealth;
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log(gameObject.name + " took " + damage + " damage!");
        if (currentHealth <= 0)
        {
            isAlive = false;
            Die();
        }

        // Setting up particle effects and sound effects for injuries 设置受伤的粒子特效和音效
        else if (currentHealth > 0 && damageEffect != null) 
        { 
            
            GameObject effectDamage = Instantiate(damageEffect, transform.position, Quaternion.identity);
            Destroy(effectDamage, 1f);

            if (gameObject.CompareTag("Player"))
            {
                audioSource.PlayOneShot(playerDamageSound);
            }
            else if (gameObject.CompareTag("Enemy"))
            {
                audioSource.PlayOneShot(damageSound);
            }
        }          
                
    }

    public void AddHealth(int addition)
    {
        // Restore Health 恢复血量
        currentHealth += addition;
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }

        // Particle effect when restoring health 恢复血量时的粒子效果
        GameObject effectAdd = Instantiate(addEffect, transform.position, Quaternion.identity);
        effectAdd.transform.SetParent(transform);
        Destroy(effectAdd, 1f);
    }


    public void Die()
    {
        Debug.Log(gameObject.name + " has died!");



        // Enemy deaths 敌人死亡
        if (gameObject.CompareTag("Enemy"))
        {

            sceneManagment.EnemyKilled();

            if (dieSound != null)
            {
                GameObject soundDie = Instantiate(dieSound, transform.position, Quaternion.identity);
                Destroy(soundDie, 2f);
            }

            if (dieEffect != null)
            {
                GameObject effectDie = Instantiate(dieEffect, transform.position, Quaternion.identity);
                Destroy(effectDie, 1f);
            }
            Destroy(gameObject);
        }

        // Player deaths 玩家死亡
        if (gameObject.CompareTag("Player"))
        {
            sceneManagment.PlayerDied();
        }

        // Crystals in the final room 最终地点的水晶
        if (gameObject.CompareTag("FinalSpot"))
        {
            audioSource.PlayOneShot(finalSpotSound);
            SpawnPrefabsInArea spawnPrefabs = GetComponent<SpawnPrefabsInArea>();
            spawnPrefabs.enabled = true;
        }
    }

}