using KiarcheContinuumWar.Pooling;
using NUnit.Framework;
using UnityEngine;

namespace KiarcheContinuumWar.Tests.EditMode
{
    public class ObjectPoolEditModeTests
    {
        [Test]
        public void Constructor_PreloadsExpectedAmount()
        {
            var prefab = CreatePrefab();
            var pool = new ObjectPool<TestPoolBehaviour>(prefab, initialCapacity: 3);

            Assert.That(pool.TotalCount, Is.EqualTo(3));
            Assert.That(pool.AvailableCount, Is.EqualTo(3));
            Assert.That(pool.ActiveCount, Is.EqualTo(0));

            Object.DestroyImmediate(prefab.gameObject);
        }

        [Test]
        public void GetAndReturn_UpdatesCountsAndInvokesPoolCallbacks()
        {
            var prefab = CreatePrefab();
            var pool = new ObjectPool<TestPoolBehaviour>(prefab, initialCapacity: 1);

            TestPoolBehaviour instance = pool.Get(new Vector3(3f, 0f, 4f), Quaternion.identity);

            Assert.That(instance, Is.Not.Null);
            Assert.That(instance.gameObject.activeSelf, Is.True);
            Assert.That(instance.activateCalls, Is.EqualTo(1));
            Assert.That(pool.ActiveCount, Is.EqualTo(1));
            Assert.That(pool.AvailableCount, Is.EqualTo(0));

            pool.Return(instance);

            Assert.That(instance.returnCalls, Is.EqualTo(1));
            Assert.That(instance.gameObject.activeSelf, Is.False);
            Assert.That(pool.ActiveCount, Is.EqualTo(0));
            Assert.That(pool.AvailableCount, Is.EqualTo(1));

            Object.DestroyImmediate(prefab.gameObject);
        }

        [Test]
        public void Get_WhenAutoExpandDisabledAndPoolEmpty_ReturnsNull()
        {
            var prefab = CreatePrefab();
            var pool = new ObjectPool<TestPoolBehaviour>(prefab, initialCapacity: 1, autoExpand: false);

            TestPoolBehaviour first = pool.Get(Vector3.zero, Quaternion.identity);
            TestPoolBehaviour second = pool.Get(Vector3.one, Quaternion.identity);

            Assert.That(first, Is.Not.Null);
            Assert.That(second, Is.Null);
            Assert.That(pool.TotalCount, Is.EqualTo(1));
            Assert.That(pool.ActiveCount, Is.EqualTo(1));

            Object.DestroyImmediate(prefab.gameObject);
        }

        [Test]
        public void ReturnAll_DeactivatesEveryActiveObject()
        {
            var prefab = CreatePrefab();
            var pool = new ObjectPool<TestPoolBehaviour>(prefab, initialCapacity: 2);

            TestPoolBehaviour first = pool.Get(Vector3.zero, Quaternion.identity);
            TestPoolBehaviour second = pool.Get(Vector3.one, Quaternion.identity);

            pool.ReturnAll();

            Assert.That(first.gameObject.activeSelf, Is.False);
            Assert.That(second.gameObject.activeSelf, Is.False);
            Assert.That(first.returnCalls, Is.EqualTo(1));
            Assert.That(second.returnCalls, Is.EqualTo(1));
            Assert.That(pool.ActiveCount, Is.EqualTo(0));
            Assert.That(pool.AvailableCount, Is.EqualTo(2));

            Object.DestroyImmediate(prefab.gameObject);
        }

        private static TestPoolBehaviour CreatePrefab()
        {
            var prefabObject = new GameObject("TestPoolPrefab");
            prefabObject.SetActive(false);
            return prefabObject.AddComponent<TestPoolBehaviour>();
        }

        private sealed class TestPoolBehaviour : MonoBehaviour, IPoolableComponent
        {
            public int activateCalls;
            public int returnCalls;

            public void OnObjectActivate()
            {
                activateCalls++;
            }

            public void OnObjectReturn()
            {
                returnCalls++;
            }
        }
    }
}
