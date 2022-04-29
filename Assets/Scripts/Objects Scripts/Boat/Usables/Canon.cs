using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Canon : MonoBehaviour
{
    private const int NB_TRAJECTORY_POINTS = 5;

    private int ID;

    [SerializeField]
    private Transform FirePoint;
    [SerializeField]
    private GameObject CanonBall;
    [SerializeField]
    private LineRenderer Line;

    private Vector3 initialVelocity;
    private Transform pivot;

    private Usable usableScript;
    private bool isUsed;
    
    private float power = 15;
    // Start is called before the first frame update
    void Start()
    {
        usableScript = this.GetComponent<Usable>();

        objectID objectID = this.GetComponent<objectID>();
        ID = objectID.myID;
        pivot = gameObject.transform.GetChild(1);
        Line.positionCount = NB_TRAJECTORY_POINTS;

        UsableEventManager.current.onAction += shoot;
        UsableEventManager.current.onUp += goUp;
        UsableEventManager.current.onDown += goDown;
        UsableEventManager.current.onRight += addPower;
        UsableEventManager.current.onLeft += reducePower;


    }

    // Update is called once per frame
    void Update()
    {

        isUsed = usableScript.isUsed;

        if (isUsed)
        {
            Line.enabled = true;
            initialVelocity = (FirePoint.position - pivot.position) * power;
            float g = Physics.gravity.magnitude;
            float velocity = initialVelocity.magnitude;
            float angle = Mathf.Atan2(initialVelocity.y, initialVelocity.x);

            Vector3 start = FirePoint.position;

            float timeStep = 0.1f;
            float fTime = 0f;
            for (int i = 0; i < NB_TRAJECTORY_POINTS; i++)
            {
                float dx = velocity * fTime * Mathf.Cos(angle);
                float dy = velocity * fTime * Mathf.Sin(angle) - (g * fTime * fTime / 2f);
                Vector3 pos = new Vector3(start.x + dx, start.y + dy, 0);
                Line.SetPosition(i, pos);
                fTime += timeStep;
            }
        }
        else
        {
            Line.enabled = false;
        }
        
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
            {
                Debug.Log(power);
                power -= 0.03f;
            }
        }
    }


}
