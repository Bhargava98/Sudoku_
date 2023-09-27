using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace BizzyBeeGames.Sudoku
{
    public class FreeCionsPopup : Popup
    {
        #region Inspector Variables

        [SerializeField] private Text rewardText = null;
        [SerializeField] private Text rewardMessageText = null;
        [SerializeField] private Image iconImage = null;
        [SerializeField] private Text headerText = null;
        [SerializeField] private GameObject coinsPS = null;
        [SerializeField] private GameObject hintsPS = null;
        [SerializeField] private GameObject startAnimObj = null;
        [SerializeField] private GameObject coinsIndicator = null;
        #endregion

        #region Member Variables
        private string watchAdId = "watchadpopup";
        private string typeMessageText = null;
        private string typeMessageAmount = null;
        #endregion

        #region Public Methods
        public override void OnShowing(object[] inData)
        {
            base.OnShowing(inData);
            GameManager.Instance.IsPaused = true;
            iconImage.sprite = inData[0] as Sprite;
            //string[] data = inData[1] as string[];
            coinsPS.SetActive(false);
            hintsPS.SetActive(false);
            //rewardText.text = "+ " + data[1].ToString() + ;
            StartCoroutine(SetToPause());
            switch (inData[1].ToString())
            {
                case "coinsDM":
                    typeMessageText = " coins";
                    rewardText.text = "+ " + inData[2].ToString() + " " + "Coins Added";
                    typeMessageAmount = inData[2].ToString();

                    CurrencyManager.Instance.SetAnimObjsGamePlay(startAnimObj);
                    CurrencyManager.Instance.Give("coins", (int)inData[2], 0.5f, true);

                    //coinsPS.SetActive(true);
                    //CurrencyManager.Instance.GiveCoins((int)inData[2]);
                    SoundManager.Instance.Play("reward");
                    break;
                case "coinsHints":
                    typeMessageText = " hints";
                    rewardText.text = "+ " + CurrencyManager.Instance.hintsAwardedPerBuy + " " + "Hints Added";
                    typeMessageAmount = CurrencyManager.Instance.hintsAwardedPerBuy.ToString();
                    StartCoroutine(setAnim());
                    CurrencyManager.Instance.GiveCoinsAndHints(-CurrencyManager.Instance.coinsToBuyHints);
                    PopupManager.Instance.HidePopup("store");
                    SoundManager.Instance.Play("reward");
                    break;
                case "Adcoins":
                    typeMessageText = " coins";
                    rewardText.text = /*"+ " + AdsManager.Instance.freeCoinsForAdWatch+ " " +*/ "Coins Added";
                    //typeMessageAmount = AdsManager.Instance.freeCoinsForAdWatch.ToString();

                    CurrencyManager.Instance.SetAnimObjsGamePlay(startAnimObj);
                    //CurrencyManager.Instance.Give("coins", AdsManager.Instance.freeCoinsForAdWatch, 0.5f, true);

                    //coinsPS.SetActive(true);                   
                    //CurrencyManager.Instance.GiveCoins(AdsManager.Instance.freeCoinsForAdWatch);
                    //GameAnalytics.EarnVirtualCurrency("Coins",AdsManager.Instance.freeCoinsForAdWatch);
                    break;
                case "Adhints":
                    typeMessageText = " hint";
                    hintsPS.SetActive(true);
                    rewardText.text = /*"+ " + AdsManager.Instance.freeHintsForAdWatch + " " + */"Hint Added";
                    //typeMessageAmount = AdsManager.Instance.freeHintsForAdWatch.ToString();
                    PopupManager.Instance.HidePopup("store");
                    //CurrencyManager.Instance.GiveHints(AdsManager.Instance.freeHintsForAdWatch);                    
                    //GameAnalytics.EarnVirtualCurrency("Hints", AdsManager.Instance.freeHintsForAdWatch);                    
                    break;
                case "Pcoins":
                    iconImage.sprite = PopupManager.Instance.rewardImage[0];
                    typeMessageText = " coins";
                    //coinsPS.SetActive(true);
                    CurrencyManager.Instance.SetAnimObjs(startAnimObj,coinsIndicator.transform.GetChild(0).gameObject);
                    
                    rewardText.text = "+ " + inData[0].ToString() + " " + "Coins Added";
                    typeMessageAmount = inData[0].ToString();
                    SoundManager.Instance.Play("reward");
                    //CurrencyManager.Instance.GiveHints(AdsManager.Instance.freeHintsForAdWatch);
                    // GameAnalytics.EarnVirtualCurrency("Coins", AdsManager.Instance.freeHintsForAdWatch);
                    break;
            }


            //SoundManager.Instance.Play("reward");
            rewardMessageText.text = "Congratulations!";//"You claimed \n" + typeMessageAmount+ " " + typeMessageText;

        }

        IEnumerator SetToPause()
        {
            yield return new WaitForSeconds(0.1f);
            GameManager.Instance.IsPaused = true;
        }

        IEnumerator setAnim()
        {
            yield return new WaitForSeconds(0.2f);
            hintsPS.SetActive(true);
        }
        public void PopupType(string id)
        {
            if (id == watchAdId)
            {
                headerText.text = "Reward";
            }
            else
            {
                headerText.text = "Purchase Success";
            }
        }

        public override void OnHiding()
        {
            base.OnHiding();
            if (coinsIndicator.activeInHierarchy)
                coinsIndicator.SetActive(false);

            //GameManager.Instance.IsPaused = false;
        }

        #endregion


    }
}
