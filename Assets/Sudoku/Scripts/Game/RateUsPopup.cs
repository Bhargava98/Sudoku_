using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BizzyBeeGames.Sudoku
{
    public class RateUsPopup : Popup
    {
        [SerializeField] private GameObject spineEffect;

        public override void OnShowing(object[] inData)
        {
            base.OnShowing(inData);
            spineEffect.SetActive(true);
        }
        
        public override void OnHiding()
        {
            base.OnHiding();
            spineEffect.SetActive(false);
        }
    }
}