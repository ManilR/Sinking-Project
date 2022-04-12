using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class objectID : MonoBehaviour
{
    public static int nextID = 0;

    public int myID;

    //Events

    void Awake()
    {

        myID = nextID;
        nextID++;
    }

    
}
