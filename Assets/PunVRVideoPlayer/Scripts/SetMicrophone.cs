using Photon.Pun;
using UnityEngine;
using Photon.Voice.Unity;
using UnityEngine.SceneManagement;

namespace Networking.Pun2
{
    public class SetMicrophone : MonoBehaviourPun
    {
        private int index;

        //For making sure that microphone is found and set to "Recorder" component from Photon Voice
        private void Start()
        {
            index = SceneManager.GetActiveScene().buildIndex;

            string[] devices = Microphone.devices;
            if (devices.Length > 0)
            {
                GetComponent<Recorder>().UnityMicrophoneDevice = devices[0];
            }

            if (index == 4)
            {
                GetComponent<AudioSource>().enabled = false;
                GetComponent<Recorder>().enabled = false;
                GetComponent<Speaker>().enabled = false;
                //GetComponent<Photonvo>().enabled = false;
            }
            else {
                GetComponent<AudioSource>().enabled = true;
                GetComponent<Recorder>().enabled = true;
                GetComponent<Speaker>().enabled = true;
            }
        }

    }
}
