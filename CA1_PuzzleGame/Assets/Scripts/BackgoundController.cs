using UnityEngine;

public class BackgoundController : MonoBehaviour
{

    private Transform mainCamera;
    private Vector3 lastCamPos;
    public float parallaxSpeed = 1f;
   
    void Start()
    {
        if (Camera.main != null)
        {
            mainCamera = Camera.main.transform;
            lastCamPos = mainCamera.position;
        }
    }
    
    void LateUpdate()
    {
        if (mainCamera != null)
        {
            //calculate how much the camera moved and move the background the same amount
            Vector3 movement = mainCamera.position - lastCamPos;
            transform.position += new Vector3(movement.x * parallaxSpeed, 0, 0);
            lastCamPos = mainCamera.position;
        }
    }
}
