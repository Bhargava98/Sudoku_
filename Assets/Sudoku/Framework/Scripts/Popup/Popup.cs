using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using BizzyBeeGames.Sudoku;
namespace BizzyBeeGames
{
	public class Popup : UIMonoBehaviour
	{
		#region Enums

		protected enum AnimType
		{
			Fade,
			Zoom,
            Slide
		}

		private enum State
		{
			Shown,
			Hidden,
			Showing,
			Hidding
		}

		#endregion

		#region Inspector Variables

		[SerializeField] protected bool				canAndroidBackClosePopup;

		[Header("Anim Settings")]
		[SerializeField] protected float			animDuration;
		[SerializeField] protected AnimType			animType;
		[SerializeField] protected AnimationCurve	animCurve;
		[SerializeField] protected RectTransform	animContainer;
         
		#endregion

		#region Member Variables

		private bool		isInitialized;
		private State		state;
		private PopupClosed	callback;

		#endregion

		#region Properties

		public bool CanAndroidBackClosePopup { get { return canAndroidBackClosePopup; } }

		#endregion

		#region Delegates

		public delegate void PopupClosed(bool cancelled, object[] outData);

		#endregion

		#region Public Methods

		public virtual void Initialize()
		{
			gameObject.SetActive(false);
			CG.alpha = 0f;
			state = State.Hidden;
		}

		public void Show()
		{
			Show(null, null);
		}

      

        public bool Show(object[] inData, PopupClosed callback)
		{
			if (state != State.Hidden)
			{
				return false;
			}
            
            //GameAnalytics.TrackData(gameObject.name);
			this.callback	= callback;
			this.state		= State.Showing;

			// Show the popup object
			gameObject.SetActive(true);
            CG.interactable = true;
			switch (animType)
			{
				case AnimType.Fade:
					DoFadeAnim();
					break;
				case AnimType.Zoom:
					DoZoomAnim();
                    break;
                case AnimType.Slide:
                    DoSlideAnimIN();
					break;
			}

            if (gameObject.name == "DailyRewardPopup")
            {              
                DailyRewardsManager.Instance.CheckRewards();
                DailyRewardsManager.Instance.UpdateUI();
            }
            if (gameObject.name == "RemoveAdsPopup")
            {
                PopupManager.Instance.CheckPurRemoveAdsPanel();
            }

            OnShowing(inData);

            if (ScreenManager.Instance.CurrentScreenId == "game")
            {
                //AdsManager.Instance.HideBanner();
            }
			return true;
		}

		public void Hide(bool cancelled)
		{
            Hide(cancelled, null);           
		}

		public void Hide(bool cancelled, object[] outData)
		{
           
            switch (state)
			{
				case State.Hidden:
				case State.Hidding:
					return;
				case State.Showing:
					UIAnimation.DestroyAllAnimations(gameObject);
					UIAnimation.DestroyAllAnimations(animContainer.gameObject);
					break;
			}

			state = State.Hidding;

			if (callback != null)
			{
				callback(cancelled, outData);
			}

			// Start the popup hide animations
			UIAnimation anim = null;
            CG.interactable = false;
            switch (animType)
            {
                case AnimType.Fade:
                case AnimType.Zoom:
                    anim = UIAnimation.Alpha(CG, 1f, 0f, animDuration);
                    anim.style = UIAnimation.Style.EaseOut;
                    anim.startOnFirstFrame = true;
                    anim.Play();
                    anim.OnAnimationFinished += (GameObject target) =>
                    {
                        state = State.Hidden;

                        GameManager.Instance.isPopupAnimationDone = true;
                        if (PopupManager.Instance.activePopups.Count > 2)
                        {
                            GameManager.Instance.isPopupAnimationDone = false;
                        }

                        gameObject.SetActive(false);
                    };

                    break;
                case AnimType.Slide:
                    DoSlideAnimOUT();
                    break;
            }

			
            
			OnHiding();
           
        }

        void SetAnimOff()
        {
            GameManager.Instance.isPopupAnimationDone = true;
            if (PopupManager.Instance.activePopups.Count > 2)
            {
                GameManager.Instance.isPopupAnimationDone = false;
            }
        }

        public void HideWithoutAnim()
        {
            switch (state)
            {
                case State.Hidden:
                case State.Hidding:
                    return;
                case State.Showing:
                    UIAnimation.DestroyAllAnimations(gameObject);
                    UIAnimation.DestroyAllAnimations(animContainer.gameObject);
                    break;
            }

            state = State.Hidding;

            
            OnHiding();
            gameObject.SetActive(false);
        }

        public void HideWithAction(string action)
		{
			Hide(false, new object[] { action });
		}

		public virtual void OnShowing(object[] inData)
		{

		}

		public virtual void OnHiding()
		{
            GameManager.Instance.stopMistakes = false;
			PopupManager.Instance.OnPopupHiding(this);
		}

		#endregion

		#region Private Methods

		private void DoFadeAnim()
		{
			// Start the popup show animations
			UIAnimation anim = null;

			anim = UIAnimation.Alpha(CG, 0f, 1f, animDuration);
			anim.startOnFirstFrame = true;
			anim.OnAnimationFinished += (GameObject obj) => { state = State.Shown; };
			anim.Play();
		}

		private void DoZoomAnim()
		{
			// Start the popup show animations
			UIAnimation anim = null;

			anim = UIAnimation.Alpha(CG, 0f, 1f, animDuration);
			anim.style = UIAnimation.Style.EaseOut;
			anim.startOnFirstFrame = true;
			anim.Play();

			anim					= UIAnimation.ScaleX(animContainer, 0f, 1f, animDuration);
			anim.style				= UIAnimation.Style.Custom;
			anim.animationCurve		= animCurve;
			anim.startOnFirstFrame	= true;
			anim.Play();

			anim					= UIAnimation.ScaleY(animContainer, 0f, 1f, animDuration);
			anim.style				= UIAnimation.Style.Custom;
			anim.animationCurve		= animCurve;
			anim.startOnFirstFrame	= true;
			anim.OnAnimationFinished += (GameObject obj) => { state = State.Shown; };
			anim.Play();

            GameManager.Instance.isPopupAnimationDone = false;
		}

        private void DoSlideAnimIN()
        {
            float screenWidth = animContainer.GetComponent<RectTransform>().rect.width;
            float fromX = -screenWidth;
            float toX = 0f;

            UIAnimation anim = UIAnimation.PositionX(animContainer.GetComponent<RectTransform>(), fromX, toX, animDuration);
            GetComponent<CanvasGroup>().alpha = 1f;
            anim.animationCurve = animCurve;
            anim.style = UIAnimation.Style.Custom;
            anim.startOnFirstFrame = true;
            anim.OnAnimationFinished += (GameObject obj) => { state = State.Shown; };
            anim.Play();
        }

        private void DoSlideAnimOUT()
        {
            float screenWidth = animContainer.GetComponent<RectTransform>().rect.width;
            float fromX = 0;
            float toX = -screenWidth;

            UIAnimation anim = UIAnimation.PositionX(animContainer.GetComponent<RectTransform>(), fromX, toX, animDuration);
            GetComponent<CanvasGroup>().alpha = 1f;
            anim.animationCurve = animCurve;
            anim.style = UIAnimation.Style.Custom;
            anim.startOnFirstFrame = true;

            anim.Play();
            
            anim.OnAnimationFinished += (GameObject target) =>
            {
                state = State.Hidden;
                gameObject.SetActive(false);
            };            

        }

       
        #endregion
    }
  
}