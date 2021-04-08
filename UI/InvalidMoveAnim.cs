using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InvalidMoveAnim : MonoBehaviour
{
    [Header("Componets")]
    [SerializeField] private GameObject wTextGameObject, bTextGameObject;
    [Header("Customizable")]
    [SerializeField] int max;
    [SerializeField] int min;
    [SerializeField] private AnimationCurve animationCurve;
    [SerializeField] float breatheTime;
    [SerializeField] float totalBreatheTime;
    private Text wText, bText;

    private bool animating;
    public bool Animating => animating;
    private void Awake()
    {
        wText = wTextGameObject.GetComponent<Text>();
        bText = bTextGameObject.GetComponent<Text>();
    }
    public void StartAnim(int side)
    {
        animating.SetTrue();
        if (side == -1)
            wTextGameObject.SetActive(true);
        else
            bTextGameObject.SetActive(true);
        StartCoroutine(BreathTimeOut(side));
        StartBreath(side);

    }
    public void StopAnim(int side)
    {
        StopAllCoroutines();
        if (side == -1)
            wTextGameObject.SetActive(false);
        else
            bTextGameObject.SetActive(false);
        animating.SetFalse();
    }
    private void StartBreath(int side) => Breathe(side, max, min);
    private IEnumerator BreathTimeOut(int side)
    {
        float time = 0;
        while(time < totalBreatheTime)
        {
            time += Time.deltaTime;
            yield return null;
        }
        StopAnim(side);
    }
    private void Breathe(int side, int start, int end) => StartCoroutine(BreatheEnum(side, start, end));
    private IEnumerator BreatheEnum(int side, int start, int end)
    {
        float time = 0;
        int fontSize;
        while (time < breatheTime)
        {
            time += Time.deltaTime;
            if (time > breatheTime)
                time = breatheTime;
            fontSize = (int)Mathf.Lerp(start, end, animationCurve.Evaluate(time / breatheTime));
            if (side < 0)
                wText.fontSize = fontSize;
            else
                bText.fontSize = fontSize;
            yield return null;
        }
        Breathe(side, end, start);
    }
}
