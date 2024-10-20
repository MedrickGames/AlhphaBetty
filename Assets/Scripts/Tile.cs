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
    float checkInterval = 0.1f;
    float moveCooldown = 0.1f;
    float lastMoveTime = 0f;

    while (true)
    {
        if (!isMovingDown)
        {
            isMovingDown = true;
            float currentTime = Time.time;
            bool movedDown = false;

            // Check bounds before accessing array (y+1) to ensure it's within bounds
            if ((int)gridPos.x >= 0 && (int)gridPos.x < gridManager.gridMapAl.GetLength(0) &&
                (int)(gridPos.y + 1) >= 0 && (int)(gridPos.y + 1) < gridManager.gridMapAl.GetLength(1))
            {
                if (!gridManager.gridMapAl[(int)gridPos.x, (int)(gridPos.y + 1)] &&
                    currentTime >= lastMoveTime + moveCooldown)
                {
                    MoveTo(new Vector2(gridPos.x, gridPos.y + 1));
                    lastMoveTime = currentTime;
                    movedDown = true;
                }
            }

            // Handle checker value, ensuring it's within bounds
            float checker = gridPos.y - 1;
            if (checker < 0f)
            {
                checker = 0;
            }

            // Additional bounds checks
            if (!movedDown && !tileManager.isSpawning &&
                (int)gridPos.x >= 0 && (int)gridPos.x < gridManager.gridMapAl.GetLength(0) &&
                (int)checker >= 0 && (int)checker < gridManager.gridMapAl.GetLength(1) &&
                (!gridManager.gridMapAl[(int)(gridPos.x), (int)(checker)] ||
                 tileManager.portal[(int)(gridPos.x), (int)(gridPos.y)]) &&
                (int)gridPos.y + 1 < gridManager.gridMapAl.GetLength(1))
            {
                // Diagonal check: down-right (x+1, y+1) with bounds check
                if ((int)(gridPos.x + 1) >= 0 && (int)(gridPos.x + 1) < gridManager.gridMapAl.GetLength(0) &&
                    (int)(gridPos.y + 1) >= 0 && (int)(gridPos.y + 1) < gridManager.gridMapAl.GetLength(1))
                {
                    if (!gridManager.gridMapAl[(int)(gridPos.x + 1), (int)(gridPos.y + 1)] &&
                        currentTime >= lastMoveTime + moveCooldown)
                    {
                        MoveTo(new Vector2(gridPos.x + 1, gridPos.y + 1));
                        lastMoveTime = currentTime;
                    }
                }

                // Diagonal check: down-left (x-1, y+1) with bounds check
                if ((int)(gridPos.x - 1) >= 0 && (int)(gridPos.x - 1) < gridManager.gridMapAl.GetLength(0) &&
                    (int)(gridPos.y + 1) >= 0 && (int)(gridPos.y + 1) < gridManager.gridMapAl.GetLength(1))
                {
                    if (!gridManager.gridMapAl[(int)(gridPos.x - 1), (int)(gridPos.y + 1)] &&
                        currentTime >= lastMoveTime + moveCooldown)
                    {
                        MoveTo(new Vector2(gridPos.x - 1, gridPos.y + 1));
                        lastMoveTime = currentTime;
                    }
                }
            }

            isMovingDown = false;
        }

        yield return new WaitForSeconds(checkInterval);
    }
}

private void MoveTo(Vector2 newPos)
{
    // Ensure newPos is within bounds before updating the array
    if ((int)newPos.x >= 0 && (int)newPos.x < gridManager.gridMapAl.GetLength(0) &&
        (int)newPos.y >= 0 && (int)newPos.y < gridManager.gridMapAl.GetLength(1))
    {
        gridManager.gridMapAl[(int)newPos.x, (int)newPos.y] = true;
        gridManager.gridMapAl[(int)gridPos.x, (int)gridPos.y] = false;

        // Update grid position
        gridPos = newPos;

        // Find new grid position and move the object
        GameObject newGridPlace = gridManager.FindGridPos($"{gridPos.x},{gridPos.y}");
        Vector3 newPosition = newGridPlace.transform.position;

        // Move the object smoothly using DOTween
        transform.DOMove(newPosition, 0.2f).OnComplete(() =>
        {
            isMovingDown = false; // Reset after movement
        });
    }
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

                    // Remove the letter of the deselected tile from the word
                    if (gameManager.word.Length > 0)
                    {
                        gameManager.word = gameManager.word.Remove(gameManager.word.Length - 1);
                    }

                    // Deselect the tile and reset its color
                    tileToDeselect.isSelected = false;
                    tileToDeselect.image.color = Color.white;

                    // Remove the tile from the selection history
                    gameManager.selectedTiles.RemoveAt(i);
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

        // Check if the word is acceptable
        if (gameManager.isAcceptable)
        {
            // Destroy all selected tiles
            foreach (Tile tile in gameManager.selectedTiles)
            {
                gridManager.gridMapAl[(int)tile.gridPos.x, (int)tile.gridPos.y] = false;
                gameManager.isAcceptable = false;
                Destroy(tile.gameObject);
            }
        }
        else
        {
            // Reset selections if the word is not acceptable
            foreach (Tile tile in gameManager.selectedTiles)
            {
                tile.isSelected = false;
                tile.image.color = Color.white;
            }
        }

        // Clear the word and reset the selection history
        gameManager.word = "";
        gameManager.lastCheckedWord = "";
        gameManager.lastTile = null;

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
