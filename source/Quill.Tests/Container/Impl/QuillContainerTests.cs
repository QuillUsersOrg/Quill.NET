using System;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using Quill.Inject;
using Quill.Tests;

namespace Quill.Container.Impl.Tests {
    [TestFixture()]
    public class QuillContainerTests : QuillTestBase {

        #region ConstructorTest
        [Test]
        public void Constructor_NullTest() {
            // Arrange/Act
            QuillContainer actual = new QuillContainer(null);
            FieldInfo fieldInfo = actual.GetType().GetField("_components", BindingFlags.NonPublic | BindingFlags.Instance);
            
            // Assert
            // コンストラクタにnullが渡された場合もデフォルトのDictionaryが設定されている
            Assert.IsNotNull(fieldInfo.GetValue(actual));
        }

        [Test]
        public void Constructor_DictionaryTest() {
            // Arrange/Act
            IDictionary<Type, object> expected = new Dictionary<Type, object>();
            QuillContainer actual = new QuillContainer(expected);
            FieldInfo fieldInfo = actual.GetType().GetField("_components", BindingFlags.NonPublic | BindingFlags.Instance);
            var component = actual.GetComponent<TestEntity>();

            // Assert
            Assert.AreEqual(expected, fieldInfo.GetValue(actual));
            Assert.IsNotNull(component);
        }

        #endregion

        #region GetComponentTest
        [Test()]
        public void GetComponent_CacheFalse_InjectionFalse_Test() {
            // Arrange
            QuillContainer target = new QuillContainer();
            TestInjector testInjector = new TestInjector();
            testInjector.IsInjected = false;
            QuillManager.Injector = testInjector;

            // Act
            var component = target.GetComponent<TestEntity>(isCache: false, withInjection: false);
            component.Hoge = default(int) + 1; // defaultとは違う値
            component = target.GetComponent<TestEntity>(isCache: false, withInjection: false);

            // Assert
            Assert.IsNotNull(component);
            Assert.AreEqual(default(int), component.Hoge, "キャッシュしていないのでdefault");
            Assert.IsFalse(testInjector.IsInjected, "Injectionは行われていない");
        }

        [Test()]
        public void GetComponent_CacheTrue_InjectionFalse_Test() {
            // Arrange
            QuillContainer target = new QuillContainer();
            TestInjector testInjector = new TestInjector();
            testInjector.IsInjected = false;
            QuillManager.Injector = testInjector;
            int expected = default(int) + 1; // defaultとは違う値

            // Act
            var component = target.GetComponent<TestEntity>(isCache: true, withInjection: false);
            component.Hoge = expected;
            component = target.GetComponent<TestEntity>(isCache: true, withInjection: false);

            // Assert
            Assert.IsNotNull(component);
            Assert.AreEqual(expected, component.Hoge, "キャッシュしているので値保持");
            Assert.IsFalse(testInjector.IsInjected, "Injectionは行われていない");
        }

        [Test()]
        public void GetComponent_CacheFalse_InjectionTrue_Test() {
            // Arrange
            QuillContainer target = new QuillContainer();
            TestInjector testInjector = new TestInjector();
            testInjector.IsInjected = false;
            QuillManager.Injector = testInjector;
            int expected = default(int) + 1; // defaultとは違う値

            // Act
            var component = target.GetComponent<TestEntity>(isCache: true, withInjection: true);
            component.Hoge = expected; // defaultとは違う値
            component = target.GetComponent<TestEntity>(isCache: true, withInjection: true);

            // Assert
            Assert.IsNotNull(component);
            Assert.AreEqual(expected, component.Hoge, "キャッシュしていないのでdefault");
            Assert.IsTrue(testInjector.IsInjected, "Injectionが行われている想定");
        }

        [Test()]
        public void GetComponent_CacheTrue_InjectionTrue_Test() {
            // Arrange
            QuillContainer target = new QuillContainer();
            TestInjector testInjector = new TestInjector();
            testInjector.IsInjected = false;
            QuillManager.Injector = testInjector;

            // Act
            var component = target.GetComponent<TestEntity>(isCache: false, withInjection: true);
            component.Hoge = default(int) + 1; // defaultとは違う値
            component = target.GetComponent<TestEntity>(isCache: false, withInjection: true);

            // Assert
            Assert.IsNotNull(component);
            Assert.AreEqual(default(int), component.Hoge, "キャッシュしていないのでdefault");
            Assert.IsTrue(testInjector.IsInjected, "Injectionが行われている想定");
        }

        #endregion

        #region Helper

        private class TestInjector : IQuillInjector {
            public bool IsInjected { get; set; }
            public void Inject(object target) {
                IsInjected = true;
            }

            public virtual void Dispose() {
                // 実装なし
            }
        }

        private class TestEntity {
            public TestField EntityField { get; set; }
            public int Hoge { get; set; }
        }

        private class TestField {
            public bool IsOK { get; set; }
        }

        #endregion
    }
}