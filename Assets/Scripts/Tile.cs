using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;
using static TMPro.TextMeshProUGUI;
using Image = UnityEngine.UI.Image;

public class Tile : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{


    private Vector2 startMousePosition;
    private Vector2 previousMousePosition;
    public Vector2 gridPos;
    private GridManager gridManager;
    private bool isMovingDown = false;
    private float time;
    private TileManager tileManager;

    private Image image;
   

    public TextMeshProUGUI letterHolder;

    public GameObject thisTile;
    public char letter;
    private bool isPointerDown = false;
    private bool isSelected = false;
    private bool isSentToWord = false;

    private void Start()
    {
        image = GetComponent<Image>();
        
        letter = GameObject.Find("LetterManager").GetComponent<LetterManager>().GetRandomLetter();
        letterHolder.text = ""+letter;
        
        tileManager = GameObject.Find("TileManager").GetComponent<TileManager>();
        gridManager = GameObject.Find("GridManager").GetComponent<GridManager>();
        StartCoroutine(CheckBelow());
    }

    private void Update()
    {
        SelectCheck();
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
                if (!movedDown && !tileManager.isSpawning &&
                    (!gridManager.gridMapAl[(int)(gridPos.x), (int)(checker)] ||
                     tileManager.portal[(int)(gridPos.x), (int)(gridPos.y)]) &&
                    gridManager.gridMapAl[(int)gridPos.x, (int)(gridPos.y + 1)])
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
    
    public void OnPointerDown(PointerEventData eventData)
    {
        this.image.color = Color.cyan;
        if (!isSentToWord)
        {
            // GameObject.Find("GameManager").GetComponent<GameManger>().word += ""+letter;
            // isSentToWord = true;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        this.image.color = Color.white;
        // isSentToWord = false;
    }

public void SelectCheck()
{
    GameManger gameManager = GameObject.Find("GameManager").GetComponent<GameManger>(); // Cache GameManager reference
    Tile lastTile = gameManager.lastTile; // Cache lastTile from GameManager

    // Check if the pointer is currently down
    if (Input.GetMouseButton(0)) // For touch, use Input.touchCount > 0
    {
        isPointerDown = true;

        // Check if the pointer is over a UI element
        if (IsPointerOverUIElement())
        {
            // Check if this tile is adjacent to the last selected tile or part of the previously selected tiles
            bool isAdjacent = lastTile == null || 
                              (Mathf.Abs(gridPos.x - lastTile.gridPos.x) <= 1 && Mathf.Abs(gridPos.y - lastTile.gridPos.y) <= 1);

            // If the tile is previously selected and we are hovering over it again
            if (gameManager.selectedTiles.Contains(this))
            {
                // Deselect all tiles after this one in the selection history
                int index = gameManager.selectedTiles.IndexOf(this);

                for (int i = gameManager.selectedTiles.Count - 1; i > index; i--)
                {
                    Tile tileToDeselect = gameManager.selectedTiles[i];
                    tileToDeselect.isSelected = false;
                    tileToDeselect.image.color = Color.white;
                    gameManager.selectedTiles.RemoveAt(i); // Remove deselected tiles from history
                }

                // Set this tile as the current last tile
                gameManager.lastTile = this;
            }
            else if (isAdjacent && !isSelected) // Select if adjacent and not already selected
            {
                // Mark this tile as selected
                isSelected = true;
                isSentToWord = true;
                image.color = Color.cyan;

                // Add this tile's letter to the word
                gameManager.word += letter;

                // Add this tile to the selection history
                gameManager.selectedTiles.Add(this);

                // Set this tile as the current last tile
                gameManager.lastTile = this;
            }
        }
    }
    else
    {
        // Reset all selections when pointer is released
        isPointerDown = false;
        isSelected = false;
        isSentToWord = false;
        image.color = Color.white;

        // Clear the word and reset the selection history
        gameManager.word = "";
        gameManager.lastCheckedWord = "";
        gameManager.lastTile = null;

        // Reset colors of all previously selected tiles
        foreach (Tile tile in gameManager.selectedTiles)
        {
            tile.isSelected = false;
            tile.image.color = Color.white;
        }

        // Clear the selection history list
        gameManager.selectedTiles.Clear();
    }
}


    private bool IsPointerOverUIElement()
    {
        RectTransform rectTransform = GetComponent<RectTransform>();
        Vector2 localMousePosition = rectTransform.InverseTransformPoint(Input.mousePosition);
        return rectTransform.rect.Contains(localMousePosition);

    }
}
