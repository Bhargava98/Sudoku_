//using System.Collections;
//using System.Collections.Generic;
//using System.Threading.Tasks;
//using UnityEngine;
////using Firebase.Analytics;
////using Firebase;
//using UnityEngine.UI;
//using BizzyBeeGames.Sudoku;
//using System;


//public class GameAnalytics : MonoBehaviour
//{
//    public Text info;

//    void Start()
//    {
//#if !UNITY_EDITOR
//        InitializeFirebaseAndStart();
//#endif

//        StartCoroutine(LoadStartAd());       
//    }

//    IEnumerator LoadStartAd()
//    {
//        yield return new WaitForEndOfFrame();
//        AdsManager.Instance.CreateAndLoadRewardedAd();
//        AdsManager.Instance.INT_AtGameStart.LoadInterstitial();

//        AdsManager.Instance.RequestBanner();
//        StartCoroutine(LoadAds());
//    }

//    IEnumerator LoadAds()
//    {
//        yield return new WaitForSeconds(2f);
//        RequestAdsWithDelay();
//    }

//    public static void RequestAdsWithDelay()
//    {       

//        if (AdsManager.Instance != null)
//        {
//           // if (UnityRemoteData.isAds_WinEnabled)
//                AdsManager.Instance.INT_LevelWin.LoadInterstitial();
//            AdsManager.Instance.INT_NewGameClicked.LoadInterstitial();
//            //  if (UnityRemoteData.isAdsnew)


//        }
//    }

//    //void InitializeFirebaseAndStart()
//    //{
//    //    Firebase.DependencyStatus dependencyStatus = Firebase.FirebaseApp.CheckDependencies();

//    //    if (dependencyStatus != Firebase.DependencyStatus.Available)
//    //    {
//    //        Firebase.FirebaseApp.FixDependenciesAsync().ContinueWith(task =>
//    //        {
//    //            dependencyStatus = Firebase.FirebaseApp.CheckDependencies();
//    //            if (dependencyStatus == Firebase.DependencyStatus.Available)
//    //            {
//    //                InitializeFirebaseComponents();
//    //            }
//    //            else
//    //            {
//    //                Debug.LogError(
//    //                    "Could not resolve all Firebase dependencies: " + dependencyStatus);
//    //                //Application.Quit();
//    //            }
//    //        });
//    //    }
//    //    else
//    //    {
//    //        InitializeFirebaseComponents();
//    //    }
//    //}

//    void InitializeFirebaseComponents()
//    {
//        // Crashlytics will use the DefaultInstance, as well;
//        // this ensures that Crashlytics is intitialized.
//        //Firebase.FirebaseApp app = Firebase.FirebaseApp.DefaultInstance;

//        //// this ensures that Remote data is intitialized.
//        //System.Threading.Tasks.Task.WhenAll(
//        //    InitializeRemoteConfig()
//        //  ).ContinueWith(task => { StartGame(); });

//        //// this ensures that Analytics data is intitialized.
//        //InitializeAnalytics();
//    }
//    // Start gathering analytic data.
//    void InitializeAnalytics()
//    {
//        //Firebase.Analytics.FirebaseAnalytics.SetAnalyticsCollectionEnabled(true);
//        ////FirebaseAnalytics.SetUserProperty("Current_Level", PlayerPrefs.GetInt("CurrentUnlockedLevel").ToString());
//        //SetUserPropertyForUnlockedLevel(GameManager.Instance.TotalLevelsDone);

//        //FirebaseAnalytics.SetUserProperty("Purchase_Count", PlayerPrefs.GetInt("PURCHASE_COUNT", 0).ToString());
//        //FirebaseAnalytics.SetUserProperty("IAP_Spend_Value", PlayerPrefs.GetFloat("TOTAL_PURCHASE_PRICE", 0f).ToString());
//    }



//    #region RemoteData
//    // Sets the default values for remote config.  These are the values that will
//    // be used if we haven't fetched yet.
//    //System.Threading.Tasks.Task InitializeRemoteConfig()
//    //{
//    //    Dictionary<string, object> defaults = new Dictionary<string, object>();
//    //    // Defaults Objects:

//    //    defaults.Add(UnityRemoteData.Inters_Ads, true);
//    //    defaults.Add(UnityRemoteData.Full_Ads, true);
//    //    defaults.Add(UnityRemoteData.Ad_Banner, true);
//    //    defaults.Add(UnityRemoteData.Ad_Win_available, true);
//    //    defaults.Add(UnityRemoteData.Ad_Fail_available, true);
//    //    defaults.Add(UnityRemoteData.Ad_AtHomeClicked_available, true);
//    //    defaults.Add(UnityRemoteData.Ad_AtTitle_available, true);
//    //    defaults.Add(UnityRemoteData.Ad_NewGame_available, true);
//    //    defaults.Add(UnityRemoteData.Ad_AtTitlePlayBtn_available, false);
//    //    defaults.Add(UnityRemoteData.Ad_AtGameStart_available, true);
//    //    defaults.Add(UnityRemoteData.Notifications_available, true);
//    //    defaults.Add(UnityRemoteData.NoAdsPanel_available, true);
//    //   // defaults.Add(UnityRemoteData.Ad_AtGameFocused_available, false);

//    //    defaults.Add(UnityRemoteData.Ad_StartLevel, 3);
//    //    defaults.Add(UnityRemoteData.Ad_Win_Counter, 2);
//    //    defaults.Add(UnityRemoteData.Ad_fail_Counter, 2);
//    //    defaults.Add(UnityRemoteData.Ad_HomeCounter, 2);
//    //    defaults.Add(UnityRemoteData.Ad_TitleCounter, 2);
//    //    defaults.Add(UnityRemoteData.Ad_NewGame_Counter, 2);
//    //    defaults.Add(UnityRemoteData.Ad_TitlePlayBtnCounter, 2);
//    //    defaults.Add(UnityRemoteData.Ad_AtGameStart_counter, 2);
//    //   // defaults.Add(UnityRemoteData.Ad_AtGameFocus_Counter, 2);
//    //    //defaults.Add(UnityRemoteData.dailyRewardData, "{\"coins\":[20,50,40,60,15],\"hints\":[2,3,4,5]}");

//    //    Firebase.RemoteConfig.FirebaseRemoteConfig.SetDefaults(defaults);
//    //    return Firebase.RemoteConfig.FirebaseRemoteConfig.FetchAsync(System.TimeSpan.Zero);
//    //}

//    // Actually start the game, once we've verified that everything
//    // is working and we have the firebase prerequisites ready to go.
//    void StartGame()
//    {
//        // Remote Config data has been fetched, so this applies it for this play session:
//        Firebase.RemoteConfig.FirebaseRemoteConfig.ActivateFetched();
//        ///---

//        UnityRemoteData.isFullAdsEnabled = Firebase.RemoteConfig.FirebaseRemoteConfig.GetValue(UnityRemoteData.Full_Ads).BooleanValue;
//        UnityRemoteData.isIntAdsEnabled = Firebase.RemoteConfig.FirebaseRemoteConfig.GetValue(UnityRemoteData.Inters_Ads).BooleanValue;
//        UnityRemoteData.isBannerAdEnabled = Firebase.RemoteConfig.FirebaseRemoteConfig.GetValue(UnityRemoteData.Ad_Banner).BooleanValue;
//        UnityRemoteData.isAds_WinEnabled = Firebase.RemoteConfig.FirebaseRemoteConfig.GetValue(UnityRemoteData.Ad_Win_available).BooleanValue;
//        UnityRemoteData.isAds_FailEnabled = Firebase.RemoteConfig.FirebaseRemoteConfig.GetValue(UnityRemoteData.Ad_Fail_available).BooleanValue;
//        UnityRemoteData.isAds_AtHomeEnabled = Firebase.RemoteConfig.FirebaseRemoteConfig.GetValue(UnityRemoteData.Ad_AtHomeClicked_available).BooleanValue;
//        UnityRemoteData.isAds_AtTitleEnabled = Firebase.RemoteConfig.FirebaseRemoteConfig.GetValue(UnityRemoteData.Ad_AtTitle_available).BooleanValue;
//        UnityRemoteData.isAds_AtTitlePlayBtnEnabled = Firebase.RemoteConfig.FirebaseRemoteConfig.GetValue(UnityRemoteData.Ad_AtTitlePlayBtn_available).BooleanValue;
//        UnityRemoteData.isAds_newGameEnabled = Firebase.RemoteConfig.FirebaseRemoteConfig.GetValue(UnityRemoteData.Ad_NewGame_available).BooleanValue;
//        UnityRemoteData.isAds_AtGameStartEnabled = Firebase.RemoteConfig.FirebaseRemoteConfig.GetValue(UnityRemoteData.Ad_AtGameStart_available).BooleanValue;
//        UnityRemoteData.NotificationsEnabled = Firebase.RemoteConfig.FirebaseRemoteConfig.GetValue(UnityRemoteData.Notifications_available).BooleanValue;
//        UnityRemoteData.NoAdsPanelEnabledAtStart = Firebase.RemoteConfig.FirebaseRemoteConfig.GetValue(UnityRemoteData.NoAdsPanel_available).BooleanValue;
//       // UnityRemoteData.isAds_AtGameFocusEnabled = Firebase.RemoteConfig.FirebaseRemoteConfig.GetValue(UnityRemoteData.Ad_AtGameFocused_available).BooleanValue;

//        UnityRemoteData.adsNewGameCounter = (int)Firebase.RemoteConfig.FirebaseRemoteConfig.GetValue(UnityRemoteData.Ad_NewGame_Counter).DoubleValue;
//        UnityRemoteData.adsWinCounter = (int)Firebase.RemoteConfig.FirebaseRemoteConfig.GetValue(UnityRemoteData.Ad_Win_Counter).DoubleValue;
//        UnityRemoteData.adsFailCounter = (int)Firebase.RemoteConfig.FirebaseRemoteConfig.GetValue(UnityRemoteData.Ad_fail_Counter).DoubleValue;
//        UnityRemoteData.adsStartlevel = (int)Firebase.RemoteConfig.FirebaseRemoteConfig.GetValue(UnityRemoteData.Ad_StartLevel).DoubleValue;
//        UnityRemoteData.adsHomeClickCounter = (int)Firebase.RemoteConfig.FirebaseRemoteConfig.GetValue(UnityRemoteData.Ad_HomeCounter).DoubleValue;
//        UnityRemoteData.adsAtTitleCounter = (int)Firebase.RemoteConfig.FirebaseRemoteConfig.GetValue(UnityRemoteData.Ad_TitleCounter).DoubleValue;
//        UnityRemoteData.adsAtTitlePlayBtnCounter = (int)Firebase.RemoteConfig.FirebaseRemoteConfig.GetValue(UnityRemoteData.Ad_TitlePlayBtnCounter).DoubleValue;
//        UnityRemoteData.adsAtGameStartCounter = (int)Firebase.RemoteConfig.FirebaseRemoteConfig.GetValue(UnityRemoteData.Ad_AtGameStart_counter).DoubleValue;
//       // UnityRemoteData.adsAtGameFocusCounter = (int)Firebase.RemoteConfig.FirebaseRemoteConfig.GetValue(UnityRemoteData.Ad_AtGameFocus_Counter).DoubleValue;


//        //----
//        // UnityRemoteData.isDemoAdsEnabled = Firebase.RemoteConfig.FirebaseRemoteConfig.GetValue(UnityRemoteData.Ads_Demo).BooleanValue;

//        string crossPromoinfo = Firebase.RemoteConfig.FirebaseRemoteConfig.GetValue("CrossPromo").StringValue;
//        if (crossPromoinfo != null) UnityRemoteData.crossPromoData = crossPromoinfo;


//        string dailyreward = Firebase.RemoteConfig.FirebaseRemoteConfig.GetValue("DailyRewardData").StringValue;
//        if (dailyreward != null) UnityRemoteData.dailyRewardData = dailyreward;

//        string rateUs = Firebase.RemoteConfig.FirebaseRemoteConfig.GetValue("Rate_MenuControl").StringValue;
//        if (rateUs != null) UnityRemoteData.rateUsData = rateUs;



       
       
//        //info.text = "ALL - " + Firebase.RemoteConfig.FirebaseRemoteConfig.GetValue(UnityRemoteData.Full_Ads).BooleanValue.ToString() + " \n" +
//        //            "FAIL - " + Firebase.RemoteConfig.FirebaseRemoteConfig.GetValue(UnityRemoteData.Ad_Fail_available).BooleanValue.ToString() + " \n" +
//        //            "WIN - " + Firebase.RemoteConfig.FirebaseRemoteConfig.GetValue(UnityRemoteData.Ad_Win_available).BooleanValue.ToString() + " \n" +
//        //            "Banner - " + Firebase.RemoteConfig.FirebaseRemoteConfig.GetValue(UnityRemoteData.Ad_Banner).BooleanValue.ToString() + " \n" +
//        //            "AdAtHomeBut - " + Firebase.RemoteConfig.FirebaseRemoteConfig.GetValue(UnityRemoteData.Ad_AtHomeClicked_available).BooleanValue.ToString() + " \n" +
//        //            "Interstitial - " + Firebase.RemoteConfig.FirebaseRemoteConfig.GetValue(UnityRemoteData.Inters_Ads).BooleanValue.ToString() + " \n" +
//        //            "Fail Count - " + (int)Firebase.RemoteConfig.FirebaseRemoteConfig.GetValue(UnityRemoteData.Ad_fail_Counter).DoubleValue + " \n" +
//        //            "Win Count - " + (int)Firebase.RemoteConfig.FirebaseRemoteConfig.GetValue(UnityRemoteData.Ad_Win_Counter).DoubleValue + " \n" +
//        //            "Home but Count - " + (int)Firebase.RemoteConfig.FirebaseRemoteConfig.GetValue(UnityRemoteData.Ad_HomeCounter).DoubleValue + " \n" +
//        //            "Start Level - " + (int)Firebase.RemoteConfig.FirebaseRemoteConfig.GetValue(UnityRemoteData.Ad_StartLevel).DoubleValue;

//    }
//    #endregion


//    public static void TrackData(string menu)
//    {
//        FirebaseAnalytics.LogEvent("Open_Screen", "Screen_Name", menu);
//        //Debug.Log("Analytics: Screen_Name= " + menu);
//    }

//    public static void SetUserPropertyForUnlockedLevel(int total_playedLevels)
//    {
//        FirebaseAnalytics.SetUserProperty("Total_Played_Levels", total_playedLevels.ToString());

//        if (total_playedLevels >= 10)
//            Completed_10_Level();

//        // Debug.Log("Analytics: Current Unlocked Level =" + total_playedLevels);  
//    }


//    public static void LevelStart(string levelNum)
//    {
//        FirebaseAnalytics.LogEvent(
//            FirebaseAnalytics.EventLevelStart,
//            new Parameter(FirebaseAnalytics.ParameterLevelName, levelNum));

//        FirebaseAnalytics.SetUserProperty("GameLevel_Progress", "Start");

//        // Debug.Log("Analytics: Level Start id ="+levelNum+"   Current Level: "+(GameManager.Instance.TotalLevelsDone+1));
//        //    FirebaseAnalytics.SetUserProperty("Current_Level", PlayerPrefs.GetInt("CurrentUnlockedLevel").ToString());
//    }

//    public static void LevelEnd(int Scorevalue) //Level Completed
//    {
//        FirebaseAnalytics.LogEvent(
//            FirebaseAnalytics.EventLevelEnd,
//            new Parameter(FirebaseAnalytics.ParameterLevelName, GameManager.Instance.currLevelName),
//            new Parameter(FirebaseAnalytics.ParameterScore, Scorevalue));

//        PlayedDifficulty(GameManager.Instance.currLevelName);
//        FirebaseAnalytics.SetUserProperty("GameLevel_Progress", "Completed");
//        //Debug.Log("Analytics: Level End id =" + GameManager.Instance.ActivePuzzleData.puzzleName + "   Current Level: " + (GameManager.Instance.TotalLevelsDone + 1)+"   Score Value : "+Scorevalue);
//    }

//    public static void LevelEndFail()
//    {
//        if (!String.IsNullOrEmpty(GameManager.Instance.ActivePuzzleData.puzzleName))
//        {
//            FirebaseAnalytics.LogEvent(
//                "level_failed",
//                new Parameter(FirebaseAnalytics.ParameterLevelName, GameManager.Instance.currLevelName));

//            //Debug.Log("Analytics: Level Failed =" + GameManager.Instance.ActivePuzzleData.puzzleName + "   Current Level:" + (GameManager.Instance.TotalLevelsDone + 1));
//        }
//    }

//    //public static void DayRewardBonus(int dayValue)
//    //{
//    //    FirebaseAnalytics.LogEvent("Daily_Bonus", "Claimed_Day", dayValue);
//    //}

//    public static void ButtonTracks(string menu, string buttonName)
//    {
//        FirebaseAnalytics.LogEvent("BTN_" + menu + "_" + buttonName,
//        new Parameter("total_played_levels", GameManager.Instance.TotalLevelsDone));

//    }

//    public static void AdFrequency(int value) //This is Intersticial Ad frequency
//    {
//        FirebaseAnalytics.SetUserProperty("Ad_Frequency", value.ToString());
//        //Debug.Log("Analytics: Ad Frequency =" + value);
//    }

//    public static void AdAnalytics(string id)
//    {
//        FirebaseAnalytics.LogEvent(
//                id,
//                new Parameter(FirebaseAnalytics.ParameterLevelName, GameManager.Instance.TotalLevelsDone));

//    }


//    public static void PurchasedCount(int count, float totalPrice)
//    {
//        //print("PurchaseCount " + "-" + count);
//        FirebaseAnalytics.SetUserProperty("Purchase_Count", count.ToString());
//        FirebaseAnalytics.SetUserProperty("IAP_Spend_Value", totalPrice.ToString());

//        FirebaseAnalytics.LogEvent("Purchase_Data",
//            new Parameter("Purchased_Count", count),
//            new Parameter("IAP_Spend_Value", totalPrice));
//    }

//    public static void PurchasedFailedCount(int count)
//    {
       
//        FirebaseAnalytics.SetUserProperty("Purchase_Fail_Count", count.ToString());

              
//    }

//    public static void PlayedDifficulty(string type)
//    {
//        string currDiff = "";

//        if (type.Contains("easy"))
//            currDiff = "easy";
//        else if (type.Contains("medium"))
//            currDiff = "medium";
//        else if (type.Contains("hard"))
//            currDiff = "hard";
//        else if (type.Contains("expert"))
//            currDiff = "expert";

       

//        PlayerPrefs.SetInt("PLAYED_LEVEL_" + currDiff, PlayerPrefs.GetInt("PLAYED_LEVEL_" + currDiff, 0) + 1);
//        FirebaseAnalytics.SetUserProperty("Played_Game_Mode_"+ currDiff, PlayerPrefs.GetInt("PLAYED_LEVEL_" + currDiff).ToString());
//    }

//    public static void Completed_5_days(int totalLevels)
//    {
//        FirebaseAnalytics.LogEvent("Played_5_Days",
//            new Parameter(FirebaseAnalytics.ParameterLevelName, "Levels_" + totalLevels.ToString()),
//            new Parameter(FirebaseAnalytics.ParameterLevel, totalLevels));

//        // Debug.Log("Analytics: Player For 5 days");
//    }

//    public static void TotalPlayedDays(int days)//Updated new
//    {
//        FirebaseAnalytics.SetUserProperty("Total_played_days", days.ToString());
       
//    }
    

//    public static void ConsistentDays(int dayValue)//Updated new
//    {
//        FirebaseAnalytics.SetUserProperty("Consistent_days", dayValue.ToString());

//       // Debug.Log("Consistent_days--> " + dayValue);
//    }

//    public static void Completed_10_Level()
//    {
//        FirebaseAnalytics.LogEvent("Completed_10_Levels",
//            new Parameter(FirebaseAnalytics.ParameterLevelName, "Completed"));
//    }

//    public static void SpendVirtualCurrency(string item_Name, int quantity, string Currencytype, int value)
//    {
//        float priceValue = value * 0.099f;
//        FirebaseAnalytics.LogEvent(FirebaseAnalytics.EventSpendVirtualCurrency,
//        new Parameter(FirebaseAnalytics.ParameterItemName, item_Name),
//        new Parameter(FirebaseAnalytics.ParameterQuantity, quantity),
//        new Parameter(FirebaseAnalytics.ParameterVirtualCurrencyName, Currencytype),
//        new Parameter(FirebaseAnalytics.ParameterValue, priceValue));
//        // new Parameter(FirebaseAnalytics.ParameterPrice, priceValue),
//        // new Parameter(FirebaseAnalytics.ParameterCurrency, "USD")

//        // Debug.Log("Analytics: VirtualCurrency Spend = "+value);
//    }

//    public static void EarnVirtualCurrency(string item_Name, int quantity)
//    {
//        FirebaseAnalytics.LogEvent(FirebaseAnalytics.EventEarnVirtualCurrency,
//           new Parameter(FirebaseAnalytics.ParameterVirtualCurrencyName, "Coins"),
//           new Parameter(FirebaseAnalytics.ParameterItemName, item_Name),
//           new Parameter(FirebaseAnalytics.ParameterValue, quantity));

//        //Debug.Log("Analytics: VirtualCurrency Earned = " + quantity);
//    }



//    public static void TutorialBegin(int levelNum)//newUpdate
//    {
//        FirebaseAnalytics.LogEvent(FirebaseAnalytics.EventTutorialBegin,
//        new Parameter(FirebaseAnalytics.ParameterLevel, levelNum));

//    }
//    //Not integrated in Sudoku yet
//    public static void TutorialComplete(int levelNum)//newUpdate
//    {
//        FirebaseAnalytics.LogEvent(FirebaseAnalytics.EventTutorialComplete,
//        new Parameter(FirebaseAnalytics.ParameterLevel, levelNum));
//    }



//    public static void LevelAdRewardedAds(string levelNum)
//    {
//        FirebaseAnalytics.LogEvent("LevelAdRewardedAds",
//            new Parameter(FirebaseAnalytics.ParameterLevelName, levelNum.ToString()),
//            new Parameter(FirebaseAnalytics.ParameterLevel, GameManager.Instance.TotalLevelsDone));
//    }

//    public static void LevelAdImpressions(string levelNum)
//    {
//        FirebaseAnalytics.LogEvent("LevelAdImpressions",
//            new Parameter(FirebaseAnalytics.ParameterLevelName, levelNum.ToString()),
//            new Parameter(FirebaseAnalytics.ParameterLevel, GameManager.Instance.TotalLevelsDone));
//    }



//}
