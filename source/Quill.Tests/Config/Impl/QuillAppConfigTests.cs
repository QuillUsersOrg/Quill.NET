using System.Linq;
using System.Xml.Linq;
using NUnit.Framework;
using Quill.Tests;

namespace Quill.Config.Impl.Tests {
    [TestFixture()]
    public class QuillAppConfigTests : QuillTestBase {

        [Test]
        public void Load_Exists() {
            IQuillConfig target = QuillAppConfig.Load();
            Assert.NotNull(target);
        }

        [Test]
        public void GetValue() {
            IQuillConfig target = QuillAppConfig.Load();
            var actual = target.GetValue("hoge");
            Assert.AreEqual("XXX", actual);
        }

        [Test]
        public void GetValue_Child() {
            IQuillConfig target = QuillAppConfig.Load();
            var actual = target.GetValue("Par.Chi");
            Assert.AreEqual("ChildValue", actual);
        }

        [Test]
        public void GetValue_AdditionalFilter() {
            IQuillConfig target = QuillAppConfig.Load();
            var actual = target.GetValue("Par.Bro", e => e.Value.StartsWith("a"));
            Assert.True(actual.StartsWith("a"));
        }

        [Test]
        public void GetValue_IllegalNodePath() {
            IQuillConfig target = QuillAppConfig.Load();
            var actual = target.GetValue("");
            Assert.IsNull(actual);
        }

        [Test]
        public void GetValues() {
            IQuillConfig target = QuillAppConfig.Load();
            var actual = target.GetValues("Mul");
            Assert.AreEqual(2, actual.Count());
            actual.ForEach(v => Assert.IsNotNull(v));
        }

        [Test]
        public void GetValues_AdditionalFilter() {
            IQuillConfig target = QuillAppConfig.Load();
            var actual = target.GetValues("Par.Sis", e => e.Value.StartsWith("x"));

            Assert.AreEqual(3, actual.Count());
            actual.ForEach(v => Assert.IsTrue(v.StartsWith("x")));
        }

        [Test]
        public void GetValues_IllegalNodePath() {
            IQuillConfig target = QuillAppConfig.Load();
            var actual = target.GetValues("");
            Assert.NotNull(actual);
            Assert.AreEqual(0, actual.Count());
        }

        [Test]
        public void GetElement() {
            IQuillConfig target = QuillAppConfig.Load();
            var actual = target.GetElement("At");

            Assert.NotNull(actual);
            Assert.AreEqual(typeof(XElement), actual.GetType());
        }

        [Test]
        public void GetElement_AdditionalFilter() {
            IQuillConfig target = QuillAppConfig.Load();
            var actual = target.GetElement("At", el => el.GetAttrValue("hogeAttr") == "wao");

            Assert.NotNull(actual);
            Assert.AreEqual("wao", actual.GetAttrValue("hogeAttr"));
        }

        [Test]
        public void GetElement_IllegalNodePath() {
            IQuillConfig target = QuillAppConfig.Load();
            var actual = target.GetElement("");
            Assert.IsNull(actual);
        }

        [Test]
        public void GetElements() {
            IQuillConfig target = QuillAppConfig.Load();
            var actual = target.GetElements("At");

            Assert.IsTrue(actual.Count() > 0);
        }

        [Test]
        public void GetElements_AdditionalFilter() {
            IQuillConfig target = QuillAppConfig.Load();
            var actual = target.GetElements("At", el => el.GetAttrValue("hogeAttr") == "wao");

            Assert.IsTrue(actual.Count() > 0);
            actual.ForEach(el => Assert.AreEqual("wao", el.GetAttrValue("hogeAttr")));
        }

        [Test]
        public void GetElements_IllegalNodePath() {
            IQuillConfig target = QuillAppConfig.Load();
            var actual = target.GetElements("");
            Assert.NotNull(actual);
            Assert.AreEqual(0, actual.Count());
        }
    }
}