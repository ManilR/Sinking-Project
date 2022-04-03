using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoatMovement : MonoBehaviour
{
    [SerializeField]
    private float movementSpeed;

    [SerializeField]
    private LayerMask whatIsWater;

    private bool isMovingForward;
    private bool isOnWater;

    private Vector2 newVelocity;

    private Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        checkMoving();
        movement();
    }
    private void checkMoving()
    {
        isOnWater = Physics2D.OverlapCircle(this.transform.position, 0.38f, whatIsWater); 
    }
    private void movement()
    {
        
            newVelocity.Set(movementSpeed, 0.0f);
            rb.velocity = newVelocity;
        
    }
}
