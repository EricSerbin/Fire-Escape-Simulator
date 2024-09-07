using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UIElements;
 //not ure about this one

//using Math;
using Color = UnityEngine.Color;
using System.Diagnostics;
using System;
using JetBrains.Annotations;

public class PathFinder : MonoBehaviour
{
    
    Graph m_graph;
    GraphView m_graphView;
    private float fixedDeltaTime;

    //remove later
    NodeView m_nodeView;
    MapData m_mapData;

    //Queue<Node> m_frontierNodes;
    
    List<Node> m_frontierNodesAstar;//could be useful for later goalNode implementation with A start
   

    public bool isComplete = false;


    public Color openCellColor= Color.white;
    public Color blockedCellColor = Color.black;
    public Color nextCellColor= Color.blue; //can also be cyan
    public Color safePersonCellColor = Color.green;
    public Color evacuatingPersonCellColor = Color.magenta;
    public Color panickingPersonCellColor = Color.cyan;

    public Color fireCellColor = Color.red;


    public void Init(Graph graph, GraphView graphView)
    {
        if (graph == null || graphView == null)
        {
            UnityEngine.Debug.LogWarning("PATH finder init are missing components");
            return;
        }
        
        m_graph = graph;
        m_graphView = graphView;
        

    }
    public void UpdateGraphView(Graph tempGraph)
    {
        Destroy(m_graphView);
        // m_graphView.UpdateBoundaries(tempGraph);
        m_graphView.Init(tempGraph); //this was used for a method for resizing the grid at runtime, which was not feasible

    }

    private void ShowColors(GraphView graphView)
    {
     

        for (int i = 0; i < m_graph.getWidth(); i++)
        {
            for (int j = 0; j < m_graph.getHeight(); j++)
            {
                
                UnityEngine.Debug.Log(" i is " + i + " and j is " + j);
                UnityEngine.Debug.Log("this is i j nodes"+m_graph.nodes[i, j]);
                UnityEngine.Debug.Log(" graphnodes i j type is " + m_graph.nodes[i, j].nodeType);
                if (m_graph.nodes[i, j].nodeType == NodeType.Open)
                {
                    graphView.nodeViews[i, j].ColorNode(openCellColor);//green

                }
                else if (m_graph.nodes[i, j].nodeType == NodeType.Blocked)//grey
                {
                    graphView.nodeViews[i, j].ColorNode(blockedCellColor);

                }
                else if (m_graph.nodes[i,j].nodeType == NodeType.Person && m_graph.nodes[i, j].nodeMode == Mode.Safe)
                {
                    graphView.nodeViews[i, j].ColorNode(safePersonCellColor);
                }
                else if (m_graph.nodes[i, j].nodeType == NodeType.Person && m_graph.nodes[i, j].nodeMode == Mode.Evacuating)
                {
                    graphView.nodeViews[i, j].ColorNode(evacuatingPersonCellColor);
                }
                else if (m_graph.nodes[i, j].nodeType == NodeType.Person && m_graph.nodes[i, j].nodeMode == Mode.Panicked)
                {
                    graphView.nodeViews[i, j].ColorNode(panickingPersonCellColor);
                }
                else if (m_graph.nodes[i,j].nodeType == NodeType.Fire)
                {
                    graphView.nodeViews[i, j].ColorNode(fireCellColor);
                }
                else
                {
                    UnityEngine.Debug.Log("no color found");
                }

            }
        }


    }
    public void ShowColors()
    {
        ShowColors(m_graphView);
    }


    public IEnumerator SearchRoutine(float timeStep=0.1f)
    {
        yield return null;

        ShowColors(m_graphView);
        
        while (!isComplete) //can change this for each number of generations
        {
            yield return new WaitForSeconds(timeStep); //this wait is dependent on the game speed, with a minimum of 1x game speed

            TraverseCells(); //traverse cells counts the cells neighbors and updates the next nodes
            UnityEngine.Debug.Log(" what about calling it here spot1? ");
            ShowColors(m_graphView);
            SetNextCells(); //updates cells
            //UnityEngine.Debug.Log(" time for m graphview");
            UnityEngine.Debug.Log(" what about calling it here spot2");
            ShowColors(m_graphView); //updates colors

            for (int i = 0; i < m_graph.getWidth(); i++)
            {
                for (int j = 0; j < m_graph.getHeight(); j++)
                {
                    m_graph.nodes[i, j].occupied = false;
                }
            }
                    //set to false here 

                }
    }
    public void TraverseCells()
    {
        for (int i = 0; i < m_graph.getWidth(); i++)
        {
            for (int j = 0; j < m_graph.getHeight(); j++)
            {

                Node newNode = m_graph.nodes[i, j];

                Mode updatedMode= getMode(newNode, i, j); //update mode func
                //UnityEngine.Debug.Log(" the mode is " + updatedMode);
                Node nextNode = new Node(i, j, newNode.nodeType, updatedMode); //gets some info but not alll
                nextNode.fireX = newNode.fireX;
                nextNode.fireY = newNode.fireY; //fire coords
                
                
                int[] tempArr = new int[2];
                tempArr = m_graph.FireLoc(i, j); //seems to work fine since accounts for neighbor in same cycle
                
                //UnityEngine.Debug.Log(" fireX is " + tempArr[0] + " fireY is " + tempArr[1]);
                //UnityEngine.Debug.Log("i/x is " + i + " and j/y is " + j);
                if((newNode.fireX==-1 && newNode.fireY==-1)&&newNode.nodeType==NodeType.Person) //if there isn't existing fire and node is person, init
                {
                    nextNode.fireX = tempArr[0];
                    nextNode.fireY = tempArr[1];
                    m_graph.nodes[i, j].fireX = nextNode.fireX;
                    m_graph.nodes[i, j].fireY = nextNode.fireY;

                }
                else if((nextNode.fireX != -1 && nextNode.fireY != -1)&&nextNode.nodeType == NodeType.Person&& (tempArr[0] != -1 && tempArr[1] != -1)) //if there's a closer fire, update
                {
                    nextNode.fireX = tempArr[0];
                    nextNode.fireY = tempArr[1];
                    m_graph.nodes[i, j].fireX = nextNode.fireX;
                    m_graph.nodes[i, j].fireY = nextNode.fireY;
                  
                }
                else
                {

                }
               
                if (newNode.occupied==true) //we don't want nodes to steal the space of others during each round, so current node is blocked
                {
                    continue;
                }
                else if (nextNode.nodeMode==Mode.Safe && nextNode.nodeType==NodeType.Person)
                {
                    //UnityEngine.Debug.Log("nothing happened, you're safe");
                    newNode.next = nextNode;
                }
                else if(nextNode.nodeMode == Mode.Evacuating && nextNode.nodeType == NodeType.Person)
                {

                    Node dummyNode = m_graph.GetModderNeighbors(i, j, tempArr); 
                    //UnityEngine.Debug.Log(" the dummyNode right after modderNeighbors is is " + dummyNode.xIndex + ", " + dummyNode.yIndex + " type: " + dummyNode.nodeType + "mode: " + dummyNode.nodeMode+" fireX: "+dummyNode.fireX+" fireY: "+dummyNode.fireY);
                    //UnityEngine.Debug.Log(" the nextNode right after modderNeighbors is is " + nextNode.xIndex + ", " + nextNode.yIndex + " type: " + nextNode.nodeType + "mode: " + nextNode.nodeMode + " fireX: " + nextNode.fireX + " fireY: " + nextNode.fireY);

                    //UnityEngine.Debug.Log(" the new location should be " + dummyNode.xIndex + ", " + dummyNode.yIndex);
                    //UnityEngine.Debug.Log(" the old location should be "+ )
                    Node tempNode = new Node (nextNode.xIndex, nextNode.yIndex, nextNode.nodeType, nextNode.nodeMode);
                    tempNode.fireX = nextNode.fireX;
                    tempNode.fireY = nextNode.fireY;
                    tempNode.priority = nextNode.priority;
                    tempNode.occupied = nextNode.occupied;
                    
                    nextNode.fireX = dummyNode.fireX;
                    nextNode.fireY = dummyNode.fireY;
                    nextNode.priority = dummyNode.priority;
                    nextNode.nodeMode = dummyNode.nodeMode;
                    nextNode.nodeType = dummyNode.nodeType;
                    
                    dummyNode.fireX = tempNode.fireX;
                    dummyNode.fireY = tempNode.fireY;
                    dummyNode.nodeMode= tempNode.nodeMode;
                    dummyNode.priority = tempNode.priority;
                    dummyNode.nodeMode = tempNode.nodeMode;
                    dummyNode.nodeType = tempNode.nodeType; //transfering values over. Indexes aren't exchanged to determine correct new spots
                    dummyNode.occupied = true;


                    Node nextNodeParent = m_graph.nodes[i, j];
                    nextNodeParent.next = nextNode;
                    nextNodeParent.occupied = true;
                    Node oldNodeParent = m_graph.nodes[dummyNode.xIndex, dummyNode.yIndex];
                    oldNodeParent.next = dummyNode;
                    oldNodeParent.occupied = true; //these were used for testing and are not relevant now

                    m_graph.nodes[i, j].next = nextNode;
                    m_graph.nodes[i, j].occupied = false;
                    
                    m_graph.nodes[dummyNode.xIndex, dummyNode.yIndex].next = dummyNode;
                    m_graph.nodes[dummyNode.xIndex, dummyNode.yIndex].occupied = true;

                    //UnityEngine.Debug.Log(" the tempNode after swap is " + tempNode.xIndex + ", " + tempNode.yIndex + " type: " + tempNode.nodeType + "mode: " + tempNode.nodeMode + " fireX: " + tempNode.fireX + " fireY: " + tempNode.fireY);
                    //UnityEngine.Debug.Log(" the dummyNode after swap is " + dummyNode.xIndex + ", " + dummyNode.yIndex + " type: " + dummyNode.nodeType + "mode: " + dummyNode.nodeMode + " fireX: " + dummyNode.fireX + " fireY: " + dummyNode.fireY);
                    //UnityEngine.Debug.Log(" the nextNode after swap is " + nextNode.xIndex + ", " + nextNode.yIndex + " type: " + nextNode.nodeType + "mode: " + nextNode.nodeMode + " fireX: " + nextNode.fireX + " fireY: " + nextNode.fireY);

                    //UnityEngine.Debug.Log(" the nextNode is " + nextNode.xIndex + ", " + nextNode.yIndex+" type: "+ nextNode.nodeType+ "mode: "+ nextNode.nodeMode);
                    //UnityEngine.Debug.Log(" the nextNodeParent is " + nextNodeParent.xIndex + ", " + nextNodeParent.yIndex + " type: " + nextNodeParent.nodeType + "mode: " + nextNodeParent.nodeMode);
                    //UnityEngine.Debug.Log(" the dummyNode is " + dummyNode.xIndex + ", " + dummyNode.yIndex + " type: " + dummyNode.nodeType + "mode: " + dummyNode.nodeMode);
                    //UnityEngine.Debug.Log(" the oldNodeParent is " + oldNodeParent.xIndex + ", " + oldNodeParent.yIndex + " type: " + oldNodeParent.nodeType + "mode: " + oldNodeParent.nodeMode);
                    
                    //UnityEngine.Debug.Log("and now the nodes i j is " + m_graph.nodes[i, j].xIndex + ", " + m_graph.nodes[i, j].yIndex + " type: " + m_graph.nodes[i, j].nodeType + "mode: " + m_graph.nodes[i, j].nodeMode + " and fire x is " + m_graph.nodes[i, j].fireX + " , fire y is " + m_graph.nodes[i, j].fireY);
                    //UnityEngine.Debug.Log("and now the nodes i j next is " + m_graph.nodes[i, j].next.xIndex + ", " + m_graph.nodes[i, j].next.yIndex + " type: " + m_graph.nodes[i, j].next.nodeType + "mode: " + m_graph.nodes[i, j].next.nodeMode + " and fire x is " + m_graph.nodes[i, j].next.fireX + " , fire y is " + m_graph.nodes[i, j].next.fireY);

                    //UnityEngine.Debug.Log(" and nodes dumbindex i j is " +m_graph.nodes[dummyNode.xIndex, dummyNode.yIndex].xIndex + ", " + m_graph.nodes[dummyNode.xIndex, dummyNode.yIndex].yIndex + " type: " + m_graph.nodes[dummyNode.xIndex, dummyNode.yIndex].nodeType + "mode: " + m_graph.nodes[dummyNode.xIndex, dummyNode.yIndex].nodeMode+ " and fire x is "+ m_graph.nodes[dummyNode.xIndex, dummyNode.yIndex].fireX+" , fire y is "+ m_graph.nodes[dummyNode.xIndex, dummyNode.yIndex].fireY);
                    //UnityEngine.Debug.Log(" and nodes dumbindex i j next is " + m_graph.nodes[dummyNode.xIndex, dummyNode.yIndex].next.xIndex + ", " + m_graph.nodes[dummyNode.xIndex, dummyNode.yIndex].next.yIndex + " type: " + m_graph.nodes[dummyNode.xIndex, dummyNode.yIndex].next.nodeType + "mode: " + m_graph.nodes[dummyNode.xIndex, dummyNode.yIndex].next.nodeMode + " and fire x is " + m_graph.nodes[dummyNode.xIndex, dummyNode.yIndex].next.fireX + " , fire y is " + m_graph.nodes[dummyNode.xIndex, dummyNode.yIndex].next.fireY);

                   
                    if ((m_graph.nodes[dummyNode.xIndex, dummyNode.yIndex].next.nodeType == NodeType.Person && m_graph.nodes[dummyNode.xIndex, dummyNode.yIndex].next.nodeMode == Mode.Evacuating) && (m_graph.nodes[dummyNode.xIndex, dummyNode.yIndex].xIndex == m_graph.nodes[dummyNode.xIndex, dummyNode.yIndex].next.xIndex &&
                        m_graph.nodes[dummyNode.xIndex, dummyNode.yIndex].yIndex == m_graph.nodes[dummyNode.xIndex, dummyNode.yIndex].next.yIndex))
                    {
                        m_graph.nodes[dummyNode.xIndex, dummyNode.yIndex].next.fireTimer++;
                    }
                    //increments fire timer if the node is a person, is evacuating, and has not moved between rounds. At two rounds of no movement panick mode starts
                    m_graph.nodes[i, j].next.fireTimer = m_graph.nodes[dummyNode.xIndex, dummyNode.yIndex].next.fireTimer; //transfering
                }
                else if(nextNode.nodeMode == Mode.Panicked && nextNode.nodeType == NodeType.Person)
                {
                    UnityEngine.Debug.Log(" here comes the sun");
                    UnityEngine.Debug.Log("suffering nextnode fire x and firey are " + nextNode.fireX + ", " + nextNode.fireY +
                        "vs " + newNode.fireX + ", " + newNode.fireY);

                    Node dummyNode = m_graph.GetPanicNeighbors(i, j);
                    UnityEngine.Debug.Log(" the dummyNode right after modderNeighbors is is " + dummyNode.xIndex + ", " + dummyNode.yIndex + " type: " + dummyNode.nodeType + "mode: " + dummyNode.nodeMode + " fireX: " + dummyNode.fireX + " fireY: " + dummyNode.fireY);
                    UnityEngine.Debug.Log(" the nextNode right after modderNeighbors is is " + nextNode.xIndex + ", " + nextNode.yIndex + " type: " + nextNode.nodeType + "mode: " + nextNode.nodeMode + " fireX: " + nextNode.fireX + " fireY: " + nextNode.fireY);

                    Node tempNode = new Node(nextNode.xIndex, nextNode.yIndex, nextNode.nodeType, nextNode.nodeMode);
                    tempNode.fireX = nextNode.fireX;
                    tempNode.fireY = nextNode.fireY;
                    tempNode.priority = nextNode.priority;
                    tempNode.occupied = nextNode.occupied;
                    //tempNode.fireTimer = nextNode.fireTimer;

                    nextNode.fireX = dummyNode.fireX;
                    nextNode.fireY = dummyNode.fireY;
                    nextNode.priority = dummyNode.priority;
                    nextNode.nodeMode = dummyNode.nodeMode;
                    nextNode.nodeType = dummyNode.nodeType;
                    //nextNode.fireTimer = dummyNode.fireTimer;

                    dummyNode.fireX = tempNode.fireX;
                    dummyNode.fireY = tempNode.fireY;
                    dummyNode.nodeMode = tempNode.nodeMode;
                    dummyNode.priority = tempNode.priority;
                    dummyNode.nodeMode = tempNode.nodeMode;
                    dummyNode.nodeType = tempNode.nodeType;
                    //dummyNode.fireTimer = tempNode.fireTimer;
                    //dummyNode.occupied = true;
                  
                    m_graph.nodes[i, j].next = nextNode;
                    m_graph.nodes[i, j].occupied = false;

                    m_graph.nodes[dummyNode.xIndex, dummyNode.yIndex].next.fireTimer = 100;
                    m_graph.nodes[dummyNode.xIndex, dummyNode.yIndex].next = dummyNode;
                    m_graph.nodes[dummyNode.xIndex, dummyNode.yIndex].occupied = true;
                    
                  

                }
                else if(nextNode.nodeType == NodeType.Open || nextNode.nodeType== NodeType.Blocked || nextNode.nodeType==NodeType.Fire)
                {
                    
                    newNode.next = nextNode;
                    m_graph.nodes[i, j] = newNode;
                    
                }
                else
                {
                    UnityEngine.Debug.Log(" no traversal selected");
                    newNode.next = nextNode;
                    m_graph.nodes[i, j] = newNode;
                }
              
            }
        }

    }
    public Mode getMode(Node newNode, int x, int y)
    {
     

        if(newNode.nodeType==NodeType.Person) //if the node is a person
        {
            
            newNode.nodeMode = m_graph.IsFire(x, y); //checks location of fire to see if any is in surrounding square
             
            Node distNode = new Node(newNode.fireX, newNode.fireY, newNode.nodeType, newNode.nodeMode); //distance from fire so people know when they're safe
            float fireDist = m_graph.GetManhattanNodeDistance(distNode, newNode);
            if (newNode.nodeMode != Mode.Safe && fireDist>=12) //if you aren't safe and you aren't near fire, you're safe
            {
                return Mode.Safe;
            }
            else if (newNode.nodeMode == Mode.Evacuating && newNode.fireTimer < 2) //if you are evacuating, you are still calm. Firetime is time without movement.
                                                                                   //It could have also been time spent adjacent to fires, but that would lead to panick in any row of fire
            {

                return Mode.Evacuating;

            }
            else if (newNode.nodeMode == Mode.Panicked && fireDist<12) //if you are panicked and close to fire you remain panicked
            {
                return Mode.Panicked;
            }
            else if (newNode.nodeMode == Mode.Evacuating && newNode.fireTimer >= 2) //if you haven't been able to move, you are now panicked
            {

                return Mode.Panicked;

            }
            else //otherwise you must be safe
            {
                return Mode.Safe;
            }
        }
        else
        {
            return newNode.nodeMode;
        }
        
    }
    public void SetNextCells() //a simple update changing next nodes into current nodes
    {
        for (int i = 0; i < m_graph.getWidth(); i++)
        {
            for (int j = 0; j < m_graph.getHeight(); j++)
            {
                Node newNode = m_graph.nodes[i, j];
                Node nextNode = m_graph.nodes[i, j].next; //iterates next elements. Does not set equal to node itself, as that leads to reading original node
               

                //UnityEngine.Debug.Log(" the newNode before setCells is" + newNode.xIndex + ", " + newNode.yIndex + " type: " + newNode.nodeType + "mode: " + newNode.nodeMode + " fireX: " + newNode.fireX + " fireY: " + newNode.fireY);
                //UnityEngine.Debug.Log(" the nextNode before setCells is" + nextNode.xIndex + ", " + nextNode.yIndex + " type: " + nextNode.nodeType + "mode: " + nextNode.nodeMode + " fireX: " + nextNode.fireX + " fireY: " + nextNode.fireY);
                //UnityEngine.Debug.Log(" the tempNode before setCells is" + tempNode.xIndex + ", " + tempNode.yIndex + " type: " + tempNode.nodeType + "mode: " + tempNode.nodeMode + " fireX: " + tempNode.fireX + " fireY: " + tempNode.fireY);

                newNode.xIndex = nextNode.xIndex;
                newNode.yIndex = nextNode.yIndex;
                newNode.fireX = nextNode.fireX;
                newNode.fireY = nextNode.fireY;
                newNode.nodeMode = nextNode.nodeMode;
                newNode.priority = nextNode.priority;
                newNode.nodeMode = nextNode.nodeMode;
                newNode.nodeType = nextNode.nodeType;
                newNode.neighbors = nextNode.neighbors;
                newNode.occupied = nextNode.occupied;
                newNode.fireTimer = nextNode.fireTimer;

                UnityEngine.Debug.Log(" the newNode after setCells is" + newNode.xIndex + ", " + newNode.yIndex + " type: " + newNode.nodeType + "mode: " + newNode.nodeMode + " fireX: " + newNode.fireX + " fireY: " + newNode.fireY);
                if(newNode.fireTimer>0)
                {

                    //UnityEngine.Debug.Log("FireTimer test: the x is " + newNode.xIndex + " and y is " + newNode.yIndex+" while fireTimer is "+newNode.fireTimer);
                }
                m_graph.nodes[i, j] = newNode;
                //m_graph.nodes[i, j] = m_graph.nodes[i, j].next;
            }
        }

    }
    
}
