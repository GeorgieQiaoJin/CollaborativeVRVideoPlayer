using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class HideFullscreenDisplay : MonoBehaviour, IPointerClickHandler
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.SetAsLastSibling();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        this.gameObject.SetActive(false);
    }
}
