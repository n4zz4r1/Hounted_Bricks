using System.Collections.Generic;
using System.Threading;
using Core.Data;
using Core.StateMachine.Resource;
using Core.Utils;
using Framework;
using Framework.Base;
using NUnit.Framework;

namespace Tests
{
    [TestFixture]
    public class DataManagerTest
    {
        
        [Test]
        public void TestConcurrentAccessToResourceData()
        {
            DataManager.UnitTest = true;
            
            const int numThreads = 10;
            const int numIterations = 200;
            var executed = 0;

            var threads = new List<Thread>();
            var currentAmount = ResourcesV1.Instance.GetResourcesAmount(ResourceType.CHEST);

            for (var i = 0; i < numThreads; i++)
            {
                threads.Add(new Thread(() =>
                {
                    for (var j = 0; j < numIterations; j++)
                    {
                        executed++;
                        ResourcesV1.Instance.AddResources(ResourceType.CHEST, 1);
                    }
                }));
            }

            // Start all threads
            foreach (var thread in threads)
                thread.Start();

            // Wait for all threads to complete
            foreach (var thread in threads)
                thread.Join();

            // Check final values
            Assert.AreEqual(numThreads * numIterations, executed, "Didnt executed all threads");
            Assert.AreEqual(2000 + currentAmount, ResourcesV1.Instance.GetResourcesAmount(ResourceType.CHEST), "Concurrency Problem: Number of topaz has to have maximum of 200");
        }
        
        [Test]
        public void SpendResourcesFailTest()
        {
            DataManager.UnitTest = true;
            ResourcesV1.Instance.SetResource(ResourceType.COIN, 0);
            ResourcesV1.Instance.AddResources(ResourceType.COIN, 120);
            
            // Check final values
            // Assert.AreEqual(numThreads * numIterations, executed, "Didnt executed all threads");
            Assert.AreEqual(false, ResourcesV1.Instance.SpendResources(ResourceType.COIN, 121), "Spend resource differs.");
            Assert.AreEqual(120, ResourcesV1.Instance.GetResourcesAmount(ResourceType.COIN), "Spend resource differs.");
        }
        
        [Test]
        public void SpendResourcesSuccessTest()
        {
            DataManager.UnitTest = true;
            ResourcesV1.Instance.SetResource(ResourceType.COIN, 0);
            ResourcesV1.Instance.AddResources(ResourceType.COIN, 120);
            
            // Check final values
            // Assert.AreEqual(numThreads * numIterations, executed, "Didnt executed all threads");
            Assert.AreEqual(true, ResourcesV1.Instance.SpendResources(ResourceType.COIN, 120), "Spend resource differs.");
            Assert.AreEqual(0, ResourcesV1.Instance.GetResourcesAmount(ResourceType.COIN), "Spend resource differs.");
        }

    }
}