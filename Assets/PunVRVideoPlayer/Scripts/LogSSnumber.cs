using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace Networking.Pun2
{
    public class LogSSnumber : MonoBehaviour
    {

        public ScreenShotStore screenShotStore;
        public ProgressbarControl progressbarControl;

        public List<ScreenShot> ss_store;
        private int total_ss_number;
        private int note_number = 0;

        public void LogScreenshot()
        {
            if(SceneManager.GetActiveScene().buildIndex != 4)
            {
                screenShotStore = GameObject.Find("ScreenShotStore").GetComponent<ScreenShotStore>();

                ss_store = screenShotStore.screenshot_store_p[PhotonNetwork.LocalPlayer.ActorNumber];
                total_ss_number = ss_store.Count;


                foreach (ScreenShot ss in ss_store)
                {
                    if (ss.note != "")
                    {
                        note_number = note_number + 1;
                    }
                }

                LogInteraction(total_ss_number, note_number);
            }
            else//basic mod
            {
                LogInteraction(0, 0);
            }
            
        }

        public void LogInteraction(int total_ss_number, int note_number)
        {
            string path = Application.persistentDataPath + "/RoomLog.log";
            using FileStream stream = new FileStream(path, FileMode.Append);
            using var sr = new StreamWriter(stream);

            sr.WriteLineAsync("-----------------");

            sr.WriteLineAsync("The number of the screenshot (includes notes) is " + total_ss_number);
            sr.WriteLineAsync("The number of the note is " + note_number);
            sr.WriteLineAsync("The number of the processbar control is " + progressbarControl.control_number);
        }


    }
}
