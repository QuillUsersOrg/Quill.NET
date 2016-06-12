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

namespace Quill.Scope {
    /// <summary>
    /// Quill修飾インターフェース
    /// </summary>
    public interface IQuillDecorator {
        /// <summary>
        /// 前処理、後処理で挟んで実行
        /// </summary>
        /// <param name="action">本処理</param>
        /// <param name="args">修飾処理内で引き継ぐ情報</param>
        void Decorate(Action action, IDictionary<string, object> args = null);

        /// <summary>
        /// 前処理、後処理で挟んで実行
        /// </summary>
        /// <typeparam name="RETURN_TYPE">本処理の戻り値型</typeparam>
        /// <param name="func">本処理</param>
        /// <param name="args">修飾処理内で引き継ぐ情報</param>
        /// <returns>本処理の戻り値</returns>
        RETURN_TYPE Decorate<RETURN_TYPE>(Func<RETURN_TYPE> func, 
            IDictionary<string, object> args = null);
    }

    /// <summary>
    /// Quill修飾インターフェース
    /// </summary>
    public interface IQuillDecorator<PARAMETER_TYPE> {
        /// <summary>
        /// 前処理、後処理で挟んで実行
        /// </summary>
        /// <param name="action">本処理</param>
        /// <param name="args">修飾処理内で引き継ぐ情報</param>
        void Decorate(Action<PARAMETER_TYPE> action, IDictionary<string, object> args = null);

        /// <summary>
        /// 前処理、後処理で挟んで実行
        /// </summary>
        /// <typeparam name="RETURN_TYPE">本処理の戻り値型</typeparam>
        /// <param name="func">本処理</param>
        /// <param name="args">修飾処理内で引き継ぐ情報</param>
        /// <returns>本処理の戻り値</returns>
        RETURN_TYPE Decorate<RETURN_TYPE>(Func<PARAMETER_TYPE, RETURN_TYPE> func, 
            IDictionary<string, object> args = null);
    }
}
