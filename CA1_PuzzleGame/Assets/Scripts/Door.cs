using UnityEngine;

public class Door : MonoBehaviour
{
    private int pressCount;
    private Collider2D doorCollider;
    private SpriteRenderer doorSpriteRenderer;

    void Awake()
    {
        doorCollider = GetComponent<Collider2D>();
        doorSpriteRenderer = GetComponent<SpriteRenderer>();
    }

    public bool IsOpen()
    {
        return pressCount > 0;
    }
    public void AddPress()
    {
        pressCount++;
        OpenCloseDoor();
    }

    public void RemovePress()
    {
        pressCount--;
        OpenCloseDoor();
    }

    void OpenCloseDoor()
    {
        doorCollider.enabled = !IsOpen();
        doorSpriteRenderer.enabled = !IsOpen();
    }
}
