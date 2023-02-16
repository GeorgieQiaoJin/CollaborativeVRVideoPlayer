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
    public class LogProcessBar : MonoBehaviour
    {
        public ProgressbarControl progressbarControl;

        public void LogScreenshot()
        {
            LogInteraction();
        }

        public void LogInteraction()
        {
            string path = Application.persistentDataPath + "/RoomLog.log";
            using FileStream stream = new FileStream(path, FileMode.Append);
            using var sr = new StreamWriter(stream);

            sr.WriteLineAsync("-----------------");

            sr.WriteLineAsync("The number of the processbar control is " + progressbarControl.control_number);
        }

    }
}
