using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingEnemy : MonoBehaviour
{
    public bool StartGoingDown;
    const float AMOUNT_OF_MOVEMENT = 3f;
    const float VELOCITY = 0.04f;
    float _initialY;
    public int ID;
    public float CATCH_TIMER_DURATION;
    public int MIN_RANGE_NET;
    public int MAX_RANGE_NET;
    public int LIFE;
    [SerializeField] float _distanceMoved;
    [SerializeField] bool _nowGoUp;
    [SerializeField] bool _nowGoDown;
    public GameObject Heart;
    SpriteRenderer _spriteRenderer;

    // Start is called before the first frame update
    void Start()
    {
        _initialY = this.transform.position.y;
        _spriteRenderer = GetComponent<SpriteRenderer>();

        if(StartGoingDown) {
            _nowGoUp = false;
            _nowGoDown = true;
            _distanceMoved = 0;
        } else {
            _nowGoUp = true;
            _nowGoDown = false;
            _distanceMoved = AMOUNT_OF_MOVEMENT;
        }
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

        if(NetSystem.instance.IsCatchingACreature) return;
        if(PauseManager.instance.IsPaused) return;

        if(_nowGoDown){
            _distanceMoved += VELOCITY;
            transform.position = new Vector2(transform.position.x, transform.position.y - VELOCITY);

            if(_distanceMoved >= AMOUNT_OF_MOVEMENT){
                _nowGoDown = false;
                _nowGoUp = true;
                _distanceMoved = AMOUNT_OF_MOVEMENT;
            }
        }

        if(_nowGoUp){
            _distanceMoved -= VELOCITY;
            transform.position = new Vector2(transform.position.x, transform.position.y + VELOCITY);

            if(_distanceMoved <= 0){
                _nowGoDown = true;
                _nowGoUp = false;
                _distanceMoved = 0;
            }
        }

        WatchTowardsPlayer();
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
