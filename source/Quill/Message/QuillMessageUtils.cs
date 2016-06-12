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

namespace Quill.Message {
    /// <summary>
    /// Quillメッセージユーティリティクラス
    /// </summary>
    public static class QuillMessageUtils {
        /// <summary>
        /// メッセージの取得
        /// </summary>
        /// <param name="messageCode">メッセージコード</param>
        /// <returns>メッセージ</returns>
        public static string Get(this QMsg messageCode) {
            return QuillManager.Message.GetMessage(messageCode);
        }
    }
}
