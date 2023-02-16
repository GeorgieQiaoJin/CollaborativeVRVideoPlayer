using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;
using Photon.Pun;
using System.IO;
using Photon.Realtime;

public class VideoPlayer_Control_Asy : MonoBehaviour
{
    public bool isPlaying;
    public VideoPlayer videoPlayer;
    public Button playButton;
    public Sprite sprt_play;
    public Sprite sprt_pause;
    public GameObject[] OUserProgressbars;
    public GameObject[] saveCameras;
    public GameObject mSaveCamera;
    public Text DebugLog;


    #region MonoBehaviour CallBacks
    // Start is called before the first frame update
    void Start()
    {
        isPlaying = true;
        UpdatePlayBTNText();
        OUserProgressbars = GameObject.FindGameObjectsWithTag("OUserProgressBar");
        mSaveCamera = GameObject.Find("/SaveImageCameraNSyncP" + PhotonNetwork.LocalPlayer.ActorNumber + "(Clone)");
    }

    // Update is called once per frame
    void Update()
    {
        // UpdatePlayingStatus();
        //float progress = (float)videoPlayer.frame / (float)videoPlayer.frameCount;
        //foreach (GameObject OUserProgressbar in OUserProgressbars)
        //{
        //    OUserProgressbar.SendMessage("SetProgress", progress);
        //}
        //if (saveCameras.Length < 2)
        //    saveCameras = GameObject.FindGameObjectsWithTag("SaveCam");
        if (mSaveCamera == null)
            mSaveCamera = GameObject.Find("/SaveImageCameraNSyncP" + PhotonNetwork.LocalPlayer.ActorNumber + "(Clone)");

    }

    #endregion

    #region Custom - local

    private void UpdatePlayingStatus(bool playstatus)
    {
        if (playstatus)
        {
            PlayVideo();

            //foreach (GameObject saveCam in saveCameras)
            //{
            //    saveCam.SendMessage("UpdatePlayingStatus", true);
            //}
            mSaveCamera.SendMessage("UpdatePlayingStatus", true);
        }
        else
        {
            PauseVideo();

            //foreach (GameObject saveCam in saveCameras)
            //{
            //    saveCam.SendMessage("UpdatePlayingStatus", false);
            //}
            mSaveCamera.SendMessage("UpdatePlayingStatus", false);
        }
    }

    public void OnPlayBTNPressed()
    {
        LogPlayPause();
        isPlaying = !isPlaying;
        UpdatePlayingStatus(isPlaying);
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

        mSaveCamera.SendMessage("UpdatePlayingFrame", setFrame);
    }

    public void PlayVideo()
    {
        videoPlayer.Play();
        isPlaying = true;
        UpdatePlayBTNText();
    }

    public void PauseVideo()
    {
        videoPlayer.Pause();
        isPlaying = false;
        UpdatePlayBTNText();
    }

    public void PlayVideoFrame(int setFrame)
    {
        videoPlayer.frame = setFrame;
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
