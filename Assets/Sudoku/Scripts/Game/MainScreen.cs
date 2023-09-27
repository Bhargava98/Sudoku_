using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BizzyBeeGames.Sudoku
{
	public class MainScreen : Screen
	{
		#region Inspector Variables

		[Space]

		[SerializeField] private GameObject	continueButton			= null;
		[SerializeField] private GameObject	playButtonSmall		    = null;
		[SerializeField] private GameObject	playButtonBig		    = null;
		[SerializeField] private Text		continueDifficultyText	= null;
		[SerializeField] private Text		continueTimeText		= null;

        [Space]
        [Header("Main Animation")]
        [SerializeField] protected AnimationCurve animCurve;
        [SerializeField] RectTransform topBar                       = null;
        [SerializeField] RectTransform bottomBar                    = null;
        [SerializeField] RectTransform []trioButtons                = null;
        [SerializeField] RectTransform []playButtonsL               = null;
        [SerializeField] RectTransform []playButtonsS               = null;
        [SerializeField] GameObject touchBlocker                    = null;
        [Space(10)]
        [SerializeField] GameObject NoAdsButton, CoinShopButton;
        #endregion

        #region private variables
        private float topBarY = 0f;
        private float bottomBarY = 0f;
        private float []trioButtonsY =  new float[4];
        private float []playButtonsLY = new float[4];
        private float []playButtonsSY = new float[4];
        private bool isBannerLoaded;
        #endregion

        #region Public Methods
             

        public override void Show(bool back, bool immediate)
		{
			base.Show(back, immediate);


            CheckPurchaseButtons();
            touchBlocker.SetActive(true);
			continueButton.SetActive(GameManager.Instance.ActivePuzzleData != null&& !GameManager.Instance.ActivePuzzleData.isGameover);
            PopupManager.Instance.CheckCrossPromo();
            AnimateMenu();
            if (GameManager.Instance.ActivePuzzleData != null && !GameManager.Instance.ActivePuzzleData.isGameover)
            {
                //continueDifficultyText.text	= "Difficulty: " + GameManager.Instance.ActivePuzzleDifficultyStr;
                //continueTimeText.text		= "Time: " +GameManager.Instance.ActivePuzzleTimeStr;
                continueDifficultyText.text = GameManager.Instance.ActivePuzzleDifficultyStr;
                continueTimeText.text = GameManager.Instance.ActivePuzzleTimeStr;
                playButtonBig.SetActive(false);
                playButtonSmall.SetActive(true);
            }
            else {

                playButtonBig.SetActive(true);
                playButtonSmall.SetActive(false);
            }

            if (GameManager.Instance.gameOpened)
            {
                if (PlayerPrefs.GetInt("RatedGame", 0) == 0)
                    GameManager.Instance.RateInitialize();


                GameManager.Instance.ShowAdPanelAtStart();
                GameManager.Instance.gameOpened = false;
            }
            //AdsManager.Instance.ShowBanner(out isBannerLoaded);
            //if (isBannerLoaded)
            //{
            //    adjustForBannerAd = true;
            //    AdjustScreen();
            //}
            //else
            //{
            //    adjustForBannerAd = false;
            //}
        }
        public void CheckPurchaseButtons()
        {
            if (PlayerPrefs.GetInt("SHOW_ADS", 0) == 1)
            {
                CoinShopButton.SetActive(true);
                NoAdsButton.SetActive(false);
            }
            else
            {
                CoinShopButton.SetActive(false);
                NoAdsButton.SetActive(true);
            }
        }

        public void QuitButtonClick()
        {
            PopupManager.Instance.HidePopup("quit");
            StartCoroutine(HideQuitPanel());         
        }

        IEnumerator HideQuitPanel()
        {
            yield return new WaitForSeconds(0.2f);

#if UNITY_ANDROID
           
            AndroidJavaObject activity = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
            activity.Call<bool>("moveTaskToBack", true);
            //Application.Quit();
#elif UNITY_IOS
            Application.Quit();
#endif
        }

        #endregion


        #region Unity Methods
        private void Awake()
        {
            topBarY = topBar.anchoredPosition.y;
            bottomBarY = bottomBar.anchoredPosition.y;
            for (int i = 0; i < trioButtons.Length; i++)
            {
                trioButtonsY[i] = trioButtons[i].anchoredPosition.y;
            }
            for (int i = 0; i < 4; ++i)
            {
               playButtonsLY[i] = playButtonsL[i].anchoredPosition.y;
               playButtonsSY[i] = playButtonsS[i].anchoredPosition.y;
            }
        }


        //private void Update()
        //{
        //    if (Input.GetKeyDown(KeyCode.Escape) && !ScreenManager.Instance.isSpinWheel)
        //    {
        //        // First try and close an active popup (If there are any)
        //        if (!PopupManager.Instance.CloseActivePopup())
        //        {
        //            // No active popups, if we are on the home screen close the app, else go back one screen
        //            if (ScreenManager.Instance.CurrentScreenId == "main")
        //            {
        //                PopupManager.Instance.Show("quit");
        //            }
        //        }
        //    }

        //}
#endregion


#region private methods
        private void AnimateMenu()
        {
            float screenWidth = GetComponent<RectTransform>().rect.width;
            float screenHeight = GetComponent<RectTransform>().rect.height;

            UIAnimation anim = UIAnimation.PositionY(topBar, screenHeight, topBarY, 0.3f);           
            anim.animationCurve = animCurve;
            anim.style = UIAnimation.Style.Custom;
            anim.startOnFirstFrame = true;           
            anim.Play();

            anim = UIAnimation.PositionY(bottomBar, -screenHeight, bottomBarY, 0.3f);
            anim.animationCurve = animCurve;
            anim.style = UIAnimation.Style.Custom;
            anim.startOnFirstFrame = true;
            anim.Play();

            for (int i = 0; i < trioButtons.Length; ++i)
            {
                anim = UIAnimation.PositionY(trioButtons[i], screenHeight, trioButtonsY[i], 0.25f *(i+1));

                anim.animationCurve = animCurve;
                anim.style = UIAnimation.Style.Custom;
                anim.startOnFirstFrame = true;
                anim.Play();
            }
            for (int i = 0; i < 4; ++i)
            {
                if (playButtonBig.activeInHierarchy)
                    anim = UIAnimation.PositionY(playButtonsL[i], -screenHeight, playButtonsLY[i], 0.25f * (i + 1));
                else
                    anim = UIAnimation.PositionY(playButtonsS[i], -screenHeight, playButtonsSY[i], 0.25f * (i + 1));
                anim.animationCurve = animCurve;
                anim.style = UIAnimation.Style.Custom;
                anim.startOnFirstFrame = true;
                anim.Play();
            }

            StartCoroutine(setOffScreen());
            //StartCoroutine(ButtonAnim1());
        }

        //IEnumerator ButtonAnim1()
        //{
        //    float screenHeight = GetComponent<RectTransform>().rect.height;
        //    UIAnimation anim;
        //    yield return new WaitForSeconds(0.4f);
        //    for (int i = 0; i < 3; ++i)
        //    {
        //        anim = UIAnimation.PositionY(trioButtons[i], screenHeight, trioButtonsY[i], 0.25f*i );

        //        anim.animationCurve = animCurve;
        //        anim.style = UIAnimation.Style.Custom;
        //        anim.startOnFirstFrame = true;
        //        anim.Play();               
        //    }


        //}

        IEnumerator setOffScreen()
        {
            yield return new WaitForSeconds(0.5f);
            touchBlocker.SetActive(false);
        }
#endregion
    }
}
