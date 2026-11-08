using System.Collections.Generic;
using TMPro;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using Debug = UnityEngine.Debug;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    //player settings
    public float moveSpeed = 1f;
    public Transform movePoint;
    [SerializeField] Rigidbody2D rb;
    private Vector3 move;
    private Vector2 input;
    private bool hasMoved = false;
    public LayerMask obstacleLayer;
    public LayerMask ladderLayer;

    // Ghost settings
    public int maxMoves = 10;
    public GameObject ghost;
    private List<Vector3> recordedMoves = new List<Vector3>();
    private GameObject activeGhost;
    private GhostManager ghostManager;
    private bool ghostSpawned = false;
    private Vector3 spawnPos;
    private Collider2D collider2D;
    
    //UI Stuff
    public GameObject deathScreen;
    public float deathDelay = 2f;
    private float deathTimer;
    private bool isDead = false;
    private CinemachineBrain brain;
    
    //Audio Stuff
    public AudioClip walkAudio;
    public AudioClip ladderAudio;
    public AudioClip plateAudio;
    private AudioSource audioSource;

    //temp
    public TMP_Text movesText;
    private Collider2D debugHit;
    private bool lastOnLadder;

    void Start()
    {
        brain = Camera.main.GetComponent<CinemachineBrain>();
        audioSource = GetComponent<AudioSource>();
        collider2D = GetComponent<BoxCollider2D>();
        if (movePoint != null)
        {
            movePoint.parent = null;
        }

        if (CameraManager.hasCheckPoint)
        {
            transform.position = CameraManager.checkPoint;
            movePoint.position = CameraManager.checkPoint;
        }
        spawnPos = transform.position;
        UpdateUIMoves();
    }

    void Update()
    {
        //load the scene again after deathTimer in seconds
        if (isDead)
        {
            deathTimer -= Time.deltaTime;
            if (deathTimer <= 0f)
            {
                SceneManager.LoadScene(0);
            }
            return;
        }
        
        //ensure the player cannot move when the camera is transitioning
        if (brain != null && brain.IsBlending)
        {
            return;
        }
        
        var restartAction = InputSystem.actions?["Restart"];
        if (restartAction != null && restartAction.IsPressed())
        {
            Restart();
        }
        
        //move the player towards the movePoint
        transform.position = Vector3.MoveTowards(transform.position, movePoint.position, moveSpeed * Time.deltaTime);
        if (activeGhost != null && Vector2.Distance(activeGhost.transform.position, transform.position) <= 0.1f)
        {
            Die();
            return;
        }
       
        bool onLadderNow = OnLadder();
        if (onLadderNow != lastOnLadder)
        {
            lastOnLadder = onLadderNow;
        }
        if (!OnLadder() && transform.position == movePoint.position)
        {
            if (ApplyGravity())
            {
                return;
            }
        }

        var moveAction = InputSystem.actions?["Move"];
        if (moveAction == null) return;

        //if the player is at the move point, allow movement
        if (moveAction.ReadValue<Vector2>() == Vector2.zero)
        {
            if (transform.position == movePoint.transform.position)
            {
                hasMoved = false;
            }
        }
        //otherwise, check input for vertical availability
        else if (!hasMoved)
        {
            input = moveAction.ReadValue<Vector2>();

            if ((input.y > 0 || input.y < 0) && !OnLadder())
            {
                input.y = 0;
            }

            move = new Vector3(input.x * 2f, input.y * 2f, 0);

            //Create an overlap box outwards from the player, if the box hits something on the obstacles layer, don't
            //allow movement
            Vector3 targetPos = movePoint.transform.position + move;
            Collider2D hit = Physics2D.OverlapBox(targetPos, new Vector2(2f, .5f), 0f, obstacleLayer);
            debugHit = hit;
            

            //if the collider finds something, check if it is a door
            if (hit != null)
            {
                Door door = hit.GetComponent<Door>();
                bool isOpenDoor = door != null && door.isOpen;
                if (!isOpenDoor)
                {
                    return; // blocked by a wall, closed door, or anything on obstacleLayer
                }
            }
            
            //check for ladders
            Collider2D ladderHit = Physics2D.OverlapBox(targetPos, new Vector2(0.5f, 0.5f), 0f, ladderLayer);
            if (input.y != 0 && ladderHit == null)
            {
                return;
            }

            //if allowed to move, move
            hasMoved = true;
            PlayMoveSound(targetPos, input.y != 0);
            movePoint.transform.position += move;
            HandleMovement(move);
        }
    }

    //Stores player moves up to max, spawns ghost when max moves reached
    void HandleMovement(Vector3 playerMove)
    {
        if (!ghostSpawned)
        {
            // Record player moves until we reach maxMoves
            recordedMoves.Add(playerMove);

            if (recordedMoves.Count >= maxMoves)
            {
                SpawnGhost();
            }
        }
        else
        {
            //Once spawned, every input triggers the ghost to take its next step
            if (ghostManager != null)
            {
                ghostManager.TriggerStep();
            }
        }
        UpdateUIMoves();
    }

    void SpawnGhost()
    {
        if (ghost != null)
        {
            activeGhost = Instantiate(ghost, spawnPos, Quaternion.identity);
            ghostManager = activeGhost.GetComponent<GhostManager>();

            if (ghostManager != null)
            {
                ghostManager.moveSpeed = moveSpeed;
                ghostManager.ladderLayer = ladderLayer;
                ghostManager.obstacleLayer = obstacleLayer;
                // Give the ghost its fixed sequence of moves
                ghostManager.InitializeMoves(new Queue<Vector3>(recordedMoves));
            }

            ghostSpawned = true;
            Debug.Log("Ghost spawned with " + maxMoves + " moves!");
        }
    }

    public void ResetGhost(Vector3 newSpawnPos)
    {
        if (activeGhost != null)
        {
            Destroy(activeGhost);
        }
        
        activeGhost = null;
        ghostSpawned = false;
        ghostManager = null;
        recordedMoves.Clear();
        spawnPos = newSpawnPos;
        UpdateUIMoves();
    }

    //checks beneath the player for terrain and falls until it finds some
    bool ApplyGravity()
    {
        RaycastHit2D hit = Physics2D.Raycast(movePoint.position, Vector2.down, Mathf.Infinity, obstacleLayer);
        if (hit.collider == null)
        {
            return false;
        }

        // Distance from the player's pivot down to the bottom of its collider
        float feetOffset = transform.position.y - collider2D.bounds.min.y;
        float fall = hit.distance - feetOffset;

        if (fall > 0.01f)
        {
            audioSource.Stop();
            movePoint.position += Vector3.down * fall;
            return true;
        }

        if (hit.collider.CompareTag("KillBox"))
        {
            Die();
        }

        return false;
    }

    private void Die()
    {
        if (isDead)
        {
            return;
        }
        
        isDead = true;
        deathTimer = deathDelay;
        
        if (deathScreen != null)
        {
            deathScreen.SetActive(true);
        }
    }
    
    private void Restart()
    {
        SceneManager.LoadScene(0);
    }

    bool OnLadder()
    {
        return Physics2D.OverlapBox(movePoint.position, new Vector2(.5f, .5f), 0f, ladderLayer);
    }

    void UpdateUIMoves()
    {
        if (movesText == null)
        {
            return;
        }
        int remaining = ghostSpawned ? 0 : maxMoves - recordedMoves.Count;
        movesText.text = "Moves: "  + remaining + "/" + maxMoves;
    }

    void PlayMoveSound(Vector3 targetPos, bool isVertical)
    {
        if (audioSource == null)
        {
            return;
        }

        AudioClip active = walkAudio;
        
        //choose a sound to play
        if (isVertical)
        {
            active = ladderAudio;
        }
        else if (OnPlate(targetPos))
        {
            active = plateAudio;
        }

        if (active == null)
        {
            return;
        }

        audioSource.volume = 0.5f;
        audioSource.pitch = Random.Range(0.9f, 1.1f);
        audioSource.Stop();
        audioSource.clip = active;
        audioSource.Play();
    }

    //Using OverlapBoxAll returns every collider the object is touching, including itself so while not an optimal call
    //this is an alternate way to determine whether I'm somewhere I want to be without using layerMasks.
    //I wanted to showcase that, a layer mask is a better solution.
    bool OnPlate(Vector3 pos)
    {
        Collider2D[] hits = Physics2D.OverlapBoxAll(pos, new Vector2(0.5f, 0.5f), 0f);

        foreach (Collider2D hit in hits)
        {
            if (hit.GetComponent<PressurePlate>() != null)
            {
                return true;
            }
        }
        return false;
    }

    public void StopMoveSound()
    {
        if (audioSource != null)
        {
            audioSource.Stop();
        }
    }

    //gizmos for debug
    void OnDrawGizmos()
    {
        if (movePoint == null) return;
        Gizmos.color = debugHit != null ? Color.red : Color.green;
        Gizmos.DrawWireCube(movePoint.position + move, new Vector3(2f, 0.5f, 0f));
        Debug.DrawRay(movePoint.position, Vector2.down * 20f, Color.red);
    }
}