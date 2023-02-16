using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasActiveControl : MonoBehaviour
{
    private bool isShowingMenu;

    public GameObject menu;
    public GameObject menu_small;


    public GameObject progressBar;
    // Start is called before the first frame update
    void Start()
    {
        isShowingMenu = true;
        menu.SetActive(isShowingMenu);
        menu_small.SetActive(!isShowingMenu);
        progressBar = GameObject.Find("/Canvas/ProgressBar");
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void ChangeMenu()
    {
        isShowingMenu = !isShowingMenu;
        menu.SetActive(isShowingMenu);
        menu_small.SetActive(!isShowingMenu);
        if (isShowingMenu)
            progressBar.SendMessage("AddMissingDisplay");
    }
}
