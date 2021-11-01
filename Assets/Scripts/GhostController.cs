﻿using System.Collections;
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
    private Vector3 pacStudentPosition;
    private PacStudentController pacStudentController;
    private bool isDead = false;
    private bool isScared = false;
    private bool isRecovering = false;
    private bool gameStarted = false;
    private Vector3? spawnExit = null;
    private int ghostType = -1;
    private Directions? lastMove;

    private enum Directions { Up, Down, Left, Right };

    IEnumerator StartUpCoroutine()
    {
        yield return new WaitForSeconds(4f); // Wait for 3, 2, 1, GO!

        pacStudent = GameObject.FindGameObjectWithTag("Player");
        pacStudentPosition = pacStudent.transform.position;
        pacStudentController = pacStudent.GetComponent<PacStudentController>();
        ghostType = int.Parse(GetComponentInChildren<Text>().text);
        gameStarted = true;
    }


    void Start()
    {
        tweener = GetComponent<Tweener>();
        StartCoroutine(StartUpCoroutine());
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

        var vectorToMove = vector == null ? new Vector3(0.0f, 1.0f, 0.0f) : (Vector3)vector;
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

    bool IsAlreadyMoving()
    {
        return tweener.TweenExists(ghost.transform);
    }

    // Update is called once per frame
    void Update()
    {
        if (gameStarted && !pacStudentController.gameOver)
        {
            isScared = animatorController.GetBool("AntIsScaredParam");
            isDead = animatorController.GetBool("AntIsDeadParam");
            isRecovering = animatorController.GetBool("AntIsRecoveringParam");

            pacStudentPosition = pacStudent.transform.position;

            if (!IsAlreadyMoving())
            {
                if (!isScared && !isDead)
                {
                    if (CheckGhostIsInSpawn())
                    { // Ghost is just trying to get out of spawn
                        GetOutOfSpawn();
                    }
                    else
                    { // Ghost is outside of spawn and using ghost-specific logic
                        spawnExit = null;
                        var nextMove = GetNextMove();
                        if (nextMove != null)
                        {
                            lastMove = (Directions)nextMove;
                            MoveGhost((Directions)nextMove);
                        }
                    }
                }
                else if (isScared || isRecovering)
                {
                    var nextMove = Ghost1Behaviour();
                    if (nextMove != null)
                    {
                        lastMove = (Directions)nextMove;
                        MoveGhost((Directions)nextMove);
                    }
                }
                else if (isDead)
                {
                    // Lerp towards the centre
                }
            }
        }
    }

    Directions IdentifyDirectionToNotBacktrack(Directions lastMove)
    {
        Directions directionToNotBacktrack = lastMove;
        switch (lastMove)
        {
            case Directions.Up:
                directionToNotBacktrack = Directions.Down;
                break;
            case Directions.Left:
                directionToNotBacktrack = Directions.Right;
                break;
            case Directions.Right:
                directionToNotBacktrack = Directions.Left;
                break;
            case Directions.Down:
                directionToNotBacktrack = Directions.Up;
                break;
        }
        return directionToNotBacktrack;
    }

    bool IsNotGoingIntoSpawnOrTunnel(Vector3 hypotheticalPosition)
    {
        if (!isScared && !isDead) {
            if (!CheckGhostIsInSpawn() &&
                (hypotheticalPosition == new Vector3(4.5f, -11.5f) ||
                hypotheticalPosition == new Vector3(5.5f, -11.5f) ||
                hypotheticalPosition == new Vector3(4.5f, -7.5f) ||
                hypotheticalPosition == new Vector3(5.5f, -7.5f)))
            {
                return false; // We don't want ghosts to go back into spawn if they're alive
            }
            else if (hypotheticalPosition == new Vector3(-3.5f, -9.5f) ||
                hypotheticalPosition == new Vector3(13.5f, -9.5f))
            {
                return false; // We don't want ghosts to use tunnels no matter what
            }
        }

        return true;
    }

    Vector3 GetHypotheticalPositionUsingDirection(
        Vector3 currentPosition,
        Directions direction)
    {
        Vector3 hypotheticalPosition = currentPosition;
        switch (direction)
        {
            case Directions.Up:
                hypotheticalPosition = currentPosition + new Vector3(0.0f, 1.0f, 0.0f);
                break;
            case Directions.Down:
                hypotheticalPosition = currentPosition + new Vector3(0.0f, -1.0f, 0.0f);
                break;
            case Directions.Left:
                hypotheticalPosition = currentPosition + new Vector3(-1.0f, 0.0f, 0.0f);
                break;
            case Directions.Right:
                hypotheticalPosition = currentPosition + new Vector3(1.0f, 0.0f, 0.0f);
                break;
        }

        return hypotheticalPosition;
    }

    bool CheckDistanceToTargetIsHigherOrLowerAndCollision(
        Vector3 target,
        float targetDistance,
        Vector3 currentPosition,
        Directions direction,
        bool lower = true)
    {
        // Don't bother calculating hypothetical distances if we
        // can't travel to the new position to begin with
        if (IsBlockedByWall(direction))
        {
            return false;
        }

        var hypotheticalPosition = GetHypotheticalPositionUsingDirection(
            currentPosition,
            direction);

        if (IsNotGoingIntoSpawnOrTunnel(hypotheticalPosition))
        {
            var hypotheticalDistance = (target - hypotheticalPosition).magnitude;

            if (lower)
            {
                return hypotheticalDistance <= targetDistance;
            }
            else
            {
                return hypotheticalDistance >= targetDistance;
            }
        }
        else
        {
            return false;
        }
    }

    Directions? Ghost1Behaviour()
    {
        return GetDirectionToTarget(pacStudentPosition, false);
    }

    Directions? Ghost2Behaviour()
    {
        return GetDirectionToTarget(pacStudentPosition);
    }

    Directions Ghost3Behaviour()
    {
        var nextMove = (Directions)Random.Range(0, 4);
        var currentPosition = ghost.transform.position;
        Directions? directionToNotBacktrack = null;

        if (lastMove != null)
        {
            directionToNotBacktrack =
                IdentifyDirectionToNotBacktrack((Directions)lastMove);
        }

        while (directionToNotBacktrack == nextMove ||
            IsBlockedByWall(nextMove) ||
            !IsNotGoingIntoSpawnOrTunnel(
                GetHypotheticalPositionUsingDirection(currentPosition, nextMove)))
        // Must be move-able direction and cannot backtrack
        // Should be able to move back if last resort
        {
            nextMove = (Directions)Random.Range(0, 4);
        }

        return nextMove;
    }

    void GetOutOfSpawn()
    {
        var positiveOrNegativeRNG = Random.Range(0f, 1f);
        if (spawnExit == null)
        {
            if ((positiveOrNegativeRNG > 0.5f && ghostType != 2) || ghostType == 1)
            // Ghost 1 wants to be as far as possible and this looks less buggy
            // Ghost 2 wants to be as close as possible and this looks less buggy
            {
                spawnExit = new Vector3(4.5f, -12.5f);
            }
            else
            {
                spawnExit = new Vector3(4.5f, -6.5f);
            }
        }
        
        var nextMove = GetDirectionToTarget((Vector3)spawnExit);

        if (nextMove != null)
        {
            lastMove = nextMove;
            MoveGhost(nextMove);
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
            nextMove = Ghost3Behaviour();
        }
        else if (ghostType == 4)
        {
            // Move clockwise around map
        }

        return nextMove;
    }

    Directions? GetDirectionToTarget(Vector3 target, bool lower = true)
    {
        var list = new List<Directions>();
        var currentPosition = ghost.transform.position;
        var headingVector = target - currentPosition;
        var distanceToTarget = headingVector.magnitude;
        Directions? directionToNotBacktrack = null;

        if (lastMove != null)
        {
            directionToNotBacktrack =
                IdentifyDirectionToNotBacktrack((Directions)lastMove);
        }

        if (directionToNotBacktrack != Directions.Up &&
            CheckDistanceToTargetIsHigherOrLowerAndCollision(
                target,
                distanceToTarget,
                currentPosition,
                Directions.Up,
                lower))
        {
            list.Add(Directions.Up);
        }
        else if (directionToNotBacktrack != Directions.Left &&
            CheckDistanceToTargetIsHigherOrLowerAndCollision(
                target,
                distanceToTarget,
                currentPosition,
                Directions.Left,
                lower))
        {
            list.Add(Directions.Left);
        }
        else if (directionToNotBacktrack != Directions.Right &&
            CheckDistanceToTargetIsHigherOrLowerAndCollision(
                target,
                distanceToTarget,
                currentPosition,
                Directions.Right,
                lower))
        {
            list.Add(Directions.Right);
        }
        else if (directionToNotBacktrack != Directions.Down &&
            CheckDistanceToTargetIsHigherOrLowerAndCollision(
                target,
                distanceToTarget,
                currentPosition,
                Directions.Down,
                lower))
        {
            list.Add(Directions.Down);
        }

        if (list.Count == 0)
        // There are no optimal directions
        // However we have to move regardless
        // Move to a random direction - which is ghost 3 behaviour
        {
            return Ghost3Behaviour();
        }

        // RNG to use a random valid direction
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
        Debug.Log("Collision Enter: " + collision.gameObject + " : " + collision.transform.position);
        if (collision.gameObject.CompareTag("Ant"))
        {
        }
    }
}
