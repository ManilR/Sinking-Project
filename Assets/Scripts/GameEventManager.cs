using SDD.Events;
using UnityEngine;

public class GameEventManager : MonoBehaviour
{
    [SerializeField] private GameObject enemyBoatPrefab;
    [SerializeField] private GameObject enemyBoat;
    [SerializeField] private GameObject waterWave2D;
    [SerializeField] private Transform enemyBoatTransform;

    [SerializeField] private GameObject shark_prefab;
    [SerializeField] private Transform sharkTransform;
    [SerializeField] private GameObject m_SharkHole;
    [SerializeField] private GameObject boat;

    [SerializeField] private GameObject seagull_prefab;
    [SerializeField] private GameObject seagull_hole;
    [SerializeField] private Transform seagullTransform;
    [SerializeField] private Transform sail;

    private bool isEnemyBoat = false;
    private void OnEnable()
    {
        EventManager.Instance.AddListener<NewEventEvent>(NewEventEventCallback);
        EventManager.Instance.AddListener<EventCompletedEvent>(EventCompletedEventCallback);
    }

    private void OnDisable()
    {
        EventManager.Instance.RemoveListener<NewEventEvent>(NewEventEventCallback);
        EventManager.Instance.RemoveListener<EventCompletedEvent>(EventCompletedEventCallback);
    }
    private void Start()
    {
    }

    void NewEventEventCallback(NewEventEvent e)
    {
        Debug.Log("New event launched : " + e.EventName);

        switch (e.EventName)
        {
            case "ENEMY_BOAT":
                if(isEnemyBoat)
                {
                    break;
                }
                GameObject enemy_boat = Instantiate(enemyBoatPrefab, enemyBoatTransform.position, Quaternion.identity);
                enemy_boat.GetComponent<EnemyBoat>().target = boat;
                enemy_boat.GetComponent<EnemyBoat>().water = waterWave2D;
                isEnemyBoat = true;
                break;

            case "SHARK":
                GameObject shark = Instantiate(shark_prefab, sharkTransform.position, Quaternion.identity);
                shark.GetComponent<SharkController>().m_SharkHole = m_SharkHole;
                shark.GetComponent<SharkController>().target = boat;
                break;

            case "SEAGULL":
                GameObject seagull = Instantiate(seagull_prefab, seagullTransform.position, Quaternion.identity);
                seagull.GetComponent<SeagullController>().m_SeagullHole = seagull_hole;
                seagull.GetComponent<SeagullController>().m_Boat = boat;
                seagull.GetComponent<SeagullController>().m_Sail = sail;
                break;

            default:
                Debug.Log("Unknown event");
                break;
        }

    }

    void EventCompletedEventCallback(EventCompletedEvent e)
    {
        Debug.Log("Event completed : " + e.EventName);
        if(e.EventName == "ENEMY_BOAT")
        {
            isEnemyBoat = false;
        }
    }
}
