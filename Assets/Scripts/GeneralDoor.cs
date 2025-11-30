using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneralDoor : MonoBehaviour
{

    public Health[] enemies; // List of enemies 敌人数组

    public Vector3 openPositionOffset; // Offset of door opening 大门打开的偏移量
    public float openSpeed = 2f; // The speed of the door opening 大门打开的速度

    public AudioClip openSound;

    private Vector3 closedPosition;
    private Vector3 openPosition;
    private bool allEnemiesDefeated = false;
    private bool havePlaySound = false;
    private AudioSource audioSource;



    // Start is called before the first frame update
    void Start()
    {
        closedPosition = transform.position;
        openPosition = closedPosition + openPositionOffset;

        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        // Check the list for surviving enemies 检查数组中是否还有存活的敌人
        allEnemiesDefeated = true;
        foreach (Health enemy in enemies)
        {
            if (enemy != null)
            {
                allEnemiesDefeated = false;
                break;
            }
        }


    }

    private void FixedUpdate()
    {
        

        // If all enemies are dead, open the door 如果所有敌人都死亡，打开大门
        if (allEnemiesDefeated)
        {
            
            transform.position = Vector3.Lerp(transform.position, openPosition, Time.deltaTime * openSpeed);

            if (!havePlaySound)
            {
                audioSource.PlayOneShot(openSound);
                havePlaySound = true;
            }
        }
    }


}
