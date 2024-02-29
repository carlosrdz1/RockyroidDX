using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigTrunk : MonoBehaviour
{
    int _phase = 1;
    Animator _animations;
    // Start is called before the first frame update
    void Start()
    {        
        _animations = GetComponent<Animator>();        
    }

    // Update is called once per frame
    void Update()
    {
        switch(_phase){
            case 1:
                _animations.Play("Idle");
            break;

            case 2:
                _animations.Play("Phase2");
            break;

            case 3:
                _animations.Play("Phase3");
            break;

            case 4:
                _animations.Play("Phase4");
            break;
        }
    }

    void OnCollisionEnter2D(Collision2D collision){
        if(collision.gameObject.tag == "Player"){
            if(PlayerController.instance.YoshiBombTriggered && PlayerController.instance.AllowYoshiBombTrunk){
                PlayerController.instance.AllowYoshiBombTrunk = false;
                _phase++;
                return;
            }
        }
    }
}
