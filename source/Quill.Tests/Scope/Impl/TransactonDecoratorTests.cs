using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using NUnit.Framework;
using Quill.Ado;
using Quill.DataSource;
using Quill.Exception;
using Quill.Scope.Impl;

namespace Quill.Tests.Scope.Impl {
    [TestFixture()]
    public class TransactonDecoratorTests : QuillTestBase {
        [Test()]
        public void Decorate_Exception_Test() {
            // Arrange
            TransactionDecorator target = new TransactionDecorator();

            // Act/Assert
            TestUtils.ExecuteExcectedException<QuillException>(
                () => target.Decorate(c => { /* Dummy */ }));
        }

        [Test()]
        public void Decorate_TxAction_Test() {
            // Arrange
            TransactionDecorator target = new TransactionDecorator();
            ConnectionDecorator cd = new ConnectionDecorator();
            cd.DataSource = new TestDataSource();
            FieldInfo cdField = target.GetType().GetField(
                "_connectionDecorator", BindingFlags.NonPublic | BindingFlags.Instance);
            cdField.SetValue(target, cd);
            
            // Act/Assert
            target.Decorate(tx => {
                Assert.IsNotNull(tx);
                Assert.AreEqual(ConnectionState.Open, tx.Connection.State);
            });

            // Assert
            Assert.IsTrue(TestTx.IsCalledCommit);
            Assert.IsTrue(TestTx.IsCalledDispose);
            Assert.IsFalse(TestTx.IsCalledRollbacked);
        }

        [Test()]
        public void Decorate_TxFunc_Test() {
            // Arrange
            const int EXPECTED = 12345;
            TransactionDecorator target = new TransactionDecorator();
            ConnectionDecorator cd = new ConnectionDecorator();
            cd.DataSource = new TestDataSource();
            FieldInfo cdField = target.GetType().GetField(
                "_connectionDecorator", BindingFlags.NonPublic | BindingFlags.Instance);
            cdField.SetValue(target, cd);

            // Act/Assert
            var actual = target.Decorate(tx => {
                Assert.IsNotNull(tx);
                Assert.AreEqual(ConnectionState.Open, tx.Connection.State);
                return EXPECTED;
            });

            // Assert
            Assert.AreEqual(EXPECTED, actual);
            Assert.IsTrue(TestTx.IsCalledCommit);
            Assert.IsTrue(TestTx.IsCalledDispose);
            Assert.IsFalse(TestTx.IsCalledRollbacked);
        }

        [Test()]
        public void Decorate_TxAction_Rollback_Test() {
            // Arrange
            TransactionDecorator target = new TransactionDecorator();
            ConnectionDecorator cd = new ConnectionDecorator();
            cd.DataSource = new TestDataSource();
            FieldInfo cdField = target.GetType().GetField(
                "_connectionDecorator", BindingFlags.NonPublic | BindingFlags.Instance);
            cdField.SetValue(target, cd);

            // Act/Assert
            TestUtils.ExecuteExcectedException<QuillException>(
            () => {
                target.Decorate(connection => {
                    throw new QuillException("TestException");
                });
            });

            // Assert
            Assert.IsFalse(TestTx.IsCalledCommit);
            Assert.IsTrue(TestTx.IsCalledDispose);
            Assert.IsTrue(TestTx.IsCalledRollbacked);
        }

        #region Test Classes

        private class TestDataSource : IDataSource {
            public void Dispose() {
                throw new NotImplementedException();
            }

            public IDbConnection GetConnection(string dataSourceName = null) {
                return new TestDbConnection();
            }
        }

        private class TestDbConnection : IDbConnection {
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

            public IDbTransaction BeginTransaction() {
                return new TestTx(this);
            }

            public IDbTransaction BeginTransaction(IsolationLevel il) {
                return new TestTx(this);
            }

            public void ChangeDatabase(string databaseName) {
                throw new NotImplementedException();
            }

            public ConnectionState State { get; private set; }
            public void Close() {
                State = ConnectionState.Closed;
            }

            public IDbCommand CreateCommand() {
                throw new NotImplementedException();
            }

            public void Dispose() {
                throw new NotImplementedException();
            }

            public void Open() {
                State = ConnectionState.Open;
            }
        }

        private class TestTx : IDbTransaction {
            private IDbConnection _connection;

            public TestTx(IDbConnection connection) {
                _connection = connection;
                IsCalledCommit = false;
                IsCalledDispose = false;
                IsCalledRollbacked = false;
            }

            public IDbConnection Connection {
                get {
                    return _connection;
                }
            }

            public IsolationLevel IsolationLevel {
                get {
                    return IsolationLevel.ReadCommitted;
                }
            }

            public static bool IsCalledCommit { get; private set; }
            public void Commit() {
                IsCalledCommit = true;
            }

            public static bool IsCalledDispose { get; private set; }
            public void Dispose() {
                IsCalledDispose = true;
            }

            public static bool IsCalledRollbacked { get; private set; }
            public void Rollback() {
                IsCalledRollbacked = true;
            }
        }

        #endregion
    }
}
