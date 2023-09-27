using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using BizzyBeeGames;


public enum DailyMissionTypes { None, Coins, Levels, Hints };

public class DailyMissionController : MonoBehaviour
{
    #region DayChange Variables
    public static DailyMissionController instance;
    private int chstartDay = 0;
    private int dayOfChallenge = 0;
    #endregion

    #region Variables
    // for different daily Mission
    [HideInInspector] public int levelCounter;
    [HideInInspector] public int levelPlayedCounter;
    [HideInInspector] public int totalCoins;
    [HideInInspector] public int coinsgained;
    [HideInInspector] public int totalHints;
    [HideInInspector] public int hintsSpent;

    [SerializeField] private GameObject tickImage = null;
    [SerializeField] private Button claimButton = null;
    [SerializeField] private Text progressText = null;
    [SerializeField] private Text descriptionText = null;

    //[SerializeField] private CrosswordController crosswordController;
    private const string dailyMissionTime = "DailyMission";
    private const string dailyMissionClaim = "DailyMissionClaim";

    private const string dailyMissionType = "DailyMissionType";
    private const string dailyMissionProgress = "DailyMissionProgress";

    private const string playerLevelCount = "PlayerLevelCount";
    private const string playerCoinCount = "PlayerCoinCount";
    private const string playerHintCount = "PlayerHintCount";

   
    public int rewardAmount = 1000;

    public static int currentMission;

    [Header("Set Game Missions")]
    public GameMissions[] myMissions;
    [HideInInspector] public int missionCounter = 0;
    [HideInInspector] public int missionProgress = 0;
    //[SerializeField] private GameObject enabledImage, disabledImage;
    private static bool missionStatus = false;

    #endregion

    #region Unity Methods
    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        if (!PlayerPrefs.HasKey("OnLoadData") || PlayerPrefs.GetInt("OnLoadData") == 0)
        {
            PlayerPrefs.SetInt("OnLoadData", 1);
            PlayerPrefs.SetInt(dailyMissionTime, System.DateTime.Now.DayOfYear);
            PlayerPrefs.SetInt(dailyMissionClaim, 0);
            tickImage.SetActive(false);
            claimButton.gameObject.SetActive(false);
            // assign random mission
            RandomMission();
        }

        if (GetDayCount(PlayerPrefs.GetInt(dailyMissionTime)) >= 1)
        {
            DateUpdate();
            
            // reset rewards
            ResetRewards();

            // random mission
            RandomMission();
        }
        else
        {
            // load previous mission
            LoadMission();
        }
        chstartDay = PlayerPrefs.GetInt(dailyMissionTime);
        dayOfChallenge = GetDayCount(chstartDay) + 1;

        currentMission = PlayerPrefs.GetInt(dailyMissionType);
    }
    #endregion

    #region Private Methods
    private void RandomMission()
    {
        int randomMission = UnityEngine.Random.Range(0, myMissions.Length);
       // int randomMission = 1;
        missionCounter = myMissions[randomMission].values;
        descriptionText.text = myMissions[randomMission].missionDiscription;
        progressText.text = missionProgress + "/" + missionCounter.ToString();
        PlayerPrefs.SetInt(dailyMissionType, randomMission);
        PlayerPrefs.SetInt(dailyMissionProgress, 0);
        //PlayerPrefs.SetInt("MissionStatus", 0);
        tickImage.SetActive(false);
    }

    private void ResetRewards()
    {
        // reset all previous values
        levelPlayedCounter = 0;
        hintsSpent = 0;
        coinsgained = 0;

        missionCounter = 0;
        tickImage.SetActive(false);
        claimButton.gameObject.SetActive(false);


        // if day changed then generate random rewards again
        //RandomMission();
    }

    int GetDayCount(int day)
    {
        int previousDay = day;
        int currentDay = System.DateTime.Now.DayOfYear;
        int dayValue = currentDay - previousDay;

        if (dayValue < 0)
        {
            if (DateTime.IsLeapYear(System.DateTime.Now.Year - 1)) previousDay = 366 - previousDay;
            else previousDay = 365 - previousDay;

            dayValue = previousDay + currentDay;
            print("Happy New Year - Game Opened After " + previousDay.ToString() + "days");
        }

        return dayValue;
    }

    void DateUpdate()
    {
        //Day Has Changed 
        PlayerPrefs.SetInt(dailyMissionTime, System.DateTime.Now.DayOfYear);
        chstartDay = PlayerPrefs.GetInt(dailyMissionTime);
        dayOfChallenge = GetDayCount(chstartDay) + 1;

        PlayerPrefs.SetInt("GamedaysCount", PlayerPrefs.GetInt("GamedaysCount") + 1);

        if (PlayerPrefs.GetInt("GamedaysCount") >= 1)
        {
            // reset reward and randomize reward
        }

        PlayerPrefs.SetInt(dailyMissionClaim, 0);
    }

    //private bool DailyMissionComplete()
    //{
    //    // for levels
    //    if (levelPlayedCounter == levelCounter && PlayerPrefs.GetInt(dailyMissionClaim) == 0)
    //    {
    //        return true;
    //    }
    //    else if (hintsSpent == totalHints && PlayerPrefs.GetInt(dailyMissionClaim) == 0)
    //    {
    //        return true;
    //    }
    //    else if (coinsgained == totalCoins && PlayerPrefs.GetInt(dailyMissionClaim) == 0)
    //    {
    //        return true;
    //    }
    //    PlayerPrefs.SetInt(dailyMissionClaim, 1);
    //    return false;
    //}

    private void GiveReward()
    {
        if (currentActiveType == DailyMissionTypes.Levels)
        {
            PopupManager.Instance.Show("rewarded", new object[] { PopupManager.Instance.rewardImage[0], "coinsDM", myMissions[1].awardCoins });
        }
        else if (currentActiveType == DailyMissionTypes.Coins)
        {
            PopupManager.Instance.Show("rewarded", new object[] { PopupManager.Instance.rewardImage[0], "coinsDM", myMissions[0].awardCoins });
        }
        else if (currentActiveType == DailyMissionTypes.Hints)
        {
            PopupManager.Instance.Show("rewarded", new object[] { PopupManager.Instance.rewardImage[0], "coinsDM", myMissions[2].awardCoins });
        }

       // PopupManager.Instance.ShowRewardsPopup("coinsDM");
        // 
        //CurrencyManager.Instance.GiveCoins(rewardAmount);
        // PlayerPrefs.SetInt(dailyMissionClaim, 0);
    }

    private DailyMissionTypes currentActiveType;
    private void LoadMission()
    {        
        currentActiveType = myMissions[PlayerPrefs.GetInt(dailyMissionType)].currentMission;
        UpdateTexts();
       
       
        //if (PlayerPrefs.GetInt(dailyMissionProgress, 0) >= missionCounter)
        //{
        //    print("Mission Complete");
        //    MissionComplete();
        //    //Missiom Status 1
        //    PlayerPrefs.SetInt("MissionStatus", 1);
        //}
    }

    #endregion

    #region Public Methods
    public void OnButtonClick()
    {
        // player has completed the mission
        if (PlayerPrefs.GetInt(dailyMissionClaim) == 0)
        {
            //if()
            // give reward
            GiveReward();
            PlayerPrefs.SetInt(dailyMissionProgress, 0);
            PlayerPrefs.SetInt(dailyMissionClaim, 1);

            ResetRewards();
            UpdateTexts();
        }
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            PlayerPrefs.DeleteAll();
    }
    public void MissionComplete()
    {
        //tickImage.SetActive(true);

        claimButton.gameObject.SetActive(true);
    }

    public void UpdateTexts()
    {
        missionCounter = myMissions[PlayerPrefs.GetInt(dailyMissionType)].values;
        descriptionText.text = myMissions[PlayerPrefs.GetInt(dailyMissionType)].missionDiscription;
        if (PlayerPrefs.GetInt(dailyMissionClaim) == 0)
        {
            if (PlayerPrefs.GetInt(dailyMissionProgress) < missionCounter)
            {
                progressText.text = PlayerPrefs.GetInt(dailyMissionProgress) + "/" + missionCounter.ToString();
                tickImage.SetActive(false);
                claimButton.gameObject.SetActive(false);
            }
            else
            { 

                progressText.text = "Daily Mission Completed";
                claimButton.gameObject.SetActive(true);
               // tickImage.SetActive(true);
            }
        }
        else
        {
            progressText.text = "Daily Mission Completed.\nCome back Tomorrow";
            claimButton.gameObject.SetActive(false);
            tickImage.SetActive(true);
        }
    }

    public void MissionProgressUpdate(int value, DailyMissionTypes type)
    {
        if (PlayerPrefs.GetInt("MissionStatus") == 1)
        {
            return;
        }
        currentActiveType = myMissions[PlayerPrefs.GetInt(dailyMissionType)].currentMission;
        switch (currentActiveType)
        {
           
            case DailyMissionTypes.Coins:
                if(type==currentActiveType)
                PlayerPrefs.SetInt(dailyMissionProgress, PlayerPrefs.GetInt(dailyMissionProgress, 0) + value);
                break;
            case DailyMissionTypes.Levels:
                if (type == currentActiveType)
                    PlayerPrefs.SetInt(dailyMissionProgress, PlayerPrefs.GetInt(dailyMissionProgress, 0) + value);
                break;
            case DailyMissionTypes.Hints:
                if (type == currentActiveType)
                    PlayerPrefs.SetInt(dailyMissionProgress, PlayerPrefs.GetInt(dailyMissionProgress, 0) + value);
                break;
            default:
                break;
        }
        UpdateTexts();

        //if (PlayerPrefs.GetInt(dailyMissionProgress, 0) >= missionCounter)
        //{
        //    print("Mission Complete");
        //    MissionComplete();
        //    //Missiom Status 1
        //    PlayerPrefs.SetInt("MissionStatus", 1);

        //}
        //else
        //{
        //    //Missiom Status 0
        //    PlayerPrefs.SetInt("MissionStatus", 0);
        //}
    }
    #endregion
}
[System.Serializable]
public class GameMissions
{
    public DailyMissionTypes currentMission;
    public int values;
    public string missionDiscription;
    public int awardCoins;
    //public Sprite missionIcon;
}