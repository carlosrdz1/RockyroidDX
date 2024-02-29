using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spring : MonoBehaviour
{
    Animator _animations;
    bool _animationTriggered;
    float _timer;
    // Start is called before the first frame update
    void Start()
    {
        _animations = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        // if(PlayerController.instance.TriggerSpringJump && !_animationTriggered){          
        //     _timer = 0.5f;
        //     _animationTriggered = true;
        //      _animations.Play("Active");
        // } 

        if(_timer > 0){
            _timer -= Time.deltaTime;
        }
        
        if(_timer <= 0){
            _animations.Play("Idle");
            _timer = 0;
            _animationTriggered = false;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if(!_animationTriggered){          
            _timer = 0.5f;
            _animationTriggered = true;
             _animations.Play("Active");
        } 
    }
}
