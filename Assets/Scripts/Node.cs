
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum NodeType
{
    Open = 0,
    Blocked= 1,
    Person=2,
    Fire =3

}
public enum Mode
{
    Safe = 0,
    Evacuating = 1,
    Panicked = 2
}

public class Node : IComparable<Node>
{
    public NodeType nodeType = NodeType.Open;
    public Mode nodeMode= Mode.Safe;
    public int xIndex = -1;
    public int yIndex = -1;
    public bool occupied =false;
    public int fireTimer=-1;



    public Vector3 position;
    public List<Node> neighbors=new List<Node>();
    public Node next = null;
    public int newbornFlag=0;
    public int priority;
    public float distanceTraveled = Mathf.Infinity;

    public int fireX = -1;
    public int fireY = -1;


    public Node(int xIndex, int yIndex, NodeType nodeType, Mode nodeMode)
    {
        this.xIndex = xIndex;
        this.yIndex = yIndex;
        this.nodeType = nodeType;
        this.nodeMode = nodeMode;
        
    }

    public int CompareTo(Node other) //this goes to the java class and we give an implementation
    {
        if (this.priority < other.priority)
        {
            return -1;
        }
        else if (this.priority > other.priority)
        {
            return 1;
        }
        else
        {
            return 0;
        }

    }
    
}

