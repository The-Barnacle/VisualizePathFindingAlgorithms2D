using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquareController : MonoBehaviour
{
    //Location respective to other blocks (Array position)
    public int x, y;
    //Heuristic is the just the distance from this block to the goal node.
    //Refered to commonly as h, set once at block creation.
    private int heuristic;
    public int Heuristic
    {
        get { return heuristic; }
    }
    //Movement cost is set dynamically. Refered to commonly as g.
    private int movementCost;
    public int MovementCost
    {
        get { return movementCost; }
        set { movementCost = value; }
    }

    Color blockColor = Color.white;
    public Sprite startSprite;
    public Sprite flagSprite;
    SpriteRenderer srBlock;
    public Sprite baseBlockSprite;
    SceneController sceneController;

    public bool isObstacle = false;
    void Awake()
    {
        sceneController = FindObjectOfType<SceneController>().GetComponent<SceneController>();
        srBlock = this.GetComponent<SpriteRenderer>();
    }

    public void CalculateHeuristic(int goalX, int goalY)
    {
        heuristic = Mathf.Abs(goalX - x) + Mathf.Abs(goalY - y);
    }
    private void OnMouseDown()
    {
        if (sceneController.isSetStart)
        {
            Debug.Log("Start Set! x:" + x + " y:" + y);
            SetStart();

        }
        else if (sceneController.isSetGoal)
        {
            Debug.Log("Goal Set! x:" + x + " y:" + y);
            SetGoal();
        }
        else
        {
            sceneController.IsSettingObstacle = !isObstacle;
            sceneController.isDrawing = true;
        }
    }

    private void OnMouseUp()
    {
        sceneController.isDrawing = false;
    }

    private void OnMouseOver()
    {
        if (sceneController.isDrawing)
        {
            if (sceneController.IsSettingObstacle)
            {
                if (!isObstacle)
                    SetObstacle();
            }
            else
            {
                if (isObstacle)
                    SetNotObstacle();
            }
        }
    }

    public void ResetBlock()
    {
        isObstacle = false;
        srBlock.sprite = baseBlockSprite;
        srBlock.color = Color.white;

    }
    public void SetStart()
    {
        this.srBlock.sprite = startSprite;
        sceneController.SetPrevStart(x, y);
        sceneController.isSetStart = false;
    }

    public void SetGoal()
    {
        this.srBlock.sprite = flagSprite;
        sceneController.SetPrevGoal(x, y);
        sceneController.isSetGoal = false;
    }
    void SetObstacle()
    {
        isObstacle = true;
        srBlock.color = Color.gray;
        Debug.Log(x + " " + y + " Set Obstacle");
    }
    void SetNotObstacle()
    {
        isObstacle = false;
        srBlock.color = Color.white;
    }

    public void SetSpriteFontier()
    {
        
    }
    /// <summary>
    /// Allows the .Sort method based on heuristic.
    /// </summary>

}
