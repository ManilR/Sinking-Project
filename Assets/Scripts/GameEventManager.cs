using SDD.Events;
using UnityEngine;

public class GameEventManager : MonoBehaviour
{
    [SerializeField]
    private GameObject enemyBoat;
    [SerializeField]
    private Transform enemyBoatTransform;

    [SerializeField]
    private GameObject shark;
    [SerializeField]
    private Transform sharkTransform;

    [SerializeField]
    private GameObject seagull;
    [SerializeField]
    private Transform seagullTransform;

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

    private void Start()
    {
        shark.SetActive(false);
        seagull.SetActive(false);
        enemyBoat.SetActive(false);
    }

    void NewEventEventCallback(NewEventEvent e)
    {
        Debug.Log("New event launched : " + e.EventName);

        switch (e.EventName)
        {
            case "ENEMY_BOAT":
                enemyBoat.SetActive(true);
                enemyBoat.transform.position = enemyBoatTransform.position;
                enemyBoat.GetComponent<EnemyBoat>().StartEvent();
                break;

            case "SHARK":
                shark.SetActive(true);
                shark.transform.position = sharkTransform.position;
                shark.GetComponent<SharkController>().StartEvent();
                break;

            case "SEAGULL":
                seagull.SetActive(true);
                seagull.transform.position = seagullTransform.position;
                seagull.GetComponent<SharkController>().StartEvent();
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
            case "ENEMY_BOAT":
                enemyBoat.SetActive(false);
                break;

            case "SHARK":
                shark.SetActive(false);
                break;

            case "SEAGULL":
                seagull.SetActive(false);
                break;

            default:
                Debug.Log("Unknown event");
                break;
        }
    }
}
