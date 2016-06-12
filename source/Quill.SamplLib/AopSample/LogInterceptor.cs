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

using Castle.DynamicProxy;
using Quill.Message;
using QM = Quill.QuillManager;

namespace Quill.SampleLib.AopSample {
    /// <summary>
    /// ログ出力インターセプター
    /// </summary>
    public class LogInterceptor : IInterceptor {
        public void Intercept(IInvocation invocation) {
            QM.OutputLog(GetType(), EnumMsgCategory.DEBUG, GetBasicTargetInfo(invocation, "Start"));
            try {
                invocation.Proceed();
            } catch(System.Exception ex) {
                QM.OutputLog(GetType(), EnumMsgCategory.DEBUG, GetBasicTargetInfo(invocation,
                    string.Format("{0}, StackTrace:{1}", ex.Message, ex.StackTrace)));
            } finally {
                QM.OutputLog(GetType(), EnumMsgCategory.DEBUG, GetBasicTargetInfo(invocation, "End"));
            }
        }

        private string GetBasicTargetInfo(IInvocation invocation, string additionalMsg) {
            return string.Format("{0}.{1} : {2}",
                invocation.Method.DeclaringType.Name, invocation.Method.Name, additionalMsg);
        }
    }
}
