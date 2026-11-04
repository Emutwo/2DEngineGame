using Unity.Cinemachine;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    
    public CinemachineCamera roomCamera;
    private static CinemachineCamera currentCamera;
    public Transform roomSpawnPoint;
    public int roomMaxMoves = 3;

    public static bool hasCheckPoint;
    public static Vector3 checkPoint;


    void Start()
    {
        if(!hasCheckPoint || roomCamera == null || roomSpawnPoint == null)
        {
            return;
        }
        bool isCheckpoint = roomSpawnPoint.position == checkPoint;
        roomCamera.Priority = isCheckpoint ? 20 : 10;
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }
        
        if (roomCamera == null)
        {
            return;
        }
        
        Player player = other.gameObject.GetComponent<Player>();
        if (player == null)
        {
            return;
        }
        
        //Reset ghost, move player and change camera on room change
        bool roomChanged = currentCamera != roomCamera;
        if (roomChanged)
        {
            if (currentCamera != null)
            {
                currentCamera.Priority = 10;
            }
            player.movePoint.position = roomSpawnPoint.position;
            player.maxMoves = roomMaxMoves;
            player.ResetGhost(roomSpawnPoint.position);
            hasCheckPoint = true;
            checkPoint = roomSpawnPoint.position;
        }
        
        roomCamera.Priority = 20;
        currentCamera = roomCamera;
        
    }
}
