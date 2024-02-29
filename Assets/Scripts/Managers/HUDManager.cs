using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUDManager : MonoBehaviour
{    
    public static HUDManager instance;
    public GameObject KeyboardKey;
    public GameObject GamepadKey;

    void Awake(){
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /*  Index: index of key 
        Key:     
        1 - UP
        2 - DOWN
        3 - LEFT
        4 - RIGHT
        5 - A (GB) - X (Jump)
        6 - B (GB) - C (Attack)    
    */
    public void SpawnObject(int index, int key)
    {
        // Create an instance of the prefab and set its parent to the Canvas.
        GameObject spawnedObject = Instantiate(KeyboardKey, transform.parent);
        spawnedObject.transform.localPosition = new Vector3(-42f + (index * 11), 64f, spawnedObject.transform.position.z);
        spawnedObject.GetComponent<Animator>().Play(key.ToString());

        GameObject spawnedGamepadKey = Instantiate(GamepadKey, transform.parent);
        spawnedGamepadKey.transform.localPosition = new Vector3(-42f + (index * 11), 48f, spawnedGamepadKey.transform.position.z);
        spawnedGamepadKey.GetComponent<Animator>().Play(key.ToString());
        
        // You can also set the position, rotation, and scale of the spawned object if needed.
        //spawnedObject.transform.position = new Vector3(-42f, 64f, 0f);
        // spawnedObject.transform.rotation = Quaternion.Euler(x, y, z);
        // spawnedObject.transform.localScale = new Vector3(x, y, z);


        // Instantiate(KeyboardKey
        //     , new Vector3(-42f, 64f, 0f)
        //     , Quaternion.identity);  
    }
}
