using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquareController : MonoBehaviour
{
    #region variables
    public Sprite startSprite;
    public Sprite flagSprite;
    SpriteRenderer srBlock;
    public Sprite baseBlockSprite;
    SceneController sceneController;

    public bool isObstacle = false;
    public int x, y;

    //Movement cost is set dynamically. Refered to commonly as g.
    private int movementCost;
    public int MovementCost
    {
        get { return movementCost; }
        set { movementCost = value; }
    }



    #endregion

    void Awake()
    {
        sceneController = FindObjectOfType<SceneController>().GetComponent<SceneController>();
        srBlock = this.GetComponent<SpriteRenderer>();
    }


    private void OnMouseDown()
    {
        if (sceneController.isSetStart)
        {
            SetStart();

        }
        else if (sceneController.isSetGoal)
        {
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
