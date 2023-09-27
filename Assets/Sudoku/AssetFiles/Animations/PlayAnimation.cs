using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayAnimation : MonoBehaviour
{
    private Animator AM;

    private void Start()
    {
        AM = GetComponent<Animator>();
    }

    public void PlayAnim()
    {
        AM.Play("AnimState",0,0);
       
    }
    
}
