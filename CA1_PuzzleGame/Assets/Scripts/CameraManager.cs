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
        
        //Reset ghost, move player and change camera on room change
        bool roomChanged = currentCamera != roomCamera;
        if (roomChanged)
        {
            if (currentCamera != null)
            {
                hasCheckPoint = true;
                checkPoint = roomSpawnPoint.position;
                currentCamera.Priority = 10;
                player.movePoint.position = roomSpawnPoint.position;
                player.maxMoves = roomMaxMoves;
                player.ResetGhost(roomSpawnPoint.position);
            }
        }
        
        roomCamera.Priority = 20;
        currentCamera = roomCamera;
        
    }
}
