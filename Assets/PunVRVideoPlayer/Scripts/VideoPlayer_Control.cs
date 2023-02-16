using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using System.IO;

public class VideoPlayer_Control : MonoBehaviourPunCallbacks, IPunObservable
{
    public bool isPlaying;
    public VideoPlayer videoPlayer;
    public Button playButton;
    public Sprite sprt_play;
    public Sprite sprt_pause;

    #region IPunObservable implementation

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // We own this player: send the others our data
        }
        else
        {
            // Network player, receive data
        }
    }

    #endregion


    #region MonoBehaviour CallBacks
    // Start is called before the first frame update
    void Start()
    {
        isPlaying = true;
        UpdatePlayBTNText();
        if (!PhotonNetwork.IsMasterClient)
        {
            this.photonView.RPC("SyncProgress", RpcTarget.MasterClient);
        }

    }

    // Update is called once per frame
    void Update()
    {
        // UpdatePlayingStatus();
    }

    #endregion

    #region Custom - local

    private void UpdatePlayingStatus()
    {
        if (isPlaying)
        {
            PlayVideo();
            this.photonView.RPC("PlayVideo", RpcTarget.Others);
        }
        else
        {
            PauseVideo();
            this.photonView.RPC("PauseVideo", RpcTarget.Others);
        }
    }

    public void OnPlayBTNPressed()
    {
        LogPlayPause();
        isPlaying = !isPlaying;
        UpdatePlayingStatus();
        UpdatePlayBTNText();
    }

    public void UpdatePlayBTNText()
    {
        if (isPlaying)
            playButton.image.sprite = sprt_pause;
        else
            playButton.image.sprite = sprt_play;
    }

    public void SetVideoFrame(int setFrame)
    {
        PlayVideoFrame(setFrame);
        this.photonView.RPC("PlayVideoFrame", RpcTarget.Others, setFrame);
    }

    #endregion


    #region Custom - online

    [PunRPC]
    public void PlayVideo()
    {
        videoPlayer.Play();
        isPlaying = true;
        UpdatePlayBTNText();
    }

    [PunRPC]
    public void PauseVideo()
    {
        videoPlayer.Pause();
        isPlaying = false;
        UpdatePlayBTNText();
    }

    [PunRPC]
    public void PlayVideoFrame(int setFrame)
    {
        videoPlayer.frame = setFrame;
    }

    [PunRPC]
    public void SyncProgress()
    {
        if (!PhotonNetwork.IsMasterClient) return;
        int setFrame = (int)videoPlayer.frame;
        this.photonView.RPC("PlayVideoFrame", RpcTarget.Others, setFrame);
    }

    #endregion


    public void LogPlayPause()
    {
        string path = Application.persistentDataPath + "/RoomLog.log";
        using FileStream stream = new FileStream(path, FileMode.Append);
        using var sr = new StreamWriter(stream);

        sr.WriteLineAsync("PP");
    }
}
