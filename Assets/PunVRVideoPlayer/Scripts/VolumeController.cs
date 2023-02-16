using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class VolumeController : MonoBehaviour
{
    [SerializeField] private Slider volumeSlider = null;


    public GameObject player;

    public void Start()
    {
        loadValues();
        volumeSlider.onValueChanged.AddListener(delegate { SetLevel(); });
    }

    public void SetLevel()
    {
        float volumeValue = volumeSlider.value;
        PlayerPrefs.SetFloat("volumeValue", volumeValue);
        loadValues();
    }

    /*
    public void OnPointerExit(PointerEventData eventData)
    {
        float volumeValue = volumeSlider.value;
        PlayerPrefs.SetFloat("volumeValue", volumeValue);
        loadValues();
    }*/

    public void loadValues()
    {
        float volumeValue = PlayerPrefs.GetFloat("volumeValue");
        volumeSlider.value = volumeValue;
        player.GetComponent<AudioSource>().volume = volumeValue;

    }
}
