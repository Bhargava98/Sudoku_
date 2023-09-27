using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableButtonForNoADS : MonoBehaviour
{
    public GameObject ButtonUI;
    // Start is called before the first frame update
    void OnEnable()
    {
        if (!UnityRemoteData.isFullAdsEnabled || (Application.internetReachability == NetworkReachability.NotReachable))
        {            
            ButtonUI.SetActive(false);
        }
        else
        {
            //if (AdsManager.Instance.isVideoAdAvailable())
            //    ButtonUI.SetActive(true);
        }
    }

}
