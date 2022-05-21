using System.Collections;
using System.Collections.Generic;
using SDD.Events;
using UnityEngine;

public class OctopusController : MonoBehaviour
{
    static readonly string EventName = "OCTOPUS";
    [SerializeField] public float m_Speed;
    [SerializeField] public GameObject m_Boat;
    [SerializeField] public float m_AttackDuration = 3;
    [SerializeField] public GameObject m_HangingPoint;

    private bool m_Attacking = false;
    private bool m_AttackIsDone = false;

    private void OnEnable()
    {
        EventManager.Instance.AddListener<ResetMapEvent>(ResetMapEventCallback);
    }

    private void OnDisable()
    {
        EventManager.Instance.RemoveListener<ResetMapEvent>(ResetMapEventCallback);
    }

    void ResetMapEventCallback(ResetMapEvent e)
    {
        Destroy(gameObject);
    }


    // Start is called before the first frame update
    void Start()
    {
        m_Attacking = false;
    }

    // Update is called once per frame
    void Update()
    {
        var step = m_Speed * Time.deltaTime; // calculate distance to move
        transform.position = Vector3.MoveTowards(transform.position, m_HangingPoint.transform.position, step);
    }
}
