using UnityEngine;

namespace Script.Enemy.BossStar
{
    public class BossStarSecondStageActive : BossStarBaseStage
    {
        private bool init;

        public BossStarSecondStageActive(BossStarContext context) : base(context)
        {
        }

        public override void EnterState()
        {
            if (!init)
            {
                foreach (var enemySpawnPosition in BossStar.EnemySpawnPositions)
                {
                    var randomEnemyTypeIndex = Random.Range(0, BossStar.EnemyTypes.Length);
                    var enemyTransform = Object.Instantiate(BossStar.EnemyTypes[randomEnemyTypeIndex].prefab,
                        enemySpawnPosition.position, enemySpawnPosition.rotation);
                    if (enemyTransform.TryGetComponent<AbstractEnemy>(out var enemy))
                    {
                        enemy.Player = BossStar.Player;
                    }
                }

                init = false;
            }
            else
            {
                var randomSpawnPosition = Random.Range(0, BossStar.EnemySpawnPositions.Length);
                var randomEnemyTypeIndex = Random.Range(0, BossStar.EnemyTypes.Length);
                var enemySpawnPosition = BossStar.EnemySpawnPositions[randomSpawnPosition];
                var enemyTransform = Object.Instantiate(BossStar.EnemyTypes[randomEnemyTypeIndex].prefab,
                    enemySpawnPosition.position, enemySpawnPosition.rotation);
                if (enemyTransform.TryGetComponent<AbstractEnemy>(out var enemy))
                {
                    enemy.Player = BossStar.Player;
                }
            }
        }

        public override void UpdateState()
        {
            if (BossStar.CurrentHealth < BossStar.MaxHealth * BossStarContext.ThirdStageMaxHealthRatio)
            {
                BossStar.SwitchState(BossStar.ThirdStageActive);
            }
            else
            {
                BossStar.SwitchState(BossStar.SecondStageRest);
            }
        }
    }
}