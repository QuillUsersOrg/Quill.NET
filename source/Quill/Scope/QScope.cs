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
using QM = Quill.QuillManager;

namespace Quill.Scope {
    /// <summary>
    /// Quillスコープ修飾クラス
    /// </summary>
    /// <typeparam name="DECORATOR_TYPE">修飾適用クラス</typeparam>
    public static class QScope<DECORATOR_TYPE> where DECORATOR_TYPE : class, IQuillDecorator {
        /// <summary>
        /// 特定の修飾処理を付与して実行
        /// </summary>
        /// <param name="action">QScopeで挟み込む処理</param>
        /// <param name="args">修飾処理内で引き継ぐ情報</param>
        public static void Execute(Action action,
            IDictionary<string, object> args = null) {

            var decorator = QM.Container.GetComponent<DECORATOR_TYPE>(withInjection: true);
            decorator.Decorate(action, args);
        }

        /// <summary>
        /// 特定の修飾処理を付与して実行
        /// </summary>
        /// <typeparam name="RETURN_TYPE">QScopeで挟み込む処理の戻り値型</typeparam>
        /// <param name="func">QScopeで挟み込む処理</param>
        /// <param name="args">修飾処理内で引き継ぐ情報</param>
        /// <returns>QScopeで挟み込む処理の戻り値</returns>
        public static RETURN_TYPE Execute<RETURN_TYPE>(Func<RETURN_TYPE> func,
            IDictionary<string, object> args = null) {
            
            var decorator = QM.Container.GetComponent<DECORATOR_TYPE>(withInjection: true);
            return decorator.Decorate(func, args);
        }
    }

    /// <summary>
    /// Quillスコープ修飾クラス
    /// </summary>
    /// <typeparam name="DECORATOR_TYPE">修飾適用クラス</typeparam>
    /// <typeparam name="PARAMETER_TYPE">QScopeで挟み込む処理に渡す引数型</typeparam>
    public static class QScope<DECORATOR_TYPE, PARAMETER_TYPE> where DECORATOR_TYPE : class, IQuillDecorator<PARAMETER_TYPE> {
        /// <summary>
        /// 特定の修飾処理を付与して実行
        /// </summary>
        /// <param name="action">QScopeで挟み込む処理</param>
        /// <param name="args">修飾処理内で引き継ぐ情報</param>
        public static void Execute(Action<PARAMETER_TYPE> action,
            IDictionary<string, object> args = null) {

            var decorator = QM.Container.GetComponent<DECORATOR_TYPE>(withInjection: true);
            decorator.Decorate(action, args);
        }

        /// <summary>
        /// 特定の修飾処理を付与して実行
        /// </summary>
        /// <typeparam name="RETURN_TYPE">QScopeで挟み込む処理の戻り値型</typeparam>
        /// <param name="func">QScopeで挟み込む処理</param>
        /// <param name="args">修飾処理内で引き継ぐ情報</param>
        /// <returns>QScopeで挟み込む処理の戻り値</returns>
        public static RETURN_TYPE Execute<RETURN_TYPE>(Func<PARAMETER_TYPE, RETURN_TYPE> func,
            IDictionary<string, object> args = null) {

            var decorator = QM.Container.GetComponent<DECORATOR_TYPE>(withInjection: true);
            return decorator.Decorate(func, args);
        }
    }
}
