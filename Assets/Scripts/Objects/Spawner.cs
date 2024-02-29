using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject Enemy;
    private bool _destroyEnemiesTriggered = false;
    public bool HasBeenCaptured;
    // Start is called before the first frame update
    void Start()
    {
        InstantiateEnemy();
    }

    void InstantiateEnemy(){
        var newEnemy = Instantiate(Enemy
                        , new Vector3(this.transform.position.x, this.transform.position.y, this.transform.position.z)
                        , Quaternion.identity);  

        newEnemy.transform.parent = gameObject.transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (this.transform.childCount == 0 && !HasBeenCaptured){
            if(PlayerController.instance.transform.position.x - this.transform.position.x > 10f
            || PlayerController.instance.transform.position.x - this.transform.position.x < -10f
            || PlayerController.instance.transform.position.y - this.transform.position.y < -10f
            || PlayerController.instance.transform.position.y - this.transform.position.y > 10f){
                InstantiateEnemy();
            }
        }

        // if(Pla.instance.IsDead){
        //     if (!_destroyEnemiesTriggered) 
        //     {   
        //         _destroyEnemiesTriggered = true;
        //         StartCoroutine(DestroyEnemies());
        //     }
        // }
    }

    // IEnumerator DestroyEnemies(){
    //     yield return new WaitForSeconds(4f);

    //     foreach (Transform child in transform) {
    //         GameObject.Destroy(child.gameObject); 
    //     }

    //     _destroyEnemiesTriggered = false;
    // }
}
