using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private Button playButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private Button tutorialButton;
    [SerializeField] private Button creditsButton;

    private void Awake()
    {
        playButton.onClick.AddListener(() =>
        {
            SceneLoader.Load(Scene.ChooseLevelScene);
        });

        settingsButton.onClick.AddListener(() =>
        {
            SettingsMenuUI.Instance.Show(() => { });
        });

        quitButton.onClick.AddListener(() =>
        {
            Debug.Log("Quit button clicked");
            Application.Quit();
        });

        tutorialButton.onClick.AddListener(() =>
        {
            SceneLoader.Load(Scene.TutorialScene);
        });

        creditsButton.onClick.AddListener(() =>
        {
            SceneLoader.Load(Scene.CreditsScene);
        });
    }
}
