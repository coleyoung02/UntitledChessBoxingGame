using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using System;

public class RoundCountdown : Timer
{
    [SerializeField] protected AudioSource source;
    [SerializeField] protected AudioClip ticking;
    [SerializeField] protected AudioClip bell;
    private bool setTicking;
    private void Start()
    {
        setTicking = false;
    }

    public override void FixedUpdate()
    {
        if (timeValue <= 10 && !setTicking)
        {
            setTicking = true;
            source.clip = ticking;
            source.Play();
        }

        base.FixedUpdate();
    }

    protected IEnumerator bellWait(Action f)
    {
        source.clip = bell;
        source.Play();
        yield return new WaitForSeconds(2f);
        f();
    }
}