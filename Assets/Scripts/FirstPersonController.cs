using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using TMPro.Examples;
using UnityEngine.UI;

public class FirstPersonController : MonoBehaviour
{

    public ArmFollowWithDelay rotationManager;

    [Header("Move and View")]
    public float moveSpeed = 5f; 
    public float lookSpeedX = 2f; 
    public float lookSpeedY = 2f; 
    public float upDownRange = 80f; 

    private float rotationX = 0f; // Current vertical rotation angle 当前垂直旋转角度
    private bool canView = true;

    private CharacterController characterController; 
    private Camera playerCamera; 

    [Header("Jump")]
    public float jumpHeight = 2f; 
    public float gravity = -9.81f; 
    public Transform groundCheck; // Transform for ground detection 用于检测地面的 Transform
    public float groundDistance = 0.4f; // Range of ground detection 地面检测的范围
    public LayerMask groundMask; // Layers of ground 表示地面的层级

    private Vector3 velocity; // Speed for handling gravity and jumping 用于处理重力和跳跃的速度
    private bool isGrounded; // Used to determine if on the ground 用于判断是否在地面上
    private bool canJump;

    [Header("Interaction")]
    private Item currentLeftItem;
    private Item currentRightItem;

    public float dropForce = 10f;

    public Transform leftHand; 
    public Transform rightHand; 

    public SceneManagment sceneManagment;

    [Header("Arm Animation")]
    public Transform leftArm; 
    public Transform rightArm; 
    public float extendDuration = 0.2f; // Duration of arm extension 手臂伸长的持续时间
    public float maxScaleMultiplier = 2f; // Maximum multiple of arm extension 手臂伸长的最大倍数
    public float retractDuration = 0.2f; // Duration of arm retraction 手臂收回的持续时间
    public float interactDistance = 5f;
    public float armExtendSpeed = 10f; // Arm extension speed 手臂伸长速度

    private Vector3 originalLeftArmScale = Vector3.one; // Initial scaling of the left arm 左手臂的初始缩放
    private Vector3 originalRightArmScale = Vector3.one; // Initial scaling of the right arm 右手臂的初始缩放

    [Header("UI")]
    public TMP_Text hintText; // Pickup hint text box 拾取提示文本框
    public Image textBack; 
    public TMP_Text leftHandHint;
    public TMP_Text rightHandHint;

    // Start is called before the first frame update
    void Start()
    {
        // Getting character controllers and cameras 获取角色控制器和相机
        characterController = GetComponent<CharacterController>();
        playerCamera = GetComponentInChildren<Camera>();

        // Reset the initial rotation of the camera to look straight ahead 重置相机的初始旋转为平视前方
        rotationX = 0f;
        playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);

        // Hide the mouse 隐藏鼠标
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Hide hint text 隐藏提示文字
        if (hintText != null)
        {
            textBack.enabled = false;
            hintText.text = "";
        }
        if (leftHandHint != null)
        {
            leftHandHint.text = "";
        }
        if (rightHandHint != null)
        {
            rightHandHint.text = "";
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // Handling player viewpoint rotation 处理玩家视角旋转
        HandleLook();

        // Handling player movement 处理玩家移动
        HandleMovement();

        // Handling player jumps 处理玩家跳跃
        HandleJump();
    }

    private void Update()
    {
        CheckForJump();

        CheckForInteractable();

        HandleItemInteraction();
    }

    void HandleLook()
    {
        if (canView)
        {
            // Horizontal rotation 水平旋转（左右转动）
            float mouseX = Input.GetAxis("Mouse X") * lookSpeedX;
            transform.Rotate(Vector3.up * mouseX);

            // vertical rotation 垂直旋转（上下视角）
            rotationX -= Input.GetAxis("Mouse Y") * lookSpeedY;
            rotationX = Mathf.Clamp(rotationX, -upDownRange, upDownRange); // Limit up/down rotation angle 限制上下旋转角度
            playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
        }
    }

    void HandleMovement()
    {
        // Getting player input 获取玩家的输入
        float moveDirectionX = Input.GetAxis("Horizontal"); 
        float moveDirectionZ = Input.GetAxis("Vertical");

        // direction of movement 移动方向
        Vector3 move = transform.right * moveDirectionX + transform.forward * moveDirectionZ;

        // Smooth character movement 使角色平滑移动
        characterController.Move(move * moveSpeed * Time.deltaTime);
    }

    void CheckForJump()
    {
        if (Input.GetButtonDown("Jump"))
        {
            canJump = true;
        }
        else
        {
            canJump = false;
        }
    }

    void HandleJump()
    {
        // Detect if on the ground 检测是否在地面上
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // Make sure the character stays on the ground 保证角色贴在地面上
        }

        if (isGrounded && canJump)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity); // Calculate initial speed based on gravity and jump height 根据重力和跳跃高度计算初始速度
        }

        // Applied gravity 应用重力
        velocity.y += gravity * Time.deltaTime;

        // Moving characters via CharacterController 通过 CharacterController 移动角色
        characterController.Move(velocity * Time.deltaTime);
    }


    private void CheckForInteractable()
    {
        // Detect if items can be picked up 检测是否可以拾取物品
        RaycastHit hit;
        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, interactDistance))
        {
            Item item = hit.collider.GetComponent<Item>();
            if (item != null)
            {

                // Displays hint text for picked up items 显示拾取物品的提示文字
                textBack.enabled = true;
                string itemName = item.itemName;
                hintText.text = $"Press [Mouse Buttons] to pick up [{itemName}]";
                return;
            }

            // Display the FinalDoor hint text 显示FinalDoor的提示文字
            FinalDoor finalDoor = hit.collider.GetComponent<FinalDoor>();
            if (finalDoor != null)
            {
                textBack.enabled = true;
                hintText.text = "Please use [Key] to open this door";
                return;
            }

        }
        // Hide hint text if no interactables are targeted 如果没有瞄准任何可交互物品，隐藏提示文字
        if (hintText != null)
        {
            textBack.enabled = false;
            hintText.text = "";
        }
    }

    void HandleItemInteraction()
    {
        if (currentLeftItem == null && Input.GetMouseButtonDown(0)) // Left button to pick up items 左键拾取物品
        {
            TryExtendArm(leftArm, originalLeftArmScale);
        }
        else if (currentRightItem == null && Input.GetMouseButtonDown(1)) // Right button to pick up items 右键拾取物品
        {
            TryExtendArm(rightArm, originalRightArmScale);
        }
        else if (currentLeftItem != null && Input.GetMouseButtonDown(0)) // Left button to use items 左键使用物品
        {
            if (currentLeftItem.CompareTag("Liquid"))
            {
                UseLiquid(leftHand); 
            }
            else
            {
                DropItem(leftHand); 
            }
        }
        else if (currentRightItem != null && Input.GetMouseButtonDown(1)) // Right button to use items 右键使用物品
        {
            if (currentRightItem.CompareTag("Liquid"))
            {
                UseLiquid(rightHand);
            }
            else
            {
                DropItem(rightHand); 
            }
        }
    }

    void DropItem(Transform hand)
    {
        sceneManagment.ItemThrown();

        if (currentLeftItem != null && hand == leftHand)
        {
            Vector3 throwDirection = playerCamera.transform.forward; // The direction of the item being thrown 物品投掷的方向
            currentLeftItem.Drop(throwDirection * dropForce); // Throwing items and setting the strength of the throw 丢弃物品，设置丢弃的力度
            leftHandHint.text = "";
            currentLeftItem = null;
        }
        if (currentRightItem != null && hand == rightHand)
        {
            Vector3 throwDirection = playerCamera.transform.forward; 
            currentRightItem.Drop(throwDirection * dropForce); 
            rightHandHint.text = "";
            currentRightItem = null;
        }
    }

    void UseLiquid(Transform hand)
    {
        if (currentLeftItem != null && hand == leftHand)
        {
            Health health = GetComponent<Health>();
            currentLeftItem.UseLiquid(health); // Passing Health components to items 将Health组件传递给物品
            leftHandHint.text = "";
            currentLeftItem = null;
        }
        if (currentRightItem != null && hand == rightHand)
        {
            Health health = GetComponent<Health>();
            currentRightItem.UseLiquid(health); 
            rightHandHint.text = "";
            currentRightItem = null;
        }
    }

    public void FreezingView(bool view)
    {
        canView = view;
    }

    private void TryExtendArm(Transform arm, Vector3 originalScale)
    {
        RaycastHit hit;
        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, interactDistance))
        {
            Item item = hit.collider.GetComponent<Item>();
            if (item != null)
            {
                StartCoroutine(ExtendArm(arm, originalScale, hit.collider.transform.position, item));
            }
        }
    }

    private System.Collections.IEnumerator ExtendArm(Transform arm, Vector3 originalScale, Vector3 targetPosition, Item item)
    {
        // Calculate arm extension direction and target scaling value 计算手臂伸长方向和目标缩放值
        Vector3 direction = targetPosition - arm.position;
        float distance = direction.magnitude;
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        Quaternion originalRotation = arm.rotation;
        Vector3 targetScale = originalScale;
        targetScale.z = originalScale.z * (distance / arm.localScale.z);



        // Gradually extend arms 逐渐伸长手臂
        float elapsedTime = 0f;
        while (elapsedTime < extendDuration)
        {
            elapsedTime += Time.deltaTime;
            arm.localScale = Vector3.Lerp(originalScale, targetScale, elapsedTime / extendDuration);
            arm.localRotation = Quaternion.Slerp(originalRotation, targetRotation, elapsedTime / extendDuration);
            yield return null;
        }

        // Attach the item to the hand 将物品附加到手
        if (arm == leftArm)
        {
            currentLeftItem = item;
            item.PickUp(leftHand);
            string itemHint = item.itemUsage;
            leftHandHint.text = $"{itemHint}";
        }
        else if (arm == rightArm)
        {
            currentRightItem = item;
            item.PickUp(rightHand);
            string itemHint = item.itemUsage;
            rightHandHint.text = $"{itemHint}";
        }

        // Wait for some time and then withdraw the arm 等待一段时间后收回手臂
        yield return new WaitForSeconds(0f);
        elapsedTime = 0f;
        
        while (elapsedTime < retractDuration)
        {
            elapsedTime += Time.deltaTime;
            arm.localScale = Vector3.Lerp(targetScale, originalScale, elapsedTime / retractDuration);
            arm.localRotation = Quaternion.Slerp(targetRotation, originalRotation, elapsedTime / extendDuration);
            yield return null;
        }
        
    }

    

}


