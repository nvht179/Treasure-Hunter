using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreditsUI : MonoBehaviour
{
    [SerializeField] private Button backButton;

    private void Awake() {
        backButton.onClick.AddListener(() => {
            SceneLoader.Load(Scene.MainMenuScene);
        });
    }
}
