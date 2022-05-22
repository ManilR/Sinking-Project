using UnityEngine;
using System.Linq;
using SDD.Events;
using System.Collections;

public enum GAMESTATE { menu, play, pause, victory, gameover, credits, controls }


// Main manager class, handle game state, launch game events
public class GameManager : MonoBehaviour
{   
    public string[] MainEventsArray = { "ENEMY_BOAT" , "OCTOPUS" };
    public string[] SmallEventsArray = { "SHARK", "SEAGULL"};
    public static string[] GameLevels = { "Easy", "Medium", "Hard" };
    public static float[] GameLevelCoefs = { 1.2f, 1f, 0.8f };
    [SerializeField] private GameObject scorePanel;

    private static GameManager m_Instance;

    public static GameManager Instance
    {
        get { return m_Instance; }
    }

    private GAMESTATE m_State;

    private int IndexMainEvent = 0;
    private int IndexSmallEvent = 0;

    public bool IsPlaying { get { return m_State == GAMESTATE.play; } }

    public float gameLevelCoef { get; set; }

    public float timer;

    void SetState(GAMESTATE newState)
    {
        m_State = newState;
        switch (m_State)
        {
            case GAMESTATE.menu:
                timer = 0;
                EventManager.Instance.Raise(new GameMenuEvent());
                EventManager.Instance.Raise(new ResetMapEvent());
                break;
            case GAMESTATE.play:
                EventManager.Instance.Raise(new GamePlayEvent());
                break;
            case GAMESTATE.pause:
                EventManager.Instance.Raise(new GamePauseEvent());
                break;
            case GAMESTATE.victory:
                EventManager.Instance.Raise(new GameVictoryEvent());
                break;
            case GAMESTATE.gameover:
                EventManager.Instance.Raise(new GameOverEvent());
                break;
            case GAMESTATE.credits:
                EventManager.Instance.Raise(new CreditsEvent());
                break;
            case GAMESTATE.controls:
                EventManager.Instance.Raise(new ControlsEvent());
                break;
            default:
                break;
        }
    }

    void Play()
    {
        Time.timeScale = 1;
        SetState(GAMESTATE.play);
    }

    private void Awake()
    {
        Application.targetFrameRate = 45;
        if (!m_Instance) m_Instance = this;
        else Destroy(this.gameObject);
        Time.timeScale = 0;
        }

    private void OnEnable()
    {
        EventManager.Instance.AddListener<PlayButtonClickedEvent>(PlayButtonClickedEventCallback);
        EventManager.Instance.AddListener<SetStateGameoverEvent>(SetStateGameoverEventCallback);
        EventManager.Instance.AddListener<SetStateVictoryEvent>(SetStateVictoryEventCallback);
        EventManager.Instance.AddListener<MainMenuButtonClickedEvent>(MainMenuButtonClickedEventCallback);
        EventManager.Instance.AddListener<ControlsButtonClickedEvent>(ControlsButtonClickedEventCallback);
        EventManager.Instance.AddListener<CreditsButtonClickedEvent>(CreditsButtonClickedEventCallback);
        EventManager.Instance.AddListener<ResetMapEvent>(ResetMapEventCallback);
    }

    private void OnDisable()
    {
        EventManager.Instance.RemoveListener<PlayButtonClickedEvent>(PlayButtonClickedEventCallback);
        EventManager.Instance.RemoveListener<SetStateGameoverEvent>(SetStateGameoverEventCallback);
        EventManager.Instance.RemoveListener<SetStateVictoryEvent>(SetStateVictoryEventCallback);
        EventManager.Instance.RemoveListener<MainMenuButtonClickedEvent>(MainMenuButtonClickedEventCallback);
        EventManager.Instance.RemoveListener<ControlsButtonClickedEvent>(ControlsButtonClickedEventCallback);
        EventManager.Instance.RemoveListener<CreditsButtonClickedEvent>(CreditsButtonClickedEventCallback);
        EventManager.Instance.RemoveListener<ResetMapEvent>(ResetMapEventCallback);
    }

    
    void Start()
    {
        SetState(GAMESTATE.menu);
    }

    public void CloseApplication()
    {
        Application.Quit();
    }

    // Launch next main event, loop on array
    void NextMainEvent()
    {
        EventManager.Instance.Raise(new NewEventEvent() { EventName = MainEventsArray[IndexMainEvent] });
        IndexMainEvent++;
        IndexMainEvent %= MainEventsArray.Length;
    }

    // Launch next small event 
    void NextSmallEvent()
    {
        EventManager.Instance.Raise(new NewEventEvent(){ EventName = SmallEventsArray[IndexSmallEvent] });
        IndexSmallEvent++;
        IndexSmallEvent %= SmallEventsArray.Length;
    }

    private void Update()
    {
        if(IsPlaying) timer += Time.deltaTime;
    }

    private void FixedUpdate()
    {
        if (Input.GetButtonDown("Pause"))
        {
            if (IsPlaying)
            {
                Time.timeScale = 0;
                SetState(GAMESTATE.pause);
            }
            else
            {
                Play();
            }
        }
    }

    IEnumerator LaunchMainEvent()
    {
        while (true)
        {
            NextMainEvent();
            yield return new WaitForSeconds(33);
        }   
    }

    IEnumerator LaunchSmallEvent()
    {
        while (true)
        {
            NextSmallEvent();
            yield return new WaitForSeconds(20);
        }
    }

    #region Events callbacks

    void PlayButtonClickedEventCallback(PlayButtonClickedEvent e)
    {
        Play();
        if (e.fromMenu)
        {
            gameLevelCoef = GameManager.GameLevelCoefs[e.levelIndex];
            EventManager.Instance.Raise(new GameLevelEvent() { levelCoef = gameLevelCoef });
            StartCoroutine(LaunchSmallEvent());
            Invoke(nameof(LaunchMainEventWithOffset), 6);
        }
    }

    void LaunchMainEventWithOffset()
    {
        StartCoroutine(LaunchMainEvent());
    }
    void SetStateGameoverEventCallback(SetStateGameoverEvent e)
    {
        SetState(GAMESTATE.gameover);
        Time.timeScale = 0;
    }

    void SetStateVictoryEventCallback(SetStateVictoryEvent e)
    {
        SetHighScore();
        SetState(GAMESTATE.victory);
        Time.timeScale = 0;
    }

    private void SetHighScore()
    {
        int score = (int)scorePanel.GetComponent<ScoreHUD>().gameScore;
        int highscore = 0;
        highscore = PlayerPrefs.GetInt("highscore", highscore);
        if (score > highscore)
        {
            highscore = score;

            PlayerPrefs.SetInt("highscore", highscore);
        }
    }

    void MainMenuButtonClickedEventCallback(MainMenuButtonClickedEvent e)
    {
        SetState(GAMESTATE.menu);
    }

    void ControlsButtonClickedEventCallback(ControlsButtonClickedEvent e)
    {
        SetState(GAMESTATE.controls);
    }

    void CreditsButtonClickedEventCallback(CreditsButtonClickedEvent e)
    {
        SetState(GAMESTATE.credits);
    }

    void ResetMapEventCallback(ResetMapEvent e)
    {
        IndexSmallEvent = 0;
        IndexMainEvent = 0;
    }
    #endregion
}
