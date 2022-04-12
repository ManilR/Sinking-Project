using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Canon : MonoBehaviour
{
    private int ID;

    Transform pivot;
    string input;
    // Start is called before the first frame update
    void Start()
    {
        objectID objectID = this.GetComponent<objectID>();
        ID = objectID.myID;
        pivot = gameObject.transform.GetChild(1);

        UsableEventManger.current.onAction += shoot;
        UsableEventManger.current.onUp += goUp;
        UsableEventManger.current.onDown += goDown;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void shoot(int id)
    {
        if(id == this.ID)
        {
            Debug.Log("Shoot");
        }
            
    }

    private void goUp(int id)
    {
        if (id == this.ID)
        {
            if (pivot.rotation.eulerAngles.z  <= 80)
                pivot.Rotate(new Vector3(0, 0, 1), 0.5f);
        }
    }
    private void goDown(int id)
    {
        if (id == this.ID)
        {
            if (pivot.rotation.eulerAngles.z >= 15)
                pivot.Rotate(new Vector3(0, 0, 1), -0.5f);
        }
    }


}
