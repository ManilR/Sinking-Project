﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    [SerializeField]
    private float movementSpeed;
    [SerializeField]
    private float groundCheckRadius;
    [SerializeField]
    private float jumpForce;
    [SerializeField]
    private float slopeCheckDistance;
    [SerializeField]
    private Transform groundCheck;
    [SerializeField]
    private LayerMask whatIsGround;
    [SerializeField]
    private LayerMask whatIsLadder;
    [SerializeField]
    private LayerMask whatIsWater;
    [SerializeField]
    private LayerMask whatIsUsable;
    [SerializeField]
    private GameObject boat;
    [SerializeField]
    private GameObject water;

    private float xInput;
    private float yInput;
    private float slopeDownAngle;
    private float baseGravityScale;

    private float lastSlopeAngle;

    private int facingDirection = 1;

    private bool isGrounded;
    private bool isOnSlope;
    private bool isJumping;
    private bool isOnUsable;
    private bool isActing;
    private bool isOnLadder;
    private bool isOnBoat;
    private bool canWalkOnSlope;
    private bool canJump;
    

    private Vector2 newVelocity;
    private Vector2 newForce;
    private Vector2 capsuleColliderSize;

    private Vector2 slopeNormalPerp;

    private Rigidbody2D rb;
    private Rigidbody2D rbBoat;
    private CapsuleCollider2D cc;

    private GameObject usable;
    private int usableID;
    private Usable usableScript;

    public Animator animator;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rbBoat = boat.GetComponent<Rigidbody2D>();
        cc = GetComponent<CapsuleCollider2D>();

        capsuleColliderSize = cc.size;
        baseGravityScale = rb.gravityScale;

        Physics2D.IgnoreCollision(cc, water.GetComponent<BoxCollider2D>());
    }

    private void Update()
    {
        CheckInput();     
    }

    private void FixedUpdate()
    {
        CheckGround();
        SlopeCheck();
        ApplyMovement();
    }

    private void CheckInput()
    {
        xInput = Input.GetAxisRaw("Horizontal");
        yInput = Input.GetAxisRaw("Vertical");

        if (xInput == 1 && facingDirection == -1 && !isActing)
        {
            Flip();
        }
        else if (xInput == -1 && facingDirection == 1 && !isActing)
        {
            Flip();
        }

        if (Input.GetButtonDown("Jump") && !isActing)
        {
            Jump();
        }

        if (isActing )
        {
            if((Input.GetButtonDown("Fire1") || Input.GetButtonDown("Fire2")))
            {
                isActing = false;
                usableScript.isUsed = false;
            }
                
            else
            {
                if (Input.GetButtonDown("Jump"))
                    UsableEventManger.current.TriggerAction(usableID);
                if (yInput == 1)
                    UsableEventManger.current.TriggerUp(usableID);
                if (yInput == -1)
                    UsableEventManger.current.TriggerDown(usableID);
                if (xInput == 1)
                    UsableEventManger.current.TriggerRight(usableID);
                if (xInput == -1)
                    UsableEventManger.current.TriggerLeft(usableID);
            }
        }
        else if (usable != null && !isActing && Input.GetButtonDown("Fire1") )
        {
            isActing = true;
            usableScript.isUsed = true;
        }
        
        if(usable == null && isActing)
        {
            isActing = false;
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if(col.tag == "Usable" && usable == null)
        {

            usable = col.gameObject;
            objectID objectID = usable.GetComponentInParent<objectID>();
            usableID = objectID.myID;
            usableScript = usable.GetComponentInParent<Usable>();
            Debug.Log(isOnUsable);
        }
        
    }
    void OnTriggerExit2D(Collider2D col)
    {
        usable = null;

    }
    private void CheckGround()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, whatIsGround);
        isOnLadder = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, whatIsLadder);
        isOnUsable = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, whatIsUsable);

        isOnBoat = !Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, whatIsWater);

        if (rb.velocity.y <= 0.0f)
        {
            isJumping = false;
        }

        if(isGrounded && !isJumping)
        {
            canJump = true;
        }

    }


    private void SlopeCheck()
    {
        Vector2 checkPos = transform.position - (Vector3)(new Vector2(0.0f, capsuleColliderSize.y / 2));

        SlopeCheckHorizontal(checkPos);
        SlopeCheckVertical(checkPos);
    }

    private void SlopeCheckHorizontal(Vector2 checkPos)
    {
        RaycastHit2D slopeHitFront = Physics2D.Raycast(checkPos, transform.right, slopeCheckDistance, whatIsGround);
        RaycastHit2D slopeHitBack = Physics2D.Raycast(checkPos, -transform.right, slopeCheckDistance, whatIsGround);

        if (slopeHitFront || slopeHitBack)
        {
            isOnSlope = true;
        }
        else
        {
            isOnSlope = false;
        }

    }

    private void SlopeCheckVertical(Vector2 checkPos)
    {      
        RaycastHit2D hit = Physics2D.Raycast(checkPos, Vector2.down, slopeCheckDistance, whatIsGround);

        if (hit)
        {

            slopeNormalPerp = Vector2.Perpendicular(hit.normal).normalized;            

            slopeDownAngle = Vector2.Angle(hit.normal, Vector2.up);

            if(slopeDownAngle != lastSlopeAngle)
            {
                isOnSlope = true;
            }                       

            lastSlopeAngle = slopeDownAngle;
           
            Debug.DrawRay(hit.point, slopeNormalPerp, Color.blue);
            Debug.DrawRay(hit.point, hit.normal, Color.green);

        }

    }

    private void Jump()
    {
        if (canJump)
        {
            canJump = false;
            isJumping = true;
            newVelocity.Set(0.0f, 0.0f);
            rb.velocity = newVelocity;
            newForce.Set(0.0f, jumpForce);
            rb.AddForce(newForce, ForceMode2D.Impulse);
        }
    }   

    private void ApplyMovement()
    {
        
        if (isGrounded && !isOnSlope && !isJumping) //if not on slope
        {
            Debug.Log("This one");
            newVelocity.Set(movementSpeed * xInput, 0.0f);
            rb.velocity = newVelocity;
            animator.SetFloat("Speed", Mathf.Abs(newVelocity.x));
        }
        else if (isGrounded && isOnSlope && canWalkOnSlope && !isJumping) //If on slope
        {
            newVelocity.Set(movementSpeed * slopeNormalPerp.x * -xInput, movementSpeed * slopeNormalPerp.y * -xInput);
            rb.velocity = newVelocity;
            animator.SetFloat("Speed", Mathf.Abs(newVelocity.x));
        }
        else if (!isGrounded) //If in air
        {
            newVelocity.Set(movementSpeed * xInput, rb.velocity.y);
            rb.velocity = newVelocity;
            animator.SetFloat("Speed", Mathf.Abs(newVelocity.x));
        }
        if (isOnLadder && !isOnSlope && !isJumping) //if not on slope
        {
            rb.gravityScale = 0;
            Debug.Log("This two");
            newVelocity.Set(movementSpeed * xInput, movementSpeed * yInput);
            rb.velocity = newVelocity;
            animator.SetFloat("Speed", Mathf.Abs(newVelocity.x));
        }
        else
        {
            rb.gravityScale = baseGravityScale;
        }

        if (isOnBoat)
        {
            //Debug.Log("testBoat");
            rb.velocity = rb.velocity + rbBoat.velocity;
            animator.SetFloat("Speed", Mathf.Abs(newVelocity.x));
        }

        if (isActing)
        {
            rb.velocity -= newVelocity;
            animator.SetFloat("Speed", Mathf.Abs(newVelocity.x));
        }
    }

    private void Flip()
    {
        facingDirection *= -1;
        transform.Rotate(0.0f, 180.0f, 0.0f);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }

}

