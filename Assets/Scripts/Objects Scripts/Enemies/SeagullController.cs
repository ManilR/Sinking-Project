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
    private bool m_Flying = true;
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

    // Start is called before the first frame update
    public void Start()
    {
        m_Flying = true;
        m_Attacking = false;
        m_AttackIsDone = false;

        // Hide the hole by default
        m_SeagullHoleSprite = m_SeagullHole.GetComponent<SpriteRenderer>();
        m_SeagullHoleSprite.enabled = false;
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
        }
    }

    void tearSail()
    {
        if (m_Sail.transform.localScale.y > 0.5f){ // if sail is down, attack
        
            if (Vector2.Distance(transform.position, m_SeagullHole.transform.position) < 2f && m_Attacking)
            {
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
        } else
        {
            // seagull runs out of shame
            m_AttackIsDone = true;
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
        if (collision.name == "MainSailSprite")
        {
            m_Flying = false;
            m_Attacking = true;
        }
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Canon Ball")
        {
            Destroy(gameObject);
        }
    }

    void ResetMapEventCallback(ResetMapEvent e)
    {
        Destroy(gameObject);
    }
}
