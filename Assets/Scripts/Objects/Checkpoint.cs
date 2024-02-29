using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public int ID;
    public int LevelID;
    public float PosX;
    public float PosY;
    // Start is called before the first frame update
    void Start()
    {
        switch (LevelID){
            case 1:
                switch(ID){
                    case 1:
                    if(PlayerStats.instance.Level1Checkpoint1)
                        Destroy(this.gameObject);
                    break;

                    case 2:
                    if(PlayerStats.instance.Level1Checkpoint2)
                        Destroy(this.gameObject);
                    break;
                }
            break;

            case 2:
                switch(ID){
                    case 1:
                    if(PlayerStats.instance.Level2Checkpoint1)
                        Destroy(this.gameObject);
                    break;

                    case 2:
                    if(PlayerStats.instance.Level2Checkpoint2)
                        Destroy(this.gameObject);
                    break;

                    case 3:
                    if(PlayerStats.instance.Level2Checkpoint3)
                        Destroy(this.gameObject);
                    break;
                }
            break;

            case 3:
            break;

            case 4:
            break;
        }
    }   

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D collision){
        if(collision.gameObject.tag == "Player"){
            PlayerStats.instance.PosX_Checkpoint = PosX;
            PlayerStats.instance.PosY_Checkpoint = PosY;
            FindObjectOfType<AudioManager>().Play("checkpoint");
            SaveCheckpointInStats();
            Destroy(this.gameObject);
        }
    }

    void SaveCheckpointInStats(){
        switch (LevelID){
            case 1:
                switch(ID){
                    case 1:
                        PlayerStats.instance.Level1Checkpoint1 = true;
                    break;

                    case 2:
                        PlayerStats.instance.Level1Checkpoint2 = true;
                    break;
                }
            break;

            case 2:
                switch(ID){
                    case 1:
                        PlayerStats.instance.Level2Checkpoint1 = true;
                    break;

                    case 2:
                        PlayerStats.instance.Level2Checkpoint2 = true;
                    break;

                    case 3:
                        PlayerStats.instance.Level2Checkpoint3 = true;
                    break;
                }
            break;

            case 3:
            break;

            case 4:
            break;
        }
    }
}
