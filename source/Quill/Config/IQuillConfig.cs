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
using System.Xml.Linq;

namespace Quill.Config {
    /// <summary>
    /// Quill設定インターフェース
    /// </summary>
    public interface IQuillConfig : IDisposable {
        /// <summary>
        /// 基底のXML要素
        /// </summary>
        XElement BaseElement { get; }

        /// <summary>
        /// XML値取得
        /// </summary>
        /// <param name="nodePath">ノードのフルパス(XXX.XXX.XXX....)</param>
        /// <returns>XML値</returns>
        string GetValue(string nodePath);

        /// <summary>
        /// XML値取得
        /// </summary>
        /// <param name="nodePath">ノードのフルパス(XXX.XXX.XXX....)</param>
        /// <param name="isTarget">読み取り対象判定処理</param>
        /// <returns>XML値</returns>
        string GetValue(string nodePath, Func<XElement, bool> isTarget);

        /// <summary>
        /// XML値取得（リスト）
        /// </summary>
        /// <param name="nodePath">ノードのフルパス(XXX.XXX.XXX....)</param>
        /// <returns>XML値（リスト）</returns>
        List<string> GetValues(string nodePath);

        /// <summary>
        /// XML値取得（リスト）
        /// </summary>
        /// <param name="nodePath">ノードのフルパス(XXX.XXX.XXX....)</param>
        /// <param name="isTarget">読み取り対象判定処理</param>
        /// <returns>XML値（リスト）</returns>
        List<string> GetValues(string nodePath, Func<XElement, bool> isTarget);

        /// <summary>
        /// XML要素の取得
        /// </summary>
        /// <param name="nodePath">ノードのフルパス(XXX.XXX.XXX....)</param>
        /// <returns>XML要素</returns>
        XElement GetElement(string nodePath);

        /// <summary>
        /// XML要素の取得
        /// </summary>
        /// <param name="nodePath">ノードのフルパス(XXX.XXX.XXX....)</param>
        /// <param name="isTarget">読み取り対象判定処理</param>
        /// <returns>XML要素</returns>
        XElement GetElement(string nodePath, Func<XElement, bool> isTarget);

        /// <summary>
        /// XML要素の取得（リスト）
        /// </summary>
        /// <param name="nodePath">ノードのフルパス(XXX.XXX.XXX....)</param>
        /// <returns>XML要素（リスト）</returns>
        List<XElement> GetElements(string nodePath);

        /// <summary>
        /// XML要素の取得（リスト）
        /// </summary>
        /// <param name="nodePath">ノードのフルパス(XXX.XXX.XXX....)</param>
        /// <param name="isTarget">読み取り対象判定処理</param>
        /// <returns>XML要素（リスト）</returns>
        List<XElement> GetElements(string nodePath, Func<XElement, bool> isTarget);
    }
}
