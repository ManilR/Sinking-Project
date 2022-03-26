using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementInput : MonoBehaviour
{
    [SerializeField] float m_TranslationSpeed;
    private Rigidbody2D m_rigidbody;

    private void Awake()
    {
        m_rigidbody = GetComponent<Rigidbody2D>();
        Debug.Log("test");
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        Debug.Log("testUpdte");
        float vInput = Input.GetAxis("Vertical");
        float hInput = Input.GetAxis("Horizontal");
        //vInput = 1;
        //hInput = -1;
        Vector2 targetVelocity = vInput * m_TranslationSpeed * transform.up; //* Vector2.ProjectOnPlane(transform.forward, Vector2.up).normalized;
        Vector2 velocityChange = targetVelocity - m_rigidbody.velocity;
        m_rigidbody.AddForce(velocityChange);

        Vector2 forward = new Vector2(1, 0.85f).normalized;
       
        
        targetVelocity = hInput * m_TranslationSpeed * forward; //* Vector2.ProjectOnPlane(transform.forward, Vector2.up).normalized;
        velocityChange = targetVelocity - m_rigidbody.velocity;
        m_rigidbody.AddForce(velocityChange);//, ForceMode2D.Force
    }
    private void Update()
    {
        
    }
}
