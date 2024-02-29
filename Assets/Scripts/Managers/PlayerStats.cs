using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats instance;
    public int[] CapturedMonsters = new int[24];    
    public float PosX_Checkpoint;
    public float PosY_Checkpoint;
    public int CurrentLevelID;

    public bool Level1Checkpoint1;
    public bool Level1Checkpoint2;
    public bool Level2Checkpoint1;
    public bool Level2Checkpoint2;
    public bool Level2Checkpoint3;
    
    void Awake(){ 
        if(instance == null){
            DontDestroyOnLoad(this.gameObject); 
            instance = this;
        }        
    }
    // Start is called before the first frame update
    void Start()
    {
        for(int x = 0; x < 24; x++){
            CapturedMonsters[x] = 0;
        }

        CurrentLevelID = 1;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
