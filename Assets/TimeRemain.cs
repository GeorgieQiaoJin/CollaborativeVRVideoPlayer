using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class TimeRemain : MonoBehaviourPunCallbacks
{
    public int timelim_min;
    public float timeremain_sec;
    // Start is called before the first frame update
    void Start()
    {
        timeremain_sec = timelim_min * 60.0f;

        string min = Mathf.Floor((int)timeremain_sec / 60).ToString("00");
        string sec = Mathf.Floor((int)timeremain_sec % 60).ToString("00");
        this.GetComponent<Text>().text = min + ":" + sec;
    }

    // Update is called once per frame
    void Update()
    {
        if (timeremain_sec > 0)
        {
            timeremain_sec -= Time.deltaTime;
        }

        string min = Mathf.Floor((int)timeremain_sec / 60).ToString("00");
        string sec = Mathf.Floor((int)timeremain_sec % 60).ToString("00");
        this.GetComponent<Text>().text = min + ":" + sec;
    }

    public void ResetTimer(float time_remain)
    {
        //timeremain_sec = time_remain;
        if (PhotonNetwork.IsMasterClient)
        {
            this.photonView.RPC("RPC_ResetTimer", RpcTarget.All, time_remain);
        }
    }

    [PunRPC]
    public void RPC_ResetTimer(float time_remain)
    {
        timeremain_sec = time_remain;
    }
}
