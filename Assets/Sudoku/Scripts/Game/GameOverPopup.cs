using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BizzyBeeGames.Sudoku
{
    public class GameOverPopup : Popup
    {

        //[SerializeField] private Button buy2ndChance = null;
        [SerializeField] private GameObject blackScreen;

        public override void OnShowing(object[] inData)
        {
            base.OnShowing(inData);
            GameManager.Instance.buyChanceCount = 0;
            GameManager.Instance.PauseGame();
            GameManager.Instance.failCounter++; print("Fail Counter = "+ GameManager.Instance.failCounter);
            //GameAnalytics.LevelEndFail();
            SoundManager.Instance.Play("game_over");          
           // buy2ndChance.gameObject.SetActive(!GameManager.Instance.ActivePuzzleData.is2ndChanceTaken);
           // buy2ndChance.interactable = true;
        }

        public void OffButton()
        {
            //buy2ndChance.interactable = false;
        }
      
        public void Retry_Game()
        {
            GameManager.Instance.ButtonClickGameover("RetryGame");          
        }

        public void GoHome()
        {
            GameManager.Instance.ButtonClickGameover("Home");           
        }
        //public override void OnHiding()
        //{
        //    base.OnHiding();
        //    if (blackScreen.activeInHierarchy)
        //    {
        //        blackScreen.SetActive(false);
        //    }
        //}

    }
}