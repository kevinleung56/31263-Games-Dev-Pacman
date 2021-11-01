using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

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
    private GameObject ghostTimerLabelObject;
    private GameObject ghostTimerObject;
    private GameObject gameTimerObject;
    private GameObject gameStartObject;
    private GameObject gameStartLabelObject;
    private GameObject gameOverObject;
    private List<GameObject> healthObjects;
    private Text gameTimer;
    private Text score;
    private Text ghostTimer;
    private Text gameStart;
    private Stopwatch gameRunningTimer;
    private Stopwatch gameStartRunningTimer;
    private Stopwatch ghostRunningTimer;
    private KeyCode? lastInput;
    private KeyCode? currentInput;
    private int playerHealth = 3;
    public bool gameStarted = false;
    public bool gameOver = false;
    private int pelletsLeft;

    private enum Directions { Up, Down, Left, Right };
    private enum GhostState { Walking, Scared, Recovering, Dead };

    void Start()
    {
        tweener = GetComponent<Tweener>();
        scoreObject = GameObject.FindGameObjectWithTag("Score");
        ghostTimerLabelObject = GameObject.FindGameObjectWithTag("GhostTimerLabel");
        ghostTimerObject = GameObject.FindGameObjectWithTag("GhostTimer");
        gameStartLabelObject = GameObject.FindGameObjectWithTag("GameStartLabel");
        gameStartObject = GameObject.FindGameObjectWithTag("GameStartTimer");
        gameTimerObject = GameObject.FindGameObjectWithTag("GameTimer");
        healthObjects = GameObject.FindGameObjectsWithTag("Health").ToList();
        gameOverObject = GameObject.FindGameObjectWithTag("GameOver");

        score = scoreObject.GetComponent<Text>();
        ghostTimer = ghostTimerObject.GetComponent<Text>();
        gameTimer = gameTimerObject.GetComponent<Text>();
        gameStart = gameStartObject.GetComponent<Text>();

        gameRunningTimer = new Stopwatch();
        ghostRunningTimer = new Stopwatch();
        gameStartRunningTimer = new Stopwatch();
        gameStartRunningTimer.Start();

        gameOverObject.SetActive(false);
        ghostTimerLabelObject.SetActive(false);
        gameTimer.text = "00:00:00";
        StartCoroutine(StartCountdownCoroutine());
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
            animatorController.SetBool("MoveLeftRightParam", false);
            StopDustTrail();
            return false;
        }

        var vectorToMove = vector == null ? new Vector3(0.0f, 1.0f, 1.0f) : (Vector3)vector;
        OnPlayerMove();
        spriteRenderer.flipX = true;
        pacStudent.transform.Rotate(new Vector3(0, 0, 90));
        animatorController.SetBool("MoveLeftRightParam", true);
        AddTweenToPosition(pacStudent.transform.position + vectorToMove, duration);
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
        OnPlayerMove();
        spriteRenderer.flipX = false;
        pacStudent.transform.Rotate(new Vector3(0, 0, 90));
        animatorController.SetBool("MoveLeftRightParam", true);
        AddTweenToPosition(pacStudent.transform.position + vectorToMove, duration);
        return true;
    }

    void TeleportTunnelIfNeeded()
    {
        if ((Vector2)pacStudent.transform.position == new Vector2(19.5f, -9.5f)) // Right tunnel
        {
            pacStudent.transform.position = new Vector2(-8.5f, -9.5f); // Teleportation
        }
        else if ((Vector2)pacStudent.transform.position == new Vector2(-9.5f, -9.5f)) // Left tunnel
        {
            pacStudent.transform.position = new Vector2(18.5f, -9.5f); // Teleportation
        }
    }

    void CheckForGhostTimerIfNeeded()
    {
        if (ghostRunningTimer != null && ghostRunningTimer.IsRunning)
        {
            var timeLeftTimer = 10 - ghostRunningTimer.Elapsed.Seconds;
            ghostTimer.text = timeLeftTimer.ToString();
            if (timeLeftTimer <= 0)
            {
                ghostTimerLabelObject.SetActive(false);
                ghostRunningTimer.Stop();
                ghostRunningTimer.Reset();
            }
        }
    }

    void UpdateGameRunningTimer()
    {
        if (gameRunningTimer.IsRunning)
        {
            var currentTime = gameRunningTimer.Elapsed;
            var currentTimeFormatted = string.Format("{0:00}:{1:00}:{2:00}", currentTime.Minutes, currentTime.Seconds, currentTime.Milliseconds / 10);
            gameTimer.text = currentTimeFormatted;
        }
    }

    void CheckGameOver(bool gameOverOverride = false)
    {
        if (pelletsLeft == 0 || playerHealth == 0 || gameOverOverride)
        {
            ghostTimerObject.SetActive(false);
            gameOverObject.SetActive(true);
            gameStarted = false;
            gameOver = true;
            gameRunningTimer.Stop();

            StartCoroutine(EndGameCoroutine());
        }
    }

    IEnumerator EndGameCoroutine()
    {
        while (gameOver)
        {
            yield return new WaitForSeconds(3f);

            // Save player prefs
            var existingHighscore = int.Parse(PlayerPrefs.GetString("highscore", "0"));
            var existingTime = TimeSpan.ParseExact(PlayerPrefs.GetString("time", "00:00:00"), @"mm\:ss\:ff", CultureInfo.InvariantCulture);
            var currentHighscore = int.Parse(score.text);
            var currentTime = (int)gameRunningTimer.Elapsed.TotalSeconds;

            var firstTime = existingHighscore == 0 || existingTime.TotalSeconds == 0;
            var newPersonalRecord = currentHighscore > existingHighscore ||
                (currentHighscore == existingHighscore && currentTime < existingTime.TotalSeconds);

            if (firstTime || newPersonalRecord)
            {
                var currentTimeSpan = gameRunningTimer.Elapsed;
                var currentTimeFormatted = string.Format(
                    "{0:00}:{1:00}:{2:00}",
                    currentTimeSpan.Minutes,
                    currentTimeSpan.Seconds,
                    currentTimeSpan.Milliseconds / 10);

                PlayerPrefs.SetString("highscore", score.text);
                PlayerPrefs.SetString("time", currentTimeFormatted);
            }

            gameOver = false;
            SceneManager.LoadSceneAsync(1);
        }
    }

    IEnumerator StartCountdownCoroutine()
    {
        while (gameStartRunningTimer.IsRunning)
        {
            gameStart.text = "3";

            yield return new WaitForSeconds(1f);

            gameStart.text = "2";

            yield return new WaitForSeconds(1f);

            gameStart.text = "1";

            yield return new WaitForSeconds(1f);

            gameStart.text = "GO!";

            yield return new WaitForSeconds(1f);

            gameStartRunningTimer.Stop();
        }

        gameStartLabelObject.SetActive(false);
        gameRunningTimer.Start();
        var pellets = GameObject.FindGameObjectsWithTag("Pellet");
        pelletsLeft = pellets.Length;
        gameStarted = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (gameStarted)
        {
            CheckGameOver();
            UpdateGameRunningTimer();
            CheckForGhostTimerIfNeeded();
            TeleportTunnelIfNeeded();
            GetInput();
        }
    }

    void GetInput()
    {
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
        else if (!tweener.TweenExists(pacStudent.transform) && lastInput != null)
        {
            switch (lastInput)
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
                    MoveLeft();
                    break;
                case KeyCode.D:
                    MoveRight();
                    break;
                case KeyCode.W:
                    MoveUp();
                    break;
                case KeyCode.S:
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

    void SetAllAntStates(GhostState state)
    {
        var ants = GameObject.FindGameObjectsWithTag("Ant");
        
        switch (state)
        {
            case GhostState.Walking:
                foreach (var ant in ants)
                {
                    var animator = ant.GetComponent<Animator>();
                    animator.SetBool("AntIsDeadParam", false);
                    animator.SetBool("AntIsRecoveringParam", false);
                    animator.SetBool("AntIsScaredParam", false);
                }

                break;
            case GhostState.Scared:
                foreach (var ant in ants)
                {
                    var animator = ant.GetComponent<Animator>();
                    animator.SetBool("AntIsDeadParam", false);
                    animator.SetBool("AntIsRecoveringParam", false);
                    animator.SetBool("AntIsScaredParam", true);
                }

                break;

            case GhostState.Recovering:
                foreach (var ant in ants)
                {
                    var animator = ant.GetComponent<Animator>();
                    animator.SetBool("AntIsDeadParam", false);
                    animator.SetBool("AntIsRecoveringParam", true);
                    animator.SetBool("AntIsScaredParam", false);
                }

                break;

            case GhostState.Dead:
                foreach (var ant in ants)
                {
                    var animator = ant.GetComponent<Animator>();
                    animator.SetBool("AntIsDeadParam", true);
                    animator.SetBool("AntIsRecoveringParam", false);
                    animator.SetBool("AntIsScaredParam", false);
                }

                break;

            default: break;
        }
    }

    void SetAntState(Animator antAnimator, GhostState state)
    {
        switch (state)
        {
            case GhostState.Walking:
                antAnimator.SetBool("AntIsDeadParam", false);
                antAnimator.SetBool("AntIsRecoveringParam", false);
                antAnimator.SetBool("AntIsScaredParam", false);

                break;
            case GhostState.Scared:
                antAnimator.SetBool("AntIsDeadParam", false);
                antAnimator.SetBool("AntIsRecoveringParam", false);
                antAnimator.SetBool("AntIsScaredParam", true);

                break;

            case GhostState.Recovering:
                antAnimator.SetBool("AntIsDeadParam", false);
                antAnimator.SetBool("AntIsRecoveringParam", true);
                antAnimator.SetBool("AntIsScaredParam", false);

                break;

            case GhostState.Dead:
                antAnimator.SetBool("AntIsDeadParam", true);
                antAnimator.SetBool("AntIsRecoveringParam", false);
                antAnimator.SetBool("AntIsScaredParam", false);

                break;

            default: break;
        }
    }

    private IEnumerator SetAllAntsScaredCoroutine()
    {
        SetAllAntStates(GhostState.Scared);

        yield return new WaitForSeconds(7);

        SetAllAntStates(GhostState.Recovering);

        yield return new WaitForSeconds(3);

        SetAllAntStates(GhostState.Walking);
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        UnityEngine.Debug.Log("Collision Enter: " + collision.gameObject + " : " + collision.transform.position);
        if (collision.gameObject.CompareTag("Pellet"))
        {
            // Add 10 to score
            score.text = (int.Parse(score.text) + 10).ToString();
            pelletsLeft--;
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
            StartCoroutine(SetAllAntsScaredCoroutine());

            ghostRunningTimer.Start();
            ghostTimerLabelObject.SetActive(true);
            Destroy(collision.gameObject);
        }
        else if (collision.gameObject.CompareTag("Ant"))
        {
            var animator = collision.gameObject.GetComponent<Animator>();
            // Ant is recovering or scared
            if (animator.GetBool("AntIsScaredParam") || animator.GetBool("AntIsRecoveringParam"))
            {
                score.text = (int.Parse(score.text) + 300).ToString();
                SetAntState(animator, GhostState.Dead);
            }
            else // Normal ant
            {
                var healthToLose = healthObjects.Find(x => x.name.Contains(playerHealth.ToString()));
                playerHealth--;
                healthToLose.SetActive(false);
                /*
                 * PacStudent dies and loses a life. Update the UI element for this.
                 *
                 * Play a particle effect around PacStudent the spot of PacStudents death.
                 *
                 * Respawn PacStudent by moving them to the top‐left hand corner,
                 * where they started the game, and wait for player input.*/

                Debug.Log(playerHealth);

                if (playerHealth == 0) // Player is dead
                {
                    // Play particle effect
                    Debug.Log("Supposed to be dead");
                    pacStudent.transform.position = new Vector2(-7.5f, 3.5f);
                }
            }
        }
    }
}
