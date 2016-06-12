using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using Castle.DynamicProxy;
using log4net;
using Quill.Config.Impl;
using Quill.Container;
using Quill.Container.Impl;
using Quill.DataSource;
using Quill.DataSource.Impl;
using Quill.Inject;
using Quill.Inject.Impl;
using Quill.Message;
using Quill.Scope.Impl;
using Quill.Util;
using QM = Quill.QuillManager;

namespace Quill.Sample {
    public class Global : HttpApplication {
        protected void Application_Start(object sender, EventArgs e) {
            QM.InitializeDefault();
            QM.ReplaceToParamMark = (pname => "@" + pname);
            QM.OutputLog = OutputLog;
            //特殊なインスタンス生成をcallbackでセット
            QM.ComponentCreator = CreateComponentCreator();

            // インジェクション対象とするフィルターを設定
            QM.InjectionFilter = CreateInjectionFilter();
        }

        protected void Session_Start(object sender, EventArgs e) {
            // 実装なし
        }

        protected void Application_BeginRequest(object sender, EventArgs e) {
            // 実装なし
        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e) {
            // 実装なし
        }

        protected void Application_Error(object sender, EventArgs e) {
            // 実装なし
        }

        protected void Session_End(object sender, EventArgs e) {
            // 実装なし
        }

        protected void Application_End(object sender, EventArgs e) {
            QM.Dispose();
        }

        #region 初期処理

        /// <summary>
        /// コンポーネント作成処理の生成
        /// </summary>
        /// <returns></returns>
        private IComponentCreator CreateComponentCreator() {
            // 設定ファイルから接続文字列を読み取り
            var config = QuillAppConfig.Load();
            var generator = new ProxyGenerator();
            var connectionString = config.GetValue("db.connection_string");

            // コネクション生成処理を設定
            // （デフォルトの動きは引数なしでnew）
            var creator = new ComponentCreators();
            creator.AddCreator(typeof(IDataSource), t => {
                return new DataSourceImpl(() => new SqlConnection(connectionString));
            });

            var componentTypeElements = config.GetElement("components").Elements().ToList();
            componentTypeElements.ForEach(element => {
                var typeName = element.Value;
                var ifTypeName = element.Attribute("interface").Value;

                creator.AddCreator(TypeUtils.GetType(ifTypeName), 
                    t => Activator.CreateInstance(TypeUtils.GetType(typeName)));
            });

            return creator;
        }

        /// <summary>
        /// インジェクション実行時フィルターの生成
        /// </summary>
        /// <returns></returns>
        private IInjectionFilter CreateInjectionFilter() {
            var injectionFilter = new InjectionFilterBase();
            injectionFilter.IsTargetTypeDefault = false;
            injectionFilter.InjectionTargetTypes.Add(typeof(ConnectionDecorator));
            injectionFilter.InjectionTargetTypes.Add(typeof(IDataSource));
            injectionFilter.InjectionTargetTypes.Add(typeof(IDbConnection));

            return injectionFilter;
        }

        /// <summary>
        /// ログ出力
        /// </summary>
        /// <param name="source"></param>
        /// <param name="category"></param>
        /// <param name="log"></param>
        private static void OutputLog(Type source, EnumMsgCategory category, string log) {
            ILog logger = LogManager.GetLogger(source);
            switch(category) {
                case EnumMsgCategory.ERROR:
                    logger.Error(log);
                    break;
                case EnumMsgCategory.WARN:
                    logger.Warn(log);
                    break;
                case EnumMsgCategory.INFO:
                    logger.Info(log);
                    break;
                default:
                    logger.Debug(log);
                    break;
            }
        }

        #endregion
    }
}