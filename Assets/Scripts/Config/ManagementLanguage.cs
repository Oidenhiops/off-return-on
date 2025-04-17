using TMPro;
using UnityEngine;

public class ManagementLanguage : MonoBehaviour
{
    public TMP_Text dialogText;
    public int id = 0;
    public ManagementData.TypeLanguage currentLanguage = ManagementData.TypeLanguage.English;
    void Reset()
    {
        dialogText = GetComponent<TMP_Text>();
    }
    void OnDestroy()
    {
        ManagementData.Instance.saveData.configurationsInfo.OnLanguageChange -= ChangeText;
    }
    void Awake()
    {
        ManagementData.Instance.saveData.configurationsInfo.OnLanguageChange += ChangeText;
        ChangeText(ManagementData.TypeLanguage.English);
    }
    public void ChangeText(ManagementData.TypeLanguage language)
    {
        currentLanguage = ManagementData.Instance.saveData.configurationsInfo.currentLanguage;
        dialogText.text = ManagementData.Instance.GetDialog(id);
    }
}