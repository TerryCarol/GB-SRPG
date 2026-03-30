using UnityEngine;
using static UnityEngine.UI.CanvasScaler;

namespace Command
{
    public class AttackCommand : ICommand
    {
        private Unit attacker;
        private Unit target;

        public AttackCommand(Unit attacker, Unit target)
        {
            this.attacker = attacker;
            this.target = target;
        }

        public void Execute()
        {
            var controller = attacker.GetComponent<UnitController>();
            controller.Attack(target);
        }
    }

    /*
    public class AttackCommand : ICommand
    {
        private Unit attacker;
        private Unit target;
        private System.Action onComplete;

        public AttackCommand(Unit attacker, Unit target, System.Action onComplete = null)
        {
            this.attacker = attacker;
            this.target = target;
            this.onComplete = onComplete;
        }

        public void Execute()
        {
            if (attacker.HasEnoughActionPoints(1))
            {
                //target.Health -= attacker.AttackPower;
                
                // 회전 (정면 바라보게)
                Vector3 dir = (target.transform.position - attacker.transform.position).normalized;
                if (dir != Vector3.zero)
                    attacker.transform.forward = dir;

                Animator animator = attacker.GetComponentInChildren<Animator>();

                if (attacker.AttackRange <= 1f)
                {
                    // 근접 공격
                    if (animator != null) animator.SetTrigger("Melee");

                    // 타이밍 맞춰 데미지 처리 (예시: 0.4초 후)
                    attacker.StartCoroutine(ApplyDelayedDamage(0.4f));
                }
                else
                {
                    // 원거리 공격
                    if (animator != null) animator.SetTrigger("Shoot");

                    attacker.StartCoroutine(ApplyDelayedDamage(0.2f));
                }

                attacker.UseActionPoint(1); // 공격 시 행동력 1 사용
            }
            else
            {
                UnityEngine.Debug.Log($"{attacker.UnitName}은 공격할 행동력이 부족함.");
                onComplete?.Invoke();
            }
        }

        // 딜레이 적용 데미지 함수
        private System.Collections.IEnumerator ApplyDelayedDamage(float delay)
        {
            yield return new UnityEngine.WaitForSeconds(delay);
            ApplyDamage();

            // 공격 완료 콜백 호출
            onComplete?.Invoke();
        }

        // 즉시 적용 데미지 함수
        private void ApplyDamage()
        {
            if (target != null && target.Health > 0)
            {
                target.Health -= attacker.AttackPower;
                

                if (target.Health <= 0)
                {
                    var controller = target.GetComponent<UnitStateController>();
                    if (controller != null)
                    {
                        controller.ChangeState(new UnitDeathState(attacker.transform));
                        UnityEngine.Debug.Log($"{attacker.UnitName} Killed {target.UnitName} with {attacker.AttackPower} damage.");
                    }
                }
                else
                {
                    UnityEngine.Debug.Log($"{attacker.UnitName} attacked {target.UnitName} with {attacker.AttackPower} damage.");
                }
            }
        }
    }
    */

}
