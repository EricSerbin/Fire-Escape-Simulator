
using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Graph : MonoBehaviour
{
    public Node[,] nodes;
    public List<Node> walls = new List<Node>();
    int[,] m_mapData;
    int m_width;
    int m_height;

    public int getWidth()
    {
        return m_width;
    }
    public void setWidth(int m_width)
    {
        this.m_width = m_width;
    }
    public int getHeight()
    {
        return m_height;
    }
    public void setHeight(int m_height)
    {
        this.m_height = m_height;
    }

    public static readonly Vector2[] allDirections =
    {
        new Vector2(0f,1f),
        new Vector2(1f,1f),
        new Vector2(1f,0f),
        new Vector2(1f,-1f),
        new Vector2(0f,-1f),
        new Vector2(-1f,-1f),  
        new Vector2(-1f,0f),
        new Vector2(-1f,1f)
    };

    public void Init(int[,] mapData)
    {
        m_mapData= mapData;
        m_width = mapData.GetLength(0);
        m_height = mapData.GetLength(1);
        nodes = new Node[m_width, m_height];

        for(int y=0;y<m_height;y++)
        {
            for(int x=0;x<m_width;x++)
            {

                NodeType type=(NodeType)mapData[x,y]; //our typecasting
                Node newNode = new Node(x, y, type, Mode.Safe);
                nodes[x,y]=newNode;//storing in array of nodes
                newNode.position = new Vector3(x, 0, y); //copying position from vector3
                

                if(type==NodeType.Blocked)
                {
                    walls.Add(newNode);
                    
                }
                else
                {

                }
            }
        }

        for (int y = 0; y < m_height; y++)
        {
            for (int x = 0; x < m_width; x++)
            {
                if (nodes[x,y].nodeType != NodeType.Blocked)
                {
                    nodes[x, y].neighbors = GetModNeighbors(x,y); 
                }
                
            }
        }


    }
    public bool IsWithinBounds(int x, int y)
    {
        return (x >= 0 && x < m_width && y >= 0 && y < m_height);//allDirections[x,y]
    }
    
    List<Node> GetModNeighbors(int x, int y, Node[,] nodeArray, Vector2[] allDirections)
    {
        List<Node> neighborNodes = new List<Node>();
        foreach (Vector2 dir in allDirections)
        {
            int xHolder= (x + (int)dir.x);
            int yHolder = (y + (int)dir.y);
            
            int newX = ((xHolder % m_width)+m_width) % m_width;
            int newY = ((yHolder % m_height)+m_height) % m_height;//same as get neighbors but using modulo, accounting for negatives

            if (IsWithinBounds(newX, newY) && nodeArray[newX, newY] != null && nodeArray[newX, newY].nodeType != NodeType.Open)
            {
                neighborNodes.Add(nodeArray[newX, newY]);
            }
        }
        return neighborNodes;
    }

    public List<Node> GetModNeighbors(int x, int y)
    {
        return GetModNeighbors(x, y, nodes, allDirections);
    }
    public float GetNodeDistance(Node source, Node target) //euclidean dist
    {
        int dx = Mathf.Abs(source.xIndex - target.xIndex);
        int dy = Mathf.Abs(source.yIndex - target.yIndex);
        int min = Mathf.Min(dx, dy);
        int max = Mathf.Max(dx, dy);
        int diagonalSteps = min;
        int straightSteps = max - min;
        return (1.4F * diagonalSteps + straightSteps);
    }

    public float GetManhattanNodeDistance(Node source, Node target)
    {
        int dx = Mathf.Abs(source.xIndex - target.xIndex);
        int dy = Mathf.Abs(source.yIndex - target.yIndex);
        int min = Mathf.Min(dx, dy);
        int max = Mathf.Max(dx, dy);
        int diagonalSteps = min;
        int straightSteps = max - min;
        int properSteps = dx + dy;
        properSteps = max + min;
        return (properSteps);

    }

    Node GetModderNeighbors(int x, int y, Node[,] nodeArray, Vector2[] allDirections, int[] tempArr)
    {
        List<Node> neighborNodes = new List<Node>();

        List<Node> sortNodes = new List<Node>();
        Node fireNode = new Node(nodeArray[x,y].fireX, nodeArray[x, y].fireY, NodeType.Open, Mode.Safe);

        //function checks in every direction and uses manhattan distance between stored fire location and new locations. If they are open without fire coordinates
        //they are valid destinations and will be weighed. This function uses heighest weights furthest from the fire while avoiding obstacles

        //fireNode.xIndex = tempArr[0];
        //fireNode.yIndex = tempArr[1];


        bool fireTrip = false;
        foreach (Vector2 dir in allDirections)
        {
            int xHolder = (x + (int)dir.x);
            int yHolder = (y + (int)dir.y);
            

            int newX = ((xHolder % m_width) + m_width) % m_width;
            int newY = ((yHolder % m_height) + m_height) % m_height;//same as get neighbors but using modulo, accounting for negatives

            
            /*if (IsWithinBounds(newX, newY) && nodeArray[x, y].nodeType == NodeType.Person
                && nodeArray[newX, newY] != null &&
                nodeArray[newX, newY].nodeType == NodeType.Open && (nodeArray[x, y].fireX != -1 && nodeArray[x, y].fireX != -1))*/
            
                if (IsWithinBounds(newX, newY) && nodeArray[x, y].nodeType == NodeType.Person 
                &&  
                nodeArray[newX, newY].nodeType == NodeType.Open && 
                (nodeArray[newX, newY].fireX==-1 && nodeArray[newX,newY].fireX==-1)) //
                // && (nodeArray[newX, newY].fireX != -1 && nodeArray[newX, newY].fireY != -1) easier to ensure they equal -1 since and it's a person, means it's open with no history
            {
                Debug.Log("I want a pony\n\n");
                fireTrip = true;
                
                //UnityEngine.Debug.Log(" the current node is " + nodeArray[newX, newY].xIndex + ", " + nodeArray[newX, newY].yIndex);
                //UnityEngine.Debug.Log(" the fire node is " + fireNode.xIndex + ", " + fireNode.yIndex);
                //UnityEngine.Debug.Log(" new x is " + newX + " and new y is " + newY);
                float distanceToNeighbor = GetManhattanNodeDistance(fireNode,nodeArray[newX, newY] ); 
                float newDistance = distanceToNeighbor;
                nodeArray[newX, newY].distanceTraveled = newDistance;
               
                sortNodes.Add(nodeArray[newX, newY]); //added to list to be sorted
            }

        }
        Debug.Log(" this loop was awful\n\n");
        Node tempNode = nodeArray[x, y];
        if(fireTrip==true)
        {
            tempNode = sortNodes[0];
            int tempIndex = 0;
            for (int i = 0; i < sortNodes.Count; i++)
            {
                Debug.Log("sortnodes " + i + " is " + sortNodes[i].xIndex+", "+sortNodes[i].yIndex+" with weight " + tempNode.distanceTraveled);
                if (sortNodes[i].distanceTraveled > tempNode.distanceTraveled)//>=
                {
                    UnityEngine.Debug.Log("for loop manhattan distance is " + sortNodes[i].distanceTraveled +"at index: "+ sortNodes[i].xIndex+", " + sortNodes[i].yIndex);
                    tempNode = sortNodes[i];
                    tempIndex = i;
                 
                }
            }
        }
        Debug.Log(" sortNodes best pick is " + tempNode.xIndex + ", " + tempNode.yIndex+" with weight "+tempNode.distanceTraveled);
        return tempNode;
    }

    public Node GetModderNeighbors(int x, int y, int[] tempArr) 
    {
        return GetModderNeighbors(x, y, nodes, allDirections, tempArr);
    }
    public Mode IsFire(int x, int y, Node[,] nodeArray, Vector2[] allDirections)
    {
        List<Node> neighborNodes = new List<Node>(); //checks if there is an adjacent fire and tells cells to evacuate if there is. Used by GetMode
        foreach (Vector2 dir in allDirections)
        {
            int xHolder = (x + (int)dir.x);
            int yHolder = (y + (int)dir.y);

            int newX = ((xHolder % m_width) + m_width) % m_width;
            int newY = ((yHolder % m_height) + m_height) % m_height;//same as get neighbors but using modulo, accounting for negatives

            if (IsWithinBounds(newX, newY) && nodeArray[newX,newY] != null 
                && nodeArray[x,y].nodeType==NodeType.Person &&
                (nodeArray[newX,newY].nodeMode == Mode.Evacuating || nodeArray[newX, newY].nodeType==NodeType.Fire))//NodeType.Fire && node.fireTimer==-1)
            {
                return Mode.Evacuating;
            }
        }
        return nodes[x, y].nodeMode;

    }

    public Mode IsFire(int x, int y)
    {
        return IsFire(x, y, nodes, allDirections);
    }


    public int[] FireLoc(int x, int y, Node[,] nodeArray, Vector2[] allDirections)
    {
        List<Node> neighborNodes = new List<Node>();
        int[] xyArr = new int[2];
        xyArr[0] = -1;
        xyArr[1] = -1; 
        foreach (Vector2 dir in allDirections)
        {
            int xHolder = (x + (int)dir.x);
            int yHolder = (y + (int)dir.y);
           
            //gets location and initializes fires to no destination. Returns locations in array

            int newX = ((xHolder % m_width) + m_width) % m_width;
            int newY = ((yHolder % m_height) + m_height) % m_height;//same as get neighbors but using modulo, accounting for negatives

            if (IsWithinBounds(newX, newY) && nodeArray[newX, newY] != null
                && nodeArray[x, y].nodeType == NodeType.Person &&
                (nodeArray[newX, newY].nodeType == NodeType.Fire))
            {
                
                xyArr[0]=newX;
                xyArr[1]=newY;
                return xyArr;
            }
            else if (IsWithinBounds(newX, newY) && nodeArray[newX, newY] != null
                && nodeArray[x, y].nodeType == NodeType.Person &&
                (nodeArray[newX, newY].nodeMode== Mode.Evacuating))
            {
                xyArr[0] = nodeArray[newX, newY].fireX;
                xyArr[1] = nodeArray[newX, newY].fireY;
                return xyArr;
                //return Mode.Evacuating;
            }
            else if (IsWithinBounds(x,y) && nodeArray[x,y].nodeType==NodeType.Fire)
            {

                xyArr[0] = -1;
                xyArr[1] = -1;
            }
        }
        return xyArr;

    }


    public int[] FireLoc(int x, int y)
    {
        return FireLoc(x, y, nodes, allDirections);
    }

    Node GetPanicNeighbors(int x, int y, Node[,] nodeArray, Vector2[] allDirections)
    {
        List<Node> neighborNodes = new List<Node>();

        List<Node> sortNodes = new List<Node>();
        Node fireNode = new Node(nodeArray[x, y].fireX, nodeArray[x, y].fireY, NodeType.Open, Mode.Safe);

        //function checks in every direction and uses manhattan distance between stored fire location and new locations. If they are open without fire coordinates
        //they are valid destinations and will be weighed. This function uses a random number since panicked people are not rational
       
        bool fireTrip = false;
        foreach (Vector2 dir in allDirections)
        {
            int xHolder = (x + (int)dir.x);
            int yHolder = (y + (int)dir.y);

            int newX = ((xHolder % m_width) + m_width) % m_width;
            int newY = ((yHolder % m_height) + m_height) % m_height;//same as get neighbors but using modulo, accounting for negatives

            if (IsWithinBounds(newX, newY) && nodeArray[x, y].nodeType == NodeType.Person
            &&
            nodeArray[newX, newY].nodeType == NodeType.Open &&
            (nodeArray[newX, newY].fireX == -1 && nodeArray[newX, newY].fireX == -1)) //seems like this trips if there's fire presumably

            {
                //Debug.Log("I want a pony\n\n");
                fireTrip = true;
                
                //UnityEngine.Debug.Log(" the current node is " + nodeArray[newX, newY].xIndex + ", " + nodeArray[newX, newY].yIndex);
                //UnityEngine.Debug.Log(" the fire node is " + fireNode.xIndex + ", " + fireNode.yIndex);
                //UnityEngine.Debug.Log(" new x is " + newX + " and new y is " + newY);
                float distanceToNeighbor = GetManhattanNodeDistance(fireNode, nodeArray[newX, newY]);
                float newDistance = distanceToNeighbor;
                nodeArray[newX, newY].distanceTraveled = newDistance;
                
                sortNodes.Add(nodeArray[newX, newY]);
            }

        }
        Node tempNode = nodeArray[x, y];
        if (fireTrip == true)
        {
            tempNode = sortNodes[0];
            var randomizer = new System.Random();
            int randomIndex = randomizer.Next(0, sortNodes.Count);
            tempNode = sortNodes[randomIndex];
        }
        //Debug.Log(" sortNodes best pick is " + tempNode.xIndex + ", " + tempNode.yIndex + " with weight " + tempNode.distanceTraveled);
        return tempNode;
    }

    public Node GetPanicNeighbors(int x, int y)
    {
        return GetPanicNeighbors(x, y, nodes, allDirections);
    }
}
