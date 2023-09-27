using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BizzyBeeGames.Sudoku
{
    public class ThemesPopup : Popup
    {

        #region Private Variables
        private const string THEME_SCEME = "Theme_Sceme";
        private const string CURRENT_COLOR_SELECTED = "Current_Color_Selected";

        private int isDarkTheme;//0 = true || 1 = false
        private int currentColor;//Stores selected color 

        [SerializeField] private GameObject DarkTick=null, lightTick =null;
        [SerializeField] private GameObject[] colorButtonsTicks = new GameObject[8];
        #endregion

        #region Unity members
        private void Awake()
        {
           // isDarkTheme = PlayerPrefs.GetInt(THEME_SCEME, 0);
           // currentColor = PlayerPrefs.GetInt(CURRENT_COLOR_SELECTED,0);
        }
        #endregion

        #region Public Methods
        public override void OnShowing(object[] inData)
        {
            base.OnShowing(inData);
            GameManager.Instance.IsPaused = true;
            isDarkTheme = PlayerPrefs.GetInt(THEME_SCEME, 0);
            currentColor = PlayerPrefs.GetInt(CURRENT_COLOR_SELECTED,0);
            UpdateThemeSchemeUI();
            UpdateButtonsUI();
        }

        public override void OnHiding()
        {
            base.OnHiding();
            if (ScreenManager.Instance.CurrentScreenId == "game")
                GameManager.Instance.IsPaused = false;
        }

        public void SetThemeSceme(int isDark)
        {
            isDarkTheme = isDark;
            PlayerPrefs.SetInt(THEME_SCEME,isDarkTheme);
            UpdateThemeSchemeUI();
            SetActiveTheme(currentColor);
        }
       

        public void SetActiveTheme(int id)
        {
            if (isDarkTheme == 0)
            {
                ThemeManager.Instance.SetActiveTheme(id + 8);
            }
            else
            {
                ThemeManager.Instance.SetActiveTheme(id);
            }
            currentColor = id;
            PlayerPrefs.SetInt(CURRENT_COLOR_SELECTED,currentColor);
            UpdateButtonsUI();
        }
        #endregion


        #region Private Variables
        private void UpdateThemeSchemeUI()
        {
            DarkTick.SetActive(Convert.ToBoolean(isDarkTheme));
            lightTick.SetActive(!Convert.ToBoolean(isDarkTheme));
        }

        private void UpdateButtonsUI()
        {
            for (int i = 0; i < 8; ++i)
            {
                 colorButtonsTicks[i].SetActive(i == currentColor);               
            }
        }
        #endregion

    }
}