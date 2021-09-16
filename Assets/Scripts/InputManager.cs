using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    private GameObject item;

    private Tweener tweener;
    private List<GameObject> itemList;

    void Start()
    {
        itemList = new List<GameObject>();
        itemList.Add(item);
        tweener = GetComponent<Tweener>();
    }

    void AddTweenToPosition(Vector3 position, float duration)
    {
        foreach (var item in itemList)
        {
            if (tweener.AddTween(item.transform, item.transform.position, position, duration))
            {
                break;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            AddTweenToPosition(new Vector3(-2.0f, 0.5f, 0.0f), 1.5f);
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            AddTweenToPosition(new Vector3(2.0f, 0.5f, 0.0f), 1.5f);
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            AddTweenToPosition(new Vector3(0.0f, 0.5f, -2.0f), 0.5f);
        }
        else if (Input.GetKeyDown(KeyCode.W))
        {
            AddTweenToPosition(new Vector3(0.0f, 0.5f, 2.0f), 0.5f);
        }
    }
}
