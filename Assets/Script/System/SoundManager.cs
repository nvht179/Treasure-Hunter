using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class SoundManager : PersistentManager<SoundManager>
{
    [SerializeField] private AudioClipRefsSO audioClipRefsSO;
    private Player player; // default position to play sound effects

    private float volume = 1f;

    protected override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        volume = DataManager.Instance.UserPreferences.sfxVolume;

        GameManager.Instance.OnStateChanged += GameManager_OnStateChanged;
        DataManager.Instance.OnUserPreferencesChanged += DataManager_OnUserPreferencesChanged;

        GameManager_OnStateChanged(GameManager.State.None, GameManager.Instance.CurrentState);
    }

    private void DataManager_OnUserPreferencesChanged(UserPreferencesData data)
    {
        SetVolume(data.sfxVolume);
    }

    private void GameManager_OnStateChanged(GameManager.State oldState, GameManager.State newState)
    {
        if (newState == GameManager.State.LevelWon)
        {
            PlaySound(audioClipRefsSO.won, player.transform.position);
            Debug.Log("SoundManager: Game won sound played.");
        }
        else if (newState == GameManager.State.LevelLost)
        {
            PlaySound(audioClipRefsSO.lost, player.transform.position);
            Debug.Log("SoundManager: Game lost sound played.");
        }
    }

    public void AttachPlayerSound(Player player)
    {
        if (player == null) return;
        this.player = player;
        player.OnMoveHorizontal += Player_OnMoveHorizontal;
        player.OnJump += Player_OnJump;
        player.OnJumpLand += Player_OnJumpLand; // not yet implemented in Player.cs: when to invoke?
        player.OnAttack += Player_OnAttack;
        player.OnAirAttack += Player_OnAirAttack;
        player.OnAttackAlternate += Player_OnAttackAlternate;
        player.OnGreenPotionSuccess += Player_OnGreenPotionSuccess;
        player.OnGreenPotionFail += Player_OnGreenPotionFail;
        player.OnBluePotionUsed += Player_OnBluePotionUsed;
        player.OnHealthPotionUsed += Player_OnHealthPotionUsed;
        player.OnResourcesCollected += Player_OnResourcesCollected;
        player.HealthSystem.OnDamageReceived += HealthSystemOnDamageReceived;
        player.HealthSystem.OnDeath += Player_OnPlayerDead;

        InventoryUI inventoryUI = player.GetInventoryUI();
        inventoryUI.OnInventoryOpen += Player_OnInventoryOpen;
        inventoryUI.OnInventoryClose += Player_OnInventoryClose;
        inventoryUI.OnItemDrop += Player_OnItemDrop;
    }
    public void AttachDoorSound(Door door)
    {
        door.OnDoorInteracted += Door_OnDoorInteracted;
    }

    private void Door_OnDoorInteracted(object sender, System.EventArgs e)
    {
        Debug.Log("Door_OnDoorInteracted");
        PlaySound(audioClipRefsSO.doorOpenClose, ((Door)sender).transform.position);
    }

    private void HealthSystemOnDamageReceived(object sender, EventArgs e)
    {
        Debug.Log("PlayerOnDamageTaken");
        // TODO: sender is HealthSystem, but it should be Player
        PlaySound(audioClipRefsSO.playerGotHit, player.transform.position);
    }

    private void Player_OnPlayerDead(object sender, EventArgs e)
    {
        Debug.Log("Player_OnPlayerDead");
        PlaySound(audioClipRefsSO.playerDead, player.transform.position);
    }

    private void Player_OnResourcesCollected(object sender, System.EventArgs e)
    {
        Debug.Log("Player_OnKeyCollected");
        PlaySound(audioClipRefsSO.keyCollected, ((Player)sender).transform.position);
    }

    public void AttachShopSound(Shop shop, ShopUI shopUI)
    {
        if (shop == null) return;
        shop.OnShopOpen += Player_OnShopOpen;
        shop.OnShopClose += Player_OnShopClose;
        shopUI.OnItemBuy += Player_OnItemBuy;
    }

    private void Player_OnHealthPotionUsed(object sender, System.EventArgs e)
    {
        Debug.Log("Player_OnHealthPotionUsed");
        PlaySound(audioClipRefsSO.redPotionUsed, ((Player)sender).transform.position);
    }

    private void Player_OnGreenPotionFail(object sender, System.EventArgs e)
    {
        Debug.Log("Player_OnGreenPotionFail");
        PlaySound(audioClipRefsSO.greenPotionFail, ((Player)sender).transform.position);
    }

    private void Player_OnGreenPotionSuccess(object sender, System.EventArgs e)
    {
        Debug.Log("Player_OnGreenPotionSuccess");
        PlaySound(audioClipRefsSO.greenPotionSuccess, ((Player)sender).transform.position);
    }

    private void Player_OnBluePotionUsed(object sender, System.EventArgs e)
    {
        Debug.Log("Player_OnBluePotionUsed");
        PlaySound(audioClipRefsSO.bluePotionUsed, ((Player)sender).transform.position);
    }

    private void Player_OnItemBuy(object sender, System.EventArgs e)
    {
        Debug.Log("Player_OnItemBuy");
        PlaySound(audioClipRefsSO.itemBuy, ((ShopUI)sender).transform.position);
    }

    private void Player_OnShopClose(object sender, System.EventArgs e)
    {
        Debug.Log("Player_OnShopClose");
        PlaySound(audioClipRefsSO.shopClose, ((Shop)sender).transform.position);
    }

    private void Player_OnShopOpen(object sender, System.EventArgs e)
    {
        Debug.Log("Player_OnShopOpen");
        PlaySound(audioClipRefsSO.shopOpen, ((Shop)sender).transform.position);
    }

    private void Player_OnMoveHorizontal(object sender, System.EventArgs e)
    {
        Debug.Log("Player_OnMoveHorizontal");
        PlaySound(audioClipRefsSO.move, ((Player)sender).transform.position, 0.5f);
    }

    private void Player_OnItemDrop(object sender, System.EventArgs e)
    {
        Debug.Log("Player_OnItemDrop");
        PlaySound(audioClipRefsSO.itemDrop, ((InventoryUI)sender).transform.position);
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

    private void Player_OnAttackAlternate(object sender, System.EventArgs e)
    {
        Debug.Log("Player_OnAttackAlternate");
        PlaySound(audioClipRefsSO.throwKnife, ((Player)sender).transform.position);
    }

    private void Player_OnAttack(object sender, System.EventArgs e)
    {
        Debug.Log("Player_OnAttack");
        PlaySound(audioClipRefsSO.attack, ((Player)sender).transform.position, 0.8f);
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
            Debug.Log("No audio clips available to play at position: " + position);
            return;
        }
        PlaySound(audioClipArray[Random.Range(0, audioClipArray.Length)], position, volume);
    }

    private void PlaySound(AudioClip audioClip, Vector3 position, float volumeMultiplier = 1f)
    {
        if (audioClip == null)
        {
            Debug.Log("No audio clip available");
            return;
        }
        AudioSource.PlayClipAtPoint(audioClip, position, volumeMultiplier * volume);
    }

    public void SetVolume(float newVolume)
    {
        volume = newVolume;
        Debug.Log($"Sound volume changed to {newVolume}");
    }

    public float GetVolume()
    {
        return volume;
    }
}
