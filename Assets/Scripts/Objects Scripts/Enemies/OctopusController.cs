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
        boatMovement = m_Boat.GetComponent<BoatMovement>();
    }

    // Update is called once per frame
    void Update()
    {

        if (m_IsApproaching)
        {
            transform.position = Vector3.MoveTowards(transform.position, m_HangingPoint.transform.position, m_Speed * Time.deltaTime);
            // Debug.Log("move towards");
        }

        if (Vector2.Distance(transform.position, m_HangingPoint.transform.position) < 2f)
        {
            // fix position
            m_Attacking = true;
            // Debug.Log("attaque pieuvre");
        }
     
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.name == "Boat" && m_IsApproaching)
        {
            m_IsApproaching = false;
            
            this.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;
            
           /* Vector2 newVelocity = Vector2.zero;

            var math = boatMovement.movementSpeed * boatMovement.movementSpeed * Mathf.Pow(10, -1) * -1;
            newVelocity.Set(math, 0.0f); // a contrary force
            this.GetComponent<Rigidbody2D>().velocity = newVelocity;*/
        }
    }
}
