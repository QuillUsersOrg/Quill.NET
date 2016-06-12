using NUnit.Framework;
using Quill.Container.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using TU = Quill.Tests.TestUtils;

namespace Quill.Container.Impl.Tests {
    [TestFixture()]
    public class AbstractConnectionCreatorTests {
        [Test()]
        public void CreateTest() {
            // Arrange
            var target = new ConnectionCreatorImpl();

            // Act
            var actual = target.Create(typeof(IDbConnection)) as IDbConnection;

            // Assert
            Assert.IsNotNull(actual);
        }

        [Test()]
        public void Create_NotIDbConnection_Test() {
            // Arrange
            var target = new ConnectionCreatorImpl();

            // Act
            TU.ExecuteExcectedException<ArgumentException>(
                () => target.Create(typeof(IComponentCreator)));
        }

        [Test()]
        public void Create_ComponentTypeNull_Test() {
            // Arrange
            var target = new ConnectionCreatorImpl();

            // Act
            TU.ExecuteExcectedException<ArgumentNullException>(
                () => target.Create(null));
        }

        /// <summary>
        /// テスト用実装クラス
        /// </summary>
        private class ConnectionCreatorImpl : AbstractConnectionCreator {
            protected override IDbConnection CreateConnection(Type connectionType) {
                return new DbConnectionImpl();
            }
        }

        private class DbConnectionImpl : IDbConnection {
            public string ConnectionString {
                get {
                    throw new NotImplementedException();
                }

                set {
                    throw new NotImplementedException();
                }
            }

            public int ConnectionTimeout {
                get {
                    throw new NotImplementedException();
                }
            }

            public string Database {
                get {
                    throw new NotImplementedException();
                }
            }

            public ConnectionState State {
                get {
                    throw new NotImplementedException();
                }
            }

            public IDbTransaction BeginTransaction() {
                throw new NotImplementedException();
            }

            public IDbTransaction BeginTransaction(IsolationLevel il) {
                throw new NotImplementedException();
            }

            public void ChangeDatabase(string databaseName) {
                throw new NotImplementedException();
            }

            public void Close() {
                throw new NotImplementedException();
            }

            public IDbCommand CreateCommand() {
                throw new NotImplementedException();
            }

            public void Dispose() {
                throw new NotImplementedException();
            }

            public void Open() {
                throw new NotImplementedException();
            }
        }
    }
}