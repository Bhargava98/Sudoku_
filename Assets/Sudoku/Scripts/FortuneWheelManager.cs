using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;
//using Assets.SimpleAndroidNotifications;

namespace BizzyBeeGames.Sudoku
{

    public enum LUCKY_SPIN_REWARDS
    {
        COINS,
        HINTS
    }

    public class FortuneWheelManager : SingletonComponent<FortuneWheelManager>
    {


        public bool clicking;
        public GameObject PaidTurnButton;               // This button is showed when you can turn the wheel for coins
        public GameObject FreeTurnButton;               // This button is showed when you can turn the wheel for free
        public GameObject Circle;                   // Rotatable GameObject on scene with reward objects
        public Text DeltaCoinsText;                 // Pop-up text with wasted or rewarded coins amount
        //public Text CurrentCoinsText;               // Pop-up text with wasted or rewarded coins amount
        public GameObject NextTurnTimerWrapper;
        public Text NextFreeTurnTimerText;          // Text element that contains remaining time to next free turn
        public Image CoinsDeltaImage;
        public RectTransform rewardHighlight;
        public GameObject RewardParent;

        public GameObject closeBtm;
        public GameObject Adbutton;
        public GameObject SpinIcon;
        public GameObject SpinIndicator;
        public GameObject AdTwoXButton;

        public int SpinCost = 25;
        public int boosterRewardCount = 1;

        public List<Sprite> awardImage;

        public Text paidSpinCost;


        public Text[] wheelItems = new Text[6];
        public int[] itemCosts = new int[6];
        public GameObject TwoXEarnedObj;

        private LUCKY_SPIN_REWARDS coin_reward = LUCKY_SPIN_REWARDS.COINS;
        private LUCKY_SPIN_REWARDS hint_reward = LUCKY_SPIN_REWARDS.HINTS;

        private bool _isStarted;                    // Flag that the wheel is spinning
        private float[] _sectorsAngles;             // All sectors angles
        private float _finalAngle;                  // The final angle is needed to calculate the reward
        private float _startAngle = 0;              // The first time start angle equals 0 but the next time it equals the last final angle
        private float _currentLerpRotationTime;     // Needed for spinning animation
        private int _turnCost = 25;             // How much coins user waste when turn when wheel
        private int _currentCoinsAmount = 0;        // Started coins amount. In your project it should be picked up from CoinsManager or from PlayerPrefs and so on
        private int _previousCoinsAmount;

        // Here you can set time between two free turns
        private int _timerMaxHours = 24;
        private int _timerMaxMinutes = 0;
        private int _timerMaxSeconds = 0;

        // Remaining time to next free turn
        private int _timerRemainingHours = 0;
        private int _timerRemainingMinutes = 0;
        private int _timerRemainingSeconds = 0;

        private DateTime _nextFreeTurnTime;

        // Key name for storing in PlayerPrefs
        private const string LAST_FREE_TURN_TIME_NAME = "LastFreeTurnTimeTicks";

        // Set TRUE if you want to let players to turn the wheel for coins while free turn is not available yet
        private bool _isPaidTurnEnabled = true;

        // Set TRUE if you want to let players to turn the wheel for FREE from time to time
        private bool _isFreeTurnEnabled = true;

        // Flag that player can turn the wheel for free right now
        private bool _isFreeTurnAvailable = false;





        protected override void Awake()
        {
            base.Awake();

           
            // define coin amount
           

            // spin cost
            _turnCost = SpinCost;

            _previousCoinsAmount = _currentCoinsAmount;
            // Show our current coins amount
           // CurrentCoinsText.text = _currentCoinsAmount.ToString();

            if (_isFreeTurnEnabled)
            {
                if (!PlayerPrefs.HasKey(LAST_FREE_TURN_TIME_NAME))
                {
                    //print("Very first free spin: " + DateTime.Now.AddDays(-1).Ticks.ToString());

                    PlayerPrefs.SetString(LAST_FREE_TURN_TIME_NAME, DateTime.Now.AddDays(-7).Ticks.ToString());
                }

                // Start our timer to next free turn
                SetNextFreeTime();
            }
            else
            {
                NextTurnTimerWrapper.gameObject.SetActive(false);
            }
        }
        void Start()
        {

            paidSpinCost.text = SpinCost.ToString();
            for (int i = 0; i < itemCosts.Length; ++i)
            {
                wheelItems[i].text = itemCosts[i].ToString();
            }

            //Adbutton.SetActive(false);
            _currentCoinsAmount = CurrencyManager.Instance.GetAmount("coins");

#if UNITY_ANDROID || UNITY_IOS
            if (Application.internetReachability != NetworkReachability.NotReachable )
            {
                //iTween.ShakeRotation(Adbutton.transform.GetChild(0).gameObject, iTween.Hash(

                //    "amount", new Vector3(0f, 0f, 5f),
                //    "easetype", iTween.EaseType.easeOutBack,
                //    "looptype", iTween.LoopType.pingPong,

                //    "oncompletetarget", Adbutton.transform.GetChild(0).gameObject,

                //    "delay", 2f,
                //    "time", 1f
                //));
                if (UnityRemoteData.isFullAdsEnabled)
                {
                    Adbutton.SetActive(true);
                    AdTwoXButton.SetActive(true);
                }
            }
#endif

        }
        private void TurnWheelForFree() { TurnWheel(true); }
        private void TurnWheelForCoins() { TurnWheel(false); }

        private void TurnWheel(bool isFree)
        {
            _currentLerpRotationTime = 0f;

            // Fill the necessary angles (for example if you want to have 12 sectors you need to fill the angles with 30 degrees step)

            //		_sectorsAngles = new float[] { 30, 60, 90, 120, 150, 180, 210, 240, 270, 300, 330, 360 };//charan commented previous angles..

            //_sectorsAngles = new float[] { 45,90,135,180,225,270,315,360};//charan add new angles

            _sectorsAngles = new float[] { 60, 120, 180, 240, 300, 360 };

            int fullTurnovers = 5;

            // Choose random final sector
            float randomFinalAngle = _sectorsAngles[UnityEngine.Random.Range(0, _sectorsAngles.Length)];

            // Set up how many turnovers our wheel should make before stop
            _finalAngle = -(fullTurnovers * 360 + randomFinalAngle);

            // Stop the wheel
            _isStarted = true;

            _previousCoinsAmount = _currentCoinsAmount;

            // Decrease money for the turn if it is not free turn
            if (!isFree)
            {
                _currentCoinsAmount -= _turnCost;

               // CoinsDeltaImage.gameObject.SetActive(false);
                // Show wasted coins
                DeltaCoinsText.text = String.Format("-{0}", _turnCost);
                //DeltaCoinsText.gameObject.SetActive (true);


                CoinsDeltaImage.overrideSprite = awardImage[0];

                // reduce coin sound
                //AudioManager.instance.CoinPayAudio();
                SoundManager.Instance.Play("coin-collect");
                CurrencyManager.Instance.GiveCoins(-_turnCost);
                //GameAnalytics.SpendVirtualCurrency("Paid Spin",1,"Coins",_turnCost);
                // update game data
               
                // Animations for coins
                //StartCoroutine(HideCoinsDelta());
                //StartCoroutine(UpdateCoinsAmount());
            }
            else
            {
                // At this step you can save current time value to your server database as last used free turn
                // We can't save long int to PlayerPrefs so store this value as string and convert to long later
                PlayerPrefs.SetString(LAST_FREE_TURN_TIME_NAME, DateTime.Now.Ticks.ToString());

#if UNITY_ANDROID
                if (UnityRemoteData.NotificationsEnabled)
                {
                    // NotificationManager.Cancel(PlayerPrefs.GetInt("REWARD_NOTF_ID"));
                    // NotificationManager.Cancel(PlayerPrefs.GetInt("FREESPIN_NOTF_ID"));
                    //NotificationManager.CancelAll();
                    //NotificationManager.SendWithAppIcon(TimeSpan.FromDays(1), "Sudoku", "Your free spin is ready!", new Color(0, 0.6f, 1), NotificationIcon.Message);
                   // PlayerPrefs.SetInt("FREESPIN_NOTF_ID", PopupManager.Instance.NotificationID);
                }
#elif UNITY_IOS
                //
#endif

                // Restart timer to next free turn
                SetNextFreeTime();
            }
        }

       
        public void TurnWheelButtonClick()
        {
            _currentCoinsAmount = CurrencyManager.Instance.GetAmount("coins");
            if (UnityRemoteData.isFullAdsEnabled && Application.internetReachability != NetworkReachability.NotReachable)
            {                
                AdTwoXButton.SetActive(true);
            }
            TwoXEarnedObj.SetActive(false);

            if (_isFreeTurnAvailable)
            {
                ScreenManager.Instance.isSpinWheel = true;
                closeBtm.SetActive(false);
                Adbutton.SetActive(false);
                //if (!AudioManager.instance.spinSound.isPlaying && PlayerPrefs.GetInt("sound_on,1") == 1)
                //    AudioManager.instance.spinSound.Play();
                SoundManager.Instance.Play("spin-rotation");
                TurnWheelForFree();
                //			Adbutton.transform.parent.gameObject.SetActive (false);
            }
            else
            {
                // If we have enabled paid turns
                if (_isPaidTurnEnabled)
                {
                    // If player have enough coins
                   
                    if (_currentCoinsAmount >= _turnCost)
                    {
                        ScreenManager.Instance.isSpinWheel = true;

                        
                        GameObject anim = Instantiate(GameManager.Instance.floatingPref, Adbutton.transform.parent.transform);
                        anim.GetComponent<FloatingAnim>().playAnim(anim.GetComponent<RectTransform>(), awardImage[0], SpinCost.ToString());
                      
                        closeBtm.SetActive(false);
                        Adbutton.SetActive(false);

                        //if (!AudioManager.instance.spinSound.isPlaying && PlayerPrefs.GetInt("sound_on,1") == 1)
                        //    AudioManager.instance.spinSound.Play();
                        SoundManager.Instance.Play("spin-rotation");
                        TurnWheelForCoins();
                        //Adbutton.transform.parent.gameObject.SetActive (false);
                    }
                    else
                    {
                        PopupManager.Instance.Show("coin_shop");                       
                    }
                }
            }
        }

        public void SetNextFreeTime()
        {
            // Reset the remaining time values
            _timerRemainingHours = _timerMaxHours;
            _timerRemainingMinutes = _timerMaxMinutes;
            _timerRemainingSeconds = _timerMaxSeconds;

            // Get last free turn time value from storage
            // We can't save long int to PlayerPrefs so store this value as string and convert to long
            _nextFreeTurnTime = new DateTime(Convert.ToInt64(PlayerPrefs.GetString(LAST_FREE_TURN_TIME_NAME, DateTime.Now.Ticks.ToString())))
                .AddHours(_timerMaxHours)
                .AddMinutes(_timerMaxMinutes)
                .AddSeconds(_timerMaxSeconds);




            _isFreeTurnAvailable = false;
        }

        private void GiveAwardByAngle()
        {
            /*
            
            // Here you can set up rewards for every sector of wheel (clockwise)
            switch ((int)_startAngle) {
            // Sector 1
            case 0:
                RewardCoins (Sector1);
                break;
                // Sector 2
            case -330:
                RewardCoins(Sector2);
                break;
                // Sector 3
            case -300:
                RewardCoins(Sector3);
                break;
                // Sector 4
            case -270:
                RewardCoins(Sector4);
                break;
                // Sector 5
            case -240:
                RewardCoins(Sector5);
                break;
                // Sector 6
            case -210:
                RewardCoins(Sector6);
                break;
                // Sector 7
            case -180:
                RewardCoins(Sector7);
                break;
                // Sector 8
            case -150:
                RewardCoins(Sector8);
                break;
                // Sector 9
            case -120:
                RewardCoins(Sector9);
                break;
                // Sector 10
            case -90:
                RewardCoins(Sector10);
                break;
                // Sector 11
            case -60:
                RewardCoins(Sector11);
                break;
                // Sector 12
            case -30:
                RewardCoins(Sector12);
                break;
            default:
                break;
            }
            */
           
            // Here you can set up rewards for every sector of wheel (clockwise)
            switch ((int)_startAngle)
            {
                // Sector 1
                case 0:
                    RewardCoins(hint_reward,itemCosts[0]);
                    break;
                // Sector 4
                case -300:
                    RewardCoins(coin_reward, itemCosts[1]);
                    break;
                // Sector 5
                case -240:
                    RewardCoins(hint_reward, itemCosts[2]);
                    break;
                // Sector 6
                case -180:
                    RewardCoins(coin_reward, itemCosts[3]);
                    break;
                // Sector 7
                case -120:
                    RewardCoins(hint_reward, itemCosts[4]);
                    break;
                // Sector 8
                case -60:
                    RewardCoins(coin_reward, itemCosts[5]);
                    break;

            }

          
        }

        private void ShowTurnButtons()
        {
            SpinIcon.transform.Rotate(0, 0, 0.7f);
            if (_isFreeTurnAvailable)               // If have free turn
            {
                ShowFreeTurnButton();
                EnableFreeTurnButton();

                //SpinIcon.transform.Rotate(0, 0, 0.7f);
                SpinIndicator.SetActive(true);
            }
            else
            {                               // If haven't free turn
                SpinIndicator.SetActive(false);
                if (!_isPaidTurnEnabled)            // If our settings allow only free turns
                {
                    ShowFreeTurnButton();
                    DisableFreeTurnButton();        // Make button inactive while spinning or timer to next free turn

                }
                else
                {                           // If player can turn for coins
                    ShowPaidTurnButton();

                    if (_isStarted) //|| _currentCoinsAmount < _turnCost)
                    {
                        DisablePaidTurnButton();    // Make button non interactable if user has not enough money for the turn of if wheel is turning right now

                    }
                    else
                    {
                        EnablePaidTurnButton(); // Can make paid turn right now

                    }
                }
            }

        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                AfterWatchedAd();
            }

            // We need to show TURN FOR FREE button or TURN FOR COINS button
            ShowTurnButtons();
            //Circle.transform.Rotate(0,0,10f);
            // Show timer only if we enable free turns
            if (_isFreeTurnEnabled) {                
                UpdateFreeTurnTimer();
            }

            if (TwoXEarnedObj.activeInHierarchy)
            {
                TwoXEarnedObj.transform.GetChild(0).Rotate(0f, 0f, 0.8f);
            }

            if (rewardHighlight.gameObject.activeInHierarchy)
            rewardHighlight.transform.Rotate(0,0,2f);

#if UNITY_ANDROID || UNITY_IOS
            //if (!Adbutton.activeSelf && IronSource.Agent.isRewardedVideoAvailable())
            //    Adbutton.SetActive(true);
#endif

            if (!_isStarted)
                return;


            // Animation time
            float maxLerpRotationTime = 4f;

            // increment animation timer once per frame
            _currentLerpRotationTime += Time.deltaTime;

            // If the end of animation
            if (_currentLerpRotationTime > maxLerpRotationTime || Circle.transform.eulerAngles.z == _finalAngle)
            {
                _currentLerpRotationTime = maxLerpRotationTime;
                _isStarted = false;
                _startAngle = _finalAngle % 360;

                GiveAwardByAngle();
                //StartCoroutine(HideCoinsDelta());
            }
            else
            {
                // Calculate current position using linear interpolation
                float t = _currentLerpRotationTime / maxLerpRotationTime;

                // This formula allows to speed up at start and speed down at the end of rotation.
                // Try to change this values to customize the speed
                t = t * t * t * (t * (6f * t - 15f) + 10f);

                float angle = Mathf.Lerp(_startAngle, _finalAngle, t);
                Circle.transform.eulerAngles = new Vector3(0, 0, angle);
            }
           
        }
        void AwardedAnim(int amt, LUCKY_SPIN_REWARDS type)
        {
            DeltaCoinsText.text = String.Format("+{0} {1}", amt,type);
            //DeltaCoinsText.gameObject.SetActive(true);
            SoundManager.Instance.Play("spin-powerup");
            RewardParent.SetActive(true);
            UIAnimation anim = null;
            anim = UIAnimation.ScaleX(RewardParent.transform.GetChild(1).GetComponent<RectTransform>(), 0f, 1f, 0.3f);
            anim.style = UIAnimation.Style.Linear;
            anim.startOnFirstFrame = true;
            anim.Play();

            anim = UIAnimation.ScaleY(RewardParent.transform.GetChild(1).GetComponent<RectTransform>(), 0f, 1f,0.3f);
            anim.style = UIAnimation.Style.Linear;
            anim.startOnFirstFrame = true;
            anim.Play();            
        }


        LUCKY_SPIN_REWARDS myReward;int myAmount;
        // Give the reward to player
        private void RewardCoins(LUCKY_SPIN_REWARDS award,int amount)
        {

            myReward = award;
            myAmount = amount;
            AwardedAnim(amount, award);
            switch (award)
            {                               
                case LUCKY_SPIN_REWARDS.COINS:
                   
                    CoinsDeltaImage.overrideSprite = awardImage[0];                    
                    break;
                case LUCKY_SPIN_REWARDS.HINTS:
                    
                    CoinsDeltaImage.overrideSprite = awardImage[1];                   
                    break;
                default:
                    print("Default award");
                    break;
            }           
        }

        public void SpinMultiplayerADReq()
        {
            //if (PopupManager.Instance.NoInternetNativePopup("Network", "No Internet Access") == false)
                //AdsManager.Instance.ShowRewardedAd(new object[] { "spinMultiplier" });
        }


        public void AfterWatchedAd()
        {
            GameManager.Instance.multiplier = 2;
            myAmount *= GameManager.Instance.multiplier;
            StartCoroutine(SetMultipliedText());
            //DeltaCoinsText.text = String.Format("+{0} {1}", myAmount, myReward);
            AdTwoXButton.SetActive(false);

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
            //DeltaCoinsText.text = String.Format("+{0} {1}", myAmount/ GameManager.Instance.multiplier + " x2", myReward);
            yield return new WaitForSeconds(0.6f);
            DeltaCoinsText.text = String.Format("+{0} {1}", myAmount, myReward);
        }

        public void closeinteract()
        {           

            switch (myReward)
            {
                case LUCKY_SPIN_REWARDS.COINS:                    
                    CurrencyManager.Instance.Give("coins", myAmount);
                    //GameAnalytics.EarnVirtualCurrency("SpinCoins", myAmount);
                    break;
                case LUCKY_SPIN_REWARDS.HINTS:                   
                    CurrencyManager.Instance.Give("hints", myAmount);
                    //GameAnalytics.EarnVirtualCurrency("SpinHints", myAmount);
                    break;
                default:
                    print("Default award");
                    break;
            }
            HideRewardAnim();
            
            GameManager.Instance.multiplier = 1;
            GameManager.Instance.isspinTwoX = false;
            StartCoroutine(HideCoinsDelta());
            ScreenManager.Instance.isSpinWheel = false;
            
            if (Application.internetReachability != NetworkReachability.NotReachable)
            {
                if (UnityRemoteData.isFullAdsEnabled)
                {
                    Adbutton.SetActive(true);
                   
                }
            }
                     
        }
        // Hide coins delta text after animation
        private IEnumerator HideCoinsDelta()
        {
            yield return new WaitForSeconds(0.5f);           
            //DeltaCoinsText.gameObject.SetActive (false);           
            RewardParent.SetActive(false);
            closeBtm.SetActive(true);
            //CoinsDeltaImage.transform.parent.gameObject.SetActive(false);
            //CoinsDeltaImage.gameObject.SetActive(false);
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
        }
        
        // Animation for smooth increasing and decreasing the number of coins
        private IEnumerator UpdateCoinsAmount()
        {
            const float seconds = 0.5f; // Animation duration
            float elapsedTime = 0;

            while (elapsedTime < seconds)
            {
                //CurrentCoinsText.text = Mathf.Floor(Mathf.Lerp(_previousCoinsAmount, _currentCoinsAmount, (elapsedTime / seconds))).ToString();
                elapsedTime += Time.deltaTime;

                yield return new WaitForEndOfFrame();
            }

            _previousCoinsAmount = _currentCoinsAmount;

           // CurrentCoinsText.text = _currentCoinsAmount.ToString();
        }

        // Change remaining time to next free turn every 1 second
        private void UpdateFreeTurnTimer()
        {
            // Don't count the time if we have free turn already
            if (_isFreeTurnAvailable)
                return;

            // Update the remaining time values
            _timerRemainingHours = (int)(_nextFreeTurnTime - DateTime.Now).Hours;
            _timerRemainingMinutes = (int)(_nextFreeTurnTime - DateTime.Now).Minutes;
            _timerRemainingSeconds = (int)(_nextFreeTurnTime - DateTime.Now).Seconds;

            // If the timer has ended
            if (_timerRemainingHours <= 0 && _timerRemainingMinutes <= 0 && _timerRemainingSeconds <= 0)
            {
                NextFreeTurnTimerText.text = "Your Free Spin is Ready..!";
                // Now we have a free turn
                _isFreeTurnAvailable = true;
            }
            else
            {
                // Show the remaining time
                NextFreeTurnTimerText.text = "Next Free Spin : " + String.Format("{0:00}:{1:00}:{2:00}", _timerRemainingHours, _timerRemainingMinutes, _timerRemainingSeconds);
                // We don't have a free turn yet
                _isFreeTurnAvailable = false;
            }
        }

        private void EnableButton(GameObject button)
        {
            button.SetActive(true);
        }

        private void DisableButton(GameObject button)
        {
            button.SetActive(false);
        }

        // Function for more readable calls
        private void EnableFreeTurnButton() { EnableButton(FreeTurnButton); }
        private void DisableFreeTurnButton() { DisableButton(FreeTurnButton); }
        private void EnablePaidTurnButton() { EnableButton(PaidTurnButton); }
        private void DisablePaidTurnButton() { DisableButton(PaidTurnButton); }

        private void ShowFreeTurnButton()
        {
            FreeTurnButton.gameObject.SetActive(true);
            PaidTurnButton.gameObject.SetActive(false);
        }

        private void ShowPaidTurnButton()
        {
            PaidTurnButton.gameObject.SetActive(true);
            FreeTurnButton.gameObject.SetActive(false);
        }

        public void ButtonClickAudio()
        {
            //AudioManager.instance.ButtonClickAudio();
        }
        IEnumerator ResetButtonClick()
        {
            yield return new WaitForSeconds(1f);

            clicking = false;
        }
        public void WatchVideoForCoinButtonClick()
        {
            // avoid multiple click
            if (clicking == true) return;

            clicking = true;

            StartCoroutine(ResetButtonClick());

#if UNITY_ANDROID || UNITY_IOS
            //Configure.instance.currentReward = AD_REWARD.SPIN_COINS;
            //ShowRewardedVideoScript1.Instance.ShowRewardedVideoButtonClicked();
#endif
        }
        public void UpdateCoinData()
        {
            //_currentCoinsAmount = GameData.instance.GetPlayerCoin();
            _currentCoinsAmount = CurrencyManager.Instance.GetAmount("coins");
        }

        public void CoinsAdReward(int coinValue)
        {

            //GameData.instance.SavePlayerCoin(GameData.instance.GetPlayerCoin() + coinValue);
            //mpScene.UpdateCoinAmountLabel();
            //UpdateCoinData();
            //StartCoroutine(mpScene.RewardPopup());

        }

    }

}