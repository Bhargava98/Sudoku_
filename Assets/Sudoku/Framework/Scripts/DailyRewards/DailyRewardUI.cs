using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BizzyBeeGames.Sudoku
{
    public class DailyRewardUI : MonoBehaviour
    {
        public Image rewardIcon;
        public Image ringImage;
        public Text rewardAmount;
        [Space(20)]
        public Sprite boxCap, boxBottom;

        public GameObject questionObj;

        public void Refresh(DailyReward reward)
        {
            //print("Update UI's");
            rewardIcon.sprite = reward.icon;
            //rewardAmount.text = "+" + reward.amount.ToString();

        }

        void updateAvailabilityUI()
        {
            //if (DailyRewardsManager.Instance.availableReward==0)
            //{
            //    ringImage.color = DailyRewardsManager.Instance.greyColor;
            //}
            //else
            //{
            //    ringImage.color = DailyRewardsManager.Instance.availableColor;
            //}
        }
        

    }
}