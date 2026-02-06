using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdeaScript : MonoBehaviour
{
    [Header("IdeaScript is used to mark an object as an Idea")]
    public IdeaType type;
    public bool hasObtained;
    public string ideaId;
    /** Returns type as string with spaces added between captial letters. I.E. */
    public override string ToString()
    {
        string name = "" + type;
        string formatted = "";
        foreach (char letter in name)
        {
            if (char.IsUpper(letter))
            {
                formatted += " " + letter;
            }
            else
            {
                formatted += letter;
            }
        }
        formatted = formatted.Trim();//remove extra space
        return formatted;
    }
    //public Image photoCapture;
    public readonly static Sprite icon; // the icon for this type of Idea.
}
