using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class TitleManager : MonoBehaviour
{
    public Animator Selection;
    public bool StartPressed = false;
    public bool IsUpPressed = false;
    public bool IsDownPressed = false;
    bool _isAllowedToStart = true;
    bool _isAllowedForDown = true;
    bool _isAllowedForUp = true;
    bool _optionHasBeenSelected;
    bool _introIsFinished;
    [SerializeField] int _optionSelected = 1;

    // Start is called before the first frame update
    void Start()
    {
        EffectsManager.instance.SetMaxBright();        
        EffectsManager.instance.TriggerGoFromBrightToNormal = true;
        StartCoroutine(TriggerLevelIntro());
    }

    // Update is called once per frame
    void Update()
    {
        if(!_introIsFinished) return;

        Controls();
        Animations();
    }

    void Controls(){
        if(_optionHasBeenSelected) return;

        if(IsUpPressed && _isAllowedForUp){
            _isAllowedForUp = false;
            _optionSelected = _optionSelected == 1 ? 2 : 1;
            FindObjectOfType<AudioManager>().Play("moveCursor");
        }

        if(IsDownPressed && _isAllowedForDown){
            _isAllowedForDown = false;
            _optionSelected = _optionSelected == 1 ? 2 : 1;
            FindObjectOfType<AudioManager>().Play("moveCursor");
        }        

        if(StartPressed && _isAllowedToStart){
            _isAllowedToStart = false;
            _optionHasBeenSelected = true;            
            FindObjectOfType<AudioManager>().Play("selectOption");
            FindObjectOfType<AudioManager>().Stop("level");
            if(_optionSelected == 1){
                Selection.Play("OptionSelected");
                StartCoroutine(GoFromNormalToBrightRoutine());
                StartCoroutine(LoadNextScene());
            } else {
                Application.Quit();
            }
        }
    }

    IEnumerator GoFromNormalToBrightRoutine(){
        yield return new WaitForSeconds(2.3f);
        EffectsManager.instance.TriggerGoFromNormalToBright = true;
    }    

    IEnumerator LoadNextScene(){
        yield return new WaitForSeconds(3f);
        SceneManager.LoadScene("IntroStory");
    }

    void Animations(){
        if(_optionHasBeenSelected) return;

        switch(_optionSelected){
            case 1:
                Selection.Play("Option1");
            break;

            case 2:
                Selection.Play("Option2");
            break;
        }
    }

    IEnumerator TriggerLevelIntro(){
        yield return new WaitForSeconds(1f);   
        _introIsFinished = true;
        FindObjectOfType<AudioManager>().Play("level");
    }

    public void UpPressed(InputAction.CallbackContext context){
        if(context.performed){
            IsUpPressed = true;
        }
        
        if(context.canceled){    
            IsUpPressed = false;
            _isAllowedForUp = true;
        }
    }

    public void DownPressed(InputAction.CallbackContext context){
        if(context.performed){
            IsDownPressed = true;
        }
        
        if(context.canceled){    
            IsDownPressed = false;
            _isAllowedForDown = true;
        }
    }

    public void Start(InputAction.CallbackContext context){
        if(context.performed){
            StartPressed = true;
        }
        
        if(context.canceled){    
            StartPressed = false;
            _isAllowedToStart = true;
        }
    }
}
