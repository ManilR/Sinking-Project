using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Progression : MonoBehaviour
{
    [SerializeField] private Transform Boat;
    [SerializeField] private Transform Water;

    private float waterSize;
    // Start is called before the first frame update
    void Start()
    {
        WaterWaves2D waterScript = Water.GetComponent<WaterWaves2D>();
        waterSize = waterScript.bounds.size.x;

    }

    // Update is called once per frame
    void Update()
    {
        

        float boatPos = Boat.position.x;
        float newPos = boatPos / waterSize * 1920;

        
        this.transform.position = new Vector3(newPos, this.transform.position.y, this.transform.position.z); 
    }
}
