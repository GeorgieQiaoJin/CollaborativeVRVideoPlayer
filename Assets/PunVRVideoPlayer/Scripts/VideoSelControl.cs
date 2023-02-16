using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class VideoSelControl : MonoBehaviourPunCallbacks
{
    public string cur_videoname = "";
    public Text DebugLog;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SelVideo(string name)
    {
        this.photonView.RPC("SetVideo", RpcTarget.All, name);
    }

    public void SelMode(int mode_id)
    {
        if (StaticClass.cur_videoname == "")
        {
            return;
        }

        if (PhotonNetwork.IsMasterClient)
        {
            // 1 = sync, 2 = nsync, 3 = basic
            switch (mode_id)
            {
                case 1:
                    // Load sync
                    this.photonView.RPC("GoToSceen", RpcTarget.All, "1Photon2Room");
                    break;
                case 2:
                    this.photonView.RPC("GoToSceen", RpcTarget.All, "1Photon2RoomNSync");
                    break;
                case 3:
                    this.photonView.RPC("GoToSceen", RpcTarget.All, "1Photon2RoomBasic");
                    break;
                default:
                    DebugLog.text = "Unkown mode.";
                    break;
            }
        }
    }

    [PunRPC]
    public void GoToSceen(string scene)
    {
        PhotonNetwork.DestroyAll();
        SceneManager.LoadScene(scene);
    }

    [PunRPC]
    public void SetVideo(string name)
    {
        cur_videoname = name;
        StaticClass.cur_videoname = name;
        DebugLog.text = StaticClass.cur_videoname;
    }

    public void getMaster()
    {
        /*//transfer masterclient
        if (PhotonNetwork.LocalPlayer.ActorNumber == 2)
        {
            PhotonNetwork.SetMasterClient(PhotonNetwork.LocalPlayer);
        }*/
        PhotonNetwork.SetMasterClient(PhotonNetwork.LocalPlayer);
    }
}
