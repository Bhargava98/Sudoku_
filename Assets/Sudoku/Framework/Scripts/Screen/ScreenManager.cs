using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using BizzyBeeGames.Sudoku;

namespace BizzyBeeGames
{
	public class ScreenManager : SingletonComponent<ScreenManager>
	{
		#region Inspector Variables

		[Tooltip("The home screen to of the app, ei. the first screen that shows when the app starts.")]
		[SerializeField] private string homeScreenId = "main";

		[Tooltip("The list of Screen components that are used in the game.")]
		[SerializeField] private List<Screen> screens = null;

        public Screen mainMenu;
        public MainScreen mainScreen;
        public Animator titleAnimCtrl;
		#endregion

		#region Member Variables

		// Screen id back stack
		private List<string> backStack;

        
		// The screen that is currently being shown
		private Screen	currentScreen;
		private bool	isAnimating;
        public bool isSpinWheel;

        public GameObject loadingScreen;
		#endregion

		#region Properties

		public string HomeScreenId		{ get { return homeScreenId; } }
		public string CurrentScreenId	{ get { return currentScreen == null ? "" : currentScreen.Id; } }

		#endregion

		#region Properties

		/// <summary>
		/// Invoked when the ScreenController is transitioning from one screen to another. The first argument is the current showing screen id, the
		/// second argument is the screen id of the screen that is about to show (null if its the first screen). The third argument id true if the screen
		/// that is being show is an overlay
		/// </summary>
		public System.Action<string, string> OnSwitchingScreens;

		/// <summary>
		/// Invoked when ShowScreen is called
		/// </summary>
		public System.Action<string> OnShowingScreen;

        #endregion

        #region Unity Methods
        public bool isTerms = false;
        private void Start()
		{
           


        /*    if (!PlayerPrefs.HasKey("TERMS_OF_SERVICE") || PlayerPrefs.GetInt("TERMS_OF_SERVICE", 0) == 0)
            {
                PluginManager.Instance.isSignOut = false;
                isTerms = true;
                PlayerPrefs.SetInt("TERMS_OF_SERVICE", 1);
#if UNITY_ANDROID
                NativeDialog dialog = new NativeDialog("Hi There",
                    "To play this game you must accept our Terms of Services." +
                    "Review our Terms and Privacy Notice about how your data is processed.", "TERMS", "OK"
                   );
                dialog.init();
#elif UNITY_IOS
//                PluginManager.Instance.isSignOut = false;
//MobileNativePopups.OpenAlertDialog(
//"Hi There", "To play this game you must accept our Terms of Services. To review our terms,and the Privacy Notice about how your data is processed, tap \"Terms\" below",
//                "TERMS",
//"OK", 
//() =>         {
//                     if (Application.internetReachability == NetworkReachability.NotReachable)
//                    {
//                        Application.OpenURL("https://www.9gape.com/terms-of-service/");
//                    }
//                    else
//                    {
//                       MobileNativePopups.OpenAlertDialog(
//"Network", "No Internet Access",
//"OK", 
//() => { Debug.Log("Accept was pressed"); });
#endif
            }*/

            backStack = new List<string>();
            Application.targetFrameRate = 60;
			// Initialize and hide all the screens
			for (int i = 0; i < screens.Count; i++)
			{
				Screen screen = screens[i];

				// Add a CanvasGroup to the screen if it does not already have one
				if (screen.gameObject.GetComponent<CanvasGroup>() == null)
				{
					screen.gameObject.AddComponent<CanvasGroup>();
				}

				// Force all screens to hide right away
				screen.Initialize();
                if(i!=3)
				screen.gameObject.SetActive(true);
				screen.Hide(false, true);
			}

            //if (!isTerms)
            //    ShowGameStartAd();
            // Show the home screen when the app starts up

            loadingScreen.SetActive(false);
            screens[0].gameObject.SetActive(true);
            Show(homeScreenId, false, true);

            Show(homeScreenId, false, true);
        }

        public void ShowGameStartAd()
        {
            StartCoroutine(AfterLoading());
        }

        float adDelay = 0;

        IEnumerator AfterLoading()
        {
            
            if (Application.internetReachability == NetworkReachability.NotReachable)
                adDelay = 3;
            else {
                adDelay = 5;                
            }

            if (adDelay == 5)
            {
                yield return new WaitForSeconds(3f);
                titleAnimCtrl.SetBool("isClose",true) ;
                adDelay = 1.3f;
            }
            else
            {
                yield return new WaitForSeconds(2f);
                titleAnimCtrl.SetBool("isClose",true);
                adDelay = 1f;
            }

            yield return new WaitForSeconds(adDelay);
                       
                //if (AdsManager.Instance.INT_AtGameStart.IsInitLoaded())
                    StartCoroutine(AtAfterGameStarts());
                //else
                //{
                //    loadingScreen.SetActive(false);
                //    screens[0].gameObject.SetActive(true);
                //    Show(homeScreenId, false, true);
                //}
            
        }

        IEnumerator AtAfterGameStarts()
        {

            if(UnityRemoteData.isAds_AtGameStartEnabled && 
                PlayerPrefs.GetInt(UnityRemoteData.Ad_AtGameStart_counter,0)>=UnityRemoteData.adsAtGameStartCounter)
                //AdsManager.Instance.INT_AtGameStart.ShowInterstitial();

            loadingScreen.SetActive(false);
            while (GameManager.interstitial_Controller)
                yield return null;
                        
            screens[0].gameObject.SetActive(true);
            Show(homeScreenId, false, true);
        }


        bool isClicked = false;

        IEnumerator ResetBackButton()
        {
            isClicked = true;
            yield return new WaitForSeconds(0.5f);
            isClicked = false;
        }

        private void Update()
		{
           
            if (Input.GetKeyDown(KeyCode.Escape) && !ScreenManager.Instance.isSpinWheel )
            {               
                    // First try and close an active popup (If there are any)
                    if (!PopupManager.Instance.CloseActivePopup())
                    {
                        // No active popups, if we are on the home screen close the app, else go back one screen
                        if (CurrentScreenId == HomeScreenId)
                        {
                            PopupManager.Instance.Show("quit");
                            //PopupManager.Instance.ClearActivePopups();
                        }
                        else if (CurrentScreenId == "game")
                        {

                            if (isClicked)
                                return;

                            isClicked = true;
                            StartCoroutine(ResetBackButton());
                            GameManager.Instance.PauseGame();
                            PopupManager.Instance.Show("pause");
                        }
                        else
                        {
                            if (CurrentScreenId != HomeScreenId)
                                Back();
                        }
                    
                    }

                if (CurrentScreenId == "game")
                {
                    if (PopupManager.Instance.activePopups.Count == 0)
                    {
                        GameManager.Instance.IsPaused = false;
                    }
                }
            }

        }

        #endregion

        #region Public Methods

        public void SendButtonAnalytics(string str)
        {
            string[] strs = str.Split('-');
            //GameAnalytics.ButtonTracks(strs[0],strs[1]);
        }


        public void Show(string screenId)
		{
            PopupManager.Instance.CloseActivePopup();
               
                    if (CurrentScreenId == screenId)
                    {
                        return;
                    }

                    Show(screenId, false, false);
           
		}

		public void Back()
		{
           
			if (backStack.Count <= 0)
			{
				Debug.LogWarning("[ScreenController] There is no screen on the back stack to go back to.");

				return;
			}

			// Get the screen id for the screen at the end of the stack (The last shown screen)
			string screenId = backStack[backStack.Count - 1];

			// Remove the screen from the back stack
			backStack.RemoveAt(backStack.Count - 1);

			// Show the screen
			Show(screenId, true, false);
		}

		/// <summary>
		/// Navigates to the screen in the back stack with the given screen id
		/// </summary>
		public void BackTo(string screenId)
		{
			for (int i = backStack.Count - 1; i >= 0; i--)
			{
				if (screenId == backStack[i])
				{
					Back();

					return;
				}
				else
				{
					backStack.RemoveAt(i);
				}
			}

			// If we get here then the screen was not found to just go to home
			Home();
		}

		public void Home()
		{
			if (CurrentScreenId == "main")
			{
				return;
			}

			Show(homeScreenId, true, false);
			ClearBackStack();
		}

		#endregion

		#region Private Methods

		private void Show(string screenId, bool back, bool immediate)
		{
			// Get the screen we want to show
			Screen screen = GetScreenById(screenId);

			if (screen == null)
			{
				Debug.LogError("[ScreenController] Could not find screen with the given screenId: " + screenId);

				return;
			}

			// Check if there is a current screen showing
			if (currentScreen != null)
			{
				// Hide the current screen
				currentScreen.Hide(back, immediate);

				if (!back)
				{
					// Add the screens id to the back stack
					backStack.Add(currentScreen.Id);
				}

				if (OnSwitchingScreens != null)
				{
					OnSwitchingScreens(currentScreen.Id, screenId);
				}
			}

			// Show the new screen
			screen.Show(back, immediate);

			// Set the new screen as the current screen
			currentScreen = screen;

			if (OnShowingScreen != null)
			{
				OnShowingScreen(screenId);
			}
		}

		private void ClearBackStack()
		{
			backStack.Clear();
		}

		private Screen GetScreenById(string id)
		{
			for (int i = 0; i < screens.Count; i++)
			{
				if (id == screens[i].Id)
				{
					return screens[i];
				}
			}

			Debug.LogError("[ScreenTransitionController] No Screen exists with the id " + id);

			return null;
		}

		#endregion
	}
}
