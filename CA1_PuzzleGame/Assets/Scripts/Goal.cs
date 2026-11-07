using UnityEngine;
public class Goal : MonoBehaviour
{
    public Transform nextSpawn;
    public GameObject winScreen;

    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"Goal entered by {other.name} tag={other.tag}");
        if (!other.CompareTag("Player"))
        {
            return;
        }
        Player player = other.GetComponent<Player>();
        if (player == null)
        {
            return;
        }

        //update player position to room 2 spawn - snap to avoid other triggers
        //if no "nextSpawn" is assigned, assume it is the final room
        if (nextSpawn != null)
        {
            player.transform.position = nextSpawn.position;
            player.movePoint.position = nextSpawn.position;
        }
        else
        {
            if (winScreen != null)
            {
                winScreen.SetActive(true);
            }
            
            player.enabled = false;
        }
    }
}
