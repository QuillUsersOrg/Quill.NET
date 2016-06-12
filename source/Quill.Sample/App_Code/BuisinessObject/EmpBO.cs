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
using Quill.SampleLib.Dao;
using Quill.SampleLib.Entity;
using QM = Quill.QuillManager;

namespace Quill.Sample.App_Code.BuisinessObject {
    /// <summary>
    /// Empデータアクセス用ビジネスオブジェクト
    /// </summary>
    public static class EmpBO {
        /// <summary>
        /// EmpテーブルDao
        /// </summary>
        public static IEmpDao Dao {
            get {
                return QM.Container.GetComponent<IEmpDao>(withInjection: true);
            }
        }

        /// <summary>
        /// 検索実行
        /// </summary>
        /// <returns></returns>
        public static List<Employ> Select() {
            return Dao.Select();
        }

        /// <summary>
        /// 更新実行
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <param name="job"></param>
        public static void Update(string id, string name, string job) {
            Dao.Update(id, name, job);
        }

        /// <summary>
        /// 挿入実行
        /// </summary>
        /// <param name="name"></param>
        /// <param name="job"></param>
        public static void Insert(string name, string job) {
            Dao.Insert(name, job);
            
        }

        /// <summary>
        /// 削除実行
        /// </summary>
        /// <param name="id"></param>
        public static void Delete(string id) {
            Dao.Delete(id);
        }
    }
}
