using System;
using UnityEngine;

public class PressurePlate : MonoBehaviour
{ 
    public Door[] doors;
    
    //if the plate detects a player or ghost, it opens its connected doors
    private void OnTriggerEnter2D(Collider2D collision)
       {
           if(collision.CompareTag("Player") || collision.CompareTag("Ghost"))
           {
               foreach (Door door in doors)
               { 
                   if(door != null) 
                   {
                           door.AddPress(); 
                   }
               }
               transform.position += Vector3.down * 0.2f;
           }
       }
    

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.CompareTag("Player") || collision.CompareTag("Ghost"))
        {
            foreach (Door door in doors)
            { 
                if(door != null) 
                {
                    door.RemovePress(); 
                }
            }
            transform.position += Vector3.up * 0.2f;
        }
    }
}


