using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spaceship : MonoBehaviour
{
    bool _triggerEffect;
    Rigidbody2D _rb;
    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if(PlayerController.instance.CurrentLevelFinished && !_triggerEffect){
            _triggerEffect = true;
            _rb.velocity = new Vector2(_rb.velocity.x, 4f);
        }
    }
}
