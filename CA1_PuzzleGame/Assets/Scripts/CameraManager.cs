using Unity.Cinemachine;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    
    public CinemachineCamera roomCamera;
    private static CinemachineCamera currentCamera;
    public Transform roomSpawnPoint;
   
    void Start()
    {
        Debug.Log($"{name}: RoomTrigger alive, camera={(roomCamera != null ? roomCamera.name : "NULL")}");
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        Player player = other.gameObject.GetComponent<Player>();
        
        if (!other.CompareTag("Player"))
        {
            return;
        }
        
        if (roomCamera == null)
        {
            return;
        }
        
        //if the camera is not the same as the previous one, reduce it's priority
        if (currentCamera != null && currentCamera != roomCamera)
        {
            currentCamera.Priority = 10;
        }
        roomCamera.Priority = 20;
        currentCamera = roomCamera;
        
        player.movePoint.position = roomSpawnPoint.position;
        
    }
}
