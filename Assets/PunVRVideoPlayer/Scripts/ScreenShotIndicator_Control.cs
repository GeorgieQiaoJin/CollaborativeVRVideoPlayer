using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine;

public class ScreenShotIndicator_Control : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    private ScreenShot screenShot;
    private GameObject ScreenShotDisplay;
    private GameObject ScreenShotNoteDisplay;
    public GameObject mainCanvas;
    public Sprite NoteIcon;
    public VideoPlayer mVideoPlayer;

    // Start is called before the first frame update
    void Start()
    {
        //ScreenShotDisplay = GameObject.Find("/Canvas/ScreenShotDisplay");
        //ScreenShotNoteDisplay = GameObject.Find("/Canvas/ScreenShotNoteDisplay");
        mainCanvas = GameObject.Find("/Canvas");
        ScreenShotDisplay = mainCanvas.transform.Find("ScreenShotDisplay").gameObject;
        ScreenShotNoteDisplay = mainCanvas.transform.Find("ScreenShotNoteDisplay").gameObject;
        mVideoPlayer = GameObject.Find("/360_videoplayer").GetComponent<VideoPlayer>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (screenShot.note == "")
        {
            ScreenShotDisplay.SetActive(true);
            RawImage display = ScreenShotDisplay.transform.GetChild(0).GetComponent<RawImage>();
            display.texture = screenShot.image;
        }
        else
        {
            ScreenShotNoteDisplay.SetActive(true);
            RawImage display = ScreenShotNoteDisplay.transform.GetChild(0).GetComponent<RawImage>();
            display.texture = screenShot.image;
            Text note = ScreenShotNoteDisplay.transform.GetChild(1).GetComponent<Text>();
            note.text = screenShot.note;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ScreenShotDisplay.SetActive(false);
        ScreenShotNoteDisplay.SetActive(false);
    }

    public void SetScreenShot(ScreenShot inputScreenShot)
    {
        this.screenShot = inputScreenShot;
        if (this.screenShot.note != "")
            gameObject.GetComponent<Image>().sprite = NoteIcon;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // Sync Mode
        if (mainCanvas.GetComponent<SyncMousePoistion>().SyncMode)
            mVideoPlayer.gameObject.SendMessage("SetVideoFrame", screenShot.progress * (float)mVideoPlayer.frameCount);

        // jump to the screenshot's position:
        if (!mainCanvas.GetComponent<FollowMode>().Mod_Follow) // NSync Mode, check if follow mode
            mVideoPlayer.gameObject.SendMessage("SetVideoFrame", screenShot.progress * (float)mVideoPlayer.frameCount);




    }
}
