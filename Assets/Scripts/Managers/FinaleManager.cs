using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FinaleManager : MonoBehaviour
{
    bool _introIsFinished;
    // Start is called before the first frame update
    void Start()
    {
        EffectsManager.instance.SetMaxBright();        
        EffectsManager.instance.TriggerGoFromBrightToNormal = true;
        StartCoroutine(TriggerLevelIntro());
        // StartCoroutine(GoFromNormalToBrightRoutine());
        // StartCoroutine(ChangeScene());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator TriggerLevelIntro(){
        yield return new WaitForSeconds(1f);           
        FindObjectOfType<AudioManager>().Play("story");
    }

    IEnumerator GoFromNormalToBrightRoutine(){
        yield return new WaitForSeconds(34f);
        EffectsManager.instance.TriggerGoFromNormalToBright = true;
            FindObjectOfType<AudioManager>().Stop("story");
    }    

    IEnumerator ChangeScene(){
        yield return new WaitForSeconds(36f);           
        SceneManager.LoadScene("TrainingLevel");
    }
}
