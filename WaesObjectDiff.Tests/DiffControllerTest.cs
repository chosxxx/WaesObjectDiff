using Moq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WaesObjectDiff.Utility;
using WaesObjectDiff.Controllers;
using WaesObjectDiff.Models;
using Shouldly;
using Newtonsoft.Json.Linq;
using System.Text;

namespace WaesObjectDiff.Tests
{
    [TestClass]
    public class DiffControllerTest
    {
        private DiffController _controller;
        private IDataHolder _dataHolder;

        [TestInitialize]
        public void Initialize()
        {
            _dataHolder = new InMemoryDataHolder();
            _controller = new DiffController(_dataHolder);
        }

        #region Test Set Elements
        [TestCategory("TestSetElements")]
        [TestMethod]
        public void SetLeftShouldSetLeft()
        {
            const string propertyName = "dummyProperty";
            const string expectedValue = "dummyValue";
            string id = "1";

            _controller.SetLeftObject(id, EncodeToBase64($"{{ \"{propertyName}\": \"{expectedValue}\" }}"));

            ValidatePropertyValue(_dataHolder.Get(id).Left, expectedValue, propertyName);
        }
        
        [TestCategory("TestSetElements")]
        [TestMethod]
        public void SetRightShouldSetRight()
        {
            const string propertyName = "dummyProperty";
            const string expectedValue = "dummyValue";
            string id = "1";

            _controller.SetRightObject(id, EncodeToBase64($"{{ \"{propertyName}\": \"{expectedValue}\" }}"));

            ValidatePropertyValue(_dataHolder.Get(id).Right, expectedValue, propertyName);
        }
        #endregion

        #region Test Diff Result
        [TestCategory("TestDiffResult")]
        [TestMethod]
        public void TestMissingElements()
        {
            string id = "1";

            _dataHolder.Set(id, new Diff
            {
                Left = JObject.Parse("{ \"clientName\": \"John P.\" }")
            });
            
            ValidateDiffResult(_controller.Index(id), DiffResultMessages.MissingElements);
        }

        [TestCategory("TestDiffResult")]
        [TestMethod]
        public void TestEqualElements()
        {
            string id = "1";

            _dataHolder.Set(id, new Diff
            {
                Left = JObject.Parse("{ \"clientName\": \"John P.\" }"),
                Right = JObject.Parse("{ \"clientName\": \"John P.\" }")
            });

            ValidateDiffResult(_controller.Index(id), DiffResultMessages.EqualElements);
        }

        [TestCategory("TestDiffResult")]
        [TestMethod]
        public void TestDifferentSize()
        {
            string id = "1";

            _dataHolder.Set(id, new Diff
            {
                Left = JObject.Parse("{ \"clientName\": \"John P.\" }"),
                Right = JObject.Parse("{ \"clientFirstName\": \"John\", \"clientLastName\": \"Prine\" }")
            });

            ValidateDiffResult(_controller.Index(id), DiffResultMessages.DifferentSize);
        }

        [TestCategory("TestDiffResult")]
        [TestMethod]
        public void TestPropertiesMismatch()
        {
            string id = "1";

            _dataHolder.Set(id, new Diff
            {
                Left = JObject.Parse("{ \"clientName\": \"John P.\" }"),
                Right = JObject.Parse("{ \"clientFullName\": \"John P.\" }")
            });

            ValidateDiffResult(_controller.Index(id), DiffResultMessages.PropertiesMismatch);
        }

        [TestCategory("TestDiffResult")]
        [TestMethod]
        public void TestPropertiesDiffer()
        {
            const string leftLastName = "P.";
            const string rightLastName = "Prine";
            const string propertyNameDiffer = "clientLastName";
            var expectedResult = new StringBuilder(DiffResultMessages.PropertiesDiffer);
            expectedResult.AppendLine($"    {propertyNameDiffer}");
            expectedResult.AppendLine($"        Left value size: {leftLastName}");
            expectedResult.AppendLine($"        Right value size: {rightLastName}");

            string id = "1";

            _dataHolder.Set(id, new Diff
            {
                Left = JObject.Parse($"{{ \"clientFirstName\": \"John\", \"{propertyNameDiffer}\": \"{leftLastName}\" }}"),
                Right = JObject.Parse($"{{ \"clientFirstName\": \"John\", \"{propertyNameDiffer}\": \"{rightLastName}\" }}")
            });

            ValidateDiffResult(_controller.Index(id), expectedResult.ToString());
        }
        #endregion

        private string EncodeToBase64(string plainText)
        {
            var plainBytes = Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainBytes);
        }

        private void ValidatePropertyValue(JObject obj, string expected, string propertyName)
        {
            obj.ShouldNotBeNull();
            obj[propertyName].ShouldBe(expected);
        }

        private void ValidateDiffResult(Diff result, string expectedResult)
        {
            result.ShouldNotBeNull();
            result.DiffResult.ShouldBe(expectedResult);
        }
    }
}
