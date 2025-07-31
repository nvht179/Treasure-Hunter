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

    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider sfxVolumeSlider;
    [SerializeField] private TextMeshProUGUI musicVolumeText;
    [SerializeField] private TextMeshProUGUI sfxVolumeText;

    [SerializeField] private Button moveLeftButton;
    [SerializeField] private Button moveRightButton;
    [SerializeField] private Button jumpButton;
    [SerializeField] private Button attackButton;
    [SerializeField] private Button pauseButton;
    [SerializeField] private Button inventoryButton;
    [SerializeField] private Button interactButton;
    [SerializeField] private TextMeshProUGUI moveLeftText;
    [SerializeField] private TextMeshProUGUI moveRightText;
    [SerializeField] private TextMeshProUGUI jumpText;
    [SerializeField] private TextMeshProUGUI attackText;
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

        musicVolumeSlider.onValueChanged.AddListener(sliderValue =>
        {
            float volume = sliderValue / 100f;
            MusicManager.Instance.SetVolume(volume);
            musicVolumeText.text = Mathf.RoundToInt(sliderValue).ToString();
        });

        sfxVolumeSlider.onValueChanged.AddListener(sliderValue =>
        {
            float volume = sliderValue / 100f;
            SoundManager.Instance.SetVolume(volume);
            sfxVolumeText.text = Mathf.RoundToInt(sliderValue).ToString();
        });

        moveLeftButton.onClick.AddListener(() => { RebindBinding(GameInput.Binding.Move_Left); });
        moveRightButton.onClick.AddListener(() => { RebindBinding(GameInput.Binding.Move_Right); });
        jumpButton.onClick.AddListener(() => { RebindBinding(GameInput.Binding.Jump); });
        attackButton.onClick.AddListener(() => { RebindBinding(GameInput.Binding.Attack); });
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

    public void SetVolume(float volume)
    {
        MusicManager.Instance.SetVolume(volume);
    }

    private void UpdateVisual()
    {
        moveLeftText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Move_Left);
        moveRightText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Move_Right);
        jumpText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Jump);
        attackText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Attack);
        pauseText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Pause);
        inventoryText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Inventory);
        interactText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Inventory);

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

    private void RebindBinding(GameInput.Binding binding)
    {
        ShowPressToRebindKey();
        if (GameInput.Instance == null)
        {
            Debug.LogError("GameInput.Instance is null. Cannot rebind binding.");
            return;
        }
        GameInput.Instance.RebindBinding(binding, () =>
        {
            HidePressToRebindKey();
            UpdateVisual();
        });
    }
}
