using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using Photon.Realtime;
using Photon.Pun;

public class Async_CamPlayer_Sync : MonoBehaviourPun
{
    public VideoPlayer mVideoPlayer; // attached to saved cam, each user generated one for all user
    public VideoPlayer mainVideoPlayer; // in the main scene, each user have their own
    public GameObject mainCanvas;
    // Start is called before the first frame update
    void Start()
    {
        mainVideoPlayer = GameObject.Find("360_videoplayer").GetComponent<VideoPlayer>();
        mainCanvas = GameObject.Find("Canvas");
    }

    // Update is called once per frame
    void Update()
    {

    }


    public void UpdatePlayingStatus(bool play)
    {
        if (this.photonView.IsMine)
        {
            this.photonView.RPC("RPCUpdatePlayingStatus", RpcTarget.All, play);
        }
    }

    public void UpdatePlayingFrame(int frame)
    {
        this.photonView.RPC("RPCUpdatePlayingFrame", RpcTarget.All, (int)frame, PhotonNetwork.LocalPlayer.ActorNumber);
    }

    [PunRPC]
    public void RPCUpdatePlayingStatus(bool play)
    {
        if (play) 
        { 
            mVideoPlayer.Play();
        }
        else 
        { 
            mVideoPlayer.Pause();
        }
        if (!this.photonView.IsMine)
            mainCanvas.SendMessage("UpdateFollowPlayStatus");
    }

    [PunRPC]
    public void RPCUpdatePlayingFrame(int frame, int from_userID)
    {
        mVideoPlayer.frame = frame;
        object[] tempStorage = new object[2];
        tempStorage[0] = frame;
        tempStorage[1] = from_userID;
        if (!this.photonView.IsMine)
            mainCanvas.SendMessage("UpdateFollowPlayFrame", tempStorage);
    }
}
