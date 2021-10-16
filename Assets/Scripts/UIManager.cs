using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    //[SerializeField]
    //RectTransform titleScreen;

    // Start is called before the first frame update
    void Start()
    {
        //titleScreen.sizeDelta = new Vector2(Screen.width, Screen.height);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void LoadFirstLevel()
    {
        SceneManager.LoadSceneAsync(0);
    }
}
