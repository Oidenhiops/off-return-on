using UnityEngine;

public class MobileHud : MonoBehaviour
{
    public GameObject[] huds;
    void Start()
    {
        GameManager.Instance.OnDeviceChanged += OnMobileHud;
        OnMobileHud(GameManager.Instance.currentDevice);
    }
    void OnDestroy()
    {
        GameManager.Instance.OnDeviceChanged -= OnMobileHud;
    }
    public void OnMobileHud(GameManager.TypeDevice device){
        if (device == GameManager.TypeDevice.MOBILE)
        {
            foreach(GameObject hud in huds)
            {
                hud.SetActive(true);
            }
        }
        else
        {
            foreach (GameObject hud in huds)
            {
                hud.SetActive(false);
            }
        }
    }
}
