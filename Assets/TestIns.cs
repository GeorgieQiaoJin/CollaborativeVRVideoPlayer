using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;

namespace Networking.Pun2
{
	public class TestIns : MonoBehaviourPunCallbacks
	{
		public GameObject sphere;
		

		// Start is called before the first frame update
		void Start()
		{
			Debug.Log("TestIns Start");
			
			//PhotonNetwork.AutomaticallySyncScene = true;
		}

		// Update is called once per frame
		void Update()
		{

		}

		private void OnTriggerEnter(Collider other)
		{
			Debug.Log("TestIns Trigger Enter");
			Vector3 startPosition = new Vector3(0, 0, 0);

			//Get the Renderer component from the new cube
			var cubeRenderer = sphere.GetComponent<Renderer>();

			
				//Call SetColor using the shader property name "_Color" and setting the color to red
				cubeRenderer.material.SetColor("_Color", Color.red);
				//GameObject DisplayImage = PhotonNetwork.InstantiateRoomObject("GenericCubeTest", transform.position, transform.rotation, 0);
			

			cubeRenderer.material.SetColor("_Color", Color.green);
		}

		


	}
}
