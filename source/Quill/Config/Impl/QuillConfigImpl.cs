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
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Quill.Exception;
using Quill.Message;
using QM = Quill.QuillManager;

namespace Quill.Config.Impl {
    /// <summary>
    /// Quill設定実装クラス
    /// </summary>
    public class QuillConfigImpl : IQuillConfig {
        /// <summary>
        /// 基底設定セクション名
        /// </summary>
        protected const string BASE_SECTION_NAME = "quill";

        /// <summary>
        /// 基底設定セクション
        /// </summary>
        public virtual XElement BaseElement { get; protected set; }

        /// <summary>
        /// コンストラクタ（内部からのみ呼び出し可能）
        /// </summary>
        protected QuillConfigImpl(XElement baseElement) {
            BaseElement = baseElement;
        }

        /// <summary>
        /// 設定の読み込み
        /// </summary>
        /// <param name="path">設定ファイルパス</param>
        /// <returns>Quill設定情報</returns>
        public static IQuillConfig Load(string path) {
            if(path == null) {
                throw new ArgumentNullException("path");
            }

            if(!File.Exists(path)) {
                throw new FileNotFoundException(
                    string.Format("{0} path={1}", QMsg.FileNotFound.Get(), path));
            }

            try {
                XDocument doc = XDocument.Load(path);
                return new QuillConfigImpl(GetBaseElement(doc, path));
            } catch(System.Exception ex) {
                throw new QuillException(
                    string.Format("{0}, path={1}", QMsg.ErrorLoadingConfig.Get(), path), 
                    ex);
            }
        }

        /// <summary>
        /// ノードパスに該当する設定値を取得
        /// </summary>
        /// <param name="nodePath">階層を"."で区切ったノード階層パス（XXX.XXX.XXX）</param>
        /// <returns>設定値</returns>
        public virtual string GetValue(string nodePath) {
            return GetValue(nodePath, e => true);
        }

        /// <summary>
        /// ノードパスに該当する設定値を取得
        /// </summary>
        /// <param name="nodePath">階層を"."で区切ったノード階層パス（XXX.XXX.XXX）</param>
        /// <param name="isTarget">ノードパス以外のノード検索条件</param>
        /// <returns>設定値</returns>
        public virtual string GetValue(string nodePath, Func<XElement, bool> isTarget) {
            return BaseElement.GetChildValue(nodePath, isTarget);
        }

        /// <summary>
        /// ノードパスに該当する設定値を取得（複数）
        /// </summary>
        /// <param name="nodePath">階層を"."で区切ったノード階層パス（XXX.XXX.XXX）</param>
        /// <returns>設定値</returns>
        public virtual List<string> GetValues(string nodePath) {
            return GetValues(nodePath, e => true);
        }

        /// <summary>
        /// ノードパスに該当する設定値を取得（複数）
        /// </summary>
        /// <param name="nodePath">階層を"."で区切ったノード階層パス（XXX.XXX.XXX）</param>
        /// <param name="isTarget">ノードパス以外のノード検索条件</param>
        /// <returns>設定値</returns>
        public virtual List<string> GetValues(string nodePath, Func<XElement, bool> isTarget) {
            return BaseElement.GetChildValues(nodePath, isTarget);
        }

        /// <summary>
        /// ノードパスに該当するノード情報を取得
        /// </summary>
        /// <param name="nodePath">階層を"."で区切ったノード階層パス（XXX.XXX.XXX）</param>
        /// <returns>ノード情報</returns>
        public virtual XElement GetElement(string nodePath) {
            return GetElement(nodePath, e => true);
        }

        /// <summary>
        /// ノードパスに該当するノード情報を取得
        /// </summary>
        /// <param name="nodePath">階層を"."で区切ったノード階層パス（XXX.XXX.XXX）</param>
        /// <param name="isTarget">ノードパス以外のノード検索条件</param>
        /// <returns>ノード情報</returns>
        public virtual XElement GetElement(string nodePath, Func<XElement, bool> isTarget) {
            return BaseElement.GetChildElement(nodePath, isTarget);
        }

        /// <summary>
        /// ノードパスに該当するノード情報を取得（複数）
        /// </summary>
        /// <param name="nodePath">階層を"."で区切ったノード階層パス（XXX.XXX.XXX）</param>
        /// <returns>ノード情報</returns>
        public virtual List<XElement> GetElements(string nodePath) {
            return GetElements(nodePath, e => true);
        }

        /// <summary>
        /// ノードパスに該当するノード情報を取得（複数）
        /// </summary>
        /// <param name="nodePath">階層を"."で区切ったノード階層パス（XXX.XXX.XXX）</param>
        /// <param name="isTarget">ノードパス以外のノード検索条件</param>
        /// <returns>ノード情報</returns>
        public virtual List<XElement> GetElements(string nodePath, Func<XElement, bool> isTarget) {
            return BaseElement.GetChildElements(nodePath, isTarget);
        }

        /// <summary>
        /// リソースの解放
        /// </summary>
        public virtual void Dispose() {
            BaseElement = null;
        }

        /// <summary>
        /// Quill設定の最上位ノードを取得
        /// </summary>
        /// <param name="doc">Quill設定XMLドキュメント</param>
        /// <param name="path">設定ファイルパス</param>
        /// <returns>Quill設定の最上位ノード</returns>
        private static XElement GetBaseElement(XDocument doc, string path) {
            IEnumerable<XElement> quillElements = doc.Elements(BASE_SECTION_NAME);
            if(quillElements == null || quillElements.Count() == 0) {
                throw new QuillException(string.Format("{0} section={1}, path={2}",
                    QMsg.NotFoundRequireSection.Get(),
                    BASE_SECTION_NAME, path));
            }

            return quillElements.First();
        }
    }
}
