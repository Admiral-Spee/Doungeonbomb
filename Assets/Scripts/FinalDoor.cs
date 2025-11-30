using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinalDoor : MonoBehaviour
{
    public Vector3 openPositionOffset; // Offset of door opening 大门打开的偏移量
    public float openSpeed = 2f; // The speed of the door opening 大门打开的速度

    public AudioClip openSound;

    private Vector3 closedPosition;
    private Vector3 openPosition;
    private bool openDoor = false;

    private AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        closedPosition = transform.position;
        openPosition = closedPosition + openPositionOffset;

        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (openDoor)
        {
            
            transform.position = Vector3.Lerp(transform.position, openPosition, Time.deltaTime * openSpeed);
        }
    }

    public void OpenDoor()
    {
        openDoor = true;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Key"))
        {
            openDoor = true;
            audioSource.PlayOneShot(openSound);
            Destroy(collision.gameObject);
        }
    }
}
