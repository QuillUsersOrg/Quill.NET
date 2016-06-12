using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using Quill.Exception;
using Quill.Message;
using Quill.Tests;
using QM = Quill.QuillManager;

namespace Quill.Inject.Impl.Tests {
    [TestFixture()]
    public class QuillInjectorTests : QuillTestBase {
        [Test()]
        public void Inject_Recursive_Test() {
            // Arrange
            QuillInjector target = new QuillInjector();
            var filter = (InjectionFilterBase)QM.InjectionFilter;
            filter.InjectionTargetTypes.Add(typeof(InjectionRecursive));
            filter.InjectionTargetTypes.Add(typeof(InjectionTargetMain));
            filter.InjectionTargetTypes.Add(typeof(InjectionTargetSub));
            var actual = new InjectionRecursive();

            // Act
            target.Inject(actual);

            // Assert
            Assert.IsNotNull(actual.Actual);
            Assert.IsNotNull(actual.Actual.Actual);
            Assert.IsNotNull(actual.SameSourceType, 
                "フィールドにインジェクション対象と同じ型のものがあても無限ループにならず、かつインスタンスは設定されている想定");
            Assert.AreEqual(actual.SameSourceType, actual);
        }

        [Test()]
        public void InjectTest() {
            // Arrange
            QuillInjector target = new QuillInjector();
            var filter = (InjectionFilterBase)QM.InjectionFilter;
            filter.InjectionTargetTypes.Add(typeof(InjectionTargetSub));
            var actual = new InjectionTargetMain();

            // Act
            target.Inject(actual);

            // Assert
            Assert.IsNotNull(actual.Actual);
            Assert.IsNull(InjectionTargetMain.StaticActual, "デフォルトではstaticフィールドはインジェクション対象外");
        }

        [Test()]
        public void Inject_AlreadyInjected_Test() {
            // Arrange
            QuillInjector target = new QuillInjector();
            var filter = (InjectionFilterBase)QM.InjectionFilter;
            filter.InjectionTargetTypes.Add(typeof(InjectionTargetSub));
            var actual = new InjectionTargetMain();

            // Act
            target.Inject(actual);
            var expected = actual.Actual;
            target.Inject(actual); // 二回実行

            // Assert
            Assert.IsNotNull(actual.Actual, "二回実行してもインジェクション結果は失われない想定");
            Assert.AreEqual(expected, actual.Actual, "二回実行してもインスタンスは変わらない想定");
        }

        [Test()]
        public void Inject_NotTarget_Test() {
            QM.OutputLog(GetType(), EnumMsgCategory.INFO, "Test start.");

            // Arrange
            QuillInjector target = new QuillInjector();
            var filter = (InjectionFilterBase)QM.InjectionFilter;
            filter.IsTargetTypeDefault = false;
            var actual = new InjectionTargetMain();

            // Act
            target.Inject(actual);

            // Assert
            Assert.IsNull(actual.Actual, "インジェクション対象外の想定");
        }

        [Test()]
        public void Inject_TargetNull_Test() {
            // Arrange
            QuillInjector target = new QuillInjector();

            // Act/Assert
            TestUtils.ExecuteExcectedException<QuillException>(() => target.Inject(null));
        }

        [Test()]
        public void DisposeTest() {
            // Arrange
            FieldInfo targetField = typeof(QuillInjector).GetField("_injectedTypes", BindingFlags.NonPublic | BindingFlags.Instance);
            QuillInjector target = new QuillInjector();
            ISet<Type> injectedTypes = targetField.GetValue(target) as ISet<Type> ;
            Assert.IsNotNull(injectedTypes);
            injectedTypes.Add(typeof(int));
            injectedTypes.Add(typeof(string));

            // Act
            target.Dispose();

            // Assert
            Assert.AreEqual(0, injectedTypes.Count(), "クリアされている想定");
        }

        #region TestClasses

        private class InjectionTargetMain {
            public static InjectionTargetSub StaticActual = null;
            public virtual InjectionTargetSub Actual { get; set; }
        }

        private class InjectionTargetSub {

        }

        private class InjectionRecursive {
            // 循環参照してしまわないか確認のため
            public virtual InjectionRecursive SameSourceType { get; set; }
            public virtual InjectionTargetMain Actual { get; set; }
        }

        #endregion
    }
}