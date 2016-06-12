using NUnit.Framework;

namespace Quill.Ado.Tests {
    [TestFixture()]
    public class SqlPropTests {
        [Test()]
        public void CreateByPlainSQL_Test() {
            // Arrange
            const string sql = "SELECT * FROM Hoge";

            // Act
            var actual = new SqlProp(sql, ReplacetoParameterMark);

            // Assert
            Assert.AreEqual(sql, actual.ActualSql);
            Assert.AreEqual(0, actual.ParameterNames.Length);
        }

        [Test()]
        public void CreateByPrametarizedSQL_Test() {
            // Arrange
            const string sql = "SELECT * FROM Hoge WHERE ID = /* id */10";
            const string expectedSql = "SELECT * FROM Hoge WHERE ID = @id";

            // Act
            var actual = new SqlProp(sql, ReplacetoParameterMark);

            // Assert
            Assert.AreNotEqual(sql, actual.ActualSql);
            Assert.AreEqual(expectedSql, actual.ActualSql);
            Assert.AreEqual(1, actual.ParameterNames.Length);
            Assert.AreEqual("@id", actual.ParameterNames[0].Mark);
            Assert.AreEqual("id", actual.ParameterNames[0].Name);
        }

        [Test()]
        public void CreateByPrametarizedSQL2_Test() {
            // Arrange
            const string sql = 
                "SELECT * FROM Hoge LEFT OUTER JOIN Huga ON Hoge.ID = Huga.ID AND Huga.Name = /* hname */'aiueo' WHERE ID = /* id */10";
            const string expectedSql = 
                "SELECT * FROM Hoge LEFT OUTER JOIN Huga ON Hoge.ID = Huga.ID AND Huga.Name = @hname WHERE ID = @id";

            // Act
            var actual = new SqlProp(sql, ReplacetoParameterMark);

            // Assert
            Assert.AreNotEqual(sql, actual.ActualSql);
            Assert.AreEqual(expectedSql, actual.ActualSql);
            Assert.AreEqual(2, actual.ParameterNames.Length);
            Assert.AreEqual("@hname", actual.ParameterNames[0].Mark);
            Assert.AreEqual("hname", actual.ParameterNames[0].Name);
            Assert.AreEqual("@id", actual.ParameterNames[1].Mark);
            Assert.AreEqual("id", actual.ParameterNames[1].Name);
        }

        [Test()]
        public void CreateByPrametarizedSQL4_Test() {
            // Arrange
            const string sql =
                "UPDATE Hoge SET id = /* id */99 , name = /* name */'quill' WHERE No IN (/* a */1 , /* b */2 )";
            const string expectedSql =
                "UPDATE Hoge SET id = @id , name = @name WHERE No IN (@a , @b )";

            // Act
            var actual = new SqlProp(sql, ReplacetoParameterMark);

            // Assert
            Assert.AreNotEqual(sql, actual.ActualSql);
            Assert.AreEqual(expectedSql, actual.ActualSql);
            Assert.AreEqual(4, actual.ParameterNames.Length);
            Assert.AreEqual("@id", actual.ParameterNames[0].Mark);
            Assert.AreEqual("id", actual.ParameterNames[0].Name);
            Assert.AreEqual("@name", actual.ParameterNames[1].Mark);
            Assert.AreEqual("name", actual.ParameterNames[1].Name);
            Assert.AreEqual("@a", actual.ParameterNames[2].Mark);
            Assert.AreEqual("a", actual.ParameterNames[2].Name);
            Assert.AreEqual("@b", actual.ParameterNames[3].Mark);
            Assert.AreEqual("b", actual.ParameterNames[3].Name);
        }

        private static string ReplacetoParameterMark(string paramName) {
            return "@" + paramName;
        }
    }
}