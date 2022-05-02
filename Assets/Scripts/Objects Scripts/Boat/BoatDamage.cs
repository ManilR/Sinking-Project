using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoatDamage : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]private Transform Holes;
    void Start()
    {
        foreach(Transform hole in Holes)
        {
            if(hole.tag == "Hole")
            {
                hole.GetComponentInChildren<SpriteRenderer>().enabled = false;
            }
            
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
