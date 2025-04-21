using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class DoorInteract : ObjectBase
{
    public TMP_Text display;
    public string key = "Lorem Ipsum";
    public Animator doorAnimator;
    public override void Interact(ManagementCharacter managementCharacter)
    {
        switch(gameObject.name)
        {
            case "Space":
                display.text += " ";
                break;
            case "Delete":
                if (gameObject.name.Length > 0)
                {
                    List<char> currentKey = display.text.ToCharArray().ToList();
                    currentKey.RemoveAt(currentKey.Count - 1);
                    string newKey = "";
                    foreach (char letter in currentKey)
                    {
                        newKey += letter;
                    }
                    display.text = newKey;
                }
                break;
            case "Enter":
                doorAnimator.SetBool("isActive", string.Equals(display.text, key, System.StringComparison.OrdinalIgnoreCase));
                break;
            default:
                if (display.text.Length < key.Length)
                {
                    display.text += gameObject.name;
                }
                break;
        }
    }
}
