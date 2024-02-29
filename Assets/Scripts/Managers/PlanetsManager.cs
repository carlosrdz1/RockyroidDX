using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlanetsManager : MonoBehaviour
{
    public Animator _animations;
    // Start is called before the first frame update
    void Start()
    {
        EffectsManager.instance.SetMaxBright();        
        EffectsManager.instance.TriggerGoFromBrightToNormal = true;
        StartCoroutine(TriggerLevelIntro());

        switch(PlayerStats.instance.CurrentLevelID){
            case 1:
                _animations.Play("TrainingToKepler");
            break;

            case 2:
                _animations.Play("KeplerToTrappist");
            break;

            case 3:
                _animations.Play("TrappistToGliese");
            break;
        }

        StartCoroutine(GoFromNormalToBrightRoutine());
        StartCoroutine(ChangeScene());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator GoFromNormalToBrightRoutine(){
        yield return new WaitForSeconds(6f);
        EffectsManager.instance.TriggerGoFromNormalToBright = true;
            FindObjectOfType<AudioManager>().Stop("overworld");
    }    


    IEnumerator TriggerLevelIntro(){
        yield return new WaitForSeconds(1f);           
        FindObjectOfType<AudioManager>().Play("overworld");
    }

    IEnumerator ChangeScene(){
        yield return new WaitForSeconds(7f);           
        SceneManager.LoadScene("Level" + PlayerStats.instance.CurrentLevelID.ToString());
    }
}
