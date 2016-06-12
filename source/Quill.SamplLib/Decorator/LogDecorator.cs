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
using Quill.Message;
using Quill.Scope;
using QM = Quill.QuillManager;

namespace Quill.SamplLib.Decorator {
    public class LogDecorator : IQuillDecorator {
        public void Decorate(Action action, IDictionary<string, object> args = null) {
            Func<object> invoker = (() => { action(); return default(object); });
            Decorate(invoker, action.Target.GetType(), action.Method.Name);
        }

        public RETURN_TYPE Decorate<RETURN_TYPE>(Func<RETURN_TYPE> func,
            IDictionary<string, object> args = null) {
            return Decorate(() => func(), func.Target.GetType(), func.Method.Name);
        }

        private RETURN_TYPE Decorate<RETURN_TYPE>(Func<RETURN_TYPE> invoker, Type targetType, string methodName) {
            QM.OutputLog(targetType, EnumMsgCategory.DEBUG, string.Format("{0} start.", methodName));
            try {
                var result = invoker();
                QM.OutputLog(targetType, EnumMsgCategory.DEBUG, string.Format("{0} is successful.", methodName));
                return result;
            } catch(System.Exception ex) {
                QM.OutputLog(targetType, EnumMsgCategory.DEBUG, string.Format("{0} is failure.", methodName));
                QM.OutputLog(targetType, EnumMsgCategory.ERROR, 
                    string.Format("Message:{0}, StackTrace:{1}", ex.Message, ex.StackTrace));
                throw;
            }
        }
    }
}
