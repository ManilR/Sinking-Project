using SDD.Events;
using UnityEngine;

public class SeagullController : MonoBehaviour
{
    static readonly string EventName = "SEAGULL";

    [SerializeField] public GameObject m_SeagullHole;
    [SerializeField] public float m_Speed;
    [SerializeField] public GameObject m_Boat;
    [SerializeField] public float m_AttackDuration = 3;
    [SerializeField] public Transform m_Sail;

    private SpriteRenderer m_SeagullHoleSprite;
    private BoatMovement m_BoatMovement;
    private bool m_Flying = true;
    private bool m_Attacking = false;
    private bool m_AttackIsDone = false;

    // Start is called before the first frame update
    public void StartEvent()
    {
        m_Flying = true;
        m_Attacking = false;
        m_AttackIsDone = false;

        // Hide the hole by default
        m_SeagullHoleSprite = m_SeagullHole.GetComponent<SpriteRenderer>();
        m_SeagullHoleSprite.enabled = false;

        m_BoatMovement = m_Boat.GetComponent<BoatMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        // freeze rotation
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, 0, transform.eulerAngles.z);

        proceedAttackOnBoat();
        tearSail();

        if (m_AttackIsDone)
        {
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(transform.position.x - 100, transform.position.y, transform.position.z), m_Speed * 2 * Time.deltaTime);
            Destroy(gameObject);
        }
    }

    void tearSail()
    {
        if (Vector2.Distance(transform.position, m_SeagullHole.transform.position) < 1.5f && m_Attacking)
        {
            Debug.Log("ça va bouffer un petit bout de voile");
            transform.position = m_SeagullHole.transform.position;

            if (m_AttackDuration > 0)
            {
                m_AttackDuration -= Time.deltaTime;
            }
            else
            {
                m_Attacking = false;
                m_Flying = true;

                m_AttackIsDone = true;
                m_SeagullHoleSprite.enabled = true; // show hole
            }
        }
    }

    void proceedAttackOnBoat()
    {
        if (m_Flying && !m_Attacking)
        {
            var step = m_Speed * Time.deltaTime; // calculate distance to move
            transform.position = Vector3.MoveTowards(transform.position, m_SeagullHole.transform.position, step);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log(collision);

        if (collision.name == "MainSailSprite")
        {
            m_Flying = false;
            m_Attacking = true;
            //m_Speed = 1;

            /*

            Vector2 newVelocity = Vector2.zero;
            
            var math = m_BoatMovement.movementSpeed * m_BoatMovement.movementSpeed * Mathf.Pow(10, -1) * -1;
            newVelocity.Set(math, 0.0f); // a contrary force
            this.GetComponent<Rigidbody2D>().velocity = newVelocity;*/
        }
        
    }
}
