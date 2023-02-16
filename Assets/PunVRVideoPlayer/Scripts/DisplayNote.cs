using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Photon.Pun;
using Photon.Realtime;

public class DisplayNote : MonoBehaviourPunCallbacks,
    IPointerEnterHandler, IPointerExitHandler
{
    public bool ShowTextBox;
    public GameObject NoteBox;
    // Start is called before the first frame update
    void Start()
    {
        NoteBox.SetActive(false);
        ShowTextBox = false;
    }

    // Update is called once per frame
    void Update()
    {
        NoteBox.SetActive(ShowTextBox);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        ShowTextBox = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ShowTextBox = false;
    }

    public void SetNoteContent(string text)
    {
        Text ui_text = NoteBox.transform.GetChild(0).GetComponent<Text>();
        ui_text.text = text;
    }
}
