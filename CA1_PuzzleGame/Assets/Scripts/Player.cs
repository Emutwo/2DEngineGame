using System.Collections.Generic;
using System.Diagnostics;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using Debug = UnityEngine.Debug;

public class Player : MonoBehaviour
{
    public float moveSpeed = 1f;
    public Transform movePoint;
    [SerializeField] Rigidbody2D rb;
    private Vector3 move;
    private Vector2 input;
    private bool hasMoved = false;
    public bool canMoveVertical = false;
    public LayerMask obstacleLayer;

    // Ghost settings
    public int maxMoves = 10;
    public GameObject ghost;

    private List<Vector3> recordedMoves = new List<Vector3>();
    private GameObject activeGhost;
    private GhostManager ghostManager;
    private bool ghostSpawned = false;
    private Vector3 spawnPos;
    
    //temp
    public TMP_Text debugText;
    private Collider2D debugHit;
    
    void Start()
    {
        GameObject textObj = GameObject.FindWithTag("DebugText");
        if (textObj != null)
        {
            debugText = textObj.GetComponent<TMP_Text>();
        }

        if (debugText == null)
        {
            Debug.LogWarning("No text component found on " + gameObject.name);
        }
        
        if (movePoint != null) movePoint.parent = null;
        spawnPos = transform.position;
    }

    void Update()
    {
        //move the player towards the movePoint
        transform.position = Vector3.MoveTowards(transform.position, movePoint.position, moveSpeed * Time.deltaTime);

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

            if((input.y > 0  || input.y < 0 )&& !canMoveVertical)
            {
                input.y = 0;
            }
            move = new Vector3(input.x * 2f, input.y * 2f, 0);

            //Create an overlap box outwards from the player, if the box hits something on the obstacles layer, don't
            //allow movement
            Vector3 targetPos = movePoint.transform.position + move;
            Collider2D hit = Physics2D.OverlapBox(targetPos, new Vector2(2f, 0.5f), 0f,obstacleLayer);
            debugHit = hit;
            
            if (debugText != null)
            {
                debugText.text = hit != null ? $"Hit: {hit.name}" : "Hit: none";
            }
            
            //if the collider finds something, check if it is a door
            if (hit != null)
            {
                Door door = hit.GetComponentInParent<Door>();
                bool isOpenDoor = door != null && door.isOpen;
                if (!isOpenDoor)
                {
                    return; // blocked by a wall, closed door, or anything on obstacleLayer
                }
            }
            //if allowed to move, move
            hasMoved = true;
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
                // Give the ghost its fixed sequence of moves
                ghostManager.InitializeMoves(new Queue<Vector3>(recordedMoves));
            }
            ghostSpawned = true;
            Debug.Log("Ghost spawned with " + maxMoves + " moves!");
        }
    }

    //gizmos for debug
    void OnDrawGizmos()
    {
        if (movePoint == null) return;
        Gizmos.color = debugHit != null ? Color.red : Color.green;
        Gizmos.DrawWireCube(movePoint.position + move, new Vector3(2f, 0.5f, 0f));
    }
}