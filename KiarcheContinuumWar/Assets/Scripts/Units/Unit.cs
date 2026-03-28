using UnityEngine;

namespace KiarcheContinuumWar.Units
{
    /// <summary>
    /// Базовый класс юнита для RTS.
    /// Обрабатывает движение, здоровье и базовые статы.
    /// </summary>
    public class Unit : MonoBehaviour
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

        // Внутренние
        private Vector3 _targetPosition;
        private bool _isMoving = false;
        private float _lastAttackTime;
        private Unit _currentTarget;

        // События
        public System.Action<Unit> OnUnitDied;
        public System.Action<Unit, bool> OnSelectionChanged;

        private void Start()
        {
            _targetPosition = transform.position;
        }

        private void Update()
        {
            if (!IsAlive) return;

            if (_isMoving)
            {
                Move();
            }

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
            _targetPosition = position;
            _isMoving = true;
        }

        /// <summary>
        /// Установить целевой юнит для атаки.
        /// </summary>
        public void SetTarget(Unit target)
        {
            _currentTarget = target;
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

        private void Move()
        {
            float distance = Vector3.Distance(transform.position, _targetPosition);
            if (distance < 0.1f)
            {
                _isMoving = false;
                return;
            }

            Vector3 direction = (_targetPosition - transform.position).normalized;
            transform.position += direction * moveSpeed * Time.deltaTime;
            transform.rotation = Quaternion.LookRotation(direction);
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
            gameObject.SetActive(false);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, 0.5f);

            if (_isMoving)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawLine(transform.position, _targetPosition);
            }
        }
    }
}
