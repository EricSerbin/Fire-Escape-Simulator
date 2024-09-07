using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpeedDown : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Button>().onClick.AddListener(speedDown);
    }

    // Update is called once per frame
    void speedDown()
    {
        if(Time.timeScale>1.0f)
        {
            Time.timeScale = Time.timeScale - 1.0f;
        }
    }
}
