using SDD.Events;
using UnityEngine;

public class GameEventManager : MonoBehaviour
{

    [SerializeField]
    private GameObject shark;
    [SerializeField]
    private Transform sharkSpawnerTransform;

    private void OnEnable()
    {
        EventManager.Instance.AddListener<NewEventEvent>(NewEventEventCallback);
        EventManager.Instance.RemoveListener<EventCompletedEvent>(EventCompletedEventCallback);
    }

    private void OnDisable()
    {
        EventManager.Instance.RemoveListener<NewEventEvent>(NewEventEventCallback);
        EventManager.Instance.RemoveListener<EventCompletedEvent>(EventCompletedEventCallback);
    }

    // Start is called before the first frame update
    void Start()
    {
        shark.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    void NewEventEventCallback(NewEventEvent e)
    {
        Debug.Log("New event launched : " + e.EventName);

        switch (e.EventName)
        {
            case "SHARK":
                shark.SetActive(true);
                shark.GetComponent<SharkController>().StartEvent();
                shark.transform.position = sharkSpawnerTransform.position;
                break;

            default:
                Debug.Log("Unknown event");
                break;
        }

    }

    void EventCompletedEventCallback(EventCompletedEvent e)
    {
        Debug.Log("Event completed : " + e.EventName);

        switch (e.EventName)
        {
            case "SHARK":
                shark.SetActive(false);
                break;

            default:
                Debug.Log("Unknown event");
                break;
        }
    }
}
