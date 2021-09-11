using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public Animator animatorController;
    public GameObject player;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            player.transform.rotation = Quaternion.identity;
            spriteRenderer.flipX = false;
            animatorController.SetBool("MoveLeftRightParam", true);
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            player.transform.rotation = Quaternion.identity;
            spriteRenderer.flipX = true;
            animatorController.SetBool("MoveLeftRightParam", true);
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            player.transform.rotation = Quaternion.identity;
            spriteRenderer.flipX = true;
            player.transform.Rotate(new Vector3(0, 0, 90));
            animatorController.SetBool("MoveUpDownParam", true);
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            player.transform.rotation = Quaternion.identity;
            spriteRenderer.flipX = false;
            player.transform.Rotate(new Vector3(0, 0, 90));
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
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            animatorController.SetBool("WormIsDeadParam", !animatorController.GetBool("WormIsDeadParam"));
        }
    }
}
