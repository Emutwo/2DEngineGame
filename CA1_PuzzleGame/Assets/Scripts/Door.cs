using UnityEngine;

public class Door : MonoBehaviour
{
    private int pressCount;
    private Collider2D doorCollider;
    private SpriteRenderer doorSpriteRenderer;
    public bool startsOpen = false;
    public bool isOpen => startsOpen ? pressCount == 0 : pressCount > 0;

    void Awake()
    {
        doorCollider = GetComponent<Collider2D>();
        doorSpriteRenderer = GetComponent<SpriteRenderer>();
        OpenCloseDoor();
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
        doorCollider.enabled = !isOpen;
        doorSpriteRenderer.enabled = !isOpen;
    }
}
