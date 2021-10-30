using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostController : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    private GameObject ghost;

    [SerializeField]
    private SpriteRenderer spriteRenderer;

    [SerializeField]
    private Animator animatorController;

    //[SerializeField]
    //private AudioSource audioOnMoveNoPellet;

    [SerializeField]
    private ParticleSystem dust;

    private Tweener tweener;
    private GameObject pacStudent;
    private Vector2 pacStudentPosition;
    private PacStudentController pacStudentController;
    private bool isDead = false;
    private bool isScared = false;
    private int ghostType = -1;

    private enum Directions { Up, Down, Left, Right };

    IEnumerator StartUpCoroutine()
    {
        yield return new WaitForSeconds(4f); // Wait for 3, 2, 1, GO!

        pacStudent = GameObject.FindGameObjectWithTag("Worm");
        pacStudentPosition = pacStudent.transform.position;
        pacStudentController = pacStudent.GetComponent<PacStudentController>();
    }


    void Start()
    {
        tweener = GetComponent<Tweener>();
        StartCoroutine(StartUpCoroutine());
        ghostType = 1; // Get this from the HUD canvas
    }

    void AddTweenToPosition(Vector3 position, float duration)
    {
        tweener.AddTween(ghost.transform, ghost.transform.position, position, duration);
    }

    void OnGhostMove()
    {
        ghost.transform.rotation = Quaternion.identity;
        //audioOnMoveNoPellet.Play();
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
                direction = ghost.transform.right;
            }
            else if (directionToGo == Directions.Down)
            {
                direction = -ghost.transform.right;
            }
            else if (directionToGo == Directions.Left)
            {
                direction = ghost.transform.up;
            }
            else if (directionToGo == Directions.Right)
            {
                direction = -ghost.transform.up;
            }
        }
        else
        {
            if (directionToGo == Directions.Up)
            {
                direction = ghost.transform.up;
            }
            else if (directionToGo == Directions.Down)
            {
                direction = -ghost.transform.up;
            }
            else if (directionToGo == Directions.Left)
            {
                direction = -ghost.transform.right;
            }
            else if (directionToGo == Directions.Right)
            {
                direction = ghost.transform.right;
            }
        }

        if (direction != null)
        {
            var hit = Physics2D.Raycast(ghost.transform.position, (Vector2)direction, 1);

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
            if (ghost.transform.rotation == Quaternion.Euler(new Vector3(0, 0, 90))) // Up
            {
                return Directions.Up;
            }

            return Directions.Right;
        }
        else // Direction is facing left or down
        {
            if (ghost.transform.rotation == Quaternion.Euler(new Vector3(0, 0, 90))) // Down
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
        OnGhostMove();
        spriteRenderer.flipX = false;
        animatorController.SetBool("MoveLeftRightParam", true);
        AddTweenToPosition(ghost.transform.position + vectorToMove, duration);
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
        OnGhostMove();
        spriteRenderer.flipX = true;
        animatorController.SetBool("MoveLeftRightParam", true);
        AddTweenToPosition(ghost.transform.position + vectorToMove, duration);
        return true;
    }

    bool MoveUp(Vector3? vector = null, float duration = 0.25f)
    {
        if (IsBlockedByWall(Directions.Up)) // When rotated, right becomes Pac-student's forward
        {
            animatorController.SetBool("MoveLeftRightParam", false);
            StopDustTrail();
            return false;
        }

        var vectorToMove = vector == null ? new Vector3(0.0f, 1.0f, 1.0f) : (Vector3)vector;
        OnGhostMove();
        spriteRenderer.flipX = true;
        ghost.transform.Rotate(new Vector3(0, 0, 90));
        animatorController.SetBool("MoveLeftRightParam", true);
        AddTweenToPosition(ghost.transform.position + vectorToMove, duration);
        return true;
    }

    bool MoveDown(Vector3? vector = null, float duration = 0.25f)
    {
        if (IsBlockedByWall(Directions.Down))
        {
            animatorController.SetBool("MoveLeftRightParam", false);
            StopDustTrail();
            return false;
        }

        var vectorToMove = vector == null ? new Vector3(0.0f, -1.0f, 0.0f) : (Vector3)vector;
        OnGhostMove();
        spriteRenderer.flipX = false;
        ghost.transform.Rotate(new Vector3(0, 0, 90));
        animatorController.SetBool("MoveLeftRightParam", true);
        AddTweenToPosition(ghost.transform.position + vectorToMove, duration);
        return true;
    }

    // Update is called once per frame
    void Update()
    {
        if (!pacStudentController.gameOver)
        {
            if (!isScared && !isDead)
            {
                var nextMove = GetNextMove();
                if (nextMove != null)
                {
                    MoveGhost((Directions)nextMove);
                }
            }
            else if (isScared)
            {

            }
            else if (isDead)
            {

            }
        }
    }

    Directions Ghost1Behaviour()
    {
        var list = new List<Directions>();
        var currentPosition = ghost.transform.position;
        var headingVector = pacStudent.transform.position - currentPosition;
        var distanceToPacstudent = headingVector.magnitude;
        //var distanceIfGoingUp = (pacStudent.transform.position -
        //    (currentPosition + new Vector3(0.0f, 1.0f, 1.0f))).magnitude >= distanceToPacstudent;
        //var distanceIfGoingLeft = (pacStudent.transform.position -
        //    (currentPosition + new Vector3(-1.0f, 0.0f, 0.0f))).magnitude >= distanceToPacstudent;
        //var distanceIfGoingRight = (pacStudent.transform.position -
        //    (currentPosition + new Vector3(1.0f, 0.0f, 0.0f))).magnitude >= distanceToPacstudent;
        //var distanceIfGoingDown = (pacStudent.transform.position -
        //    (currentPosition + new Vector3(0.0f, -1.0f, 0.0f))).magnitude >= distanceToPacstudent; 
        if ((pacStudent.transform.position -
             (currentPosition + new Vector3(0.0f, 1.0f, 1.0f))).magnitude >= distanceToPacstudent)
        {
            list.Add(Directions.Up);
        }
        else if ((pacStudent.transform.position -
            (currentPosition + new Vector3(-1.0f, 0.0f, 0.0f))).magnitude >= distanceToPacstudent)
        {
            list.Add(Directions.Left);
        }
        if ((pacStudent.transform.position -
            (currentPosition + new Vector3(1.0f, 0.0f, 0.0f))).magnitude >= distanceToPacstudent)
        {
            list.Add(Directions.Right);
        }
        if ((pacStudent.transform.position -
            (currentPosition + new Vector3(0.0f, -1.0f, 0.0f))).magnitude >= distanceToPacstudent)
        {
            list.Add(Directions.Down);
        }

        var randomValidDirectionIndex = Random.Range(0, list.Count);
        return list[randomValidDirectionIndex];

        //if (distanceIfGoingUp >= distanceToPacstudent)
        //{
        //    return Directions.Up;
        //}
        //else if (distanceIfGoingLeft >= distanceToPacstudent)
        //{
        //    return Directions.Left;
        //}
        //else if (distanceIfGoingRight >= distanceToPacstudent)
        //{
        //    return Directions.Right;
        //}
        //else if (distanceIfGoingDown >= distanceToPacstudent)
        //{
        //    return Directions.Down;
        //}
    }

    Directions Ghost2Behaviour()
    {
        var list = new List<Directions>();
        var currentPosition = ghost.transform.position;
        var headingVector = pacStudent.transform.position - currentPosition;
        var distanceToPacstudent = headingVector.magnitude;
        //var distanceIfGoingUp = (pacStudent.transform.position -
        //    (currentPosition + new Vector3(0.0f, 1.0f, 1.0f))).magnitude >= distanceToPacstudent;
        //var distanceIfGoingLeft = (pacStudent.transform.position -
        //    (currentPosition + new Vector3(-1.0f, 0.0f, 0.0f))).magnitude >= distanceToPacstudent;
        //var distanceIfGoingRight = (pacStudent.transform.position -
        //    (currentPosition + new Vector3(1.0f, 0.0f, 0.0f))).magnitude >= distanceToPacstudent;
        //var distanceIfGoingDown = (pacStudent.transform.position -
        //    (currentPosition + new Vector3(0.0f, -1.0f, 0.0f))).magnitude >= distanceToPacstudent; 
        if ((pacStudent.transform.position -
             (currentPosition + new Vector3(0.0f, 1.0f, 1.0f))).magnitude <= distanceToPacstudent)
        {
            list.Add(Directions.Up);
        }
        else if ((pacStudent.transform.position -
            (currentPosition + new Vector3(-1.0f, 0.0f, 0.0f))).magnitude <= distanceToPacstudent)
        {
            list.Add(Directions.Left);
        }
        if ((pacStudent.transform.position -
            (currentPosition + new Vector3(1.0f, 0.0f, 0.0f))).magnitude <= distanceToPacstudent)
        {
            list.Add(Directions.Right);
        }
        if ((pacStudent.transform.position -
            (currentPosition + new Vector3(0.0f, -1.0f, 0.0f))).magnitude >= distanceToPacstudent)
        {
            list.Add(Directions.Down);
        }

        var randomValidDirectionIndex = Random.Range(0, list.Count);
        return list[randomValidDirectionIndex];

        //if (distanceIfGoingUp >= distanceToPacstudent)
        //{
        //    return Directions.Up;
        //}
        //else if (distanceIfGoingLeft >= distanceToPacstudent)
        //{
        //    return Directions.Left;
        //}
        //else if (distanceIfGoingRight >= distanceToPacstudent)
        //{
        //    return Directions.Right;
        //}
        //else if (distanceIfGoingDown >= distanceToPacstudent)
        //{
        //    return Directions.Down;
        //}
    }


    Directions? GetNextMove()
    {
        // Centre of map is (5, -9.5)
        //var currentPosition = ghost.transform.position;
        //var headingVector = pacStudent.transform.position - currentPosition;
        //var distanceToPacstudent = headingVector.magnitude;
        if (ghostType == 1)
        {
            MoveGhost(Ghost1Behaviour());
        }
        else if (ghostType == 2)
        {
            MoveGhost(Ghost2Behaviour());
        }
        else if (ghostType == 3)
        {
            MoveGhost((Directions)Random.Range(0, 4));
        }
        else if (ghostType == 4)
        {
            // Move clockwise around map
            MoveGhost((Directions)Random.Range(0, 4));
        }

        return null;
    }

    void MoveGhost(Directions nextMove)
    {
        if (!tweener.TweenExists(ghost.transform))
        {
            switch(nextMove)
            {
                case Directions.Left:
                    MoveLeft();
                    break;
                case Directions.Right:
                    MoveRight();
                    break;
                case Directions.Up:
                    MoveUp();
                    break;
                case Directions.Down:
                    MoveDown();
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
        UnityEngine.Debug.Log("Collision Enter: " + collision.gameObject + " : " + collision.transform.position);
        if (collision.gameObject.CompareTag("Ant"))
        {
        }
    }
}
