using System;
#if SILVERLIGHT
using Microsoft.Phone.Testing;
#endif
#if NETFX_CORE
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#else
using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif
using Wintellect.Sterling.Core;
using Wintellect.Sterling.Test.Helpers;

namespace Wintellect.Sterling.Test.Keys
{
#if SILVERLIGHT
    [Tag("CompositeKey")]
#endif
    [TestClass]
    public class TestCompositeKey
    {
        private SterlingEngine _engine;
        private ISterlingDatabaseInstance _databaseInstance;

        [TestInitialize]
        public void TestInit()
        {            
            _engine = Factory.NewEngine();
            _engine.Activate();
            _databaseInstance = _engine.SterlingDatabase.RegisterDatabase<TestDatabaseInstance>();    
            _databaseInstance.PurgeAsync().Wait();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _databaseInstance.PurgeAsync().Wait();
            _engine.Dispose();
            _databaseInstance = null;            
        }
       

        [TestMethod]
        public void TestSave()
        {
            const int LISTSIZE = 20;

            var random = new Random();
            
            // test saving and reloading
            var list = new TestCompositeClass[LISTSIZE];

            for (var x = 0; x < LISTSIZE; x++)
            {
                var testClass = new TestCompositeClass
                                    {
                                        Key1 = random.Next(),
                                        Key2 = random.Next().ToString(),
                                        Key3 = Guid.NewGuid(),
                                        Key4 = DateTime.Now.AddMinutes(-1*random.Next(100)),
                                        Data = Guid.NewGuid().ToString()
                                    };
                list[x] = testClass;
                _databaseInstance.SaveAsync( testClass ).Wait();
            }

            for (var x = 0; x < LISTSIZE; x++)
            {
                var compositeKey = TestDatabaseInstance.GetCompositeKey(list[x]);
                var actual = _databaseInstance.LoadAsync<TestCompositeClass>( compositeKey ).Result;
                Assert.IsNotNull(actual, "Load failed.");
                Assert.AreEqual(compositeKey, TestDatabaseInstance.GetCompositeKey(actual), "Load failed: key mismatch.");
                Assert.AreEqual(list[x].Data, actual.Data, "Load failed: data mismatch.");
            }
        }
    }
}