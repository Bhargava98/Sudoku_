//using System;
//using UnityEngine;
//using GoogleMobileAds.Api;
//using BizzyBeeGames;
//using BizzyBeeGames.Sudoku;
//using System.Collections.Generic;
//using System.Collections;

//public class AdsManager : MonoBehaviour
//{
//    public static AdsManager Instance;

//    public int freeCoinsForAdWatch = 100;
//    public int freeHintsForAdWatch = 1;

//    [HideInInspector] public int showAds;

//    private BannerView bannerView;
//    //private InterstitialAd interstitial;
//    private RewardedAd rewardedAd;
//    private static string outputMessage = string.Empty;

//    [Header("Interstial Ads")]
//    public string adLevelWinID;
//    //public string adLevelFailID;
//    //public string adHomeClickedID;
//    //public string adToTitleID;
//    //public string adTitlePlayClickedID;
//    public string adNewGameClickedID;
//    public string adAtGameStartID;
//    //public string adAfterGameFocused;

//    #region
//    [HideInInspector] public Interstitial_Units INT_LevelWin;
//    //[HideInInspector] public Interstitial_Units INT_LevelFail;
//    //[HideInInspector] public Interstitial_Units INT_HomeClicked;
//    //[HideInInspector] public Interstitial_Units INT_ToTitlePage;
//    //[HideInInspector] public Interstitial_Units INT_TitlePlayClicked;
//    [HideInInspector] public Interstitial_Units INT_NewGameClicked;
//    [HideInInspector] public Interstitial_Units INT_AtGameStart;
//    //[HideInInspector] public Interstitial_Units INT_AfterFocused;
//    #endregion
//    //CURRENT REWARD DATA
//    private object[] inData;
//    private int showIndex;
//    private int requestIndex;

//    /// <summary>---------------------------------------------------------------------
//    /// Original Ad ids
//    /// </summary>
//    //private string appIdAndroid = "ca-app-pub-1092248466701784~4519737274";
//    //private string appIdiOS = "ca-app-pub-1092248466701784~7837516827";


//    //private string adAndroidBanner = "ca-app-pub-1092248466701784/7385990020";
//    //private string adiOSBanner = "ca-app-pub-1092248466701784/8757769685";


//    ////private string adAndroidInterst = "ca-app-pub-1092248466701784/6762757236";
//    //private string adiOSInterst = "ca-app-pub-1092248466701784/3917102044";

//    //private string adAndroidReward = "ca-app-pub-1092248466701784/9972725570";
//    //private string adiOSReward = "ca-app-pub-1092248466701784/6141291773";
//    ///////////------------------------------------------------------------------------


//    ///// <summary>---------------------------------------------------------------------
//    ///// Demo ad ids
//    ///// </summary>
//    private string appIdAndroid = "ca-app-pub-3940256099942544~3347511713";
//    private string appIdiOS = "ca-app-pub-3940256099942544~1458002511";


//    private string adAndroidBanner = "ca-app-pub-3940256099942544/6300978111";
//    private string adiOSBanner = "ca-app-pub-3940256099942544/2934735716";


//    private string adAndroidInterst = "ca-app-pub-3940256099942544/1033173712";
//    private string adiOSInterst = "ca-app-pub-3940256099942544/4411468910";

//    private string adAndroidReward = "ca-app-pub-3940256099942544/5224354917";
//    private string adiOSReward = "ca-app-pub-3940256099942544/1712485313";
//    /////////////------------------------------------------------------------------------


//    public static string OutputMessage
//    {
//        set { outputMessage = value; }
//    }

//    public void Awake()
//    {
//        if (Instance == null)
//            Instance = this;
//        else
//        {
//            if (Instance != this)
//                Instance = this;
//        }

//        INT_LevelWin.ad_interstitialID = adLevelWinID;
//        //INT_LevelFail.ad_interstitialID = adLevelFailID;
//        //INT_HomeClicked.ad_interstitialID = adHomeClickedID;
//        //INT_ToTitlePage.ad_interstitialID = adToTitleID;
//        //INT_TitlePlayClicked.ad_interstitialID = adTitlePlayClickedID;
//        INT_NewGameClicked.ad_interstitialID = adNewGameClickedID;
//        INT_AtGameStart.ad_interstitialID = adAtGameStartID;
//        //INT_AfterFocused.ad_interstitialID = adAfterGameFocused;


//        showAds = PlayerPrefs.GetInt("SHOW_ADS", 0);



//#if UNITY_ANDROID
//        string appId = appIdAndroid;
//#elif UNITY_IPHONE
//        string appId = appIdiOS;
//#else
//        string appId = "unexpected_platform";
//#endif

//        MobileAds.SetiOSAppPauseOnBackground(true);

//        // Initialize the Google Mobile Ads SDK.
//        //MobileAds.Initialize(appId);

//        MobileAds.Initialize((initStatus) =>
//        {
//            Dictionary<string, AdapterStatus> map = initStatus.getAdapterStatusMap();
//            foreach (KeyValuePair<string, AdapterStatus> keyValuePair in map)
//            {
//                string className = keyValuePair.Key;
//                AdapterStatus status = keyValuePair.Value;
//                switch (status.InitializationState)
//                {
//                    case AdapterState.NotReady:
//                        // The adapter initialization did not complete.
//                        MonoBehaviour.print("Adapter: " + className + " not ready.");
//                        break;
//                    case AdapterState.Ready:
//                        // The adapter was successfully initialized.
//                        MonoBehaviour.print("Adapter: " + className + " is initialized.");
//                        break;
//                }
//            }
//        });

//    }


//    public void RemoveAdsValueSet()
//    {
//        if (!PlayerPrefs.HasKey("SHOW_ADS") || PlayerPrefs.GetInt("SHOW_ADS", 0) == 0)
//        {
//            DestroyBanner();
//            PlayerPrefs.SetInt("SHOW_ADS", 1);
//            showAds = PlayerPrefs.GetInt("SHOW_ADS", 0);
//        }
//    }


//    // Returns an ad request with custom ad targeting.
//    private AdRequest CreateAdRequest()
//    {
//        return new AdRequest.Builder()
//            // .AddTestDevice(AdRequest.TestDeviceSimulator)
//            .AddKeyword("Sudoku")
//            .Build();
//    }

//    public void RequestBanner()
//    {
//        if (showAds == 1) return;
//        if (!UnityRemoteData.isFullAdsEnabled || !UnityRemoteData.isBannerAdEnabled)
//            return;
//        //if ((GameManager.Instance.TotalLevelsDone < UnityRemoteData.adsStartlevel))
//        //    return;

//        // These ad units are configured to always serve test ads.
//#if UNITY_EDITOR
//        string adUnitId = "unused";
//#elif UNITY_ANDROID
//        string adUnitId = adAndroidBanner;
//#elif UNITY_IPHONE
//        string adUnitId = adiOSBanner;
//#else
//        string adUnitId = "unexpected_platform";
//#endif

//        // Clean up banner ad before creating a new one.
//        if (this.bannerView != null)
//        {
//            this.bannerView.Destroy();
//        }

//        // Create a 320x50 banner at the bottom of the screen.
//        //this.bannerView = new BannerView(adUnitId, AdSize.SmartBanner, AdPosition.Bottom);

//        //// Register for ad events.
//        //this.bannerView.OnAdLoaded += this.HandleAdLoaded;
//        //this.bannerView.OnAdFailedToLoad += this.HandleAdFailedToLoad;
//        //this.bannerView.OnAdOpening += this.HandleAdOpened;
//        //this.bannerView.OnAdClosed += this.HandleAdClosed;
//        //this.bannerView.OnAdLeavingApplication += this.HandleAdLeftApplication;

//        // Load a banner ad.
//        this.bannerView.LoadAd(this.CreateAdRequest());
//        bannerView.Hide();
//    }
//    public void DestroyBanner()
//    {
//        if (showAds == 1) return;
//        //if (!UnityRemoteData.isFullAdsEnabled || !UnityRemoteData.isBannerAdEnabled)
//        //    return;
//        //if ((GameManager.Instance.TotalLevelsDone < UnityRemoteData.adsStartlevel))
//        //    return;

//        if (this.bannerView != null)
//        {
//            this.bannerView.Destroy();
//        }
//    }

//    public void ShowBanner(out bool isLoaded)
//    {
//        isLoaded = false;
//        if (Application.internetReachability == NetworkReachability.NotReachable)
//        {
//            isLoaded = false;
//            return;
//        }


//        if (showAds == 1)
//        {
//            isLoaded = false;
//            return;
//        }

//        if (this.bannerView != null && UnityRemoteData.isFullAdsEnabled && UnityRemoteData.isBannerAdEnabled)
//        {
//            bannerView.Show();
//            isLoaded = true;
//        }

//        if (this.bannerView == null)
//        {
//            RequestBanner();
//            isLoaded = false;
//        }
//    }

//    public void HideBanner()
//    {
//        if (this.bannerView != null)
//            bannerView.Hide();
//    }
   
//    public void CreateAndLoadRewardedAd()
//    {

//        if (!UnityRemoteData.isFullAdsEnabled)
//            return;
        
//#if UNITY_EDITOR
//        string adUnitId = "unused";
//#elif UNITY_ANDROID
//        string adUnitId = adAndroidReward;
//#elif UNITY_IPHONE
//        string adUnitId = adiOSReward;
//#else
//        string adUnitId = "unexpected_platform";
//#endif

//        if (this.rewardedAd == null || !isVideoAdAvailable())
//        {
            
//            // Create new rewarded ad instance.
//            //this.rewardedAd = new Rewarded(adUnitId);

//            //// Called when an ad request has successfully loaded.
//            //this.rewardedAd.OnAdLoaded += HandleRewardedAdLoaded;
//            //// Called when an ad request failed to load.
//            //this.rewardedAd.OnAdFailedToLoad += HandleRewardedAdFailedToLoad;
//            //// Called when an ad is shown.
//            //this.rewardedAd.OnAdOpening += HandleRewardedAdOpening;
//            //// Called when an ad request failed to show.
//            //this.rewardedAd.OnAdFailedToShow += HandleRewardedAdFailedToShow;
//            //// Called when the user should be rewarded for interacting with the ad.
//            //this.rewardedAd.OnUserEarnedReward += HandleUserEarnedReward;
//            //// Called when the ad is closed.
//            //this.rewardedAd.OnAdClosed += HandleRewardedAdClosed;

//            //// Create an empty ad request.
//            //AdRequest request = this.CreateAdRequest();
//            //// Load the rewarded ad with the request.
//            //this.rewardedAd.LoadAd(request);
            
//        }


//    }

   


//    [System.Serializable]
//    public class Interstitial_Units
//    {
//        private InterstitialAd interstitialAD;
        
//        public string ad_interstitialID = "";
//        private bool isInterstitial_loaded = false;

       

//        public bool IsInitLoaded()
//        {
//            if (Application.internetReachability == NetworkReachability.NotReachable)
//            {
//                //GameManager.interstitial_Controller = false;
//                return false;
//            }

//            if (Instance.showAds == 1) return false;
//            if (!UnityRemoteData.isFullAdsEnabled || !UnityRemoteData.isIntAdsEnabled)
//                return false;
//            if ((GameManager.Instance.TotalLevelsDone < UnityRemoteData.adsStartlevel))
//                return false;
//            if(interstitialAD!=null)
//            if (interstitialAD.CanShowAd())
//            {
//                return true;
//            }
//            return false;
//        }

//        public void LoadInterstitial()
//        {
//            if (AdsManager.Instance.showAds == 1) return;
//            if (!UnityRemoteData.isFullAdsEnabled && !UnityRemoteData.isIntAdsEnabled)
//                return;

//            //These ad units are configured to always serve test ads.
//#if UNITY_EDITOR
//            string adUnitId = "unused";
//#elif UNITY_ANDROID
//        string adUnitId = ad_interstitialID;
//#elif UNITY_IPHONE
//        string adUnitId = ad_interstitialID;
//#else
//        string adUnitId = "unexpected_platform";
//#endif

//            if (!isInterstitial_loaded)//&& PlayerPrefs.GetInt("AgePopup", 0) != 0)
//            {
//                // Clean up interstitial ad before creating a new one.
//                if (this.interstitialAD != null)
//                {
//                    this.interstitialAD.Destroy();
//                }

//                //// Create an interstitial.
//                //this.interstitialAD = new InterstitialAd(adUnitId);

//                //// Register for ad events.
//                //this.interstitialAD.OnAdLoaded += this.HandleInterstitialLoaded;
//                //this.interstitialAD.OnAdFailedToLoad += this.HandleInterstitialFailedToLoad;
//                //this.interstitialAD.OnAdOpening += this.HandleInterstitialOpened;
//                //this.interstitialAD.OnAdClosed += this.HandleInterstitialClosed;
//                //this.interstitialAD.OnAdLeavingApplication += this.HandleInterstitialLeftApplication;

//                //// Load an interstitial ad.
//                //this.interstitialAD.LoadAd(Instance.CreateAdRequest());
//                //GameAnalytics.AdAnalytics("AD_LOAD_INT");
//            }


//        }

//        public void ShowInterstitial()
//        {
//            if (Application.internetReachability == NetworkReachability.NotReachable)
//            {
//                //GameManager.interstitial_Controller = false;
//                return;
//            }

//            if (Instance.showAds == 1) return;
//            if (!UnityRemoteData.isFullAdsEnabled || !UnityRemoteData.isIntAdsEnabled)
//                return;
//            if ((GameManager.Instance.TotalLevelsDone < UnityRemoteData.adsStartlevel))
//                return;


//            if (this.interstitialAD != null && interstitialAD.CanShowAd())
//            {
                
//                interstitialAD.Show();
//                GameManager.interstitial_Controller = true;
//               // Debug.LogError("Showing ad!!");
//                //GameAnalytics.LevelAdImpressions(GameManager.Instance.currLevelName);
//                //GameAnalytics.AdAnalytics("AD_SHOW_INT");
//                //showIndex++; //Debug.Log("Show Interstitial Index :" + showIndex);

//                if ((GameManager.Instance.winCounter >= UnityRemoteData.adsWinCounter))
//                    GameManager.Instance.winCounter = 0;
//                if ((GameManager.Instance.failCounter >= UnityRemoteData.adsFailCounter))
//                    GameManager.Instance.failCounter = 0;
//                if ((GameManager.Instance.homeAdCounter >= UnityRemoteData.adsHomeClickCounter))
//                    GameManager.Instance.homeAdCounter = 0;
//                if ((GameManager.Instance.titleAdCounter >= UnityRemoteData.adsAtTitleCounter))
//                    GameManager.Instance.titleAdCounter = 0;
//                if ((GameManager.Instance.titlePlayBtnCounter >= UnityRemoteData.adsAtTitlePlayBtnCounter))
//                    GameManager.Instance.titlePlayBtnCounter = 0;
//                if ((GameManager.Instance.newGameCounter >= UnityRemoteData.adsNewGameCounter))
//                    GameManager.Instance.newGameCounter = 0;
//                if (PlayerPrefs.GetInt(UnityRemoteData.Ad_AtGameStart_counter, 0) >= UnityRemoteData.adsAtGameStartCounter)
//                    PlayerPrefs.SetInt(UnityRemoteData.Ad_AtGameStart_counter, 0);
//                //Debug.LogError("Showing Intersitial");
//            }
//            else
//            {
//                GameManager.interstitial_Controller = false;
//               // Debug.LogError("Interstitial is not ready yet");
//                LoadInterstitial();
//            }
//        }


//        #region Interstitial callback handlers

//        public void HandleInterstitialLoaded(object sender, EventArgs args)
//        {
//           // Debug.LogError("Init Loaded");
//            isInterstitial_loaded = true;
//            MonoBehaviour.print("HandleInterstitialLoaded event received");
//        }

//        public void HandleInterstitialFailedToLoad(object sender, AdFailedToLoadEventArgs args)
//        {
//          //  Debug.LogError("Init failed to load");
//            isInterstitial_loaded = false;
//            //MonoBehaviour.print(
//            //    "HandleInterstitialFailedToLoad event received with message: " + args.Message);
//            //if (Application.internetReachability != NetworkReachability.NotReachable)
//            //{
//            //    RequestInterstitial();
//            //}
//        }

//        public void HandleInterstitialOpened(object sender, EventArgs args)
//        {
//            //Debug.LogError("Init Opened");
//            MonoBehaviour.print("HandleInterstitialOpened event received");
//            isInterstitial_loaded = false;
//            LoadInterstitial();
//            // GameManager.interstitial_Controller = false;
//        }

//        public void HandleInterstitialClosed(object sender, EventArgs args)
//        {
//            MonoBehaviour.print("HandleInterstitialClosed event received");
//            //Debug.LogError("Init closed");
            
//            PlayerPrefs.SetInt("AD_FREQUENCY", PlayerPrefs.GetInt("AD_FREQUENCY", 0) + 1);
//            //GameAnalytics.AdFrequency(PlayerPrefs.GetInt("AD_FREQUENCY", 0));
//            //GameAnalytics.AdAnalytics("AD_CLOSE_INT");
//            GameManager.interstitial_Controller = false;

//        }

//        public void HandleInterstitialLeftApplication(object sender, EventArgs args)
//        {
//            MonoBehaviour.print("HandleInterstitialLeftApplication event received");
//            GameManager.interstitial_Controller = false;
//        }

//        #endregion

       
//    }




//    public void ShowRewardedAd(object[] data)
//    {
//        if (Application.internetReachability == NetworkReachability.NotReachable)
//            return;

//        if (!UnityRemoteData.isFullAdsEnabled)
//        {
//            PopupManager.Instance.CustomNativePopup("Not Loaded!", "Rewarded ad is not ready yet");
//           // Debug.LogError("Not Ready Yet! Native Popup must show");
//        }

//        if (rewardedAd != null)
//        {
//            if (rewardedAd.CanShowAd())
//            {
//                if (!UnityRemoteData.isFullAdsEnabled)
//                {
//                    PopupManager.Instance.NoInternetNativePopup("Not Loaded!", "Rewarded ad is not ready yet");
                   
//                    return;
//                }
                
//                inData = data;

//                if (inData[0].ToString() == "spinMultiplier")
//                {
//                    GameManager.Instance.isspinTwoX = true;
//                }

//                Debug.LogError("Show called");
//                //this.rewardedAd.Show();
//                //GameAnalytics.AdAnalytics("AD_START_RWD");
//                //GameAnalytics.LevelAdRewardedAds(GameManager.Instance.currLevelName);
//            }
//            else
//            {
//                this.CreateAndLoadRewardedAd();
//                PopupManager.Instance.CustomNativePopup("Not Loaded!", "Rewarded ad is not ready yet");
//               // Debug.LogError("NoLoaded  reward!");
                
//                //MonoBehaviour.print("Rewarded ad is not ready yet");
//            }
//        }
//    }

//    #region Banner callback handlers

//    public void HandleAdLoaded(object sender, EventArgs args)
//    {
//        MonoBehaviour.print("HandleAdLoaded event received");
//    }

//    public void HandleAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
//    {
//        //MonoBehaviour.print("HandleFailedToReceiveAd event received with message: " + args.Message);
//    }

//    public void HandleAdOpened(object sender, EventArgs args)
//    {
//        MonoBehaviour.print("HandleAdOpened event received");
//    }

//    public void HandleAdClosed(object sender, EventArgs args)
//    {
//        MonoBehaviour.print("HandleAdClosed event received");
//    }

//    public void HandleAdLeftApplication(object sender, EventArgs args)
//    {
//        MonoBehaviour.print("HandleAdLeftApplication event received");
//    }

//    #endregion

   

//    #region RewardedAd callback handlers

//    public bool isVideoAdAvailable()
//    {
//        if (rewardedAd == null) return false;

//        return this.rewardedAd.CanShowAd();
//    }

//    public void HandleRewardedAdLoaded(object sender, EventArgs args)
//    {
//        MonoBehaviour.print("HandleRewardedAdLoaded event received");
        
//    }

//    public void HandleRewardedAdFailedToLoad(object sender, AdErrorEventArgs args)
//    {
//        MonoBehaviour.print(
//            "HandleRewardedAdFailedToLoad event received with message: " + args.Message);
//         //Debug.LogError("Failed to load  reward!");
//        if (Application.internetReachability != NetworkReachability.NotReachable)
//            this.CreateAndLoadRewardedAd();
//    }

//    public void HandleRewardedAdOpening(object sender, EventArgs args)
//    {
//        MonoBehaviour.print("HandleRewardedAdOpening event received");
//        //Debug.LogError("open  reward!");
//        this.CreateAndLoadRewardedAd();
//    }

//    public void HandleRewardedAdFailedToShow(object sender, AdErrorEventArgs args)
//    {
//        MonoBehaviour.print(
//            "HandleRewardedAdFailedToShow event received with message: " + args.Message);
//        //Debug.LogError("Failed to show  reward!");
//        this.CreateAndLoadRewardedAd();
//    }

//    public void HandleRewardedAdClosed(object sender, EventArgs args)
//    {
//        MonoBehaviour.print("HandleRewardedAdClosed event received");
//        PlayerPrefs.SetInt("AD_FREQUENCY", PlayerPrefs.GetInt("AD_FREQUENCY", 0) + 1);
//        //GameAnalytics.AdFrequency(PlayerPrefs.GetInt("AD_FREQUENCY", 0));
//        //GameAnalytics.AdAnalytics("AD_CLOSED_RWD");
//        //this.CreateAndLoadRewardedAd();
//    }

//    public void HandleUserEarnedReward(object sender, Reward args)
//    {
//        string type = args.Type;
//        double amount = args.Amount;

//        StartCoroutine(DelayedReward((int)amount));

//        MonoBehaviour.print(
//            "HandleRewardedAdRewarded event received for "
//                        + amount.ToString() + " " + type);
//        // this.CreateAndLoadRewardedAd();
//    }


//    IEnumerator DelayedReward(int amount)
//    {
//        yield return new WaitForSeconds(0.2f);
//        if (inData[0].ToString() == "continue_game")
//        {
//            GameManager.Instance.WatchedTheAd();
//        }
//        else if (inData[0].ToString() == "spinMultiplier")
//        {
//            if (GameManager.Instance.isspinTwoX)
//                FortuneWheelManager.Instance.AfterWatchedAd();
//        }
//        else if (inData[0].ToString() == "dailyMultiplier")
//        {
//            DailyRewardsManager.Instance.AfterWatchedAd();
//        }
//        else if (inData[0].ToString() == "freeCoinsPanel")
//        {
//            freeCoinsForAdWatch = amount;
//            GameManager.Instance.AfterFreeCoinsPanelAdWatch();
//        }
//        else
//        {
//            if (inData[1].ToString() == "Adcoins")
//            {
//                freeCoinsForAdWatch = amount;
//            }

//            PopupManager.Instance.Show("rewarded", inData);
//        }
//        //GameAnalytics.AdAnalytics("AD_COMPLETE_RWD");
       
//    }

//    #endregion

//}
