using System.Collections;
using System.Collections.Generic;
using SDD.Events;
using UnityEngine;

public class OctopusController : MonoBehaviour
{
    static readonly string EventName = "OCTOPUS";
    [SerializeField] public float m_Speed = 10;
    [SerializeField] public GameObject m_Boat;
    [SerializeField] public float m_AttackDuration = 15;
    [SerializeField] public GameObject m_HangingPoint;

    private BoatMovement boatMovement;
    private bool m_Attacking = false;
    private bool m_AttackIsDone = false;
    private bool m_IsApproaching = true;

    private Rigidbody2D rb;
    private Rigidbody2D rbBoat;

    [SerializeField] public float health = 60;

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
        rb = this.GetComponent<Rigidbody2D>();
        rbBoat = m_Boat.GetComponent<Rigidbody2D>();
        m_Attacking = false;
        boatMovement = m_Boat.GetComponent<BoatMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        if(health <= 0)
        {
            Destroy(gameObject);
        }

        health -= Time.deltaTime;
        movement();

        if (Vector2.Distance(transform.position, m_HangingPoint.transform.position) < 2f)
        {
            // fix position
            m_Attacking = true;
            // Debug.Log("attaque pieuvre");
        }
     
    }




    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Boat")
        {
            m_IsApproaching = false;
            m_Attacking = true;
        }
    }

    private void movement()
    {
        if (m_IsApproaching)
        {
            transform.position = Vector3.MoveTowards(transform.position, m_HangingPoint.transform.position, m_Speed * Time.deltaTime);
            // Debug.Log("move towards");
        }
        else
        {
            rb.velocity = rbBoat.velocity;
        }

    }
}
