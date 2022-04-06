using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputUsable : MonoBehaviour
{
    private string input;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        input = null;
    }

    public void setInput(string i)
    {
        input = i;
    }

    public string getInput()
    {
        return input;
    }
}
