using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveLREnemy : MonoBehaviour
{
    public float JUMP_FORCE;
    public float HORIZONTAL_FORCE;
    public int ID;
    public float CATCH_TIMER_DURATION;
    public int MIN_RANGE_NET;
    public int MAX_RANGE_NET;
    public int LIFE;
    public int DAMAGE;
    public TypeOfEnemy TYPE_OF_ENEMY;
    public float TIMER_FLYING_DISTANCE;
    public Transform DownDetector;
    float DetectorSizeX = 0.55f;
    float DetectorSizeY = 0.5f;
    public LayerMask WhatIsGround;
    float _raycastDistance = 1.0f;
    float _cliffDetectionDistance = 1f;
    [SerializeField] bool _isGrounded;  
    [SerializeField] bool _touchLeft;  
    [SerializeField] bool _touchRight;  
    Rigidbody2D _rigidBody2D;
    [SerializeField] EnemyState _enemyState;
    [SerializeField] float flying_timer;
    Animator _animations;
    SpriteRenderer _spriteRenderer;
    [SerializeField] bool _movingLeft;
    public GameObject Heart;

    enum EnemyState{
          IDLE     
    }

    public enum TypeOfEnemy{
       GROUND_WALLS
     , GROUND_NO_WALLS
     , FLYING
    }

    // Start is called before the first frame update
    void Start()
    {             
        // JUMP_FORCE = 8f;
        // HORIZONTAL_FORCE = 2f;
        // ID = 6;
        // CATCH_TIMER_DURATION = 5f;
        // LIFE = 6;
        // MIN_RANGE_NET = 4;
        // MAX_RANGE_NET = 10;
        // TYPE_OF_ENEMY = TypeOfEnemy.FLYING;
        // TIMER_FLYING_DISTANCE = 3f;

        _rigidBody2D = this.GetComponent<Rigidbody2D>();   
        _animations = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _movingLeft = true;

        if(TYPE_OF_ENEMY == TypeOfEnemy.FLYING){
            flying_timer = TIMER_FLYING_DISTANCE;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(PlayerController.instance.transform.position.x - this.transform.position.x > 10f
            || PlayerController.instance.transform.position.x - this.transform.position.x < -10f
            || PlayerController.instance.transform.position.y - this.transform.position.y < -10f
            || PlayerController.instance.transform.position.y - this.transform.position.y > 10f){
                return;
            }

        if(NetSystem.instance.IsCatchingACreature){
            _rigidBody2D.Sleep();
            return;
        }

        switch(TYPE_OF_ENEMY){
            case TypeOfEnemy.GROUND_WALLS:
                GroundWallsMechanics();
            break;

            case TypeOfEnemy.GROUND_NO_WALLS:
                GroundNoWallsCliffMechanics();
            break;

            case TypeOfEnemy.FLYING:
                Flying();
            break;
        }

        UpdateState();
        Animations();
    }

    void Flying(){
        _rigidBody2D.gravityScale = 0;

        if(flying_timer == 0){
            _movingLeft = !_movingLeft;
            flying_timer = TIMER_FLYING_DISTANCE;
        }

        if(flying_timer > 0){
            flying_timer -= Time.deltaTime;
        }

        if(flying_timer < 0){
            flying_timer = 0;
        }

        if(_movingLeft){
            MoveEnemyToTheLeft();
        } else {
            MoveEnemyToTheRight();
        }
    }

    void GroundNoWallsCliffMechanics(){
        // Cast a ray downwards to detect if there is ground below.
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, _cliffDetectionDistance, WhatIsGround);        

        // If there's no ground detected, reverse the enemy's direction.
        if (hit.collider == null)
        {
            _movingLeft = !_movingLeft;
        }

        if(_movingLeft){
            MoveEnemyToTheLeft();
        } else {
            MoveEnemyToTheRight();
        }
    }

    void GroundWallsMechanics(){
        RaycastHit2D hitLeft = Physics2D.Raycast(transform.position, Vector2.left, _raycastDistance, WhatIsGround);
        RaycastHit2D hitRight = Physics2D.Raycast(transform.position, Vector2.right, _raycastDistance, WhatIsGround);

        Debug.DrawRay(transform.position, Vector2.left * _raycastDistance, Color.red);
        Debug.DrawRay(transform.position, Vector2.right * _raycastDistance, Color.blue);


        if(hitRight.collider != null){
            _movingLeft = true;
        }

        if(hitLeft.collider != null){
            _movingLeft = false;
        }

        if(_movingLeft){
            MoveEnemyToTheLeft();
        } else {
            MoveEnemyToTheRight();
        }
    }

    void MoveEnemyToTheLeft(){
        _rigidBody2D.velocity = new Vector2(-HORIZONTAL_FORCE, _rigidBody2D.velocity.y);
        Vector3 theScale = this.gameObject.transform.localScale;          
        theScale.x = 1;
        this.gameObject.transform.localScale = theScale; 
    }

    void MoveEnemyToTheRight(){
        _rigidBody2D.velocity = new Vector2(HORIZONTAL_FORCE, _rigidBody2D.velocity.y);
        Vector3 theScale = this.gameObject.transform.localScale;         
        theScale.x = -1;
        this.gameObject.transform.localScale = theScale; 
    }

    void Animations(){
        _animations.Play(ID.ToString() + _enemyState.ToString());        
    }

    void UpdateState(){
        _enemyState = EnemyState.IDLE;
    }

    public void OnTriggerEnter2D(Collider2D collision){
        if(collision.gameObject.tag == "Boomerang"){
            LevelManager.instance.AmountOfBoomerangs--;
            FindObjectOfType<AudioManager>().Stop("boomerang");
            Destroy(collision.gameObject);            

            if(LIFE > 1){
                LIFE--;
                StartCoroutine(EffectsHelper.FlashEnemy(_spriteRenderer, 16, 0.02f));
                FindObjectOfType<AudioManager>().Play("damageEnemy");
            } else {
                FindObjectOfType<AudioManager>().Play("bombExplode");
                CinemachineHelper.instance.ShakeCamera();
                Destroy(this.gameObject);

                int randomNumber = Random.Range(1, 11);

                if(randomNumber >= 5){
                    Instantiate(Heart
                    , new Vector3(this.transform.position.x, this.transform.position.y, this.transform.position.z)
                    , Quaternion.identity);       
                }  
            }
        } 

        if(collision.gameObject.tag == "Net"){            
            NetSystem.instance.IsCatchingACreature = true;
            NetSystem.instance.TimerDuration = CATCH_TIMER_DURATION;
            NetSystem.instance.CreatureID = ID;
            NetSystem.instance.MinRangeNet = MIN_RANGE_NET;
            NetSystem.instance.MaxRangeNet = MAX_RANGE_NET;

            if(this.transform.parent != null){
                var parent = this.transform.parent.gameObject; 
                parent.GetComponent<Spawner>().HasBeenCaptured = true;                
            }

            Destroy(collision.gameObject);
            Destroy(this.gameObject);

            Instantiate(NetSystem.instance.CaptureCapsule
                , new Vector3(this.transform.position.x, this.transform.position.y, this.transform.position.z)
                , Quaternion.identity);  
        }
    }

    // This method draws the raycast in the Scene view in the Unity Editor.
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red; // Set the color of the raycast line.

        // Calculate the raycast origin position.
        Vector2 raycastOrigin = transform.position;
        raycastOrigin.y -= GetComponent<BoxCollider2D>().size.y / 2; // Adjust for the height of the enemy.

        // Draw the raycast line.
        Gizmos.DrawRay(raycastOrigin, Vector2.down * _cliffDetectionDistance);
    }
}
