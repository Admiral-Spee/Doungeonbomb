using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagment : MonoBehaviour
{
    [Header("Timer")]
    private float elapsedTime = 0f; // Recording elapsed time 记录经过的时间

    [Header("Counter")]
    private int enemyKilled = 0;
    private int itemThrown = 0;

    [Header("Canvas")]
    public GameObject deathMenu; 
    public GameObject HUD;

    public TMP_Text timeText;
    public TMP_Text itemText;
    public TMP_Text enemyText;
    public TMP_Text deathText;

    [Header("Sounds")]
    public AudioClip gameOverSound;
    public AudioClip SucceedSound;

    private AudioSource audioSource;
    private bool havePlaySucceedSound = false;

    [Header("FinalSpots")]
    public List<GameObject> spawnPrefabs = new List<GameObject>();

    public FirstPersonController fpc;

    // Start is called before the first frame update
    void Start()
    {
        
        Time.timeScale = 1f;
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        elapsedTime += Time.deltaTime;

        CheckSpawnedObjects();
    }

    public void RestartGame()
    {
        
        Time.timeScale = 1f;

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void BackToMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void StartGame()
    {
        SceneManager.LoadScene(1);
    }

    public void PlayerDied()
    {
        // Unlock mouse 解锁鼠标
        Cursor.lockState = CursorLockMode.Confined;
        fpc.FreezingView(false);

        // Display mouse
        Cursor.visible = true;

        // Show death menu 显示死亡菜单
        timeText.text = $"{elapsedTime:F2}s";
        itemText.text = $"{itemThrown}";
        enemyText.text = $"{enemyKilled}";
        deathText.text = "You Died!";
        deathMenu.SetActive(true);

        HUD.SetActive(false);

        audioSource.PlayOneShot(gameOverSound);

        Time.timeScale = 0f;
    }

    void CheckSpawnedObjects()
    {
        foreach (GameObject spawnPrefab in spawnPrefabs)
        {
            if (spawnPrefab != null)
            {
                return;
            }
        }

        PlayerSucceed();
    }

    public void PlayerSucceed()
    {
        // Unlock mouse 解锁鼠标
        Cursor.lockState = CursorLockMode.Confined;
        fpc.FreezingView(false);

        // Display mouse 显示鼠标
        Cursor.visible = true;

        // Show death menu 显示死亡菜单
        timeText.text = $"{elapsedTime:F2}s";
        itemText.text = $"{itemThrown}";
        enemyText.text = $"{enemyKilled}";
        deathText.text = "Level Cleared!";
        deathMenu.SetActive(true);

        HUD.SetActive(false);

        if (!havePlaySucceedSound)
        {
            audioSource.PlayOneShot(SucceedSound);
            havePlaySucceedSound = true;
        }

        Time.timeScale = 0f;
    }

    public void EnemyKilled()
    {
        enemyKilled += 1;
    }

    public void ItemThrown()
    {
        itemThrown += 1;
    }

}
