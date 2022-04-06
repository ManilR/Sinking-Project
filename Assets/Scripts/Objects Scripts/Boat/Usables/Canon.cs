using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Canon : MonoBehaviour
{
    InputUsable inputManager;
    Transform pivot;
    string input;
    // Start is called before the first frame update
    void Start()
    {
        inputManager = gameObject.GetComponent<InputUsable>();
        pivot = gameObject.transform.GetChild(1);
    }

    // Update is called once per frame
    void Update()
    {
        if (inputManager.getInput() != null)
        {
            Debug.Log(inputManager.getInput());
            input = inputManager.getInput();
        }
        else
            input = null;

        if (input == "Up" && pivot.rotation.z < 80)
            pivot.Rotate(new Vector3(0,0,1), 0.5f);
        if (input == "Down" && pivot.rotation.z > 15)
            pivot.Rotate(new Vector3(0, 0, 1), -0.5f);
    }

    
}
