using UnityEngine;

public class GameManagerHelper : MonoBehaviour
{
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
}