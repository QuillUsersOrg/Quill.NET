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
using System.Linq;
using System.Reflection;
using Quill.Exception;
using Quill.Message;

namespace Quill.Util {
    /// <summary>
    /// 型情報処理ユーティリティクラス
    /// </summary>
    public static class TypeUtils {
        private static readonly IDictionary<string, Assembly> _cachedAssemblies 
            = CreateCachedAssemblies();
        private static readonly IDictionary<string, Type> _cachedTypes
            = new Dictionary<string, Type>();

        /// <summary>
        /// 型情報の取得
        /// </summary>
        /// <param name="typeFullName">名前空間を含む型名</param>
        /// <param name="assemblyName">アセンブリ名</param>
        /// <returns>型情報</returns>
        /// <exception cref="ArgumentNullException">型名が指定されていない（null）場合</exception>
        /// <exception cref="QuillException">型情報を取得できなかった場合</exception>
        public static Type GetType(string typeFullName, string assemblyName = null) {
            if(typeFullName == null) {
                throw new ArgumentNullException("typeFullName");
            }

            if(!_cachedTypes.ContainsKey(typeFullName)) {
                Type targetType = null;
                if(string.IsNullOrEmpty(assemblyName)) {
                    foreach(var assembly in _cachedAssemblies.Values) {
                        var type = assembly.GetType(typeFullName);
                        if(type != null) {
                            targetType = type;
                            break;
                        }
                    }
                } else if(_cachedAssemblies.ContainsKey(assemblyName)) {
                    var assembly = _cachedAssemblies[assemblyName];
                    targetType = assembly.GetType(typeFullName);
                }

                if(targetType != null) {
                    _cachedTypes[typeFullName] = targetType;
                }
            }

            if(_cachedTypes.ContainsKey(typeFullName)) {
                return _cachedTypes[typeFullName];
            }
            throw new QuillException(string.Format("{0}, TypeFullName={1}, AssemblyName={2}",
                QMsg.TypeNotFound.Get(), typeFullName, assemblyName));
        }


        /// <summary>
        /// 親クラスも含めた全フィールド情報の取得
        /// </summary>
        /// <param name="type">フィールド情報を取得する型</param>
        /// <param name="flags">取得条件フラグ</param>
        /// <returns>フィールド情報</returns>
        public static FieldInfo[] GetAllFields(this Type type, BindingFlags flags) {
            ISet<FieldInfo> fields = new HashSet<FieldInfo>();
            for(Type currentType = type; currentType != null; currentType = currentType.BaseType) {
                FieldInfo[] currentFields = currentType.GetFields(flags);
                foreach(FieldInfo fi in currentFields) {
                    if(!fields.Contains(fi)) {
                        fields.Add(fi);
                    }
                }
            }
            return fields.ToArray();
        }

        /// <summary>
        /// キャッシュ済アセンブリ情報の生成
        /// </summary>
        /// <returns>キャッシュ済アセンブリ情報</returns>
        private static IDictionary<string, Assembly> CreateCachedAssemblies() {
            IDictionary<string, Assembly> cachedAssemblies =
                new Dictionary<string, Assembly>();
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach(var assembly in assemblies) {
                string key = assembly.GetName().Name;
                if(!cachedAssemblies.ContainsKey(key)) {
                    cachedAssemblies[key] = assembly;
                }
            }

            return cachedAssemblies;
        }
    }
}
