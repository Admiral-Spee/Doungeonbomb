using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeAttack : MonoBehaviour
{

    public float attackCooldown = 2f; // Attack cooldown time 攻击冷却时间
    public int damageAmount = 1; // Attack damage 攻击伤害
    private float lastAttackTime; // Time of last attack 上次攻击的时间
    private bool isStunned = false; // Whether or not the enemy is stunned 敌人是否被定身
    private float stunTime = 0f; // Enemy stun remaining time 敌人定身剩余时间

    private Transform target; // Player Transform

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // If the enemy is stunned, update the duration of the stun 如果敌人被定身，更新定身时间
        if (isStunned)
        {
            stunTime -= Time.deltaTime;
            if (stunTime <= 0f)
            {
                isStunned = false;
            }
        }
    }

    public void Attack(Transform player)
    {
        target = player;

        if (Time.time > lastAttackTime + attackCooldown && !isStunned)
        {
            if (player != null)
            {

                // Do damage to the player 对玩家造成伤害
                Health health = player.GetComponent<Health>();
                if (health != null)
                {
                    health.TakeDamage(damageAmount);

                }

                // Play attack displacement effect 播放攻击位移效果
                StartCoroutine(PerformAttackMovement());

                // Play attack sound effect 播放攻击音效
                AudioSource audioSource = GetComponent<AudioSource>();
                if (audioSource != null)
                {
                    audioSource.Play();
                }

            }



            // Stuns the enemy for 2 seconds 给敌人造成定身效果，定身 2 秒
            StunEnemy(2f);

            // Updating the time of the last attack 更新上次攻击时间
            lastAttackTime = Time.time;
        }
    }

    void StunEnemy(float duration)
    {
        if (!isStunned)
        {
            isStunned = true;
            stunTime = duration;
          
        }
    }

    IEnumerator PerformAttackMovement()
    {
        // Initial position before attack 攻击前的初始位置
        Vector3 startPosition = transform.position;

        // Targeting the player's location 以玩家为目标位置
        Vector3 attackPosition = target.position - (target.position - startPosition).normalized * 1f;

        float attackDuration = 0.1f; // Duration of the attack action 攻击动作的持续时间
        float elapsedTime = 0f;

        // Smooth movement to target position 平滑移动到目标位置
        while (elapsedTime < attackDuration)
        {
            transform.position = Vector3.Lerp(startPosition, attackPosition, elapsedTime / attackDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Return to initial position 回到初始位置
        elapsedTime = 0f;
        while (elapsedTime < attackDuration)
        {
            transform.position = Vector3.Lerp(attackPosition, startPosition, elapsedTime / attackDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

       
    }


}
