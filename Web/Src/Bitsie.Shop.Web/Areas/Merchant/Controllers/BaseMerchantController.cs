using Bitsie.Shop.Domain;
using Bitsie.Shop.Services;
using Bitsie.Shop.Web.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Bitsie.Shop.Web.Areas.Merchant.Controllers
{
    public class BaseMerchantController : BaseController
    {
        private readonly IUserService _userService;

        public BaseMerchantController(IUserService userService) : base(userService) {
            _userService = userService;
        }

        /// <summary>
        /// Render a Merchant's custom template if they have one, or fall back to default template if not.
        /// </summary>
        /// <param name="merchant"></param>
        /// <param name="view"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        protected ActionResult MerchantView(User merchant, string view, object model)
        {
            string content = "";
            string layoutHtml = RenderToString("Template", model);
            string viewHtml = RenderToString(view, model);
            string templateHtml = merchant.Settings.HtmlTemplate;

            if (String.IsNullOrEmpty(templateHtml) || (merchant.Status != UserStatus.Active && merchant.Status != UserStatus.Suspended))
            {
                // Render default
                templateHtml = "{{form}}";
            }

            templateHtml = templateHtml.Replace("{{form}}", viewHtml);
            content = layoutHtml.Replace("{{template}}", templateHtml);
            return Content(content);
        }

        /// <summary>
        /// Retrieve a merchant by their ID or return exceptions
        /// </summary>
        /// <param name="merchantId"></param>
        /// <returns></returns>
        protected User GetMerchant(string merchantId)
        {
            var merchant = _userService.GetUserByMerchantId(merchantId);
            if (merchant == null)
            {
                throw new HttpException(404, "Merchant not found.");
            }

            if (merchant.Role != Domain.Role.Merchant)
            {
                throw new HttpException(404, "User is not a merchant.");
            }
            return merchant;
        }
    }
}