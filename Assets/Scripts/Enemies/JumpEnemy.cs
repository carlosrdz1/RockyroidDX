using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpEnemy : MonoBehaviour
{
    public float TIME_TO_STAY_IDLE; // const
    public float JUMP_FORCE;
    public float HORIZONTAL_FORCE;
    public int ID;
    public float CATCH_TIMER_DURATION;
    public int MIN_RANGE_NET;
    public int MAX_RANGE_NET;
    public int LIFE;
    public int DAMAGE;
    [SerializeField] float _timerStayingIdle;
    public Transform DownDetector;
    float DetectorSizeX = 0.55f;
    float DetectorSizeY = 0.5f;
    public LayerMask WhatIsGround;
    [SerializeField] bool _isGrounded;  
    Rigidbody2D _rigidBody2D;
    [SerializeField] EnemyState _enemyState;
    Animator _animations;
    SpriteRenderer _spriteRenderer;
    public GameObject Heart;

    enum EnemyState{
          IDLE
        , PREJUMP
        , JUMP
    }

    // Start is called before the first frame update
    void Start()
    {
        _rigidBody2D = this.GetComponent<Rigidbody2D>();
        // TIME_TO_STAY_IDLE = 3f;
        // JUMP_FORCE = 8f;
        // HORIZONTAL_FORCE = 1f;
        // ID = 2;
        // CATCH_TIMER_DURATION = 5f;
        _animations = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if(PlayerController.instance.transform.position.x - this.transform.position.x > 10f
            || PlayerController.instance.transform.position.x - this.transform.position.x < -10f
            || PlayerController.instance.transform.position.y - this.transform.position.y < -10f
            || PlayerController.instance.transform.position.y - this.transform.position.y > 10f){
                return;
            }

        if(NetSystem.instance.IsCatchingACreature){
            return;
        }

        _isGrounded = Physics2D.OverlapBox(DownDetector.position
                        , new Vector2(DetectorSizeX, DetectorSizeY)
                        , 0f, WhatIsGround);     

        if(_isGrounded && _timerStayingIdle == 0){
            _timerStayingIdle = TIME_TO_STAY_IDLE;
        }
        
        if(_timerStayingIdle > 0 && _isGrounded){
            _timerStayingIdle -= Time.deltaTime;

            if(_timerStayingIdle <= 0){
                _timerStayingIdle = 0;
                Jump();
            }
        }
        WatchTowardsPlayer();
        UpdateState();
        Animations();
    }

    void Animations(){
        _animations.Play(ID.ToString() + _enemyState.ToString());        
    }

    void UpdateState(){
        if(_isGrounded && _timerStayingIdle > 1f){
            _enemyState = EnemyState.IDLE;
        }

        if(_isGrounded && _timerStayingIdle <= 1f){
            _enemyState = EnemyState.PREJUMP;
        }

        if(!_isGrounded){
            _enemyState = EnemyState.JUMP;
        }
    }

    void Jump(){
        if(this.gameObject.transform.localScale.x == -1)
            _rigidBody2D.velocity = new Vector2(HORIZONTAL_FORCE, JUMP_FORCE);
        else{
            _rigidBody2D.velocity = new Vector2(-HORIZONTAL_FORCE, JUMP_FORCE);
        }
    }

    void WatchTowardsPlayer(){        
        if(PlayerController.instance.transform.position.x > this.transform.position.x){
            Vector3 theScale = this.gameObject.transform.localScale;         
            theScale.x = -1;
            this.gameObject.transform.localScale = theScale; 
        }
        
        if(PlayerController.instance.transform.position.x < this.transform.position.x){
            Vector3 theScale = this.gameObject.transform.localScale;          
            theScale.x = 1;
            this.gameObject.transform.localScale = theScale; 
        }        
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
               // Debug.Log(parent.name);
               // if(parent != null){
                    parent.GetComponent<Spawner>().HasBeenCaptured = true;
                //}
            }

            Destroy(collision.gameObject);
            Destroy(this.gameObject);

            Instantiate(NetSystem.instance.CaptureCapsule
                , new Vector3(this.transform.position.x, this.transform.position.y, this.transform.position.z)
                , Quaternion.identity);  
        }
    }
}
