using UnityEngine;

public class WinGame : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    bool isWin = false;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isWin)
        {
            isWin = true;
            Time.timeScale = 0;
            GameManager.Instance.ChangeSceneSelector(GameManager.TypeScene.WinScene);
        }
    }
}
