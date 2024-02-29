using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamagedBlock : MonoBehaviour
{
    Animator _animations;
    bool _triggerDamage;
    float _timerToRestore;
    const float TIME_RESTORE = 10f;
    // Start is called before the first frame update
    void Start()
    {
        _animations = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if(_triggerDamage){
            _triggerDamage = false;
            _animations.Play("Active");
            _timerToRestore = TIME_RESTORE;
        }

 
        if(!_triggerDamage && _timerToRestore > 0){
            _timerToRestore -= Time.deltaTime;
        }

        if(_timerToRestore <= 0){
            _timerToRestore = 0;
            _animations.Play("Idle");
            _triggerDamage = false;
        }        
    }

    void OnCollisionEnter2D(Collision2D collision){
        if(collision.gameObject.tag == "Player"){
            _triggerDamage = true;
        }
    }
}
