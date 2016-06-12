using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web.UI.WebControls;
using Quill.Message;
using QM = Quill.QuillManager;

namespace Quill.Sample {
    public partial class MainForm : System.Web.UI.Page {
        private static readonly IDictionary<string, string> INSERT_KEY_MAP
            = CreateInsertKeyMap();

        protected void Page_Load(object sender, EventArgs e) {

        }

        protected void GridView1_RowCommand(object sender, GridViewCommandEventArgs e) {
            if(e.CommandName == "Add") {
                var parameters = CreateParameters(Request.Form);
                ObjectDataSource1.InsertParameters.Clear();
                foreach(var pKey in parameters.Keys) {
                    // 挿入パラメータを書き換え
                    ObjectDataSource1.InsertParameters.Add(pKey, parameters[pKey]);
                }
                ObjectDataSource1.Insert();
            }
        }

        private static IDictionary<string, string> CreateParameters(NameValueCollection requestForm) {
            IDictionary<string, string> parameters = new Dictionary<string, string>();
            foreach(var rKey in requestForm.AllKeys) {
                foreach(var pKey in INSERT_KEY_MAP.Keys) {
                    if(rKey.Contains(pKey)) {
                        parameters[INSERT_KEY_MAP[pKey]] = requestForm[rKey];
                    }
                }
            }

            return parameters;
        }

        private static IDictionary<string, string> CreateInsertKeyMap() {
            var keyMap = new Dictionary<string, string>();
            keyMap["txtNewName"] = "name";
            keyMap["txtNewJob"] = "job";
            return keyMap;
        }
    }
}