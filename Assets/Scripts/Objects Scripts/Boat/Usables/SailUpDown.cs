using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SailUpDown : MonoBehaviour
{
    [SerializeField]
    private GameObject Sail1;
    [SerializeField]
    private GameObject Sail2;

    private int ID;

    private Usable usableScript;
    private bool isUsed;

    public static float sailSize = 1;
    // Start is called before the first frame update
    void Start()
    {
        usableScript = this.GetComponent<Usable>();

        objectID objectID = this.GetComponent<objectID>();
        ID = objectID.myID;

        UsableEventManager.current.onUp += sailsUp;
        UsableEventManager.current.onDown += sailsDown;



    }

    // Update is called once per frame
    void Update()
    {

        isUsed = usableScript.isUsed;

        if (isUsed)
        {
            Sail1.transform.localScale = new Vector3(1, sailSize, 1);
            Sail2.transform.localScale = new Vector3(1, sailSize, 1);
        }
    }



    private void sailsUp(int id)
    {
        if (id == this.ID && sailSize > 0.1f)
        {
            sailSize -= 0.002f;

        }

    }
    private void sailsDown(int id)
    {
        if (id == this.ID && sailSize < 1)
        {
            sailSize += 0.002f;

        }

    }
}
