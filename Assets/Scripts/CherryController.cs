using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CherryController : MonoBehaviour
{
    private Tweener tweener;

    [SerializeField]
    private GameObject cherry;

    // Start is called before the first frame update
    void Start()
    {
        tweener = GetComponent<Tweener>();

        StartCoroutine(SpawnCherryCoroutine());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static int tileMapSizeX = 27; // top left -8.5, 4.5
    public static int tileMapSizeY = 28; // bottom right 18.5, -23.5 // total size = (27, 28)
    public static Vector2 centreOfMapPosition = new Vector2(5f, -9.5f); // bottom right 18.5, -23.5 // total size = (27, 28)
    // centre point is 5, -9.5


    IEnumerator SpawnCherryCoroutine()
    {
        yield return new WaitForSeconds(4); // Wait for 3, 2, 1, GO!

        while (true)
        {
            yield return new WaitForSeconds(10f);

            StartCoroutine(GenerateCherryAndTween());
        }
    }

    IEnumerator GenerateCherryAndTween()
    {
        var randomPositionOutOfBounds = GenerateRandomPointOutOfBounds();
        var newCherry = Instantiate(cherry, randomPositionOutOfBounds, Quaternion.identity);
        var finalPosition = (centreOfMapPosition - randomPositionOutOfBounds) + centreOfMapPosition;
        tweener.AddTween(newCherry.transform, newCherry.transform.position, centreOfMapPosition, 5f);
        yield return new WaitForSeconds(5f);

        if (newCherry != null) // If player has not collided already
        {
            tweener.AddTween(newCherry.transform, newCherry.transform.position, finalPosition, 5f);
            yield return new WaitForSeconds(5f);

            if (newCherry != null)
            {
                Destroy(newCherry);
            }
        }
    }

    Vector2 GenerateRandomPointOutOfBounds()
    {
        var randomPointLeftOfCameraX = Random.Range(-20.0f, -10.0f);
        var randomPointRightOfCameraX = Random.Range(20.0f, 30.0f);
        var randomPointAboveCameraY = Random.Range(-10.0f, 30.0f);
        var randomPointBelowCameraY = Random.Range(-10.0f, 30.0f);

        var takeLeftOrRight = Random.Range(0, 2);
        var takeUpOrDown = Random.Range(0, 2);

        Vector2 randomPosition;
        float randomPositionX;
        float randomPositionY;

        randomPositionX = takeLeftOrRight == 0 ? randomPointLeftOfCameraX : randomPointRightOfCameraX;
        randomPositionY = takeUpOrDown == 0 ? randomPointAboveCameraY : randomPointBelowCameraY;

        randomPosition = new Vector2(randomPositionX, randomPositionY);

        return randomPosition;
    }
}
