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

namespace Quill.Container {
    /// <summary>
    /// コンポーネントの受け取り側の型とコンテナ上で管理している型の紐づけインターフェース
    /// </summary>
    public interface ITypeMap : IDisposable {
        /// <summary>
        /// 紐づける型の追加
        /// </summary>
        /// <param name="receiptType">受け取り型</param>
        /// <param name="implType">実装型</param>
        void Add(Type receiptType, Type implType);

        /// <summary>
        /// コンポーネント型の取得（引数の型をそのまま返す）
        /// </summary>
        /// <param name="receiptType">戻り値として返す型</param>
        /// <returns>コンポーネント型（紐づく型がなければreceiptTypeをそのまま返す</returns>
        Type GetComponentType(Type receiptType);

        /// <summary>
        /// コンポーネントの受け取り側の型とコンテナ上で管理している型が紐づけられているか判定
        /// </summary>
        /// <param name="receipedType">コンポーネントを受け取る型</param>
        /// <returns>true:紐づけあり, false:紐づけなし</returns>
        bool IsMapped(Type receipedType);
    }
}
