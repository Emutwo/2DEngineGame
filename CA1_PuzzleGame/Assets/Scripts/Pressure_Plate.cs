using System;
using UnityEngine;

public class PressurePlate : MonoBehaviour
{ 
    public Door door;
    
    //if the plate detects a player, it opens its connected door
    private void OnTriggerEnter2D(Collider2D collision)
       {
           if(collision.CompareTag("Player") || collision.CompareTag("Ghost"))
           {
               if(door != null)
               {
                   door.isOpen = true;
               }
           }
       }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") || collision.CompareTag("Ghost"))
        {
            if (door != null)
            {
                door.isOpen = false;
            }
        }
    }
}
