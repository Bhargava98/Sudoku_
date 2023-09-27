using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BizzyBeeGames.Sudoku
{
    public class GameScreen : Screen
    {
        #region Inspector Variables

        [Space]

        [SerializeField] private Text difficultyText = null;
        [SerializeField] private Text timeText = null;
        // [SerializeField] private List<GameObject> mistakesUIs = null;
        [SerializeField] private Text mistakesText = null;
        [SerializeField] private RectTransform mistakesUI = null;
        [SerializeField] private float playerMaxInactiveTime = 8f;

        //[Header("Main Animation")]
        //[SerializeField] protected AnimationCurve animCurve;
        //[SerializeField] RectTransform gameBoard = null;
        //[SerializeField] GameObject touchBlocker = null;
        #endregion

        #region private variables
        private float topBarY = 0f;
        private float bottomBarY = 0f;
        private bool isBannerLoaded;
        #endregion

        #region Unity Methods
        private void Awake()
        {
            //topBarY = topBar.anchoredPosition.x;
            //bottomBarY = bottomBar.anchoredPosition.x;
        }


        private void Update()
        {
            timeText.text = GameManager.Instance.ActivePuzzleTimeStr;
        }

        #endregion

        #region Public Methods

        public override void Show(bool back, bool immediate)
        {
            base.Show(back, immediate);

            //GameAnalytics.RequestAdsWithDelay();
            
            //isFocused = false;
            //touchBlocker.SetActive(true);
            //  print(GameManager.Instance.ActivePuzzleData.mistakes);

            //if (GameManager.Instance.ActivePuzzleData.mistakes ==0)
            //    resetMistakesUI();

           
            //if (isBannerLoaded)
            //{
            //    adjustForBannerAd = true;
            //    AdjustScreen();
            //}
            //else
            //{
            //    adjustForBannerAd = false;
            //}

            if (GameManager.Instance.TotalLevelsDone == 0 && GameManager.Instance.isNewGame && PlayerPrefs.GetInt("HOW_TO_PLAY", 0) == 0)
            {
                PopupManager.Instance.Show("how_to_play");

            }
            else
            {
                //AdsManager.Instance.ShowBanner(out isBannerLoaded);
            }

            GameManager.Instance.IsPaused = false;
            if (GameManager.Instance.ActivePuzzleData != null)
            {
                difficultyText.text = GameManager.Instance.ActivePuzzleDifficultyStr;
            }
        }

        public override void Hide(bool back, bool immediate)
        {
            base.Hide(back, immediate);
           
            //AdsManager.Instance.HideBanner();
            GameManager.Instance.isContinue = false;
        }

        //float clickTimer = 0f;
        //private void FixedUpdate()
        //{
        //    clickTimer += Time.deltaTime;

        //    if (Input.touchCount > 0)
        //    {
        //        clickTimer = 0f;
        //    }

        //    if()

        //}
        //IEnumerator setOffScreen()
        //{
        //    yield return new WaitForSeconds(0.5f);
        //    touchBlocker.SetActive(false);
        //}
        public void resetMistakesUI()
        {
            //for (int i = 0; i < mistakesUIs.Count; ++i)
            //    mistakesUIs[i].SetActive(false);
            mistakesText.text = "0/3";           
        }

        public void SetThreeMistakes()
        {
            mistakesText.text = "3/3";
        }

        public void setMistakes()
        {
           
            int mistakes = 0;
            if (GameManager.Instance.ActivePuzzleData.mistakes > 0)
            {
                mistakes = GameManager.Instance.ActivePuzzleData.mistakes;
            }
            else
            {
                mistakes = GameManager.Instance.currMistakes;
            }
            //if (isFocused)
            //{
            //    mistakes = GameManager.Instance.currMistakes;
            //}

            //print("MistakesUISET: "+mistakes);
            mistakesText.text = mistakes + "/3";

            if(GameManager.Instance.IsPaused == false)
            StartCoroutine(ZoomAnimation(0.2f));
            //switch (mistakes)
            //{
            //    case 1:
            //        mistakesUIs[0].SetActive(true);
            //        break;
            //    case 2:
            //        mistakesUIs[0].SetActive(true);
            //        mistakesUIs[1].SetActive(true);
            //        break;
            //    case 3:
            //        mistakesUIs[0].SetActive(true);
            //        mistakesUIs[1].SetActive(true);
            //        mistakesUIs[2].SetActive(true);
            //        break;                
            //}
        }
                

        IEnumerator ZoomAnimation(float animSpeed)
        {
            int i = 0;
            float toX = 1f, fromX = 1.2f;

            while (i <= 3)
            {
                UIAnimation anim = null;

                anim = UIAnimation.ScaleX(mistakesUI, toX, fromX, animSpeed);
                anim.style = UIAnimation.Style.Linear;
                anim.startOnFirstFrame = true;
                anim.Play();

                anim = UIAnimation.ScaleY(mistakesUI, toX,  fromX, animSpeed);
                anim.style = UIAnimation.Style.Linear;
                anim.startOnFirstFrame = true;
                anim.Play();              
                yield return new WaitForSeconds(animSpeed);
                i++;
                toX = toX + fromX;
                fromX = toX - fromX;
                toX = toX - fromX;
            }
           
        }
        #endregion


        private void OnApplicationPause(bool focus)
        {
            if (!focus)
            {
                GameManager.Instance.ActivePuzzleData.mistakes = 0;

               // if(!GameManager.interstitial_Controller)
               // GameManager.Instance.ShowInitAtGameFocus();
            }
        }
    }
}
