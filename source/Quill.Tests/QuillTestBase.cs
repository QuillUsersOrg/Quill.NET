using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;
using log4net.Config;
using log4net.Repository.Hierarchy;
using NUnit.Framework;
using NUnit.Framework.Internal;
using Quill.Message;

namespace Quill.Tests {
    /// <summary>
    /// Quillテスト基底クラス
    /// </summary>
    [TestFixture]
    public class QuillTestBase {
        [SetUp]
        public virtual void SetUp() {
            QuillManager.InitializeDefault();
            QuillManager.OutputLog = OutputLog;
        }

        [TearDown]
        public virtual void TearDown() {
            QuillManager.Dispose();
        }

        private void OutputLog(Type source, EnumMsgCategory category, string log) {
            var logger = LogManager.GetLogger(source);
            if(category == EnumMsgCategory.ERROR && logger.IsErrorEnabled) {
                logger.ErrorFormat("[{0}] {1}", source, log);
            } else if(category == EnumMsgCategory.WARN && logger.IsWarnEnabled) {
                logger.WarnFormat("[{0}] {1}", source, log);
            } else if(category == EnumMsgCategory.INFO && logger.IsInfoEnabled) {
                logger.InfoFormat("[{0}] {1}", source, log);
            } else if(category == EnumMsgCategory.DEBUG && logger.IsDebugEnabled) {
                logger.DebugFormat("[{0}] {1}", source, log);
            }
        }
    }
}