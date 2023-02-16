using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Photon.Pun;
using Photon.Realtime;
using Networking.Pun2;

public class ProgressbarControl : MonoBehaviourPunCallbacks,
    IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, IDragHandler,
    IPointerUpHandler, IPointerDownHandler
{
    public VideoPlayer videoPlayer;
    public Image progressIndicater;

    public Color oriColor;
    public Color highColor;
    private bool inControl;

    public float leftPosition;
    public float rightPosition;
    public float barTop;
    public float barCenter;
    
    public Image Display;
    public Image Display_Tall;

    public Image DisplayNote;
    public Image DisplayNote_Tall;

    public Canvas mainCanvas;
    public GameObject Clicker;

    public Camera ScreenshotCamera;

    public Text DebugLog;


    private float last_image_position;
    private bool last_image_tall;

    public GameObject ScreenShotStore;

    public Image ScreenShotIndicator;
    public Image ScreenShotNoteIndicator;

    public Text StartTime;
    public Text EndTime;

    public bool Mod_Follow;
    public int control_number;



    // Start is called before the first frame update
    void Start()
    {
        //oriColor = progressIndicater.color;
        inControl = false;
        control_number = 0;

        leftPosition = (float)transform.localPosition[0] - 0.5f * (float)GetComponent<RectTransform>().rect.width;
        rightPosition = (float)transform.localPosition[0] + 0.5f * (float)GetComponent<RectTransform>().rect.width;
        barTop = (float)transform.localPosition[1] + 0.5f * (float)GetComponent<RectTransform>().rect.height;
        barCenter = (float)transform.localPosition[1];

        //leftPosition = (float)transform.localPosition[0] - 0.5f * (float)transform.localScale[0];
        //rightPosition = (float)transform.localPosition[0] + 0.5f * (float)transform.localScale[0];
        //barTop = (float)transform.localPosition[1] + 0.5f * (float)transform.localScale[1];
        last_image_tall = false;

        ScreenShotStore = GameObject.Find("ScreenShotStore");

        StartTime.text = "00:00";
        //string min = Mathf.Floor((int)videoPlayer.clip.length / 60).ToString("00");
        //string sec = Mathf.Floor((int)videoPlayer.clip.length % 60).ToString("00");
        //EndTime.text = min + ":" + sec;

        Mod_Follow = false;

        // playerid = PhotonNetwork.LocalPlayer.ActorNumber;
        Color32 red = new Color32(199, 59, 11, 255);
        Color32 blue = new Color32(86, 180, 233, 255);
        Color32 yellow = new Color32(239, 199, 132, 255);
        Color32 green = new Color32(37, 190, 103, 255);
        Color32 orange = new Color32(230, 159, 0, 255);


        switch (PhotonNetwork.LocalPlayer.ActorNumber)
        {
            case 1:
                oriColor = orange;// Color.red;
                break;
            case 2:
                oriColor = blue;//Color.cyan;
                break;
            case 3:
                oriColor = green;// Color.green;
                break;
            case 4:
                oriColor = Color.yellow;
                break;
            case 5:
                oriColor = Color.magenta;
                break;
            default:
                oriColor = Color.blue;
                break;
        }

        progressIndicater.color = oriColor;
        highColor = oriColor;
        highColor.a = 150;
    }

    // Update is called once per frame
    void Update()
    {
        if (!inControl)
        {
            float progress = (float)videoPlayer.frame / (float)videoPlayer.frameCount;
            progressIndicater.fillAmount = progress;
            if (videoPlayer.frame == (long)videoPlayer.frameCount)
                progressIndicater.fillAmount = 1.0f;
        }

        // update time:
        string min = Mathf.Floor((int)videoPlayer.time / 60).ToString("00");
        string sec = Mathf.Floor((int)videoPlayer.time % 60).ToString("00");
        StartTime.text = min + ":" + sec;
    }

    public void SetFollowMode(bool followmod)
    {
        this.Mod_Follow = followmod;
        if (Mod_Follow)
            progressIndicater.color = new Color32(177, 177, 177, 255);
        else
            progressIndicater.color = oriColor;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (Mod_Follow) return;
        progressIndicater.color = highColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (Mod_Follow) return;
        progressIndicater.color = oriColor;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (Mod_Follow) return;

        Vector3 click_worldposition = Clicker.transform.position;
        //click_worldposition.z = Vector3.Distance(mainCanvas.transform.position, mainCam.transform.position);
        Vector3 click_locakposition = mainCanvas.transform.InverseTransformPoint(click_worldposition);
        float inputX = click_locakposition[0];
        float inputProgress = (inputX - leftPosition) / (rightPosition - leftPosition);
        progressIndicater.fillAmount = inputProgress;

        // videoPlayer.frame = (int)(inputProgress * videoPlayer.frameCount);
        int setFrame = (int)(inputProgress * videoPlayer.frameCount);
        videoPlayer.gameObject.SendMessage("SetVideoFrame", setFrame);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (Mod_Follow) return;

        inControl = true;
        control_number++;
        Vector3 click_worldposition = Clicker.transform.position;
        //click_worldposition.z = Vector3.Distance(mainCanvas.transform.position, mainCam.transform.position);
        Vector3 click_locakposition = mainCanvas.transform.InverseTransformPoint(click_worldposition);
        float inputX = (float) click_locakposition[0];
        float inputProgress = (inputX - leftPosition) / (rightPosition - leftPosition);
        progressIndicater.fillAmount = inputProgress;

        // videoPlayer.frame = (int)(inputProgress * videoPlayer.frameCount);
        int setFrame = (int)(inputProgress * videoPlayer.frameCount);
        videoPlayer.gameObject.SendMessage("SetVideoFrame", setFrame);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (Mod_Follow) return;

        inControl = true;
        control_number++;
        Vector3 click_worldposition = Clicker.transform.position;
        //click_worldposition.z = Vector3.Distance(mainCanvas.transform.position, mainCam.transform.position);
        Vector3 click_locakposition = mainCanvas.transform.InverseTransformPoint(click_worldposition);
        float inputX = (float) click_locakposition[0];
        float inputProgress = (inputX - leftPosition) / (rightPosition - leftPosition);
        videoPlayer.frame = (int)(inputProgress * videoPlayer.frameCount);

        // videoPlayer.frame = (int)(inputProgress * videoPlayer.frameCount);
        int setFrame = (int)(inputProgress * videoPlayer.frameCount);
        videoPlayer.gameObject.SendMessage("SetVideoFrame", setFrame);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (Mod_Follow) return;

        inControl = false;

    }

    public void AddDisplay(ScreenShot screenShot)
    {
        //float progress = (float) videoPlayer.frame / (float) videoPlayer.frameCount;
        screenShot.Displayed = true;

        float positionX = screenShot.progress * (rightPosition - leftPosition) + leftPosition;
        screenShot.positionX = positionX;

        Image display_image;
        float hight_variance;

        // go through all the other screenshots to decide which level this one should be on:
        List<ScreenShot> all_screenshots = ScreenShotStore.GetComponent<ScreenShotStore>().screenshot_store;
        float min_x_dis = rightPosition - leftPosition;
        int min_x_level = 1;
        foreach (ScreenShot ss in all_screenshots)
        {
            if (ss == screenShot) continue;
            if (Mathf.Abs(positionX - ss.positionX) < min_x_dis)
            {
                min_x_dis = Mathf.Abs(positionX - ss.positionX);
                min_x_level = ss.level;
            }
        }
        if (min_x_dis < 80.0f && min_x_level == 1)
        {
            if (screenShot.note == "")
                display_image = Display_Tall;
            else
                display_image = DisplayNote_Tall;
            hight_variance = 70.0f;
            last_image_tall = true;
            screenShot.level = 2;
        }
        else
        {
            if (screenShot.note == "")
                display_image = Display;
            else
                display_image = DisplayNote;
            hight_variance = 35.0f;
            last_image_tall = false;
            screenShot.level = 1;
            //DebugLog.text = "screenShot note is " + (screenShot.note == "") + " empty.";
        }

        var DisplayIndicator = Instantiate(ScreenShotIndicator, mainCanvas.transform);

        // decide the height of the indicator based on user ID, here we assume PlayerID = 1 means is the researcher
        float height = 18.0f;
        float length = 20.0f;
        if (screenShot.playerID == 2)
        {
            height += 18.0f;
            length += 20.0f;
        }
        if (screenShot.playerID == 3)
        {
            height += 36.0f;
            length += 40.0f;
        }
        DisplayIndicator.transform.localPosition = new Vector3(positionX, barTop + height, 0);
        DisplayIndicator.color = screenShot.playerColor;
        DisplayIndicator.SendMessage("SetScreenShot", screenShot);

        RawImage DisplayGun = DisplayIndicator.transform.GetChild(0).GetComponent<RawImage>();
        DisplayGun.color = screenShot.playerColor;
        DisplayGun.GetComponent<RectTransform>().sizeDelta = new Vector2(0.5f, length);
        float current_gun_Y = DisplayGun.transform.localPosition.y;
        current_gun_Y -= length / 4;
        DisplayGun.transform.localPosition = new Vector3(0, current_gun_Y, 0);

        //var DisplayImage = Instantiate(display_image, mainCanvas.transform);
        //DisplayImage.transform.localPosition = new Vector3(positionX + 50.0f, barTop + hight_variance, 0);
        //DisplayImage.color = screenShot.playerColor;

        //RawImage display = DisplayImage.transform.GetChild(0).GetComponent<RawImage>();
        //display.texture = screenShot.image;

        //if (screenShot.note != "")
        //{
        //    RawImage NoteIcon = DisplayImage.transform.GetChild(1).GetComponent<RawImage>();
        //    NoteIcon.color = screenShot.playerColor;
        //    DisplayImage.SendMessage("SetNoteContent", screenShot.note);
        //}
    }

    // will not be using
    public void AddMissingDisplay()
    {
        // in case any screenshot is missing:
        List<ScreenShot> all_screenshots = ScreenShotStore.GetComponent<Networking.Pun2.ScreenShotStore>().screenshot_store;
        foreach (ScreenShot screenShot in all_screenshots)
        {
            if (screenShot.Displayed) continue;

            float positionX = screenShot.progress * (rightPosition - leftPosition) + leftPosition;
            screenShot.positionX = positionX;

            Image display_image;
            float hight_variance;

            float min_x_dis = rightPosition - leftPosition;
            int min_x_level = 1;
            foreach (ScreenShot ss in all_screenshots)
            {
                if (ss == screenShot) continue;
                if (Mathf.Abs(positionX - ss.positionX) < min_x_dis)
                {
                    min_x_dis = Mathf.Abs(positionX - ss.positionX);
                    min_x_level = ss.level;
                }
            }
            if (min_x_dis < 80.0f && min_x_level == 1)
            {
                if (screenShot.note != "")
                    display_image = DisplayNote_Tall;
                else
                    display_image = Display_Tall;
                hight_variance = 70.0f;
                last_image_tall = true;
                screenShot.level = 2;
            }
            else
            {
                if (screenShot.note != "")
                    display_image = DisplayNote;
                else
                    display_image = Display;
                hight_variance = 35.0f;
                last_image_tall = false;
                screenShot.level = 1;
            }

            var DisplayIndicator = Instantiate(ScreenShotIndicator, mainCanvas.transform);
            DisplayIndicator.transform.localPosition = new Vector3(positionX, barTop + 18.0f, 0);
            DisplayIndicator.color = screenShot.playerColor;
            RawImage DisplayGun = DisplayIndicator.transform.GetChild(0).GetComponent<RawImage>();
            DisplayGun.color = screenShot.playerColor;
            DisplayIndicator.SendMessage("SetScreenShot", screenShot);

            //var DisplayImage = Instantiate(display_image, mainCanvas.transform);
            //DisplayImage.transform.localPosition = new Vector3(positionX + 50.0f, barTop + hight_variance, 0);
            //DisplayImage.color = screenShot.playerColor;

            //RawImage display = DisplayImage.transform.GetChild(0).GetComponent<RawImage>();
            //display.texture = screenShot.image;

            //screenShot.Displayed = true;

        }
    }

    //[PunRPC]
    //public void DisplayScreenshot(int screenshot_idx, float progress)
    ////public void DisplayScreenshot(byte[] bImageArray, float progress)
    //{
    //    Texture2D Image = new Texture2D(ScreenshotCamera.pixelWidth, ScreenshotCamera.pixelHeight);
    //    Image.LoadImage(all_screenshots[screenshot_idx]);

    //    float positionX = progress * (rightPosition - leftPosition) + leftPosition;

    //    var DisplayImage = Instantiate(Display, mainCanvas.transform);
    //    DisplayImage.transform.localPosition = new Vector3(positionX + 50.0f, barTop + 50.0f, 0);

    //    RawImage display = DisplayImage.transform.GetChild(0).GetComponent<RawImage>();
    //    display.texture = Image;
    //}

}