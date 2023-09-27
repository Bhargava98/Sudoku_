using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BizzyBeeGames.Sudoku
{
    public class LoginScreen : Screen
    {
        #region Variables
        Coroutine lastRoutine = null;
        private bool isBannerLoaded;
        #endregion

        #region private methods

        public override void Show(bool back, bool immediate)
        {


            base.Show(back, immediate);

         

            lastRoutine = StartCoroutine(ShowBannerWithDelay());
            GameManager.Instance.titleAdCounter++;
            GameManager.Instance.titlePlayBtnCounter++;
        }

        IEnumerator ShowBannerWithDelay()
        {
            yield return new WaitForSeconds(0.4f);
            if (!GameManager.Instance.rateUsShown)
            {
                //AdsManager.Instance.ShowBanner(out isBannerLoaded);
                if (isBannerLoaded)
                {
                    adjustForBannerAd = true;
                    AdjustScreen();
                }
                else
                {
                    adjustForBannerAd = false;
                }
            }
            else
                GameManager.Instance.rateUsShown = false;

        }
        public override void Hide(bool back, bool immediate)
        {
            base.Hide(back, immediate);
            //AdsManager.Instance.HideBanner();
        }

        public void HideBannerPlayButtonClicked()
        {
            if (lastRoutine != null)
            {
                StopCoroutine(lastRoutine);
                lastRoutine = null;
            }
        }
        #endregion
    }
}