using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class UIButtonSound : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler
{
    [SerializeField] private AudioClip clickSound;
    [SerializeField] private AudioClip hoverSound;

    [Range(0f, 1f)] public float volume = 1f;

    public void OnPointerClick(PointerEventData eventData)
    {
        PlaySound(clickSound);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        //PlaySound(hoverSound);
    }

    private void PlaySound(AudioClip clip)
    {
        if (clip != null)
        {
            AudioSource.PlayClipAtPoint(clip, Vector3.zero, SoundManager.Instance.GetVolume());
        }
    }
}
