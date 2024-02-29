using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Capsule : MonoBehaviour
{  
    Rigidbody2D _rigidBody2D;
    float _velocity;
    bool _isRight;
    Vector3 _smoothDampZero = Vector3.zero;
    // Start is called before the first frame update
    void Start()
    {
        _rigidBody2D = GetComponent<Rigidbody2D>();
        
        if(_isRight){
            _velocity = 14f;
        } else {
            _velocity = -14f;
        }
    }

    // Update is called once per frame
    void Update()
    {
        DestroyIfFarAway();
    }

    void DestroyIfFarAway(){
        if(this.transform.position.x > PlayerController.instance.transform.position.x + 6f){
            Destroy(this.gameObject);
        }

        if(this.transform.position.x < PlayerController.instance.transform.position.x - 6f){
            Destroy(this.gameObject);
        }
    }

    void FixedUpdate(){
        Vector3 targetVel = new Vector2(_velocity, _rigidBody2D.velocity.y);
        _rigidBody2D.velocity = Vector3.SmoothDamp(_rigidBody2D.velocity, targetVel, ref _smoothDampZero, 0.05f);
    }

    public void SetDirection(bool GoesRight)
    {
        _isRight = GoesRight;
    }

    public void OnTriggerEnter2D(Collider2D collision){
       // if(collision.gameObject.tag == "Grid"){ 
            // Debug.Log(collision.gameObject.tag);
            // FindObjectOfType<AudioManager>().Play("bombExplode");
            // Destroy(this.gameObject);
       // }    
    }

    void OnCollisionStay2D(Collision2D collision){
        Debug.Log(collision.gameObject.tag);
        FindObjectOfType<AudioManager>().Play("bombExplode");
        Destroy(this.gameObject);
    }
}
