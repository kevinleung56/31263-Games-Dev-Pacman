using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostMovement : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public Animator animatorController;
    public GameObject ghost;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            ghost.transform.rotation = Quaternion.identity;
            spriteRenderer.flipX = false;
            animatorController.SetBool("MoveLeftRightParam", true);
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            ghost.transform.rotation = Quaternion.identity;
            spriteRenderer.flipX = true;
            animatorController.SetBool("MoveLeftRightParam", true);
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            ghost.transform.rotation = Quaternion.identity;
            spriteRenderer.flipX = true;
            ghost.transform.Rotate(new Vector3(0, 0, 90));
            animatorController.SetBool("MoveUpDownParam", true);
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            ghost.transform.rotation = Quaternion.identity;
            spriteRenderer.flipX = false;
            ghost.transform.Rotate(new Vector3(0, 0, 90));
            animatorController.SetBool("MoveUpDownParam", true);
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            animatorController.SetBool("AntIsDeadParam", !animatorController.GetBool("AntIsDeadParam"));
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            animatorController.SetBool("AntIsScaredParam", !animatorController.GetBool("AntIsScaredParam"));
        }
    }
}
