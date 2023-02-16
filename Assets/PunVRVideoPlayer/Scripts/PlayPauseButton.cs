using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Networking.Pun2
{
	public class PlayPauseButton : MonoBehaviour
	{
		public GameObject VideoControl;
	    // Start is called before the first frame update
	    void Start()
	    {
	        
	    }

	    // Update is called once per frame
	    void Update()
	    {
	        
	    }

	    private void OnTriggerEnter(Collider other)
	    {
	    	VideoControl.GetComponent<VideoControl_Sync>().SwitchPlayPause();
	    }
	}
}
