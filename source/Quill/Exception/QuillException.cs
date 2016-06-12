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

using Quill.Message;
using QM = Quill.QuillManager;

namespace Quill.Exception {
    /// <summary>
    /// Quill内部で発生する例外クラス
    /// </summary>
    public class QuillException : System.Exception {
        /// <summary>
        /// 例外メッセージを指定してインスタンス生成（ログ出力付き）
        /// </summary>
        /// <param name="message">エラーメッセージ</param>
        public QuillException(string message) : base(message) {
            QM.OutputLog(GetType(), EnumMsgCategory.ERROR, message);
        }

        /// <summary>
        /// 例外メッセージ、内部例外を指定してインスタンス生成（ログ出力付き）
        /// </summary>
        /// <param name="message">エラーメッセージ</param>
        /// <param name="innerException">内部例外</param>
        public QuillException(string message, System.Exception innerException)
            : base(message, innerException) {
            QM.OutputLog(GetType(), EnumMsgCategory.ERROR, message);
        }
    }
}
