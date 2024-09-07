using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public PathFinder pathfinder; //2nd edition?
    public MapData mapData;
    public Graph graph;
  
    public float timeStep = 0.1f; 

    //he did say we could show this off to a prospective employer because its complex and a 400 level course
    // Start is called before the first frame update
    void Start()
    {
        if (mapData != null && graph!=null)
        {
            int[,] mapInstance=mapData.MakeMap();
            graph.Init(mapInstance);
            GraphView graphView = graph.gameObject.GetComponent<GraphView>();
            if(graphView != null ) 
            {
                graphView.Init(graph);

            }
            if (pathfinder != null)
            {
                
                pathfinder.Init(graph, graphView);
                StartCoroutine(pathfinder.SearchRoutine(timeStep));
            }
            else
            {
                Debug.Log("The graph was not within bounds");
            }
        }
        
    }

}
