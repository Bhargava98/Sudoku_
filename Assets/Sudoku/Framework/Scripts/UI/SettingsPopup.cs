using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BizzyBeeGames.Sudoku
{
    public class SettingsPopup : Popup
    {
        #region Inspector Variables

        [Space]

        [SerializeField] private ToggleSlider musicToggle = null;
        [SerializeField] private ToggleSlider soundToggle = null;
        [SerializeField] private ToggleSlider vibrateToggle = null;
        [SerializeField] private ToggleSlider mistakeToggle = null;
        [SerializeField] private GameObject blackFade = null;
        [SerializeField] private Color startColor ;
 
        //[SerializeField] private List<RectTransform> items = null;
        #endregion


        #region Unity Methods


       
        protected void Start()
        {
         
            musicToggle.OnValueChanged += OnMusicValueChanged;
            soundToggle.OnValueChanged += OnSoundEffectsValueChanged;
            vibrateToggle.OnValueChanged += OnVibrateValueChanged;
            if(mistakeToggle!=null)
            mistakeToggle.OnValueChanged += OnMistakesToggleChanged;

            musicToggle.SetToggle(SoundManager.Instance.IsMusicOn, false);
            soundToggle.SetToggle(SoundManager.Instance.IsSoundEffectsOn, false);
            vibrateToggle.SetToggle(SoundManager.Instance.IsVibrationOn, false);
            if (mistakeToggle != null)
            mistakeToggle.SetToggle(GameManager.Instance.ShowIncorrectNumbers, false);
        }

        public override void OnShowing(object[] inData)
        {
            base.OnShowing(inData);
            if(blackFade!=null)
            blackFade.GetComponent<Image>().color = startColor;

            GameManager.Instance.homeAdCounter++;
            musicToggle.SetToggle(SoundManager.Instance.IsMusicOn, false);
            soundToggle.SetToggle(SoundManager.Instance.IsSoundEffectsOn, false);
            vibrateToggle.SetToggle(SoundManager.Instance.IsVibrationOn, false);
            if (mistakeToggle != null)
                mistakeToggle.SetToggle(GameManager.Instance.ShowIncorrectNumbers, false);

            //  StartCoroutine(StartAnimate());
        }
        #endregion

        #region Private Methods


   

        private void OnMusicValueChanged(bool isOn)
		{
			SoundManager.Instance.SetSoundTypeOnOff(SoundManager.SoundType.Music, isOn);
		}

		private void OnSoundEffectsValueChanged(bool isOn)
		{
			SoundManager.Instance.SetSoundTypeOnOff(SoundManager.SoundType.SoundEffect, isOn);
		}

        private void OnVibrateValueChanged(bool isOn)
        {
            SoundManager.Instance.SetValueOnOff(SoundManager.ButtonType.vibration,isOn);
        }

        private void OnMistakesToggleChanged(bool isOn)
        {
           GameManager.Instance.stopMistakes = isOn;
           GameManager.Instance.SetGameSetting("mistakes", isOn);
        }


        public void GoHomeClicked()
        {                       
            // ShowInterstAd_Win();
            StartCoroutine(OpenNextMenu());
        }

        void ShowInterstAd_GoHome()
        {            
            if (UnityRemoteData.isAds_AtHomeEnabled && (GameManager.Instance.homeAdCounter >= UnityRemoteData.adsHomeClickCounter))
            {
                //AdsManager.Instance.INT_LevelWin.ShowInterstitial();
            }            
        }

        
        IEnumerator OpenNextMenu()
        {
            PopupManager.Instance.isInterst = true;
            Hide(true);

            blackFade.GetComponent<Image>().color = Color.black;

            ScreenManager.Instance.BackTo("main");
            
             yield return new WaitForSeconds(0.1f);
             PopupManager.Instance.isInterst = false;
             ShowInterstAd_GoHome();

            //if (GameManager.Instance.rateCounter >= GameManager.Instance.totalRateCounter)
            //{
            //    if (!GameManager.interstitial_Controller)
            //    {
            //        PopupManager.Instance.Show("rate_us");
            //        GameManager.Instance.rateCounter = 0;
            //    }
            //}

           
            
            //    ScreenManager.Instance.Show("main");

        }

        public override void OnHiding()
        {
            base.OnHiding();
           // GameManager.Instance.IsPaused = false;
        }
        //IEnumerator StartAnimate()
        //{
        //    UIAnimation tween;
        //    if (items.Count != 0)
        //    {
        //        foreach (RectTransform obj in items)
        //        {
        //            tween = UIAnimation.PositionX(obj, -1000f, obj.anchoredPosition.x, 0.4f);
        //            tween.style = UIAnimation.Style.EaseIn;
        //            tween.startOnFirstFrame = true;
        //            tween.Play();
        //            yield return new WaitForEndOfFrame();
        //        }
        //    }

        //}

        #endregion


    }
}
