using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    RectTransform titleScreen;

    // Start is called before the first frame update
    void Start()
    {
        titleScreen.sizeDelta = new Vector2(Screen.width, Screen.height);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
