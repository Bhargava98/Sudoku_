using BizzyBeeGames;
using System;
using UnityEngine;

// Example script showing how to invoke the Google Mobile Ads Unity plugin.
public class CheatSheet : MonoBehaviour
{
    float accelerometerUpdateInterval = 1.0f / 60.0f;
    // The greater the value of LowPassKernelWidthInSeconds, the slower the
    // filtered value will converge towards current input sample (and vice versa).
    float lowPassKernelWidthInSeconds = 1.0f;
    // This next parameter is initialized to 2.0 per Apple's recommendation,
    // or at least according to Brady! ;)
    float shakeDetectionThreshold = 2.0f;

    float lowPassFilterFactor;
    Vector3 lowPassValue;


    void Start()
    {
        lowPassFilterFactor = accelerometerUpdateInterval / lowPassKernelWidthInSeconds;
        shakeDetectionThreshold *= shakeDetectionThreshold;
        lowPassValue = Input.acceleration;
        show = true;
    }

    public bool show;
    void Update()
    {
        Vector3 acceleration = Input.acceleration;
        lowPassValue = Vector3.Lerp(lowPassValue, acceleration, lowPassFilterFactor);
        Vector3 deltaAcceleration = acceleration - lowPassValue;

        if (deltaAcceleration.sqrMagnitude >= shakeDetectionThreshold)
        {
            show = (show == true) ? false : true;
        }
    }
    public void OnGUI()
    {
        GUIStyle style = new GUIStyle();
               

        Rect requestBannerRect = new Rect(
            50f,
            0.05f * UnityEngine.Screen.height+100,
            100,
            50);

        Rect requestBannerRect2 = new Rect(
            200f,
            0.05f * UnityEngine.Screen.height+100,
            100,
            50);
        if (show)
        {
            if (GUI.Button(requestBannerRect, "Request Coins"))
            {
                CurrencyManager.Instance.Give("coins", 500);
            }

            if (GUI.Button(requestBannerRect2, "Request Hints"))
            {
                CurrencyManager.Instance.Give("hints", 20);
            }
        }
      
    }

   
   
}
