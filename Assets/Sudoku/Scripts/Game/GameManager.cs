using MiniJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BizzyBeeGames.Sudoku
{
    public class GameManager : SingletonComponent<GameManager>, ISaveable
    {
        #region Inspector Variables

        [Header("Data")]
        [SerializeField] private List<PuzzleGroupData> puzzleGroups = null;

        [Header("Values")]
        [SerializeField] private int hintsPerCompletedPuzzle = 1;
        [SerializeField] private int numLevelsBetweenAds = 3;
        [SerializeField] private int mistakesPerPuzzle = 3;
        public int coinsPerCompletedPuzzle = 20;
        [SerializeField] private int coinsForSecondChance = 200;

        [Header("Game Settings")]
        [SerializeField] private bool showIncorrectNumbers = true;
        [SerializeField] private bool removeNumbersFromNotes = true;
        [SerializeField] private bool hideAllPlacedNumbers = true;

        [Header("Components")]
        [SerializeField] private PuzzleBoard puzzleBoard = null;
        [SerializeField] private GameScreen gameScreen = null;

        [Header("Score Calculations")]
        public float levelMaxTime = 1200; // The player has to completed the level within 30 seconds
        public float levelScore = 0.2f; // The player will be awarded of 50 points per remaining second

        [Header("Rate Us Popup")]
        public int rateLevel;
        public static int rateIncrementer = 0;
        public static int ratePer_initial = 3;
        public static int ratePer_nextSteps = 5;
        public static int ratePer_Days = 1;
        public static int rateTemp = 0;        
        public static bool rateActive = true;
        bool showingRateUs = false;

        public static bool interstitial_Controller = false; //This is to controll Menus for interstitial Ads
        #endregion

        [HideInInspector] public int buyChanceCount = 0;
        [HideInInspector] public bool stopMistakes = false;
        [HideInInspector] public bool isErased = false;
        [HideInInspector] public bool isUndo = false;
        [HideInInspector] public int TotalLevelsDone = 0;
        [HideInInspector] public int winCounter = 0;
        [HideInInspector] public int newGameCounter = 0;
        [HideInInspector] public int failCounter = 0;
        [HideInInspector] public int homeAdCounter = 0;
        [HideInInspector] public int titleAdCounter = 0;
        [HideInInspector] public int titlePlayBtnCounter = 0;
        [HideInInspector] public int AdAtFocusCounter = 0;
       
        [HideInInspector] public bool isNewGame = false;
        [HideInInspector] public bool isPopupAnimationDone = false;
        [HideInInspector] public int currMistakes = 0;
        [HideInInspector] public bool isContinue = false;
        [HideInInspector] public bool isspinTwoX = false;
        [HideInInspector] public bool isDailyRewardAvailable = false;
        [HideInInspector] public bool gameOpened = false;
        [HideInInspector] public int multiplier = 1;
        

        [Header("Floating Anim For Currency")]
        public GameObject floatingPref;
        public GameObject adshowImage;
        public GameObject watchButtoninFreePanel, watchAdIconRef;
        #region Properties

        public string SaveId { get { return "game_manager"; } }
        public bool IsPaused { get; set; }
        public PuzzleData ActivePuzzleData { get; private set; }
        public int NumLevelsTillAdd { get; private set; }

        public List<PuzzleGroupData> PuzzleGroupDatas { get { return puzzleGroups; } }
        public bool ShowIncorrectNumbers { get { return showIncorrectNumbers; } }
        public bool RemoveNumbersFromNotes { get { return removeNumbersFromNotes; } }
        public bool HideAllPlacedNumbers { get { return hideAllPlacedNumbers; } }
        public int HintsPerCompletedPuzzle { get { return hintsPerCompletedPuzzle; } }
        public int CoinsPerCompletedPuzzle { get { return coinsPerCompletedPuzzle; } }
        public bool isCellClicked = false;
        public int currentScore = 0;
        public int highScore = 0;
        public string currLevelName = "";
        public string currGroupId = "easy";

        public System.Action<string> OnGameSettingChanged { get; set; }

        public string ActivePuzzleDifficultyStr
        {
            get
            {
                if (ActivePuzzleData == null) return "";

                PuzzleGroupData puzzleGroupData = GetPuzzleGroup(ActivePuzzleData.groupId);

                if (puzzleGroupData == null) return "";

                return puzzleGroupData.displayName;
            }
        }

        public string ActivePuzzleTimeStr
        {
            get
            {
                if (ActivePuzzleData == null) return "00:00";

                return Utilities.FormatTimer(ActivePuzzleData.elapsedTime);
            }
        }

        #endregion

        #region Unity Methods

        protected override void Awake()
        {
            base.Awake();
            winCounter = failCounter = 0;
            currentScore = highScore = PlayerPrefs.GetInt("HIGH_SCORE", 0);
            ///
            UnityEngine.Screen.sleepTimeout = SleepTimeout.NeverSleep;
            ///
            isNewGame = true;
            gameOpened = true;
            SaveManager.Instance.Register(this);

            puzzleBoard.Initialize();

            for (int i = 0; i < puzzleGroups.Count; i++)
            {
                puzzleGroups[i].Load();
            }

            LoadSave();

            for (int i = 0; i < puzzleGroups.Count; i++)
            {
                TotalLevelsDone += puzzleGroups[i].PuzzlesCompleted;
            }
            //Debug.Log("Total_Levels done :"+TotalLevelsDone);
        }

        void Start()
        {
            if (Application.internetReachability != NetworkReachability.NotReachable)
            { StartCoroutine(GetDelayedRateData()); }
            else
            {
                GetRateMenuData(rateUsData);
            }
        }

        private void Update()
        {
            if (!IsPaused && ActivePuzzleData != null && ScreenManager.Instance.CurrentScreenId == "game")
            {
                ActivePuzzleData.elapsedTime += Time.deltaTime;
            }

            //if (Input.GetKeyDown(KeyCode.A))
            //{
            //    AfterFreeCoinsPanelAdWatch();
            //}
            //if (GameManager.Instance.ActivePuzzleData != null)
            //    print(GameManager.Instance.ActivePuzzleData.mistakes);
            //print("Mistakes: "+currMistakes);
        }

        #endregion

        #region Public Methods

        public void SetHighScore()
        {
            if (currentScore > highScore)
            {
                highScore = currentScore;
                PlayerPrefs.SetInt("HIGH_SCORE", highScore);
            }
        }

        public void PlayNewGame(int groupIndex)
        {
            // Make sure the groupIndex is within the bounds of puzzleGroups
            if (groupIndex >= 0 && groupIndex < puzzleGroups.Count)
            {
                PlayNewGame(puzzleGroups[groupIndex]);

                return;
            }

            Debug.LogErrorFormat("[GameManager] PlayNewGame(int groupIndex) : The given groupIndex ({0}) is out of bounds for the puzzleGroups of size {1} \"{0}\"", groupIndex, puzzleGroups.Count);
        }

        string currNewGameIndex;
        public void NewGameAfterValidation(string id)//Newly added
        {
            currNewGameIndex = id;
            PopupManager.Instance.Show("validateNew");
        }

        public void ClickedYesButton()//Validation for new Level//Newly added
        {
            ButtonClickedNewGame(currNewGameIndex);
        }
        public void OnPlayNewGameLcicked(string groupId)
        {
            ButtonClickedNewGame(groupId);
        }

        public void PlayNewGame(string groupId)
        {
            if (PlayerPrefs.GetInt("REWARD_AT_LC", 0) >= 3)
                PlayerPrefs.SetInt("REWARD_AT_LC", 0);

            newGameCounter++;
            // Get the PuzzleGroupData for the given groupId
            for (int i = 0; i < puzzleGroups.Count; i++)
            {
                PuzzleGroupData puzzleGroupData = puzzleGroups[i];

                if (groupId == puzzleGroupData.groupId)
                {
                    //if(UnityRemoteData.isAds_SelectGameMode )
                    // AdsManager.Instance.ShowInterstitial();

                    PlayNewGame(puzzleGroupData);
                    isNewGame = false;
                    return;
                }
            }

            Debug.LogErrorFormat("[GameManager] PlayNewGame(string groupId) : Could not find a PuzzleGroupData with the given id \"{0}\"", groupId);
        }

        public void NextLevel()
        {
            PlayNewGame(currGroupId);
            IsPaused = false;
        }
        public void ContinueActiveGame()
        {
            //isContinue = false;
            int tempMistakes = 0;
            if (currMistakes != 0)
                tempMistakes = currMistakes;
            else
                tempMistakes = ActivePuzzleData.mistakes;

            //if (UnityRemoteData.isAds_SelectGameMode)
            //    AdsManager.Instance.ShowInterstitial();  

            isNewGame = false;
            PlayGame(ActivePuzzleData);

            if (isContinue)
                currMistakes = tempMistakes;

            setMistakesUI();
        }

        public bool rateUsShown = false;
        string rateUsData = "{\"data\":[{\"Active\":true,\"Days_Counter\":1,\"Initial_Open_Count\":3,\"Next_Open_Count\":5}]}";

        public void RateInitialize()
        {
            rateUsShown = false;

            if (Application.internetReachability != NetworkReachability.NotReachable && PlayerPrefs.GetInt("RateUsOpened", 0) == 0 && rateActive)
            {
                if (PlayerPrefs.GetInt("Initial_Rate_Appear", 0) == 0)
                {
                    if (PlayerPrefs.GetInt("RateUsAvailableToday", 0) == 0 && PlayerPrefs.GetInt("RateCounter", 0) >= ratePer_initial)//rateIncrementer >= ratePer_initial) //2.6.2 
                    {
                        PlayerPrefs.SetInt("Initial_Rate_Appear", 1);
                        PlayerPrefs.SetInt("RateUsAvailableToday", 1);
                        PlayerPrefs.SetInt("RateCounter", 0);
                        rateTemp = 0;
                        rateUsShown = true;
                        StartCoroutine(OpenRateUsPopup());
                    }
                }
                else
                {
                    if (PlayerPrefs.GetInt("RateUsAvailableToday", 0) == 0 && PlayerPrefs.GetInt("RateCounter", 0) >= ratePer_nextSteps)
                    {
                        PlayerPrefs.SetInt("RateUsAvailableToday", 1);
                        PlayerPrefs.SetInt("RateCounter", 0);
                        rateTemp = 0;
                        rateUsShown = true;
                        StartCoroutine(OpenRateUsPopup());
                    }
                }
            }
        }

        IEnumerator OpenRateUsPopup()
        {
            showingRateUs = true;
            yield return new WaitForSeconds(0.5f);
            PopupManager.Instance.Show("rate_us");
        }

        public void ShowAdPanelAtStart()
        {
            
            StartCoroutine(DelayShowAdPanel());   
        }

        IEnumerator DelayShowAdPanel()
        {
            yield return new WaitForSeconds(0.5f);
            if (!DailyChangeScript.Instance.firstTime)
                if (!showingRateUs)
            {
                if (isDailyRewardAvailable)
                {
                    PopupManager.Instance.Show("daily_reward");
                }
                else
                {
                    if(PlayerPrefs.GetInt("SHOW_ADS", 0) == 0 && UnityRemoteData.NoAdsPanelEnabledAtStart)
                    PopupManager.Instance.Show("RemoveAds");
                }
            }
        }

        IEnumerator GetDelayedRateData()//v_0.6
        {
            yield return new WaitForSeconds(0.3f);

            if (UnityRemoteData.rateUsData != null)
            {
                GetRateMenuData(UnityRemoteData.rateUsData);
            }
            else
            {
                GetRateMenuData(rateUsData);
            }
        }

        public void GetRateMenuData(string json_info)
        {
            if (json_info != null)
            {
                Dictionary<string, object> testData = Json.Deserialize(json_info) as Dictionary<string, object>;
                List<object> data = testData["data"] as List<object>;

                for (int i = 0; i < data.Count; i++)
                {
                    rateActive = (bool)(((Dictionary<string, object>)data[i])["Active"]);
                    ratePer_Days = int.Parse(((Dictionary<string, object>)data[i])["Days_Counter"].ToString());
                    ratePer_initial = int.Parse(((Dictionary<string, object>)data[i])["Initial_Open_Count"].ToString());
                    ratePer_nextSteps = int.Parse(((Dictionary<string, object>)data[i])["Next_Open_Count"].ToString());
                }
            }
        }

        /// <summary>
        /// Check for 
        /// s done and call game over popup
        /// </summary>
        public void GameOver()
        {
            if (currMistakes >= mistakesPerPuzzle && ScreenManager.Instance.CurrentScreenId == "game")
            {
                PopupManager.Instance.Show("game_over");
                Instance.ActivePuzzleData.isGameover = true;
                return;
            }
        }

        public void RestartGame()
        {
            //print("Restart!");
            //if (UnityRemoteData.isAds_RestartGame)
            //   AdsManager.Instance.ShowInterstitial();
            currMistakes = ActivePuzzleData.mistakes = 0;
            //puzzleBoard.LoadingPopup.SetActive(true);
            GameManager.Instance.resetMistakesUI();
            ActivePuzzleData.elapsedTime = 0f;
            ActivePuzzleData.isGameover = false;
            ActivePuzzleData.is2ndChanceTaken = false;
            ActivePuzzleData.RestoreGame();
            isNewGame = false;

            //PlayGame(ActivePuzzleData);
            puzzleBoard.Setup(ActivePuzzleData);
            //puzzleBoard.AnimateSequenceAtStart();
            IsPaused = false;
        }




        public void PauseGame()
        {
            IsPaused = true;
        }
        public void UnpauseGame()
        {
            IsPaused = false;
        }

        public void setMistakesUI()
        {
            gameScreen.setMistakes();
        }
        public void resetMistakesUI()
        {
            gameScreen.resetMistakesUI();
        }

        public void SecondChanceForCoins()
        {
            if (CurrencyManager.Instance.GetAmount("coins") < coinsForSecondChance && buyChanceCount < 1)
            {
                currMistakes = 0;
                PopupManager.Instance.Show("coin_shop");

                gameScreen.SetThreeMistakes();
                //gameScreen.resetMistakesUI();
            }
            else
            {
                if (buyChanceCount < 1)
                {
                    CurrencyManager.Instance.Give("coins", -coinsForSecondChance);
                    //GameAnalytics.SpendVirtualCurrency("Second Chance", 1, "Coins", coinsForSecondChance);
                    currMistakes = 0;
                    buyChanceCount++;
                    gameScreen.setMistakes();
                    gameScreen.resetMistakesUI();
                    ActivePuzzleData.is2ndChanceTaken = true;
                    PopupManager.Instance.HidePopup("game_over");
                    IsPaused = false;
                }
            }
        }

        public void SecondChanceForAdWatch()
        {
            //if (PopupManager.Instance.NoInternetNativePopup("Network", "No Internet Access") == false)
                //AdsManager.Instance.ShowRewardedAd(new object[] { "continue_game" });

        }



        public void WatchedTheAd()
        {
            currMistakes = ActivePuzzleData.mistakes = 0;
            gameScreen.setMistakes();
            gameScreen.resetMistakesUI();
            ActivePuzzleData.is2ndChanceTaken = true;
            PopupManager.Instance.HidePopup("game_over");
        }

        /// <summary>
        /// Free coins watch ad panel 
        /// </summary>
        public void FreeCoinPanelReqREWAd()
        {

            //if (PopupManager.Instance.NoInternetNativePopup("Network", "No Internet Access") == false)
            //    AdsManager.Instance.ShowRewardedAd(new object[] { "freeCoinsPanel" });
        }


        public void AfterFreeCoinsPanelAdWatch()
        {
            watchButtoninFreePanel.GetComponent<Button>().interactable = false;
            CurrencyManager.Instance.SetAnimObjsGamePlay(watchAdIconRef);
            //CurrencyManager.Instance.GiveCoins(AdsManager.Instance.freeCoinsForAdWatch, 0.5f, true);
            StartCoroutine(SetOffFreepanel());
        }

        IEnumerator SetOffFreepanel()
        {
            yield return new WaitForSeconds(0.8f);
            PopupManager.Instance.HidePopup("FreeCoinsPanel");
            watchButtoninFreePanel.GetComponent<Button>().interactable = true;
        }
        ///--------------------->


        public void SetGameSetting(string setting, bool value)
        {
            switch (setting)
            {
                case "mistakes":
                    showIncorrectNumbers = value;
                    break;
                case "notes":
                    removeNumbersFromNotes = value;
                    break;
                case "numbers":
                    hideAllPlacedNumbers = value;
                    break;
            }

            if (OnGameSettingChanged != null)
            {
                OnGameSettingChanged(setting);
            }
        }

        public void ActivePuzzleCompleted()
        {
            // Get the PuzzleGroupData for the puzzle
            PuzzleGroupData puzzleGroup = GetPuzzleGroup(ActivePuzzleData.groupId);
            float elapsedTime = ActivePuzzleData.elapsedTime;

            float timeScore = Mathf.Max(0, levelMaxTime - elapsedTime) * levelScore;
            currentScore += (int)timeScore;
            //Debug.Log("Current Score is: " + currentScore);
            SetHighScore();
            // print("CurrentScore ="+currentScore+"TimeScore="+ timeScore);
            // currentScore+=
            // Set the puzzle data to null now so the game can't be continued
            ActivePuzzleData = null;

            puzzleGroup.PuzzlesCompleted += 1;
            puzzleGroup.TotalTime += elapsedTime;

            //Social.ReportScore(currentScore, GPGSIds.leaderboard_top_player, (bool success) => {
            //    Debug.Log("Reported Score: " + currentScore);
            //});

            bool newBest = false;
            currGroupId = puzzleGroup.groupId;
            //switch (puzzleGroup.groupId)
            //{
            //    case "easy":
            //        PluginManager.Instance.Easy_LevelAchievement(puzzleGroup.PuzzlesCompleted);
            //        break;
            //    case "medium":
            //        PluginManager.Instance.Medium_LevelAchievement(puzzleGroup.PuzzlesCompleted);
            //        break;
            //    case "hard":
            //        PluginManager.Instance.Hard_LevelAchievement(puzzleGroup.PuzzlesCompleted);
            //        break;
            //    case "expert":
            //        PluginManager.Instance.Expert_LevelAchievement(puzzleGroup.PuzzlesCompleted);
            //        break;
            //}
            if (puzzleGroup.MinTime == 0 || elapsedTime < puzzleGroup.MinTime)
            {
                newBest = true;
                puzzleGroup.MinTime = elapsedTime;
            }


            // Award the player their hint
            //CurrencyManager.Instance.Give("coins", coinsPerCompletedPuzzle);

            DailyMissionController.instance.MissionProgressUpdate(1, DailyMissionTypes.Levels);

            object[] popupData =
            {
                puzzleGroup.displayName,
                elapsedTime,
                puzzleGroup.MinTime,
                newBest
            };


            // Show the puzzle complete popup
            PopupManager.Instance.Show("puzzle_complete", popupData, (bool cancelled, object[] outData) =>
            {
                // If the popup was closed without the cancelled flag being set then the player selected New Game
                //if (!cancelled && puzzleGroup != null)
                //{
                //	PlayNewGame(puzzleGroup);
                //}
                //// Else go back to the main screen
                //else
                //{
                //	ScreenManager.Instance.Back();
                //}               
            });


            //GameAnalytics.EarnVirtualCurrency("LevelDoneAwardedCoins", coinsPerCompletedPuzzle);
            //GameAnalytics.LevelEnd((int)currentScore);

        }

        void ShowInterstAd_NewGame()
        {
            //if (UnityRemoteData.isAds_newGameEnabled && (newGameCounter >= UnityRemoteData.adsNewGameCounter))
            //{
            //    AdsManager.Instance.INT_NewGameClicked.ShowInterstitial();
            //}

        }


        void ShowInterstAd_Win()
        {
            //if (UnityRemoteData.isAds_WinEnabled && (winCounter >= UnityRemoteData.adsWinCounter))
            //{
            //    AdsManager.Instance.INT_LevelWin.ShowInterstitial();
            //}

        }

        void ShowInterstAd_Fail()
        {
            //if (UnityRemoteData.isAds_FailEnabled && (failCounter >= UnityRemoteData.adsFailCounter))
            //{
            //    AdsManager.Instance.INT_LevelWin.ShowInterstitial();
            //}
        }

        public void ShowInterstAd_OnTitle()
        {
            if (UnityRemoteData.isAds_AtTitleEnabled && (GameManager.Instance.titleAdCounter >= UnityRemoteData.adsAtTitleCounter))
            {
               // AdsManager.Instance.INT_ToTitlePage.ShowInterstitial();
            }
        }

        //public void ShowInterstAd_OnPlayButtonTitle()
        //{
        //    if (UnityRemoteData.isAds_AtTitlePlayBtnEnabled && (GameManager.Instance.titlePlayBtnCounter >= UnityRemoteData.adsAtTitlePlayBtnCounter))
        //    {
        //        AdsManager.Instance.INT_TitlePlayClicked.ShowInterstitial();
        //    }
        //}

        public void ShowInitAtGameFocus()
        {
            //if (UnityRemoteData.isAds_AtGameFocusEnabled && (GameManager.Instance.AdAtFocusCounter >= UnityRemoteData.adsAtGameFocusCounter))
            //{
            //    AdsManager.Instance.INT_AfterFocused.ShowInterstitial();
            //}
        }

        public void ButtonClickGameover(string menu)
        {
            StartCoroutine(OpenAtGameover(menu));
        }
        public void ButtonClickAfterComplete(string menu)
        {
            StartCoroutine(OpenAfterLevelComplete(menu));
        }
        public void ButtonClickAfterHomeClicked()
        {
            //StartCoroutine(AdAtTitleDelayed());
        }
        void ButtonClickedNewGame(string id)
        {
            StartCoroutine(AdAtNewGameStartDelayed(id));
        }
        public void ButtonPlayClickedTitle()
        {
            //StartCoroutine(AdAtPlayClickTitleDelayed());
        }

        public void ContinueButtonclicked()
        {
            StartCoroutine(AdAtcontinueGameDelayed());
        }
        bool bannerAdShow;
        IEnumerator OpenAtGameover(string menuName)
        {
            // yield return new WaitForSeconds(0.26f);
            if (UnityRemoteData.isAds_FailEnabled && (failCounter >= UnityRemoteData.adsFailCounter))
                PopupManager.Instance.isInterst = true;

            PopupManager.Instance.HidePopup("game_over");
            if (Application.internetReachability != NetworkReachability.NotReachable /*&& AdsManager.Instance.showAds != 1*/)
            {
                
                StartCoroutine(ShowPopupWithDelay());

                while (isPopupAnimationDone == false)
                    yield return null;

                ShowInterstAd_Fail();

                while (interstitial_Controller)
                    yield return null;
                adshowImage.SetActive(false);
            }
            if (menuName == "RetryGame")
            {
                GameManager.Instance.RestartGame();
                //AdsManager.Instance.ShowBanner(out bannerAdShow);
            }
            else if (menuName == "Home")
            {
                //AdsManager.Instance.HideBanner();
                ScreenManager.Instance.BackTo("main");
            }

        }

        IEnumerator OpenAfterLevelComplete(string menuName)
        {
            if (UnityRemoteData.isAds_WinEnabled && (winCounter >= UnityRemoteData.adsWinCounter))
                PopupManager.Instance.isInterst = true;

            PopupManager.Instance.HidePopup("puzzle_complete");

            if (Application.internetReachability != NetworkReachability.NotReachable /*&& AdsManager.Instance.showAds != 1*/)
            {
                
                StartCoroutine(ShowPopupWithDelay());

                while (isPopupAnimationDone == false)
                    yield return null;

                //yield return new WaitForSeconds(0.25f);
                ShowInterstAd_Win();

                while (interstitial_Controller)
                    yield return null;
                adshowImage.SetActive(false);
            }
            if (menuName == "NextLevel")
            {
                NextLevel();
            }
            else if (menuName == "main")
            {
                ScreenManager.Instance.BackTo("main");
            }

        }

        IEnumerator AdAtTitleDelayed()
        {
            ShowInterstAd_OnTitle();

            while (interstitial_Controller)
                yield return null;

            ScreenManager.Instance.Show("login");
        }

        //IEnumerator AdAtPlayClickTitleDelayed()
        //{
        //    if (AdsManager.Instance.INT_TitlePlayClicked.IsInitLoaded())
        //    {
        //        ScreenManager.Instance.loginScreen.Hide(false, true);
        //    }
        //    yield return new WaitForSeconds(0.2f);
        //    ShowInterstAd_OnPlayButtonTitle();
           
        //    while (interstitial_Controller)
        //        yield return null;

        //    ScreenManager.Instance.Show("main");
        //}

        IEnumerator AdAtNewGameStartDelayed(string index)
        {
            //if (AdsManager.Instance.INT_NewGameClicked.IsInitLoaded())
            {
                PopupManager.Instance.HidePopup("validateNew");
                ScreenManager.Instance.mainMenu.Hide(false, true);
            }
            yield return new WaitForSeconds(0.2f);
            ShowInterstAd_NewGame();

            
            while (interstitial_Controller)
                yield return null;

            PlayNewGame(index);

        }

        IEnumerator AdAtcontinueGameDelayed()
        {
            //if (AdsManager.Instance.INT_NewGameClicked.IsInitLoaded())
            {               
                ScreenManager.Instance.mainMenu.Hide(false, true);
            }
            yield return new WaitForSeconds(0.2f);
            ShowInterstAd_NewGame();


            while (interstitial_Controller)
                yield return null;

            ContinueActiveGame();

        }


        private IEnumerator ShowPopupWithDelay()
        {
            yield return new WaitForSeconds(0.0f);
            
            adshowImage.SetActive(true);
        }
        #endregion

        #region Private Methods

        /// <summary>
        /// Gets a new PuzzleData from the given PuzzleGroupData and sets up the game to play it
        /// </summary>
        private void PlayNewGame(PuzzleGroupData puzzleGroupData)
        {
            // Get a puzzle that has not yet been played by the user
            PuzzleData puzzleData = puzzleGroupData.GetPuzzle();
            puzzleData.puzzleName = currLevelName;

            // Play the game using the new puzzle data
            PlayGame(puzzleData);
            GameManager.Instance.currMistakes = 0;
            GameManager.Instance.resetMistakesUI();
        }

        /// <summary>
        /// Starts the game using the given PuzzleData
        /// </summary>
        private void PlayGame(PuzzleData puzzleData)
        {
            // Set the active puzzle dat
            ActivePuzzleData = puzzleData;
            //GameManager.Instance.ActivePuzzleData.mistakes = 0;
            // Setup the puzzle board to display the numbers

            puzzleBoard.Setup(puzzleData);
            // puzzleBoard.AnimateSequenceAtStart();
            // Show the game screen
            ScreenManager.Instance.Show("game");
            
#if UNITY_ANDROID || UNITY_IOS
            //GameAnalytics.LevelStart(ActivePuzzleData.puzzleName);
           
#endif

            NumLevelsTillAdd++;

            if (NumLevelsTillAdd > numLevelsBetweenAds)
            {
                //if (MobileAdsManager.Instance.ShowInterstitialAd(null))
                //{
                //	NumLevelsTillAdd = 0;
                //}
            }
        }

        /// <summary>
        /// Gets the puzzle group with the given id
        /// </summary>
        private PuzzleGroupData GetPuzzleGroup(string id)
        {
            for (int i = 0; i < puzzleGroups.Count; i++)
            {
                PuzzleGroupData puzzleGroup = puzzleGroups[i];

                if (id == puzzleGroup.groupId)
                {
                    return puzzleGroup;
                }
            }

            return null;
        }



        #endregion

        #region Save Methods

        public Dictionary<string, object> Save()
        {
            Dictionary<string, object> saveData = new Dictionary<string, object>();


            // Save the active puzzle if there is one
            if (ActivePuzzleData != null)
            {
                ActivePuzzleData.mistakes = currMistakes;
                saveData["activePuzzle"] = ActivePuzzleData.Save();
            }

            // Save all the puzzle groups data
            List<object> savedPuzzleGroups = new List<object>();

            for (int i = 0; i < puzzleGroups.Count; i++)
            {
                PuzzleGroupData puzzleGroup = puzzleGroups[i];
                Dictionary<string, object> savedPuzzleGroup = new Dictionary<string, object>();

                savedPuzzleGroup["id"] = puzzleGroup.groupId;
                savedPuzzleGroup["data"] = puzzleGroup.Save();

                savedPuzzleGroups.Add(savedPuzzleGroup);
            }

            saveData["savedPuzzleGroups"] = savedPuzzleGroups;

            // Save the game settings
            saveData["showIncorrectNumbers"] = showIncorrectNumbers;
            saveData["removeNumbersFromNotes"] = removeNumbersFromNotes;
            saveData["hideAllPlacedNumbers"] = hideAllPlacedNumbers;
            saveData["NumLevelsTillAdd"] = NumLevelsTillAdd;

            return saveData;
        }

        private bool LoadSave()
        {
            JSONNode json = SaveManager.Instance.LoadSave(this);

            if (json == null)
            {
                return false;
            }

            // If there is a saved active puzzle load it
            if (json.AsObject.HasKey("activePuzzle"))
            {
                ActivePuzzleData = new PuzzleData(json["activePuzzle"]);
            }

            // Load the saved group data
            JSONArray savedPuzzleGroups = json["savedPuzzleGroups"].AsArray;

            for (int i = 0; i < savedPuzzleGroups.Count; i++)
            {
                JSONNode savedPuzzleGroup = savedPuzzleGroups[i];
                PuzzleGroupData puzzleGroup = GetPuzzleGroup(savedPuzzleGroup["id"].Value);

                if (puzzleGroup != null)
                {
                    puzzleGroup.Load(savedPuzzleGroup["data"]);
                }
            }

            // Load the game settings
            showIncorrectNumbers = json["showIncorrectNumbers"].AsBool;
            removeNumbersFromNotes = json["removeNumbersFromNotes"].AsBool;
            hideAllPlacedNumbers = json["hideAllPlacedNumbers"].AsBool;
            NumLevelsTillAdd = json["NumLevelsTillAdd"].AsInt;

            return true;
        }

        #endregion
    }
}
