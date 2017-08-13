using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using WaesObjectDiff.Models;
using WaesObjectDiff.Utility;

namespace WaesObjectDiff.Controllers
{
    public class DiffController : ApiController
    {
        private IDataHolder tempDataHolder;

        public DiffController() : this(new InMemoryDataHolder()) { }
        public DiffController(IDataHolder _tempDataHolder)
        {
            tempDataHolder = _tempDataHolder;
        }

        private string ObjectDiffId;
        private Diff ObjectDiff
        {
            get
            {
                return tempDataHolder.Get(ObjectDiffId);
            }
            set
            {
                tempDataHolder.Set(ObjectDiffId, value);
            }
        }

        [HttpGet]
        public Diff Index(string id)
        {
            InitializeResponseObject(id);
            return ObjectDiff;
        }

        [HttpGet]
        [ActionName("Left")]
        public void SetLeftObject(string id, string data)
        {
            InitializeResponseObject(id);
            ObjectDiff.Left = DeserializeBase64EncodedString(data);
        }

        [HttpGet]
        [ActionName("Right")]
        public void SetRightObject(string id, string data)
        {
            InitializeResponseObject(id);
            ObjectDiff.Right = DeserializeBase64EncodedString(data);
        }

        private void InitializeResponseObject(string id)
        {
            ObjectDiffId = id;
            if (ObjectDiff == null) ObjectDiff = new Diff { Id = id };
        }

        private dynamic DeserializeBase64EncodedString(string base64EncodedString)
        {
            byte[] byteArray;
            try
            {
                byteArray = Convert.FromBase64String(base64EncodedString);
            } catch (Exception e)
            {
                throw new HttpResponseException(
                    new HttpResponseMessage(System.Net.HttpStatusCode.BadRequest)
                    {
                        Content = new StringContent($"Unable to decode string: \"{base64EncodedString}\". {e.Message}"),
                        ReasonPhrase = "Not a valid base64 encoded string"
                    });
            }

            string jsonObject = Encoding.UTF8.GetString(byteArray);
            try
            {
                return JObject.Parse(jsonObject);
            }
            catch (Exception e)
            {
                throw new HttpResponseException(
                    new HttpResponseMessage(System.Net.HttpStatusCode.BadRequest)
                    {
                        Content = new StringContent($"Unable to parse Json object from string: \"{jsonObject}\". {e.Message}"),
                        ReasonPhrase = "Not a valid Json object"
                    });
            }
        }
    }
}
