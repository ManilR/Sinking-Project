using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HighScoreHUD : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        int highscore = 0;
        highscore = PlayerPrefs.GetInt("highscore", highscore);
        this.GetComponent<Text>().text = "HIGHSCORE : " + highscore.ToString();
    }
}
