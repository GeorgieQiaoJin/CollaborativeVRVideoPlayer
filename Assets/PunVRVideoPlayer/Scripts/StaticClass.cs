using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class StaticClass
{
    public static string cur_videoname = "";
    public static List<string> has_played_video = new List<string>();

    public static void SetVideo(string name)
    {
        if (!name.Contains("tutorial") && has_played_video.Contains(name))
        {
            cur_videoname = "";
            return;
        }
        
        cur_videoname = name;
        has_played_video.Add(name);
    }
}
