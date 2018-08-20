using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutLog : MonoBehaviour {

    List<string> mLines = new List<string>();
    public bool isOpen;
    GUIStyle logStyle = new GUIStyle();
    private void Start()
    {
        logStyle.fontSize = 30;
        logStyle.normal.textColor = Color.red;
        if (isOpen)
            Application.logMessageReceived += HandleLog;
    }

    

    private void OnGUI()
    {
        if (mLines.Count == 0)
            return;
        for(int i = 0; i < mLines.Count; i++)
        {
            GUILayout.Label(mLines[i], logStyle);
        }
    }

    void HandleLog(string lonStr, string stackTrace, LogType type)
    {
        if(type == LogType.Error || type == LogType.Exception)
        {
            mLines.Add(lonStr);
            mLines.Add(stackTrace);

            if(mLines.Count > 30)
            {
                mLines.RemoveRange(0, 2);
            }
        }
    }

    
}
