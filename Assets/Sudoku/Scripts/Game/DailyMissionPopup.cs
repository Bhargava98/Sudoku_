using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Globalization;
using System;

namespace BizzyBeeGames.Sudoku {

    public class DailyMissionPopup : Popup
    {
        [SerializeField] private Text dayText;
        [SerializeField] private Text monthText;

        public override void OnShowing(object[] inData)
        {
            base.OnShowing(inData);

            dayText.text = DateTime.Now.Day.ToString();
            monthText.text = CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(DateTime.Now.Month).ToString();
        }

    }
}