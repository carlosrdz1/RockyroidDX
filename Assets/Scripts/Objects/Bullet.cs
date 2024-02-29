using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float VELOCITY;
    public bool GOES_LEFT;
    Rigidbody2D _rb;

    // Start is called before the first frame update
    void Start()
    {
        // VELOCITY= 4f;
        // GOES_LEFT = true;

        SetDirection(GOES_LEFT);
        _rb = GetComponent<Rigidbody2D>();
        _rb.velocity = new Vector2(VELOCITY, _rb.velocity.y);
    }

    // Update is called once per frame
    void Update()
    {
        if(PlayerController.instance.transform.position.x - this.transform.position.x > 10f
            || PlayerController.instance.transform.position.x - this.transform.position.x < -10f
            || PlayerController.instance.transform.position.y - this.transform.position.y < -10f
            || PlayerController.instance.transform.position.y - this.transform.position.y > 10f){
                Destroy(this.gameObject);
            }

        if(NetSystem.instance.IsCatchingACreature){
            Destroy(this.gameObject);
        }      
        
    }

    public void SetDirection(bool left){
        if(left){
            VELOCITY *= -1;
        }
    }

    public void SetVelocity(float vel){
        VELOCITY = vel;
    }

    void OnTriggerEnter2D(Collider2D collision){
        if(collision.gameObject.tag == "Player" ||
        collision.gameObject.tag == "Grid"){
            Destroy(this.gameObject);
        }
    }
}
