using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : IComparable<Node>
{

    public float X;
    public float Y;
    public int xMapIndex;
    public int yMapIndex;
    public bool hasCollider;
    public Node parent;
    public Vector2 action;
    public int cost;
    public int heuristic;
    public int priority;

    public Node(bool hasCollider, float X, float Y)
    {
        this.hasCollider = hasCollider;
        this.X = X;
        this.Y = Y;
    }

    public int CompareTo(Node n)
    {
        if(this.priority < n.priority)
        {
            return -1;
        }
        else if (this.priority == n.priority)
        {
            return 0;
        }
        else if (this.priority > n.priority)
        {
            return 1;
        }
        return 0;
    }

}
