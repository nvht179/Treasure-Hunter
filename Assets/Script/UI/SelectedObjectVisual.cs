using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectedObjectVisual : MonoBehaviour {
    [SerializeField] private GameObject interactiveObjectGameObject;
    [SerializeField] private GameObject[] visualGameObjectArray;
    [SerializeField] private Player player;

    private IInteractiveObject interactiveObject;

    private void Awake() {
        interactiveObject = interactiveObjectGameObject.GetComponent<IInteractiveObject>();
    }

    private void Start() {
        player.OnSelectedObjectChanged += Player_OnSelectedObjectChanged;
    }

    private void Player_OnSelectedObjectChanged(object sender, Player.OnSelectedObjectChangedEventArgs e) {
        if (e.selectedObject == interactiveObject) {
            Show();
        }
        else {
            Hide();
        }
    }

    private void Show() {
        foreach (GameObject visualGameObject in visualGameObjectArray) {
            visualGameObject.SetActive(true);
        }
    }

    private void Hide() {
        foreach (GameObject visualGameObject in visualGameObjectArray) {
            visualGameObject.SetActive(false);
        }
    }
}
