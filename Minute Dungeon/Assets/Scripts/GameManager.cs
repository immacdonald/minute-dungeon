using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using Cinemachine;

#pragma warning disable 0649
public enum GameState { Regular, Paused }
public class GameManager : Singleton<GameManager>
{

    [HideInInspector] public GameState gameState = GameState.Regular;

    public bool inTitleScreen = false;
    [SerializeField] bool startOnTitle;
    [SerializeField] private GameObject titleScreen;
    [SerializeField] private Transform player;
    [SerializeField] private PlayerUI playerUI;
    [SerializeField] private GameObject[] startingRooms;

    [SerializeField] public GenerateDungeon generateDungeon;

    public static float GameTime;
    public static float GamePhysicsTime;
    public static float PlayTime;
    public static float DeltaTime;

    [HideInInspector] public bool countPlayTime = true;

    public int totalCoins = 0;
    public TMPro.TextMeshProUGUI coinText;
    public TMPro.TextMeshProUGUI timeText;

    public GameObject currentDungeon;

    public float dungeonTimeLeft;

    IEnumerator Start()
    {
        DontDestroyOnLoad(gameObject);
        Cursor.visible = false;

        if (startOnTitle)
        {
            gameState = GameState.Paused;
            titleScreen.SetActive(true);
            ToggleCursorVisibility(true);
        }
        else
        {
            yield return new WaitForSeconds(5);
            gameState = GameState.Regular;
            titleScreen.SetActive(false);
            ToggleCursorVisibility(false);
        }
    }

    public void ToggleCursorVisibility(bool visible)
    {
        Cursor.lockState = visible ? CursorLockMode.Confined : CursorLockMode.Locked;
        Cursor.visible = visible;
    }

    private void Update()
    {
        if (gameState == GameState.Regular)
        {
            GameTime += Time.deltaTime;
            DeltaTime = Time.deltaTime;
        } else
        {
            DeltaTime = 0;
        }

        if (gameState != GameState.Paused && countPlayTime)
        {
            PlayTime += Time.deltaTime;
        }
    }

    private void FixedUpdate()
    {
        if (gameState == GameState.Regular)
        {
            GamePhysicsTime += Time.fixedDeltaTime;
        }
    }

    private Vector2 defaultPlayerPosition = new Vector3(-2, 9.5f, 0);
    private Color defaultTimeColor = Color.white;

    public void StartGame()
    {
        player.position = defaultPlayerPosition;
        titleScreen.SetActive(false);
        ToggleCursorVisibility(false);
        gameState = GameState.Regular;
        inTitleScreen = false;
        currentDungeon = generateDungeon.GenerateNextRun();
        startingRooms[0].SetActive(true);
        startingRooms[1].SetActive(false);
        dungeonTimeLeft = 65;
        StartCoroutine(trackDungeonTime());
    }

    public void ReturnToHome()
    {
        StopCoroutine(trackDungeonTime());
        player.position = defaultPlayerPosition;
        totalCoins += player.GetComponent<Player>().GetCoinCount();
        player.GetComponent<Player>().ClearCoins();
        coinText.text = $"{totalCoins} Coins";
        timeText.color = defaultTimeColor;
        titleScreen.SetActive(true);
        ToggleCursorVisibility(true);
        gameState = GameState.Paused;
        inTitleScreen = true;
        Destroy(currentDungeon);
        startingRooms[0].SetActive(false);
        startingRooms[1].SetActive(true);
    }

    public void FullGameQuit()
    {
        Application.Quit();
    }

    private IEnumerator trackDungeonTime()
    {
        float dungeonStartTime = GameTime;
        bool changedColor = false;
        int totalTime = 22;

        while(totalTime - (GameTime - dungeonStartTime) > 0)
        {
            float timeRemaining = totalTime - (GameTime - dungeonStartTime);
            Debug.Log($"{timeRemaining}");
            timeText.text = $"{Mathf.Min(60, Mathf.RoundToInt(timeRemaining))}";

            if(!changedColor && timeRemaining < 10)
            {
                timeText.color = Color.red;
                changedColor = true;
            }

            yield return new WaitForSeconds(0.1f);
        }

        playerUI.ReturnToHome();
    }

}