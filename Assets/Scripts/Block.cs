using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : IComparable
{
    //Co-Ordinates
    public int x;
    public int y;
    //Path cost
    public int g;
    //Heuristic
    public int h;
    //f = g+h
    public int f
    {
        get { return g+h; }
    }

    //Whether the block has been searched.
    public bool searched = false;
    //Location of parent, used to fill in the correct path once found.
    public Block parent;
    public GameObject blockGameObject;

    public int CompareTo(object obj)
    {
        var otherObject = obj as Block;
        return f.CompareTo(otherObject.f);
    }
}
