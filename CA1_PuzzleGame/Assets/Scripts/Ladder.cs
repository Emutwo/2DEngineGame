using UnityEngine;

public class Ladder : MonoBehaviour
{
    //simple collision trigger to allow vertical movement
   private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            Player player = collision.GetComponent<Player>();
            if(player != null)
            {
                player.canMoveVertical = true;
            }
        }
    }
    
    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            Player player = collision.GetComponent<Player>();
            if(player != null)
            {
                player.canMoveVertical = false;
            }
        }
        

    }
}
