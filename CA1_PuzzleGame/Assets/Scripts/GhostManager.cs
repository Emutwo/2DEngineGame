using System.Collections.Generic;
using UnityEngine;

public class GhostManager : MonoBehaviour
{
    public float moveSpeed = 5f;
    public Transform movePoint;
    private Queue<Vector3> moves = new Queue<Vector3>();
    public LayerMask obstacleLayer;
    private Collider2D collider2D;

    //instantiate movePoint and set it to the players position
    void Awake()
    {
        collider2D = GetComponent<Collider2D>();
        GameObject mp = new GameObject(gameObject.name + "movePoint");
        mp.transform.position = transform.position;
        movePoint = mp.transform;
        movePoint.parent = null; 
    }

    void Update()
    {
        if (movePoint == null) return;

        //move towards its movePoint
        transform.position = Vector3.MoveTowards(transform.position, movePoint.position, moveSpeed * Time.deltaTime);

        if (transform.position == movePoint.position)
        {
            ApplyGravity();
        }
    }

    // Receives recorded moves from the player
    public void InitializeMoves(Queue<Vector3> recordedMoves)
    {
        moves = new Queue<Vector3>(recordedMoves);
    }

    // Called when the player makes a move
    public void TriggerStep()
    {
        // Only move if there are still steps left in the queue
        if (moves.Count > 0)
        {
            Vector3 nextMove = moves.Peek();
            Vector2 targetPosition = movePoint.position + nextMove;

            //the same collision check the player has
            Collider2D hit = Physics2D.OverlapBox(targetPosition, new Vector2(0.5f, 0.5f), 0f, obstacleLayer);

            if (hit != null)
            {
                Door door = hit.GetComponentInParent<Door>();
                bool isOpenDoor = door != null && door.isOpen;
                if (!isOpenDoor)
                {
                    return;
                }
            }
            
            nextMove = moves.Dequeue();
            movePoint.position += nextMove;
            Debug.Log("Ghost took a step. Remaining moves: " + moves.Count);
        }
    }
    
    bool ApplyGravity()
    {
        RaycastHit2D hit = Physics2D.Raycast(movePoint.position, Vector2.down, Mathf.Infinity, obstacleLayer);
        if (hit.collider == null) return false;

        // Distance from the player's pivot down to the bottom of its collider
        float feetOffset = transform.position.y - collider2D.bounds.min.y;
        float fall = hit.distance - feetOffset;
        
        if (fall > 0.01f)
        {
            movePoint.position += Vector3.down * fall;
            return true;
        }
        return false;
    }

    void OnDestroy()
    {
        if (movePoint != null)
        {
            Destroy(movePoint.gameObject);
        }
    }
}