using Bitsie.Shop.Api;
using Bitsie.Shop.Web.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Bitsie.Shop.Web.Controllers
{
    public class StatusController : Controller
    {
        private readonly IBitcoinApi _bitcoinApi;

        public StatusController(IBitcoinApi bitcoinApi)
        {
            _bitcoinApi = bitcoinApi;
        }

        [NoCache]
        public void Index()
        {
            try
            {
                int blockHeight = _bitcoinApi.GetBlockHeight();
                Response.Write("Current block: " + blockHeight);
            }
            catch (Exception ex)
            {
                throw new HttpException(500, ex.Message);
            }
        }
    }
}
