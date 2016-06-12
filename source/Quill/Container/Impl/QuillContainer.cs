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
using System.Collections.Concurrent;
using System.Collections.Generic;
using Quill.Exception;
using Quill.Message;
using QM = Quill.QuillManager;

namespace Quill.Container.Impl {

    /// <summary>.
    /// Quillコンテナ実装クラス
    /// </summary>
    public class QuillContainer : IDisposable {
        /// <summary>
        /// 生成済コンポーネントキャッシュ
        /// </summary>
        private readonly IDictionary<Type, object> _components;

        /// <summary>
        /// QuillContainer初期化
        /// </summary>
        public QuillContainer(IDictionary<Type, object> container = null) {
            _components = (container == null ? CreateContainer() : container);
        }

        /// <summary>
        /// 保持リソースの解放
        /// </summary>
        public virtual void Dispose() {
            _components.Clear();
        }

        /// <summary>
        /// コンポーネントの取得
        /// </summary>
        /// <typeparam name="T">コンポーネント型</typeparam>
        /// <param name="isCache">true:キャッシュする, false:毎回インスタンス新規生成</param>
        /// <param name="withInjection">true:インジェクション済のコンポーネント取得, false:インジェクションなし</param>
        /// <returns>コンポーネントのインスタンス</returns>
        public virtual T GetComponent<T>(bool isCache = true, bool withInjection = false)
            where T : class {

            Type componentType = GetComponentType(typeof(T));
            return (T)GetComponent(componentType, isCache, withInjection);
        }

        /// <summary>
        /// コンポーネントの取得
        /// </summary>
        /// <param name="componentType">コンポーネントの型</param>
        /// <param name="isCache">true:キャッシュする, false:毎回インスタンス新規生成</param>
        /// <param name="withInjection">true:インジェクション済のコンポーネント取得, false:インジェクションなし</param>
        /// <returns>コンポーネントのインスタンス</returns>
        public virtual object GetComponent(Type componentType, bool isCache = true, bool withInjection = false) {
            if(isCache) {
                QM.OutputLog(GetType(), EnumMsgCategory.DEBUG, 
                    string.Format("CacheType:{0}", _components.GetType().Name));

                if(_components is ConcurrentDictionary<Type, object>) {
                    var components = (ConcurrentDictionary<Type, object>)_components;
                    return components.GetOrAdd(componentType, t => CreateComponent(t, withInjection));
                }

                if(!_components.ContainsKey(componentType)) {
                    _components.Add(componentType, CreateComponent(componentType, withInjection));
                }
                return _components[componentType];
            }
            return CreateComponent(componentType, withInjection);
        }

        /// <summary>
        /// 並列でインジェクション可能か判定
        /// </summary>
        /// <returns>true:可, false:不可</returns>
        public virtual bool CanParallelInjection() {
            return _components is ConcurrentDictionary<Type, object>;
        }

        /// <summary>
        /// コンポーネント生成
        /// </summary>
        /// <param name="componentType">コンポーネントの型</param>
        /// <param name="withInjection">true:Injection済のコンポーネントを返す, false:Injectionなし</param>
        /// <returns>生成したコンポーネント</returns>
        protected virtual object CreateComponent(Type componentType, bool withInjection) {
            object component = QM.ComponentCreator.Create(componentType);
            if(withInjection) {
                QM.Injector.Inject(component);
            }
            return component;
        }

        /// <summary>
        /// 生成済コンポーネントキャッシュの生成
        /// </summary>
        /// <returns>生成済コンポーネントキャッシュ</returns>
        protected virtual IDictionary<Type, object> CreateContainer() {
            // スレッドセーフなDictionary
            return new ConcurrentDictionary<Type, object>();
        }

        /// <summary>
        /// コンポーネント型の取得
        /// </summary>
        /// <param name="receiptType">戻り値型</param>
        /// <returns>生成したコンポーネント</returns>
        protected virtual Type GetComponentType(Type receiptType) {
            Type componentType = receiptType;
            if(QM.TypeMap.IsMapped(receiptType)) {
                componentType = QM.TypeMap.GetComponentType(receiptType);
            }

            if(!receiptType.IsAssignableFrom(componentType)) {
                throw new QuillException(string.Format("{0} receiptType=[{1}], componentType=[{2}]",
                    QMsg.NotAssignable.Get(), receiptType, componentType));
            }
            return componentType;
        }
    }
}
