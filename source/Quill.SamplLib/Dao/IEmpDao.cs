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

using System.Collections.Generic;
using Quill.SampleLib.Entity;

namespace Quill.SampleLib.Dao {
    /// <summary>
    /// Empデータアクセスオブジェクト
    /// </summary>
    public interface IEmpDao {
        /// <summary>
        /// 検索実行
        /// </summary>
        /// <returns></returns>
        List<Employ> Select();

        /// <summary>
        /// 更新実行
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <param name="job"></param>
        void Update(string id, string name, string job);

        /// <summary>
        /// 挿入実行
        /// </summary>
        /// <param name="name"></param>
        /// <param name="job"></param>
        void Insert(string name, string job);

        /// <summary>
        /// 削除実行
        /// </summary>
        /// <param name="id"></param>
        void Delete(string id);
    }
}
