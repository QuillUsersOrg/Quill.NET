using System;
using System.Data;
using System.Reflection;
using NUnit.Framework;

namespace Quill.DataSource.Impl.Tests {
    [TestFixture()]
    public class DataSourceImplTests {
        [Test()]
        public void DataSourceImplTest() {
            // Arrange / Act
            using(var actual = new DataSourceImpl(() => new DsTestConnection())) {
                // Assert
                var fi = actual.GetType().GetField("_connection", BindingFlags.Instance | BindingFlags.NonPublic);
                var connection = fi.GetValue(actual);
                Assert.IsNotNull(connection);
            }
        }

        [Test()]
        public void GetConnectionTest() {
            // Arrange
            using(var source = new DataSourceImpl(() => new DsTestConnection())) {
                // Act
                var actual = source.GetConnection();

                // Assert
                Assert.IsNotNull(actual);
                Assert.AreEqual(typeof(DsTestConnection), actual.GetType());
            }
        }

        [Test()]
        public void DisposeTest() {
            // Arrange
            var connection = new DsTestConnection();
            var source = new DataSourceImpl(() => connection);

            // Act
            var actual = (DsTestConnection)source.GetConnection();
            source.Dispose();

            // Assert
            Assert.IsTrue(actual.IsDispose);
        }

        #region TestClasses

        private class DsTestConnection : IDbConnection {
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
                    return ConnectionState.Closed;
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

            public bool IsDispose { get; private set; }
            public void Dispose() {
                IsDispose = true;
            }

            public void Open() {
                throw new NotImplementedException();
            }
        }

        #endregion
    }
}