using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TopLadder : MonoBehaviour
{
    public static  bool _triggerUpwardsMovement = false;
    public static  bool _positionHasBeenSaved = false;
    public static float _posYDestination = 0f;
    
    public static TopLadder instance;
    // Start is called before the first frame update

    void Start()
    {        
        instance = this; 
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void OnTriggerEnter2D(Collider2D collision){    
        if(collision.gameObject.tag == "UpTransform" && !LadderSystem.instance.TriggerGoingDownLadder){            
            _triggerUpwardsMovement = true;
            Debug.Log("triggerupwards");
        }
    }
}
