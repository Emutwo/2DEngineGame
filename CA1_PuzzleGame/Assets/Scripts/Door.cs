using UnityEngine;

public class Door : MonoBehaviour
{
    public bool isOpen = false;
    private Collider2D doorCollider;
    private SpriteRenderer doorSpriteRenderer;

    void Start()
    {
        doorCollider = GetComponent<Collider2D>();
        doorSpriteRenderer = GetComponent<SpriteRenderer>();
    }
    
    void Update()
    {
        if (isOpen)
        {
            Open();
        }
        else if (!isOpen)
        {
            Close();
        }
    }
    void Open()
    {
        doorCollider.enabled = false;
        doorSpriteRenderer.enabled = false;
    }

    void Close()
    {
        doorCollider.enabled = true;
        doorSpriteRenderer.enabled = true;
    }
}
