    using System;
    using UnityEngine;

    [CreateAssetMenu(menuName = "Items/Passive Items/Boots")]
    public class Boots : PassiveItemSO
    {
        private Guid staminaBuffId;
        private Guid moveSpeedBuffId;

        public float staminaRegenerationAmount = 1; // e.g., 1 means +1 stamina per second
        public float moveSpeedAmount = 0.1f; // e.g., 0.1 means +10% move speed

    private void OnValidate()
        {
            description = "Passive: Provides " + staminaRegenerationAmount + " stamina restore per second and move " + (moveSpeedAmount * 100).ToString() + "% faster";
            droppable = true;
            consumable = true;
        }
        public override void ApplyEffect(Player player)
        {
            staminaBuffId = player.StaminaSystem.ApplyBonusRegen(staminaRegenerationAmount);
            moveSpeedBuffId = player.MoveSpeedSystem.AddSpeedBonus(moveSpeedAmount);
        }

        public override void RemoveEffect(Player player)
        {
            player.StaminaSystem.RemoveRegenBuff(staminaBuffId);
            player.MoveSpeedSystem.RemoveBuff(moveSpeedBuffId);
        }
    }