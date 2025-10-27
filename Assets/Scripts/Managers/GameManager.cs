using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using UnityEngine.Events;
using UnityEditor;
//using UnityEngine.Audio;

[DefaultExecutionOrder(-10)]
public class GameManager : MonoBehaviour
{
    //public AudioMixerGroup masterMixerGroup;
    //public AudioMixerGroup musicMixerGroup;
    //public AudioMixerGroup sfxMixerGroup;

    public delegate void PlayerSpawnDelegate(PlayerController playerInstance);
    public event PlayerSpawnDelegate OnPlayerControllerCreated;

    #region Player Controller Information
    public PlayerController playerPrefab;
    private PlayerController _playerInstance;
    public PlayerController playerInstance => _playerInstance;
    private Vector3 currentCheckpoint;
    #endregion

    //public AudioClip scorePickup;
    //private AudioSource audioSource;

    public event Action<int> OnLivesChanged;
    public event Action<int> OnScoreChanged;

    #region Stats
    private int _lives = 3;
    private int _score = 0;

    public int score
    {
        get => _score;
        set
        {
            if (value < 0)
                _score = 0;
            else
                _score = value;
            Debug.Log($"Score: {_score}");
            OnScoreChanged?.Invoke(_score);
        }
    }
    public int lives
    {
        get => _lives;
        set
        {
            if (value < 0)
            {
                //gameover goes here
                Debug.Log("Game Over! You have no life points left.");
                GameOver();
                _lives = 0;
            }
            else if (value < _lives)
            {
                //play hurt sound
                Debug.Log("Ouch! You lost a life point.");
                Respawn();

                _lives = value;
            }
            else if (value > maxLives)
            {
                _lives = maxLives;
            }
            else
            {
                _lives = value;
            }
            Debug.Log($"Lives: {_lives}");
            OnLivesChanged?.Invoke(_lives);
        }
    }

    public int maxLives = 6;
    #endregion

    #region Singleton Pattern
    private static GameManager _instance;
    public static GameManager Instance => _instance;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
            return;
        }

        Destroy(gameObject);
    }
    #endregion

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    private void OnEnable()
    {
        SceneManager.activeSceneChanged += SceneChanged;
    }

    private void OnDisable()
    {
        SceneManager.activeSceneChanged -= SceneChanged;
    }

    void SceneChanged(Scene current, Scene next)
    {
        if (next.name == "Game")
            StartLevel(new Vector3(-13.0209999f, 0.693000019f, 0.769403458f));
    }

    void GameOver()
    {
        SceneManager.LoadScene(0);
    }

    void Respawn()
    {
        _playerInstance.transform.position = currentCheckpoint;
    }

    public void StartLevel(Vector3 startPositon)
    {
        currentCheckpoint = startPositon;
        _playerInstance = Instantiate(playerPrefab, currentCheckpoint, Quaternion.identity);
        Debug.Log("GameManager: Player instantiated.");
        OnPlayerControllerCreated?.Invoke(_playerInstance);
        Debug.Log("GameManager: OnPlayerControllerCreated event fired.");
    }

    private bool isPaused = false;
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (SceneManager.GetActiveScene().buildIndex == 0)
            {
                SceneManager.LoadScene(1);
            }
            else
            {
                SceneManager.LoadScene(0);
            }
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            lives--; // set to subtract lives for testing
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            TogglePause();
        }
    }
    private void TogglePause()
    {
        isPaused = !isPaused;
        Time.timeScale = isPaused ? 0f : 1f;
        // toggle pause menu canvas in canvas manager script
    }

}