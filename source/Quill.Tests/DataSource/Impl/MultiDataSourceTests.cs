using NUnit.Framework;
using Quill.DataSource.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Data;
using Quill.Tests;

namespace Quill.DataSource.Impl.Tests {
    [TestFixture()]
    public class MultiDataSourceTests {
        [Test()]
        public void MultiDataSourceTest() {
            // Arrange / Act
            var source = new MultiDataSource();
            var fi = source.GetType().GetField("_sources", BindingFlags.NonPublic | BindingFlags.Instance);
            var actual = fi.GetValue(source);

            // Assert
            Assert.IsNotNull(actual);
        }

        [Test()]
        public void RegisterDataSourceTest() {
            // Arrange
            const int EXPECTED = 1;
            var source = new MultiDataSource();
            var fi = source.GetType().GetField("_sources", BindingFlags.NonPublic | BindingFlags.Instance);
            var actual = (IDictionary<string, IDataSource>)fi.GetValue(source);

            // Act
            source.RegisterDataSource("HogeSource", new DataSourceImpl(() => new MultiDsTestConnection()));

            // Assert
            Assert.AreEqual(EXPECTED, actual.Count());
        }

        [Test()]
        public void GetConnection_NoDataSourceName_Test() {
            // Arrange
            var source = new MultiDataSource();
            source.DefaultDataSource = new DataSourceImpl(() => new MultiDsTestConnection());

            // Act
            var actual = source.GetConnection();

            // Assert
            Assert.IsNotNull(actual);
        }

        [Test()]
        public void GetConnection_DataSourceName_Test() {
            // Arrange
            const string TEST_DS_NAME = "HogeSource";
            var source = new MultiDataSource();
            source.RegisterDataSource(TEST_DS_NAME, new DataSourceImpl(() => new MultiDsTestConnection()));

            // Act
            var actual = source.GetConnection(TEST_DS_NAME);

            // Assert
            Assert.IsNotNull(actual);
        }

        [Test()]
        public void GetConnection_DataSourceNotFound_Test() {
            // Arrange
            const string NOT_FOUND_DS_NAME = "HogeSource";
            var source = new MultiDataSource();

            // Act
            TestUtils.ExecuteExcectedException<ArgumentException>(() => {
                source.GetConnection(NOT_FOUND_DS_NAME);
            });
        }

        [Test()]
        public void DisposeTest() {
            // Arrange
            const int EXPECTED = 0;
            var source = new MultiDataSource();
            source.RegisterDataSource("HogeSource", new DataSourceImpl(() => new MultiDsTestConnection()));

            var fi = source.GetType().GetField("_sources", BindingFlags.NonPublic | BindingFlags.Instance);
            var actual = (IDictionary<string, IDataSource>)fi.GetValue(source);

            // Act
            source.Dispose();

            // Assert
            Assert.AreEqual(EXPECTED, actual.Count());
        }

        #region TestClasses

        private class MultiDsTestConnection : IDbConnection {
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

            public void Dispose() {
                // 空実装
            }

            public void Open() {
                throw new NotImplementedException();
            }
        }

        #endregion
    }
}