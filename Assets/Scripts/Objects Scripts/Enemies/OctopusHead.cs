using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OctopusHead : MonoBehaviour
{
    [SerializeField] private GameObject octopus;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Canon Ball")
        {
            octopus.GetComponent<OctopusController>().health -= 5;
        }
    }
}
