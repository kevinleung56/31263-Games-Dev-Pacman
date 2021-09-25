using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class LevelGenerator : MonoBehaviour
{
    public Tilemap map;
    public GameObject[] tileObjects;
    private GameObject newTile;
    private GameObject gamemap;

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
        gamemap = new GameObject("Map");
        var tilemap = GameObject.Find("Tilemap");
        var tilemapComponent = tilemap.GetComponent<Tilemap>();
        var apples = GameObject.FindGameObjectsWithTag("Apple");
        foreach(var apple in apples)
        {
            Destroy(apple); // Delete the manual level 0 layout apples
        }
        tilemapComponent.ClearAllTiles(); // Clear manual level 0 layout

        GenerateLevel0Layout(); // Second quadrant
        gamemap.transform.Rotate(new Vector3(0, 0, -90f));
        gamemap.transform.position = new Vector2(-8.5f, 4.5f);

        GenerateQuadrant(
            new Vector2(18.5f, 4.5f),
            new Vector2(-1, 1),
            Quaternion.Euler(0, 0, 90)); // First quadrant

        GenerateQuadrant(
            new Vector2(-8.5f, -23.5f), // Y-axis overlap by 1 unit to make way for tunnel
            new Vector2(1, -1),
            Quaternion.Euler(0, 0, 90)); // Third quadrant

        GenerateQuadrant(
            new Vector2(18.5f, -23.5f), // Y-axis overlap by 1 unit to make way for tunnel
            new Vector2(-1, -1),
            Quaternion.Euler(0, 0, -90)); // Fourth quadrant
    }

    void GenerateLevel0Layout()
    {
        for (int x = 0; x < levelMap.GetLength(0); x++) // First dimension elements
        {
            for (int y = 0; y < levelMap.GetLength(1); y++) // Second dimension elements
            {
                var spriteValue = levelMap[x, y];

                if (spriteValue != 0) // If not empty tile
                {
                    newTile = Instantiate(tileObjects[spriteValue], new Vector2(x, y), Quaternion.identity, gamemap.transform);
                    newTile.transform.rotation = SetRotation(x, y, spriteValue);
                }
            }
        }
    }

    void GenerateQuadrant(Vector3 position, Vector3 scale, Quaternion rotation)
    {
        GameObject quadrant = Instantiate(gamemap, position, rotation, map.transform.parent.transform);
        quadrant.transform.localScale = scale;
    }

    Quaternion SetRotation(int x, int y, int spriteValue)
    {
        switch (spriteValue)
        {
            case 1: return CornerRotation(x, y);
            case 2: return WallRotation(x, y);
            case 3: return CornerRotation(x, y);
            case 4: return WallRotation(x, y);
            case 6: return Quaternion.Euler(0, 0, 90);
            case 7: return Quaternion.Euler(0, 0, 90);
            default: return Quaternion.identity;
        }
    }

    Quaternion CornerRotation(int x, int y) // Get rotation of corner from coordinate
    {
        var leftExists = Physics2D.Raycast(new Vector2(x, y), -newTile.transform.right, 1);
        var downExists = Physics2D.Raycast(new Vector2(x, y), -newTile.transform.up, 1);
        if (leftExists && downExists) // If in contact with left and down
        {
            if (levelMap[x - 1, y] == 3) // If left is inside corner
            {
                return Quaternion.Euler(0, 0, leftExists.collider.transform.rotation.eulerAngles.z - 90f);
            }
            else if (levelMap[x + 1, y] == 4 && levelMap[x, y - 1] == 4) // If right & below is inside wall
            {
                return Quaternion.identity;
            }
            else if (
                levelMap[x - 1, y] == 4 && levelMap[x, y - 1] == 4
                && (int)leftExists.transform.rotation.eulerAngles.z  // If left & below is inside wall
                == (int)downExists.transform.rotation.eulerAngles.z) // And have same rotation
            {
                return Quaternion.Euler(0, 0, 180);
            }
            else
            {
                return Quaternion.Euler(0, 0, -90);
            }
        }
        else if (leftExists) // If left contact only
        {
            return Quaternion.Euler(0, 0, 180);
        }
        else if (downExists) // If down contact only
        {
            return Quaternion.identity;
        }
        else
        {
            return Quaternion.Euler(0, 0, 90);
        }
    }

    Quaternion WallRotation(int x, int y) // Get rotation of wall from coordinate
    {
        var leftExists = Physics2D.Raycast(new Vector2(x, y), -newTile.transform.right, 1);
        if (leftExists) // If left contact
        {
            if (leftExists.collider.gameObject.GetComponent<SpriteRenderer>().sprite // If left is wall
                == newTile.GetComponent<SpriteRenderer>().sprite)
            {
                return leftExists.collider.transform.rotation * Quaternion.identity; // Continue the wall
            }
            else
            {
                return Quaternion.Euler(0, 0, 90); // If left not wall, use vertical wall
            }
        }
        else
        {
            return Quaternion.identity;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
