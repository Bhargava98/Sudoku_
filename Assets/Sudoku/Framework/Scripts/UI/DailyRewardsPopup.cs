using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BizzyBeeGames
{
    public class DailyRewardsPopup : Popup
    {
        [SerializeField] private GameObject scrollRect;

        public override void Initialize()
        {
            base.Initialize();
          // scrollRect.SetActive(false);           
        }

    }
}