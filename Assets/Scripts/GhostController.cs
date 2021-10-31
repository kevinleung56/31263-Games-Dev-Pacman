using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    private bool gameStarted = false;
    private int ghostType = -1;
    private Directions? lastDirection;

    private enum Directions { Up, Down, Left, Right };

    IEnumerator StartUpCoroutine()
    {
        yield return new WaitForSeconds(4f); // Wait for 3, 2, 1, GO!

        pacStudent = GameObject.FindGameObjectWithTag("Player");
        pacStudentPosition = pacStudent.transform.position;
        pacStudentController = pacStudent.GetComponent<PacStudentController>();
        gameStarted = true;
    }


    void Start()
    {
        tweener = GetComponent<Tweener>();
        StartCoroutine(StartUpCoroutine());
        ghostType = int.Parse(GetComponentInChildren<Text>().text);
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
        if (gameStarted && !pacStudentController.gameOver)
        {
            if (!isScared && !isDead)
            {
                if (CheckGhostIsInSpawn())
                { // Ghost is just trying to get out of spawn
                    GetOutOfSpawn();
                }
                else
                { // Ghost is outside of spawn and using ghost-specific logic
                    var nextMove = GetNextMove();
                    if (nextMove != null)
                    {
                        MoveGhost((Directions)nextMove);
                        lastDirection = nextMove;
                    }
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

    Directions? Ghost1Behaviour()
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
            if (lastDirection != Directions.Up)
            {
                list.Add(Directions.Up);
            }
        }
        else if ((pacStudent.transform.position -
            (currentPosition + new Vector3(-1.0f, 0.0f, 0.0f))).magnitude <= distanceToPacstudent)
        {
            if (lastDirection != Directions.Left)
            {
                list.Add(Directions.Left);
            }
        }
        else if ((pacStudent.transform.position -
            (currentPosition + new Vector3(1.0f, 0.0f, 0.0f))).magnitude <= distanceToPacstudent)
        {
            if (lastDirection != Directions.Right)
            {
                list.Add(Directions.Right);
            }
        }
        else if ((pacStudent.transform.position -
            (currentPosition + new Vector3(0.0f, -1.0f, 0.0f))).magnitude <= distanceToPacstudent)
        {
            if (lastDirection != Directions.Down)
            {
                list.Add(Directions.Down);
            }
        }
        
        if (list.Count == 0)
        {
            return null;
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

    Directions? Ghost2Behaviour()
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
            if (lastDirection != Directions.Up)
            {
                list.Add(Directions.Up);
            }
        }
        else if ((pacStudent.transform.position -
            (currentPosition + new Vector3(-1.0f, 0.0f, 0.0f))).magnitude >= distanceToPacstudent)
        {
            if (lastDirection != Directions.Left)
            {
                list.Add(Directions.Left);
            }
        }
        else if ((pacStudent.transform.position -
            (currentPosition + new Vector3(1.0f, 0.0f, 0.0f))).magnitude >= distanceToPacstudent)
        {
            if (lastDirection != Directions.Right)
            {
                list.Add(Directions.Right);
            }
        }
        else if ((pacStudent.transform.position -
            (currentPosition + new Vector3(0.0f, -1.0f, 0.0f))).magnitude >= distanceToPacstudent)
        {
            if (lastDirection != Directions.Down)
            {
                list.Add(Directions.Down);
            }
        }

        if (list.Count == 0)
        {
            return null;
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

    void GetOutOfSpawn()
    {
        Vector3 target = new Vector3(5f, -6.5f);
        
        if (!tweener.TweenExists(ghost.transform))
        {
            var nextMove = GetDirectionToTarget(target);

            if (nextMove != null)
            {
                MoveGhost(nextMove);
            }
        }
    }

    bool CheckGhostIsInSpawn()
    {
        var position = ghost.transform.position;
        var isInSpawnXRange = position.x > 1.5 && position.x < 8.5;
        var isInSpawnYRange = position.y < -7.5 && position.y > -11.5;

        return isInSpawnXRange && isInSpawnYRange;
    }

    Directions? GetNextMove()
    {
        if (lastDirection != null && !IsBlockedByWall((Directions)lastDirection))
        // Ants should be going in one direction until they can't
        {
            return lastDirection;
        }

        Directions? nextMove = null;

        if (ghostType == 1)
        {
            nextMove = Ghost1Behaviour();
        }
        else if (ghostType == 2)
        {
            nextMove = Ghost2Behaviour();
        }
        else if (ghostType == 3)
        {
            nextMove = (Directions)Random.Range(0, 4);

            while (IsBlockedByWall((Directions)nextMove)) // Must be move-able direction
            {
                nextMove = (Directions)Random.Range(0, 4);
            }
        }
        else if (ghostType == 4)
        {
            // Move clockwise around map
        }

        return nextMove;
    }

    Directions? GetDirectionToTarget(Vector3 target)
    {
        var list = new List<Directions>();
        var currentPosition = ghost.transform.position;
        var distanceToTarget = (target - currentPosition).magnitude;

        if (!IsBlockedByWall(Directions.Up) && 
            (target - (currentPosition + new Vector3(0.0f, 1.0f, 1.0f)))
            .magnitude <= distanceToTarget)
        {
            if (lastDirection != Directions.Up)
            {
                list.Add(Directions.Up);
            }
        }
        else if (!IsBlockedByWall(Directions.Left) &&
            (target - (currentPosition + new Vector3(-1.0f, 0.0f, 0.0f)))
            .magnitude <= distanceToTarget)
        {
            if (lastDirection != Directions.Left)
            {
                list.Add(Directions.Left);
            }
        }
        else if (!IsBlockedByWall(Directions.Right) &&
            (target - (currentPosition + new Vector3(1.0f, 0.0f, 0.0f)))
            .magnitude <= distanceToTarget)
        {
            if (lastDirection != Directions.Right)
            {
                list.Add(Directions.Right);
            }
        }
        else if (!IsBlockedByWall(Directions.Down) &&
            (target - (currentPosition + new Vector3(0.0f, -1.0f, 0.0f)))
            .magnitude <= distanceToTarget)
        {
            if (lastDirection != Directions.Down)
            {
                list.Add(Directions.Down);
            }
        }

        if (list.Count == 0)
        {
            return null;
        }

        var randomValidDirectionIndex = Random.Range(0, list.Count);
        return list[randomValidDirectionIndex];
    }

    void MoveGhost(Directions? nextMove)
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

    //Directions? GetDirectionToTarget(Vector2 newPosition)
    //{
    //    if (!tweener.TweenExists(ghost.transform))
    //    {
    //        if (lastDirection != null && !IsBlockedByWall((Directions)lastDirection))
    //        // Ants should be going in one direction until they can't
    //        {
    //            return (Directions)lastDirection;
    //        }
    //        else
    //        {
    //            return GetDirectionToTarget(newPosition);
    //        }
    //    }

    //    return null;
    //}

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
