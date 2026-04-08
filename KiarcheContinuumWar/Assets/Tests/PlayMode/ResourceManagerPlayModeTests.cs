using System.Collections;
using KiarcheContinuumWar.Core;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace KiarcheContinuumWar.Tests.PlayMode
{
    public class ResourceManagerPlayModeTests
    {
        [UnityTest]
        public IEnumerator Start_InitializesExpectedStartingResources()
        {
            var gameObject = new GameObject("ResourceManagerTest");
            var manager = gameObject.AddComponent<ResourceManager>();

            yield return null;

            Assert.That(manager.Materials, Is.EqualTo(100));
            Assert.That(manager.Energy, Is.EqualTo(100));
            Assert.That(manager.Food, Is.EqualTo(100));
            Assert.That(manager.Knowledge, Is.EqualTo(50));

            Object.Destroy(gameObject);
        }

        [UnityTest]
        public IEnumerator AddAndSpendResource_UpdateValuesAndRaiseEvents()
        {
            var gameObject = new GameObject("ResourceManagerTest");
            var manager = gameObject.AddComponent<ResourceManager>();
            ResourceType? changedType = null;
            int changedValue = -1;
            manager.OnResourceChanged += (type, value) =>
            {
                changedType = type;
                changedValue = value;
            };

            yield return null;

            manager.AddResource(ResourceType.Materials, 25);
            Assert.That(changedType, Is.EqualTo(ResourceType.Materials));
            Assert.That(changedValue, Is.EqualTo(125));
            Assert.That(manager.Materials, Is.EqualTo(125));

            bool spent = manager.SpendResource(ResourceType.Materials, 40);
            Assert.That(spent, Is.True);
            Assert.That(manager.Materials, Is.EqualTo(85));

            Object.Destroy(gameObject);
        }

        [UnityTest]
        public IEnumerator SpendResource_WhenInsufficient_DoesNotChangeValue()
        {
            var gameObject = new GameObject("ResourceManagerTest");
            var manager = gameObject.AddComponent<ResourceManager>();

            yield return null;

            bool spent = manager.SpendResource(ResourceType.Knowledge, 1000);

            Assert.That(spent, Is.False);
            Assert.That(manager.Knowledge, Is.EqualTo(50));
            Assert.That(manager.CanAfford(ResourceType.Knowledge, 51), Is.False);

            Object.Destroy(gameObject);
        }

        [UnityTest]
        public IEnumerator PositiveRate_AddsPassiveIncomeOverFrame()
        {
            var gameObject = new GameObject("ResourceManagerTest");
            var manager = gameObject.AddComponent<ResourceManager>();
            float reportedRate = -1f;
            manager.OnResourceRateChanged += (_, rate) => reportedRate = rate;

            yield return null;

            int initialMaterials = manager.Materials;
            manager.SetResourceRate(ResourceType.Materials, 1000f);

            yield return null;

            Assert.That(reportedRate, Is.EqualTo(1000f));
            Assert.That(manager.Materials, Is.GreaterThan(initialMaterials));

            Object.Destroy(gameObject);
        }
    }
}
