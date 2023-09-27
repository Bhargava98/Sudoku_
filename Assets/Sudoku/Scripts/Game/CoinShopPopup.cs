using BizzyBeeGames;
using BizzyBeeGames.Sudoku;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CoinShopPopup : Popup
{

    #region Inspector Vars
    [SerializeField] private Text[] buyButtonTexts = new Text[5];
    [SerializeField] private Text[] coinAmountTexts = new Text[5];
    #endregion
    [SerializeField] private GameObject RestoreButton;

    #region Public Methods
    public override void OnShowing(object[] inData)
    {
        base.OnShowing(inData);
        CurrencyManager.Instance.isPurchasedIAP = false;
        //if(PurchaseManager.Instance.iapValues!=null)
        //for (int i = 0; i < PurchaseManager.Instance.iapValues.Length; ++i)
        //{
        //    buyButtonTexts[i].text = "$" + PurchaseManager.Instance.iapValues[i].price.ToString();
        //    coinAmountTexts[i].text ="x"+PurchaseManager.Instance.iapValues[i].amount.ToString();
        //}

        //if(PurchaseManager.localPrices!=null)
        //for (int i = 0; i < PurchaseManager.localPrices.Length; i++)
        //{
        //    if(PurchaseManager.localPrices[i] != "$0.01" && PurchaseManager.localPrices[i]!=null)
        //    buyButtonTexts[i].text = PurchaseManager.localPrices[i];
        //}
        CheckNoAdsPurchased();

    }

    public void CheckNoAdsPurchased()
    {
        if (PlayerPrefs.GetInt("SHOW_ADS", 0) == 1)
        {
            //RestoreButton.SetActive(true);
            buyButtonTexts[5].text = "Purchased";
            coinAmountTexts[5].transform.parent.GetComponent<Button>().interactable = false;
        }
        else
        {
            //RestoreButton.SetActive(false);
            //if (PurchaseManager.localPrices != null)
            //    buyButtonTexts[5].text = PurchaseManager.localPrices[5];
           
            coinAmountTexts[5].transform.parent.GetComponent<Button>().interactable = true;
        }
    }

    public override void OnHiding()
    {
        base.OnHiding();

        if (Application.internetReachability != NetworkReachability.NotReachable && UnityRemoteData.isFullAdsEnabled)
        {
            //if(AdsManager.Instance.isVideoAdAvailable())
            if (!CurrencyManager.Instance.isPurchasedIAP)
            {
                PopupManager.Instance.Show("FreeCoinsPanel");
                GameManager.Instance.IsPaused = true;
            }
        }
    }
    #endregion



}
