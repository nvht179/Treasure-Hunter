using System;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace Script.Enemy.BossStar
{
    public class BossStarThirdStageActive : BossStarBaseStage
    {
        private bool alternateSpray;
        private float activeTimer;
        private float bulletCooldown;

        public BossStarThirdStageActive(BossStarContext context) : base(context)
        {
        }

        public override void EnterState()
        {
            BossStar.SetActive(true);
            activeTimer = BossStar.ThirdStageActiveTime;
        }

        public override void UpdateState()
        {
            activeTimer -= Time.deltaTime;

            if (BossStar.CurrentHealth == 0)
            {
                BossStar.SwitchState(BossStar.Dead);
            }
            else if (activeTimer < 0)
            {
                BossStar.SwitchState(BossStar.ThirdStageRest);
            }

            var shootPattern = Random.Range(0, 3);
            var repeat = Random.Range(1, BossStarContext.MaxConsecutive + 1);
            switch (shootPattern)
            {
                case 0:
                    ShootRepeat(repeat, SprayShoot);
                    break;
                case 1:
                    ShootRepeat(repeat, BarrelShoot);
                    break;
                case 2:
                    ShootRepeat(repeat, SnipeShoot);
                    break;
            }
        }

        private void ShootRepeat(int times, Action action)
        {
            for (var i = 0; i < times; i++)
            {
                if (bulletCooldown < 0)
                {
                    action();
                    i++;
                }
                else
                {
                    bulletCooldown -= Time.deltaTime;
                }
            }
        }

        private void SprayShoot()
        {
            for (var j = alternateSpray ? 1 : 0; j < BossStar.FiringPoints.Count; j += 2)
            {
                var bulletTypeIndex = Random.Range(0, BossStar.BulletTypes.Length);
                var bulletTransform = Object.Instantiate(BossStar.BulletTypes[bulletTypeIndex].prefab,
                    BossStar.FiringPoints[j].position, BossStar.FiringPoints[j].rotation);
                if (bulletTransform.TryGetComponent<FlyingObject>(out var bullet))
                {
                    bullet.SetDirection(BossStar.SprayFiringDirections[j]);
                }
            }

            alternateSpray = !alternateSpray;
            bulletCooldown = BossStarContext.HighCooldown;
        }

        private void BarrelShoot()
        {
            var direction = BossStar.Player.transform.position - BossStar.Pivot.position;

            foreach (var t in BossStar.FiringPoints)
            {
                var bulletTypeIndex = Random.Range(0, BossStar.BulletTypes.Length);
                var bulletTransform = Object.Instantiate(BossStar.BulletTypes[bulletTypeIndex].prefab,
                    t.position, t.rotation);
                if (bulletTransform.TryGetComponent<FlyingObject>(out var bullet))
                {
                    bullet.SetDirection(direction);
                }
            }

            bulletCooldown = BossStarContext.HighCooldown;
        }

        private void SnipeShoot()
        {
            var direction = BossStar.Player.transform.position - BossStar.Pivot.position;
            var bulletTypeIndex = Random.Range(0, BossStar.BulletTypes.Length);
            var bulletType = BossStar.BulletTypes[bulletTypeIndex];
            var bulletTransform = Object.Instantiate(bulletType.prefab,
                BossStar.CentralTransform.position, BossStar.CentralTransform.rotation);
            if (bulletTransform.TryGetComponent<FlyingObject>(out var bullet))
            {
                bullet.SetDirection(direction);
                var velocity = direction * bulletType.speed;
                bullet.SetVelocity(velocity);
            }

            bulletCooldown = BossStarContext.MediumCooldown;
        }
    }
}