using System;
using System.Collections.Generic;
using System.Data;
using NUnit.Framework;
using Quill.Attr;
using Quill.Consts;
using Quill.DataSource;
using Quill.DataSource.Impl;
using Quill.Tests;

namespace Quill.Scope.Impl.Tests {
    [TestFixture()]
    public class ConnectionDecoratorTests : QuillTestBase {

        [Test()]
        public void Decorate_Action_Test() {
            // Arrange
            ConnectionDecorator target = new ConnectionDecorator();
            target.DataSource = new TestDataSource();

            // Act/Assert
            target.Decorate(connection => {
                Assert.IsNotNull(connection);
                Assert.AreEqual(ConnectionState.Open, connection.State);
            });
        }

        [Test()]
        public void Decorate_Func_Test() {
            // Arrange
            const int EXPECTED = 12345;
            ConnectionDecorator target = new ConnectionDecorator();
            target.DataSource = new TestDataSource();

            // Act/Assert
            int result = target.Decorate(conection => {
                Assert.IsNotNull(conection);
                Assert.AreEqual(ConnectionState.Open, conection.State);
                return EXPECTED;
            });
            Assert.AreEqual(EXPECTED, result);
            Assert.AreEqual(ConnectionState.Closed, target.DataSource.GetConnection().State);
        }

        [Test()]
        public void Decorate_MultiDataSource_Test() {
            // Arrange
            const string EXPECTED_DS_NAME = "Hoge";
            const string NOT_EXPECTED_DS_NAME = "Huga";
            ConnectionDecorator target = new ConnectionDecorator();
            MultiDataSource dataSources = new MultiDataSource();
            dataSources.RegisterDataSource(EXPECTED_DS_NAME, new TestDataSource2());
            dataSources.RegisterDataSource(NOT_EXPECTED_DS_NAME, new TestDataSource());
            target.DataSource = dataSources;

            var scopeArgs = new Dictionary<string, object>();
            scopeArgs[QuillKey.DATA_SOURCE_NAME] = EXPECTED_DS_NAME;

            // Act/Assert
            target.Decorate(TestAction, scopeArgs);
        }

        [Test()]
        public void Decorate_MultiDataSource_Exception_Test() {
            // Arrange
            const string NOT_EXPECTED_DS_NAME = "Huga";
            ConnectionDecorator target = new ConnectionDecorator();
            MultiDataSource dataSources = new MultiDataSource();
            dataSources.RegisterDataSource(NOT_EXPECTED_DS_NAME, new TestDataSource());
            target.DataSource = dataSources;

            var scopeArgs = new Dictionary<string, object>();
            scopeArgs[QuillKey.DATA_SOURCE_NAME] = "HogeSource";

            // Act/Assert
            TestUtils.ExecuteExcectedException<ArgumentException>(
                () => target.Decorate(TestAction, scopeArgs));
        }

        [Test()]
        public void Decorate_MultiDataSource_Default_Test() {
            // Arrange
            const string NOT_EXPECTED_DS_NAME = "Hoge";
            ConnectionDecorator target = new ConnectionDecorator();
            MultiDataSource dataSources = new MultiDataSource();
            dataSources.DefaultDataSource = new TestDataSource();
            dataSources.RegisterDataSource(NOT_EXPECTED_DS_NAME, new TestDataSource2());
            target.DataSource = dataSources;

            // Act/Assert
            target.Decorate(TestActionNoAttr);
        }

        [Test()]
        public void Decorate_Twice_Test() {
            // Arrange
            ConnectionDecorator target = new ConnectionDecorator();
            DataSourceImpl ds = new DataSourceImpl(() => new DbConnectionForConnectionDecorator());
            target.DataSource = ds;

            // Act/Assert
            target.Decorate(connection => {
                target.Decorate(connection2 => {
                    Assert.AreEqual(ConnectionState.Open, connection2.State);
                    Assert.AreEqual(connection, connection2);
                });
                Assert.AreEqual(ConnectionState.Open, connection.State);
            });
        }

        #region Helper

        private void TestAction(IDbConnection connection) {
            var actual = connection as DbConnectionForConnectionDecorator;
            Assert.IsNotNull(actual);
            Assert.AreEqual("ExpectedConnection2", actual.Name);
        }

        private void TestActionNoAttr(IDbConnection connection) {
            var actual = connection as DbConnectionForConnectionDecorator;
            Assert.IsNotNull(actual);
            Assert.AreEqual("ExpectedConnection", actual.Name);
        }

        #endregion

        #region TestClasses

        private class TestDataSource : IDataSource {
            private DbConnectionForConnectionDecorator _connection = new DbConnectionForConnectionDecorator();
            public bool IsCalledDispose { get; private set; }
            public void Dispose() {
                IsCalledDispose = true;
            }

            public bool IsCalledGetConnection { get; private set; }
            public IDbConnection GetConnection(string dataSourceName = null) {
                IsCalledGetConnection = true;
                _connection.Name = "ExpectedConnection";
                return _connection;
            }
        }

        private class TestDataSource2 : IDataSource {
            public bool IsCalledDispose { get; private set; }
            public void Dispose() {
                IsCalledDispose = true;
            }

            public bool IsCalledGetConnection { get; private set; }
            public IDbConnection GetConnection(string dataSourceName = null) {
                IsCalledGetConnection = true;
                var connection = new DbConnectionForConnectionDecorator();
                connection.Name = "ExpectedConnection2";
                return connection;
            }
        }

        private class DbConnectionForConnectionDecorator : IDbConnection {
            public string Name { get; set; }

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

            public ConnectionState State { get; set; }

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
                State = ConnectionState.Closed;
            }

            public IDbCommand CreateCommand() {
                throw new NotImplementedException();
            }

            public bool IsCalledDispose { get; private set; }
            public void Dispose() {
                IsCalledDispose = true;
            }

            public void Open() {
                State = ConnectionState.Open;
            }
        }

        #endregion
    }
}