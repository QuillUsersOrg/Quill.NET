using System;
using log4net.Repository.Hierarchy;
using NUnit.Framework;

namespace Quill.Tests {
    /// <summary>
    /// テストユーティリティクラス
    /// </summary>
    public static class TestUtils {
        /// <summary>
        /// 例外発生を想定したメソッド実行
        /// </summary>
        /// <typeparam name="EX"></typeparam>
        /// <param name="action"></param>
        public static void ExecuteExcectedException<EX>(Action action) where EX : System.Exception {
            try {
                action();
                Assert.Fail(string.Format("ExpectedException:{0}", typeof(EX).FullName));
            } catch(EX) {
                //QuillManager.OutputLog("ExpectedException", ex.Message);
                // OK
            } catch(System.Exception ex) {
                Assert.Fail(string.Format("Expected Exception:[{0}] but [{1}], StackTrace={2}", 
                    typeof(EX).FullName, ex.GetType().FullName, ex.StackTrace));
            }
        }
    }
}
