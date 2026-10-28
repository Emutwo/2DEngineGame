using System;
using UnityEngine;

public class PressurePlate : MonoBehaviour
{ 
    public Door door;
    
    //if the plate detects a player, it opens its connected door
    private void OnTriggerEnter2D(Collider2D collision)
       {
           Debug.Log("Plate trigger by: " + collision.gameObject.name);
           if(collision.CompareTag("Player"))
           {
               if(door != null)
               {
                   door.isOpen = true;
               }
           }
       }
}
