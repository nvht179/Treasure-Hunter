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
        GameManager.Instance.OnStateChanged += GameManager_OnStateChanged;
    }

    private void GameManager_OnStateChanged(GameManager.State oldState, GameManager.State newState)
    {
        if (newState == GameManager.State.LevelWon)
        {
            PlaySound(audioClipRefsSO.won, Vector3.zero);
            Debug.Log("SoundManager: Game won sound played.");
        }
        else if (newState == GameManager.State.LevelLost)
        {
            PlaySound(audioClipRefsSO.lost, Vector3.zero);
            Debug.Log("SoundManager: Game lost sound played.");
        }
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
        player.OnItemDrop += Player_OnItemDrop;
        player.OnGreenPotionSuccess += Player_OnGreenPotionSuccess;
        player.OnGreenPotionFail += Player_OnGreenPotionFail;
        player.OnBluePotionUsed += Player_OnBluePotionUsed;
        player.OnRedPotionUsed += Player_OnRedPotionUsed;
        player.OnKeyCollected += Player_OnKeyCollected;

        InventoryUI inventoryUI = player.GetInventoryUI();
        inventoryUI.OnInventoryOpen += Player_OnInventoryOpen;
        inventoryUI.OnInventoryClose += Player_OnInventoryClose;
        inventoryUI.OnItemUse += Player_OnItemUse;
        inventoryUI.OnItemDrop += Player_OnItemDrop;
    }

    private void Player_OnKeyCollected(object sender, System.EventArgs e)
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

    private void Player_OnRedPotionUsed(object sender, System.EventArgs e)
    {
        Debug.Log("Player_OnRedPotionUsed");
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
