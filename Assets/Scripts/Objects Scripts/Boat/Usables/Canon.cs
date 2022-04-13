using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Canon : MonoBehaviour
{
    private int ID;
    [SerializeField]
    private Transform FirePoint;
    [SerializeField]
    private GameObject CanonBall;

    private Vector3 initialVelocity;
    private Transform pivot;

    private float power = 15;
    // Start is called before the first frame update
    void Start()
    {
        objectID objectID = this.GetComponent<objectID>();
        ID = objectID.myID;
        pivot = gameObject.transform.GetChild(1);

        UsableEventManger.current.onAction += shoot;
        UsableEventManger.current.onUp += goUp;
        UsableEventManger.current.onDown += goDown;
        UsableEventManger.current.onRight += addPower;
        UsableEventManger.current.onLeft += reducePower;

        //initialVelocity = new Vector3(5, 0, 0);


    }

    // Update is called once per frame
    void Update()
    {

        
    }

    private void shoot(int id)
    {
        
        if(id == this.ID)
        {
            initialVelocity = (FirePoint.position - pivot.position) * power;
            // instantiate a cannon ball
            GameObject cannonBall = Instantiate(CanonBall,
                FirePoint.position, Quaternion.identity);
            // apply some force
            Rigidbody2D rb = cannonBall.GetComponent<Rigidbody2D>();
            rb.AddForce(initialVelocity, ForceMode2D.Impulse);
        }
             
    }

    private void goUp(int id)
    {
        if (id == this.ID)
        {
            if (pivot.rotation.eulerAngles.z  <= 80)
            {
                pivot.Rotate(new Vector3(0, 0, 1), 0.5f);
            }
                
        }
    }
    private void goDown(int id)
    {
        if (id == this.ID)
        {
            if (pivot.rotation.eulerAngles.z >= 15)
            {
                pivot.Rotate(new Vector3(0, 0, 1), -0.5f);
            }
                
        }
    }

    private void addPower(int id)
    {
        if (id == this.ID)
        {
            if (power < 20)
            {
                Debug.Log(power);
                power += 0.03f;
            }

        }
    }

    private void reducePower(int id)
    {
        if (id == this.ID)
        {
            if (power > 5)
                Debug.Log(power);
                power -= 0.03f;
            }

            {
        }
    }


}
