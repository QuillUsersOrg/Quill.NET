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

namespace Quill.Container.Impl {
    /// <summary>
    /// 戻り値型ーコンポーネント型紐づけクラス
    /// </summary>
    public class TypeMapImpl : ITypeMap {
        private readonly IDictionary<Type, Type> _typeMap;

        /// <summary>
        /// コンストラクタ（内部で保持しているDictionaryを初期化
        /// </summary>
        public TypeMapImpl() {
            _typeMap = CreateDictionary();
        }

        /// <summary>
        /// 戻り値型ーコンポーネント型組み合わせの追加
        /// </summary>
        /// <param name="receiptType">戻り値型</param>
        /// <param name="componentType">コンポーネント型</param>
        public virtual void Add(Type receiptType, Type componentType) {
            _typeMap[receiptType] = componentType;
        }

        /// <summary>
        /// リソースの解放
        /// </summary>
        public virtual void Dispose() {
            _typeMap.Clear();
        }

        /// <summary>
        /// コンポーネント型の取得（引数の型をそのまま返す）
        /// </summary>
        /// <param name="receiptType">戻り値として返す型</param>
        /// <returns>コンポーネント型（紐づく型がなければreceiptTypeをそのまま返す</returns>
        public virtual Type GetComponentType(Type receiptType) {
            if(_typeMap.ContainsKey(receiptType)) {
                return _typeMap[receiptType];
            }
            return receiptType;
        }

        /// <summary>
        /// コンポーネントの受け取り側の型とコンテナ上で管理している型が紐づけられているか判定
        /// </summary>
        /// <param name="receipedType">コンポーネントを受け取る型</param>
        /// <returns>true:紐づけあり, false:紐づけなし</returns>
        public virtual bool IsMapped(Type receipedType) {
            return _typeMap.ContainsKey(receipedType);
        }

        /// <summary>
        /// 戻り値型ーコンポーネント型紐づけDictionaryの生成
        /// </summary>
        /// <returns>戻り値型ーコンポーネント型紐づけDictionary</returns>
        protected virtual IDictionary<Type, Type> CreateDictionary() {
            return new Dictionary<Type, Type>();
        }
    }
}
