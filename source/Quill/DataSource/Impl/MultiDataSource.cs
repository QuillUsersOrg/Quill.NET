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
using System.Data;
using Quill.Message;

namespace Quill.DataSource.Impl {
    /// <summary>
    /// 複数データソース
    /// </summary>
    public class MultiDataSource : IDataSource {
        /// <summary>
        /// データソースMap
        /// </summary>
        protected readonly IDictionary<string, IDataSource> _sources;

        /// <summary>
        /// デフォルトのデータソース（データソース名が指定されない場合に使用）
        /// </summary>
        public IDataSource DefaultDataSource { get; set; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public MultiDataSource() {
            _sources = new Dictionary<string, IDataSource>();
        }

        /// <summary>
        /// データソース登録
        /// </summary>
        /// <param name="dataSourceName">データソース名</param>
        /// <param name="dataSource">データソース</param>
        public virtual void RegisterDataSource(string dataSourceName, IDataSource dataSource) {
            _sources[dataSourceName] = dataSource;
        }

        /// <summary>
        /// コネクションの取得
        /// </summary>
        /// <param name="dataSourceName">データソース名（省略化）</param>
        /// <returns>コネクション</returns>
        public virtual IDbConnection GetConnection(string dataSourceName = null) {
            if(dataSourceName == null || dataSourceName == string.Empty) {
                return DefaultDataSource.GetConnection();
            }

            if(_sources.ContainsKey(dataSourceName)) {
                return _sources[dataSourceName].GetConnection(dataSourceName);
            }
            throw new ArgumentException(string.Format("{0} DataSource={1}",
               QMsg.NotRegisteredDataSource.Get(), dataSourceName));
        }

        /// <summary>
        /// リソース解放
        /// </summary>
        public virtual void Dispose() {
            foreach(IDataSource source in _sources.Values) {
                source.Dispose();
            }
            _sources.Clear();
        }
    }
}
