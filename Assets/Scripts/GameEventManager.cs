using SDD.Events;
using UnityEngine;

// Handles game events launched by GameManager.cs
public class GameEventManager : MonoBehaviour
{
    #region Enemies variables
    // Enemy boat
    [SerializeField] private GameObject enemyBoatPrefab;
    [SerializeField] private GameObject enemyBoat;
    [SerializeField] private GameObject waterWave2D;
    [SerializeField] private Transform enemyBoatTransform;
    

    // Shark
    [SerializeField] private GameObject shark_prefab;
    [SerializeField] private Transform sharkTransform;
    [SerializeField] private GameObject m_SharkHole;
    [SerializeField] private GameObject boat;

    // Seagull
    [SerializeField] private GameObject seagull_prefab;
    [SerializeField] private GameObject seagull_hole;
    [SerializeField] private Transform seagullTransform;
    [SerializeField] private Transform sail;

    // Octopus
    [SerializeField] private GameObject octopus_prefab;
    [SerializeField] private Transform octopusTransform;
    [SerializeField] private GameObject octopus_hanging_point;
    [SerializeField] private GameObject octopus_block;

    private bool isMainEvent = false;
    #endregion

    [SerializeField] private GameObject scorePanel;

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

    // Callback of event "new event" launched 
    // Instantiate prefab on their transform spawner
    void NewEventEventCallback(NewEventEvent e)
    {
        switch (e.EventName)
        {
            case "ENEMY_BOAT":
                if(isMainEvent)
                {
                    break;
                }
                GameObject enemy_boat = Instantiate(enemyBoatPrefab, enemyBoatTransform.position, Quaternion.identity);
                enemy_boat.GetComponent<EnemyBoat>().target = boat;
                enemy_boat.GetComponent<EnemyBoat>().water = waterWave2D;
                isMainEvent = true;
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
            case "OCTOPUS":
                if (isMainEvent)
                {
                    break;
                }
                GameObject octopus = Instantiate(octopus_prefab, octopusTransform.position, Quaternion.identity);
                octopus.GetComponent<OctopusController>().m_HangingPoint = octopus_hanging_point;
                octopus.GetComponent<OctopusController>().m_Boat = boat;
                octopus.GetComponent<OctopusController>().m_block = octopus_block;
                isMainEvent = true;
                break;

            default:
                Debug.Log("Unknown event");
                break;
        }
    }

    // Add bonus when enemy boat is sinked
    void EventCompletedEventCallback(EventCompletedEvent e)
    {
        if(e.EventName == "ENEMY_BOAT" || e.EventName == "OCTOPUS")
        {
            isMainEvent = false;
        }
        scorePanel.GetComponent<ScoreHUD>().gameScore += 500;
    }
}
