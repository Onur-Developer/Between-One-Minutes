using System;
using System.Collections;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.Video;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public GameObject enemies;
    public Transform[] playerPool;
    public int playerorderCount;
    public int cycleCount;
    public Transform ammos;
    private float _timeValue;
    public float changePlayerTime;
    private Camera _camera;
    public Transform players;
    private bool _isFiveSecond;
    public bool isPlay;
    public int score;
    private int killingenemies;
    public bool isJoyStick;
    public JoystickNullCheck jn;
    public FloatingJoystick jf;
    [HideInInspector] public bool isTutorial=true;

    [Header("Enemies Property")] public float enemyspeed;
    public float xMinDistance;
    public float xMaxDistance;
    public float yMinDistance;
    public float yMaxDistance;
    public float lauchPower;
    public float enemyAttackCooldownMax=2f;
    private const float defaultenemySpeed = 2f;
    private const float defaultxMinDistance = -11f;
    private const float defaultxMaxDistance = 11f;
    private const float defaultyMinDistance = -7.5f;
    private const float defaultyMaxDistance = 5.5f;
    private const float defaultLaunchPower = 450f;

    [Header("UI")] [SerializeField] private TextMeshProUGUI timeUI;
    [SerializeField] private TextMeshProUGUI enemySpawnerUI;
    [SerializeField] private TextMeshProUGUI enemySpeedUI;
    [SerializeField] private TextMeshProUGUI enemyLauncUI;
    [SerializeField] private TextMeshProUGUI cycleCountUI;
    [SerializeField] private TextMeshProUGUI diedEnemiesUI;
    [SerializeField] private RectTransform healthBar;
    [SerializeField] private Vector2 defaultHealthBarTransform;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI highScoreText;
    public GameObject scorePanel;
    public VideoPlayer videoPlayer;
    public GameObject videoBackground;
    public TextMeshProUGUI videoCount;

    [Header("Walls")] [SerializeField] private Transform[] walls;
    [SerializeField] private Vector2[] positionWalls;
    [SerializeField] private Vector2[] scaleWalls;
    [SerializeField] private Vector2[] defaultPositionWalls;
    [SerializeField] private Vector2[] defaultScaleWalls;

    [Header("Enemy Spawner")] [SerializeField]
    private Transform enemySpawnerParent;

    [SerializeField] private GameObject enemySpawner;
    [SerializeField] private Transform mouse;
    [SerializeField] private GameObject replayButton;
    [SerializeField] private GameObject homeButton;
    [SerializeField] private Transform replayMenu;
    public GameObject gameManagerUI;
    public GameObject mainMenu;
    public AudioSource backgroundPlayer;
    public float enemySpawn = 3f;
    public CameraShake cameraShake;

    public delegate void DiedDelegate();

    public event DiedDelegate RestartEvent;
    public event DiedDelegate CompleteCircleEvent;


    public int KillingEnemies
    {
        get { return killingenemies; }
        set
        {
            killingenemies += value;
            diedEnemiesUI.text = (killingenemies / 10).ToString();
        }
    }

    private void Awake()
    {
        if (instance == null)
            instance = this;

        _camera = Camera.main;

        RestartEvent += ResetWallandUI;
        RestartEvent += AddEnemySpawner;
        RestartEvent += ResetEnemies;
        isTutorial = PlayerPrefs.GetInt("Tutorial", 1) == 1;
    }

    private void Update()
    {
        WriteTime();
    }
    
    void WriteScore()
    {
        scorePanel.SetActive(true);
        int time = Convert.ToInt32(_timeValue);
        score += time + killingenemies + (cycleCount * 100);
        scoreText.text = score.ToString();
        if (PlayerPrefs.GetInt("HighScore") < score)
            PlayerPrefs.SetInt("HighScore", score);
        highScoreText.text = PlayerPrefs.GetInt("HighScore").ToString();
    }

    void WriteTime()
    {
        if (!isPlay)
            return;
        _timeValue += Time.deltaTime;
        changePlayerTime += Time.deltaTime;
        timeUI.text = string.Format("{0:F0}", _timeValue);
        if (changePlayerTime > 60f)
        {
            changePlayerTime = 0;
            ChangePlayer();
            FiveSecondEnd();
        }
    }

    public void OkButton()
    {
        videoBackground.SetActive(false);
        backgroundPlayer.Play();
        Time.timeScale = 1;
    }

    public void NextButton()
    {
        MainPlayer mainPlayer = playerPool[playerorderCount].GetComponent<MainPlayer>();
        mainPlayer.PlayTutorialVideo();
    }

    public void ChangePlayer()
    {
        Transform oldPlayer = playerPool[playerorderCount];
        oldPlayer.gameObject.SetActive(false);
        playerorderCount++;
        if (playerorderCount >= 5)
        {
            playerorderCount = 0;
            cycleCount++;
            CompleteCycle();
        }

        Transform newPlayer = playerPool[playerorderCount];
        newPlayer.gameObject.SetActive(true);
        newPlayer.position = oldPlayer.position;
        if (enemies.transform.childCount > 0)
            enemies.BroadcastMessage("FindPlayer");
    }

    public void PlayerDied()
    {
        isPlay = false;
        ResetWallandUI();
        DeleteSpawner();
        FiveSecondEnd();
        StopCoroutine(nameof(FiveSecond));
        isTutorial = PlayerPrefs.GetInt("Tutorial", 1) == 1;
    }

    public void ReturnMouse()
    {
        players.gameObject.SetActive(false);
        mouse.gameObject.SetActive(true);
        ReplaySetting();
        WriteScore();
    }

    void ReplaySetting()
    {
        Vector2 position = new Vector2(Random.Range(2f,11f), Random.Range(-2.5f, 6f));
        GameObject obj1 = Instantiate(replayButton, replayMenu);
        obj1.transform.position = position;
        position = new Vector2(Random.Range(2f,11f), Random.Range(-2.5f, 6f));
        while (Vector2.Distance(obj1.transform.position, position) < 2f)
        {
            position = new Vector2(Random.Range(2f, 11f), Random.Range(-2.5f, 6f));
        }

        GameObject obj2 = Instantiate(homeButton, replayMenu);
        obj2.transform.position = position;
    }

    public void RestartButton()
    {
        gameManagerUI.SetActive(true);
        isPlay = true;
        _timeValue = 0;
        changePlayerTime = 0;
        playerorderCount = 0;
        cycleCount = 0;
        score = 0;
        killingenemies = 0;
        KillingEnemies = 0;
        enemyAttackCooldownMax = 2f;
        enemySpawn = 3f;
        StartCoroutine(nameof(FiveSecond));
        players.gameObject.SetActive(true);
        for (int i = 0; i < playerPool.Length; i++)
        {
            playerPool[i].gameObject.SetActive(false);
            playerPool[i].transform.position = mouse.position;
        }

        playerPool[0].gameObject.SetActive(true);
        if (RestartEvent != null)
            RestartEvent();
    }

    public void QuitButton()
    {
        Application.Quit();
    }

    void CompleteCycle()
    {
        for (int i = 0; i < walls.Length; i++)
        {
            Transform wall = walls[i];
            Vector2 newPosition = new Vector2(wall.position.x + positionWalls[i].x,
                wall.position.y + positionWalls[i].y);
            Vector2 newscale = new Vector2(wall.localScale.x + scaleWalls[i].x, wall.localScale.y);
            wall.transform.position = newPosition;
            wall.localScale = newscale;
        }

        _camera.orthographicSize += 1.65f; // 2.2f
        healthBar.transform.position = new Vector2(healthBar.transform.position.x+.6f,
            healthBar.transform.position.y + .9f);
        enemySpawn += 3f;
        AddEnemySpawner();
        UpdateEnemies();
        isTutorial = false;
        if (CompleteCircleEvent != null)
            CompleteCircleEvent();
    }

    void UpdateEnemies()
    {
        xMinDistance -= 1.3f;
        xMaxDistance += 1.3f;
        yMinDistance -= .8f;
        yMaxDistance += .8f;
        enemyspeed += 1;
        lauchPower += 50;
        enemyAttackCooldownMax += 0.7f;
        WriteValues();
    }

    void ResetEnemies()
    {
        xMinDistance = defaultxMinDistance;
        xMaxDistance = defaultxMaxDistance;
        yMinDistance = defaultyMinDistance;
        yMaxDistance = defaultyMaxDistance;
        enemyspeed = defaultenemySpeed;
        lauchPower = defaultLaunchPower;
        WriteValues();
    }

    void WriteValues()
    {
        enemySpawnerUI.text = enemySpawnerParent.childCount.ToString();
        enemySpeedUI.text = Mathf.CeilToInt(enemyspeed).ToString(CultureInfo.InvariantCulture);
        enemyLauncUI.text = lauchPower.ToString(CultureInfo.InvariantCulture);
        cycleCountUI.text = cycleCount.ToString();
    }

    void ResetWallandUI()
    {
        _camera.orthographicSize = 16;
        healthBar.position = defaultHealthBarTransform;
        for (int i = 0; i < walls.Length; i++)
        {
            walls[i].localScale = defaultScaleWalls[i];
            walls[i].position = defaultPositionWalls[i];
        }
    }

    void DeleteSpawner()
    {
        for (int i = 0; i < enemySpawnerParent.childCount; i++)
        {
            Destroy(enemySpawnerParent.GetChild(i).gameObject);
        }
    }

    void AddEnemySpawner()
    {
        Instantiate(enemySpawner, enemySpawnerParent);
    }

    void FiveSecondEnd()
    {
        if (enemies.transform.childCount > 0)
        {
            for (int i = 0; i < enemies.transform.childCount; i++)
            {
                Destroy(enemies.transform.GetChild(i).gameObject);
            }
        }

        for (int i = 0; i < enemySpawnerParent.childCount; i++)
        {
            enemySpawnerParent.GetChild(i).GetComponent<EnemySpawner>().enabled = changePlayerTime < 2;
        }
    }

    IEnumerator FiveSecond()
    {
        yield return new WaitForSeconds(55f);
        FiveSecondEnd();
        yield return new WaitForSeconds(5f);
        StartCoroutine(nameof(FiveSecond));
    }
}