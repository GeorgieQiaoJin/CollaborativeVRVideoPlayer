using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using UnityEngine.EventSystems;

public class ShowUserView : MonoBehaviour, 
    IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public GameObject ViewDisplay;
    public GameObject ViewDisplayFullscreen;
    public RenderTexture mRenderTexture;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        LogViewPeek();
        ViewDisplay.SetActive(true);
        RawImage display = ViewDisplay.transform.GetChild(0).GetComponent<RawImage>();
        display.texture = mRenderTexture;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ViewDisplay.SetActive(false);
    }
    
    public void OnPointerClick(PointerEventData eventData)
    {
        LogViewSlave();
        ViewDisplayFullscreen.SetActive(true);
        RawImage display = ViewDisplayFullscreen.transform.GetChild(0).GetComponent<RawImage>();
        display.texture = mRenderTexture;
    }

    public void LogViewPeek()
    {
        string path = Application.persistentDataPath + "/RoomLog.log";
        using FileStream stream = new FileStream(path, FileMode.Append);
        using var sr = new StreamWriter(stream);

        sr.WriteLineAsync("V-P");
    }

    public void LogViewSlave()
    {
        string path = Application.persistentDataPath + "/RoomLog.log";
        using FileStream stream = new FileStream(path, FileMode.Append);
        using var sr = new StreamWriter(stream);

        sr.WriteLineAsync("V-S");
    }

    
}
