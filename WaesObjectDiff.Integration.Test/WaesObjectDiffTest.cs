using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WaesObjectDiff.Models;
using Newtonsoft.Json.Linq;
using Shouldly;
using System.Text;
using System.Configuration;
using System.Net;
using System.IO;

namespace WaesObjectDiff.Integration.Test
{
    [TestClass]
    public class WaesObjectDiffTest
    {
        #region Test Service
        [TestMethod]
        public void TestMissingElements()
        {
            string id = Guid.NewGuid().ToString();
            string left = "{ \"clientName\": \"John P.\" }";
            string right = null;

            string diffResult = SendObjectsToEndpoint(id, left, right);

            diffResult.ShouldBe(DiffResultMessages.MissingElements);
        }

        [TestMethod]
        public void TestEqualElements()
        {
            string id = Guid.NewGuid().ToString();
            string left = "{ \"clientName\": \"John P.\" }";
            string right = "{ \"clientName\": \"John P.\" }";

            string diffResult = SendObjectsToEndpoint(id, left, right);

            diffResult.ShouldBe(DiffResultMessages.EqualElements);
        }

        [TestMethod]
        public void TestDifferentSize()
        {
            string id = Guid.NewGuid().ToString();
            string left = "{ \"clientName\": \"John P.\" }";
            string right = "{ \"clientFirstName\": \"John\", \"clientLastName\": \"Prine\" }";

            string diffResult = SendObjectsToEndpoint(id, left, right);

            diffResult.ShouldBe(DiffResultMessages.DifferentSize);
        }

        [TestMethod]
        public void TestPropertiesMismatch()
        {
            string id = Guid.NewGuid().ToString();
            string left = "{ \"clientName\": \"John P.\" }";
            string right = "{ \"clientFullName\": \"John P.\" }";

            string diffResult = SendObjectsToEndpoint(id, left, right);

            diffResult.ShouldBe(DiffResultMessages.PropertiesMismatch);
        }

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

            string id = Guid.NewGuid().ToString();

            string left = $"{{ \"clientFirstName\": \"John\", \"{propertyNameDiffer}\": \"{leftLastName}\" }}";
            string right = $"{{ \"clientFirstName\": \"John\", \"{propertyNameDiffer}\": \"{rightLastName}\" }}";

            string diffResult = SendObjectsToEndpoint(id, left, right);

            diffResult.ShouldBe(expectedResult.ToString());
        }
        #endregion

        private string SendObjectsToEndpoint(string id, string left, string right)
        {
            string baseUrl = $"{ConfigurationManager.AppSettings["EndpointUrl"]}{id}/";

            if (!string.IsNullOrWhiteSpace(left))
                ((HttpWebRequest)WebRequest.Create($"{baseUrl}/left?data={EncodeToBase64(left)}")).GetResponse();
            if (!string.IsNullOrWhiteSpace(right))
                ((HttpWebRequest)WebRequest.Create($"{baseUrl}/right?data={EncodeToBase64(right)}")).GetResponse();
            
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(baseUrl);

            JObject responseObject;
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
            {
                responseObject = JObject.Parse(reader.ReadToEnd());
            }

            return responseObject["DiffResult"].ToString();
        }

        private string EncodeToBase64(string plainText)
        {
            var plainBytes = Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(plainBytes);
        }
    }
}
