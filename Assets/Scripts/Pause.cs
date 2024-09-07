using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Pause : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Button>().onClick.AddListener(pausePlay);
        Time.timeScale = Time.timeScale;
    }

    // Update is called once per frame
    public void pausePlay()
    {
       
        if (Time.timeScale == 0.0f)
        {
            Debug.Log("timeScale is set to play\n");
            Time.timeScale = 1.0f;
            GetComponent<Button>().GetComponentInChildren<Text>().text = "Pause";

        }
        else if (Time.timeScale != 0.0f)
        {
            Debug.Log("timeScale is set to pause\n");
            Time.timeScale = 0.0f;
            GetComponent<Button>().GetComponentInChildren<Text>().text = "Play";

        }
        else
        {
            Debug.Log("Pause play has failed");
        }

    }
}
