using SDD.Events;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NewEventDescriptionHUD : MonoBehaviour
{
    [SerializeField] private float displayDuration = 2;

    private static readonly Dictionary<string, string> dict
    = new Dictionary<string, string>
    {
        { "ENEMY_BOAT", "Captain !\nAn enemy boat is approaching ! Attack it with canons" },
        { "SHARK", "Captain !\n A shark is bitting our boat, repair the hole" },
        {"SEAGULL", "Captain !\n A Seagull is attacking our sails, repairs them !" },
        {"OCTOPUS", "Captain !\n Be aware that an octopus is on its way to block us from accessing to the front!" }
    };

    private Text text;
    private Image image;

    private void OnEnable()
    {
        EventManager.Instance.AddListener<NewEventEvent>(NewEventEventCallback);
    }

    private void OnDisable()
    {
        EventManager.Instance.RemoveListener<NewEventEvent>(NewEventEventCallback);
    }
    private void Start()
    {
        text = this.transform.GetChild(0).GetComponent<UnityEngine.UI.Text>();
        image = this.GetComponent<UnityEngine.UI.Image>();
    }

    void NewEventEventCallback(NewEventEvent e)
    {
        CancelInvoke(nameof(CloseDescription));
        image.enabled = true;
        text.enabled = true;
        text.text = dict[e.EventName].ToString();
        Invoke(nameof(CloseDescription), displayDuration);
    }

    void CloseDescription()
    {
        image.enabled = false;
        text.enabled = false;
    }
}