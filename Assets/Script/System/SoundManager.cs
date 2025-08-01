using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : PersistentManager<SoundManager>
{


    private const string PLAYER_PREFS_SOUND_EFFECTS_VOLUME = "SoundEffectsVolume";



    [SerializeField] private AudioClipRefsSO audioClipRefsSO;


    private float volume = 1f;


    protected override void Awake()
    {
        base.Awake();
        volume = PlayerPrefs.GetFloat(PLAYER_PREFS_SOUND_EFFECTS_VOLUME, 1f);
    }

    private void Start()
    {

    }

    public void AttachPlayerSound(Player player)
    {
        if (player == null) return;
        //player.OnAttack += Player_OnAttack;
        //player.OnAttackAlternate += Player_OnAttackAlternate;
        //player.OnInteract += Player_OnInteract;
        player.OnJump += Player_OnJump;
    }

    private void Player_OnJump(object sender, System.EventArgs e)
    {
        PlaySound(audioClipRefsSO.jump, ((Player)sender).transform.position);
    }

    private void PlaySound(AudioClip[] audioClipArray, Vector3 position, float volume = 1f)
    {
        PlaySound(audioClipArray[Random.Range(0, audioClipArray.Length)], position, volume);
    }

    private void PlaySound(AudioClip audioClip, Vector3 position, float volumeMultiplier = 1f)
    {
        AudioSource.PlayClipAtPoint(audioClip, position, volumeMultiplier * volume);
    }

    //    public void PlayFootstepsSound(Vector3 position, float volume)
    //    {
    //        PlaySound(audioClipRefsSO.footstep, position, volume);
    //    }

    //    public void PlayCountdownSound()
    //    {
    //        PlaySound(audioClipRefsSO.warning, Vector3.zero);
    //    }

    //    public void PlayWarningSound(Vector3 position)
    //    {
    //        PlaySound(audioClipRefsSO.warning, position);
    //    }

    public void SetVolume(float volume)
    {
        this.volume = volume;

        PlayerPrefs.SetFloat(PLAYER_PREFS_SOUND_EFFECTS_VOLUME, volume);
        PlayerPrefs.Save();
    }

    public float GetVolume()
    {
        return volume;
    }


}
