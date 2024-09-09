using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public static GAMESTATE gameState { get; private set; }
    public static int MovesAvailable;

    public GameObject MainMenuPanel;
    public GameObject _InGamePanel;
    public GameObject StageFinishedPanel;
    public GameObject RetryPanel;

    public GameObject MainThemeAudioSource;
    static bool AudioInstantiated = false;

    public Transform[] PartyPS;

    public Material shapesMaterial;

    private void Awake()
    {
        Application.targetFrameRate = 60;

        instance = this;
        gameState = GAMESTATE.Menu;

        GameAnalyticsSDK.GameAnalytics.Initialize();
    }

    private void Start()
    {
        StartSetter();
        AudioManager.instance.Play("camera");
        if (AudioInstantiated == false)
        {
            GameObject g = Instantiate(MainThemeAudioSource, MainThemeAudioSource.transform.position, Quaternion.identity);
            DontDestroyOnLoad(g);
            AudioInstantiated = true;
        }
    }

    void StartSetter()
    {
        MainMenuPanel.SetActive(true);
        _InGamePanel.SetActive(false);
        StageFinishedPanel.SetActive(false);
        RetryPanel.SetActive(false);
    }

    public void StartGame()
    {
        MainMenuPanel.SetActive(false);
        _InGamePanel.SetActive(true);

        gameState = GAMESTATE.Game;
    }

    public void StageFinished()
    {
        _InGamePanel.SetActive(false);
        StageFinishedPanel.SetActive(true);
        //SpawnPartyPapers();

        gameState = GAMESTATE.Win;
    }

    public void GameOver()
    {
        gameState = GAMESTATE.Lose;
        _InGamePanel.SetActive(false);
        RetryPanel.SetActive(true);
    }

    private void Update()
    {
        GetMouseInput();
    }

    void GetMouseInput()
    {
        if(Input.GetMouseButtonDown(1))
        {
            ReloadScene();
        }
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            if (gameState == GAMESTATE.Menu)
                Application.Quit();
            else
                ReloadScene();
        }
    }

    public void MoveUsed()
    {
        MovesAvailable -= 1;
        InGamePanel.instance.UpdateMoves();

        if(MovesAvailable == 0 && PathsController.completed == false)
        {
            GameOver();
        }
    }

    public void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void SpawnPartyPapers()
    {
        for(int i = 0; i < PartyPS.Length;i++)
        {
            Instantiate(PartyPS[i], Vector3.zero, PartyPS[i].rotation);
        }
    }
}

public enum GAMESTATE
{
    Menu,
    Game,
    Win,
    Lose
}
