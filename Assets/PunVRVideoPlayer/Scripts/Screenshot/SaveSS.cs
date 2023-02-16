using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveSS : MonoBehaviour
{

    

    // Start is called before the first frame update
    void Start()
    {
        //Debug.Log("run into the SaveSS");

        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
