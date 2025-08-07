using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Script.Enemy.BossStar
{
    public class BossStarFirstStageActive : BossStarBaseStage
    {
        private float firstStageActiveTimer;
        private float bulletCooldownTimer;
        private bool alternate;
        private int bulletTypeIndex;

        public BossStarFirstStageActive(BossStarContext context) : base(context)
        {
        }

        public override void EnterState()
        {
            BossStar.SetActive(true);
            BossStar.transform.position = BossStar.CentralTransform.position;
            firstStageActiveTimer = BossStar.FirstStageActiveTime;
            bulletCooldownTimer = BossStarContext.LowCooldown;
        }

        public override void UpdateState()
        {
            firstStageActiveTimer -= Time.deltaTime;
            bulletCooldownTimer -= Time.deltaTime;

            if (BossStar.CurrentHealth < BossStar.MaxHealth * BossStarContext.SecondStageMaxHealthRatio)
            {
                BossStar.SwitchState(BossStar.SecondStageActive);
            }
            else if (firstStageActiveTimer < 0)
            {
                BossStar.SwitchState(BossStar.FirstStageRest);
            }

            if (bulletCooldownTimer < 0)
            {
                Shoot();
                bulletCooldownTimer = BossStarContext.LowCooldown;
            }
        }

        private void Shoot()
        {
            var bulletTypeCounts = BossStar.BulletTypes.Length;
            for (var i = alternate ? 1 : 0; i < BossStar.FiringPoints.Count; i += 2)
            {
                var bulletTransform = Object.Instantiate(BossStar.BulletTypes[bulletTypeIndex].prefab,
                    BossStar.FiringPoints[i].position, BossStar.FiringPoints[i].rotation);
                if (bulletTransform.TryGetComponent<FlyingObject>(out var bullet))
                {
                    bullet.SetDirection(BossStar.SprayFiringDirections[i]);
                }
            }

            alternate = !alternate;
            bulletTypeIndex = (bulletTypeIndex + 1) % bulletTypeCounts;
        }
    }
}