using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Command
{
    public class DeathCommand : ICommand
    {
        private Unit unit;

        public DeathCommand(Unit unit)
        {
            this.unit = unit;
        }

        public void Execute()
        {
            var controller = unit.GetComponent<UnitController>();
            if (controller != null)
            {
                controller.Die();
            }

            Debug.Log($"{unit.UnitName} has died.");
        }
    }

    // 구버전
    /*
    public class DeathCommand : ICommand
    {
        private Unit unit;
        private Transform attacker;

        public DeathCommand(Unit unit, Transform attacker = null)
        {
            this.unit = unit;
            this.attacker = attacker;
        }

        public void Execute()
        {
            if (unit == null || unit.Health > 0) return;

            float ragdollChance = 0.9f;
            Animator animator = unit.GetComponentInChildren<Animator>();

            if (UnityEngine.Random.value < ragdollChance)
            {
                Vector3 force = CalculateForce();
                TriggerRagdoll(force);
            }
            else
            {
                if (animator != null)
                {
                    animator.applyRootMotion = true;
                    animator.SetTrigger("Die");
                    float tempDelay = UnityEngine.Random.Range(1.5f, 4.383f);
                    unit.StartCoroutine(ActivateRagdollAfterDelay(tempDelay)); // 애니메이션 길이만큼 대기
                }
            }

            DeathInfoUpdate();

            var controller = unit.GetComponent<UnitStateController>();
            if (controller != null) controller.IsBusy = true;

            Debug.Log($"{unit.UnitName} has died.");
        }

        private IEnumerator ActivateRagdollAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            TriggerRagdoll(Vector3.zero); // 힘 없이 넘어지게
        }

        private void TriggerRagdoll(Vector3 force)
        {
            Animator animator = unit.GetComponentInChildren<Animator>();
            if (animator != null) animator.enabled = false;

            if (unit.RagdollRoot != null)
            {
                unit.RagdollRoot.SetActive(true);
                foreach (var rb in unit.RagdollRoot.GetComponentsInChildren<Rigidbody>())
                {
                    rb.isKinematic = false;
                    rb.AddForce(force);
                }
            }
        }

        private Vector3 CalculateForce(float minPower = 50f, float maxPower = 400f)    // 랜덤 충격 에너지 생성기 (무기 시스템 도입 시 탄종 또는 무기에 따라 키네틱 에너지 받는 것으로 대체)
        {
            Vector3 direction;
            if (attacker != null)
            {
                direction = (unit.transform.position - attacker.position).normalized;
            }
            else
            {
                direction = Vector3.zero;   // 공격자가 없는 상황 (환경요인 또는 지속 데미지 등으로 사망)
            }

            float power = UnityEngine.Random.Range(minPower, maxPower);
            Vector3 upward = Vector3.up * UnityEngine.Random.Range(0.5f, 1.2f);
            Vector3 randomized = direction + upward + Random.insideUnitSphere * 0.3f;

            return randomized.normalized * power;
        }

        private void DeathInfoUpdate()
        {
            unit.CurrentState = UnitState.Dying;

            if (unit.currentTile != null)
            {
                unit.currentTile.isOccupied = false;
            }
            if (!unit.IsCorpse)
            {
                unit.IsCorpse = true;
            }
        }
    }
    */
}

