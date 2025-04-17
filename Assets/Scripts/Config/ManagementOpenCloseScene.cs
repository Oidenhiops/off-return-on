using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ManagementOpenCloseScene : MonoBehaviour
{
    public Animator openCloseSceneAnimator;
    [SerializeField] Image openCloseSceneLoader;
    [NonSerialized] public bool finishLoad = false;
    float currentLoad = 0;
    public bool auto = false;
    void Start()
    {
        if (auto) StartCoroutine(AutoCharge());
    }
    public void Update()
    {
        if (!finishLoad)
        {
            float value = currentLoad / 100 > 0 ? currentLoad / 100 : 1;
            openCloseSceneLoader.fillAmount = Mathf.Lerp(openCloseSceneLoader.fillAmount, currentLoad / 100, value * Time.deltaTime);
            if (openCloseSceneLoader.fillAmount >= 0.99)
            {
                openCloseSceneLoader.fillAmount = 1;
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
            yield return new WaitForSeconds(2);
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
}