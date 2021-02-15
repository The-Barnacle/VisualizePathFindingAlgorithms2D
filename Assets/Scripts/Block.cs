using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : IComparable
{
    public Block()
    {
        Debug.Log("CREATED");
    }
    //Co-Ordinates
    public int x;
    public int y;
    //Path cost
    public int g;
    //Heuristic
    public int h;
    //f = g+h
    public int f;
    //Whether the block has been searched.
    public bool searched = false;
    //Location of parent, used to fill in the correct path once found.
    public Block parent;
    public GameObject blockGameObject;

    public void CalculateF()
    {
        f = h + g;
    }
    public int CompareTo(object obj)
    {
        var otherObject = obj as Block;
        return f.CompareTo(otherObject.f);
    }
}
