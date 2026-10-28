using System.Collections.Generic;
using UnityEngine;

public class GhostManager : MonoBehaviour
{
    public float moveSpeed = 5f;
    public Transform movePoint;
    private Queue<Vector3> moves = new Queue<Vector3>();
    public LayerMask obstacleLayer;

    void Awake()
    {
        GameObject mp = new GameObject(gameObject.name + "_MovePoint");
        mp.transform.position = transform.position;
        movePoint = mp.transform;
        movePoint.parent = null; 
    }

    void Update()
    {
        if (movePoint == null) return;

        //move towards its movePoint every frame
        transform.position = Vector3.MoveTowards(transform.position, movePoint.position, moveSpeed * Time.deltaTime);
    }

    // Receives the initial batch of recorded moves from the player
    public void InitializeMoves(Queue<Vector3> recordedMoves)
    {
        moves = new Queue<Vector3>(recordedMoves);
    }

    // Called ONLY when the player makes a new move
    public void TriggerStep()
    {
        // Only move if there are still steps left in the queue
        if (moves.Count > 0)
        {
            Vector3 nextMove = moves.Peek();
            Vector2 targetPosition = movePoint.position + nextMove;

            Collider2D hit = Physics2D.OverlapBox(targetPosition, new Vector2(0.5f, 0.5f), 0f, obstacleLayer);

            if (hit != null)
            {
                Door door = hit.gameObject.GetComponent<Door>();
                if (!door.isOpen)
                {
                    Debug.Log("clone collision");
                    return;
                }
            }
            nextMove = moves.Dequeue();
            movePoint.position += nextMove;
            Debug.Log("Ghost took a step. Remaining moves: " + moves.Count);
        }
    }

    void OnDestroy()
    {
        if (movePoint != null)
        {
            Destroy(movePoint.gameObject);
        }
    }
}