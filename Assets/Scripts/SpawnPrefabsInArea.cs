using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class PrefabSpawnData
{
    public GameObject prefab; // Prefab 预制体
    public int spawnCount; // Quantity of this prefab generated 该预制体的生成数量
}

public class SpawnPrefabsInArea : MonoBehaviour
{
    public List<PrefabSpawnData> prefabSpawnDataList; // List of prefabs and their quantities 预制体及其数量数组
    public Vector2 areaSize = new Vector2(10f, 10f); // Area size (width and height) 区域大小（宽和高）
    public float minDistance = 2f; // Minimum distance between each generated item 每个生成物品之间的最小距离

    private List<Vector3> spawnedPositions = new List<Vector3>(); // List of locations of generated items 已生成物品的位置列表
    private List<GameObject> spawnedObjects = new List<GameObject>(); // List for storing generated objects 用于存储生成的对象的列表

    // Start is called before the first frame update
    void Start()
    {
        SpawnPrefabs();
        
            
    }

    // Update is called once per frame
    void Update()
    {
        CheckSpawnedObjects();
    }

    void SpawnPrefabs()
    {
        foreach (PrefabSpawnData spawnData in prefabSpawnDataList)
        {
            for (int i = 0; i < spawnData.spawnCount; i++)
            {
                Vector3 spawnPosition;
                int attempts = 0;

                // Try to find the generated position that meets the minimum distance 尝试找到符合最小距离的生成位置
                do
                {
                    spawnPosition = GetRandomPosition();
                    attempts++;
                }
                while (!IsPositionValid(spawnPosition) && attempts < 100); 

                if (attempts < 100) // If a valid location is found 如果找到有效位置
                {
                    // Generate a random rotation angle for the Y-axis 随机生成一个 Y 轴的旋转角度
                    Quaternion spawnRotation = GetRandomRotation();

                    // Generating prefabs 生成预制体
                    GameObject newObject = Instantiate(spawnData.prefab, spawnPosition, spawnRotation);

                    // Record the location of the generation 记录生成的位置
                    spawnedPositions.Add(spawnPosition);

                    // Storage of generated prefabs 存储生成的预制体
                    spawnedObjects.Add(newObject);
                }
                
            }
        }
    }

    Vector3 GetRandomPosition()
    {
        // Generate a random point in the area 在区域内随机生成一个点
        float x = Random.Range(-areaSize.x / 2, areaSize.x / 2);
        float z = Random.Range(-areaSize.y / 2, areaSize.y / 2);

        // Returns the generated position, assuming the area is in the plane (y-axis is 0) 返回生成位置，假设区域在平面上（y轴为0）
        return new Vector3(x, 0f, z) + transform.position;
    }

    Quaternion GetRandomRotation()
    {
        // Generate a random Y-axis angle 随机生成一个 Y 轴角度
        float randomY = Random.Range(0f, 360f);
        return Quaternion.Euler(0f, randomY, 0f);
    }

    bool IsPositionValid(Vector3 position)
    {
        // Check that it is far enough away from the generated items 检查是否与已生成的物品距离足够远
        foreach (Vector3 spawnedPosition in spawnedPositions)
        {
            if (Vector3.Distance(position, spawnedPosition) < minDistance)
            {
                return false;
            }
        }
        return true;
    }

    void CheckSpawnedObjects()
    {
        

            foreach (GameObject obj in spawnedObjects)
            {
                if (obj != null)
                {
                    return;
                }
            }



        Destroy(gameObject);
    }

    void OnDrawGizmosSelected()
    {
        
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, new Vector3(areaSize.x, 0.1f, areaSize.y));
    }
}
