using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    private GameObject item;

    [SerializeField]
    private SpriteRenderer spriteRenderer;

    [SerializeField]
    private Animator animatorController;

    [SerializeField]
    private AudioSource audioOnMoveNoPellet;

    private Tweener tweener;
    private List<GameObject> itemList;

    void Start()
    {
        itemList = new List<GameObject>();
        itemList.Add(item);
        tweener = GetComponent<Tweener>();

        // Make pac-worm move in top left automatically on play
        StartCoroutine(MovePacwormClockwiseAroundTopBlock());
    }

    IEnumerator MovePacwormClockwiseAroundTopBlock()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);

            MoveRight(new Vector3(5.0f, 0.0f, 0.0f), 1.25f);
            yield return new WaitForSeconds(1.25f);

            MoveDown(new Vector3(0.0f, -4.0f, 0.0f), 1.0f);
            yield return new WaitForSeconds(1f);

            MoveLeft(new Vector3(-5.0f, 0.0f, 0.0f), 1.25f);
            yield return new WaitForSeconds(1.25f);

            MoveUp(new Vector3(0.0f, 4.0f, 0.0f), 1.0f);
        }
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

    void OnPlayerMove()
    {
        item.transform.rotation = Quaternion.identity;
        audioOnMoveNoPellet.Play();
    }

    void MoveLeft(Vector3? vector = null, float duration = 0.25f)
    {
        var vectorToMove = vector == null ? new Vector3(-1.0f, 0.0f, 0.0f) : (Vector3)vector;
        OnPlayerMove();
        spriteRenderer.flipX = false;
        animatorController.SetBool("MoveLeftRightParam", true);
        AddTweenToPosition(itemList[0].transform.position + vectorToMove, duration);
    }

    void MoveRight(Vector3? vector = null, float duration = 0.25f)
    {
        var vectorToMove = vector == null ? new Vector3(1.0f, 0.0f, 0.0f) : (Vector3)vector;
        OnPlayerMove();
        spriteRenderer.flipX = true;
        animatorController.SetBool("MoveLeftRightParam", true);
        AddTweenToPosition(itemList[0].transform.position + vectorToMove, duration);
    }

    void MoveUp(Vector3? vector = null, float duration = 0.25f)
    {
        var vectorToMove = vector == null ? new Vector3(0.0f, 1.0f, 1.0f) : (Vector3)vector;
        OnPlayerMove();
        spriteRenderer.flipX = true;
        item.transform.Rotate(new Vector3(0, 0, 90));
        animatorController.SetBool("MoveUpDownParam", true);
        AddTweenToPosition(itemList[0].transform.position + vectorToMove, duration);
    }

    void MoveDown(Vector3? vector = null, float duration = 0.25f)
    {
        var vectorToMove = vector == null ? new Vector3(0.0f, -1.0f, 0.0f) : (Vector3)vector;
        OnPlayerMove();
        spriteRenderer.flipX = false;
        item.transform.Rotate(new Vector3(0, 0, 90));
        animatorController.SetBool("MoveUpDownParam", true);
        AddTweenToPosition(itemList[0].transform.position + vectorToMove, duration);

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.A))
        {
            MoveLeft();
        }
        else if (Input.GetKey(KeyCode.D))
        {
            MoveRight();
        }
        else if (Input.GetKey(KeyCode.W))
        {
            MoveUp();
        }
        else if (Input.GetKey(KeyCode.S))
        {
            MoveDown();
        }
        else if (Input.GetKey(KeyCode.Space))
        {
            animatorController.SetBool("WormIsDeadParam", !animatorController.GetBool("WormIsDeadParam"));
        }
    }
}
