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
using System.Data;

namespace Quill.Container.Impl {
    /// <summary>
    /// DBコネクション生成抽象クラス
    /// </summary>
    public abstract class AbstractConnectionCreator : IComponentCreator {
        /// <summary>
        /// インスタンス生成
        /// </summary>
        /// <param name="componentType">コンポーネント型(NotNull)</param>
        /// <returns>生成したインスタンス</returns>
        public virtual object Create(Type componentType) {
            if(componentType == null) {
                throw new ArgumentNullException("componentType");
            }

            if(!typeof(IDbConnection).IsAssignableFrom(componentType)) {
                throw new ArgumentException(componentType.FullName, "componentType");
            }

            return CreateConnection(componentType);
        }

        /// <summary>
        /// リソースの解放
        /// </summary>
        public virtual void Dispose() {
            // 保持リソースがないため、実装なし
        }

        /// <summary>
        /// コネクションインスタンスの生成
        /// </summary>
        /// <param name="connectionType">コネクション実装型</param>
        /// <returns>生成したコネクションインスタンス</returns>
        protected abstract IDbConnection CreateConnection(Type connectionType);
    }
}
