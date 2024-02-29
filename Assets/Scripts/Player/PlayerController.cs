using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;
    bool _allowHorizontalMovement = true;
    readonly float _playerVelocity = 6f;    
    float _horizontal;
    float _vertical;    
    [SerializeField] bool _isGrounded;  
    bool _isWaitingWhileHit;  
    float _directionPressed;
    [SerializeField] float _lastDirectionPressed = 1;
    SpriteRenderer _spriteRenderer;
    Rigidbody2D _rigidBody2D;
    Vector3 _smoothDampZero = Vector3.zero;
    float DetectorSizeX = 0.55f;
    float DetectorSizeY = 0.5f;
    const float BACK_FORCE = 25f;
    public Transform DownDetector;
    public LayerMask WhatIsGround;
    Animator _playerSprites;
    [SerializeField] PlayerState _playerState;
    public bool PlayerIsInCamera;
    [SerializeField] Transform _detDown;
    public GameObject SparkCoin;

    [Header("Player Stats")]
    public bool IsDead;
    public int Life;
    public bool IsInvincible;
    public bool CurrentLevelFinished;

    [Header("Jump Variables")]
    [SerializeField] float _hangCounter;
    [SerializeField] bool _isJumpingControl;
    [SerializeField] bool _allowJump;    
    [SerializeField] float _jumpBufferCount;    
	[SerializeField] bool _isJumping;
    [SerializeField] bool _isFalling;
    bool _spawnParticles;
    const float HANG_TIME = 0.1f;
    const float JUMP_FORCE = 14f;	
    const float JUMP_BUFFER_LENGHT = 0.1f; 
    public GameObject DustParticle;

    [Header("Down Yoshi Mechanics")]
    [SerializeField] bool _isDownPressed;
    [SerializeField] bool _yoshiBombTriggered;
    const float YOSHI_WAIT_TIME = 0.2f;
    const float YOSHI_BOMB_FORCE = 18f;
    [SerializeField] float _yoshi_wait_timer;
    [SerializeField] bool _allowYoshiBomb;
    [SerializeField] bool _allowYoshiBombTrunk;

    [Header("Helicopter Spin")]
    [SerializeField] bool _canHelicopterSpin;
    [SerializeField] bool _helicopterWasTriggered;
    float _gravityStore = 0f;
    const float GRAVITY_HELICOPTER = 1f;
    const float HELICOPTER_SPIN_VELOCITY = -2f;

    [Header("Weapons")]
    [SerializeField] bool _isFirePressed;
    public GameObject Boomerang;
    bool _allowFireWeapon = true;
    const int AMOUNT_OF_BOOMERANGS_ALLOWED = 1;
    const float WEAPON_FIRED_TIME = 0.2f;
    float _weaponFiredTimerForAnimations;
    [SerializeField] int _powerUpSelected;
    public GameObject Capsule;
    public Animator CurrentItemAnimation;

    [Header("Spring")]
    [SerializeField] bool _triggerSpringJump;
    [SerializeField] bool _springJumpIsActive;
    const float SPRING_FORCE = 18f;

    [Header("Controls")]
    [SerializeField] bool _isUpPressed;
    [SerializeField] bool _isLeftPressed;
    [SerializeField] bool _isRightPressed;
    [SerializeField] bool _allowLeft;
    [SerializeField] bool _allowRight;
    [SerializeField] bool _allowUp;
    [SerializeField] bool _allowDown;

    public enum PlayerState {
          IDLE
        , JUMP
        , WALK
        , FALL
        , CROUCH
        , YOSHI_BOMB
        , HIT
        , DEAD
        , SHOOT
    }

    public bool TriggerSpringJump{ get { return _springJumpIsActive; }}
    public bool YoshiBombTriggered { get { return _yoshiBombTriggered; }}  
    public bool IsGrounded {get {return _isGrounded; } set {_isGrounded = value; } }
    public bool AllowYoshiBomb { get { return _allowYoshiBomb; } set {_allowYoshiBomb = value;}}      
    public bool AllowYoshiBombTrunk { get { return _allowYoshiBombTrunk; } set {_allowYoshiBombTrunk = value;}}    

    public bool IsUpPressed {get {return _isUpPressed; }}
    public bool IsDownPressed {get {return _isDownPressed; }}
    public bool IsLeftPressed {get {return _isLeftPressed; }}
    public bool IsRightPressed {get {return _isRightPressed; }}
    public bool IsJumpPressed {get {return _isJumpingControl; }}
    public bool IsFirePressed {get {return _isFirePressed; }}

    public bool AllowUp {get {return _allowUp; } set { _allowUp = value;}}
    public bool AllowDown {get {return _allowDown; } set {_allowDown = value;}}
    public bool AllowLeft {get {return _allowLeft; } set { _allowLeft = value;}}
    public bool AllowRight {get {return _allowRight; } set {_allowRight = value;}}
    public bool AllowJump {get{return _allowJump;} set { _allowJump = value; }}
    public bool AllowFireWeapon {get{return _allowFireWeapon;} set { _allowFireWeapon = value; }}

    void Awake(){
        if(instance == null){
            instance = this;
        } 

        Application.targetFrameRate = 60;  
        _rigidBody2D = GetComponent<Rigidbody2D>();   
        _spriteRenderer = GetComponent<SpriteRenderer>();	
        Life = 4;
    }

    // Start is called before the first frame update
    void Start()
    {     
        _allowJump = true;
        _allowUp = true;
        _allowDown = true;
        _allowLeft = true;
        _allowRight = true;
        _allowYoshiBomb = true;
        _allowYoshiBombTrunk = true;
        _playerState = PlayerState.IDLE;   
        _playerSprites = GetComponent<Animator>();
        _gravityStore = _rigidBody2D.gravityScale;
        _powerUpSelected = 1;
    }

    // Update is called once per frame
    void Update()
    {       
        if(IsDead || CurrentLevelFinished){
            _playerState = PlayerState.DEAD;
            UpdateAnimations();
            _rigidBody2D.Sleep();
            return;
        }

        if(!LevelManager.instance.IntroIsFinished) return;

        if(NetSystem.instance.IsCatchingACreature){
            _rigidBody2D.Sleep();
            return;
        }
        
        UpdateAnimations();
        UpdatePlayerState();
        PowerUps();  
    }

    void FixedUpdate(){
        if(!LevelManager.instance.IntroIsFinished) return;

        if(IsDead || CurrentLevelFinished) 
        {
            _playerState = PlayerState.DEAD;
            UpdateAnimations();
            _rigidBody2D.Sleep();
            return;
        }

        if(NetSystem.instance.IsCatchingACreature){
            _rigidBody2D.velocity = new Vector2(0f, 0f);
            return;
        }
        // Movement
        PlayerHorizontalMovement();
        PlayerJump();
        Spring();
        HelicopterSpin();
        YoshiBombMovement();
        CrouchRoutine();
        FlipSprite();        

        _isGrounded = Physics2D.OverlapBox(DownDetector.position, new Vector2(DetectorSizeX, DetectorSizeY), 0f, WhatIsGround);     
        //Debug.Log("okoko;: " + DetectorSizeX);
    }

    private void OnDrawGizmos()
    {
        // Gizmos.color = Color.red;
        // Gizmos.DrawWireCube(DownDetector.position, new Vector2(DetectorSizeX, DetectorSizeY));
    }

    void CrouchRoutine(){
        if(LadderSystem.instance.IsTouchingLadder) return;

        if(_horizontal == 0 && _isGrounded && _isDownPressed){
            _rigidBody2D.velocity = new Vector2(0, 0);
        }
    }

    void Spring(){
        if(_triggerSpringJump){
            _triggerSpringJump = false;
            if(_isFalling){       
                //original line
                _rigidBody2D.gravityScale = _gravityStore;

                if(_yoshiBombTriggered){
                    StopYoshiBombMovement();
                    _rigidBody2D.velocity = new Vector2(_rigidBody2D.velocity.x, 22f);
                } else {
                    _rigidBody2D.velocity = new Vector2(_rigidBody2D.velocity.x, 18f);
                }

                FindObjectOfType<AudioManager>().Play("spring");
                _isJumping = true;
                _isGrounded = false;
                _hangCounter = 0;
                _jumpBufferCount = 0;
                StopHelicopterSpin();
                //end                
            }
        }

        if(_isGrounded || !_isJumping){
            _springJumpIsActive = false;
        }
    }

    public void UpdatePlayerState(){
        if(_rigidBody2D.velocity.y < 0) _isJumping = false;      

		if(!_isGrounded && !_isJumping) {
            _isFalling = true;
            _spawnParticles = true;
        }

        if(_isGrounded || _isJumping) _isFalling = false;

        if(IsDead){
            _playerState = PlayerState.DEAD;
            return;
        }

        if(_horizontal != 0 && _isGrounded && !_isDownPressed){
            _playerState = PlayerState.WALK;
        }

        if(_horizontal == 0 && _isGrounded){
            _playerState = PlayerState.IDLE;
        }

        if(_horizontal == 0 && _isGrounded && _isDownPressed){
            _playerState = PlayerState.CROUCH;
        }

        if(_yoshiBombTriggered){
            _playerState = PlayerState.YOSHI_BOMB;
        } else {
            if(!_isGrounded && _isJumping){
                _playerState = PlayerState.JUMP;
            }

            if(!_isGrounded && _isFalling){
                _playerState = PlayerState.FALL;
            }
        }

        if(_weaponFiredTimerForAnimations > 0){
            _playerState = PlayerState.SHOOT;
        }
    }

    void UpdateAnimations(){
        switch(_playerState){
            case PlayerState.IDLE:
                _playerSprites.Play("Idle");
            break;

            case PlayerState.WALK:
                _playerSprites.Play("Walk");
            break;   

            case PlayerState.CROUCH:
                _playerSprites.Play("Crouch");
            break;   

            case PlayerState.SHOOT:
                _playerSprites.Play("Shoot");
            break;   

            case PlayerState.JUMP:
                _playerSprites.Play("Jump");
            break;   

            case PlayerState.FALL:
                _playerSprites.Play("Fall");
            break;   

            case PlayerState.YOSHI_BOMB:
                _playerSprites.Play("Bomb");
            break;

            case PlayerState.DEAD:
                _playerSprites.Play("Dead");
            break;
        }
    }

    void FlipSprite(){
        _directionPressed = _horizontal;
        // Flip SPRITE.
        if(_directionPressed != 0){
            if(_directionPressed == -1){
                _lastDirectionPressed = -1;
                Vector3 theScale = this.gameObject.transform.localScale;         
                theScale.x = -1;
                this.gameObject.transform.localScale = theScale; 
            }
            
            if(_directionPressed == 1){
                _lastDirectionPressed = 1;
                Vector3 theScale = this.gameObject.transform.localScale;          
                theScale.x = 1;
                this.gameObject.transform.localScale = theScale; 
            }
        }
    }

    void PowerUps(){        
        if(_isUpPressed && _allowUp){
            _allowUp = false;
            
            if(_powerUpSelected == 1){
                _powerUpSelected = 2;
            } else if (_powerUpSelected == 2){
                _powerUpSelected = 1;
            }

            FindObjectOfType<AudioManager>().Play("moveCursor");

            switch(_powerUpSelected){
                case 1:
                    CurrentItemAnimation.Play("Boomerang");
                break;

                case 2:
                    CurrentItemAnimation.Play("Capsule");
                break;
            }
        }

        switch(_powerUpSelected){
            case 1:
            //Debug.Log("entra al suich"); // SI ENTRA
                if(_isFirePressed && _allowFireWeapon && LevelManager.instance.AmountOfBoomerangs < AMOUNT_OF_BOOMERANGS_ALLOWED){
                    if(_yoshiBombTriggered) return;
                    _allowFireWeapon = false;
                    float _positionForInstantiate;            
                    _positionForInstantiate = _lastDirectionPressed == 1 ? 0.7f : -0.7f;
                    _weaponFiredTimerForAnimations = WEAPON_FIRED_TIME;
                    GameObject bullet = Instantiate(Boomerang, new Vector2(transform.position.x + _lastDirectionPressed, transform.position.y), Quaternion.identity);
                    Boomerang boomerang = bullet.GetComponent<Boomerang>();

                    if (boomerang != null)
                    {
                        boomerang.SetDirection(_lastDirectionPressed == 1);
                        FindObjectOfType<AudioManager>().Play("boomerang");
                    }            
                }
            break;

            case 2:
                if(_isFirePressed && _allowFireWeapon){
                    if(_yoshiBombTriggered) return;
                    _allowFireWeapon = false;
                    float _positionForInstantiate;            
                    _positionForInstantiate = _lastDirectionPressed == 1 ? 0.7f : -0.7f;
                    _weaponFiredTimerForAnimations = WEAPON_FIRED_TIME;

                    GameObject bullet = Instantiate(Capsule, new Vector2(transform.position.x + _lastDirectionPressed, transform.position.y), Quaternion.identity);
                    Capsule capsule = bullet.GetComponent<Capsule>();

                    if (capsule != null)
                    {
                        capsule.SetDirection(_lastDirectionPressed == 1);
                        FindObjectOfType<AudioManager>().Play("capsule");
                    }            
                }
            break;
        }

        if(_weaponFiredTimerForAnimations > 0){
            _weaponFiredTimerForAnimations -= Time.deltaTime;

            if(_weaponFiredTimerForAnimations <= 0)
                _weaponFiredTimerForAnimations = 0;
        }
    }

    void YoshiBombMovement(){
        if(LadderSystem.instance.IsGrabbingLadder) {
             _yoshiBombTriggered = false;
            _yoshi_wait_timer = 0;
            _allowYoshiBomb = false;
            return;   
        }       

        // Initialize movement. If player is in the air and press down button: trigger timer to suspend the player before the movement.
        if(_allowYoshiBomb && !_isGrounded && _isDownPressed && !_yoshiBombTriggered && !IsInvincible){
            _allowYoshiBomb = false;
            _yoshiBombTriggered = true;
            _allowHorizontalMovement = false;
            _rigidBody2D.bodyType = RigidbodyType2D.Kinematic;
            _yoshi_wait_timer = YOSHI_WAIT_TIME;
            _rigidBody2D.velocity = new Vector2(0, 0);
            FindObjectOfType<AudioManager>().Play("triggeredYoshiBomb");                
        }

        // If Yoshi Bomb mov. is triggered and it lands, shake camera.
        if(_yoshiBombTriggered && _isGrounded && !(_isJumping || _isFalling)){
            CinemachineHelper.instance.ShakeCameraCustom(1f, 0.4f);
            FindObjectOfType<AudioManager>().Play("hitYoshiBomb");
        }

        // If grounded, stop movement.
        if(_isGrounded){
            StopYoshiBombMovement();
        }

        // While the timer is not 0...
        if(_yoshi_wait_timer > 0){
            _yoshi_wait_timer -= Time.deltaTime;
        }

        // If the timer is 0 and the movement has been triggered, start movement.
        if(_yoshi_wait_timer <= 0 && _yoshiBombTriggered){
            _yoshi_wait_timer = 0;
            _rigidBody2D.bodyType = RigidbodyType2D.Dynamic;
            _rigidBody2D.velocity = new Vector2(0, -YOSHI_BOMB_FORCE);
        }
    }

    void StopYoshiBombMovement(){
        _yoshiBombTriggered = false;
        _allowHorizontalMovement = true;
        _rigidBody2D.bodyType = RigidbodyType2D.Dynamic;
        _yoshi_wait_timer = 0;
    }

    void PlayerJump(){       
        if(_isGrounded){
            _hangCounter = HANG_TIME;
        } else {
            _hangCounter -= Time.deltaTime;
        }

        if(_hangCounter < 0) _hangCounter = 0;

        if(_isGrounded && _isJumpingControl && _allowJump && _playerState != PlayerState.CROUCH){   
            _allowJump = false;         
            _jumpBufferCount = JUMP_BUFFER_LENGHT;
        } else {
            _jumpBufferCount -= Time.deltaTime;
        }

        if(_jumpBufferCount < 0) _jumpBufferCount = 0;

        if(_jumpBufferCount > 0 && _hangCounter > 0f){
            //AQUI NO ENTRA
            StopHelicopterSpin();
            FindObjectOfType<AudioManager>().Play("jump");
            _rigidBody2D.velocity = new Vector2(_rigidBody2D.velocity.x, JUMP_FORCE);
            _isJumping = true;
            _isGrounded = false;
            _hangCounter = 0;
            _jumpBufferCount = 0;
        }

        // If user stops pressing jump button...
        if(!_isJumpingControl && _rigidBody2D.velocity.y > 0 && !_springJumpIsActive){
            _rigidBody2D.velocity = new Vector2(_rigidBody2D.velocity.x, _rigidBody2D.velocity.y * .5f);             
            _allowJump = true;
        }

        if(_rigidBody2D.velocity.y < -12){
            _rigidBody2D.velocity = new Vector2(_rigidBody2D.velocity.x, -12);
        }   

        if(_isGrounded && _spawnParticles){
            _spawnParticles = false;

            Instantiate(DustParticle
                , new Vector3(this.transform.position.x, this.transform.position.y - .02f, this.transform.position.z)
                , Quaternion.identity);  
        }
    }

    void HelicopterSpin(){
        // If it's NOT in ground and jump button is not pressed, then it can perform the helicopter spin.
        if(!_isGrounded){
            if(!_isJumpingControl){
                _canHelicopterSpin = true;
            }
        }

        // If jump button is pressed while the player can do the helicopter spin, then it will perform the movement.
        if(!_springJumpIsActive && _isFalling && _isJumpingControl && _canHelicopterSpin && !_helicopterWasTriggered && _hangCounter == 0){
            _helicopterWasTriggered = true;       
            // Aqui es donde salta mucho
            _rigidBody2D.gravityScale = 0;     
            _rigidBody2D.velocity = new Vector2(_rigidBody2D.velocity.x, HELICOPTER_SPIN_VELOCITY);             
            _allowJump = false;
        }

        // If it's touching ground, helicopter spin movement will end.
        if(_isGrounded){
            StopHelicopterSpin();
        }
    }

    void StopHelicopterSpin(){
        _rigidBody2D.gravityScale = _gravityStore;
        _helicopterWasTriggered = false;
        _canHelicopterSpin = false;
    }

    void PlayerHorizontalMovement(){
        if(_playerState == PlayerState.CROUCH) return;
        if(LadderSystem.instance.IsGrabbingLadder) return;
        
        if(_allowHorizontalMovement){
            Vector3 targetVel = new Vector2(_horizontal * _playerVelocity, _rigidBody2D.velocity.y);
            _rigidBody2D.velocity = Vector3.SmoothDamp(_rigidBody2D.velocity, targetVel, ref _smoothDampZero, 0.05f);
        }
    }

    void DeathRoutine(){
        IsDead = true;        
        LevelManager.instance.RestartLevel = true;
        Life = 0;
        FindObjectOfType<AudioManager>().Stop("level");
        FindObjectOfType<AudioManager>().Play("deathTheme");
    }

    public void StopTimeScale(float duration){
        if(_isWaitingWhileHit) return;
        Time.timeScale = 0.0f;
        StartCoroutine(WaitWhileTimeScaleIsZero(duration));
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Spring" && !IsDead && _isFalling){ 
            _triggerSpringJump = true;
            _springJumpIsActive = true;
        }
    }

    public void OnTriggerEnter2D(Collider2D collision){
        if(collision.gameObject.tag == "InstaDeath"){
            DeathRoutine();
        } 

        if(collision.gameObject.tag == "Coin"){
            Instantiate(SparkCoin
                , new Vector3(collision.gameObject.transform.position.x
                            , collision.gameObject.transform.position.y
                            , collision.gameObject.transform.position.z)
                , Quaternion.identity);              

            Destroy(collision.gameObject);            
            FindObjectOfType<AudioManager>().Play("coin");
            LevelManager.instance.AmountOfCoins++;
            LevelManager.instance.CoinsRoutine();
        } 

        if(collision.gameObject.tag == "Spaceship"){
            CurrentLevelFinished = true;
            FindObjectOfType<AudioManager>().Stop("level");
            FindObjectOfType<AudioManager>().Play("finishLevel");

            if(SceneManager.GetActiveScene().name != "TrainingLevel"){
                PlayerStats.instance.CurrentLevelID++;
            }            
        } 
    }

    void OnTriggerStay2D(Collider2D collision){
        if(collision.gameObject.tag == "Spike" && !IsInvincible){
            DeathRoutine();
        } 

        if(collision.gameObject.tag == "Ladder"){
            LadderSystem.instance.IsTouchingLadder = true;
            LadderSystem.instance.CurrentLadderPosX = collision.gameObject.transform.position.x;
            LadderSystem.instance.CurrentLadderPosY = collision.gameObject.transform.position.y;
        }      
        if(collision.gameObject.tag == "Heart" && !IsDead){
            if(Life < 4){
                Life++;                
            }

            Destroy(collision.gameObject);
            FindObjectOfType<AudioManager>().Play("heart");
        }

        if(collision.gameObject.tag == "Enemy" && !IsDead && !IsInvincible){ 
            
            if(SceneManager.GetActiveScene().name != "TrainingLevel")
                Life--;

            if(Life > 0){
                StopHelicopterSpin();
                FindObjectOfType<AudioManager>().Play("hit");
                CinemachineHelper.instance.ShakeCamera();
                StopTimeScale(0.1f);                
            }

            if(Life == 0){
                DeathRoutine();
            }

            StopYoshiBombMovement();
        }

        if(collision.gameObject.tag == "Room"){
            PlayerIsInCamera = true;
        } 
    }

    void OnTriggerExit2D(Collider2D collision){
        if(collision.gameObject.tag == "Room"){
            PlayerIsInCamera = false;
        }

        if(collision.gameObject.tag == "Ladder"){
            LadderSystem.instance.IsTouchingLadder = false;
            LadderSystem.instance.CurrentLadderPosX = 0f;
            LadderSystem.instance.IsGrabbingLadder = false;                       
            _rigidBody2D.bodyType = RigidbodyType2D.Dynamic;
        }
    }

    IEnumerator WaitWhileTimeScaleIsZero(float duration){
        _isWaitingWhileHit = true;
        IsInvincible = true;
        yield return new WaitForSecondsRealtime(duration);
        StartCoroutine(EffectsHelper.FlashSprite(_spriteRenderer, 16, 0.02f));        
        Time.timeScale = 1.0f;
        _isWaitingWhileHit = false;

        if(_directionPressed == 1){
            _rigidBody2D.velocity = new Vector2(-BACK_FORCE, _rigidBody2D.velocity.y);
        } else if(_directionPressed == -1){
            _rigidBody2D.velocity = new Vector2(BACK_FORCE, _rigidBody2D.velocity.y);
        }
    }

    /*============================================================================
               _____          __  __ ______ _____        _____  
             / ____|   /\   |  \/  |  ____|  __ \ /\   |  __ \ 
            | |  __   /  \  | \  / | |__  | |__) /  \  | |  | |
            | | |_ | / /\ \ | |\/| |  __| |  ___/ /\ \ | |  | |
            | |__| |/ ____ \| |  | | |____| |  / ____ \| |__| |
            \_____/_/    \_\_|  |_|______|_| /_/    \_\_____/                                                                              
    
    ============================================================================*/
    public void Move(InputAction.CallbackContext context){     
        if(context.performed)
            _horizontal = context.ReadValue<Vector2>().x;
        
        if(context.canceled){
            _horizontal = context.ReadValue<Vector2>().x;
        }
    }

    public void Down(InputAction.CallbackContext context){    
        if(context.performed)
            _isDownPressed = true;
        
        if(context.canceled){
            _isDownPressed = false;
            _allowYoshiBomb = true;
            _allowYoshiBombTrunk = true;
            _allowDown = true;
        }
    }

    public void Up(InputAction.CallbackContext context){
        if(context.performed){
            _isUpPressed = true;
        }
        
        if(context.canceled){    
            _isUpPressed = false;
            _allowUp = true;
        }
    }

    public void Left(InputAction.CallbackContext context){
        //if(PauseManager.instance.IsPaused) return;
        if(context.performed){
            _isLeftPressed = true;
        }
        
        if(context.canceled){    
            _isLeftPressed = false;
            _allowLeft = true;
        }
    }

    public void Right(InputAction.CallbackContext context){
        //if(PauseManager.instance.IsPaused) return;
        if(context.performed){
            _isRightPressed = true;
        }
        
        if(context.canceled){    
            _isRightPressed = false;
            _allowRight = true;
        }
    }

    public void Jump(InputAction.CallbackContext context){
        //if(PauseManager.instance.IsPaused) return;

        if(context.started){
            _isJumpingControl = true;
        }
        
        if(context.canceled){
            _isJumpingControl = false;
            _allowJump = true;
            _rigidBody2D.gravityScale = _gravityStore;
        }
    }

    public void Fire(InputAction.CallbackContext context){     
        //if(PauseManager.instance.IsPaused) return;
        if(context.performed)
            _isFirePressed = true;
        
        if(context.canceled){
            _isFirePressed = false;
            _allowFireWeapon = true;
        }
    }
}
