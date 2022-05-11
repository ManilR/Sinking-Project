using SDD.Events;
using UnityEngine;

public class BoatMovement : MonoBehaviour
{
    private const float MAX_SPEED = 5;
    [SerializeField]
    public float BASE_SPEED;

    public float movementSpeed;

    [SerializeField]
    private LayerMask whatIsWater;

    private bool isMovingForward;
    private bool isOnWater;

    private Vector2 newVelocity;

    private Rigidbody2D rb;
    private float timer;

    [SerializeField]
    private GameObject sailHole;
    [SerializeField]
    private Transform sail;

    private Vector3 initPos;

    private void OnEnable()
    {
        EventManager.Instance.AddListener<ResetMapEvent>(ResetMapEventCallback);
    }

    private void OnDisable()
    {
        EventManager.Instance.RemoveListener<ResetMapEvent>(ResetMapEventCallback);
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        checkMoving();
        movement();

        movementSpeed = BASE_SPEED * sail.localScale.y;

        if (sailHole.GetComponent<SpriteRenderer>().enabled == true)
            movementSpeed = BASE_SPEED / 2;
    }
    private void checkMoving()
    {
        isOnWater = Physics2D.OverlapCircle(this.transform.position, 0.38f, whatIsWater); 
    }
    private void movement()
    {
        float speedY = 0;
        if ((int)timer % 2 == 0)
            speedY = 1;
        else
            speedY = -1;
        newVelocity.Set(movementSpeed, speedY);
        rb.velocity = newVelocity;
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "End")
            EventManager.Instance.Raise(new NewEventEvent() { EventName = "VICTORY" });
    }

    void ResetMapEventCallback(ResetMapEvent e)
    {
        rb = GetComponent<Rigidbody2D>();
        movementSpeed = BASE_SPEED;

        if(initPos != Vector3.zero)
        {
            rb.transform.position = initPos;
        }
        else
        {
            initPos = rb.transform.position;
        }
        // hide the seagull hole by default
        sailHole.GetComponent<SpriteRenderer>().enabled = false;
    }
}
