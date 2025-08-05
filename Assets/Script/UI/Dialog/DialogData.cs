using System.Collections.Generic;
using UnityEngine.Events;

public class DialogButton
{
    public string Label;
    public DialogButtonType ButtonType;   // instead of raw Sprite/prefab
    public UnityAction Callback;
}

public struct DialogData
{
    public string Title;
    public string Message;
    public List<DialogButton> Buttons;
}

public enum DialogButtonType
{
    Accept,
    Decline,
    Warning,
    Danger,
    Link
}
