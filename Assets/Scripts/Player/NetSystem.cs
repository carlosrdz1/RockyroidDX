using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetSystem : MonoBehaviour
{
    public static NetSystem instance;
    public bool IsCatchingACreature;    
    public int CreatureLevel;
    public int CreatureID;
    public float TimerDuration;
    public int MinRangeNet;
    public int MaxRangeNet;
    int[] _sequence;
    bool _sequenceDefined;
    bool _readyCounterHasFinished;
    int _amountOfNodes; // Amount of keys in the sequence
    [SerializeField] int _currentKeyIndex; // The key that is active during the catching process
    GameObject[] _keys;
    GameObject[] _gamepadKeys;
    public Animator TimerAnimation;
    public Animator CatchingCreatureBar;
    public Animator[] CreatureLifes;
    bool _gotKeys;
    [SerializeField] bool _flagControl;
    [SerializeField] int Life = 3;
    public GameObject CaptureCapsule;
    public Animator CaptureText;
    public Animator TheCreatureText;
    public Animator WellDoneText;
    bool _canUpdateTimer = true;

    /*
    1 - UP
    2 - DOWN
    3 - LEFT
    4 - RIGHT
    5 - A (GB) - X (Jump)
    6 - B (GB) - C (Attack)    
    */

    void Awake(){
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        //NetIsActive = true;
        //KeyboardKeys[0].Play("3");
    }

    // Update is called once per frame
    void Update()
    {
        if(!_flagControl)
            Controls();

        if(IsCatchingACreature){
            CatchingCreatureBar.Play("Active");
            if(!_sequenceDefined){
                DefineSequence();
                StartCoroutine(ReadyAnimation());
                CaptureText.Play("CaptureActive");
                TheCreatureText.Play("TheCreatureActive");

               // CreateObjects();
                //AllowVariablesReset();
            }
        }

        if(_readyCounterHasFinished && _sequenceDefined && _canUpdateTimer){
            TimerDuration -= Time.deltaTime;
            TimerAnimation.Play((Mathf.Round(TimerDuration) + 1).ToString());
            //Debug.Log("Countdown: " + Mathf.Round(TimerDuration));
        }

        if(TimerDuration > 0 && _readyCounterHasFinished){
            UpdateLife();
            CatchProcess();
        }

        if(_sequenceDefined && TimerDuration <= 0){
            StartCoroutine(FinishCatchingProcess(false));
        }
    }

    void UpdateLife(){
        switch(Life){
            case 3:
                CreatureLifes[0].Play("Active");
                CreatureLifes[1].Play("Active");
                CreatureLifes[2].Play("Active");        
            break;

            case 2:
                CreatureLifes[0].Play("Active");
                CreatureLifes[1].Play("Active");
                CreatureLifes[2].Play("Idle");      
            break;
            
            case 1:
                CreatureLifes[0].Play("Active");
                CreatureLifes[1].Play("Idle");
                CreatureLifes[2].Play("Idle");  
            break;

            case 0:
                CreatureLifes[0].Play("Idle");
                CreatureLifes[1].Play("Idle");
                CreatureLifes[2].Play("Idle");  
            break;
        }
    }

    void Controls(){
        if(PlayerController.instance.IsUpPressed && PlayerController.instance.AllowUp){
            PlayerController.instance.AllowUp = false;    
        }

        if(PlayerController.instance.IsDownPressed && PlayerController.instance.AllowDown){
            PlayerController.instance.AllowDown = false;    
        }

        if(PlayerController.instance.IsLeftPressed && PlayerController.instance.AllowLeft){
            PlayerController.instance.AllowLeft = false;    
        }

        if(PlayerController.instance.IsRightPressed && PlayerController.instance.AllowRight){
            PlayerController.instance.AllowRight = false;    
        }
    }    

    void AllowVariablesReset(){
        PlayerController.instance.AllowUp = false;
        PlayerController.instance.AllowDown = false;
        PlayerController.instance.AllowLeft = false;
        PlayerController.instance.AllowRight = false;
        PlayerController.instance.AllowJump = false;
        PlayerController.instance.AllowFireWeapon = false;
    }

    IEnumerator FinishCatchingProcess(bool correct){
        yield return new WaitForSeconds(1.5f);
        WellDoneText.Play("Idle");
        _flagControl = false;
        IsCatchingACreature = false;        
        CreatureLevel = 0;
        TimerDuration = 0;
        //NetIsActive = false;
        

        _sequenceDefined = false;
        _readyCounterHasFinished = false;
        _gotKeys = false;
        _currentKeyIndex = 0;

        CatchingCreatureBar.Play("Idle");
        TimerAnimation.Play("Empty");

        CreatureLifes[0].Play("Idle");
        CreatureLifes[1].Play("Idle");
        CreatureLifes[2].Play("Idle");

        CaptureText.Play("CaptureIdle");
        TheCreatureText.Play("TheCreatureIdle");
        
        foreach(GameObject key in _keys)
            GameObject.Destroy(key);

        foreach(GameObject gpKey in _gamepadKeys)
            GameObject.Destroy(gpKey);

        Life = 3;

        if(correct){
            GameObject.FindWithTag("CatchMachine").GetComponent<Animator>().Play("Completed");
            RegisterCreature();
            StartCoroutine(DestroyCompletedCapsule());
        } else {
            Destroy(GameObject.FindWithTag("CatchMachine"));
        }

        _canUpdateTimer = true;
    }

    void RegisterCreature(){
        PlayerStats.instance.CapturedMonsters[CreatureID - 1] = 1;
    }

    IEnumerator DestroyCompletedCapsule(){
        yield return new WaitForSeconds(1f);
        Destroy(GameObject.FindWithTag("CatchMachine"));
    }

    IEnumerator ReadyAnimation(){
        yield return new WaitForSeconds(3f);
        _readyCounterHasFinished = true;
    }
    

    void DefineSequence(){
       // if(CreatureLevel > 2){
            _currentKeyIndex = 1;
            _amountOfNodes = Random.Range(MinRangeNet, MaxRangeNet);
            _sequence = new int[_amountOfNodes];

            for(int x = 0; x < _amountOfNodes; x++){
                _sequence[x] = Random.Range(1, 7);
                HUDManager.instance.SpawnObject(x, _sequence[x]);
            }
       // }
        
        _sequenceDefined = true;
    }

    void CatchProcess(){
        if(!_gotKeys){
            _gotKeys = true;
            _keys = GameObject.FindGameObjectsWithTag("Key");
            _gamepadKeys = GameObject.FindGameObjectsWithTag("GamepadKey");
        }

        if(_currentKeyIndex <= _amountOfNodes){
            switch(_sequence[_currentKeyIndex - 1]){
                case 1:
                    if(PlayerController.instance.IsUpPressed && PlayerController.instance.AllowUp){
                        PlayerController.instance.AllowUp = false;    
                        CorrectKeyProcess();
                    }   
                        if(PlayerController.instance.IsDownPressed && PlayerController.instance.AllowDown){
                            PlayerController.instance.AllowDown = false;    
                            IncorrectKeyProcess();
                        }

                        if(PlayerController.instance.IsLeftPressed && PlayerController.instance.AllowLeft){
                            PlayerController.instance.AllowLeft = false;    
                            IncorrectKeyProcess();
                        }   

                        if(PlayerController.instance.IsRightPressed && PlayerController.instance.AllowRight){
                            PlayerController.instance.AllowRight = false;    
                            IncorrectKeyProcess();
                        }   

                        if(PlayerController.instance.IsJumpPressed && PlayerController.instance.AllowJump){
                            PlayerController.instance.AllowJump = false;    
                            IncorrectKeyProcess();
                        }        

                        if(PlayerController.instance.IsFirePressed && PlayerController.instance.AllowFireWeapon){
                            PlayerController.instance.AllowFireWeapon = false;    
                            IncorrectKeyProcess();
                        }        
                break;

                case 2:
                    if(PlayerController.instance.IsDownPressed && PlayerController.instance.AllowDown){
                        PlayerController.instance.AllowDown = false;    
                        CorrectKeyProcess();
                    }   
                        if(PlayerController.instance.IsUpPressed && PlayerController.instance.AllowUp){
                            PlayerController.instance.AllowUp = false;    
                            IncorrectKeyProcess();
                        }

                        if(PlayerController.instance.IsLeftPressed && PlayerController.instance.AllowLeft){
                            PlayerController.instance.AllowLeft = false;    
                            IncorrectKeyProcess();
                        }   

                        if(PlayerController.instance.IsRightPressed && PlayerController.instance.AllowRight){
                            PlayerController.instance.AllowRight = false;    
                            IncorrectKeyProcess();
                        }   

                        if(PlayerController.instance.IsJumpPressed && PlayerController.instance.AllowJump){
                            PlayerController.instance.AllowJump = false;    
                            IncorrectKeyProcess();
                        }        

                        if(PlayerController.instance.IsFirePressed && PlayerController.instance.AllowFireWeapon){
                            PlayerController.instance.AllowFireWeapon = false;    
                            IncorrectKeyProcess();
                        }        
                break;

                case 3:
                    if(PlayerController.instance.IsLeftPressed && PlayerController.instance.AllowLeft){
                        PlayerController.instance.AllowLeft = false;    
                        CorrectKeyProcess();
                    }   
                        if(PlayerController.instance.IsUpPressed && PlayerController.instance.AllowUp){
                            PlayerController.instance.AllowUp = false;    
                            IncorrectKeyProcess();
                        }

                        if(PlayerController.instance.IsDownPressed && PlayerController.instance.AllowDown){
                            PlayerController.instance.AllowDown = false;    
                            IncorrectKeyProcess();
                        }

                        if(PlayerController.instance.IsRightPressed && PlayerController.instance.AllowRight){
                            PlayerController.instance.AllowRight = false;    
                            IncorrectKeyProcess();
                        }   

                        if(PlayerController.instance.IsJumpPressed && PlayerController.instance.AllowJump){
                            PlayerController.instance.AllowJump = false;    
                            IncorrectKeyProcess();
                        }        

                        if(PlayerController.instance.IsFirePressed && PlayerController.instance.AllowFireWeapon){
                            PlayerController.instance.AllowFireWeapon = false;    
                            IncorrectKeyProcess();
                        }        
                break;

                case 4:
                    if(PlayerController.instance.IsRightPressed && PlayerController.instance.AllowRight){
                        PlayerController.instance.AllowRight = false;    
                        CorrectKeyProcess();
                    }   
                        if(PlayerController.instance.IsUpPressed && PlayerController.instance.AllowUp){
                            PlayerController.instance.AllowUp = false;    
                            IncorrectKeyProcess();
                        }

                        if(PlayerController.instance.IsDownPressed && PlayerController.instance.AllowDown){
                            PlayerController.instance.AllowDown = false;    
                            IncorrectKeyProcess();
                        }

                        if(PlayerController.instance.IsLeftPressed && PlayerController.instance.AllowLeft){
                            PlayerController.instance.AllowLeft = false;    
                            IncorrectKeyProcess();
                        }   

                        if(PlayerController.instance.IsJumpPressed && PlayerController.instance.AllowJump){
                            PlayerController.instance.AllowJump = false;    
                            IncorrectKeyProcess();
                        }        

                        if(PlayerController.instance.IsFirePressed && PlayerController.instance.AllowFireWeapon){
                            PlayerController.instance.AllowFireWeapon = false;    
                            IncorrectKeyProcess();
                        }       
                break;

                case 5:
                    if(PlayerController.instance.IsJumpPressed && PlayerController.instance.AllowJump){
                        PlayerController.instance.AllowJump = false;    
                        CorrectKeyProcess();
                    }          
                        if(PlayerController.instance.IsUpPressed && PlayerController.instance.AllowUp){
                            PlayerController.instance.AllowUp = false;    
                            IncorrectKeyProcess();
                        }

                        if(PlayerController.instance.IsDownPressed && PlayerController.instance.AllowDown){
                            PlayerController.instance.AllowDown = false;    
                            IncorrectKeyProcess();
                        }

                        if(PlayerController.instance.IsLeftPressed && PlayerController.instance.AllowLeft){
                            PlayerController.instance.AllowLeft = false;    
                            IncorrectKeyProcess();
                        }   

                        if(PlayerController.instance.IsRightPressed && PlayerController.instance.AllowRight){
                            PlayerController.instance.AllowRight = false;    
                            IncorrectKeyProcess();
                        } 

                        if(PlayerController.instance.IsFirePressed && PlayerController.instance.AllowFireWeapon){
                            PlayerController.instance.AllowFireWeapon = false;    
                            IncorrectKeyProcess();
                        }             
                break;

                case 6:
                    if(PlayerController.instance.IsFirePressed && PlayerController.instance.AllowFireWeapon){
                        PlayerController.instance.AllowFireWeapon = false;    
                        CorrectKeyProcess();
                    }      
                        if(PlayerController.instance.IsUpPressed && PlayerController.instance.AllowUp){
                            PlayerController.instance.AllowUp = false;    
                            IncorrectKeyProcess();
                        }

                        if(PlayerController.instance.IsDownPressed && PlayerController.instance.AllowDown){
                            PlayerController.instance.AllowDown = false;    
                            IncorrectKeyProcess();
                        }

                        if(PlayerController.instance.IsLeftPressed && PlayerController.instance.AllowLeft){
                            PlayerController.instance.AllowLeft = false;    
                            IncorrectKeyProcess();
                        }   

                        if(PlayerController.instance.IsRightPressed && PlayerController.instance.AllowRight){
                            PlayerController.instance.AllowRight = false;    
                            IncorrectKeyProcess();
                        } 

                        if(PlayerController.instance.IsJumpPressed && PlayerController.instance.AllowJump){
                            PlayerController.instance.AllowJump = false;    
                            IncorrectKeyProcess();
                        }        
                break;
            }

            if(TimerDuration > 0 )
                _flagControl = true;
            
        }
    }

    void CorrectKeyProcess(){
        _keys[_currentKeyIndex - 1].GetComponent<Animator>().Play("0");
        _gamepadKeys[_currentKeyIndex - 1].GetComponent<Animator>().Play("0");
        _currentKeyIndex++; 

        foreach(GameObject key in _keys){            
            key.transform.localPosition = new Vector3(key.transform.localPosition.x - 11f
                    , key.transform.localPosition.y
                    , key.transform.localPosition.z);
        }

        foreach(GameObject gpKey in _gamepadKeys){            
            gpKey.transform.localPosition = new Vector3(gpKey.transform.localPosition.x - 11f
                    , gpKey.transform.localPosition.y
                    , gpKey.transform.localPosition.z);
        }

        FindObjectOfType<AudioManager>().Play("correctKey");

        if(_currentKeyIndex > _amountOfNodes){
            WellDoneText.Play("Active");      
            _canUpdateTimer = false;      
            StartCoroutine(FinishCatchingProcess(true));
        }
    }

    void IncorrectKeyProcess(){
        FindObjectOfType<AudioManager>().Play("incorrectKey");
        Life--;

        if(Life == 0){
            StartCoroutine(FinishCatchingProcess(false));
        }
    }
}
