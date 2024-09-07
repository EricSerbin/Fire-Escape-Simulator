using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpeedUp : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Button>().onClick.AddListener(speedUp);

    }

    // Update is called once per frame
    void speedUp()
    {
        Time.timeScale = Time.timeScale + 1.0f;
    }
}
