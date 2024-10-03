using System;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public int columns = 5;
    public int rows = 5;
    public float tileSize = 100f;

    public RectTransform gridParent; 
    public GameObject tilePrefab;

    public RectTransform[,] grid;
    public bool[,] gridMapAl;
    public Dictionary<String, GameObject> tileDictionary = new Dictionary<String, GameObject>();

    

    void Awake()
    {
        CreateGrid(1);
        PrintAllKeys();
    }

    void CreateGrid()
    {
        grid = new RectTransform[columns, rows];
        gridMapAl = new bool[columns, rows];
        
        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < columns; x++)
            {
                GameObject tile = Instantiate(tilePrefab, gridParent);
                RectTransform tileRect = tile.GetComponent<RectTransform>();
                tileRect.anchoredPosition = new Vector2(x * tileSize, -y * tileSize); // Negative y because of UI coordinate system
                tile.name = $"[{x},{y}]";
                grid[x, y] = tileRect;

                String gridPos = $"{x},{y}";
                
                tileDictionary.Add(gridPos, tile);

               
            }
        }
    }

    //to create what shape you like
    void CreateGrid(int shape)
    {
        CreateGrid();
        deleteGrid("1,0");
        deleteGrid("2,0");
       
        deleteGrid("4,0");
        deleteGrid("5,0");
        deleteGrid("6,0");
        deleteGrid("0,0");
        deleteGrid("0,1");
        deleteGrid("0,2");
        deleteGrid("0,3");
        deleteGrid("0,4");
        deleteGrid("0,5");
        deleteGrid("0,6");
    }

    public bool isEmpty()
    {
        return false;
    }
    
    public void PrintAllKeys()
    {
        foreach (var key in tileDictionary.Keys)
        {
            //Debug.Log($"Key in tileDictionary: {key}");
        }
    }

    public GameObject FindGridPos(String pos)
    {
        if (tileDictionary.TryGetValue(pos, out GameObject result))
        {
            return result;
        }
        else
        {
            Debug.LogError($"Key '{pos}' not found in the dictionary.");
            return null;
        }
    }
    public void deleteGrid(String position)
    {
        GameObject target = FindGridPos(position);
        Destroy(target.gameObject);
        tileDictionary.Remove(position);
    }
   
 
}