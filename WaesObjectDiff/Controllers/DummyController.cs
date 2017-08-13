using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace WaesObjectDiff.Controllers
{
    public class DummyController : ApiController
    {
        [HttpGet]
        public object Index()
        {
            return new { responseMessage = "Endpoints available at v1/diff/{id}" };
        }
    }
}