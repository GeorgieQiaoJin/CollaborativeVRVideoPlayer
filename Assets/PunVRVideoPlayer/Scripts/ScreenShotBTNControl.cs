using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScreenShotBTNControl : MonoBehaviour
{
    public GameObject[] saveCameras;
    public Button screenshotBTN;
    // Start is called before the first frame update
    void Start()
    {
        saveCameras = GameObject.FindGameObjectsWithTag("SaveCam");

        screenshotBTN = GetComponent<Button>();
        // screenshotBTN.onClick.AddListener(TaskOnClick);
    }

    // Update is called once per frame
    void Update()
    {
        // if (saveCameras.Length == 0)
            saveCameras = GameObject.FindGameObjectsWithTag("SaveCam");
    }

    public void TaskOnClick()
    {
        foreach (GameObject saveCam in saveCameras)
        {
            saveCam.SendMessage("TakeScreenshot");
        }
    }
}
