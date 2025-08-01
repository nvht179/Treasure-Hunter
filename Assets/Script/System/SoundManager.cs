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

    public void AttachPlayerSound(Player player)
    {
        if (player == null) return;
        player.OnMoveHorizontal += Player_OnMoveHorizontal;
        player.OnJump += Player_OnJump;
        player.OnJumpLand += Player_OnJumpLand; // not yet implemented in Player.cs: when to invoke?
        player.OnAttack += Player_OnAttack;
        player.OnAirAttack += Player_OnAirAttack;
        player.OnAttackAlternate += Player_OnAttackAlternate;
        player.OnInteract += Player_OnInteract;

        InventoryUI inventoryUI = player.GetInventoryUI();
        inventoryUI.OnInventoryOpen += Player_OnInventoryOpen;
        inventoryUI.OnInventoryClose += Player_OnInventoryClose;
        inventoryUI.OnItemUse += Player_OnItemUse;
        inventoryUI.OnItemDrop += Player_OnItemDrop;
    }

    private void Player_OnMoveHorizontal(object sender, System.EventArgs e)
    {
        Debug.Log("Player_OnMoveHorizontal");
        PlaySound(audioClipRefsSO.move, ((Player)sender).transform.position);
    }

    private void Player_OnItemDrop(object sender, System.EventArgs e)
    {
        Debug.Log("Player_OnItemDrop");
        PlaySound(audioClipRefsSO.itemDrop, ((InventoryUI)sender).transform.position);
    }

    private void Player_OnItemUse(object sender, System.EventArgs e)
    {
        Debug.Log("Player_OnItemUse");
        PlaySound(audioClipRefsSO.itemUse, ((InventoryUI)sender).transform.position);
    }

    private void Player_OnJumpLand(object sender, System.EventArgs e)
    {
        Debug.Log("Player_OnJumpLand");
        PlaySound(audioClipRefsSO.jumpLand, ((Player)sender).transform.position);
    }

    private void Player_OnInventoryClose(object sender, System.EventArgs e)
    {
        Debug.Log("Player_OnInventoryClose");
        PlaySound(audioClipRefsSO.inventoryClose, ((InventoryUI)sender).transform.position);
    }

    private void Player_OnInventoryOpen(object sender, System.EventArgs e)
    {
        Debug.Log("Player_OnInventoryOpen");
        PlaySound(audioClipRefsSO.inventoryOpen, ((InventoryUI)sender).transform.position);
    }

    private void Player_OnInteract(object sender, System.EventArgs e)
    {
        Debug.Log("Player_OnInteract");
        PlaySound(audioClipRefsSO.interact, ((Player)sender).transform.position);
    }

    private void Player_OnAttackAlternate(object sender, System.EventArgs e)
    {
        Debug.Log("Player_OnAttackAlternate");
        PlaySound(audioClipRefsSO.throwKnife, ((Player)sender).transform.position);
    }

    private void Player_OnAttack(object sender, System.EventArgs e)
    {
        Debug.Log("Player_OnAttack");
        PlaySound(audioClipRefsSO.attack, ((Player)sender).transform.position);
    }

    private void Player_OnAirAttack(object sender, System.EventArgs e)
    {
        Debug.Log("Player_OnAirAttack");
        PlaySound(audioClipRefsSO.airAttack, ((Player)sender).transform.position);
    }

    private void Player_OnJump(object sender, System.EventArgs e)
    {
        Debug.Log("Player_OnJump");
        PlaySound(audioClipRefsSO.jump, ((Player)sender).transform.position);
    }

    private void PlaySound(AudioClip[] audioClipArray, Vector3 position, float volume = 1f)
    {
        if (audioClipArray == null || audioClipArray.Length == 0)
        {
            Debug.LogWarning("No audio clips available to play at position: " + position);
            return;
        }
        PlaySound(audioClipArray[Random.Range(0, audioClipArray.Length)], position, volume);
    }

    private void PlaySound(AudioClip audioClip, Vector3 position, float volumeMultiplier = 1f)
    {
        AudioSource.PlayClipAtPoint(audioClip, position, volumeMultiplier * volume);
    }

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
