using System.Collections.Generic;
using UnityEngine;

namespace KiarcheContinuumWar.Pooling
{
    /// <summary>
    /// Универсальный пул объектов.
    /// Позволяет переиспользовать объекты вместо создания/уничтожения.
    /// </summary>
    /// <typeparam name="T">Тип пулингового объекта (MonoBehaviour)</typeparam>
    public class ObjectPool<T> where T : MonoBehaviour
    {
        private readonly Stack<T> _available = new Stack<T>();
        private readonly List<T> _active = new List<T>();
        private readonly T _prefab;
        private readonly Transform _parent;
        private readonly int _initialCapacity;
        private readonly bool _autoExpand;

        public int TotalCount => _available.Count + _active.Count;
        public int AvailableCount => _available.Count;
        public int ActiveCount => _active.Count;

        public ObjectPool(T prefab, int initialCapacity = 100, Transform parent = null, bool autoExpand = true)
        {
            _prefab = prefab;
            _initialCapacity = initialCapacity;
            _parent = parent ?? new GameObject($"Pool<{typeof(T).Name}>").transform;
            _autoExpand = autoExpand;

            // Предварительное заполнение пула
            Preload(initialCapacity);
        }

        /// <summary>
        /// Предварительно заполнить пул объектами.
        /// </summary>
        public void Preload(int count)
        {
            for (int i = 0; i < count; i++)
            {
                T obj = CreateNewObject();
                obj.gameObject.SetActive(false);
                _available.Push(obj);
            }
        }

        /// <summary>
        /// Взять объект из пула.
        /// </summary>
        public T Get(Vector3 position, Quaternion rotation)
        {
            T obj;

            if (_available.Count > 0)
            {
                obj = _available.Pop();
            }
            else if (_autoExpand)
            {
                obj = CreateNewObject();
            }
            else
            {
                Debug.LogWarning($"[ObjectPool<{typeof(T).Name}>] Пул пуст! Рассмотрите увеличение capacity.");
                return null;
            }

            obj.transform.position = position;
            obj.transform.rotation = rotation;
            obj.gameObject.SetActive(true);
            _active.Add(obj);

            // Вызвать событие активации
            if (obj.TryGetComponent(out IPoolableComponent poolable))
            {
                poolable.OnObjectActivate();
            }

            return obj;
        }

        /// <summary>
        /// Вернуть объект в пул.
        /// </summary>
        public void Return(T obj)
        {
            if (!_active.Contains(obj))
            {
                Debug.LogWarning($"[ObjectPool<{typeof(T).Name}>] Попытка вернуть объект, который не в активном списке!");
                return;
            }

            // Вызвать событие деактивации
            if (obj.TryGetComponent(out IPoolableComponent poolable))
            {
                poolable.OnObjectReturn();
            }

            obj.gameObject.SetActive(false);
            _active.Remove(obj);
            _available.Push(obj);
        }

        /// <summary>
        /// Вернуть все активные объекты в пул.
        /// </summary>
        public void ReturnAll()
        {
            // Копируем список, так как Return modifies _active
            var activeCopy = new List<T>(_active);
            foreach (var obj in activeCopy)
            {
                Return(obj);
            }
        }

        /// <summary>
        /// Очистить пул (уничтожить все объекты).
        /// </summary>
        public void Clear()
        {
            foreach (var obj in _available)
            {
                if (obj != null)
                {
                    Object.Destroy(obj.gameObject);
                }
            }
            _available.Clear();
            _active.Clear();
        }

        private T CreateNewObject()
        {
            T obj = Object.Instantiate(_prefab, _parent);
            obj.gameObject.name = _prefab.gameObject.name;
            return obj;
        }
    }

    /// <summary>
    /// Интерфейс для компонентов, реагирующих на активацию/возврат объекта.
    /// </summary>
    public interface IPoolableComponent
    {
        void OnObjectActivate();
        void OnObjectReturn();
    }
}
