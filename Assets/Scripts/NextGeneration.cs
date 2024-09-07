using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;

public class NextGeneration : MonoBehaviour
{
    public Graph my_graph;
    public PathFinder my_pathFinder;
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Button>().onClick.AddListener(generate);
    }

    // Update is called once per frame
    void generate()
    {
        my_pathFinder.TraverseCells(); //traverse cells counts the cells neighbors and updates the next nodes
        my_pathFinder.ShowColors();
        my_pathFinder.SetNextCells(); //updates cells
        my_pathFinder.ShowColors(); //updates colors

        for (int i = 0; i < my_graph.getWidth(); i++)
        {
            for (int j = 0; j < my_graph.getHeight(); j++)
            {
                my_graph.nodes[i, j].occupied = false;
            }
        }
    }
}
