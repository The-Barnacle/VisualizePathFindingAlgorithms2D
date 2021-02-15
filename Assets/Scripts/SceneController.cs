using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SceneController : MonoBehaviour
{
    public GameObject blockPrefab;
    public Block prevGoal;
    public Block prevStart;
    public float width = 8.5f;
    public float height = 4.5f;
    public bool isSetStart = false;
    public bool isSetGoal = false;
    Dropdown selectedAlgorithm;

    List<List<Block>> blockArray;
    public int startX, startY;
    public int goalX, goalY;
    //Set when mouse is clicked on a block
    public bool isDrawing { get; set; }
    //If current block is becoming an obstacle
    private bool isSettingObstacle = false;
    //Whether the mouse down event is setting obstacle or removing obstacle.
    public bool IsSettingObstacle
    {
        get { return isSettingObstacle; }
        set
        {
            isSettingObstacle = value;
        }
    }

    void Awake()
    {
        blockArray = new List<List<Block>>();
        populateGrid();
    }

    private void populateGrid()
    {
        int xCounter = 0;
        for (float i = -width; i <= width; i += 0.25f)
        {
            int yCounter = 0;
            List<Block> row = new List<Block>();
            for (float j = -height; j <= height-1; j += 0.25f)
            {
                Block block = new Block();
                block.x = xCounter;
                block.y = yCounter;
                block.blockGameObject =  Instantiate(blockPrefab, new Vector2(i, j), Quaternion.identity);
                SquareController temp = block.blockGameObject.GetComponent<SquareController>();
                temp.x = xCounter;
                temp.y = yCounter;
                row.Add(block);
                yCounter++;
            }
            xCounter++;
            blockArray.Add(row);

        }
    }

    private void Start()
    {
        prevGoal = blockArray[blockArray.Count - 1][blockArray[0].Count - 1];
        prevStart = blockArray[0][0];
        blockArray[0][0].blockGameObject.GetComponent<SquareController>().SetStart();
        blockArray[blockArray.Count - 1][blockArray[0].Count - 1].blockGameObject.GetComponent<SquareController>().SetGoal();
    }


    public void ResetBoard()
    {
        foreach (List<Block> g in blockArray)
        {
            foreach (Block b in g)
            {
                b.blockGameObject.GetComponent<SquareController>().ResetBlock();
                
            }
        }
    }
    public void SetPrevGoal(int x, int y)
    {
        prevGoal = blockArray[x][y];
        goalX = x;
        goalY = y;
    }
    public void SetPrevStart(int x, int y)
    {
        prevStart = blockArray[x][y];
        startX = x;
        startY = y;
        
    }
    public void SetGoalFlag()
    {
        if (prevGoal != null)
            prevGoal.blockGameObject.GetComponent<SquareController>().ResetBlock();
        isSetStart = false;
        isSetGoal = true;
        isDrawing = false;
    }
    public void SetStartFlag()
    {
        if (prevStart != null)
            prevStart.blockGameObject.GetComponent<SquareController>().ResetBlock();
        isSetGoal = false;
        isSetStart = true;
        isDrawing = false;
    }

    #region PathFinding Algorithm General


    /// <summary>
    /// Assign each block an heuristic value based on goal location
    /// </summary>
    public void CalculateHeuristics()
    {
        foreach(List<Block> l in blockArray)
        {
            foreach(Block b in l)
            {
                b.h = Math.Abs(goalX - b.x) + Math.Abs(goalY - b.y);
            }
        }
    }

    /// <summary>
    /// Get a list of adjacent members.
    /// </summary>
    private List<Block> GetNeighbours(Block parent)
    {
        List<Block> returnList = new List<Block>();
        int x = parent.x, y = parent.y;
        if (x < blockArray.Count - 1)
        {
            if(!blockArray[x + 1][y].blockGameObject.GetComponent<SquareController>().isObstacle)
            {
                if (!blockArray[x + 1][y].searched)
                {
                    blockArray[x + 1][y].parent = parent;
                    returnList.Add(blockArray[x + 1][y]);
                }
            }
        }
        if (x >= 1)
        {
            if (!blockArray[x - 1][y].blockGameObject.GetComponent<SquareController>().isObstacle)
            {
                if (!blockArray[x - 1][y].searched)
                {
                    blockArray[x - 1][y].parent = parent;
                    returnList.Add(blockArray[x - 1][y]);
                }
            }

        }
        if (y >= 1)
        {
            if (!blockArray[x][y-1].blockGameObject.GetComponent<SquareController>().isObstacle)
            {
                if (!blockArray[x][y - 1].searched)
                {
                    blockArray[x][y - 1].parent = parent;
                    returnList.Add(blockArray[x][y - 1]);
                }
            }
        }
        if (y < blockArray[0].Count - 1)
        {
            if (!blockArray[x ][y+1].blockGameObject.GetComponent<SquareController>().isObstacle)
            {
                if(!blockArray[x][y+1].searched)
                {
                    blockArray[x][y + 1].parent = parent;
                    returnList.Add(blockArray[x][y + 1]);
                }

            }
        }
        return returnList;
    }

    IEnumerator AnimateSearch(List<Block> bl)
    {
        Debug.Log("Animate Called");
        for(int i = 0; i<bl.Count; i++ )
        {
            bl[i].blockGameObject.GetComponent<SpriteRenderer>().color = Color.red;
            Debug.Log("Done one");
            yield return new WaitForSeconds(0.025f);
        }
    }
    #endregion

    #region A* 

    public void AStarAlgorithm()
    {
        CalculateHeuristics();
        //Create frontier list
        List<Block> frontier = new List<Block>();
        //Reset the state of the array
        foreach (List<Block> l in blockArray)
            foreach (Block b in l)
                b.searched = false;
        foreach (Block b in GetNeighbours(blockArray[startX][startY]))
        {
            b.g = 1;
            b.searched = true;
            b.CalculateF();
            frontier.Add(b);
        }
        List<Block> animatedList = new List<Block>();
        //SOMETHING BIG BROKE HERE, UNITY HANGS
        while (!prevGoal.searched)
        {
            frontier.Sort();
            Block expanded = frontier[0];
            Debug.Log("Expanded X:" + expanded.x + "  Y:" + expanded.y + " g:" + expanded.g);
            animatedList.Add(expanded);
            frontier.RemoveAt(0);
            foreach (Block b in GetNeighbours(expanded))
            {

                b.g = b.parent.g + 1;
                b.searched = true;
                b.CalculateF();

                frontier.Add(b);
            }
        }
        StartCoroutine(AnimateSearch(animatedList));
        Debug.Log(frontier.Count);

    }

    #endregion
}
