using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class CanvasManager : MonoBehaviour
{
    [Header("Buttons")]
    public Button playButton;
    public Button settingsButton;
    public Button backButton;
    public Button quitButton;

    public Button resumeGame;
    public Button returnToMenu;


    [Header("Panels")]
    public GameObject mainMenuPanel;
    public GameObject settingsPanel;
    public GameObject pauseMenuPanel;

    [Header("Text Elements")]
    public TMP_Text livesText;
    public TMP_Text scoreText;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (playButton) playButton.onClick.AddListener(() => SceneManager.LoadScene(1));
        if (settingsButton) settingsButton.onClick.AddListener(() => SetMenus(settingsPanel, mainMenuPanel));
        if (backButton) backButton.onClick.AddListener(() => SetMenus(mainMenuPanel, settingsPanel));

        if (quitButton) quitButton.onClick.AddListener(QuitGame);

        if (resumeGame) resumeGame.onClick.AddListener(() => SetMenus(null, pauseMenuPanel));
        if (returnToMenu) returnToMenu.onClick.AddListener(() => SceneManager.LoadScene(0));

        //if (livesText)
        //{
        //    livesText.text = $"Lives: {GameManager.Instance.lives}";
        //    GameManager.Instance.OnLivesChanged += (lives) => livesText.text = $"Lives: {lives}";
        //}
        //if (scoreText)
        //{
        //    scoreText.text = $"Score: {GameManager.Instance.score}";
        //    GameManager.Instance.OnScoreChanged += (score) => scoreText.text = $"Score: {score}";
        //}
    }


    void SetMenus(GameObject menuToActivate, GameObject menuToDeactivate)
    {
        if (menuToActivate) menuToActivate.SetActive(true);
        if (menuToDeactivate) menuToDeactivate.SetActive(false);
    }

    void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    // Update is called once per frame
    void Update()
    {
        if (!pauseMenuPanel) return;

        if (Input.GetKeyDown(KeyCode.P))
        {
            if (pauseMenuPanel.activeSelf)
            {
                SetMenus(null, pauseMenuPanel);
            }
            else
            {
                SetMenus(pauseMenuPanel, null);
            }
        }
    }
}