using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Damage : MonoBehaviour
{
    
    [Header("Use for Ball")]
    public int damageAmount = 1; // damage value 伤害数值
    public GameObject collisionEffect; // Ball collision effects 球碰撞的特效
    public AudioClip collisionSound;

    [Header("Use for Bomb")]
    public int explosionDamage = 1; // Damage to enemies 对敌人的伤害
    public float explosionRadius = 5f; // Explosive range 爆炸范围
    public GameObject explosionEffect; // Explosive particle effects 爆炸粒子效果预制件
    public bool canExplosion = false;
    public float explosionForce = 700f; // impact of explosion 爆炸的冲击力
    public float explosionUpwardModifier = 1f; // Upward deflection of impact 冲击力的向上偏移
    public GameObject explosionSound;

    [Header("Use for Item")]
    public GameObject brokenEffect; // Broken particles effect 破损粒子效果预制件
    public GameObject brokenSound;

    [Header("Use for Enemy")]
    public GameObject dieEffect;

    private AudioSource audioSource;

    private void OnCollisionEnter(Collision collision)
    {
        if (gameObject.CompareTag("Ball") || gameObject.CompareTag("Key"))
        {
            // Detect if the collision object has a Health script 检测碰撞对象是否有 Health 脚本
            Health health = collision.gameObject.GetComponent<Health>();
            if (health != null)
            {
                // Damage to colliding objects 对碰撞对象造成伤害
                health.TakeDamage(damageAmount);
            }

            // Getting collision point 获取碰撞点信息
            ContactPoint contact = collision.contacts[0];
            Vector3 collisionPoint = contact.point;

            // Generate effects at collision points 在碰撞点生成特效
            GameObject effectCollision = Instantiate(collisionEffect, collisionPoint, Quaternion.identity);

            Destroy(effectCollision, 1f);

            audioSource.PlayOneShot(collisionSound);
        }
        else if (gameObject.CompareTag("Item") && canExplosion == true)
        {

            // Detect if the collision object has a Health script 检测碰撞对象是否有 Health 脚本
            Health health = collision.gameObject.GetComponent<Health>();
            if (health != null)
            {
                // Damage to colliding objects 对碰撞对象造成伤害
                health.TakeDamage(damageAmount);
            }
            if (brokenEffect != null)
            {
                GameObject effectBroken = Instantiate(brokenEffect, transform.position, Quaternion.identity);
                Destroy(effectBroken, 1f);
            }
            if (brokenSound != null)
            {
                GameObject soundBroken = Instantiate(brokenSound, transform.position, Quaternion.identity);
                Destroy(soundBroken, 2f);
            }

            Destroy(gameObject);
        }
        else if (gameObject.CompareTag("Bomb") && canExplosion == true)
        {

            // Play explosion effect 播放爆炸效果
            if (explosionEffect != null)
            {
                GameObject effectExplosion = Instantiate(explosionEffect, transform.position, Quaternion.identity);
                Destroy(effectExplosion, 1f);
            }

            if (explosionSound != null)
            {
                GameObject soundExplosion = Instantiate(explosionSound, transform.position, Quaternion.identity);
                Destroy(soundExplosion, 2f);
            }

            // Detects enemies within explosion range 检测爆炸范围内的敌人
            Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);

            foreach (Collider nearbyObject in colliders)
            {
                Health health = nearbyObject.GetComponent<Health>();
                if (health != null)
                {
                    health.TakeDamage(explosionDamage);
                }

                Rigidbody rb = nearbyObject.GetComponent<Rigidbody>();

                if (rb != null)
                {
                    // Impact on nearby objects 对附近的物体施加冲击力
                    rb.AddExplosionForce(explosionForce, transform.position, explosionRadius, explosionUpwardModifier);
                }
            }

            Destroy(gameObject);
        }
        else if (gameObject.CompareTag("Enemy") && canExplosion == true)
        {
            // Detect if the collision object has a Health script 检测碰撞对象是否有 Health 脚本
            Health health = collision.gameObject.GetComponent<Health>();
            Health myHealth = GetComponent<Health>();
            if (health != null && health != myHealth)
            {
                // Damage to colliding objects 对碰撞对象造成伤害
                health.TakeDamage(damageAmount);
            }
            
            myHealth.Die();
        }

    }

    void OnDrawGizmosSelected()
    {
        // Display of explosion range 在编辑器中显示爆炸范围
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
