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
using System.Reflection;

namespace Quill.Inject {
    /// <summary>
    /// Injectionフィルターインターフェース
    /// </summary>
    public interface IInjectionFilter : IDisposable {
        /// <summary>
        /// Injection対象とするフィールドの抽出条件取得
        /// </summary>
        /// <returns>Injection対象とするフィールドの抽出条件</returns>
        BindingFlags GetTargetFieldBindinFlags();

        /// <summary>
        /// Injection対象の型か判定
        /// </summary>
        /// <param name="componentType">判定対象の型</param>
        /// <returns>true:Injection対象, false:非対象</returns>
        bool IsTargetType(Type componentType);

        /// <summary>
        /// Injection対象のフィールドか判定
        /// </summary>
        /// <param name="componentType">判定対象の型</param>
        /// <param name="fieldInfo">フィールド情報</param>
        /// <returns></returns>
        bool IsTargetField(Type componentType, FieldInfo fieldInfo);
    }
}
