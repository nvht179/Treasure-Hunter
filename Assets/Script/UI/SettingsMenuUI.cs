using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenuUI : PersistentManager<SettingsMenuUI>
{
    public enum Category
    {
        Audio,
        Control
    }

    [Serializable]
    public struct CategoryUI
    {
        public Category category;
        public Button button;
        public GameObject contentPanel;
    }

    [SerializeField] private List<CategoryUI> categoryUIs;

    private Dictionary<Category, Button> categoryButtons;
    private Dictionary<Category, GameObject> categoryPanels;
    private Category activeCategory;

    [SerializeField] private Button backButton;
    [SerializeField] private Button resetButton;
    [SerializeField] private Button deleteAllDataButton;

    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider sfxVolumeSlider;
    [SerializeField] private TextMeshProUGUI musicVolumeText;
    [SerializeField] private TextMeshProUGUI sfxVolumeText;

    [SerializeField] private Button moveLeftButton;
    [SerializeField] private Button moveRightButton;
    [SerializeField] private Button jumpButton;
    [SerializeField] private Button attackButton;
    [SerializeField] private Button attackAlternateButton;
    [SerializeField] private Button pauseButton;
    [SerializeField] private Button inventoryButton;
    [SerializeField] private Button interactButton;
    [SerializeField] private TextMeshProUGUI moveLeftText;
    [SerializeField] private TextMeshProUGUI moveRightText;
    [SerializeField] private TextMeshProUGUI jumpText;
    [SerializeField] private TextMeshProUGUI attackText;
    [SerializeField] private TextMeshProUGUI attackAlternateText;
    [SerializeField] private TextMeshProUGUI pauseText;
    [SerializeField] private TextMeshProUGUI inventoryText;
    [SerializeField] private TextMeshProUGUI interactText;

    [SerializeField] private Transform pressToRebindKeyTransform;

    private Action onBackButtonAction;

    protected override void Awake()
    {
        base.Awake();
        categoryButtons = new Dictionary<Category, Button>();
        categoryPanels = new Dictionary<Category, GameObject>();

        foreach (var ui in categoryUIs)
        {
            categoryButtons[ui.category] = ui.button;
            categoryPanels[ui.category] = ui.contentPanel;

            ui.button.onClick.AddListener(() => OnCategorySelected(ui.category));
        }

        backButton.onClick.AddListener(() =>
        {
            Hide();
            onBackButtonAction();
        });

        resetButton.onClick.AddListener(() =>
        {
            DialogManager.Instance.ShowDialog(new DialogData {
                Title = "Reset all preferences!",
                Message = "Are you sure? This action cannot be undone!",
                Buttons = new List<DialogButton> { 
                    new DialogButton {
                        Label = "Yes",
                        ButtonType = DialogButtonType.Accept,
                        Callback = () => {
                            // Reset the preferences
                            DataManager.Instance.ResetUserPreferences();
                            
                            // Update the UI to reflect the reset values
                            UpdateVisual();
                            
                            // Delay the success dialog to avoid nesting issues
                            StartCoroutine(ShowResetSuccessDialog());
                        }
                    },
                    new DialogButton {
                        Label = "No",
                        ButtonType = DialogButtonType.Decline,
                        Callback = () => {
                            Debug.Log("Reset cancelled by user.");
                        }
                    }
                },
            });
        });

        deleteAllDataButton.onClick.AddListener(() =>
        {
            DialogManager.Instance.ShowDialog(new DialogData {
                Title = "Delete all game progress!",
                Message = "Are you sure? This action cannot be undone!",
                Buttons = new List<DialogButton> { 
                    new DialogButton {
                        Label = "Yes",
                        ButtonType = DialogButtonType.Accept,
                        Callback = () => {
                            // Delete all user data
                            DataManager.Instance.DeleteSaveData();

                            StartCoroutine(ShowDeleteSuccessDialog());
                        }
                    },
                    new DialogButton {
                        Label = "No",
                        ButtonType = DialogButtonType.Decline,
                        Callback = () => {
                            Debug.Log("Delete all data cancelled by user.");
                        }
                    }
                },
            });
        });

        musicVolumeSlider.onValueChanged.AddListener(sliderValue =>
        {
            float volume = sliderValue / 100f;
            DataManager.Instance.UpdateMusicVolume(volume);
            musicVolumeText.text = Mathf.RoundToInt(sliderValue).ToString();
        });

        sfxVolumeSlider.onValueChanged.AddListener(sliderValue =>
        {
            float volume = sliderValue / 100f;
            DataManager.Instance.UpdateSfxVolume(volume);
            sfxVolumeText.text = Mathf.RoundToInt(sliderValue).ToString();
        });

        moveLeftButton.onClick.AddListener(() => { RebindBinding(GameInput.Binding.Move_Left); });
        moveRightButton.onClick.AddListener(() => { RebindBinding(GameInput.Binding.Move_Right); });
        jumpButton.onClick.AddListener(() => { RebindBinding(GameInput.Binding.Jump); });
        attackButton.onClick.AddListener(() => { RebindBinding(GameInput.Binding.Attack); });
        attackAlternateButton.onClick.AddListener(() => { RebindBinding(GameInput.Binding.AttackAlternate); });
        pauseButton.onClick.AddListener(() => { RebindBinding(GameInput.Binding.Pause); });
        inventoryButton.onClick.AddListener(() => { RebindBinding(GameInput.Binding.Inventory); }); 
        interactButton.onClick.AddListener(() => { RebindBinding(GameInput.Binding.Interact); });
    }

    private void Start()
    {
        UpdateVisual();
        HidePressToRebindKey();
        Hide();
    }

    private void OnCategorySelected(Category selected)
    {
        foreach (var kvp in categoryPanels)
        {
            kvp.Value.SetActive(kvp.Key == selected);

            // Make other category buttons blur
            if (kvp.Key == selected)
            {
                categoryButtons[kvp.Key].GetComponent<Image>().color = Color.white; // Active color
            }
            else
            {
                categoryButtons[kvp.Key].GetComponent<Image>().color = new Color(0.5f, 0.5f, 0.5f); // Inactive color
            }
        }

        activeCategory = selected;
    }

    private void UpdateVisual()
    {
        moveLeftText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Move_Left);
        moveRightText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Move_Right);
        jumpText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Jump);
        attackText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Attack);
        attackAlternateText.text = GameInput.Instance.GetBindingText(GameInput.Binding.AttackAlternate);
        pauseText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Pause);
        inventoryText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Inventory);
        interactText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Interact);

        float savedVolume = MusicManager.Instance.GetVolume() * 100f;
        musicVolumeSlider.value = savedVolume;
        musicVolumeText.text = savedVolume.ToString();

        float sfxVolume = SoundManager.Instance.GetVolume() * 100f;
        sfxVolumeSlider.value = sfxVolume;
        sfxVolumeText.text = sfxVolume.ToString();
    }

    public void Show(Action onBackButtonAction)
    {
        this.onBackButtonAction = onBackButtonAction;

        gameObject.SetActive(true);

        if (categoryPanels.Count > 0)
        {
            OnCategorySelected(activeCategory);
        }
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }

    private void ShowPressToRebindKey()
    {
        pressToRebindKeyTransform.gameObject.SetActive(true);
    }

    private void HidePressToRebindKey()
    {
        pressToRebindKeyTransform.gameObject.SetActive(false);
    }

    private IEnumerator ShowResetSuccessDialog()
    {
        // Wait one frame to ensure the previous dialog is fully closed
        yield return null;
        
        DialogManager.Instance.ShowDialog(new DialogData
        {
            Title = "Reset successfully!",
            Message = "All preferences have been reset to default values.",
            Buttons = new List<DialogButton> {
                new DialogButton {
                    Label = "OK",
                    ButtonType = DialogButtonType.Accept,
                    Callback = () => { Debug.Log("Reset completed successfully."); }
                }
            }
        });
    }

    private IEnumerator ShowDeleteSuccessDialog()
    {
        // Wait one frame to ensure the previous dialog is fully closed
        yield return null;

        DialogManager.Instance.ShowDialog(new DialogData
        {
            Title = "Delete successfully!",
            Message = "All game progress data have been deleted.",
            Buttons = new List<DialogButton> {
                new DialogButton {
                    Label = "OK",
                    ButtonType = DialogButtonType.Accept,
                    Callback = () => { Debug.Log("Delete completed successfully."); }
                }
            }
        });
    }

    private void RebindBinding(GameInput.Binding binding)
    {
        ShowPressToRebindKey();
        if (GameInput.Instance == null)
        {
            Debug.LogError("GameInput.Instance is null. Cannot rebind binding.");
            return;
        }

        GameInput.Instance.RebindBinding(binding, success =>
        {
            HidePressToRebindKey();
            UpdateVisual();

            // Build a single-OK button
            var okButton = new DialogButton
            {
                Label = "",
                ButtonType = DialogButtonType.Accept,
                Callback = () => { Debug.Log("Dialog button works."); }
            };

            if (success)
            {
                DialogManager.Instance.ShowDialog(new DialogData
                {
                    Title = "Rebind Succeeded!",
                    Message = "Your new key has been saved.",
                    Buttons = new List<DialogButton> { okButton },
                });
            }
            else
            {
                DialogManager.Instance.ShowDialog(new DialogData
                {
                    Title = "Rebind Failed!",
                    Message = "That key is already in use. Please try again.",
                    Buttons = new List<DialogButton> { okButton },
                });
            }
        });
    }

}
