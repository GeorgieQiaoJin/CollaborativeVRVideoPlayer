using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;

public class SetVideo : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public VideoClip porto_videoClip;
    public VideoClip tutorial_videoClip;
    public VideoClip granada_videoClip;
    public VideoClip canaria_videoClip;
    public VideoClip saville_videoClip;

    public Text DisEndTime;
    // Start is called before the first frame update
    void Start()
    {
        videoPlayer = this.GetComponent<VideoPlayer>();

        if (StaticClass.cur_videoname.Contains("porto"))
        {
            videoPlayer.clip = porto_videoClip;

        }else if (StaticClass.cur_videoname.Contains("granada"))
        {

            videoPlayer.clip = granada_videoClip;
        }
        else if (StaticClass.cur_videoname.Contains("saville"))
        {
            videoPlayer.clip = saville_videoClip;

        }
        else if (StaticClass.cur_videoname.Contains("canaria"))
        {
            videoPlayer.clip = canaria_videoClip;
        }
        else
        {
            videoPlayer.clip = tutorial_videoClip;

        }

        string min = Mathf.Floor((int)videoPlayer.clip.length / 60).ToString("00");
        string sec = Mathf.Floor((int)videoPlayer.clip.length % 60).ToString("00");
        DisEndTime.text = min + ":" + sec;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
