using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boomerang : MonoBehaviour
{
    Rigidbody2D _rigidBody2D;
    Vector2 _force;
    Vector3 _smoothDampZero = Vector3.zero;
    bool _isRight;
    float _velocity = 14f;
    float _deceleration = -0.4f;
    float _randomVelY;

    private void Start()
    {
        _rigidBody2D = GetComponent<Rigidbody2D>();

        if(_isRight){
            _velocity = 14f;
            _deceleration = -0.4f;
        } else {
            _velocity = -14f;
            _deceleration = 0.4f;
        }

        _randomVelY = Random.Range(-3f, 3f);

        LevelManager.instance.AmountOfBoomerangs++;
    }

    private void Update()
    {
        _velocity += _deceleration;

        if(_isRight){
            if(_velocity < -16f) 
                DestroyBoomerang();
        }
        else {
            if(_velocity > 16f) 
                DestroyBoomerang();
        }
    }

    void DestroyBoomerang(){
        LevelManager.instance.AmountOfBoomerangs--;
        Destroy(this.gameObject);
    }

    void FixedUpdate(){
        Vector3 targetVel = new Vector2(_velocity, _randomVelY);
        _rigidBody2D.velocity = Vector3.SmoothDamp(_rigidBody2D.velocity, targetVel, ref _smoothDampZero, 0.05f);
    }

    public void SetDirection(bool GoesRight)
    {
        _isRight = GoesRight;
    }
}