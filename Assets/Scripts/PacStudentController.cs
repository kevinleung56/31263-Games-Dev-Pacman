using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;

public class PacStudentController : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    private GameObject pacStudent;

    [SerializeField]
    private SpriteRenderer spriteRenderer;

    [SerializeField]
    private Animator animatorController;

    [SerializeField]
    private AudioSource audioOnMoveNoPellet;

    [SerializeField]
    private ParticleSystem dust;

    private Tweener tweener;
    private GameObject scoreObject;
    private GameObject ghostTimerObject;
    private GameObject gameTimerObject;
    private Text gameTimer;
    private Text score;
    private Stopwatch watch;
    private KeyCode? lastInput;
    private KeyCode? currentInput;
    private enum Directions { Up, Down, Left, Right };

    void Start()
    {
        tweener = GetComponent<Tweener>();
        scoreObject = GameObject.FindGameObjectWithTag("Score");
        ghostTimerObject = GameObject.FindGameObjectWithTag("GhostTimer");
        gameTimerObject = GameObject.FindGameObjectWithTag("GameTimer");

        score = scoreObject.GetComponent<Text>();
        gameTimer = gameTimerObject.GetComponent<Text>();
        watch = new Stopwatch();
        watch.Start();

        ghostTimerObject.SetActive(false);
    }

    void AddTweenToPosition(Vector3 position, float duration)
    {
        tweener.AddTween(pacStudent.transform, pacStudent.transform.position, position, duration);
    }

    void OnPlayerMove()
    {
        pacStudent.transform.rotation = Quaternion.identity;
        audioOnMoveNoPellet.Play();
        CreateDustTrail();
    }

    bool IsBlockedByWall(Directions directionToGo)
    {
        var facingDirection = IdentifyFacingDirection();
        Vector2? direction = null;

        if (facingDirection == Directions.Up || facingDirection == Directions.Down)
        {
            if (directionToGo == Directions.Up)
            {
                direction = pacStudent.transform.right;
            }
            else if (directionToGo == Directions.Down)
            {
                direction = -pacStudent.transform.right;
            }
            else if (directionToGo == Directions.Left)
            {
                direction = pacStudent.transform.up;
            }
            else if (directionToGo == Directions.Right)
            {
                direction = -pacStudent.transform.up;
            }
        }
        else
        {
            if (directionToGo == Directions.Up)
            {
                direction = pacStudent.transform.up;
            }
            else if (directionToGo == Directions.Down)
            {
                direction = -pacStudent.transform.up;
            }
            else if (directionToGo == Directions.Left)
            {
                direction = -pacStudent.transform.right;
            }
            else if (directionToGo == Directions.Right)
            {
                direction = pacStudent.transform.right;
            }
        }

        if (direction != null)
        {
            var hit = Physics2D.Raycast(pacStudent.transform.position, (Vector2)direction, 1);

            if (hit.collider != null && hit.collider.gameObject.CompareTag("Wall"))
            {
                return true;
            }
        }

        return false;
    }

    Directions IdentifyFacingDirection()
    {
        if (spriteRenderer.flipX) // Direction is facing up or right
        {
            if (pacStudent.transform.rotation == Quaternion.Euler(new Vector3(0, 0, 90))) // Up
            {
                return Directions.Up;
            }

            return Directions.Right;
        }
        else // Direction is facing left or down
        {
            if (pacStudent.transform.rotation == Quaternion.Euler(new Vector3(0, 0, 90))) // Down
            {
                return Directions.Down;
            }

            return Directions.Left;
        }
    }

    bool MoveLeft(Vector3? vector = null, float duration = 0.25f)
    {
        if (IsBlockedByWall(Directions.Left))
        {
            animatorController.SetBool("MoveLeftRightParam", false);
            StopDustTrail();
            return false;
        }

        var vectorToMove = vector == null ? new Vector3(-1.0f, 0.0f, 0.0f) : (Vector3)vector;
        OnPlayerMove();
        spriteRenderer.flipX = false;
        animatorController.SetBool("MoveLeftRightParam", true);
        AddTweenToPosition(pacStudent.transform.position + vectorToMove, duration);
        return true;
    }

    bool MoveRight(Vector3? vector = null, float duration = 0.25f)
    {
        if (IsBlockedByWall(Directions.Right))
        {
            animatorController.SetBool("MoveLeftRightParam", false);
            StopDustTrail();
            return false;
        }

        var vectorToMove = vector == null ? new Vector3(1.0f, 0.0f, 0.0f) : (Vector3)vector;
        OnPlayerMove();
        spriteRenderer.flipX = true;
        animatorController.SetBool("MoveLeftRightParam", true);
        AddTweenToPosition(pacStudent.transform.position + vectorToMove, duration);
        return true;
    }

    bool MoveUp(Vector3? vector = null, float duration = 0.25f)
    {
        if (IsBlockedByWall(Directions.Up)) // When rotated, right becomes Pac-student's forward
        {
            animatorController.SetBool("MoveUpDownParam", false);
            StopDustTrail();
            return false;
        }

        var vectorToMove = vector == null ? new Vector3(0.0f, 1.0f, 1.0f) : (Vector3)vector;
        OnPlayerMove();
        spriteRenderer.flipX = true;
        pacStudent.transform.Rotate(new Vector3(0, 0, 90));
        animatorController.SetBool("MoveUpDownParam", true);
        AddTweenToPosition(pacStudent.transform.position + vectorToMove, duration);
        return true;
    }

    bool MoveDown(Vector3? vector = null, float duration = 0.25f)
    {
        if (IsBlockedByWall(Directions.Down))
        {
            animatorController.SetBool("MoveUpDownParam", false);
            StopDustTrail();
            return false;
        }

        var vectorToMove = vector == null ? new Vector3(0.0f, -1.0f, 0.0f) : (Vector3)vector;
        OnPlayerMove();
        spriteRenderer.flipX = false;
        pacStudent.transform.Rotate(new Vector3(0, 0, 90));
        animatorController.SetBool("MoveUpDownParam", true);
        AddTweenToPosition(pacStudent.transform.position + vectorToMove, duration);
        return true;
    }

    // Update is called once per frame
    void Update()
    {
        var currentTime = watch.Elapsed;
        var currentTimeFormatted = string.Format("{0:00}:{1:00}:{2:00}", currentTime.Minutes, currentTime.Seconds, currentTime.Milliseconds / 10);

        gameTimer.text = currentTimeFormatted;


        if ((Vector2)pacStudent.transform.position == new Vector2(19.5f, -9.5f)) // Right tunnel
        {
            pacStudent.transform.position = new Vector2(-8.5f, -9.5f); // Teleportation
        }
        else if ((Vector2)pacStudent.transform.position == new Vector2(-9.5f, -9.5f)) // Left tunnel
        {
            pacStudent.transform.position = new Vector2(18.5f, -9.5f); // Teleportation
        }

        if (Input.GetKey(KeyCode.A))
        {
            MoveLeft();
            lastInput = KeyCode.A;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            MoveRight();
            lastInput = KeyCode.D;
        }
        else if (Input.GetKey(KeyCode.W))
        {
            MoveUp();
            lastInput = KeyCode.W;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            MoveDown();
            lastInput = KeyCode.S;
        }
        //else if (Input.GetKey(KeyCode.Space))
        //{
        //    animatorController.SetBool("WormIsDeadParam", !animatorController.GetBool("WormIsDeadParam"));
        //}
        else if (!tweener.TweenExists(pacStudent.transform) && lastInput != null)
        {
            switch(lastInput)
            {
                case KeyCode.A:
                    if (MoveLeft())
                    {
                        currentInput = lastInput;
                    }
                    else
                    {
                        MoveCurrentInput();
                    }
                    break;
                case KeyCode.D:
                    if (MoveRight())
                    {
                        currentInput = lastInput;
                    }
                    else
                    {
                        MoveCurrentInput();
                    }
                    break;
                case KeyCode.W:
                    if (MoveUp())
                    {
                        currentInput = lastInput;
                    }
                    else
                    {
                        MoveCurrentInput();
                    }
                    break;
                case KeyCode.S:
                    if (MoveDown())
                    {
                        currentInput = lastInput;
                    }
                    else
                    {
                        MoveCurrentInput();
                    }
                    break;
                default: return;
            }
        }
    }

    void MoveCurrentInput()
    {
        if (currentInput != null)
        {
            switch (currentInput)
            {
                case KeyCode.A:
                    if (!MoveLeft())
                    {
                        return; // Do nothing
                    }
                    break;
                case KeyCode.D:
                    if (!MoveRight())
                    {
                        return;
                    }
                    break;
                case KeyCode.W:
                    if (!MoveUp())
                    {
                        return;
                    }
                    break;
                case KeyCode.S:
                    if (!MoveDown())
                    {
                        return;
                    }
                    break;
                default: break;
            }
        }
    }

    void CreateDustTrail()
    {
        dust.Play();
    }

    void StopDustTrail()
    {
        if (dust.isPlaying)
        {
            dust.Stop();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //UnityEngine.Debug.Log("Collision Enter: " + collision.gameObject + " : " + collision.transform.position);
        if (collision.gameObject.CompareTag("Pellet"))
        {
            // Add 10 to score
            score.text = (int.Parse(score.text) + 10).ToString();
            Destroy(collision.gameObject);
        }
        else if (collision.gameObject.CompareTag("Cherry"))
        {
            // Add 100 to score
            score.text = (int.Parse(score.text) + 100).ToString();
            Destroy(collision.gameObject);
        }
        else if (collision.gameObject.CompareTag("PowerPellet"))
        {
            /* 
             * Change the Ghost animator state to Scared.
             * 
             * Change the background music to match this state.
             * 
             * Start a timer for 10 seconds. Make the Ghost Timer UI element visible and set it to this timer.
             * 
             * With 3 seconds left to go on this timer, change the Ghosts to the Recovering state.
             * 
             * After 10 seconds have passed, set the Ghosts back to their Walking states
             * and hide the Ghost Timer UI element.*/


        }
        else if (collision.gameObject.CompareTag("NormalAnt"))
        {
            /*
             * PacStudent dies and loses a life. Update the UI element for this.
             *
             * Play a particle effect around PacStudent the spot of PacStudents death.
             *
             * Respawn PacStudent by moving them to the top‐left hand corner,
             * where they started the game, and wait for player input.*/


        }
        else if (collision.gameObject.CompareTag("ScaredAnt"))
        {
            /*
             * PacStudent dies and loses a life. Update the UI element for this.
             *
             * Play a particle effect around PacStudent the spot of PacStudents death.
             *
             * Respawn PacStudent by moving them to the top‐left hand corner,
             * where they started the game, and wait for player input.*/


        }
    }


}
