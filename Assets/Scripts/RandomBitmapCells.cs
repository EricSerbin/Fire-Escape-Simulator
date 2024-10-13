using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RandomBitmapCells : MonoBehaviour
{
    // Start is called before the first frame update
    public Graph my_graph;
    public PathFinder my_pathFinder;
    [Range(1, 100)]
    public float maxRange=1;
    [Range(1, 50)]
    public float minPeople=1;
    [Range(1, 50)]
    public float maxPeople = 1;
    [Range(1, 50)]
    public float minFires = 1;
    [Range(1, 50)]
    public float maxFires = 1;
    [Range(1, 50)]
    public float minWalls = 1;
    [Range(1, 50)]
    public float maxWalls = 1;
    
    void Start()
    {

        GetComponent<Button>().onClick.AddListener(BitmapRandomize);

    }

    // Update is called once per frame
    void BitmapRandomize()
    {
        
        Debug.Log("graph width is " + my_graph.getWidth() + "and graph height is " + my_graph.getHeight() + "\n");
        

        for (int i = 0; i < my_graph.getWidth(); i++)
        {
            for (int j = 0; j < my_graph.getHeight(); j++)
            {
                if (my_graph.nodes[i,j].nodeType==NodeType.Open)
                {
                    float randomValue = Random.Range(0, maxRange);

                    if (randomValue >= minPeople && randomValue < maxPeople)
                    {
                        my_graph.nodes[i, j].nodeType = NodeType.Person;
                        my_graph.nodes[i, j].nodeMode = Mode.Safe;

                    }
                    else if (randomValue >= minFires && randomValue < maxFires)
                    {
                        my_graph.nodes[i, j].nodeType = NodeType.Fire;

                    }
                    else if(randomValue >= minWalls && randomValue<maxWalls)
                    {
                        my_graph.nodes[i, j].nodeType = NodeType.Blocked;
                    }
                }
            }
        }
        my_pathFinder.ShowColors();
    }
}
