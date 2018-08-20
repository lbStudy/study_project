using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Shiyingfenbianlv : MonoBehaviour {

    public float standard_width = 1920;
    public float standard_height = 1080;
    float device_width = 0;
    float device_heigth = 0;
    float adjustor = 0;
	void Start ()
    {
        device_width = Screen.width;
        device_heigth = Screen.height;
        Debug.Log($"Screen.width {Screen.width} Screen.height{Screen.height}");
        float standard_aspect = standard_width / standard_height;
        float device_aspect = device_width / device_heigth;

        if(device_aspect < standard_aspect)
        {
            adjustor = standard_aspect / device_aspect;
        }
        CanvasScaler canvasScalerTemp = transform.GetComponent<CanvasScaler>();
        if(adjustor == 0)
        {
            canvasScalerTemp.matchWidthOrHeight = 1;
        }
        else
        {
            canvasScalerTemp.matchWidthOrHeight = 0;
        }
    }
	
}
