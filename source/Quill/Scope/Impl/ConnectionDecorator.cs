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
using System.Collections.Generic;
using System.Data;
using Quill.Consts;
using Quill.DataSource;
using Quill.DataSource.Impl;
using Quill.Message;
using Quill.Util;
using QM = Quill.QuillManager;

namespace Quill.Scope.Impl {
    /// <summary>
    /// コネクション接続修飾クラス
    /// </summary>
    public class ConnectionDecorator : IQuillDecorator<IDbConnection> {
        private IDataSource _dataSource;

        /// <summary>
        /// データソース
        /// </summary>
        public virtual IDataSource DataSource {
            get { return _dataSource; }
            set { _dataSource = value; }
        }

        /// <summary>
        /// 複数データソース対応フラグ
        /// </summary>
        public virtual bool IsMultiDataSource {
            get { return (DataSource != null && DataSource is MultiDataSource); }
        }

        /// <summary>
        /// 前後をDB接続の開始、終了で挟んで実行
        /// </summary>
        /// <param name="action">本処理</param>
        /// <param name="args">修飾処理内で引き継ぐ情報</param>
        public virtual void Decorate(Action<IDbConnection> action,
            IDictionary<string, object> args = null) {

            string dataSourceName = GetDataSourceName(action, args);
            Tuple<IDbConnection, bool> connection = GetOpenedConnection(dataSourceName);

            try {
                action(connection.Item1);
            } finally {
                CloseConnection(connection.Item1, connection.Item2);
            }
        }

        /// <summary>
        /// 前後をDB接続の開始、終了で挟んで実行
        /// </summary>
        /// <typeparam name="RETURN_TYPE">本処理の戻り値型</typeparam>
        /// <param name="func">本処理</param>
        /// <param name="args">修飾処理内で引き継ぐ情報</param>
        /// <returns>本処理の戻り値</returns>
        public virtual RETURN_TYPE Decorate<RETURN_TYPE>(Func<IDbConnection, RETURN_TYPE> func,
            IDictionary<string, object> args = null) {

            string dataSourceName = GetDataSourceName(func, args);
            Tuple<IDbConnection, bool> connection = GetOpenedConnection(dataSourceName);

            try {
                return func(connection.Item1);
            } finally {
                CloseConnection(connection.Item1, connection.Item2);
            }
        }

        /// <summary>
        /// メソッド情報の取得
        /// </summary>
        /// <param name="invoker">委譲処理</param>
        /// <param name="args">修飾処理内で引き継ぐ情報</param>
        /// <returns>メソッド情報</returns>
        protected virtual string GetDataSourceName(Delegate invoker,
            IDictionary<string, object> args) {
            var dataSourceName = string.Empty;
            if(IsMultiDataSource) {
                // 複数データソース対応時のみメソッド情報を取り出す
                dataSourceName = (args != null && args.ContainsKey(QuillKey.DATA_SOURCE_NAME)) ?
                    (string)args[QuillKey.DATA_SOURCE_NAME] : string.Empty;

                QM.OutputLog(GetType(), EnumMsgCategory.INFO,
                    string.Format("Target:{0}.GetDataSourceName, DataSourceName:{1}", 
                        GetType().Name, dataSourceName));
            }
            return dataSourceName;
        }

        /// <summary>
        /// Open状態のコネクションを取得
        /// </summary>
        /// <param name="dataSourceName">データソース名</param>
        /// <returns>コネクション、接続開始処理フラグ</returns>
        protected virtual Tuple<IDbConnection, bool> GetOpenedConnection(string dataSourceName) {
            IDbConnection connection = DataSource.GetConnection(dataSourceName);
            bool isOpener = false;
            if(connection.State != ConnectionState.Open) {
                connection.OpenConnection();
                isOpener = true;
            }
            return new Tuple<IDbConnection, bool>(connection, isOpener);
        }

        /// <summary>
        /// DB接続終了
        /// </summary>
        /// <param name="connection">コネクション</param>
        /// <param name="isOpener">接続開始フラグ</param>
        protected virtual void CloseConnection(IDbConnection connection, bool isOpener) {
            if(isOpener) {
                connection.CloseConnection();
            }
        }
    }
}
