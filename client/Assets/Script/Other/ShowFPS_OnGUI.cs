using UnityEngine;
using System.Collections;

public class ShowFPS_OnGUI : MonoBehaviour
{
    [SerializeField]
    public float fpsMeasuringDelta = 2.0f;
    [SerializeField]
    private bool IsShow = true;
    private float timePassed;
    private int m_FrameCount = 0;
    private float m_FPS = 0.0f;

    private void Awake()
    {
        Application.targetFrameRate = 60;
    }
    private void Start()
    {
        timePassed = 0.0f;
    }

    private void Update()
    {
        if(IsShow)
        {
            if (Time.deltaTime > 0.1f)
                Debug.Log($" Time.deltaTime {Time.deltaTime}");

            m_FrameCount = m_FrameCount + 1;
            timePassed = timePassed + Time.deltaTime;

            if (timePassed > fpsMeasuringDelta)
            {
                m_FPS = m_FrameCount / timePassed;

                timePassed = 0.0f;
                m_FrameCount = 0;
            }
        }
    }

    private void OnGUI()
    {
        if(IsShow)
        {
            GUIStyle bb = new GUIStyle();
            bb.normal.background = null;    //这是设置背景填充的
            bb.normal.textColor = new Color(1.0f, 0.5f, 0.0f);   //设置字体颜色的
            bb.fontSize = 40;       //当然，这是字体大小

            //居中显示FPS
            GUI.Label(new Rect((Screen.width / 2) - 40, 0, 200, 200), "FPS: " + m_FPS, bb);
        }
    }
}