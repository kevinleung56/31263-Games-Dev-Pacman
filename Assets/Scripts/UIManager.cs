using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    Text score;
    Text time;

    // Start is called before the first frame update
    void Start()
    {
        score = GameObject.FindGameObjectWithTag("Score").GetComponent<Text>();
        time = GameObject.FindGameObjectWithTag("GameTimer").GetComponent<Text>();

        var highscore = PlayerPrefs.GetString("highscore", "0");
        var timeRecord = PlayerPrefs.GetString("time", "00:00:00");

        if (highscore != "0" && timeRecord != "00:00:00")
        {
            score.text = highscore;
            time.text = timeRecord;
        }
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
