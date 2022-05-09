using SDD.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoatDamage : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]private Transform HoleParent;

    private List<Transform> holes = new List<Transform>();

    [SerializeField] public float MAX_HEALTH;
    public float health = 0;

    private int nbHoles = 0;
    private float damageTimer;
    private float damageTic; // 1dmg/sec
    void Start()
    {
        health = MAX_HEALTH;
        foreach(Transform hole in HoleParent)
        {
            if(hole.tag == "Hole" && hole.name != "SharkBite")
            {
                hole.GetComponentInChildren<SpriteRenderer>().enabled = false;
                holes.Add(hole);
                nbHoles++;
            }
            
        }
    }

    // Update is called once per frame
    void Update()
    {
        damageTimer += Time.deltaTime;
        damageTic += Time.deltaTime;

        if(damageTic >= 1f)
        {
            damageTic = 0;
            foreach (Transform hole in holes)
            {
                if (hole.GetComponentInChildren<SpriteRenderer>().enabled == true)
                {
                    health--;
                }

            }
        }

        if (health <= 0)
        {
            EventManager.Instance.Raise(new SetStateGameoverEvent());
        }

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Canon Ball" && collision.relativeVelocity.sqrMagnitude > 100)
        {
            HullDamage();
        }
    }

    private void HullDamage()
    {

        if(damageTimer > 0.1)
        {
            damageTimer = 0;
            health--;
            int HolePos = Random.Range(0, nbHoles-1);
            for (int i = 0; i < nbHoles; i++)
            {
                if (holes[HolePos].GetComponentInChildren<SpriteRenderer>().enabled == false)
                {
                    holes[HolePos].GetComponentInChildren<SpriteRenderer>().enabled = true;
                    break;
                }
                if (HolePos == nbHoles -1)
                    HolePos = 0;
                else
                    HolePos++;

            }
        }
        
        
    }

}
