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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using Quill.Attr;
using Quill.Message;
using QM = Quill.QuillManager;

namespace Quill.Ado {
    /// <summary>
    /// DbCommand生成デリゲート
    /// </summary>
    /// <param name="connection">コネクション</param>
    /// <param name="sql">SQL</param>
    /// <param name="sqlGenInvoker">実行SQL生成処理</param>
    /// <param name="createParamInvoker">パラメータ生成処理</param>
    /// <returns>生成したDbCommand</returns>
    public delegate IDbCommand DelegateCreateCommand(
        IDbConnection connection,
        string sql,
        Func<string, string> sqlGenInvoker,
        Action<int, string, IDataParameter> createParamInvoker);

    /// <summary>
    /// SQLユーティリティクラス
    /// </summary>
    public static class SqlUtils {
        /// <summary>
        /// SQL情報キャッシュ
        /// </summary>
        private static readonly ConcurrentDictionary<string, SqlProp> _cachedSqls
            = new ConcurrentDictionary<string, SqlProp>();

        /// <summary>
        /// エンティティへの変換
        /// </summary>
        /// <typeparam name="ENTITY_TYPE">エンティティ型</typeparam>
        /// <param name="reader">DB読み込みオブジェクト</param>
        /// <param name="entity">変換先のエンティティ</param>
        public static void TransToEntity<ENTITY_TYPE>(this IDataReader reader, ENTITY_TYPE entity) where ENTITY_TYPE : new() {
            var propInfos = entity.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
            if(propInfos.Count() == 0) {
                return;
            }

            // １レコード情報を列名-値のMap形式で保持
            var resultMap = new Dictionary<string, object>();
            for(int i = 0; i < reader.FieldCount; i++) {
                resultMap[reader.GetName(i)] = reader.GetValue(i);
            }

            // フィールド名と一致する項目に値設定
            foreach(var propInfo in propInfos) {
                var fieldName = GetPropertyName(propInfo);
                if(resultMap.ContainsKey(fieldName)) {
                    propInfo.SetValue(entity, resultMap[fieldName]);
                }
            }
        }

        /// <summary>
        /// 検索実行
        /// </summary>
        /// <typeparam name="ENTITY_TYPE">検索結果格納エンティティ</typeparam>
        /// <param name="connection">DB接続</param>
        /// <param name="sql">SQL</param>
        /// <param name="parameters">SQLパラメータ</param>
        /// <returns>検索結果リスト</returns>
        public static List<ENTITY_TYPE> Select<ENTITY_TYPE>(
            this IDbConnection connection, string sql, IDictionary<string, object> parameters = null)
            where ENTITY_TYPE : new() {

            return Select<ENTITY_TYPE>(connection, sql,SqlUtils.TransToEntity, parameters);
        }

        /// <summary>
        /// 検索実行
        /// </summary>
        /// <typeparam name="ENTITY_TYPE">検索結果格納エンティティ</typeparam>
        /// <param name="connection">DB接続</param>
        /// <param name="sql">SQL</param>
        /// <param name="setEntity">エンティティの設定</param>
        /// <param name="parameters">SQLパラメータ</param>
        /// <returns>検索結果リスト</returns>
        public static List<ENTITY_TYPE> Select<ENTITY_TYPE>(
            this IDbConnection connection, string sql, 
            Action<IDataReader, ENTITY_TYPE> setEntity, 
            IDictionary<string, object> parameters = null)
            where ENTITY_TYPE : new() {

            return Select(connection, () => sql, () => new ENTITY_TYPE(), setEntity,
                (index, paramName, dbParam) => {
                    object v = default(object);
                    if(parameters != null && parameters.ContainsKey(paramName)) {
                        v = parameters[paramName];
                    }
                    dbParam.Value = v;
                });
        }

        /// <summary>
        /// 検索実行
        /// </summary>
        /// <typeparam name="ENTITY_TYPE">検索結果格納エンティティ</typeparam>
        /// <param name="connection">DB接続</param>
        /// <param name="sqlGen">SQL生成</param>
        /// <param name="createEntity">エンティティのインスタンス生成</param>
        /// <param name="setEntity">エンティティの設定</param>
        /// <param name="setParameter">パラメータの設定</param>
        /// <param name="transaction">トランザクション</param>
        /// <returns>検索結果リスト</returns>
        public static List<ENTITY_TYPE> Select<ENTITY_TYPE>(
            this IDbConnection connection,
            Func<string> sqlGen,
            Func<ENTITY_TYPE> createEntity,
            Action<IDataReader, ENTITY_TYPE> setEntity,
            Action<int, string, IDataParameter> setParameter = null,
            IDbTransaction transaction = null) 
            where ENTITY_TYPE : new() {

            string sql = sqlGen();
            List<ENTITY_TYPE> results = new List<ENTITY_TYPE>();

            using(var command = CreateCommand(connection, sql, QM.ReplaceToParamMark, setParameter)) {
                if(transaction != null) {
                    command.Transaction = transaction;
                }

                using(var reader = command.ExecuteReader()) {
                    try {
                        while(reader.Read()) {
                            ENTITY_TYPE entity = createEntity();
                            setEntity(reader, entity);
                            results.Add(entity);
                        }
                    } finally {
                        if(!reader.IsClosed) {
                            reader.Close();
                        }
                    }
                }
            }
            return results;
        }

        /// <summary>
        /// 検索実行
        /// </summary>
        /// <typeparam name="ENTITY_TYPE">検索結果格納エンティティ</typeparam>
        /// <param name="transaction">トランザクション</param>
        /// <param name="sql">SQL</param>
        /// <param name="parameters">SQLパラメータ</param>
        /// <returns>検索結果リスト</returns>
        public static List<ENTITY_TYPE> Select<ENTITY_TYPE>(
            this IDbTransaction transaction, string sql, IDictionary<string, object> parameters = null)
            where ENTITY_TYPE : new() {

            return Select<ENTITY_TYPE>(transaction, sql, TransToEntity, parameters);
        }

        /// <summary>
        /// 検索実行
        /// </summary>
        /// <typeparam name="ENTITY_TYPE">検索結果格納エンティティ</typeparam>
        /// <param name="transaction">トランザクション</param>
        /// <param name="sql">SQL</param>
        /// <param name="setEntity">エンティティの設定</param>
        /// <param name="parameters">SQLパラメータ</param>
        /// <returns>検索結果リスト</returns>
        public static List<ENTITY_TYPE> Select<ENTITY_TYPE>(
            this IDbTransaction transaction, string sql,
            Action<IDataReader, ENTITY_TYPE> setEntity,
            IDictionary<string, object> parameters = null)
            where ENTITY_TYPE : new() {

            return Select(transaction, () => sql, () => new ENTITY_TYPE(), setEntity,
                (index, paramName, dbParam) => {
                    object v = default(object);
                    if(parameters != null && parameters.ContainsKey(paramName)) {
                        v = parameters[paramName];
                    }
                    dbParam.Value = v;
                });
        }

        /// <summary>
        /// 検索実行
        /// </summary>
        /// <typeparam name="ENTITY_TYPE">検索結果格納エンティティ</typeparam>
        /// <param name="transaction">トランザクション</param>
        /// <param name="sqlGen">SQL生成</param>
        /// <param name="createEntity">エンティティのインスタンス生成</param>
        /// <param name="setEntity">エンティティの設定</param>
        /// <param name="setParameter">パラメータの設定</param>
        /// <returns>検索結果リスト</returns>
        public static List<ENTITY_TYPE> Select<ENTITY_TYPE>(
            this IDbTransaction transaction,
            Func<string> sqlGen,
            Func<ENTITY_TYPE> createEntity,
            Action<IDataReader, ENTITY_TYPE> setEntity,
            Action<int, string, IDataParameter> setParameter = null)
            where ENTITY_TYPE : new() {

            return Select(transaction.Connection, sqlGen, createEntity, 
                setEntity, setParameter, transaction);
        }

        /// <summary>
        /// DB更新
        /// </summary>
        /// <param name="connection">DB接続</param>
        /// <param name="sql">SQL</param>
        /// <param name="setParameter">SQLパラメータ設定</param>
        /// <returns>更新件数</returns>
        public static int Update(this IDbConnection connection, string sql,
            Action<int, string, IDataParameter> setParameter = null) {

            return Update(connection, () => sql, setParameter);
        }

        /// <summary>
        /// DB更新
        /// </summary>
        /// <param name="connection">DB接続</param>
        /// <param name="sqlGen">SQL生成処理</param>
        /// <param name="setParameter">SQLパラメータ設定</param>
        /// <param name="transaction">トランザクション</param>
        /// <returns>更新件数</returns>
        public static int Update(this IDbConnection connection, Func<string> sqlGen,
            Action<int, string, IDataParameter> setParameter = null,
            IDbTransaction transaction = null) {

            string sql = sqlGen();
            int updateCount = 0;
            using(var command = CreateCommand(connection, sql, QM.ReplaceToParamMark, setParameter)) {
                if(transaction != null) {
                    command.Transaction = transaction;
                }
                updateCount = command.ExecuteNonQuery();
            }
            return updateCount;
        }

        /// <summary>
        /// DB更新
        /// </summary>
        /// <param name="connection">DB接続</param>
        /// <param name="sqlGen">SQL生成処理</param>
        /// <param name="parameters">更新パラメータ</param>
        /// <param name="transaction">トランザクション</param>
        /// <returns>更新件数</returns>
        public static int Update(this IDbConnection connection, Func<string> sqlGen,
            IDictionary<string, object> parameters,
            IDbTransaction transaction = null) {

            return Update(connection, sqlGen,
                (no, pName, dbParam) => dbParam.Value = parameters[pName],
                transaction);
        }

        /// <summary>
        /// DB更新
        /// </summary>
        /// <param name="transaction">トランザクション</param>
        /// <param name="sql">SQL</param>
        /// <param name="setParameter">SQLパラメータ設定</param>
        /// <returns>更新件数</returns>
        public static int Update(this IDbTransaction transaction, string sql,
            Action<int, string, IDataParameter> setParameter = null) {

            return Update(transaction, () => sql, setParameter);
        }

        /// <summary>
        /// DB更新
        /// </summary>
        /// <param name="transaction">トランザクション</param>
        /// <param name="sql">SQL</param>
        /// <param name="parameters">更新パラメータ</param>
        /// <returns>更新件数</returns>
        public static int Update(this IDbTransaction transaction, string sql,
            IDictionary<string, object> parameters) {

            return Update(transaction, () => sql, 
                (no, pName, dbParam) => dbParam.Value = parameters[pName]);
        }

        /// <summary>
        /// DB更新
        /// </summary>
        /// <param name="transaction">トランザクション</param>
        /// <param name="sqlGen">SQL生成処理</param>
        /// <param name="setParameter">SQLパラメータ設定</param>
        /// <returns>更新件数</returns>
        public static int Update(this IDbTransaction transaction, Func<string> sqlGen,
            Action<int, string, IDataParameter> setParameter = null) {

            return Update(transaction.Connection, sqlGen, setParameter, transaction);
        }

        /// <summary>
        /// DbCommandの生成
        /// </summary>
        /// <param name="connection">DB接続</param>
        /// <param name="sql">SQL</param>
        /// <param name="replaceToParamMark">パラメータ名からDB固有のSQLパラメータ名への変換処理</param>
        /// <param name="setParameter">パラメータ設定処理</param>
        /// <returns>生成したDbCommandインスタンス</returns>
        private static IDbCommand CreateCommand(
            IDbConnection connection, string sql, 
            Func<string, string> replaceToParamMark,
            Action<int, string, IDataParameter> setParameter) {

            var command = connection.CreateCommand();
            var sqlProp = _cachedSqls.GetOrAdd(sql, new SqlProp(sql, replaceToParamMark));

            command.CommandText = sqlProp.ActualSql;

            if(setParameter != null) {
                var parameterNames = sqlProp.ParameterNames;
                for(int i = 0; i < parameterNames.Count(); i++) {
                    var parameter = command.CreateParameter();
                    parameter.ParameterName = parameterNames[i].Mark;
                    setParameter(i, parameterNames[i].Name, parameter);
                    command.Parameters.Add(parameter);
                }
            }

            // 実行SQLをログ出力
            QM.OutputLog(typeof(SqlUtils), EnumMsgCategory.DEBUG,
                GetSqlLogString(sqlProp, command.Parameters));

            return command;
        }

        /// <summary>
        /// プロパティ名の取得
        /// </summary>
        /// <param name="propInfo">プロパティ情報</param>
        /// <returns>検索結果とマッピングするプロパティ名</returns>
        private static string GetPropertyName(PropertyInfo propInfo) {
            if(propInfo.IsDefined(typeof(ColumnAttribute))) {
                return propInfo.GetCustomAttribute<ColumnAttribute>().ColumnName;
            }
            return propInfo.Name;
        }

        /// <summary>
        /// SQLログの取得
        /// </summary>
        /// <param name="sqlProp"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        private static string GetSqlLogString(SqlProp sqlProp, IDataParameterCollection parameters) {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine();
            builder.AppendLine("SQL=[");
            builder.AppendLine(sqlProp.ActualSql);
            builder.AppendLine("]");
     
            if(parameters != null && parameters.Count > 0) {
                builder.AppendLine(" -> Parameters:{");
                foreach(IDbDataParameter parameter in parameters) {
                    builder.AppendFormat("        [{0} = {1}] ", parameter.ParameterName, parameter.Value);
                    builder.AppendLine();
                }
                builder.Append("    }");
            }
            return builder.ToString();
        }
    }
}
