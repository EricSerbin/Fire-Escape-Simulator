using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClearCells : MonoBehaviour
{
    // Start is called before the first frame update
    public Graph my_graph;
    public PathFinder my_pathFinder;

    void Start()
    {
        GetComponent<Button>().onClick.AddListener(randomize);
        

    }

    // Update is called once per frame
    void randomize()
    {
        Debug.Log("graph width is "+ my_graph.getWidth() + "and graph height is "+ my_graph.getHeight() + "\n");
        
        for (int i = 0; i < my_graph.getWidth(); i++)
        {
            for (int j = 0; j < my_graph.getHeight(); j++)
            {
                my_graph.nodes[i, j].nodeType = NodeType.Open;
            }
            Debug.Log("\n");
        }
        my_pathFinder.ShowColors();
    }
}
