using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    private Camera gameCamera;

    // Start is called before the first frame update
    void Start()
    {
        gameCamera = FindObjectOfType<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(2 * transform.position - gameCamera.transform.position);
    }
}
