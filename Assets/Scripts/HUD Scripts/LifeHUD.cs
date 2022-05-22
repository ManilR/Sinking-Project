using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifeHUD : MonoBehaviour
{
    [SerializeField] private Transform Boat;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.localScale = new Vector3(
            Boat.GetComponent<BoatDamage>().health / Boat.GetComponent<BoatDamage>().initHealth, 1, 1);
    }
}
