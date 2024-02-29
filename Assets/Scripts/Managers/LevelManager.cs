using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;
    public bool IntroIsFinished;
    public bool RestartLevel;
    public int AmountOfBoomerangs;
    public int AmountOfCoins;
    public Animator Hundreds;
    public Animator Tens;
    public Animator Units;
    bool _triggerNewScene;

    void Awake(){
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        EffectsManager.instance.SetMaxBright();
        EffectsManager.instance.TriggerGoFromBrightToNormal = true;
        AmountOfCoins = 0;
        StartCoroutine(TriggerLevelIntro());
        CoinsRoutine();

        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Spawner");
        foreach(GameObject enemy in enemies){
            enemy.GetComponent<SpriteRenderer>().enabled = false;
        }

        if(PlayerStats.instance.PosX_Checkpoint == 0 && PlayerStats.instance.PosY_Checkpoint == 0){

        } else{
            PlayerController.instance.transform.position = new Vector2(PlayerStats.instance.PosX_Checkpoint,  PlayerStats.instance.PosY_Checkpoint);            
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(RestartLevel){
            RestartLevel = false;
            StartCoroutine(GoFromNormalToBrightRoutine());
            StartCoroutine(RestartLevelRoutine());
        }           

        if(PlayerController.instance.CurrentLevelFinished && !_triggerNewScene){
            _triggerNewScene = true;
            PlayerStats.instance.PosX_Checkpoint = 0;
            PlayerStats.instance.PosY_Checkpoint = 0;
            StartCoroutine(GoFromNormalToBrightRoutine());
            StartCoroutine(ChangeScene());
        }
    }

    public void CoinsRoutine(){
        Hundreds.Play(GetDigitsArray(AmountOfCoins)[0].ToString());
        Tens.Play(GetDigitsArray(AmountOfCoins)[1].ToString());
        Units.Play(GetDigitsArray(AmountOfCoins)[2].ToString());
    }

    IEnumerator TriggerLevelIntro(){
        yield return new WaitForSeconds(1f);
        IntroIsFinished = true;        
        FindObjectOfType<AudioManager>().Play("level");
    }

    IEnumerator GoFromNormalToBrightRoutine(){
        yield return new WaitForSeconds(3f);
        EffectsManager.instance.TriggerGoFromNormalToBright = true;
    }    

    IEnumerator RestartLevelRoutine(){
        yield return new WaitForSeconds(3.5f);
        SceneManager.LoadScene("Level" + PlayerStats.instance.CurrentLevelID.ToString());        
    }

    IEnumerator ChangeScene(){
        yield return new WaitForSeconds(3.5f); 
        if(PlayerStats.instance.CurrentLevelID <= 3){
            SceneManager.LoadScene("Overworld");
        } else {
            SceneManager.LoadScene("Finale");
        }
    }

    int[] GetDigitsArray(int number)
    {
        int[] digits = new int[3];

        // Ensure the input is between 0 and 999.
        number = Mathf.Clamp(number, 0, 999);

        // Extract hundreds, tens, and units.
        digits[0] = number / 100;     // Hundreds place
        digits[1] = (number / 10) % 10; // Tens place
        digits[2] = number % 10;      // Units place

        return digits;
    }
}
