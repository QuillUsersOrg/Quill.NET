using NUnit.Framework;
using Quill.Container.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace Quill.Container.Impl.Tests {
    [TestFixture()]
    public class TypeMapImplTests {
        [Test()]
        public void AddTest() {
            // Arragne
            TypeMapImpl target = new TypeMapImpl();

            // Act
            target.Add(typeof(int), typeof(string));

            // Assert
            FieldInfo field = typeof(TypeMapImpl).GetField("_typeMap", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetField);
            object actual = field.GetValue(target);
            Assert.IsNotNull(actual);
            Assert.IsTrue(typeof(IDictionary<Type, Type>).IsAssignableFrom(actual.GetType()));

            IDictionary<Type, Type> actualValue = (IDictionary<Type, Type>)actual;
            Assert.AreEqual(typeof(string), actualValue[typeof(int)]);
        }

        [Test()]
        public void GetComponentTypeTest() {
            // Arrange
            TypeMapImpl target = new TypeMapImpl();
            target.Add(typeof(long), typeof(DateTime));

            // Act
            Type actual = target.GetComponentType(typeof(long));

            // Arrange
            Assert.AreEqual(typeof(DateTime), actual);
        }

        [Test()]
        public void GetComponentType_NotMapped_Test() {
            // Arrange
            TypeMapImpl target = new TypeMapImpl();
            target.Add(typeof(long), typeof(DateTime));

            // Act
            Type actual = target.GetComponentType(typeof(float));

            // Arrange
            Assert.AreEqual(typeof(float), actual);
        }

        [Test()]
        public void IsMapped_True_Test() {
            // Arrange
            TypeMapImpl target = new TypeMapImpl();
            target.Add(typeof(long), typeof(DateTime));

            // Act
            bool actual = target.IsMapped(typeof(long));

            // Arrange
            Assert.IsTrue(actual);
        }

        [Test()]
        public void IsMapped_False_Test() {
            // Arrange
            TypeMapImpl target = new TypeMapImpl();
            target.Add(typeof(long), typeof(DateTime));

            // Act
            bool actual = target.IsMapped(typeof(double));

            // Arrange
            Assert.IsFalse(actual);
        }
    }
}