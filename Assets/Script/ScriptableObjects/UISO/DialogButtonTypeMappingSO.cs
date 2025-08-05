using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "UI/ButtonTypeMapping")]
public class DialogButtonTypeMapping : ScriptableObject
{
    [System.Serializable]
    public struct Entry
    {
        public DialogButtonType type;
        public GameObject buttonPrefab;    // prefab styled for this type
        public Sprite iconSprite;         // optional icon for icon buttons
    }

    public Entry[] entries;

    // Helper lookup
    private Dictionary<DialogButtonType, Entry> dict;
    public void Initialize()
    {
        dict = new Dictionary<DialogButtonType, Entry>();
        foreach (var e in entries) dict[e.type] = e;
    }

    public Entry GetEntry(DialogButtonType type)
    {
        if (dict == null) Initialize();
        return dict[type];
    }
}
