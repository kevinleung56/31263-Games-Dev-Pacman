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
            Debug.Log(position);
            if (tweener.AddTween(item.transform, item.transform.position, position, duration))
            {
                break;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.A))
        {
            AddTweenToPosition(itemList[0].transform.position + new Vector3(-1.0f, 0.0f, 0.0f), 0.25f);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            AddTweenToPosition(itemList[0].transform.position + new Vector3(1.0f, 0.0f, 0.0f), 0.25f);
        }
        else if (Input.GetKey(KeyCode.S))
        {
            AddTweenToPosition(itemList[0].transform.position + new Vector3(0.0f, -1.0f, 0.0f), 0.25f);
        }
        else if (Input.GetKey(KeyCode.W))
        {
            AddTweenToPosition(itemList[0].transform.position + new Vector3(0.0f, 1.0f, 1.0f), 0.25f);
        }
    }
}
