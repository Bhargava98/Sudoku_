using System.Collections;
using System.Collections.Generic;
using UnityEngine.Analytics;
using UnityEngine;
using RS = UnityEngine.RemoteSettings;

public class UnityRemoteData : MonoBehaviour
{

    // public static bool vaulesUpdated = false;
    //This are keys for remote settings

    public static string Full_Ads = "All_ADS";
    public static string Inters_Ads = "All_Interstitial";
    public static string Ad_Banner = "Ad_Banners";
    //public static string Ads_Demo = "Ads_Demo"; 

    public static string Ad_NewGame_available = "Ads_At_NewLevelStart";
    public static string Ad_Win_available = "Ads_At_LevelWin";
    public static string Ad_Fail_available = "Ads_At_LevelFail";
    public static string Ad_AtHomeClicked_available = "Ad_AtHomeClicked_available";
    public static string Ad_AtTitle_available = "Ad_AtTitle_available";
    public static string Ad_AtTitlePlayBtn_available = "Ad_AtTitlePlayBtn_available";
    public static string Ad_AtGameStart_available = "Ad_AtGameStart_available";
    public static string Ad_AtGameFocused_available = "Ad_AtMinimize_available";
    public static string Notifications_available = "localnotifications";
    public static string NoAdsPanel_available = "NoAdsPanelAtStart_available";
    //public static string Ad_Select_Mode = "Ad_Select_Mode";
    //public static string Ad_Restart_Game = "Ad_Restart_Game";
    //public static string Ad_Game_Over = "Ad_Game_Over";

    public static string Ad_NewGame_Counter = "Ad_NewLevel_Counter";
    public static string Ad_Win_Counter = "Ad_LevelWin_Counter";
    public static string Ad_fail_Counter = "Ad_LevelFail_Counter";
    public static string Ad_StartLevel = "Ad_StartLevel";
    public static string Ad_HomeCounter = "Ad_GoHome_Counter";
    public static string Ad_TitleCounter = "Ad_TitleCounter";
    public static string Ad_TitlePlayBtnCounter = "Ad_TitlePlayBtnCounter";
    public static string Ad_AtGameStart_counter = "Ad_AtGameStart_counter";
    public static string Ad_AtGameFocus_Counter = "Ad_AtMinimize_Counter";

    //=====================================================//

    public static bool isIntAdsEnabled = true;
    public static bool isBannerAdEnabled = true;
    public static bool isFullAdsEnabled = true;
    //public static bool isDemoAdsEnabled = true;

    public static bool isAds_newGameEnabled = true;
    public static bool isAds_WinEnabled = true;
    public static bool isAds_FailEnabled = true;
    public static bool isAds_AtHomeEnabled = true;
    public static bool isAds_AtTitleEnabled = true;
    public static bool isAds_AtTitlePlayBtnEnabled = true;
    public static bool isAds_AtGameStartEnabled = true;
    public static bool isAds_AtGameFocusEnabled = false;
    public static bool NotificationsEnabled = true;
    public static bool NoAdsPanelEnabledAtStart = true;

    //public static bool isAds_forFamily = true;

    public static int adsWinCounter = 2;
    public static int adsNewGameCounter = 2;
    public static int adsFailCounter = 2;
    public static int adsStartlevel = 2;

    public static int adsHomeClickCounter = 2;
    public static int adsAtTitleCounter = 2;
    public static int adsAtTitlePlayBtnCounter = 2;
    public static int adsAtGameStartCounter = 2;
    public static int adsAtGameFocusCounter = 3;
    public static string crossPromoData = null;
    public static string dailyRewardData = null;
    public static string rateUsData = null;
}
