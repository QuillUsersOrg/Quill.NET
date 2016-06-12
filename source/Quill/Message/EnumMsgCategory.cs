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
    /// メッセージカテゴリ列挙体
    /// </summary>
    public enum EnumMsgCategory {
        /// <summary>デバッグ</summary>
        DEBUG = 0,

        /// <summary>運用参考情報</summary>
        INFO,

        /// <summary>警告</summary>
        WARN,

        /// <summary>エラー</summary>
        ERROR
    };

    /// <summary>
    /// メッセージカテゴリ列挙体ユーティリティクラス
    /// </summary>
    public static class EnumMsgCategoryUtils {
        /// <summary>
        /// カテゴリ名を取得
        /// </summary>
        /// <param name="category">メッセージカテゴリ</param>
        /// <returns>メッセージカテゴリ名</returns>
        public static string GetCategoryName(this EnumMsgCategory category) {
            switch(category) {
                case EnumMsgCategory.INFO:
                    return "INFO";
                case EnumMsgCategory.WARN:
                    return "INFO";
                case EnumMsgCategory.ERROR:
                    return "ERROR";
                default:
                    return "DEBUG";
            }
        }
    }
}
