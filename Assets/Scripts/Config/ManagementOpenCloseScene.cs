using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.UI;

public class ManagementOpenCloseScene : MonoBehaviour
{
    public Animator openCloseSceneAnimator;
    public bool _finishLoad;
    public Action<bool>OnFinishLoadChange;
    public bool finishLoad
    {
        get => _finishLoad;
        set
        {
            if (_finishLoad != value)
            {
                _finishLoad = value;
                OnFinishLoadChange?.Invoke(_finishLoad);
            }
        }
    }
    public float currentLoad = 0;
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
            currentLoad += 20;
            yield return new WaitForSecondsRealtime(1);
        }
    }
    public void AdjustLoading(float amount)
    {
        currentLoad += amount;
    }
    public void FinishLoad()
    {
        Time.timeScale = 1;
        _= AudioManager.Instance.FadeIn();
    }
    public async Awaitable WaitFinishCloseAnimation()
    {
        while (openCloseSceneAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f)
        {
            await Task.Delay(TimeSpan.FromSeconds(0.05));
        }
        ResetValues();
    }
    public void ResetValues()
    {
        if (auto)
        {
            try
            {
                currentLoad = 0;
                finishLoad = false;
                StartCoroutine(AutoCharge());
            }
            catch(Exception e)
            {
                print(e);
            }
        }
    }
}