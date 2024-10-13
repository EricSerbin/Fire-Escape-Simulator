using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Graph))]
public class GraphView : MonoBehaviour
{
    public GameObject nodeviewPrefab;
    public NodeView[,] nodeViews;
    public Color wallColor = Color.black;
    public Color walkableColor = Color.white;
    public void Init(Graph graph)
    {
        if(graph==null)
        {
            Debug.LogWarning("GraphView - no graph to initialize");
            return;
        }
        nodeViews=new NodeView[graph.getWidth(), graph.getHeight()];

        foreach(Node n in graph.nodes)
        {
            GameObject instance = Instantiate(nodeviewPrefab,
                                   Vector3.zero, Quaternion.identity);
            NodeView nodeView = instance.GetComponent<NodeView>();
            if(nodeView != null )
            {
                nodeView.Init(n);
                nodeViews[n.xIndex,n.yIndex] = nodeView;

                if(n.nodeType==NodeType.Blocked)
                {
                    nodeView.ColorNode(wallColor);
                }
                else
                {
                    nodeView.ColorNode(walkableColor);
                }
            }

        }
    }
    public void UpdateBoundaries(Graph my_graph)
    {
      
       /* foreach(Node n in my_graph.nodes)
        {
            nodeViews[n.xIndex, n.yIndex].DestroyTile();// n.DestroyTile();
        }
        nodeViews = new NodeView[my_graph.getWidth(), my_graph.getHeight()];
        */ 
       //destructor test
        foreach (Node n in my_graph.nodes)
        {
            GameObject instance = Instantiate(nodeviewPrefab,
                                   Vector3.zero, Quaternion.identity);
            NodeView nodeView = instance.GetComponent<NodeView>();
            if (nodeView != null)
            {
                nodeView.Init(n);
                nodeViews[n.xIndex, n.yIndex] = nodeView;

                if (n.nodeType == NodeType.Blocked)
                {
                    nodeView.ColorNode(wallColor);
                }
                else
                {
                    nodeView.ColorNode(walkableColor);
                }
            }
        }
    }
    public void ColorNodes(List<Node> nodes, Color color)
    {
        foreach(Node n in nodes)
        {
            if(n!=null)
            {
                NodeView nodeView = nodeViews[n.xIndex, n.yIndex]; //single nodeview object on left, array elements on right
                if(nodeView!= null)
                {
                    nodeView.ColorNode(color);
                }
            }
        }
    }


}
