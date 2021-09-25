using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class LevelGenerator : MonoBehaviour
{
    public Tilemap map;
    public GameObject[] tileObjects;

    int[,] levelMap =
    {
        {1,2,2,2,2,2,2,2,2,2,2,2,2,7},
        {2,5,5,5,5,5,5,5,5,5,5,5,5,4},
        {2,5,3,4,4,3,5,3,4,4,4,3,5,4},
        {2,6,4,0,0,4,5,4,0,0,0,4,5,4},
        {2,5,3,4,4,3,5,3,4,4,4,3,5,3},
        {2,5,5,5,5,5,5,5,5,5,5,5,5,5},
        {2,5,3,4,4,3,5,3,3,5,3,4,4,4},
        {2,5,3,4,4,3,5,4,4,5,3,4,4,3},
        {2,5,5,5,5,5,5,4,4,5,5,5,5,4},
        {1,2,2,2,2,1,5,4,3,4,4,3,0,4},
        {0,0,0,0,0,2,5,4,3,4,4,3,0,3},
        {0,0,0,0,0,2,5,4,4,0,0,0,0,0},
        {0,0,0,0,0,2,5,4,4,0,3,4,4,0},
        {2,2,2,2,2,1,5,3,3,0,4,0,0,0},
        {0,0,0,0,0,0,5,0,0,0,4,0,0,0},
    };

    // Start is called before the first frame update
    void Start()
    {
        //var tilemap = map.GetComponent<Tilemap>();
        //var renderer = map.GetComponent<TilemapRenderer>();
        //var anchor = tilemap.tileAnchor;
        //var cellBounds = tilemap.cellBounds;
        //var width = cellBounds.size.x;
        //var height = cellBounds.size.y;

        //Destroy(tilemap); // Remove layout for level 01
        //Destroy(renderer); // Remove layout for level 01

        //proceduralTilemap = map.AddComponent<Tilemap>();
        //proceduralTilemap.tileAnchor = anchor;
        //proceduralTilemapRenderer = map.AddComponent<TilemapRenderer>();

        //proceduralTilemap.origin = new Vector3Int(0, 0, 0);
        //proceduralTilemap.size = new Vector3Int(width, height, 0);
        //var newCellBounds = proceduralTilemap.cellBounds;
        //var newWidth = newCellBounds.size.x;
        //var newHeight = newCellBounds.size.y;
        //var allTiles = proceduralTilemap.GetTilesBlock(newCellBounds);

        for (int x = 0; x < levelMap.GetLength(0); x++) // First dimension elements
        {
            for (int y = 0; y < levelMap.GetLength(1); y++) // Second dimension elements
            {
                var spriteValue = levelMap[x, y];

                if (spriteValue != 0) // If not empty
                {
                    var newTile = Instantiate(tileObjects[spriteValue], new Vector2(x, y), Quaternion.identity, map.transform.parent.transform);
                    newTile.transform.rotation = setRotation(x, y, spriteValue);
                }
            }
        }
    }

    Quaternion setRotation(int x, int y, int spriteValue)
    {
        switch (spriteValue)
        {
            case 1: return cornerRotation(x, y);
            case 2: return wallRotation(x, y);
            case 3: return cornerRotation(x, y);
            case 4: return wallRotation(x, y);
            default: return Quaternion.identity;
        }
    }

    Quaternion cornerRotation(int x, int y) // Get rotation of corner from coordinate
    {
        return Quaternion.identity;
    }

    Quaternion wallRotation(int x, int y) // Get rotation of wall from coordinate
    {
        return Quaternion.identity;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
