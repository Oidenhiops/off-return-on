using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Splines;

public class ListenerStartCinematic : MonoBehaviour
{
    public PlayableDirector playableDirector;
    public SplineAnimate splineAnimate;
    void Start()
    {
        GameManager.Instance.openCloseScene.OnFinishLoadChange += AwaitToStart;
    }
    void OnDestroy()
    {
        GameManager.Instance.openCloseScene.OnFinishLoadChange -= AwaitToStart;
    }
    public void AwaitToStart(bool finishLoad)
    {
        playableDirector.Play();
        splineAnimate.Play();
    }
}
