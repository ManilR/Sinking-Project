using SDD.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoatMovement : MonoBehaviour
{
    private const float MAX_SPEED = 5;
    [SerializeField]
    public float movementSpeed;

    [SerializeField]
    private LayerMask whatIsWater;

    private bool isMovingForward;
    private bool isOnWater;

    private Vector2 newVelocity;

    private Rigidbody2D rb;
    private float timer;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        checkMoving();
        movement();
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
}
