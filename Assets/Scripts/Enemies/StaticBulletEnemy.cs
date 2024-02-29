using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticBulletEnemy : MonoBehaviour
{    
    public int ID;
    public float BULLET_VELOCITY;
    public float TIME_BETWEEN_BULLETS;
    public float CATCH_TIMER_DURATION;
    public int MIN_RANGE_NET;
    public int MAX_RANGE_NET;
    public int LIFE;
    public int DAMAGE;
    SpriteRenderer _spriteRenderer;
    public GameObject Heart;
    public GameObject BulletObject;
    float _timerBullets;

    // Start is called before the first frame update
    void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();   
        _timerBullets = TIME_BETWEEN_BULLETS;     
    }

    // Update is called once per frame
    void Update()
    {
        WatchTowardsPlayer();

        if(PlayerController.instance.transform.position.x - this.transform.position.x > 10f
            || PlayerController.instance.transform.position.x - this.transform.position.x < -10f
            || PlayerController.instance.transform.position.y - this.transform.position.y < -10f
            || PlayerController.instance.transform.position.y - this.transform.position.y > 10f){
                return;
            }

        if(NetSystem.instance.IsCatchingACreature){
            return;
        }
        
        if(_timerBullets > 0){
            _timerBullets -= Time.deltaTime;
        }
        
        if(_timerBullets <= 0){
            _timerBullets = TIME_BETWEEN_BULLETS;
            //GameObject _bulletInstantiated = Instantiate(BulletObject, transform.);
            GameObject _bulletInstantiated = Instantiate(BulletObject, new Vector2(transform.position.x, transform.position.y), Quaternion.identity);
            _bulletInstantiated.GetComponent<Bullet>().SetVelocity(BULLET_VELOCITY);
            _bulletInstantiated.GetComponent<Bullet>().SetDirection(!LookingLeft());
        }

        // spawnedGamepadKey.transform.localPosition = new Vector3(-42f + (index * 11), 48f, spawnedGamepadKey.transform.position.z);
        // spawnedGamepadKey.GetComponent<Animator>().Play(key.ToString());
        
    }

    bool LookingLeft(){
        return this.gameObject.transform.localScale.x == -1;
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

                if(randomNumber >= 4){
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
}
