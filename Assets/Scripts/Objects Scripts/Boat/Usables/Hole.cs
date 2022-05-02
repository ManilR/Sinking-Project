using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hole : MonoBehaviour
{
    private int ID;

    private Usable usableScript;
    private bool isUsed;

    private float fixing = 0;

    private BoxCollider2D boxCollider;
    private SpriteRenderer sprite;
    // Start is called before the first frame update
    void Start()
    {
        usableScript = this.GetComponent<Usable>();

        objectID objectID = this.GetComponent<objectID>();
        ID = objectID.myID;

        UsableEventManager.current.onAction += fix;

        boxCollider = gameObject.GetComponentInChildren<BoxCollider2D>();
        sprite = gameObject.GetComponentInChildren<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {

        isUsed = usableScript.isUsed;

        if (isUsed)
        {
        }
        else if(fixing > 0)
        {
            fixing -= 0.03f;
        }
        if (fixing < 0)
            fixing = 0;

        if(sprite.enabled == false)
        {
            boxCollider.enabled = false;
        }
        else
            boxCollider.enabled = true;


    }



    private void fix(int id)
    {
        Debug.Log(id);
        if (id == this.ID)
        {
            fixing += 1;
            Debug.Log(fixing);
            if (fixing >= 10f)
            {
                Debug.Log("eo");
                this.GetComponentInParent<BoatDamage>().health++;
                sprite.enabled = false;
                fixing = 0;
            }
                
        }

    }

    private void OnDestroy()
    {
        
    }


}
