using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class SceneController : MonoBehaviour
{
    public GameObject blockPrefab;
    public Sprite circleSprite;
    public Block goalPos;
    public Block startPos;
    private float width = 8.5f, height = 4.5f;
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
        goalPos = blockArray[blockArray.Count - 1][blockArray[0].Count - 1];
        startPos = blockArray[0][0];
        blockArray[0][0].blockGameObject.GetComponent<SquareController>().SetStart();
        blockArray[blockArray.Count - 1][blockArray[0].Count - 1].blockGameObject.GetComponent<SquareController>().SetGoal();
    }


    public void ResetBoard()
    {
        StopAllCoroutines();
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
        goalPos = blockArray[x][y];
        goalX = x;
        goalY = y;
    }
    public void SetPrevStart(int x, int y)
    {
        startPos = blockArray[x][y];
        startX = x;
        startY = y;
        
    }
    public void SetGoalFlag()
    {
        if (goalPos != null)
            goalPos.blockGameObject.GetComponent<SquareController>().ResetBlock();
        isSetStart = false;
        isSetGoal = true;
        isDrawing = false;
    }
    public void SetStartFlag()
    {
        if (startPos != null)
            startPos.blockGameObject.GetComponent<SquareController>().ResetBlock();
        isSetGoal = false;
        isSetStart = true;
        isDrawing = false;
    }

    #region PathFinding Algorithm General

    public void CallSelectedAlgorithm()
    {
        Dropdown dropDownBox = FindObjectOfType<Dropdown>();
        switch(dropDownBox.value) //0 - A*, 1 - Greedy Best-First. Based on position added
        {
            case 0:
                AStarAlgorithm();
                break;
            case 1:
                GreedyBestFirst();
                break;
            case 2:
                DepthFirst();
                break;
            case 3:
                BreadthFirst();
                break;
            default:
                Debug.Log("Check dropdown box, chosen value outside range.");
                break;
        }
    }


    /// <summary>
    /// Assign each block an heuristic value based on goal location
    /// </summary>
    private void CalculateHeuristics()
    {
        foreach(List<Block> l in blockArray)
        {
            foreach(Block b in l)
            {
                b.h = Math.Abs(goalX - b.x) + Math.Abs(goalY - b.y);
            }
        }
    }

    private List<Block> GetNeighbours(Block parent)
    {
        List<Block> returnList = new List<Block>();
        int x = parent.x, y = parent.y;
        if (x < blockArray.Count - 1)
        {
            if (!blockArray[x + 1][y].blockGameObject.GetComponent<SquareController>().isObstacle)
            {
                    returnList.Add(blockArray[x + 1][y]);
            }
        }
        if (x >= 1)
        {
            if (!blockArray[x - 1][y].blockGameObject.GetComponent<SquareController>().isObstacle)
            {
                    returnList.Add(blockArray[x - 1][y]);
            }

        }
        if (y >= 1)
        {
            if (!blockArray[x][y - 1].blockGameObject.GetComponent<SquareController>().isObstacle)
            {
                    returnList.Add(blockArray[x][y - 1]);
            }
        }
        if (y < blockArray[0].Count - 1)
        {
            if (!blockArray[x][y + 1].blockGameObject.GetComponent<SquareController>().isObstacle)
            {
                    returnList.Add(blockArray[x][y + 1]);
            }
        }
        return returnList;
    }
    IEnumerator AnimateSearch(List<Block> bl, Color color)
    {
        for(int i = 0; i<bl.Count; i++ )
        {
            bl[i].blockGameObject.GetComponent<SpriteRenderer>().color = color;
            yield return new WaitForSeconds(0.025f);
        }
    }

    #endregion

    #region A* 
    private void AStarAlgorithm()
    {
        CalculateHeuristics();
        List<Block> openList = new List<Block>();
        List<Block> closedList = new List<Block>();
        Block current = null;
        int g = 0;
        // Add start position to list
        openList.Add(startPos);
        
        while (openList.Count >0)
        {
            //Get the square with the lowest F score.
            var lowest = openList.Min(l => l.f);
            current = openList.First(l => l.f == lowest);

            //Add the lowest F value to the list
            closedList.Add(current);
            //Remove from the open list.
            openList.Remove(current);

            //Check whether the goal has been reached i.e. added to the closed list
            if (closedList.FirstOrDefault(l => l.x == goalPos.x && l.y == goalPos.y) != null)
                break;

            var neighbours = GetNeighbours(current);
            g = current.g + 1;

            foreach(var neighbour in neighbours)
            {
                //If it exists in the closed list
                if (closedList.FirstOrDefault(l => l.x == neighbour.x && l.y == neighbour.y) != null)
                    continue;

                //If not in the open list
                if(openList.FirstOrDefault(l => l.x == neighbour.x && l.y == neighbour.y) == null)
                {
                    neighbour.g = g;
                    neighbour.parent = current;
                    openList.Insert(0,neighbour);
                }
                else
                {
                    //Check if the current G score is lower than the stored G score.
                    {
                        if(g + neighbour.h < neighbour.f)
                        {
                            neighbour.g = g;
                            neighbour.parent = current;
                        }
                    }
                }
            }
        }
        StartCoroutine(AnimateSearch(closedList, Color.red));

        List<Block> circleList = new List<Block>();
        foreach (List<Block> l in blockArray)
            foreach (Block b in l)
            {
                //Check if it's not in the closed list
                if (closedList.FirstOrDefault(l => l.x == b.x && l.y == b.y) == null)
                {
                    b.blockGameObject.GetComponent<SpriteRenderer>().sprite = circleSprite;
                    b.blockGameObject.GetComponent<CircleCollider2D>().enabled = true;
                    b.blockGameObject.GetComponent<BoxCollider2D>().enabled = false;
                }
            }
    }
    #endregion

    #region Greedy Best-First
    private void GreedyBestFirst()
    {
        CalculateHeuristics();
        //Create frontier list
        List<Block> openList = new List<Block>();
        List<Block> closedList = new List<Block>();
        Block current = null;
        //Add starting pos to open list
        openList.Add(startPos);

        while(openList.Count > 0)
        {
            //Get the square with the lowest H score.
            var lowest = openList.Min(l => l.h);
            current = openList.First(l => l.h == lowest);

            //Add the lowest h value to the list
            closedList.Add(current);
            //Remove from the open list.
            openList.Remove(current);

            //Check whether the goal has been reached i.e. added to the closed list
            if (closedList.FirstOrDefault(l => l.x == goalPos.x && l.y == goalPos.y) != null)
                break;

            var neighbours = GetNeighbours(current);
            foreach(var neighbour in neighbours)
            {
                //If it exists in the closed list
                if (closedList.FirstOrDefault(l => l.x == neighbour.x && l.y == neighbour.y) != null)
                    continue;

                //If not in the open list
                if (openList.FirstOrDefault(l => l.x == neighbour.x && l.y == neighbour.y) == null)
                {
                    neighbour.parent = current;
                    openList.Insert(0, neighbour);
                }
            }
        }
        StartCoroutine(AnimateSearch(closedList, Color.green));
    }
    #endregion

    #region Depth-First
    private void DepthFirst()
    {
        List<Block> openList = new List<Block>();
        List<Block> closedList = new List<Block>();
        Block current = null;

        //Add starting pos to openList
        openList.Add(startPos);
        int breakCheck = 0;
        while(openList.Count >0)
        {
            current = openList[0];
            openList.Remove(current);
            closedList.Add(current);

            //Check whether the closedList contains the goal
            if (closedList.FirstOrDefault(l => l.x == goalPos.x && l.y == goalPos.y) != null)
                break;

            var neighbours = GetNeighbours(current);
            foreach(var neighbour in neighbours)
            {
                //If it exists in the closed list
                if (closedList.FirstOrDefault(l => l.x == neighbour.x && l.y == neighbour.y) != null)
                    continue;
                //If not in the open list
                if (openList.FirstOrDefault(l => l.x == neighbour.x && l.y == neighbour.y) == null)
                {
                    neighbour.parent = current;
                    openList.Insert(0, neighbour);
                }
            }
            breakCheck++;

        }
        StartCoroutine(AnimateSearch(closedList, Color.blue));

    }

    #endregion

    #region Breadth First
    private void BreadthFirst()
    {
        List<Block> openList = new List<Block>();
        List<Block> closedList = new List<Block>();
        Block current = null;

        //Add starting pos to openList
        openList.Add(startPos);
        int breakCheck = 0;
        while (openList.Count > 0)
        {
            current = openList[openList.Count-1];
            openList.Remove(current);
            closedList.Add(current);

            //Check whether the closedList contains the goal
            if (closedList.FirstOrDefault(l => l.x == goalPos.x && l.y == goalPos.y) != null)
                break;

            var neighbours = GetNeighbours(current);
            foreach (var neighbour in neighbours)
            {
                //If it exists in the closed list
                if (closedList.FirstOrDefault(l => l.x == neighbour.x && l.y == neighbour.y) != null)
                    continue;
                //If not in the open list
                if (openList.FirstOrDefault(l => l.x == neighbour.x && l.y == neighbour.y) == null)
                {
                    neighbour.parent = current;
                    openList.Insert(0, neighbour);
                }
            }
            breakCheck++;

        }
        StartCoroutine(AnimateSearch(closedList, Color.yellow));
    }
    #endregion

}
