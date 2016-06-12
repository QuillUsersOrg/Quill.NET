using System;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using Quill.Tests;

namespace Quill.Inject.Impl.Tests {
    [TestFixture()]
    public class InjectionFilterBaseTests : QuillTestBase {
        [Test()]
        public void InjectionFilterBaseTest() {
            // Arrange/Act
            var target = new InjectionFilterBase();

            // Assert
            Assert.IsNotNull(target.NotInjectionTargetTypes);
            Assert.IsNotNull(target.InjectionTargetTypes);
            Assert.IsTrue(target.IsTargetTypeDefault);
        }

        [Test()]
        public void GetTargetFieldBindinFlagsTest() {
            // Arrange
            var target = new InjectionFilterBase();
            var expected = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

            // Act
            var actual = target.GetTargetFieldBindinFlags();

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [Test()]
        public void IsTargetType_InjectionTargetTypeContains_Test() {
            // Arrange
            var target = new InjectionFilterBase();
            var targetType = typeof(int);
            target.InjectionTargetTypes.Add(targetType);

            // Act
            var actual = target.IsTargetType(targetType);

            // Assert
            Assert.IsTrue(actual);
        }

        [Test()]
        public void IsTargetType_NotInjectionTargetTypeContains_Test() {
            // Arrange
            var target = new InjectionFilterBase();
            var targetType = typeof(int);
            target.IsTargetTypeDefault = true;
            target.NotInjectionTargetTypes.Add(targetType);

            // Act
            var actual = target.IsTargetType(targetType);

            // Assert
            Assert.IsFalse(actual);
        }

        [Test()]
        public void IsTargetType_IsDefaultTargetTrue_Test() {
            // Arrange
            var target = new InjectionFilterBase();
            target.IsTargetTypeDefault = true;

            // Act
            var actual = target.IsTargetType(typeof(TestFieldClass));

            // Assert
            Assert.IsTrue(actual);
        }

        [Test()]
        public void IsTargetType_IsDefaultTargetFalse_Test() {
            // Arrange
            var target = new InjectionFilterBase();
            target.IsTargetTypeDefault = false;

            // Act
            var actual = target.IsTargetType(typeof(int));

            // Assert
            Assert.IsFalse(actual);
        }

        [Test()]
        public void IsTargetType_BothTargetTypeContains_Test() {
            // Arrange
            var target = new InjectionFilterBase();
            var targetType = typeof(int);
            target.IsTargetTypeDefault = false;
            target.InjectionTargetTypes.Add(targetType);
            target.NotInjectionTargetTypes.Add(targetType);

            // Act
            var actual = target.IsTargetType(targetType);

            // Assert
            Assert.IsTrue(actual);
        }

        [Test()]
        public void DisposeTest() {
            // Arrange
            var target = new InjectionFilterBase();
            target.InjectionTargetTypes.Add(typeof(string));
            target.NotInjectionTargetTypes.Add(typeof(DateTime));
            Assert.Greater(target.InjectionTargetTypes.Count(), 0);
            Assert.Greater(target.NotInjectionTargetTypes.Count(), 0);

            // Act
            target.Dispose();

            // Assert
            Assert.AreEqual(0, target.InjectionTargetTypes.Count());
            Assert.AreEqual(0, target.NotInjectionTargetTypes.Count());
        }

        [Test()]
        public void IsTargetFieldTest() {
            // Arrange
            var target = new InjectionFilterBase();
            var testEntity = new TestClass();
            var testField = typeof(TestClass).GetField("_testField", BindingFlags.NonPublic | BindingFlags.Instance);

            // Act
            var actual = target.IsTargetField(typeof(TestClass), testField);

            // Assert
            Assert.AreEqual(target.IsTargetTypeDefault, actual);
        }

        #region

        private class TestClass {
            /// <summary>
            /// テスト用フィールド
            /// </summary>
            private TestFieldClass _testField = new TestFieldClass();

            public override string ToString() {
                return _testField.ToString();
            }
        }

        private class TestFieldClass {

        }

        #endregion
    }
}