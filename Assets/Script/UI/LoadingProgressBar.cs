using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingProgressBar : MonoBehaviour
{
    private Image image;
    private bool isFirstUpdate = true;

    private void Awake()
    {
        image = GetComponent<Image>();
    }

    private void Update()
    {
        if (isFirstUpdate)
        {
            isFirstUpdate = false;
            image.fillAmount = 0f; // Initialize the fill amount to 0
            Debug.Log($"Loading progress: {image.fillAmount * 100f}%");
            return;
        }
        float rawProgress = SceneLoader.GetLoadingProgress();
        image.fillAmount = Mathf.Clamp01(rawProgress / 0.9f);
        Debug.Log($"Loading progress: {image.fillAmount * 100f}%");
    }
}
