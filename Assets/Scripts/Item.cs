using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{

    public string itemName; 
    public string itemUsage; 
    public AudioClip pickupSound;

    [Header("Use for Liquid")]
    public int liquidEffect; // The amount of health restored by the liquid 药水恢复的血量
    public GameObject drinkSound;

    private AudioSource audioSource;



    public void PickUp(Transform hand)
    {
        Collider collider = GetComponent<Collider>();
        if (collider != null)
        {
            collider.enabled = false;
        }

        SlimeAttack slimeAttack = GetComponent<SlimeAttack>();
        if(slimeAttack != null)
        {
            slimeAttack.enabled = false;
        }

        EnemyAI enemyAI = GetComponent<EnemyAI>();
        if(enemyAI != null)
        {
            enemyAI.enabled = false;
        }

        UnityEngine.AI.NavMeshAgent agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        if (agent != null)
        {
            agent.enabled = false; 
        }

        // Move the item to the hand 将物品移到手上
        transform.SetParent(hand);
        transform.localPosition = Vector3.zero; 
        transform.localRotation = Quaternion.identity; 
        transform.localScale = Vector3.one;
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true; 
        }
        if (pickupSound != null)
        {
            audioSource.PlayOneShot(pickupSound);
        }

    }

    public void Drop(Vector3 direction)
    {
        // Throwing objects 投掷物品
        Collider collider = GetComponent<Collider>();
        if (collider != null)
        {
            collider.enabled = true;
        }

        transform.SetParent(null);
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false; // Restoration of physical properties 物品恢复物理属性
            rb.AddForce(direction, ForceMode.Impulse); 
        }

        Damage damage = GetComponent<Damage>();
        damage.canExplosion = true;

        if (gameObject.CompareTag("Bomb"))
        {
            
        }
    }

    public void UseLiquid(Health health)
    {
        if (gameObject.CompareTag("Liquid"))
        {
            if (drinkSound != null)
            {
                GameObject soundDrink = Instantiate(drinkSound, transform.position, Quaternion.identity);
                Destroy(soundDrink, 2f);
            }

            // Calling the Health component to restore health 调用Health组件恢复血量
            health.AddHealth(liquidEffect);
            Destroy(gameObject);
        }
    }

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
