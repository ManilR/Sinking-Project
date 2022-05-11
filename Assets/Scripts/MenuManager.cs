using SDD.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    [SerializeField] GameObject m_MenuPanel;
    [SerializeField] GameObject m_PausePanel;
    [SerializeField] GameObject m_HudPanel;
    [SerializeField] GameObject m_VictoryPanel;  // Not use for moment
    [SerializeField] GameObject m_GameOverPanel;

    List<GameObject> m_Panels = new List<GameObject>();

    void OpenPanel(GameObject panel)
    {
        m_Panels.ForEach(item => {
            if (item)
            {
                item.SetActive(item == panel);
            }
        });
    }

    private void Awake()
    {
        m_Panels.AddRange(new GameObject[] { m_MenuPanel, m_PausePanel, m_HudPanel, m_VictoryPanel, m_GameOverPanel });
    }

    private void OnEnable()
    {
        EventManager.Instance.AddListener<GameMenuEvent>(GameMenuEventCallback);
        EventManager.Instance.AddListener<GamePlayEvent>(GamePlayEventCallback);
        EventManager.Instance.AddListener<GamePauseEvent>(GamePauseEventCallback);
        EventManager.Instance.AddListener<GameOverEvent>(GameOverEventCallback);
    }

    private void OnDisable()
    {
        EventManager.Instance.RemoveListener<GameMenuEvent>(GameMenuEventCallback);
        EventManager.Instance.RemoveListener<GamePlayEvent>(GamePlayEventCallback);
        EventManager.Instance.RemoveListener<GamePauseEvent>(GamePauseEventCallback);
        EventManager.Instance.RemoveListener<GameOverEvent>(GameOverEventCallback);
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    #region GameManager events callbacks
    void GameMenuEventCallback(GameMenuEvent e)
    {
        OpenPanel(m_MenuPanel);
    }

    void GamePlayEventCallback(GamePlayEvent e)
    {
        OpenPanel(m_HudPanel);
    }

    void GamePauseEventCallback(GamePauseEvent e)
    {
        OpenPanel(m_PausePanel);
    }

    void GameOverEventCallback(GameOverEvent e)
    {
        OpenPanel(m_GameOverPanel);
    }

    #endregion

    #region UI events callbacks

    public void PlayButtonFromMenuClickedUICallback()
    {
        EventManager.Instance.Raise(new PlayButtonClickedEvent() { fromMenu = true });
    }

    public void PlayButtonFromPauseClickedUICallback()
    {
        EventManager.Instance.Raise(new PlayButtonClickedEvent() { fromMenu = false });
    }

    public void MenuButtonClickedUICallback()
    {
        EventManager.Instance.Raise(new MainMenuButtonClickedEvent());
    }
    #endregion
}
