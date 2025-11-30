using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmSwing : MonoBehaviour
{
    public ArmFollowWithDelay rotationManager;

    public Transform leftArm; // Transform of the left arm 左手臂的Transform
    public Transform rightArm; // Transform of the right arm 右手臂的Transform
    public float swingAmount = 15f; // amplitude of swing 摆动的幅度
    public float swingSpeed = 2f; // Speed of swing 摆动的速度

    private CharacterController characterController;
    private float currentSpeed;

    // Start is called before the first frame update
    void Start()
    {
        characterController = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // Gets the character's movement speed 获取角色的移动速度
        currentSpeed = characterController.velocity.magnitude;

        float moveInput = Input.GetAxis("Vertical");

        // Calculate the angle of arm swing 计算手臂摆动的角度
        float swingAngle = Mathf.Sin(Time.time * swingSpeed) * swingAmount * moveInput;

        // Updated arm rotation 更新手臂的旋转
        if (leftArm != null)
        {
            // leftArm.localRotation = Quaternion.Euler(swingAngle, 0, 0);

            rotationManager.AddLeftRotation(Quaternion.Euler(swingAngle, 0, 0));
        }
        if (rightArm != null)
        {
            // rightArm.localRotation = Quaternion.Euler(-swingAngle, 0, 0);

            rotationManager.AddRightRotation(Quaternion.Euler(-swingAngle, 0, 0));
        }
    }
}
