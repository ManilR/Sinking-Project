using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boatdamaging : MonoBehaviour
{
    public int health = 3;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void HullDamage(int level)
    {
        health--;

    }
}
