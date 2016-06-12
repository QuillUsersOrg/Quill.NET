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

using System.Collections.Generic;

namespace Quill.Message {
    /// <summary>
    /// Quillメッセージコード列挙体
    /// </summary>
    public enum QMsg {
        /// <summary>インスタンス生成不可</summary>
        NotAssignable = 0,
        /// <summary>Injection非対象</summary>
        NotInjectionTargetType,
        /// <summary>既にInjectionされている</summary>
        AlreadyInjected,
        /// <summary>引数にnullが渡されている</summary>
        ArgumentNull,
        /// <summary>ファイルが見つからない</summary>
        FileNotFound,
        /// <summary>設定読み込みエラー</summary>
        ErrorLoadingConfig,
        /// <summary>必須セクションが見つからない</summary>
        NotFoundRequireSection,
        /// <summary>ノードパスが見つからない</summary>
        NotFoundNodePath,
        /// <summary>Quillで使用できないConstructor定義</summary>
        IllegalConstructor,

        /// <summary>DB接続修飾クラスが見つからない</summary>
        NotFoundDBConnectionDecorator,
        /// <summary>DBコネクションのインスタンス未設定</summary>
        NoDbConnectionInstance,
        /// <summary>コネクションが開かれた</summary>
        ConnectionOpened,
        /// <summary>コネクションが閉じられた</summary>
        ConnectionClosed,
        /// <summary>コネクションが破棄された</summary>
        ConnectionDisposed,
        /// <summary>トランザクション開始</summary>
        BeginTx,
        /// <summary>トランザクション終了（コミット）</summary>
        Committed,
        /// <summary>トランザクション終了（ロールバック）</summary>
        Rollbacked,
        /// <summary>データソース未登録</summary>
        NotRegisteredDataSource,
        /// <summary>型が見つからない</summary>
        TypeNotFound,
    };

    /// <summary>
    /// Quillメッセージクラス
    /// </summary>
    public class QuillMessage {
        #region メッセージ保持、基本処理

        /// <summary>
        /// メッセージコード―メッセージ紐づけMap
        /// </summary>
        private readonly IDictionary<QMsg, string> _messageMap;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="messageMap">メッセージコード―メッセージ紐づけMap</param>
        public QuillMessage(IDictionary<QMsg, string> messageMap) {
            _messageMap = messageMap;
        }

        /// <summary>
        /// メッセージの取得
        /// </summary>
        /// <param name="messageCode">メッセージコード</param>
        /// <returns>メッセージ</returns>
        public virtual string GetMessage(QMsg messageCode) {
            if(_messageMap.ContainsKey(messageCode)) {
                return _messageMap[messageCode];
            }
            return "[Unknown Message]";
        }
        #endregion

        /// <summary>
        /// Quillメッセージ（日本語）の取得
        /// </summary>
        /// <returns>メッセージ</returns>
        public static QuillMessage CreateForJPN() {
            var msgMap = new Dictionary<QMsg, string>();
            msgMap[QMsg.NotAssignable] = "引数の型を戻り値の型に設定することはできません。";
            msgMap[QMsg.NotInjectionTargetType] = "インジェクション対象の型ではありません。";
            msgMap[QMsg.AlreadyInjected] = "既にインジェクション済です。";
            msgMap[QMsg.ArgumentNull] = "Null入力は禁止です。";
            msgMap[QMsg.FileNotFound] = "ファイルが見つかりません。";
            msgMap[QMsg.ErrorLoadingConfig] = "設定ファイルの読み込みに失敗しました。";
            msgMap[QMsg.NotFoundRequireSection] = "必須セクションが定義されていません。";
            msgMap[QMsg.NotFoundNodePath] = "指定されたノードパスが見つかりません。";
            msgMap[QMsg.IllegalConstructor] = "Quillで使用可能なコンストラクタがありません。";
            msgMap[QMsg.NotFoundDBConnectionDecorator] = "DB接続修飾オブジェクトが設定されていません。";
            msgMap[QMsg.NoDbConnectionInstance] = "DB接続オブジェクトが設定されていません";
            msgMap[QMsg.ConnectionOpened] = "DB接続を開始しました。";
            msgMap[QMsg.ConnectionClosed] = "DB接続を終了しました。";
            msgMap[QMsg.ConnectionDisposed] = "DB接続のリソースを解放しました。";
            msgMap[QMsg.BeginTx] = "トランザクションを開始しました。";
            msgMap[QMsg.Committed] = "コミットしました。";
            msgMap[QMsg.Rollbacked] = "ロールバックしました。";
            msgMap[QMsg.NotRegisteredDataSource] = "データソースが登録されていません。";
            msgMap[QMsg.TypeNotFound] = "型情報が見つかりません。";

            return new QuillMessage(msgMap);
        }
    }
}
