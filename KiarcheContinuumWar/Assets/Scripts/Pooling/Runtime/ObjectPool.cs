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
        private readonly bool _autoExpand;

        public int TotalCount => _available.Count + _active.Count;
        public int AvailableCount => _available.Count;
        public int ActiveCount => _active.Count;

        public ObjectPool(T prefab, int initialCapacity = 100, Transform parent = null, bool autoExpand = true)
        {
            _prefab = prefab;
            _parent = parent ?? new GameObject($"Pool<{typeof(T).Name}>").transform;
            _autoExpand = autoExpand;

            Preload(initialCapacity);
        }

        public void Preload(int count)
        {
            for (int i = 0; i < count; i++)
            {
                T obj = CreateNewObject();
                obj.gameObject.SetActive(false);
                _available.Push(obj);
            }
        }

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

            if (obj.TryGetComponent(out IPoolableComponent poolable))
            {
                poolable.OnObjectActivate();
            }

            return obj;
        }

        public void Return(T obj)
        {
            if (!_active.Contains(obj))
            {
                Debug.LogWarning($"[ObjectPool<{typeof(T).Name}>] Попытка вернуть объект, который не в активном списке!");
                return;
            }

            if (obj.TryGetComponent(out IPoolableComponent poolable))
            {
                poolable.OnObjectReturn();
            }

            obj.gameObject.SetActive(false);
            _active.Remove(obj);
            _available.Push(obj);
        }

        public void ReturnAll()
        {
            var activeCopy = new List<T>(_active);
            foreach (T obj in activeCopy)
            {
                Return(obj);
            }
        }

        public void Clear()
        {
            foreach (T obj in _available)
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

    public interface IPoolableComponent
    {
        void OnObjectActivate();
        void OnObjectReturn();
    }
}
