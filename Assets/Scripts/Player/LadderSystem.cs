using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LadderSystem : MonoBehaviour
{
    public static LadderSystem instance;
    Rigidbody2D _rigidBody2D;

    [Header("Ladder Variables")]
    [SerializeField] bool _isTouchingLadder = false;
    [SerializeField] bool _isGrabbingLadder = false;
    [SerializeField] float _currentLadderPosX = 0f;
    [SerializeField] float _currentLadderPosY = 0f;
    [SerializeField] bool _positionHasBeenSaved = false;
    [SerializeField] bool _triggerGoingDownLadder = false;
    [SerializeField] float _newPosYGoingDown = 0f;
    const float LADDER_SPEED = 0.1f;

    public bool TriggerGoingDownLadder {
        get { return _triggerGoingDownLadder; }
    }

    public bool IsTouchingLadder { get { return _isTouchingLadder;} set {_isTouchingLadder = value; }}
    public bool IsGrabbingLadder { get { return _isGrabbingLadder;} set {_isGrabbingLadder = value; }}
    public float CurrentLadderPosX { get { return _currentLadderPosX;} set {_currentLadderPosX = value; }}
    public float CurrentLadderPosY { get { return _currentLadderPosY;} set {_currentLadderPosY = value; }}

    void Awake(){
        if(instance == null){
            instance = this;
        } 

        _rigidBody2D = GetComponent<Rigidbody2D>();   
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    void LadderRoutine(){
        PlayerController.instance.UpdatePlayerState();
        
        if(_triggerGoingDownLadder){
            if(!_positionHasBeenSaved){
                _positionHasBeenSaved = true;
                _newPosYGoingDown = this.transform.position.y - 2f;
            }

            this.transform.position = 
                Vector2.MoveTowards(this.transform.position
                , new Vector2(this.transform.position.x, _newPosYGoingDown)
                , LADDER_SPEED);

            if(transform.position.y <= _newPosYGoingDown){
                transform.position = new Vector2(transform.position.x, _newPosYGoingDown);
                _positionHasBeenSaved = false;
                _triggerGoingDownLadder = false;
            }     
        }

        if(TopLadder._triggerUpwardsMovement) {
            this.transform.position = 
                Vector2.MoveTowards(this.transform.position
                , new Vector2(this.transform.position.x, _currentLadderPosY + 2.5f)
                , LADDER_SPEED);
                  
            _rigidBody2D.bodyType = RigidbodyType2D.Kinematic;

            if(transform.position.y >= _currentLadderPosY + 2.5f){
                transform.position = new Vector2(transform.position.x, _currentLadderPosY + 2.5f);
                TopLadder._triggerUpwardsMovement = false;
                _rigidBody2D.bodyType = RigidbodyType2D.Dynamic;
                PlayerController.instance.AllowYoshiBomb = true;
            }     

            return;
        }

        // This is used when it is first grabbing the ladder from below.
        // It WON'T trigger the animation of going up because it is not reaching the top of the ladder.
        if(PlayerController.instance.IsUpPressed && _isTouchingLadder && !PlayerController.instance.IsGrounded){
            _rigidBody2D.bodyType = RigidbodyType2D.Kinematic;
            _isGrabbingLadder = true;     
            _rigidBody2D.position = new Vector2(_currentLadderPosX, this.transform.position.y);      
            _rigidBody2D.velocity = Vector3.zero; 
        }

        // This is used to start using the ladder from above, going downgards.
        // It will trigger the animation of going down.
        if(PlayerController.instance.IsDownPressed && _isTouchingLadder && PlayerController.instance.IsGrounded){
            PlayerController.instance.IsGrounded = false;
            _isGrabbingLadder = true;     
            _triggerGoingDownLadder = true;
            this.transform.position = new Vector2(_currentLadderPosX, this.transform.position.y);// -1.8f);                   
            _rigidBody2D.bodyType = RigidbodyType2D.Kinematic;
            _rigidBody2D.velocity = Vector3.zero; 
        }     

        if(_isGrabbingLadder && PlayerController.instance.IsUpPressed){
            _rigidBody2D.position = new Vector2(_currentLadderPosX, this.transform.position.y + LADDER_SPEED);
        }

        if(_isGrabbingLadder && PlayerController.instance.IsDownPressed){
            _rigidBody2D.position = new Vector2(_currentLadderPosX, this.transform.position.y - LADDER_SPEED);
        }

        //UpdateLadderAnimations();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        LadderRoutine();
    }
}
