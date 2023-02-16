using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DontDestroyTool : MonoBehaviour
{
	public bool DontDestroyOnLoad = true;
	public bool DontCreateNewWhenBackToThisScene = true;
	public static DontDestroyTool Instance = null;

	void Awake()
	{
		if (Instance != null)
		{
			GameObject.Destroy(this.gameObject);
			return;
		}
		Instance = this;
		if (this.DontDestroyOnLoad)
			GameObject.DontDestroyOnLoad(this);
	}

}