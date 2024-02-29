using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    public static RoomManager instance;    
    public GameObject virtualCam;

    void Start(){
        instance = this;
    }

    // Start is called before the first frame update
    void OnTriggerEnter2D(Collider2D other){
        if(other.CompareTag("Player") && !other.isTrigger && !PlayerController.instance.IsDead){
            virtualCam.SetActive(true);                            
        }
    }

    void OnTriggerExit2D(Collider2D other){
        if(other.CompareTag("Player") && !other.isTrigger && !PlayerController.instance.IsDead){
          //  if(PlayerController.instance.PlayerIsInCamera)
                virtualCam.SetActive(false);
        }
    }
}
 