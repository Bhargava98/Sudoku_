using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BizzyBeeGames.Sudoku
{
    public class HintShopPopup : Popup
    {
        [SerializeField]private Text amountText;
        [SerializeField]private Text coinText;
        
        public override void OnShowing(object[] inData)
        {
            base.OnShowing(inData);

            amountText.text = "Get " + CurrencyManager.Instance.hintsAwardedPerBuy +" Hints";
            coinText.text = CurrencyManager.Instance.coinsToBuyHints.ToString();
        }

    }
}