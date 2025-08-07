using System.Linq;
using UnityEngine;

namespace Script.Enemy.BossStar
{
    public class BossStarThirdStageRest : BossStarBaseStage
    {
        private float initialHealth;
        private Vector3 fleeTarget;
        private bool isFleeing;
        private float restTimer;
        private Transform targetedTransform;

        public BossStarThirdStageRest(BossStarContext context) : base(context)
        {
        }

        public override void EnterState()
        {
            BossStar.SetActive(false);
            initialHealth = BossStar.CurrentHealth;
            restTimer = BossStar.ThirdStageRestTime;
        }

        public override void UpdateState()
        {
            restTimer -= Time.deltaTime;

            if (BossStar.CurrentHealth == 0)
            {
                BossStar.SwitchState(BossStar.Dead);
                return;
            }

            if (restTimer < 0)
            {
                BossStar.SwitchState(BossStar.ThirdStageActive);
                return;
            }

            Wander();

            // if (ShouldFlee())
            // {
            //     HandleFlee();
            // }
        }

        private bool ShouldFlee()
        {
            var healthLost = initialHealth - BossStar.CurrentHealth;
            var fleeThreshold = BossStar.MaxHealth * BossStarContext.FleePercentageMaxHealthThreshold;
            return healthLost > fleeThreshold;
        }

        private void HandleFlee()
        {
            if (!isFleeing)
            {
                StartFleeing();
            }
            else
            {
                ContinueFleeing();
            }
        }

        private void StartFleeing()
        {
            fleeTarget = FindFarthestFleePosition();
            isFleeing = true;
        }

        private void ContinueFleeing()
        {
            if (BossStar.transform.position == fleeTarget)
            {
                isFleeing = false;
                initialHealth = BossStar.CurrentHealth;
            }
            else
            {
                BossStar.transform.position = Vector3.MoveTowards(
                    BossStar.transform.position,
                    fleeTarget,
                    BossStar.FleeSpeed * Time.deltaTime);
            }
        }

        private Vector3 FindFarthestFleePosition()
        {
            var maxDistance = 0f;
            var maxDistanceIndex = 0;

            for (var i = 0; i < BossStar.FleePositions.Length; i++)
            {
                var distance = Vector3.Distance(
                    BossStar.FleePositions[i].transform.position,
                    BossStar.transform.position);

                if (distance > maxDistance)
                {
                    maxDistance = distance;
                    maxDistanceIndex = i;
                }
            }

            return BossStar.FleePositions[maxDistanceIndex].position;
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