using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DisplayInScene : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        string name = SceneManager.GetActiveScene().name;
        int index = SceneManager.GetActiveScene().buildIndex;

        if (index == 1)
        {//index == 1 under discussion room
            
        }
        else
        { //under video room
            this.transform.localPosition = new Vector3(0, 0, -100);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
