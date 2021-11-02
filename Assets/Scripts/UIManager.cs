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
    Tweener tweener;
    private List<GameObject> itemList;
    public GameObject apple;
    public GameObject banana;
    public GameObject bread;
    public GameObject heart;

    // Start is called before the first frame update
    void Start()
    {
        itemList = new List<GameObject>();
        itemList.Add(apple);
        itemList.Add(banana);
        itemList.Add(bread);
        itemList.Add(heart);

        tweener = GetComponent<Tweener>();
        score = GameObject.FindGameObjectWithTag("Score").GetComponent<Text>();
        time = GameObject.FindGameObjectWithTag("GameTimer").GetComponent<Text>();

        var highscore = PlayerPrefs.GetString("highscore", "0");
        var timeRecord = PlayerPrefs.GetString("time", "00:00:00");

        if (highscore != "0" && timeRecord != "00:00:00")
        {
            score.text = highscore;
            time.text = timeRecord;
        }

        StartCoroutine(BananaCoroutine());
        StartCoroutine(AppleCoroutine());
        StartCoroutine(BreadCoroutine());
        StartCoroutine(HeartCoroutine());
    }

    IEnumerator BananaCoroutine()
    {
        var banana = itemList[1];

        while (true)
        {
            yield return new WaitForSeconds(1.25f);

            Move(banana, new Vector3(0f, -5), 1.25f);
            yield return new WaitForSeconds(1.25f);

            Move(banana, new Vector3(0.0f, 10f), 1.25f);
            yield return new WaitForSeconds(1.25f);

            Move(banana, new Vector3(0f, -10f), 1.25f);
            yield return new WaitForSeconds(1.25f);

            Move(banana, new Vector3(0.0f, 5f), 1.25f);
            yield return new WaitForSeconds(1.25f);
        }
    }

    IEnumerator AppleCoroutine()
    {
        var apple = itemList[0];

        while (true)
        {
            yield return new WaitForSeconds(1.25f);

            Move(apple, new Vector3(0f, 5), 1.25f);
            yield return new WaitForSeconds(1.25f);

            Move(apple, new Vector3(0.0f, -10f), 1.25f);
            yield return new WaitForSeconds(1.25f);

            Move(apple, new Vector3(0f, 10f), 1.25f);
            yield return new WaitForSeconds(1.25f);

            Move(apple, new Vector3(0.0f, -5f), 1.25f);
            yield return new WaitForSeconds(1.25f);
        }
    }

    IEnumerator BreadCoroutine()
    {
        var bread = itemList[2];

        while (true)
        {
            yield return new WaitForSeconds(1.25f);

            Move(bread, new Vector3(-3, 0), 1.25f);
            yield return new WaitForSeconds(1.25f);

            Move(bread, new Vector3(6f, 0), 1.25f);
            yield return new WaitForSeconds(1.25f);

            Move(bread, new Vector3(-6f, 0), 1.25f);
            yield return new WaitForSeconds(1.25f);

            Move(bread, new Vector3(3f, 0), 1.25f);
            yield return new WaitForSeconds(1.25f);
        }
    }

    IEnumerator HeartCoroutine()
    {
        var heart = itemList[3];

        while (true)
        {
            yield return new WaitForSeconds(1.25f);

            Move(heart, new Vector3(3, 0), 1.25f);
            yield return new WaitForSeconds(1.25f);

            Move(heart, new Vector3(-6f, 0), 1.25f);
            yield return new WaitForSeconds(1.25f);

            Move(heart, new Vector3(6f, 0), 1.25f);
            yield return new WaitForSeconds(1.25f);

            Move(heart, new Vector3(-3f, 0), 1.25f);
            yield return new WaitForSeconds(1.25f);
        }
    }


    void Move(GameObject item, Vector3 vector, float duration = 0.25f)
    {
        AddTweenToPosition(item, item.transform.position + vector, duration);
    }

    void AddTweenToPosition(GameObject item, Vector3 position, float duration)
    {
        tweener.AddTween(item.transform, item.transform.position, position, duration);
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
        StopAllCoroutines();
        SceneManager.LoadSceneAsync(0);
    }
}
