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
using System.Text.RegularExpressions;

namespace Quill.Ado {
    /// <summary>
    /// パラメータ名構造体
    /// </summary>
    public struct ParameterNameStruct {
        /// <summary>パラメータ名</summary>
        public string Name;
        /// <summary>パラメータ置換マーク</summary>
        public string Mark;
    };

    /// <summary>
    /// SQL簡易解析結果
    /// </summary>
    public class SqlProp {
        private const string REG_EXTRACT_PARAMS = "/\\*(.+?)\\*/[^\\s,]*";

        /// <summary>
        /// 実行時SQL
        /// </summary>
        public string ActualSql { get; private set; }

        /// <summary>
        /// パラメータ名一覧
        /// </summary>
        public ParameterNameStruct[] ParameterNames { get; private set; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="sql">SQL</param>
        /// <param name="replaceToParamMark">パラメータ名からSQLパラメータ名への変換処理</param>
        public SqlProp(string sql, Func<string, string> replaceToParamMark) {
            var extractedSqlStruct = ReadyExecuteSQL(sql, replaceToParamMark);
            ActualSql = extractedSqlStruct.Sql;
            ParameterNames = extractedSqlStruct.ParameterNames;
        }

        /// <summary>
        /// SQL実行準備
        /// </summary>
        /// <param name="baseSql">元のSQL</param>
        /// <param name="replaceToParamMark">パラメータ名からSQLパラメータ名への変換処理</param>
        /// <returns></returns>
        private SqlStruct ReadyExecuteSQL(string baseSql, Func<string, string> replaceToParamMark) {
            string sql = baseSql;
            var matches = Regex.Matches(sql, REG_EXTRACT_PARAMS);

            var parameterNames = new List<ParameterNameStruct>();
            for(int i = 0; i < matches.Count; i++) {
                Match match = matches[i];

                var paramNameStruct = new ParameterNameStruct();
                paramNameStruct.Name = match.Groups[1].Value.Trim();
                paramNameStruct.Mark = replaceToParamMark(paramNameStruct.Name);
                parameterNames.Add(paramNameStruct);

                sql = sql.Replace(match.Value, paramNameStruct.Mark);
            }
            var sqlStruct = new SqlStruct();
            sqlStruct.Sql = sql;
            sqlStruct.ParameterNames = parameterNames.ToArray();
            return sqlStruct;
        }

        /// <summary>
        /// SQL実行情報構造体
        /// </summary>
        private struct SqlStruct {
            /// <summary>実行用SQL</summary>
            public string Sql;

            /// <summary>パラメータ名(SQL内、パラメータ置換文字列)</summary>
            public ParameterNameStruct[] ParameterNames;
        };
    }
}
