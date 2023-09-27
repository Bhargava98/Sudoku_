using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using BizzyBeeGames.Sudoku;

public class DailyChangeScript : MonoBehaviour {
	public static DailyChangeScript Instance { get; private set; }

	private int chstartDay = 0;
	private int dayOfChallenge = 0;
    public bool firstTime = false;
	//public Text chalengeTimmer;

	// Use this for initialization
	void Start () 
	{
        PlayerPrefs.SetInt(UnityRemoteData.Ad_AtGameStart_counter, PlayerPrefs.GetInt(UnityRemoteData.Ad_AtGameStart_counter, 0) + 1);

        Instance = this;
        if (!PlayerPrefs.HasKey("OnLoadData") || PlayerPrefs.GetInt("OnLoadData") == 0)
        {
            PlayerPrefs.SetInt("OnLoadData", 1);
            firstTime = true;
            PlayerPrefs.SetInt("chstartDay", System.DateTime.Now.DayOfYear);
            PlayerPrefs.GetInt("Initial_Rate_Appear", 0);
            //PlayerPrefs.SetInt("DailyRewardClaim", 0);
            // PlayerPrefs.SetInt();
        }

        if (getDayCount(PlayerPrefs.GetInt("chstartDay")) >= 1)
        {
            PlayerPrefs.SetInt("RateUsAvailableToday", 0);
            DateUpdate();
        }
        if (getDayCount(PlayerPrefs.GetInt("chstartDay")) == 1)
        {
            PlayerPrefs.SetInt("CONSISTANT_DAYS", PlayerPrefs.GetInt("CONSISTANT_DAYS", 0) + 1);
            //GameAnalytics.ConsistentDays(PlayerPrefs.GetInt("CONSISTANT_DAYS"));
        }
        else if (getDayCount(PlayerPrefs.GetInt("chstartDay")) >= 2)
        {
            PlayerPrefs.SetInt("CONSISTANT_DAYS", 0);
        }

        chstartDay = PlayerPrefs.GetInt("chstartDay");

        dayOfChallenge = getDayCount(chstartDay) + 1;


    }



    //public bool isNewDay()
    //{
    //    return (getDayCount(PlayerPrefs.GetInt("chstartDay")) >= 1);
    //}
	int getDayCount(int day)
	{
		int privousDay = day;
		int currentDay = System.DateTime.Now.DayOfYear;
		int dayValue = currentDay - privousDay;
		return dayValue;

	}
	void DateUpdate()
	{
        //Day Has Changed 

		//PlayerPrefs.SetInt("chstartDay",System.DateTime.Now.DayOfYear);
		//chstartDay = PlayerPrefs.GetInt ("chstartDay");
		//dayOfChallenge = getDayCount (chstartDay)+1;

    
        PlayerPrefs.SetInt("GamedaysCount", PlayerPrefs.GetInt("GamedaysCount") + 1);


        if (PlayerPrefs.GetInt("GamedaysCount") >= 5)
            Debug.Log("" + GameManager.Instance.TotalLevelsDone + 1);
        //    GameAnalytics.Completed_5_days(GameManager.Instance.TotalLevelsDone+1);

        //GameAnalytics.TotalPlayedDays(PlayerPrefs.GetInt("GamedaysCount"));
        //PlayerPrefs.SetInt("DailyRewardClaim", 0);


    }







}
