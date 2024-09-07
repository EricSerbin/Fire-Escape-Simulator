using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RandomCells : MonoBehaviour
{
    // Start is called before the first frame update
    public Graph my_graph;
    public PathFinder my_pathFinder;
    void Start()
    {

        GetComponent<Button>().onClick.AddListener(clear);

    }

    // Update is called once per frame
    void clear()
    {
        //GraphView my_graphView;// = GetComponent<GraphView>();
        //        Graph my_graph = GetComponent<Graph>();
        //public Graph m_graph;
        //Graph my_graph = Graph.GetComponent<Graph>();

        ///Graph my_graph = GetComponentInChildren<Graph>();
        


        //Graphy my_graph= 
        Debug.Log("graph width is " + my_graph.getWidth() + "and graph height is " + my_graph.getHeight() + "\n");
        //my_graphView = GetComponent<GraphView>();

        for (int i = 0; i < my_graph.getWidth(); i++)
        {
            for(int j = 0; j < my_graph.getHeight(); j++)
            {
                float randomValue = Random.Range(0,16);
                if (randomValue>=0&& randomValue<4)
                {
                    my_graph.nodes[i, j].nodeType = NodeType.Blocked;

                }
                else if(randomValue>=4 && randomValue<9)
                {
                    my_graph.nodes[i, j].nodeType = NodeType.Open;
                }
                else if(randomValue >= 9 && randomValue < 15)
                {
                    my_graph.nodes[i, j].nodeType = NodeType.Person;
                    my_graph.nodes[i, j].nodeMode = Mode.Safe;

                }
                else if(randomValue >= 15 && randomValue < 16)
                {
                    my_graph.nodes[i, j].nodeType = NodeType.Fire;

                }
            }
        }
        my_pathFinder.ShowColors();
    }
}
