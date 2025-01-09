using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdeaScript : MonoBehaviour
{
    [Header("IdeaScript is used to mark an object as an Idea")]
    public IdeaType type;
    //public Image photoCapture;
    public readonly static Sprite icon; // the icon for this type of Idea.
}

public enum IdeaType
{
    Rock,
    Brick,
    Screw,
    Window,
    Potato
}
