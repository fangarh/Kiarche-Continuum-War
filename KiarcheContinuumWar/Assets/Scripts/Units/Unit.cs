using UnityEngine;
using KiarcheContinuumWar.Pooling;

namespace KiarcheContinuumWar.Units
{
    /// <summary>
    /// Базовый класс юнита для RTS.
    /// Обрабатывает здоровье, статы и делегирует движение UnitPathfinder.
    /// </summary>
    public class Unit : MonoBehaviour, IPoolableComponent
    {
        [Header("Unit Stats")]
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float health = 100f;
        [SerializeField] private float damage = 10f;
        [SerializeField] private float attackRange = 2f;
        [SerializeField] private float attackCooldown = 1f;

        [Header("References")]
        [SerializeField] private Transform attackPoint;

        // Public свойства для доступа из редактора и других скриптов
        public float MoveSpeed { get => moveSpeed; set => moveSpeed = value; }
        public float Health { get => health; set => health = value; }
        public float Damage { get => damage; set => damage = value; }
        public float AttackRange { get => attackRange; set => attackRange = value; }
        public float AttackCooldown { get => attackCooldown; set => attackCooldown = value; }
        public Transform AttackPoint { get => attackPoint; set => attackPoint = value; }

        // Состояние
        public bool IsSelected { get; private set; }
        public bool IsAlive => health > 0;
        public float CurrentHealth => health;

        // Компоненты
        private UnitPathfinder _pathfinder;

        // Внутренние
        private float _lastAttackTime;
        private Unit _currentTarget;

        // События
        public System.Action<Unit> OnUnitDied;
        public System.Action<Unit, bool> OnSelectionChanged;

        private void Awake()
        {
            _pathfinder = GetComponent<UnitPathfinder>();
        }

        private void Update()
        {
            if (!IsAlive) return;

            if (_currentTarget != null && _currentTarget.IsAlive)
            {
                TryAttack();
            }
        }

        /// <summary>
        /// Установить целевую позицию для движения.
        /// </summary>
        public void SetTargetPosition(Vector3 position)
        {
            if (_pathfinder != null)
            {
                _pathfinder.SetTargetPosition(position);
            }
        }

        /// <summary>
        /// Установить целевой юнит для атаки.
        /// </summary>
        public void SetTarget(Unit target)
        {
            _currentTarget = target;
            
            if (_pathfinder != null)
            {
                _pathfinder.SetTarget(target);
            }
        }

        /// <summary>
        /// Выделить юнита.
        /// </summary>
        public void Select()
        {
            IsSelected = true;
            OnSelectionChanged?.Invoke(this, true);
        }

        /// <summary>
        /// Снять выделение с юнита.
        /// </summary>
        public void Deselect()
        {
            IsSelected = false;
            
            // Остановить юнита при снятии выделения
            var pathfinder = GetComponent<UnitPathfinder>();
            if (pathfinder != null)
            {
                pathfinder.Stop();
            }
            
            OnSelectionChanged?.Invoke(this, false);
        }

        /// <summary>
        /// Получить урон.
        /// </summary>
        public void TakeDamage(float damage)
        {
            health -= damage;
            if (health <= 0)
            {
                Die();
            }
        }

        private void TryAttack()
        {
            if (Time.time - _lastAttackTime < attackCooldown) return;

            float distance = Vector3.Distance(transform.position, _currentTarget.transform.position);
            if (distance <= attackRange)
            {
                Attack(_currentTarget);
                _lastAttackTime = Time.time;
            }
        }

        private void Attack(Unit target)
        {
            if (target != null && target.IsAlive)
            {
                target.TakeDamage(damage);
            }
        }

        private void Die()
        {
            OnUnitDied?.Invoke(this);
            
            // Возврат в пул вместо уничтожения
            var poolManager = FindAnyObjectByType<Managers.UnitPoolManager>();
            if (poolManager != null)
            {
                poolManager.DespawnUnit(this);
            }
            else
            {
                gameObject.SetActive(false);
            }
        }

        private void OnDrawGizmosSelected()
        {
            // Отрисовка только когда объект выбран в инспекторе
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, 0.5f);
        }

        private void OnDrawGizmos()
        {
            // Отрисовка выделения в рантайме
            if (IsSelected)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(transform.position, 0.6f);
            }
        }

        // IPoolableComponent implementation
        public void OnObjectActivate()
        {
            // Сброс состояния при активации из пула
            health = 100f; // TODO: вынести в настройки
            _currentTarget = null;
            Deselect();
        }

        public void OnObjectReturn()
        {
            // Очистка состояния перед возвратом в пул
            Deselect();
            _currentTarget = null;
        }
    }
}
