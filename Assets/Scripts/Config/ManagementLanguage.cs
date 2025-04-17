using TMPro;
using UnityEngine;

public class ManagementLanguage : MonoBehaviour
{
    public TMP_Text dialogText;
    public int id = 0;
    public GameData.TypeLanguage currentLanguage = GameData.TypeLanguage.English;
    void Reset()
    {
        dialogText = GetComponent<TMP_Text>();
    }
    void OnDestroy()
    {
        GameData.Instance.saveData.configurationsInfo.OnLanguageChange -= ChangeText;
    }
    void Awake()
    {
        GameData.Instance.saveData.configurationsInfo.OnLanguageChange += ChangeText;
        ChangeText(GameData.TypeLanguage.English);
    }
    public void ChangeText(GameData.TypeLanguage language)
    {
        currentLanguage = GameData.Instance.saveData.configurationsInfo.currentLanguage;
        dialogText.text = GameData.Instance.GetDialog(id);
    }
}