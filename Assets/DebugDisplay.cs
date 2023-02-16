using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugDisplay : MonoBehaviour
{
    Dictionary<string, string> debugLogs = new Dictionary<string, string>();
    public Text display;

    void OnEnable()
    {
        
        Application.logMessageReceived += HandleLog;
    }

    void Ondisable()
    {
        Application.logMessageReceived -= HandleLog;
    }

    void HandleLog(string logString, string stackTrace, LogType type)
    {
        if (type == LogType.Log || type == LogType.Warning || type == LogType.Error)
        {
            string[] splitString = logString.Split(char.Parse(":"));
            string debugKey = splitString[0];
            string debugValue = splitString.Length > 1 ? splitString[1] : "";

            if (debugLogs.ContainsKey(debugKey))
                debugLogs[debugKey] = debugValue;
            else
                debugLogs.Add(debugKey, debugValue);
        }

        string displayText = "";

        foreach (KeyValuePair<string, string> log in debugLogs)
        {
            if (log.Value == "")
                displayText += log.Key + "\n";
            else
                displayText += type + "-" + log.Key + ": " + log.Value + "\n";
        }

        display.text = displayText;
       
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    }


}
