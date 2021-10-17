using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CherryController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static int randomSpawnSizeX = 27; // top left -8.5, 4.5
    public static int randomSpawnSizeY = 27; // bottom right 18.5, -23.5 // total size = (27, 28)
    // centre point is 5, -9.5


    IEnumerable SpawnCherryCoroutine()
    {
        var cameraViewHeight = Camera.main.orthographicSize;
        var cameraViewWidth = Camera.main.orthographicSize * Camera.main.aspect;

        var map = LevelGenerator.levelMap;

        yield return new WaitForSeconds(10);
    }
}
