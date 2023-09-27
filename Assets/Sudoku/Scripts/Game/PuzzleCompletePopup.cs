using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BizzyBeeGames.Sudoku
{
    public class PuzzleCompletePopup : Popup
    {
        #region Inspector Variables

        [Space]

        [SerializeField] private Text difficultyNameText = null;
        [SerializeField] private Text puzzleTimeText = null;
        [SerializeField] private Text bestTimeText = null;
        [SerializeField] private GameObject newBestIndicator = null;
        [SerializeField] private Text awardedHintsText = null;
        [SerializeField] private GameObject startObj, endObj = null;

        [Space(10)]
        [SerializeField] Slider rewardSlider = null;
        [SerializeField] Text   levelsDoneText = null;
       
        public int getRewardAfterLevel = 5;
        [SerializeField] RewardItems[] rewards = new RewardItems[2];
        [Space(10)]
        [SerializeField] GameObject rewardParent = null;
        [SerializeField] Text rewardedAmount = null;
        [SerializeField] Image rewardedImage = null;
        [SerializeField] GameObject OrigGiftBox = null;
        [SerializeField] GameObject GiftBoxAnim = null;
        [SerializeField] GameObject bGFadeForReward = null;
        [SerializeField] Text   lvlcountText = null;
        #endregion

        #region 
        private int currRewardIndex;
        int currRewardStatus;

        Color themeColor;
        #endregion

        #region Public Methods

        private void OnEnable()
        {
            currRewardStatus = PlayerPrefs.GetInt("REWARD_AT_LC", 0);
            
            //Per Level Reward
            
            if (currRewardStatus == 0)
            {
                rewardSlider.value = 0;
            }

            PlayerPrefs.SetInt("REWARD_AT_LC", currRewardStatus + 1);

            levelsDoneText.text = (currRewardStatus + 1) + "/" + getRewardAfterLevel;

            if (currRewardStatus >= 3)
            {
                levelsDoneText.text = "Rewarded!";
                currRewardIndex = 0;
                rewardSlider.value = 1f;
                OrigGiftBox.GetComponent<Animation>().Play();
                bGFadeForReward.SetActive(true);

                PlayerPrefs.SetInt("REWARD_AT_LC", 0);
                switch (currRewardIndex)
                {
                    case 0:
                        CurrencyManager.Instance.GiveCoins(rewards[0].cost);
                        break;
                    case 1:
                        // CurrencyManager.Instance.GiveHints(rewards[1].cost);
                        break;
                }
                
                StartCoroutine(CollectYourReward());
                // lvlcountText.color = Color.white;
                //OrigGiftBox.SetActive(false);
                //GiftBoxAnim.SetActive(true);
            }
            else
            {
                if (rewardSlider.value < 1)
                    StartCoroutine(GradualIncreaseValue((1f / getRewardAfterLevel) * (currRewardStatus + 1)));

            }
            CurrencyManager.Instance.SetAnimObjsGamePlay(startObj);
            CurrencyManager.Instance.Give("coins", GameManager.Instance.coinsPerCompletedPuzzle, 0.5f, true);
        }

        public override void OnShowing(object[] inData)
        {
            base.OnShowing(inData);

            string groupName = (string)inData[0];
            float puzzleTime = (float)inData[1];
            float bestTime = (float)inData[2];
            bool newBest = (bool)inData[3];

            ThemeManager.Instance.GetItemColor("after_level_reward", out themeColor);


            difficultyNameText.text = groupName;
            puzzleTimeText.text = " " + Utilities.FormatTimer(puzzleTime);
            bestTimeText.text = " " + Utilities.FormatTimer(bestTime);
            GameManager.Instance.winCounter++;
            newBestIndicator.SetActive(newBest);
            GameManager.Instance.TotalLevelsDone++;
            

            OrigGiftBox.SetActive(true);
            GiftBoxAnim.SetActive(false);
            bGFadeForReward.SetActive(false);
           // lvlcountText.color = themeColor;

            OrigGiftBox.GetComponent<Animation>().Stop();
           
            
            //GameAnalytics.SetUserPropertyForUnlockedLevel(GameManager.Instance.TotalLevelsDone);
            //awardedHintsText.text = "+" + GameManager.Instance.HintsPerCompletedPuzzle;
            awardedHintsText.text = "+" + GameManager.Instance.CoinsPerCompletedPuzzle + " Coins";

        }

        /// <summary>
        /// Gradually increases the reward progress bar
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        IEnumerator GradualIncreaseValue(float val)
        {            
            while (rewardSlider.value < val)
            {
                rewardSlider.value += 0.008f;
                yield return null;
            }
            levelsDoneText.text = (currRewardStatus + 1) + "/" + getRewardAfterLevel;
            if (rewardSlider.value >=1)
            {
                
                levelsDoneText.text = "Rewarded!";
                currRewardIndex = 0;
                OrigGiftBox.GetComponent<Animation>().Play();
                bGFadeForReward.SetActive(true);
                PlayerPrefs.SetInt("REWARD_AT_LC", 0);
                switch (currRewardIndex)
                {
                    case 0:
                        CurrencyManager.Instance.GiveCoins(rewards[0].cost);
                        break;
                    case 1:
                        // CurrencyManager.Instance.GiveHints(rewards[1].cost);
                        break;
                }
                StartCoroutine(CollectYourReward());
                //OrigGiftBox.SetActive(false);
                //GiftBoxAnim.SetActive(true);
            }
        }

        /// <summary>
        /// Gradually Decreases the reward progress bar
        /// </summary>
        /// <returns></returns>
        void GradualDecreaseValue()
        {
            bGFadeForReward.SetActive(false);
            
            //while (rewardSlider.value > 0.005f)
            //{
            //    rewardSlider.value -= 0.06f;
            //    yield return null;
            //}

            //rewardSlider.value = 0f;
            OrigGiftBox.SetActive(true);
            OrigGiftBox.GetComponent<Animation>().Stop();
            GiftBoxAnim.SetActive(false);
            OrigGiftBox.GetComponent<RectTransform>().rotation = Quaternion.Euler(Vector3.zero);
        }

        /// <summary>
        /// Collect the reward after progress is full
        /// </summary>
        IEnumerator CollectYourReward()
        {
            yield return new WaitForSeconds(0.2f);
            OrigGiftBox.SetActive(false);
            GiftBoxAnim.SetActive(true);
            

            StartCoroutine(ShowReward());
        }

        /// <summary>
        /// Show and give the reward 
        /// </summary>
        /// <returns></returns>
        private IEnumerator ShowReward()
        {
            yield return new WaitForSeconds(1f);
            rewardedAmount.text = String.Format("+{0} {1}", rewards[currRewardIndex].cost, rewards[currRewardIndex].type);
            
            rewardParent.SetActive(true);           
         
            StartCoroutine(HideRewardWithDelay());
        }

        /// <summary>
        /// Hide Reward after a delay of 1 sec
        /// </summary>
        /// <returns></returns>
        private IEnumerator HideRewardWithDelay()
        {
            yield return new WaitForSeconds(2f);
            HideReward();
        }
        

        public void HideReward()
        {
            rewardParent.SetActive(false);
            //levelsDoneText.text = 0 + "/" + getRewardAfterLevel;
            //lvlcountText.color = themeColor;
            //StartCoroutine(GradualDecreaseValue());
            GradualDecreaseValue();
        }

        //ShowAds at Game Win
        public void GoToNextLevel()
        {
            // ShowInterstAd_Win();
            // StartCoroutine(GameManager.Instance.OpenAfterLevelComplete("NextLevel"));
            //UpdateCoinText();
            GameManager.Instance.ButtonClickAfterComplete("NextLevel");
           
        }

        public void GoHome()
        {
            //UpdateCoinText();
            // ShowInterstAd_Win();
            //StartCoroutine(GameManager.Instance.OpenAfterLevelComplete("main"));
            GameManager.Instance.ButtonClickAfterComplete("main");            
        }

        [Serializable]
        class RewardItems
        {
            public string type;
            public Sprite image_;
            public int cost;
        }

        //void UpdateCoinText()
        //{
        //    for (int i =0;i<startObj.transform.childCount;++i)
        //    {
        //        Destroy(startObj.transform.GetChild(i).gameObject);
        //    }
        //    StopCoroutine(GradualIncrementCoins());
        //    coinsText.text = CurrencyManager.Instance.GetAmount("coins").ToString();            
        //}

        //IEnumerator GradualIncrementCoins()
        //{
        //    yield return new WaitForSeconds(1.3f);
        //    float inc = GameManager.Instance.coinsPerCompletedPuzzle / 7f;
        //    float t = 0;

        //    while (t < 1f)
        //    {
        //        coinsText.text = (int.Parse(coinsText.text) + (int)inc).ToString();
        //        //CurrencyManager.Instance.Give("coins", 1);
        //        t += 0.2f;
        //        yield return new WaitForSeconds(0.1f);
        //    }

        //    coinsText.text = CurrencyManager.Instance.GetAmount("coins").ToString();
        //}

        //// coin animation
        //IEnumerator PlayCashAnim(GameObject parent, GameObject target)
        //{
        //    float t = 0f;
        //    while (t < 1f)
        //    {
        //        yield return new WaitForSeconds(0.05f);
        //        StartCoroutine(CollectItemAnim(parent, target));
        //        t += 0.1f;
        //        //Debug.Log("t - "+t);
        //    }
        //    t = 0f;
        //    yield return new WaitForSeconds(1f);
        //}

        //private IEnumerator CollectItemAnim(GameObject parent, GameObject target)
        //{
        //    GameObject cash = Instantiate(coinPref, parent.transform.position/*+ new Vector3(UnityEngine.Random.RandomRange(0, 1), UnityEngine.Random.Range(0,2), 0)*/, Quaternion.identity, parent.transform);
        //    //Debug.Log("cash  - " +cash.transform.position);
        //    cash.transform.DOMove(parent.transform.position + new Vector3(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f)), 0.3f);
        //    yield return new WaitForSeconds(0.3f);
        //    cash.transform.DOMove(target.transform.position, 1.2f).SetEase(Ease.InOutSine, 2f);
        //    yield return new WaitForSeconds(0.9f);
        //    target.transform.DOShakeScale(0.1f, 0.05f);
        //    Destroy(cash);
        //}

        //public override void OnHiding()
        //{
        //    base.OnHiding();
        //    //UpdateCoinText();
        //    //coinsText.GetComponent<CurrencyText>().AddEvent();           
        //}

        #endregion
    }
    
}
