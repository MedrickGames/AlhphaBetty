using System.Collections;
using UnityEngine;
using DG.Tweening;
using UnityEditor.Experimental.GraphView;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;

public class Tile : MonoBehaviour
{
    
    private Vector2 startMousePosition;
    private Vector2 previousMousePosition;
    public Vector2 gridPos;
    private GridManager gridManager;
    private bool isMovingDown = false;
    private float time;
    private TileManager tileManager;
    

    private void Start()
    {

        tileManager = GameObject.Find("TileManager").GetComponent<TileManager>();
        gridManager = GameObject.Find("GridManager").GetComponent<GridManager>();
        StartCoroutine(CheckBelow());
    }
    

 IEnumerator CheckBelow()
{
    float checkInterval = 0.1f; // Time interval between checks
    float moveCooldown = 0.1f; // Cooldown duration for moves
    float lastMoveTime = 0f; // Last time a move occurred

    while (true) // Keep checking for movement
    {
        if (!isMovingDown) // Only check if not already moving
        {
            isMovingDown = true;

            // Get current time
            float currentTime = Time.time;
            bool movedDown = false;

            // Check directly below
            if ((int)(gridPos.y + 1) < gridManager.gridMapAl.GetLength(1))
            {
                if (!gridManager.gridMapAl[(int)gridPos.x, (int)(gridPos.y + 1)] && 
                    currentTime >= lastMoveTime + moveCooldown)
                {
                    MoveTo(new Vector2(gridPos.x, gridPos.y + 1));
                    lastMoveTime = currentTime; // Update last move time
                    movedDown = true; // Indicate that a downward move occurred
                }
            }


            float checker = gridPos.y - 1;
            if (checker < 0f)
            {
                checker = 0;
            }
            else
            {
                checker = gridPos.y - 1;
            }

            // If not moved down, check for sliding
            if (!movedDown && !tileManager.isSpawning && (!gridManager.gridMapAl[(int)(gridPos.x ), (int)(checker)] || tileManager.portal[(int)(gridPos.x ),(int)(gridPos.y )] ) )
            {
                // Check diagonal down-right
                if ((int)(gridPos.x + 1) < gridManager.gridMapAl.GetLength(0) && 
                    (int)(gridPos.y + 1) < gridManager.gridMapAl.GetLength(1))
                {
                    if (!gridManager.gridMapAl[(int)(gridPos.x + 1), (int)(gridPos.y + 1)] && 
                        currentTime >= lastMoveTime + moveCooldown)
                    {
                        MoveTo(new Vector2(gridPos.x + 1, gridPos.y + 1));
                        lastMoveTime = currentTime; // Update last move time
                    }
                }

                // Check diagonal down-left
                if ((int)(gridPos.x - 1) >= 0 && (int)(gridPos.y + 1) < gridManager.gridMapAl.GetLength(1))
                {
                    if (!gridManager.gridMapAl[(int)(gridPos.x - 1), (int)(gridPos.y + 1)] && 
                        currentTime >= lastMoveTime + moveCooldown)
                    {
                        MoveTo(new Vector2(gridPos.x - 1, gridPos.y + 1));
                        lastMoveTime = currentTime; // Update last move time
                    }
                }
            }

            isMovingDown = false; // Reset moving status after checks
        }
        yield return new WaitForSeconds(0.1f); // Check every checkInterval seconds
    }
}

    private void MoveTo(Vector2 newPos)
    {
        gridManager.gridMapAl[(int)newPos.x, (int)newPos.y] = true;
        gridManager.gridMapAl[(int)gridPos.x, (int)gridPos.y] = false;

        // Update the grid position
        gridPos = newPos;

        // Move to the new position
        GameObject newGridPlace = gridManager.FindGridPos($"{gridPos.x},{gridPos.y}");
        Vector3 newPosition = newGridPlace.transform.position;

        transform.DOMove(newPosition, 0.2f).OnComplete(() => 
        {
            isMovingDown = false; // Reset moving status when movement is complete
        });
    }

    public void SetPosition(Vector2 pos)
    {
        this.gridPos = pos;
    }

    

}
