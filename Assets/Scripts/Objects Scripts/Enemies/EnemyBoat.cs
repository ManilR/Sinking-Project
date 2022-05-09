using SDD.Events;
using UnityEngine;

public class EnemyBoat : MonoBehaviour
{
    static readonly string EventName = "ENEMY_BOAT";

    [SerializeField]
    public float movementSpeed;
    private Vector2 newVelocity;

    [SerializeField]
    public GameObject target;
    [SerializeField]
    public GameObject water;
    private Rigidbody2D rb;
    private Rigidbody2D rbTarget;

    private bool isOnPos = false;

    private float health = 5;
    private float shootCD = 0;
    private float pivotCD = 0;
    private bool pivotUP = true;

    private Canon canonScript;
    private objectID canonID;

    public float movCD = 0;
    // Start is called before the first frame update
    void Start()
    {
        Physics2D.IgnoreCollision(GetComponentInChildren<BoxCollider2D>(), water.GetComponent<BoxCollider2D>());
        isOnPos = false;
        health = 5;
        shootCD = 0;
        pivotCD = 0;
        pivotUP = true;
        canonScript = GetComponentInChildren<Canon>();
        canonID = GetComponentInChildren<objectID>();
        rb = GetComponent<Rigidbody2D>();
        rbTarget = target.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        shootCD += Time.deltaTime;
        pivotCD += Time.deltaTime;

        if(shootCD > 8)
        {
            UsableEventManager.current.TriggerAction(canonID.myID);
            shootCD = 0;
        }
        if(pivotCD > 3.5)
        {
            pivotCD = 0;
            pivotUP = !pivotUP;
        }
        if (pivotUP)
            UsableEventManager.current.TriggerUp(canonID.myID);
        else
            UsableEventManager.current.TriggerDown(canonID.myID);


        movement();
        if (this.transform.rotation.eulerAngles.z < 1)
            this.transform.Rotate(new Vector3(0, 0, 1), 0.25f);
        else if (this.transform.rotation.eulerAngles.z > -1)
            this.transform.Rotate(new Vector3(0, 0, 1), -0.25f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Boat")
        {
            isOnPos = true;
        }
    }
    //private void OnTriggerExit2D(Collider2D collision)
    //{
    //    if (collision.gameObject.tag == "Boat")
    //    {
    //        isOnPos = false;
    //    }
    //}

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Canon Ball")
        {
            health--;
        }

        if (health == 0)
        {
            Destroy(gameObject);
        }  
    }

    private void movement()
    {
        float speedY = 0;
        if (isOnPos)
        {
            if ((int)movCD % 2 == 0)
                speedY = 1f;
            else
                speedY = -1f;
        }
        
        if (!isOnPos)
        {
            newVelocity.Set(movementSpeed * -1, speedY);
            
        }
        else
        {
            movCD += Time.deltaTime;
            //rb.velocity = rbTarget.velocity;
            if (movCD < 4f)
                newVelocity.Set(rbTarget.velocity.x * 0f, speedY);
            else
                newVelocity.Set(rbTarget.velocity.x * 1.25f, speedY);
            if (movCD > 8f)
                movCD = 0;
        }
        rb.velocity = newVelocity;

    }
}
