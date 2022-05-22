using SDD.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoatDamage : MonoBehaviour
{
    [SerializeField]private Transform HoleParent;

    private List<Transform> holes = new List<Transform>();

    [SerializeField] public float MAX_HEALTH;
    public float initHealth = 0;
    public float health = 0;

    private int nbHoles = 0;
    private float damageTimer;
    private float damageTic; // 1dmg/sec

    private void OnEnable()
    {
        EventManager.Instance.AddListener<ResetMapEvent>(ResetMapEventCallback);
    }

    private void OnDisable()
    {
        EventManager.Instance.RemoveListener<ResetMapEvent>(ResetMapEventCallback);
    }

    void Start()
    {
        initHealth = MAX_HEALTH;
        initHealth *= GameManager.Instance.gameLevelCoef;
        health = initHealth;
        foreach(Transform hole in HoleParent)
        {
            if(hole.name != "SharkBite")
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
            foreach (Transform hole in HoleParent)
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

    void ResetMapEventCallback(ResetMapEvent e)
    {
        health = MAX_HEALTH;
        holes.ForEach((h) => h.GetComponentInChildren<SpriteRenderer>().enabled = false);
    }
}
