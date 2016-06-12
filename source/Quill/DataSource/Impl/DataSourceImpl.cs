#region License
/*
 * Copyright 2015 Quill Users
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
#endregion

using System;
using System.Data;
using System.Threading;
using Quill.Util;

namespace Quill.DataSource.Impl {
    /// <summary>
    /// データソース実装クラス
    /// </summary>
    public class DataSourceImpl : IDataSource {
        /// <summary>
        /// コネクション
        /// </summary>
        private ThreadLocal<IDbConnection> _connection;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="connectionFactory">コネクション生成処理</param>
        public DataSourceImpl(Func<IDbConnection> connectionFactory) {
            _connection = new ThreadLocal<IDbConnection>(connectionFactory);
        }

        /// <summary>
        /// リソース解放
        /// </summary>
        public void Dispose() {
            if(_connection.IsValueCreated) {
                IDbConnection connection = _connection.Value;
                if(connection.State == ConnectionState.Open) {
                    connection.CloseConnection();
                }
                connection.Dispose();
            }
            _connection.Dispose();
        }

        /// <summary>
        /// コネクション取得
        /// </summary>
        /// <param name="dataSourceName">データソース名（省略可能）</param>
        /// <returns>コネクション</returns>
        public virtual IDbConnection GetConnection(string dataSourceName = null) {
            return _connection.Value;
        }
    }
}
