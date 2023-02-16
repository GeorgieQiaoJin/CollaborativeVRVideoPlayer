using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class LogView : MonoBehaviour
{
    System.DateTime startTime;
    float timer = 0;
    bool flag = false;

    [SerializeField]
    public VideoPlayer videoPlayer;


    private void Start()
    {
        flag = true;
        videoPlayer.time = 0;
    }
    private void Update()
    {
        
        timer += Time.deltaTime;
        if(timer > 0.03f)
        {
            LogInteraction(true);
            timer = 0;
            
        }
    }


    public void LogInteraction(bool start)
    {
        string path = Application.persistentDataPath + "/RoomLog.log";
        using FileStream stream = new FileStream(path, FileMode.Append);
        using var sr = new StreamWriter(stream);

        string localrotate = this.transform.localRotation.eulerAngles.ToString();

        System.DateTime currentDT = System.DateTime.Now;
        if (flag)
        {
            sr.WriteLineAsync("-------Local rotate from "+ SceneManager.GetActiveScene().name +"---------");
            startTime = currentDT;
            flag = false;
        }

        sr.WriteLineAsync(currentDT.ToString("hh:mm:ss") +"-"+ localrotate + "-video time-" + videoPlayer.time);//current time in seconds

    }



}
