using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ManagementOpenCloseScene : MonoBehaviour
{
    public Animator openCloseSceneAnimator;
    [NonSerialized] public bool finishLoad = false;
    float currentLoad = 0;
    public bool auto = false;
    void Start()
    {
        ResetValues();
    }
    public void Update()
    {
        if (!finishLoad)
        {
            float value = currentLoad / 100 > 0 ? currentLoad / 100 : 1;
            if (currentLoad == 100)
            {
                finishLoad = true;
                GameManager.Instance.EnterScene();
                FinishLoad();
            }
        }
    }
    public IEnumerator AutoCharge()
    {
        while (true)
        {
            if (currentLoad >= 100)
            {
                break;
            }
            currentLoad += 50;
            yield return new WaitForSeconds(0.5f);
        }
    }
    public void AdjustLoading(float amount)
    {
        currentLoad += amount;
    }
    public void FinishLoad()
    {
        GameManager.Instance.startGame = true;
    }
    public void ResetValues()
    {
        if (auto)
        {
            currentLoad = 0;
            finishLoad = false;
            StartCoroutine(AutoCharge());
        }
    }
}