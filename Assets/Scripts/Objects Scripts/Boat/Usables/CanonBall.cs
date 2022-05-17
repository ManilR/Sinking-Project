using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanonBall : MonoBehaviour
{
    private CircleCollider2D collider;
    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, 10);
        collider = gameObject.GetComponent<CircleCollider2D>();
        //collider.isTrigger = true;
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag != "Player")
            Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Hull")
        {
            collider.isTrigger = false;
        }
    }
}
