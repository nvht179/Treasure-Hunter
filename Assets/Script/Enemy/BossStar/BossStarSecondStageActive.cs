using System.Linq;
using UnityEngine;

namespace Script.Enemy.BossStar
{
    public class BossStarSecondStageActive : BossStarBaseStage
    {
        private Transform targetedTransform;

        public BossStarSecondStageActive(BossStarContext context) : base(context)
        {
        }

        public override void EnterState()
        {
            BossStar.SetActive(true);
            if (!BossStar.SecondStageInit)
            {
                foreach (var enemySpawnPosition in BossStar.EnemySpawnPositions)
                {
                    var randomEnemyTypeIndex = Random.Range(0, BossStar.EnemyTypes.Length);
                    var enemyTransform = Object.Instantiate(BossStar.EnemyTypes[randomEnemyTypeIndex].prefab,
                        enemySpawnPosition.position, enemySpawnPosition.rotation);
                    if (enemyTransform.TryGetComponent<AbstractEnemy>(out var enemy))
                    {
                        enemy.Player = BossStar.Player;
                        BossStar.RegisterEnemy(enemy);
                    }
                }

                BossStar.InitSecondStage();
            }
            else
            {
                if (BossStar.AliveEnemies.Count < BossStar.SecondStageEnemyLimit)
                {
                    var randomSpawnPosition = Random.Range(0, BossStar.EnemySpawnPositions.Length);
                    var randomEnemyTypeIndex = Random.Range(0, BossStar.EnemyTypes.Length);
                    var enemySpawnPosition = BossStar.EnemySpawnPositions[randomSpawnPosition];
                    var enemyTransform = Object.Instantiate(BossStar.EnemyTypes[randomEnemyTypeIndex].prefab,
                        enemySpawnPosition.position, enemySpawnPosition.rotation);
                    if (enemyTransform.TryGetComponent<AbstractEnemy>(out var enemy))
                    {
                        enemy.Player = BossStar.Player;
                        BossStar.RegisterEnemy(enemy);
                    }
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

            Wander();

            var enemiesToUnregister = BossStar.AliveEnemies.Where(aliveEnemy => aliveEnemy.CurrentHealth == 0).ToList();
            foreach (var aliveEnemy in enemiesToUnregister)
            {
                BossStar.UnregisterEnemy(aliveEnemy);
            }
        }

        private void Wander()
        {
            if (targetedTransform == null)
            {
                targetedTransform = BossStar.FleePositions[Random.Range(0, BossStar.FleePositions.Length)];
            }
            else if (BossStar.transform.position != targetedTransform.position)
            {
                BossStar.transform.position = Vector3.MoveTowards(
                    BossStar.transform.position,
                    targetedTransform.position,
                    BossStar.WanderSpeed * Time.deltaTime);
            }
            else
            {
                var nextPositionsCount = BossStar.FleePositions.Count(t => t != targetedTransform);
                var randomIndex = Random.Range(0, nextPositionsCount);
                targetedTransform = BossStar.FleePositions.Where(t => t != targetedTransform).ElementAt(randomIndex);
            }
        }
    }
}