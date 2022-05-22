using SDD.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeHUD : MonoBehaviour
{
    public int timeSeconds;

    // Update is called once per frame
    void Update()
    {

        timeSeconds = (int) GameManager.Instance.timer;
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
