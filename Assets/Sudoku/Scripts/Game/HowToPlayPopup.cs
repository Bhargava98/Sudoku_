using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace BizzyBeeGames.Sudoku
{
    public class HowToPlayPopup : Popup
    {

        [SerializeField] private RectTransform content;
        [SerializeField] private ScrollSnapRect sRect;
        private bool isBannerLoaded;
        public override void OnShowing(object[] inData)
        {
            base.OnShowing(inData);
            StartCoroutine(pauseGame());
            content.anchoredPosition = new Vector3(872f,0f,0f);
            sRect.ResetDots();
        }

        public override void OnHiding()
        {
            base.OnHiding();
            if (ScreenManager.Instance.CurrentScreenId == "game" && PlayerPrefs.GetInt("HOW_TO_PLAY", 0) == 0)
            {
                //AdsManager.Instance.ShowBanner(out isBannerLoaded);
                PlayerPrefs.SetInt("HOW_TO_PLAY", 1);
            }
        }

        IEnumerator pauseGame()
        {
            yield return new WaitForSeconds(0.2f);
            GameManager.Instance.PauseGame();
        }

        
    }
}