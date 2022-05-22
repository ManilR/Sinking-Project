using SDD.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    #region Variables
    [SerializeField]
    private int id ;
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

    [SerializeField]
    private PhysicsMaterial2D material;

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
    private bool isInWater;
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

    private Vector3 initPos;
    #endregion

    private void OnEnable()
    {
        EventManager.Instance.AddListener<ResetMapEvent>(ResetMapEventCallback);
    }

    private void OnDisable()
    {
        EventManager.Instance.RemoveListener<ResetMapEvent>(ResetMapEventCallback);
    }

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
        if (!GameManager.Instance.IsPlaying) return;
        CheckInput();
    }

    private void FixedUpdate()
    {
        CheckGround();
        CheckOnBoat();
        SlopeCheck();
        ApplyMovement();
    }

    private void CheckInput()
    {
        xInput = Input.GetAxisRaw("Horizontal_"+id); // Get axis input depending on Player id

        yInput = Input.GetAxisRaw("Vertical_" + id);

        if (xInput == 1 && facingDirection == -1 && !isActing)
        {
            Flip();
        }
        else if (xInput == -1 && facingDirection == 1 && !isActing)
        {
            Flip();
        }

        if (Input.GetButtonDown("Jump_" + id) && !isActing)
        {
            Jump();
        }

        if (isActing )
        {
            if(Input.GetButtonDown("Fire_" + id))
            {
                isActing = false;
                usableScript.isUsed = false;
            }
                
            else
            {
                if (Input.GetButtonDown("Jump_" + id))
                {
                    UsableEventManager.current.TriggerAction(usableID);
                    FindObjectOfType<AudioManager>().Play("PlankRepair");
                }
                if (yInput == 1)
                    UsableEventManager.current.TriggerUp(usableID);
                if (yInput == -1)
                    UsableEventManager.current.TriggerDown(usableID);
                if (xInput == 1)
                    UsableEventManager.current.TriggerRight(usableID);
                if (xInput == -1)
                    UsableEventManager.current.TriggerLeft(usableID);
            }
        }
        else if (usable != null && !isActing && Input.GetButtonDown("Fire_" + id) )
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
        }
        if (col.gameObject.name == "OnBoatCheck")
            isOnBoat = true;
    }
    void OnTriggerExit2D(Collider2D col)
    {
        if(col.tag == "Usable")
            usable = null;


        if (col.gameObject.name == "OnBoatCheck")
            isOnBoat = false;
    }


    private void CheckGround()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, whatIsGround);
        isOnLadder = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, whatIsLadder);
        isOnUsable = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, whatIsUsable);

        
        if (rb.velocity.y <=  0.0f)
        {
            isJumping = false;
        }

        if(isGrounded && !isJumping)
        {
            canJump = true;
        }

    }

    private void CheckOnBoat()
    {
        if (isOnBoat)
        {
            Physics2D.IgnoreCollision(cc, water.GetComponent<BoxCollider2D>());
        }
        else
        {
            Physics2D.IgnoreCollision(cc, water.GetComponent<BoxCollider2D>(), false);
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
        canWalkOnSlope = true;

        //if (isOnSlope)
        //{
        //    boat.GetComponent<Rigidbody2D>().sharedMaterial = null;
        //}
        //else
        //{
        //    boat.GetComponent<Rigidbody2D>().sharedMaterial = material;
        //}
        
        if (isGrounded && !isOnSlope && !isJumping && !isOnLadder) //if not on slope
        {
            Debug.Log("This one");
            newVelocity.Set(movementSpeed * xInput, 0.0f);

        }
        else if (isGrounded && isOnSlope && canWalkOnSlope && !isJumping && !isOnLadder) //If on slope
        {
            newVelocity.Set(movementSpeed * slopeNormalPerp.x * -xInput, movementSpeed * slopeNormalPerp.y * -xInput);
            //boat.GetComponent<Rigidbody2D>().sharedMaterial.friction = 0;
        }
        else if (!isGrounded) //If in air
        {
            newVelocity.Set(movementSpeed * xInput, rb.velocity.y);

        }
        if (isOnLadder && !isOnSlope && !isJumping) //if not on slope
        {
            //rb.gravityScale = 0;
            newVelocity.Set(movementSpeed * xInput, movementSpeed * yInput);
        }
        else
        {
            rb.gravityScale = baseGravityScale;
        }

        animator.SetFloat("Speed", Mathf.Abs(newVelocity.x));

        //if (isActing)
        //{
        //    newVelocity.Set(-0.75f, 0);
        //}

        if (isOnBoat)
        {
            //Debug.Log("testBoat");
            float newX = newVelocity.x + rbBoat.velocity.x;
            float newY = newVelocity.y + rbBoat.velocity.y;
            newVelocity.Set(newX, newY);
        }
        rb.velocity = newVelocity;

        if (isActing)
        {
            if(usable !=null)
                this.transform.position = new Vector2( usable.transform.position.x, this.transform.position.y);
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

    void ResetMapEventCallback(ResetMapEvent e)
    {
        if (initPos != Vector3.zero)
        {
            rb.transform.position = initPos;
        }
        else
        {
            initPos = rb.transform.position;
        }
    }
}

