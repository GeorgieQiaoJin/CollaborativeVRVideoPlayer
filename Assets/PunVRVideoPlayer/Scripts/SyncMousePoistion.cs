using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.EventSystems;

public class SyncMousePoistion : MonoBehaviourPunCallbacks, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject Pointer;
    public Canvas mainCanvas;
    public GameObject mPointer;
    public Color playerColor;
    private bool ShowPointer;
    public GameObject Clicker;
    public bool SyncMode;
    // Start is called before the first frame update
    void Start()
    {
        if (SyncMode)
            mPointer = PhotonNetwork.Instantiate(Pointer.name, Clicker.transform.position, Quaternion.identity, 0);
        else
            mPointer = GameObject.Instantiate(Pointer, Clicker.transform.position, Quaternion.identity);

        ShowPointer = true;

        mPointer.SetActive(ShowPointer);
    }

    // Update is called once per frame
    void Update()
    {
        if (ShowPointer)
        {
            Vector3 click_worldposition = Clicker.transform.position;
            Vector3 click_locakposition = mainCanvas.transform.InverseTransformPoint(click_worldposition);
            mPointer.transform.localPosition = click_locakposition;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        ShowPointer = true;
        mPointer.SetActive(ShowPointer);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ShowPointer = false;
        mPointer.SetActive(ShowPointer);
    }
}
