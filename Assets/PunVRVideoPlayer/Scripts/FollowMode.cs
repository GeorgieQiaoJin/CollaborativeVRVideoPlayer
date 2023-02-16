using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;
using Photon.Pun;
using System.IO;
using Photon.Realtime;

public class FollowMode : MonoBehaviourPun
{
    public GameObject BTN_Play;
    public GameObject ProgressBar;
    public VideoPlayer mVideoPlayer;
    public GameObject fSaveCamera;
    public GameObject frSaveCamera;
    public GameObject OVRCamera;
    public VideoPlayer fVideoPlayer;
    public bool Mod_Follow;
    public bool isFollowed;
    public int followedby;
    public int current_follow;

    public Button[] BTN_Follows;

    public Text DebugLog;

    // Start is called before the first frame update
    void Start()
    {
        Mod_Follow = false;
        current_follow = -1;
        followedby = 0;
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void OnFollowButtonClicked(int playerID)
    {
        if (!Mod_Follow)
        {
            // if currently not following other user:
            Mod_Follow = true;
            current_follow = playerID;
            //BTN_Follows[current_follow - 1].GetComponentInChildren<Text>().text = "Unfo";
            this.photonView.RPC("RPC_BroadFollowing", RpcTarget.All, PhotonNetwork.LocalPlayer.ActorNumber, current_follow);
        }
        else
        {
            // if currently following other user:
            // first remove the follow mode on the current following user's savecam
            fSaveCamera.SendMessage("SetFollowMode", false);
            fSaveCamera = null;
            fVideoPlayer = null;
            //BTN_Follows[current_follow - 1].GetComponentInChildren<Text>().text = "Follow";

            // also tell who's been following that I'm not following any more:
            this.photonView.RPC("RPC_BroadUnfollow", RpcTarget.All, PhotonNetwork.LocalPlayer.ActorNumber, current_follow);

            // if just un follow the current follow user:
            if (current_follow == playerID)
            {
                Mod_Follow = false;
                current_follow = -1;
            }
            // unfollow the current user and follow another user:
            else
            {
                Mod_Follow = true;
                current_follow = playerID;
                //BTN_Follows[current_follow - 1].GetComponentInChildren<Text>().text = "Unfo";
                this.photonView.RPC("RPC_BroadFollowing", RpcTarget.All, PhotonNetwork.LocalPlayer.ActorNumber, current_follow);
            }
        }

        ProgressBar.SendMessage("SetFollowMode", Mod_Follow);
        BTN_Play.SetActive(!Mod_Follow);

        if (Mod_Follow)
        {
            fSaveCamera = GameObject.Find("/SaveImageCameraNSyncP" + current_follow + "(Clone)");
            fSaveCamera.SendMessage("SetFollowMode", true);
            LogViewFollow();

            fVideoPlayer = GameObject.Find("/SaveImageCameraNSyncP" + current_follow + "(Clone)/360_videoplayer_ouser").GetComponent<VideoPlayer>();
            if (fVideoPlayer == null)
                return;
            //mVideoPlayer.frame = fVideoPlayer.frame;
            //if (fVideoPlayer.isPlaying) mVideoPlayer.Play();
            //else mVideoPlayer.Pause();

            //if (fVideoPlayer.isPlaying) mVideoPlayer.gameObject.SendMessage("PlayVideo");
            //else mVideoPlayer.gameObject.SendMessage("PauseVideo");
            mVideoPlayer.gameObject.SendMessage("UpdatePlayingStatus", fVideoPlayer.isPlaying);

            int f_frame = (int)fVideoPlayer.frame;
            mVideoPlayer.gameObject.SendMessage("SetVideoFrame", f_frame);
        }
    }

    public void UpdateFollowPlayStatus()
    {
        if (!Mod_Follow) return;

        //if (fVideoPlayer.isPlaying) mVideoPlayer.Play();
        //else mVideoPlayer.Pause();
        //if (fVideoPlayer.isPlaying) mVideoPlayer.gameObject.SendMessage("PlayVideo");
        //else mVideoPlayer.gameObject.SendMessage("PauseVideo");

        mVideoPlayer.gameObject.SendMessage("UpdatePlayingStatus", fVideoPlayer.isPlaying);
    }


    public void UpdateFollowPlayFrame(object[] tempStorage)
    {
        int frame = (int)tempStorage[0];
        int from_id = (int)tempStorage[1];

        if (!Mod_Follow) return;
        if (from_id != current_follow) return;

        //mVideoPlayer.frame = fVideoPlayer.frame;
        int setFrame = (int)(fVideoPlayer.frame);
        mVideoPlayer.gameObject.SendMessage("SetVideoFrame", frame);
    }

    [PunRPC]
    public void RPC_BroadFollowing(int whoisfollowing, int whoisfollowed) 
    {
        int localplayer = PhotonNetwork.LocalPlayer.ActorNumber;

        if ((localplayer != whoisfollowing)&& (localplayer != whoisfollowed))
        {
            DebugLog.text = "do nothing";
            //do nothing
        }
        else
        {
            //define whoisfollowing layer
            int whoisfollowing_layer = whoisfollowing + 7;
            int whoisfollowed_layer = whoisfollowed + 7;
            object[] layers = new object[2];
            layers[0] = whoisfollowing_layer;
            layers[1] = whoisfollowed_layer;
            DebugLog.text = layers[0]+"-"+ layers[1];
            //if whoisfollowed is local player, he should able to see the annotation from whoisfollowing
            //set cullingmask on OVR camera
            OVRCamera = GameObject.Find("/OVRNetworkCameraRig");
            OVRCamera.SendMessage("DeleteFilter", layers);//whoisfollowing delete filter, whoisfollowed delete filter
        }

        if (whoisfollowed == localplayer)
        {    
            followedby += whoisfollowing;
            // set follow button to inactable:
            foreach (Button btn in BTN_Follows)
            {
                btn.interactable = false;
            }

            // change this button's color at all users:
            this.photonView.RPC("ChangeBtnColor", RpcTarget.All, whoisfollowed, whoisfollowed);

            // show the view box of who's following me
            frSaveCamera = GameObject.Find("/SaveImageCameraNSyncP" + whoisfollowing + "(Clone)");
            frSaveCamera.SendMessage("SetFollowMode", true);
        }
    }

    [PunRPC]
    public void ChangeBtnColor(int change_id, int color_id)
    {

        Color32 red = new Color32(199, 59, 11, 255);
        Color32 blue = new Color32(86, 180, 233, 255);
        Color32 yellow = new Color32(239, 199, 132, 255);
        Color32 green = new Color32(37, 190, 103, 255);
        Color32 orange = new Color32(230, 159, 0, 255);
        Color btn_color;

        switch (color_id)
        {
            case 0: // case 0 is turn to white (reset color)
                btn_color = new Color32(255, 255, 255, 255);
                break;
            case 1:
                btn_color = orange;
                break;
            case 2:
                btn_color = blue;
                break;
            case 3:
                btn_color = green;
                break;
            case 4:
                btn_color = Color.yellow;
                break;
            case 5:
                btn_color = Color.magenta;
                break;
            default:
                btn_color = Color.black;
                break;
        }

        BTN_Follows[change_id - 1].GetComponent<Image>().color = btn_color;
    }

    [PunRPC]
    public void RPC_BroadUnfollow(int whoisfollowing, int whowasfollowed)
    {
        //set cullingmask on OVR camera
        OVRCamera = GameObject.Find("/OVRNetworkCameraRig");
        //if whoisfollowed is local player, he should able to see the annotation from whoisfollowing
        //define whoisfollowing layer
        OVRCamera.SendMessage("LayerFilter", PhotonNetwork.LocalPlayer.ActorNumber);

        if (whowasfollowed == PhotonNetwork.LocalPlayer.ActorNumber)
        {
            followedby -= whoisfollowing;
            if (followedby == 0)
            {
                // then no one is following me, I can follow again:
                foreach (Button btn in BTN_Follows)
                {
                    btn.interactable = true;

                    // change this button's color at all users:
                    this.photonView.RPC("ChangeBtnColor", RpcTarget.All, whowasfollowed, 0);
                }
            }

            // hide who ever was follow me's viewbox:
            frSaveCamera = GameObject.Find("/SaveImageCameraNSyncP" + whoisfollowing + "(Clone)");
            frSaveCamera.SendMessage("SetFollowMode", false);
        }
    }


    public void LogViewFollow()
    {
        string path = Application.persistentDataPath + "/RoomLog.log";
        using FileStream stream = new FileStream(path, FileMode.Append);
        using var sr = new StreamWriter(stream);

        sr.WriteLineAsync("V-F");
    }
}
