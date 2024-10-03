using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using Object = System.Object;


public class TileManager : MonoBehaviour
{
    public bool[,] gridMap;
    public GameObject tile;
    public GameObject tileParent;
    private GridManager gridManager;
    private int columns;
    private int rows;
    private float tileSize;
    private int portalCount;
    public bool[,] portal;
    private Vector2[] portalPosition;
    public bool isSpawning;

    void Start()
    {
        gridManager = GameObject.Find("GridManager").GetComponent<GridManager>();
        gridMap = gridManager.gridMapAl;
        columns = gridManager.columns;
        rows = gridManager.rows;
        tileSize = gridManager.tileSize - 20f;
        portal = new bool[columns, rows];
        portalPosition = new Vector2[10];
        SetPortals();
        ShowPortals();
        
        // startInstansiate(rows);
//        Vector2 firstPos = new Vector2(gridManager.FindGridPos("0,0").transform.position.x,
          //  gridManager.FindGridPos("0,0").transform.position.y);

        //gridManager.deleteGrid("0,0");
        // GameObject tile = gridManager.FindGridPos("2,0");
        // var positionX = tile.transform.position.x;
        // Debug.Log(positionX);
        // RectTransform tileO =  gridManager.grid[2, 0];
        // Debug.Log(tileO.anchoredPosition.x);
    }


    void Update()
    {
        StartCoroutine(SpawnTile());
    }

    void SetPortals()
    {
        bool isFirst = true;
        int topestY = 100;
        portalCount = 0;
        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < columns; x++)
            {
                if (gridManager.tileDictionary.TryGetValue($"{x},{y}", out GameObject result))
                {
                    if (isFirst)
                    {
                        topestY = y;
                        isFirst = false;
                        portal[x, y] = true;
                        portalPosition[portalCount] = new Vector2(x,y);
                        portalCount++;
                    }
                    else if (y > topestY)
                    {
                        break;
                    }
                    else
                    {
                        portal[x, y] = true;
                        portalPosition[portalCount] = new Vector2(x, y);
                        portalCount++;
                    }
                }
            }
        }
    }
    void ShowPortals()
    {
        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < columns; x++)
            {
                if (portal[x,y])
                {
                    Debug.Log($"[{x},{y}] is a portal");
                }
            }
        }
    }

    IEnumerator SpawnTile()
    {
        isSpawning = true;
        Vector2 port; 
        for (int i = 0; i < portalCount; i++)
        {
            port = portalPosition[i];

            // Check if the grid position is empty
            if (!gridManager.gridMapAl[(int)port.x, (int)port.y])
            {
                // Find the grid position to spawn the tile
                GameObject portalPos = gridManager.FindGridPos($"{port.x},{port.y}");

                // Instantiate the tile at the grid position
                GameObject newTile = Instantiate(tile, new Vector3(portalPos.transform.position.x, portalPos.transform.position.y), Quaternion.identity);

                // Set the tile's position and parent
                newTile.GetComponent<Tile>().SetPosition(port);
                newTile.transform.SetParent(tileParent.transform);

                // Mark the grid position as occupied
                gridManager.gridMapAl[(int)port.x, (int)port.y] = true;
            }

            // Add a small delay between spawns
            yield return new WaitForSeconds(0.1f);
            isSpawning = false;
        }
    }

    void startInstansiate(int row)
        {

            for (int x = 0; x < columns; x++)
            {
                // Instantiate(tile, gridManager.FindGridPos($"{x},{y}".t));

                // tileRect.anchoredPosition = new Vector2(x * tileSize, -y * tileSize);

            }
        }
}