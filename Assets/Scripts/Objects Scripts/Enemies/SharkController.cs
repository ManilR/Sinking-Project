using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SharkController : MonoBehaviour
{
    [SerializeField] private GameObject m_SharkHole;
    [SerializeField] public float m_Speed;
    [SerializeField] public GameObject target;
    private BoatMovement boatMovement;
    private SpriteRenderer m_SharkHoleSprite;
    private bool m_Swimming = true;
    private bool m_Attacking = false;
    private bool m_AttackIsDone = false;
    private float m_AttackDuration = 5;


    // Start is called before the first frame update
    void Start()
    {
        m_SharkHoleSprite = m_SharkHole.GetComponent<SpriteRenderer>();
        m_SharkHoleSprite.enabled = false;

        boatMovement = target.GetComponent<BoatMovement>();
    }

    void proceedAttackOnBoat()
    {
        if (m_Swimming && !m_Attacking)
        {
            var step = m_Speed * Time.deltaTime; // calculate distance to move
            transform.position = Vector3.MoveTowards(transform.position, m_SharkHole.transform.position, step);
        }
        else
        {
            var step = boatMovement.movementSpeed * Time.deltaTime; // calculate distance to move
            transform.position = Vector3.MoveTowards(transform.position, m_SharkHole.transform.position, step);
        }
    }

    void biteBoatStructure()
    {
        if (!m_Attacking)
        {
            m_Attacking = true;
            transform.Rotate(new Vector3(0, 0, 20));

            Vector2 newVelocity = Vector2.zero;

            var math = target.GetComponent<BoatMovement>().movementSpeed * target.GetComponent<BoatMovement>().movementSpeed * Mathf.Pow(10, -1) * -1;
            newVelocity.Set(math, 0.0f); // a contrary force
            this.GetComponent<Rigidbody2D>().velocity = newVelocity;
        }
    }


    // Update is called once per frame
    void Update()
    {
        proceedAttackOnBoat();

        if (m_Attacking)
        {
            if (m_AttackDuration > 0)
            {
                m_AttackDuration -= Time.deltaTime;
            } else
            {
                m_Attacking = false;
                m_Swimming = false;
                m_AttackIsDone = true;

                transform.localRotation = Quaternion.Euler(0, 180, 0); // flip on himself
                transform.Rotate(new Vector3(0, 0, -20)); // rotate to the bottom

                m_SharkHoleSprite.enabled = true; // show hole
               // Debug.Log("fin de l'attaque");
            }
        }

        if (m_AttackIsDone)
        {
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(transform.position.x - 70, transform.position.y - 100, transform.position.z), m_Speed*2 * Time.deltaTime);
        }
    }

    //private void OnCollisionEnter2D(Collision2D collision)
    //{
    //    if (collision.gameObject.name == "Boat" && m_Swimming)
    //    {
    //        biteBoatStructure();
    //    }
    //}

    private void OnTriggerEnter2D(Collider2D collision)
    {
      //  Debug.Log(collision);
        if (collision.gameObject.tag == "Boat" && m_Swimming)
        {
            biteBoatStructure();
        }
    }
}