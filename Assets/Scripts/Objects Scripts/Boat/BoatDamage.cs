using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoatDamage : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]private Transform HoleParent;

    private List<Transform> holes = new List<Transform>();

    [SerializeField] private int MAX_HEALTH;
    public int health = 0;

    void Start()
    {
        health = MAX_HEALTH;
        foreach(Transform hole in HoleParent)
        {
            if(hole.tag == "Hole")
            {
                hole.GetComponentInChildren<SpriteRenderer>().enabled = false;
                holes.Add(hole);
            }
            
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Canon Ball")
        {
            HullDamage();
        }
    }

    private void HullDamage()
    {
        health--;
        int HolePos = Random.Range(0, 9);
        holes[HolePos].GetComponentInChildren<SpriteRenderer>().enabled = true;
    }

}
