using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmFollowWithDelay : MonoBehaviour
{

    public Transform cameraTransform; 
    public Transform playerTransform; 
    public Transform leftArmTransform;    
    public Transform rightArmTransform;    

    public float positionLerpSpeed = 5f; // Smoothing speed for position following 位置跟随的平滑速度
    public float rotationLerpSpeed = 5f; // Smoothing speed for rotation following 旋转跟随的平滑速度
    public Vector3 leftArmPositionOffset = new Vector3(-0.6f, 0.6f, 1f); // Arm position offset relative to the camera 手臂相对于摄像机的位置偏移
    public Vector3 rightArmPositionOffset = new Vector3(0.6f, 0.6f, 1f); 

    private Quaternion additiveLeftRotation = Quaternion.identity; // Rotational offset of the arm relative to the camera 手臂相对于摄像机的旋转偏移
    private Quaternion additiveRightRotation = Quaternion.identity;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddLeftRotation(Quaternion rotation)
    {
        additiveLeftRotation *= rotation; // Rotational offset 累加旋转
    }

    public void AddRightRotation(Quaternion rotation)
    {
        additiveRightRotation *= rotation; 
    }

    void FixedUpdate()
    {

        // Target position = camera position + offset 目标位置 = 摄像机位置 + 偏移
        Vector3 leftTargetPosition = cameraTransform.position + cameraTransform.TransformDirection(leftArmPositionOffset);
        Vector3 rightTargetPosition = cameraTransform.position + cameraTransform.TransformDirection(rightArmPositionOffset);



        // Smooth interpolated position 平滑插值位置
        leftArmTransform.position = Vector3.Lerp(leftArmTransform.position, leftTargetPosition, Time.deltaTime * positionLerpSpeed);
        rightArmTransform.position = Vector3.Lerp(rightArmTransform.position, rightTargetPosition, Time.deltaTime * positionLerpSpeed);

        // Target Rotation = Camera Rotation + Offset 目标旋转 = 摄像机旋转 + 偏移
        Quaternion leftTargetRotation = cameraTransform.rotation * additiveLeftRotation;
        Quaternion rightTargetRotation = cameraTransform.rotation * additiveRightRotation;

        // Smoothly interpolated rotation 平滑插值旋转
        leftArmTransform.rotation = Quaternion.Slerp(leftArmTransform.rotation, leftTargetRotation, Time.deltaTime * rotationLerpSpeed);
        rightArmTransform.rotation = Quaternion.Slerp(rightArmTransform.rotation, rightTargetRotation, Time.deltaTime * rotationLerpSpeed);

        additiveLeftRotation = Quaternion.identity;
        additiveRightRotation = Quaternion.identity;
    }
}

