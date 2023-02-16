using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.Video;

public class SaveImage : MonoBehaviourPunCallbacks
{
    public Camera userCamera;
    public RenderTexture currentRT;

    public GameObject progressBar;

    public GameObject ScreenShotStore;

    public Text DebugLog;

    public int PlayerID;

    public VideoPlayer videoPlayer;

    public ScreenShot nextNoteScreenshot;

    public bool AsyncMode;

    public GameObject RenderTextureMutex;

    // Start is called before the first frame update
    void Start()
    {
        userCamera = GetComponent<Camera>();
        progressBar = GameObject.Find("/Canvas/ProgressBar");
        ScreenShotStore = GameObject.Find("ScreenShotStore");
        DebugLog = GameObject.Find("/Canvas/DebugLog").GetComponent<Text>();
        if (AsyncMode)
            videoPlayer = this.transform.GetChild(0).GetComponent<VideoPlayer>();
        else
            videoPlayer = GameObject.Find("360_videoplayer").GetComponent<VideoPlayer>();
        PlayerID = PhotonNetwork.LocalPlayer.ActorNumber;
        RenderTextureMutex = GameObject.Find("/RenderTextureMutex");
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void TakeScreenshot()
    {
        if (photonView.IsMine)
        {
            this.photonView.RPC("CamCapture", RpcTarget.All, PlayerID);
        }
    }

    public void TakeNoteSC()
    {
        if (photonView.IsMine)
        {
            this.photonView.RPC("NoteCamCapture", RpcTarget.All, PlayerID);
        }
    }

    public void SetNoteContent(string note_content)
    {
        if (photonView.IsMine)
        {
            this.photonView.RPC("SetNote", RpcTarget.All, note_content);
        }
    }

    [PunRPC]
    public void CamCapture(int playerID)
    {
        //while (RenderTextureMutex.GetComponent<RenderTextureMutex>().RenderTextureInUse)
        //{
        //    continue;
        //}
        //RenderTextureMutex.GetComponent<RenderTextureMutex>().RenderTextureInUse = true;
        RenderTexture currentRT = RenderTexture.active;
        RenderTexture.active = userCamera.targetTexture;

        userCamera.Render();
        //
        Texture2D Image = new Texture2D(userCamera.pixelWidth, userCamera.pixelHeight, TextureFormat.ARGB32, false, true);
        Image.ReadPixels(new Rect(0, 0, userCamera.pixelWidth, userCamera.pixelHeight), 0, 0);
        //Image.Resize(userCamera.pixelWidth / 2, userCamera.pixelHeight / 2);

        //Color[] pixels = Image.GetPixels();
        //for (int p = 0; p < pixels.Length; p++)
        //{
        //    pixels[p] = pixels[p].gamma;
        //}
        //Image.SetPixels(pixels);
        //
        Image.Apply();
        RenderTexture.active = currentRT;

        float progress = (float)videoPlayer.frame / (float)videoPlayer.frameCount;
        ScreenShot screenShot = new ScreenShot(Image, playerID, progress);
        ScreenShotStore.SendMessage("EnqueueScreenshot", screenShot);

        // display screenshot:
        progressBar.SendMessage("AddDisplay", screenShot);

        DebugLog.text = "SC taken at " + progress + " from user " + playerID;

        //RenderTextureMutex.GetComponent<RenderTextureMutex>().RenderTextureInUse = false;
    }

    [PunRPC]
    public void NoteCamCapture(int playerID)
    {
        RenderTexture currentRT = RenderTexture.active;
        RenderTexture.active = userCamera.targetTexture;

        userCamera.Render();

        //Texture2D Image = new Texture2D(userCamera.pixelWidth, userCamera.pixelHeight);
        //Image.ReadPixels(new Rect(0, 0, userCamera.pixelWidth, userCamera.pixelHeight), 0, 0);

        //
        Texture2D Image = new Texture2D(userCamera.pixelWidth, userCamera.pixelHeight, TextureFormat.ARGB32, false, true);
        Image.ReadPixels(new Rect(0, 0, userCamera.pixelWidth, userCamera.pixelHeight), 0, 0);

        /*
        Color[] pixels = Image.GetPixels();
        for (int p = 0; p < pixels.Length; p++)
        {
            pixels[p] = pixels[p].gamma;
        }
        Image.SetPixels(pixels);*/
        //
        Image.Apply();
        RenderTexture.active = currentRT;

        float progress = (float)videoPlayer.frame / (float)videoPlayer.frameCount;
        nextNoteScreenshot = new ScreenShot(Image, playerID, progress);
    }

    [PunRPC]
    public void SetNote(string note_content)
    {
        DebugLog.text = "SetNote_PUNRPC";
        nextNoteScreenshot.note = note_content;
        ScreenShotStore.SendMessage("EnqueueScreenshot", nextNoteScreenshot);
        progressBar.SendMessage("AddDisplay", nextNoteScreenshot);
    }
}
