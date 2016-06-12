using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using NUnit.Framework;
using Quill.Ado;
using Quill.Attr;
using Quill.Consts;
using Quill.Container;
using Quill.Container.Impl;
using Quill.DataSource;
using Quill.DataSource.Impl;
using Quill.Tests;
using QM = Quill.QuillManager;

namespace Quill.Scope.Tests {
    [TestFixture()]
    public class TxTests : QuillTestBase {
        [Test()]
        public void ExecuteTest() {
            // Arrange
            QM.ComponentCreator = CreateComponentCreator();

            var sql = "SELECT Name FROM Hoge WHERE ID=/* id */123";
            var parameters = new Dictionary<string, object>();
            parameters["name"] = "test";
            parameters["id"] = 256;

            var scopeArgs = new Dictionary<string, object>();
            scopeArgs[QuillKey.DATA_SOURCE_NAME] = "HogeSource";
       
            // Act
            var actual = Tx.Execute(tx => tx.Select<TestEntity>(sql, parameters), scopeArgs);

            // Assert
            Assert.IsNotNull(actual);
        }

        #region Helper

        private IComponentCreator CreateComponentCreator() {
            var creator = new ComponentCreators();
            creator.AddCreator(typeof(IDataSource), t => {
                var source = new MultiDataSource();
                source.RegisterDataSource("HogeSource", new DataSourceImpl(() => new ConnectionImpl()));
                return source;
            });

            return creator;
        }
        #endregion

        #region TestClasses

        private class TestEntity {
            public int Id { get; set; }
            public string Name { get; set; }
            [Column("UpdateTime")]
            public DateTime ModifyTime { get; set; }
        }

        private class DataReaderImpl : IDataReader {
            public const int COL_ID = 0;
            public const int COL_NAME = 1;
            public const int COL_UPD = 2;

            public int CurrentIndex { get; set; }

            public static readonly string[] COLUMN_NAMES = {
                "Id", "Name", "UpdateTime"
            };

            public static readonly object[,] DATAS =  {
                { 1, "秋山好古", new DateTime(2000, 1, 1, 10, 0, 0) },
                { 2, "秋山真之", new DateTime(2010, 2, 3, 4, 5, 6) },
                { 3, "正岡子規", new DateTime(2025, 10, 15, 11, 20, 30) }
            };

            public DataReaderImpl() {
                CurrentIndex = -1;
            }

            public object this[string name] {
                get {
                    throw new NotImplementedException();
                }
            }

            public object this[int i] {
                get {
                    throw new NotImplementedException();
                }
            }

            public int Depth {
                get {
                    throw new NotImplementedException();
                }
            }

            public int FieldCount {
                get {
                    return COLUMN_NAMES.Length;
                }
            }

            public bool IsClosed {
                get {
                    return false;
                }
            }

            public int RecordsAffected {
                get {
                    throw new NotImplementedException();
                }
            }

            public void Close() {
                // 実装なし
            }

            public void Dispose() {
                // 実装なし
            }

            public bool GetBoolean(int i) {
                throw new NotImplementedException();
            }

            public byte GetByte(int i) {
                throw new NotImplementedException();
            }

            public long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length) {
                throw new NotImplementedException();
            }

            public char GetChar(int i) {
                throw new NotImplementedException();
            }

            public long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length) {
                throw new NotImplementedException();
            }

            public IDataReader GetData(int i) {
                throw new NotImplementedException();
            }

            public string GetDataTypeName(int i) {
                throw new NotImplementedException();
            }

            public DateTime GetDateTime(int i) {
                throw new NotImplementedException();
            }

            public decimal GetDecimal(int i) {
                throw new NotImplementedException();
            }

            public double GetDouble(int i) {
                throw new NotImplementedException();
            }

            public Type GetFieldType(int i) {
                throw new NotImplementedException();
            }

            public float GetFloat(int i) {
                throw new NotImplementedException();
            }

            public Guid GetGuid(int i) {
                throw new NotImplementedException();
            }

            public short GetInt16(int i) {
                throw new NotImplementedException();
            }

            public int GetInt32(int i) {
                throw new NotImplementedException();
            }

            public long GetInt64(int i) {
                throw new NotImplementedException();
            }

            public string GetName(int i) {
                return COLUMN_NAMES[i];
            }

            public int GetOrdinal(string name) {
                throw new NotImplementedException();
            }

            public DataTable GetSchemaTable() {
                throw new NotImplementedException();
            }

            public string GetString(int i) {
                throw new NotImplementedException();
            }

            public object GetValue(int i) {
                return DATAS[CurrentIndex, i];
            }

            public int GetValues(object[] values) {
                throw new NotImplementedException();
            }

            public bool IsDBNull(int i) {
                throw new NotImplementedException();
            }

            public bool NextResult() {
                throw new NotImplementedException();
            }

            public bool Read() {
                CurrentIndex++;
                return (CurrentIndex < (DATAS.Length / COLUMN_NAMES.Length));
            }
        }

        private class ConnectionImpl : IDbConnection {
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

            private ConnectionState _state = ConnectionState.Closed;
            public ConnectionState State {
                get {
                    return _state;
                }
            }

            public IDbTransaction BeginTransaction() {
                return new TestTx(this);
            }

            public IDbTransaction BeginTransaction(IsolationLevel il) {
                throw new NotImplementedException();
            }

            public void ChangeDatabase(string databaseName) {
                throw new NotImplementedException();
            }

            public void Close() {
                _state = ConnectionState.Closed;
            }

            public IDbCommand CreateCommand() {
                return new CommandImpl(this);
            }

            public void Dispose() {
                // 実装なし
            }

            public void Open() {
                _state = ConnectionState.Open;
            }
        }

        private class CommandImpl : IDbCommand {
            public string CommandText { get; set; }

            public int CommandTimeout { get; set; }

            public CommandType CommandType { get; set; }

            public IDbConnection Connection { get; set; }

            private IDataParameterCollection _parameters = new DataParameterCollectionImpl();
            public IDataParameterCollection Parameters {
                get {
                    return _parameters;
                }
            }

            private IDbTransaction _tx;
            public IDbTransaction Transaction {
                get {
                    return _tx;
                }

                set {
                    _tx = value;
                }
            }

            public UpdateRowSource UpdatedRowSource {
                get {
                    throw new NotImplementedException();
                }

                set {
                    throw new NotImplementedException();
                }
            }

            private readonly IDbConnection _connection;
            public CommandImpl(IDbConnection connection) {
                _connection = connection;
            }

            public void Cancel() {
                throw new NotImplementedException();
            }

            public IDbDataParameter CreateParameter() {
                return new DataParameterImpl();
            }

            public void Dispose() {
                // 実装なし
            }

            public int ExecuteNonQuery() {
                return Parameters.Count;
            }

            public IDataReader ExecuteReader() {
                return new DataReaderImpl();
            }

            public IDataReader ExecuteReader(CommandBehavior behavior) {
                throw new NotImplementedException();
            }

            public object ExecuteScalar() {
                throw new NotImplementedException();
            }

            public void Prepare() {
                throw new NotImplementedException();
            }
        }

        private class DataParameterImpl : IDbDataParameter {
            public DbType DbType {
                get {
                    return DbType.Int32;
                }

                set {
                    throw new NotImplementedException();
                }
            }

            public ParameterDirection Direction {
                get {
                    return ParameterDirection.Input;
                }

                set {
                    throw new NotImplementedException();
                }
            }

            public bool IsNullable {
                get {
                    return false;
                }
            }

            public string ParameterName { get; set; }

            public byte Precision {
                get {
                    throw new NotImplementedException();
                }

                set {
                    throw new NotImplementedException();
                }
            }

            public byte Scale {
                get {
                    throw new NotImplementedException();
                }

                set {
                    throw new NotImplementedException();
                }
            }

            public int Size {
                get {
                    return default(int);
                }

                set {
                    throw new NotImplementedException();
                }
            }

            public string SourceColumn {
                get {
                    return "TestColumn";
                }

                set {
                    throw new NotImplementedException();
                }
            }

            public DataRowVersion SourceVersion {
                get {
                    throw new NotImplementedException();
                }

                set {
                    throw new NotImplementedException();
                }
            }

            public object Value { get; set; }
        }

        private class DataParameterCollectionImpl : IDataParameterCollection {
            private IList<object> _parameters = new List<object>();
            public object this[int index] {
                get {
                    return _parameters[index];
                }

                set {
                    _parameters[index] = value;
                }
            }

            public object this[string parameterName] {
                get {
                    return default(object);
                }

                set {
                    throw new NotImplementedException();
                }
            }

            public int Count {
                get {
                    return _parameters.Count;
                }
            }

            public bool IsFixedSize {
                get {
                    throw new NotImplementedException();
                }
            }

            public bool IsReadOnly {
                get {
                    throw new NotImplementedException();
                }
            }

            public bool IsSynchronized {
                get {
                    throw new NotImplementedException();
                }
            }

            public object SyncRoot {
                get {
                    throw new NotImplementedException();
                }
            }

            public int Add(object value) {
                _parameters.Add(value);
                return _parameters.Count();
            }

            public void Clear() {
                _parameters.Clear();
            }

            public bool Contains(object value) {
                throw new NotImplementedException();
            }

            public bool Contains(string parameterName) {
                throw new NotImplementedException();
            }

            public void CopyTo(Array array, int index) {
                throw new NotImplementedException();
            }

            public IEnumerator GetEnumerator() {
                return _parameters.GetEnumerator();
            }

            public int IndexOf(object value) {
                throw new NotImplementedException();
            }

            public int IndexOf(string parameterName) {
                throw new NotImplementedException();
            }

            public void Insert(int index, object value) {
                throw new NotImplementedException();
            }

            public void Remove(object value) {
                throw new NotImplementedException();
            }

            public void RemoveAt(int index) {
                throw new NotImplementedException();
            }

            public void RemoveAt(string parameterName) {
                throw new NotImplementedException();
            }
        }

        private class TestTx : IDbTransaction {
            private IDbConnection _connection;

            public TestTx(IDbConnection connection) {
                _connection = connection;
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

            public void Commit() {
                // 空実装
            }

            public void Dispose() {
                // 空実装
            }

            public void Rollback() {
                // 空実装
            }
        }
        #endregion
    }
}