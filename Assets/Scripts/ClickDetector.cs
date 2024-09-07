using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ClickDetector : MonoBehaviour
{
    // Start is called before the first frame update
    
    void OnMouseDown()
    {
        NodeView nodeView = GetComponent<NodeView>();
        
        nodeView.ColorNode(Color.cyan);
        
        Debug.Log("Cell was toggled");
    }

}
