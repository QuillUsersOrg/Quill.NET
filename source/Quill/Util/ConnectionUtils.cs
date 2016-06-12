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

using System.Data;
using Quill.Message;
using QM = Quill.QuillManager;

namespace Quill.Util {
    /// <summary>
    /// コネクションユーティリティクラス
    /// </summary>
    public static class ConnectionUtils {
        /// <summary>
        /// 接続開始
        /// </summary>
        /// <param name="connection">コネクション</param>
        public static void OpenConnection(this IDbConnection connection) {
            if(connection != null && connection.State != ConnectionState.Open) {
                connection.Open();
                QM.OutputLog(typeof(ConnectionUtils), EnumMsgCategory.DEBUG, QMsg.ConnectionOpened.Get());
            }
        }

        /// <summary>
        /// 接続終了
        /// </summary>
        /// <param name="connection">コネクション</param>
        public static void CloseConnection(this IDbConnection connection) {
            if(connection != null && connection.State == ConnectionState.Open) {
                connection.Close();
                QM.OutputLog(typeof(ConnectionUtils), EnumMsgCategory.DEBUG, QMsg.ConnectionClosed.Get());
            }
        }
    }
}
