using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public Animator animatorController;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            spriteRenderer.flipX = false;
            animatorController.SetBool("MoveLeftRightParamB", true);
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            spriteRenderer.flipX = true;
            animatorController.SetBool("MoveLeftRightParamB", true);
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            spriteRenderer.flipY = false;
            animatorController.SetBool("MoveUpDownParamB", true);
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            spriteRenderer.flipY = true;
            animatorController.SetBool("MoveUpDownParamB", true);
        }
    }
}
