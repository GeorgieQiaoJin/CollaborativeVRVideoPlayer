using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class OUserProgressControl : MonoBehaviourPunCallbacks
{
    public int ItemID;
    public GameObject UserIcon;
    public Image progressIndicater;
    public Color itemColor;
    public float CurrentProgress;
    private float height1 = -103.0f;
    private float height2 = -123.0f;
    private float MaxUserIconX = 450.0f;

    public VideoPlayer mVideoPlayer;

    // Start is called before the first frame update
    void Start()
    {

        if (PhotonNetwork.LocalPlayer.ActorNumber == 2 && ItemID == 1)
        {
            this.transform.localPosition = new Vector3(-225.0f, height1, 0.0f);
        }

        if (PhotonNetwork.LocalPlayer.ActorNumber == 3 && ItemID == 1)
        {
            this.transform.localPosition = new Vector3(-225.0f, height1, 0.0f);
        }

        if (PhotonNetwork.LocalPlayer.ActorNumber == 3 && ItemID == 2)
        {
            this.transform.localPosition = new Vector3(-225.0f, height2, 0.0f);
        }

        Color32 red = new Color32(199, 59, 11, 255);
        Color32 blue = new Color32(86, 180, 233, 255);
        Color32 yellow = new Color32(239, 199, 132, 255);
        Color32 green = new Color32(37, 190, 103, 255);
        Color32 orange = new Color32(230, 159, 0, 255);

        switch (ItemID)
        {
            case 1:
                itemColor = orange;
                break;
            case 2:
                itemColor = blue;
                break;
            case 3:
                itemColor = green;
                break;
            case 4:
                itemColor = Color.yellow;
                break;
            case 5:
                itemColor = Color.magenta;
                break;
            default:
                itemColor = Color.black;
                break;
        }

        progressIndicater.color = itemColor;
        UserIcon.GetComponent<RawImage>().color = itemColor;

        //hide itself:
        if (PhotonNetwork.LocalPlayer.ActorNumber == ItemID)
        {
            Color currColor = progressIndicater.color;
            currColor.a = 0f;
            progressIndicater.color = currColor;

            currColor = UserIcon.GetComponent<RawImage>().color;
            currColor.a = 0f;
            UserIcon.GetComponent<RawImage>().color = currColor;

            // NDZKBDDDFQ
            this.transform.localPosition = new Vector3(-225.0f, 600.0f, 0.0f);
        }

        mVideoPlayer = GameObject.Find("/SaveImageCameraNSyncP" + ItemID + "(Clone)").transform.GetChild(0).GetComponent<VideoPlayer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (mVideoPlayer == null)
            mVideoPlayer = GameObject.Find("/SaveImageCameraNSyncP" + ItemID + "(Clone)").transform.GetChild(0).GetComponent<VideoPlayer>();
        float progress = (float)mVideoPlayer.frame / (float)mVideoPlayer.frameCount;
        SetProgress(progress);
    }

    public void SetProgress(float progress)
    {
        if (PhotonNetwork.LocalPlayer.ActorNumber == ItemID)
        {
            CurrentProgress = progress;
            this.photonView.RPC("SetProgressUserIcon", RpcTarget.All, progress);
        }
    }

    [PunRPC]
    public void SetProgressUserIcon(float progress)
    {
        progressIndicater.fillAmount = Mathf.Max(0.0f, progress - 0.03f);
        UserIcon.transform.localPosition = new Vector3(progress * MaxUserIconX, 0.0f, 0.0f);
    }
}
