﻿using UnityEngine;
using System.Linq;
using SDD.Events;

public enum GAMESTATE { menu, play, pause, victory, gameover }

public class GameManager : MonoBehaviour
{   
    // Watch out to Cache, change name of arrays to be sure to update their content
    public string[] MainEventsArray = { "ENEMY_BOAT" };
    public string[] SmallEventsArray = { "SHARK", "SEAGULL" };

    private static GameManager m_Instance;

    public static GameManager Instance
    {
        get { return m_Instance; }
    }

    private GAMESTATE m_State;

    private int IndexMainEvent = 0;
    private int IndexSmallEvent = 0;

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
        EventManager.Instance.AddListener<EventCompletedEvent>(EventCompletedEventCallback);
    }

    private void OnDisable()
    {
        EventManager.Instance.RemoveListener<PlayButtonClickedEvent>(PlayButtonClickedEventCallback);
        EventManager.Instance.RemoveListener<EventCompletedEvent>(EventCompletedEventCallback);
  
    }

    // Start is called before the first frame update
    void Start()
    {
        SetState(GAMESTATE.menu);
    }

    void NextMainEvent()
    {
        EventManager.Instance.Raise(new NewEventEvent() { EventName = MainEventsArray[IndexMainEvent] });
        IndexMainEvent++;
        IndexMainEvent %= MainEventsArray.Length;
    }

    void NextSmallEvent()
    {
        EventManager.Instance.Raise(new NewEventEvent(){ EventName = SmallEventsArray[IndexSmallEvent] });
        IndexSmallEvent++;
        IndexSmallEvent %= SmallEventsArray.Length;
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
        if (IndexMainEvent == 0) NextSmallEvent();
        if (IndexMainEvent == 0) NextMainEvent();
    }

    public void EventCompletedEventCallback(EventCompletedEvent e)
    {
        // Completed event is a Main one
        if (MainEventsArray.Contains(e.EventName))
        {
            NextMainEvent();
        }
        // Completed event is a Small one
        else
        {
            NextSmallEvent();
        }
    }

    #endregion
}
