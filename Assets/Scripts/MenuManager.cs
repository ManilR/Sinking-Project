using SDD.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    [SerializeField] GameObject m_MenuPanel;

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
        m_Panels.AddRange(new GameObject[] { m_MenuPanel });
    }

    private void OnEnable()
    {
        EventManager.Instance.AddListener<GameMenuEvent>(GameMenuEventCallback);
        EventManager.Instance.AddListener<GamePlayEvent>(GamePlayEventCallback);
    }

    private void OnDisable()
    {
        EventManager.Instance.RemoveListener<GameMenuEvent>(GameMenuEventCallback);
        EventManager.Instance.RemoveListener<GamePlayEvent>(GamePlayEventCallback);
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
        OpenPanel(null);
    }

    #endregion

    #region UI events callbacks

    public void PlayButtonClickedUICallback()
    {
        EventManager.Instance.Raise(new PlayButtonClickedEvent());
    }

    public void MenuButtonClickedUICallback()
    {
        EventManager.Instance.Raise(new MainMenuButtonClickedEvent());
    }
    #endregion
}
