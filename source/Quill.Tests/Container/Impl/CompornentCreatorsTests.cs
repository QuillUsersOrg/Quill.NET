using System;
using NUnit.Framework;
using Quill.Exception;
using Quill.Tests;
using TU = Quill.Tests.TestUtils;

namespace Quill.Container.Impl.Tests {
    [TestFixture()]
    public class CompornentCreatorsTests : QuillTestBase {
        [Test()]
        public void Create_ComponentTypeNull_Test() {
            // Arrange
            ComponentCreators target = new ComponentCreators();

            // Act
            TU.ExecuteExcectedException<ArgumentNullException>(
                () => target.Create(null));
        }

        [Test()]
        public void Create_Default_Test() {
            // Arrange
            ComponentCreators target = new ComponentCreators();

            // Act
            var actual = target.Create(typeof(NormalComponent)) as NormalComponent;

            // Assert
            Assert.IsNotNull(actual);
        }

        [Test()]
        public void Create_ArgumentComponent_Test() {
            // Arrange
            ComponentCreators target = new ComponentCreators();

            // Act
            TU.ExecuteExcectedException<QuillException>(
                () => target.Create(typeof(ArgumentConstructorComponent)));
        }

        [Test()]
        public void AddCreatorTest() {
            // Arrange
            ComponentCreators target = new ComponentCreators();
            target.AddCreator(typeof(IComponentCreator), t => new ComponentCreators());

            // Act
            var actual = target.Create(typeof(IComponentCreator)) as IComponentCreator;

            // Assert
            Assert.IsNotNull(actual);
        }

        [Test()]
        public void AddCreator_NoCreatorIF_Test() {
            // Arrange
            ComponentCreators target = new ComponentCreators();

            // Act
            TU.ExecuteExcectedException<QuillException>(
                () => target.Create(typeof(IComponentCreator)));
        }

        #region 内部クラス

        /// <summary>
        /// 通常のクラス
        /// </summary>
        private class NormalComponent {
            public NormalComponent() { }
        }

        /// <summary>
        /// 引数ありコンストラクタのみのクラス
        /// </summary>
        private class ArgumentConstructorComponent {
            public ArgumentConstructorComponent(string hoge) { }
        }
        #endregion
    }
}