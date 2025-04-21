using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Playables;
using UnityEngine.Splines;

public class ListenerStartCinematic : MonoBehaviour
{
    public PlayableDirector playableDirector;
    public SplineAnimate splineAnimate;
    public PlayerInputs playerInputs;
    public bool skipScene = false;
    void Start()
    {
        GameManager.Instance.openCloseScene.OnFinishLoadChange += AwaitToStart;
        playerInputs.playerControls.Player.Interact.started += SkipScene;
    }
    void OnDestroy()
    {
        GameManager.Instance.openCloseScene.OnFinishLoadChange -= AwaitToStart;
        playerInputs.playerControls.Player.Interact.started -= SkipScene;
    }
    public void AwaitToStart(bool finishLoad)
    {
        playableDirector.Play();
        splineAnimate.Play();
    }
    public void SkipScene(InputAction.CallbackContext context)
    {
        if (!skipScene)
        {
            skipScene = true;
            GameManager.Instance.ChangeSceneSelector(GameManager.TypeScene.NextLevel);
        }
    }
}
