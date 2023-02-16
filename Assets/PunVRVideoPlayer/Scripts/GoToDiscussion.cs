using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

using UnityEngine.SceneManagement;

public class GoToDiscussion : MonoBehaviourPunCallbacks
{
    public string SceneName2;
    public Text DebugLog;
    //public ScreenShotStore screenshotstore;
    //[SerializeField] GameObject gameObject;
    public string GoToScene2;
    public GameObject image_parent_player1;
    public GameObject image_parent_player2;
    public GameObject image_parent_player3;
    public GameObject image_parent_player4;
    public GameObject image_parent_player5;
    private static bool isNoDestroyHandler = true;

    // Start is called before the first frame update
    void Start()
    {
        /*if (isNoDestroyHandler)
        {
            isNoDestroyHandler = false;
            DontDestroyOnLoad(image_parent_player1);
            DontDestroyOnLoad(image_parent_player2);
            DontDestroyOnLoad(image_parent_player3);
            DontDestroyOnLoad(image_parent_player4);
            DontDestroyOnLoad(image_parent_player5);
        }*/
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GoToScene()
    {
        //Save and display all notes and screenshots

        DebugLog.text = "Go To Scene: " + SceneName2;
        //this.photonView.RPC("RPC_GoToScene", RpcTarget.All, SceneName);

        this.photonView.RPC("GoToSceen", RpcTarget.All, GoToScene2);
        
    }


    [PunRPC]
    public void GoToSceen(string scene)
    {/*
        DontDestroyOnLoad(image_parent_player1);
        DontDestroyOnLoad(image_parent_player2);
        DontDestroyOnLoad(image_parent_player3);
        DontDestroyOnLoad(image_parent_player4);
        DontDestroyOnLoad(image_parent_player5);
        */
        SceneManager.LoadScene(GoToScene2);
    }
}

    
