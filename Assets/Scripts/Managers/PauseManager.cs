using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.InputSystem;


public class PauseManager : MonoBehaviour
{    
    public UnityEvent GamePaused;
    public UnityEvent GameResumed;
    public static PauseManager instance;
    public bool PauseButtonPressed = false;
    public bool IsSelectOptionPressed = false;
    bool _allowSelectOption = true;
    bool _isPaused;
    int _selectedOption;
    const int AMOUNT_OF_CREATURES = 24;

    [Header("Controls")]
    [SerializeField] bool _isPauseUpPressed;
    [SerializeField] bool _isPauseDownPressed;
    [SerializeField] bool _isPauseLeftPressed;
    [SerializeField] bool _isPauseRightPressed;
    [SerializeField] bool _allowPauseUp;
    [SerializeField] bool _allowPauseDown;
    [SerializeField] bool _allowPauseLeft;
    [SerializeField] bool _allowPauseRight;

    [Header("Pause")]
    public Animator PauseMenuSquare;
    public Animator PauseSelectedOption;
    public Animator PauseOptions;

    [Header("Creatures")]
    public Animator CreaturesLayout;
    public Animator CreaturesSelectedOption;
    public Animator CreaturesBG1;
    public Animator CreaturesBG2;
    bool _creaturesMenuSelected;
    int _creatureSelectedID;

    [Header("MiniCreatures")]
    public Animator[] MiniCreatures;

    [Header("CreaturePreviewName")]
    public Animator CreaturePreviewAnimation;
    public Animator CreatureNameAnimation;

    public bool IsPaused {get{return _isPaused;}}

    void Awake(){
        instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        HidePauseMenu();
        HideCreaturesMenu();
        _selectedOption = 1;
        _allowPauseDown = true;
        _allowPauseUp = true;
        _allowPauseLeft = true;
        _allowPauseRight = true;
        _creatureSelectedID = 1;
    }

    // Update is called once per frame
    void Update()
    {
        if(NetSystem.instance.IsCatchingACreature) return;
        if(PlayerController.instance.IsDead) return;
        if(PlayerController.instance.IsInvincible) return;

        if(PauseButtonPressed){
            PauseButtonPressed = false;
            _isPaused = !_isPaused;

            if(_isPaused)
                FindObjectOfType<AudioManager>().Play("pause");
        }

        if(_isPaused){
            PauseMenuSquare.Play("Active");
            //PauseSelectedOption.Play("Continue");
            PauseOptions.Play("Active");
            Pause();            
            GetControls();
            AnimationsForOption();
        } else {
            UnpauseGame();
        }        

    }

    // Method that can be use anywhere to unpause the game. It's NOT Unpause()
    void UnpauseGame(){
        _creaturesMenuSelected = false;
        _selectedOption = 1;
        _creatureSelectedID = 1;
        HidePauseMenu();
        HideCreaturesMenu();
        Unpause();
    }

    void HidePauseMenu(){
        PauseMenuSquare.Play("Idle");
        PauseSelectedOption.Play("Idle");
        PauseOptions.Play("Idle");
    }

    void HideCreaturesMenu(){
        CreaturesLayout.Play("Idle");
        CreaturesSelectedOption.Play("Idle");
        CreaturesBG1.Play("BG1Idle");
        CreaturesBG2.Play("BG2Idle");

        for(int x = 0; x < AMOUNT_OF_CREATURES; x++){
            MiniCreatures[x].Play((x+1) + "Idle");
        }

        CreaturePreviewAnimation.Play("Idle");
        CreatureNameAnimation.Play("Idle");
    }

    void AnimationsForOption(){
        switch(_selectedOption){
            case 1:
                PauseSelectedOption.Play("Continue");
            break;

            case 2:
                PauseSelectedOption.Play("Creatures");
            break;

            case 3:
                PauseSelectedOption.Play("ExitLevel");
            break;

            case 4:
                PauseSelectedOption.Play("ExitGame");
            break;
        }
    }

    void GetControls(){
        if(!_creaturesMenuSelected){
            if(_isPauseUpPressed && _allowPauseUp){
                if(_selectedOption > 1){
                    FindObjectOfType<AudioManager>().Play("moveCursor");
                    _allowPauseUp = false;
                    _selectedOption--;
                }
            }

            if(_isPauseDownPressed && _allowPauseDown){
                if(_selectedOption < 4){
                    FindObjectOfType<AudioManager>().Play("moveCursor");
                    _allowPauseDown = false;
                    _selectedOption++;
                }
            }
        }

        // First pause menu
        if(IsSelectOptionPressed && _allowSelectOption && !_creaturesMenuSelected){
            _allowSelectOption = false;

            switch(_selectedOption){
                case 1:
                    _isPaused = false;
                    FindObjectOfType<AudioManager>().Play("selectOption");
                    UnpauseGame();
                break;

                case 2:
                    FindObjectOfType<AudioManager>().Play("selectOption");
                    _creaturesMenuSelected = true;
                    CreaturesLayout.Play("Active");
                    CreaturesBG1.Play("BG1Active");
                    CreaturesBG2.Play("BG2Active");
                    DisplayMiniCreatures();
                    HidePauseMenu();
                break;

                case 3:
                    
                break;

                case 4:
                    FindObjectOfType<AudioManager>().Play("selectOption");
                    Application.Quit();
                break;
            }
        }

        // Control creatures menu
        if(_creaturesMenuSelected){
            if(_isPauseRightPressed && _allowPauseRight){
                if(_creatureSelectedID < 24){
                    FindObjectOfType<AudioManager>().Play("moveCursor");
                    _allowPauseRight = false;
                    _creatureSelectedID++;
                } 
            }

            if(_isPauseLeftPressed && _allowPauseLeft){
                if(_creatureSelectedID > 1){
                    FindObjectOfType<AudioManager>().Play("moveCursor");
                    _allowPauseLeft = false;
                    _creatureSelectedID--;
                } 
            }

            if(_isPauseDownPressed && _allowPauseDown){
                if(_creatureSelectedID < 17){
                    FindObjectOfType<AudioManager>().Play("moveCursor");
                    _allowPauseDown = false;
                    _creatureSelectedID+= 8;
                }
            }

            if(_isPauseUpPressed && _allowPauseUp){
                if(_creatureSelectedID > 8){
                    FindObjectOfType<AudioManager>().Play("moveCursor");
                    _allowPauseUp = false;
                    _creatureSelectedID-= 8;
                }
            }

            CreaturesSelectedOption.Play(_creatureSelectedID.ToString());
            SetCreatureProperties();
        }
    }

    void SetCreatureProperties(){
        if(PlayerStats.instance.CapturedMonsters[_creatureSelectedID - 1] == 1){
            CreatureNameAnimation.Play(_creatureSelectedID.ToString());
            CreaturePreviewAnimation.Play(_creatureSelectedID.ToString());
        }
        else{        
            CreaturePreviewAnimation.Play("Idle");
            CreatureNameAnimation.Play("Idle");
        }
    }

    void DisplayMiniCreatures(){
        for(int x = 0; x < AMOUNT_OF_CREATURES; x++){
            if(PlayerStats.instance.CapturedMonsters[x] == 1){
                MiniCreatures[x].Play((x+1) + "Active");
            }
        }
    }

    void Pause(){
        GamePaused.Invoke();
        Time.timeScale = 0;
    }

    void Unpause(){
        GameResumed.Invoke();
        Time.timeScale = 1;
    }

    /*============================================================================
               _____          __  __ ______ _____        _____  
             / ____|   /\   |  \/  |  ____|  __ \ /\   |  __ \ 
            | |  __   /  \  | \  / | |__  | |__) /  \  | |  | |
            | | |_ | / /\ \ | |\/| |  __| |  ___/ /\ \ | |  | |
            | |__| |/ ____ \| |  | | |____| |  / ____ \| |__| |
            \_____/_/    \_\_|  |_|______|_| /_/    \_\_____/                                                                              
    
    ============================================================================*/

    public void SelectOptionButton(InputAction.CallbackContext context){
        if(context.performed){
            IsSelectOptionPressed = true;
        }
        
        if(context.canceled){    
            IsSelectOptionPressed = false;
            _allowSelectOption = true;
        }
    }

    public void Pause(InputAction.CallbackContext context){
        if(context.performed){
            PauseButtonPressed = true;
        }
        
        if(context.canceled){
            PauseButtonPressed = false;            
        }
    }

    public void PauseUp(InputAction.CallbackContext context){
        if(context.performed){
            _isPauseUpPressed = true;
        }
        
        if(context.canceled){    
            _isPauseUpPressed = false;
            _allowPauseUp = true;
        }
    }

    public void PauseDown(InputAction.CallbackContext context){    
        if(context.performed)
            _isPauseDownPressed = true;
        
        if(context.canceled){
            _isPauseDownPressed = false;
            _allowPauseDown = true;
        }
    }

    public void PauseLeft(InputAction.CallbackContext context){
        if(context.performed){
            _isPauseLeftPressed = true;
        }
        
        if(context.canceled){    
            _isPauseLeftPressed = false;
            _allowPauseLeft = true;
        }
    }

    public void PauseRight(InputAction.CallbackContext context){    
        if(context.performed)
            _isPauseRightPressed = true;
        
        if(context.canceled){
            _isPauseRightPressed = false;
            _allowPauseRight = true;
        }
    }
}
