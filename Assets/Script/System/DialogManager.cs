using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogManager : PersistentManager<DialogManager>
{
    [SerializeField] private DialogButtonTypeMapping typeMappingSO;  // assign in Inspector
    [SerializeField] private GameObject dialogCanvasPrefab;
    [SerializeField] private GameObject buttonPrefab;

    private Transform buttonContainer;
    private TextMeshProUGUI titleText, bodyText;
    private Button closeButton;

    private GameObject currentDialog;

    public void ShowDialog(DialogData data)
    {
        if (currentDialog != null) Destroy(currentDialog);

        typeMappingSO.Initialize();
        currentDialog = Instantiate(dialogCanvasPrefab, transform);
        // Cache references inside prefab:
        var panel = currentDialog.transform;
        titleText = panel.Find("DialogPanel/Content/TitleText").GetComponent<TextMeshProUGUI>();
        bodyText = panel.Find("DialogPanel/Content/BodyText").GetComponent<TextMeshProUGUI>();
        buttonContainer = panel.Find("DialogPanel/ButtonContainer");
        closeButton = panel.Find("DialogPanel/CloseButton").GetComponent<Button>();

        titleText.text = data.Title;
        bodyText.text = data.Message;

        closeButton.onClick.AddListener(() => {
            CloseDialog();
        });

        foreach (var btnData in data.Buttons)
        {
            // Lookup the prefab for this type:
            var entry = typeMappingSO.GetEntry(btnData.ButtonType);
            var btnObj = Instantiate(entry.buttonPrefab, buttonContainer);

            // Set text/icon if needed:
            var tmp = btnObj.GetComponentInChildren<TextMeshProUGUI>();
            if (tmp != null) tmp.text = btnData.Label;

            var icon = btnObj.transform.Find("Image").GetComponent<Image>();
            if (icon != null && entry.iconSprite != null)
                icon.sprite = entry.iconSprite;

            // Wire callback:
            var button = btnObj.GetComponent<Button>();
            button.onClick.AddListener(() => {
                btnData.Callback?.Invoke();
                CloseDialog();
            });
        }

        currentDialog.SetActive(true);
        Debug.Log($"Dialog shown: {data.Title}");
    }

    public void CloseDialog()
    {
        if (currentDialog != null)
        {
            Destroy(currentDialog);
            currentDialog = null;
        }
    }
}
