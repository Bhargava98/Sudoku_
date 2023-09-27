using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Globalization;
//using Assets.SimpleAndroidNotifications;
using MiniJSON;
using UnityEngine.UI;

namespace BizzyBeeGames.Sudoku{
    public class DailyRewardsManager : SingletonComponent<DailyRewardsManager>
    {

        #region  Inspector Vars
        public DailyReward[] dailyRewards;
        //public Color availableColor, greyColor;
        [SerializeField] private Transform starParticles = null;
        #endregion

        #region Public Members variables
        [HideInInspector] public DateTime timer;
        [HideInInspector] public DateTime lastRewardTime;
        private int availableReward;
        private DailyReward currReward;
        #endregion
        [Space(20)]
        public Text dueTime, collectReward;
        [Space(30)]
        public List<DailyRewardUI> randButtons;

        private int[] coinAmounts;
        private int[] hintAmounts;
        private string rewardData = "{\"coins\":[20,50,40,60,15],\"hints\":[2,3,4,5]}";
        public GameObject RewardParent;
        public Text DeltaCoinsText;
        public Image panelIcon;
        public GameObject AdTwoXButton;

        [Space(40)]
        public Image giftBoxCap, giftBoxBottom;
        public GameObject TwoXEarnedObj;
            
        #region Private Member variables
        private float t;
        private bool is_Initialized;
        #endregion

        #region Constants Keys
        private const string LAST_REWARD_TIME = "LastRewardTime";  
        private const string FMT = "O";
        #endregion

        #region Unity Methods
        protected override void Awake()
        {
            base.Awake();
            CheckRewards();
            PlayerPrefs.SetInt("RateCounter", PlayerPrefs.GetInt("RateCounter", 0) + 1);
            

        }

        private void Start()
        {
            if (Application.internetReachability != NetworkReachability.NotReachable)
            { StartCoroutine(GetDailyRewardsData()); }
            else
            {
                GetDailyRewardData(rewardData);
            }

#if UNITY_ANDROID
            if (UnityRemoteData.NotificationsEnabled)
            {
                // if (PlayerPrefs.GetInt("START_NOTIFICATION", 0) != 0)
                // NotificationManager.Cancel(PlayerPrefs.GetInt("START_NOTIFICATION"));
                //NotificationManager.CancelAll();

                //    NotificationManager.SendWithAppIcon(TimeSpan.FromDays(1), "Sudoku", "Challenge yourself and keep your brain healthy.", new Color(0, 0.6f, 1), NotificationIcon.Message);
                    //PlayerPrefs.SetInt("START_NOTIFICATION", PopupManager.Instance.NotificationID);
                
            }
#elif UNITY_IOS
                //
#endif 
        }

        IEnumerator GetDailyRewardsData()//v_0.6
        {
            yield return new WaitForSeconds(0.3f);

            if (UnityRemoteData.dailyRewardData != null)
            {
                GetDailyRewardData(UnityRemoteData.dailyRewardData);
            }
            else
            {
                GetDailyRewardData(rewardData);
            }
        } 

        private void Update()
        {

            

            t += Time.deltaTime;
            if (t >= 1)
            {
                timer = timer.AddSeconds(1);
                t = 0;
            }

            if (TwoXEarnedObj.activeInHierarchy)
            {
                TwoXEarnedObj.transform.GetChild(0).Rotate(0f,0f,0.8f);
            }

            if (dueTime.IsActive())
            {
                TimeSpan difference = (lastRewardTime - timer).Add(new TimeSpan(0, 24, 0, 0));
                if (difference.TotalSeconds <= 0)
                {
                    CheckRewards();
                    //UpdateUI();
                    return;
                }

                string formattedTs = string.Format("{0:D2}:{1:D2}:{2:D2}", difference.Hours, difference.Minutes, difference.Seconds);
                dueTime.text = "Next Reward in " + formattedTs + "";
            }
            else
            {

                TimeSpan difference = (lastRewardTime - timer).Add(new TimeSpan(0, 24, 0, 0));

                string formattedTs = string.Format("{0:D2}:{1:D2}:{2:D2}", difference.Hours, difference.Minutes, difference.Seconds);

                //dueTime.text = (difference.TotalHours > 0) ? formattedTs + "" : "Collect Reward now";
                //if (difference.TotalHours <= 0)
                //{
                //    dailyRewardICON.GetComponent<Animator>().enabled = true;
                //    dailyRewardICON.GetComponent<Animator>().Play("RewardPrize");
                //}
                //else
                //{
                //    dailyRewardICON.GetComponent<Animator>().enabled = false;
                //    dailyRewardICON.transform.localScale = Vector3.one;
                //    dailyRewardICON.transform.localPosition = new Vector3(0, -17.1f, 0);
                //}

            }

            //if (Input.GetKeyDown(KeyCode.Space))
            //    Reset();
        }
        #endregion

        #region Private Methods
        private void Initialize()
        {
            timer = DateTime.Now;
           
            is_Initialized = true;
        }

        //private void UpdateValue(int day)
        //{
        //    switch (dailyRewards[day - 1].reward_type)
        //    {
        //        case RewardType.coin:
        //            //CurrencyManager.Instance.Give("coins",dailyRewards[day-1].amount);
        //            //StartCoroutine(StartCoinIncrement(dailyRewards[day - 1].amount));
        //            //GameAnalytics.EarnVirtualCurrency("DailyRewardCoins", dailyRewards[day - 1].amount);
        //            SoundManager.Instance.Play("reward");
        //            break;
        //        case RewardType.hint:
        //            //CurrencyManager.Instance.Give("hints",dailyRewards[day-1].amount);
        //            //GameAnalytics.EarnVirtualCurrency("DailyRewardHints", dailyRewards[day - 1].amount);
        //            SoundManager.Instance.Play("reward");
        //            break;
        //    }
        //}


        public void UpdateUI()
        {
           // Debug.Log("AvailableReward:"+availableReward);
            dueTime.transform.parent.gameObject.SetActive((availableReward==0));
            collectReward.gameObject.SetActive(((availableReward != 0)));
            for (int i = 0; i < randButtons.Count; ++i)
            {
                //randButtons[i].ringImage.color = (availableReward == 0) ? greyColor : availableColor ;
                if (availableReward == 0)
                {
                    randButtons[i].questionObj.GetComponent<Button>().enabled = false;
                }
                else
                {
                    randButtons[i].questionObj.GetComponent<Button>().enabled = true;
                }
            }
        }

        

        IEnumerator StartCoinIncrement(int amount)
        {
            yield return new WaitForSeconds(0.2f);
            CurrencyManager.Instance.Give("coins", amount, 0.5f, true);
        }
        #endregion

        #region Public Member methods
        public void CheckRewards()
        {
            if (!is_Initialized)
            {
                Initialize();
            }
            
            string lastClaimedTime = PlayerPrefs.GetString(LAST_REWARD_TIME);//Debug.Log("lastClaimedTime"+ lastClaimedTime);
            

            //greyColor = GetThemeColor("reward_bg");
            //print("LAST_REWARD_TIME: "+ lastClaimedTime);
            //print("LAST_REWARD: "+ lastReward);

            if (!string.IsNullOrEmpty(lastClaimedTime))
            {
                lastRewardTime = DateTime.ParseExact(lastClaimedTime, FMT, CultureInfo.InstalledUICulture);

                TimeSpan diff = timer - lastRewardTime;
                
                int days = 0;

                if ((int)diff.TotalHours < 0)
                    days = 0;
                else
                    days = (int)(Math.Abs(diff.TotalHours) / 24);


                if (days == 0)
                {
                    // No claim for you. Try tomorrow
                    availableReward = 0;
                    GameManager.Instance.isDailyRewardAvailable = false;
                    //Debug.Log("Reward Not Available!");
                    UpdateUI();
                    return;
                }

                // The player can only claim if he logs between the following day and the next.
                if (days >= 1 && days < 2)
                {                    
                    //Can claim the prime now
                    availableReward = 1;
                    GameManager.Instance.isDailyRewardAvailable = true;
                    // Debug.Log("Reward Available!");
                    UpdateUI();
                    return;
                }

                if (days >= 2)
                {
                    // The player loses the following day reward and resets the prize
                    availableReward = 1;
                    GameManager.Instance.isDailyRewardAvailable = true;
                    UpdateUI();
                    //Debug.Log("Prize reset ");
                }

            }
            else
            {
                availableReward = 1;
                GameManager.Instance.isDailyRewardAvailable = true;
                UpdateUI();
            }
            
        }

        public void ClaimPrize(int num)
        {
            if (availableReward == 1)
            {
                GameManager.Instance.multiplier = 1;
                TwoXEarnedObj.SetActive(false);

                if (UnityRemoteData.isFullAdsEnabled && Application.internetReachability != NetworkReachability.NotReachable)
                AdTwoXButton.SetActive(true);

                float rand = UnityEngine.Random.value;
                if (rand >= 0.5f)
                {
                    currReward = dailyRewards[0];
                    currReward.amount = coinAmounts[UnityEngine.Random.Range(0, coinAmounts.Length)];
                }
                else
                {
                    currReward = dailyRewards[1];
                    currReward.amount = hintAmounts[UnityEngine.Random.Range(0, hintAmounts.Length)];
                }

                randButtons[num].rewardIcon.sprite = currReward.icon;
                randButtons[num].rewardAmount.text = "+"+currReward.amount.ToString();
                giftBoxCap.sprite = randButtons[num].boxCap;
                giftBoxBottom.sprite = randButtons[num].boxBottom;

                DeltaCoinsText.text = "+" + currReward.amount;
                panelIcon.sprite = currReward.icon;

                randButtons[num].questionObj.SetActive(false);

                Invoke("AwardedAnim",0.4f);
                string lastClaimedStr = timer.ToString(FMT);
                PlayerPrefs.SetString(LAST_REWARD_TIME, lastClaimedStr);

#if UNITY_ANDROID
                if (UnityRemoteData.NotificationsEnabled)
                {
                    //NotificationManager.CancelAll();

                    //NotificationManager.SendWithAppIcon(TimeSpan.FromDays(1), "Sudoku", " Collect your daily reward.", new Color(0, 0.6f, 1), NotificationIcon.Message);
                    //PlayerPrefs.SetInt("REWARD_NOTF_ID", PopupManager.Instance.NotificationID);
                }
#elif UNITY_IOS
                //
#endif                
               // HideRewardAnim();
                //RewardParent.SetActive(false);


            }
            
           
        }

        public void OnClaimClicked()
        {
            //Give reward and add amount
            switch (currReward.reward_type)
            {
                case RewardType.coin:
                    SoundManager.Instance.Play("reward");
                    CurrencyManager.Instance.Give("coins", currReward.amount);
                    //GameAnalytics.EarnVirtualCurrency("DailyRewardCoins", currReward.amount);
                    break;
                case RewardType.hint:
                    SoundManager.Instance.Play("reward");
                    CurrencyManager.Instance.Give("hints", currReward.amount);
                    //GameAnalytics.EarnVirtualCurrency("DailyRewardHints", currReward.amount);
                    break;
                default:
                    print("Default award");
                    break;
            }

            HideRewardAnim();
            
        }


        /// <summary>
        /// Ads code
        /// </summary>

        public void DailyRewMultiplayerADReq()
        {
            //if (PopupManager.Instance.NoInternetNativePopup("Network", "No Internet Access") == false)
            //    AdsManager.Instance.ShowRewardedAd(new object[] { "dailyMultiplier" });
        }


        public void AfterWatchedAd()
        {
            GameManager.Instance.multiplier = 2;
            currReward.amount *= GameManager.Instance.multiplier;
            StartCoroutine(SetMultipliedText());
            //DeltaCoinsText.text = String.Format("+{0} {1}", myAmount, myReward);
           

        }

        IEnumerator SetMultipliedText()
        {
            AdTwoXButton.SetActive(false);
            TwoXEarnedObj.SetActive(true);
            UIAnimation anim = null;
            anim = UIAnimation.ScaleX(TwoXEarnedObj.transform.GetComponent<RectTransform>(), 1.6f, 1f, 0.4f);
            anim.style = UIAnimation.Style.EaseIn;
            anim.startOnFirstFrame = true;
            anim.Play();

            anim = UIAnimation.ScaleY(TwoXEarnedObj.transform.GetComponent<RectTransform>(), 1.6f, 1f, 0.4f);
            anim.style = UIAnimation.Style.EaseIn;
            anim.startOnFirstFrame = true;
            anim.Play();



            //yield return new WaitForSeconds(0.5f);
            //DeltaCoinsText.text = String.Format("+{0} {1}", currReward.amount / GameManager.Instance.multiplier + " x2", " ");
            yield return new WaitForSeconds(0.6f);
            DeltaCoinsText.text = String.Format("+{0} {1}", currReward.amount, " ");
        }
        ///---------------?
        void AwardedAnim()
        {
            //DeltaCoinsText.gameObject.SetActive(true);
            //SoundManager.Instance.Play("spin-powerup");
            CheckRewards();
            SoundManager.Instance.Play("btn-close");
            RewardParent.SetActive(true);
            UIAnimation anim = null;
            anim = UIAnimation.ScaleX(RewardParent.transform.GetChild(1).GetComponent<RectTransform>(), 0f, 1f, 0.3f);
            anim.style = UIAnimation.Style.Linear;
            anim.startOnFirstFrame = true;
            anim.Play();

            anim = UIAnimation.ScaleY(RewardParent.transform.GetChild(1).GetComponent<RectTransform>(), 0f, 1f, 0.3f);
            anim.style = UIAnimation.Style.Linear;
            anim.startOnFirstFrame = true;
            anim.Play();
        }

        void HideRewardAnim()
        {
            UIAnimation anim = null;
            anim = UIAnimation.ScaleX(RewardParent.transform.GetChild(1).GetComponent<RectTransform>(), 1f, 0f, 0.4f);
            anim.style = UIAnimation.Style.Linear;
            anim.startOnFirstFrame = true;
            anim.Play();

            anim = UIAnimation.ScaleY(RewardParent.transform.GetChild(1).GetComponent<RectTransform>(), 1f, 0f, 0.4f);
            anim.style = UIAnimation.Style.Linear;
            anim.startOnFirstFrame = true;
            anim.Play();
            StartCoroutine(HidePanel());
        }

        IEnumerator HidePanel()
        {
            yield return new WaitForSeconds(0.4f);
            RewardParent.SetActive(false);
        }
        //public void SetEffectPos()
        //{
        //    starParticles.GetComponent<RectTransform>().anchoredPosition = effectObj.GetComponent<RectTransform>().localPosition;
        //    if (!starParticles.GetChild(0).GetComponent<ParticleSystem>().isPlaying)
        //        starParticles.GetChild(0).GetComponent<ParticleSystem>().Play();
        //    CurrencyManager.Instance.SetAnimObjsGamePlay(starParticles.transform.GetChild(0).gameObject);
        //}


        /// <summary>
        /// Get Daily Reward data 
        /// </summary>
        /// <param name="json_info"></param>
        public void GetDailyRewardData(string json_info)//v_0.5
        {
            if (json_info != null)
            {                
                Dictionary<string, object> testData = Json.Deserialize(json_info) as Dictionary<string, object>;
               

                List<object> data1 = testData["coins"] as List<object>;
                List<object> data2 = testData["hints"] as List<object>;

                coinAmounts = new int[data1.Count];
                hintAmounts = new int[data2.Count];
                for (int i = 0; i < data1.Count; ++i)
                {
                    coinAmounts[i] = Convert.ToInt32(data1[i]);                    
                }
                for (int i = 0; i < data2.Count; ++i)
                {
                    hintAmounts[i] = Convert.ToInt32(data2[i]);                   
                }
                
                
            }
        }


        public void Reset()
        {           
            PlayerPrefs.DeleteKey(LAST_REWARD_TIME);
        }

      
        private void OnApplicationFocus(bool focus)
        {
            timer = DateTime.Now;

            //if(Application.internetReachability!=NetworkReachability.NotReachable)
            GameManager.interstitial_Controller = false;
            
        }

        private Color GetThemeColor(string itemId)
        {
            Color color = Color.white;

            ThemeManager.Instance.GetItemColor(itemId, out color);

            return color;
        }

        #endregion
    }


    [System.Serializable]
    public class DailyReward
    {    
        public Sprite icon;
        public RewardType reward_type;
        public int amount;
    }
    
    public enum RewardType
    {
        coin,
        hint,
        none
    }
}