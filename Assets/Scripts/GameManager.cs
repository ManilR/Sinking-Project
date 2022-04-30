using UnityEngine;
using SDD.Events;

public enum GAMESTATE { menu, play, pause, victory, gameover }
public class GameManager : MonoBehaviour
{
    private static GameManager m_Instance;

    public static GameManager Instance
    {
        get { return m_Instance; }
    }

    private GAMESTATE m_State;

    public bool IsPlaying { get { return m_State == GAMESTATE.play; } }

    void SetState(GAMESTATE newState)
    {
        m_State = newState;
        switch (m_State)
        {
            case GAMESTATE.menu:
                EventManager.Instance.Raise(new GameMenuEvent());
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
    }

    private void OnDisable()
    {
        EventManager.Instance.RemoveListener<PlayButtonClickedEvent>(PlayButtonClickedEventCallback);
    }

    // Start is called before the first frame update
    void Start()
    {
        SetState(GAMESTATE.menu);
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
    #region Events callbacks

    void PlayButtonClickedEventCallback(PlayButtonClickedEvent e)
    {
        Play();
    }
    #endregion
}
