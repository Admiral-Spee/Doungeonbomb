using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    private Transform player; // Player Transform
    public float detectionRadius = 10f; // Range of enemies detecting the player 敌人探测玩家的范围
    public float stopDistance = 2f; // The distance the enemy stops around the player 敌人停留在玩家周围的距离
    public float moveSpeed = 3f; // Enemy movement speed 敌人的移动速度
    public float dropForce = 10f;

    public float itemDetectionRadius = 5f; // Range of enemy detection items 检测物品的范围
    public LayerMask itemLayer; // Layers of detected items 被检测物品的层级
    public float pickUpDistance = 1f; // Distance of enemy pickup items 敌人拾取物品的距离
    public Transform hand;

    private UnityEngine.AI.NavMeshAgent agent; // NavMeshAgent of enemy
    private bool isPlayerInRange = false; // Whether the player is within enemy detection range 玩家是否在敌人探测范围内
    private bool haveBallInHand = true;

    // Start is called before the first frame update
    void Start()
    {
        // Automatically find player objects in the scene 自动寻找场景中的玩家对象
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.transform;
        }

        // Get the NavMeshAgent component on the enemy 获取敌人身上的 NavMeshAgent 组件
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        if (agent != null)
        {
            agent.speed = moveSpeed; // Setting the movement speed 设置移动速度
        }

        Item item = hand.GetComponentInChildren<Item>();
        if (item != null)
        {
            
            item.PickUp(hand); 
            haveBallInHand = true;
        }

    }

    // Update is called once per frame
    void Update()
    {
        // Detects if the player is in the detection range 检测玩家是否进入探测范围
        if (Vector3.Distance(transform.position, player.position) <= detectionRadius)
        {
            isPlayerInRange = true;
        }
        else
        {
            isPlayerInRange = false;
        }
    }

    void FixedUpdate()
    {
        
        if (haveBallInHand == false) // If no ball is in hand 如果手中没有球
        {
            // Detect the nearest item and approach to pick it up 检测最近的物品并靠近拾取
            DetectClosestItem();
        }
        else if (isPlayerInRange == true && haveBallInHand == true) 
        {
            // Make sure the enemy is facing the player 确保敌人面向玩家
            FacePlayer();
            // If the player is in range, the enemy moves towards the player 如果玩家在范围内，敌人朝玩家移动
            MoveTowardsPlayer();

        }
        else if (isPlayerInRange == false && haveBallInHand == true)
        {
            // Player is out of range, enemy stops 玩家不在范围内，敌人停止
            StopMovement();
        }
    }

    void MoveTowardsPlayer()
    {
        // Calculate the distance between the player and the enemy 计算玩家和敌人之间的距离
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // If the distance is less than the target stopping distance, the enemy stops 如果距离小于目标停止距离，则敌人停下
        if (distanceToPlayer > stopDistance)
        {
            agent.SetDestination(player.position);
        }
        else
        {
            agent.ResetPath();

            // launch an attack 发动攻击
            SlimeAttack slimeAttack = GetComponent<SlimeAttack>();
            if (slimeAttack != null)
            {
                slimeAttack.Attack(player);

            }
            else
            {
                DropItem();
                haveBallInHand = false;
            }
        }
    }

    void StopMovement()
    {
        agent.ResetPath();
    }

    void DetectClosestItem()
    {
        Collider[] items = Physics.OverlapSphere(transform.position, itemDetectionRadius, itemLayer);

        if (items.Length > 0)
        {
            Transform closestItem = null;
            float closestDistance = Mathf.Infinity;

            foreach (Collider item in items)
            {
                float distance = Vector3.Distance(transform.position, item.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestItem = item.transform;
                }
            }

            if (closestItem != null)
            {
                MoveTowardsItem(closestItem);
            }
        }
    }

    void MoveTowardsItem(Transform item)
    {

        float distanceToPlayer = Vector3.Distance(transform.position, item.position);
        // If the distance is less than the pickup distance, the enemy picks up the item 如果距离小于拾取距离，则敌人拾取物品
        if (distanceToPlayer > pickUpDistance)
        {
            agent.SetDestination(item.position);
        }
        else
        {
            Item item1 = item.GetComponent<Item>();
            item1.PickUp(hand);
            Damage damage = item.GetComponent<Damage>();
            damage.canExplosion = true;
            haveBallInHand = true;
        }
    }

    void DropItem()
    {
        Item item = hand.GetComponentInChildren<Item>();
        if(item != null)
        {
            Vector3 throwDirection = gameObject.transform.forward; // The direction in which the item is thrown 物品投掷的方向
            item.Drop(throwDirection * dropForce); // Discard items and set the strength of the throw 丢弃物品，设置丢弃的力度

        }
    }

    void FacePlayer()
    {

        Vector3 direction = (player.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
    }

    

    void OnDrawGizmosSelected()
    {
        // Display item detection range 显示物品检测范围
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, itemDetectionRadius);

        // Display player detection range 显示玩家检测范围
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }

}

