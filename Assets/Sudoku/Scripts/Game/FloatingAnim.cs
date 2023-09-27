using BizzyBeeGames;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FloatingAnim : MonoBehaviour
{
    [SerializeField] Image iconImage;
    [SerializeField] Text amountText;

    private CanvasGroup CG;
    private void Awake()
    {
        CG = GetComponent<CanvasGroup>();
    }

    public void playAnim(RectTransform objTransform, Sprite icon,string amount)
    {
        iconImage.sprite = icon;
        amountText.text = "-" + amount;

        FloatTweenAnim(objTransform);
    }

    void FloatTweenAnim(RectTransform trans)
    {
        UIAnimation anim = null;

        anim = UIAnimation.Alpha(CG, 1f, 0f, 0.5f);
        anim.style = UIAnimation.Style.EaseOut;
        //anim.startOnFirstFrame = true;
        anim.startDelay = 0.5f;
        anim.Play();

        anim = UIAnimation.PositionY(trans, trans.anchoredPosition.y, 120f, 1f);
        anim.style = UIAnimation.Style.Linear;
        anim.startOnFirstFrame = true;
        anim.OnAnimationFinished += (GameObject obj) => { Destroy(this.gameObject); };
        anim.Play();
    }
}
