using TMPro;
using UnityEngine;

public class ManagementLanguage : MonoBehaviour
{
    TMP_Text dialogText;
    public int id = 0;
    public GameData.TypeLanguage currentLanguage = GameData.TypeLanguage.English;
    void OnDestroy()
    {
        GameData.Instance.saveData.configurationsInfo.OnLanguageChange -= ChangeText;
    }
    void Awake()
    {
        dialogText = GetComponent<TMP_Text>();
        GameData.Instance.saveData.configurationsInfo.OnLanguageChange += ChangeText;
        ChangeText(GameData.TypeLanguage.English);
    }
    public void ChangeText(GameData.TypeLanguage language)
    {
        currentLanguage = GameData.Instance.saveData.configurationsInfo.currentLanguage;
        dialogText.text = GameData.Instance.GetDialog(id);
    }
}