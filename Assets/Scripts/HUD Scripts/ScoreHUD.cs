using System.Collections;
using System.Collections.Generic;
using SDD.Events;
using UnityEngine;
using UnityEngine.UI;

public class ScoreHUD : MonoBehaviour
{

    public float gameScore;

    // Start is called before the first frame update
    void Start()
    {
        gameScore = 5000;
    }

    // Update is called once per frame
    void Update()
    {
        gameScore -= Time.deltaTime * 5;
        this.GetComponent<Text>().text =((int) gameScore).ToString();
    }
}
