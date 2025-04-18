using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManagerHelper : MonoBehaviour
{
    [SerializeField] Animator _unloadAnimator;
    public void ChangeScene(int typeScene)
    {
        GameManager.TypeScene scene = (GameManager.TypeScene)typeScene;
        GameManager.Instance.ChangeSceneSelector(scene);
    }
    public void PlayASound(AudioClip audioClip){
        GameManager.Instance.PlayASound(audioClip);
    }
    public void PlayASound(AudioClip audioClip, float initialRandomPitch)
    {
        GameManager.Instance.PlayASound(audioClip, initialRandomPitch);
    }
    public void PlayASoundButton(AudioClip audioClip)
    {
        GameManager.Instance.PlayASound(audioClip, 1);
    }
    public void SetAudioMixerData()
    {
        GameManager.Instance.SetAudioMixerData();
    }
    public void UnloadScene()
    {
        string sceneForUnload = ValidateScene();
        StartCoroutine(UnloadSceneOptions(sceneForUnload));
    }
    public string ValidateScene()
    {
        int sceneCount = SceneManager.sceneCount;
        List<string> scenes = new List<string>();
        for (int i = 0; i < sceneCount; i++)
        {
            scenes.Add(SceneManager.GetSceneAt(i).name);
        }
        if (scenes.Contains("CreditsScene")) return "CreditsScene";
        return "OptionsScene";
    }
    public IEnumerator UnloadSceneOptions(string sceneForUnload)
    {
        _unloadAnimator.SetBool("exit", true);
        yield return new WaitForSecondsRealtime(0.5f);
        if (sceneForUnload == "OptionsScene") Time.timeScale = 1;
        SceneManager.UnloadSceneAsync(sceneForUnload);
        GameManager.Instance.isPause = false;
    }
}