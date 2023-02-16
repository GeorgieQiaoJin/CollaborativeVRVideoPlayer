using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

namespace Networking.Pun2
{
    public class SetMC : MonoBehaviourPunCallbacks
    {
        [SerializeField]
        public Button BTN_ss;

        // Start is called before the first frame update
        void Start()
        {
            if (PhotonNetwork.IsMasterClient)
            {
                BTN_ss.GetComponent<CanvasGroup>().alpha = 1;
            }
            else
            {
                BTN_ss.GetComponent<CanvasGroup>().alpha = 0;
            }
            //PhotonNetwork.SetMasterClient(PhotonNetwork.LocalPlayer);
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}