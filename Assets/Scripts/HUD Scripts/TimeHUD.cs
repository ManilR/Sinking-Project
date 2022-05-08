using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeHUD : MonoBehaviour
{
    private float timer;
    public int timeSeconds;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        timeSeconds = (int) timer;
        int minutes = timeSeconds / 60;
        int seconds = timeSeconds % 60;
        string minStr = minutes.ToString();
        if (minutes < 10)
            minStr = "0" + minStr;
        string secStr = seconds.ToString();
        if (seconds < 10)
            secStr = "0" + secStr;
        string str = minStr + " : " + secStr;
        this.GetComponent<Text>().text = str;
    }
}
