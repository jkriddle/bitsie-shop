using Bitsie.Shop.Services;
using Bitsie.Shop.Web.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Bitsie.Shop.Web.Areas.Merchant.Controllers
{
    public class DownloadController : BaseMerchantController
    {
        private readonly IOrderService _orderService;

        public DownloadController(IUserService userService, IOrderService orderService) : base(userService)
        {
            _orderService = orderService;
        }

        public ActionResult File(string orderNumber)
        {
            var order = _orderService.GetOrderByNumber(orderNumber);

            if (order == null) throw new HttpException(404, "Order not found: " + orderNumber);
            if (order.Product == null) throw new HttpException(404, "Order does not have product attached: " + orderNumber);
            if (order.Status != Domain.OrderStatus.Complete &&
                order.Status != Domain.OrderStatus.Paid &&
                order.Status != Domain.OrderStatus.Confirmed)
            {
                return Redirect("/" + order.User.MerchantId + "/checkout?orderNumber=" + order.OrderNumber);
            }

            if (order.OrderDate.AddHours(24) < DateTime.UtcNow)
            {
                return MerchantView(order.User, "Expired", new BaseMerchantViewModel
                {
                    Merchant = order.User
                });
            }

            string url = order.Product.RedirectUrl;
            string ext = System.IO.Path.GetExtension(url);

            //Create a stream for the file
            Stream stream = null;

            //This controls how many bytes to read at a time and send to the client
            int bytesToRead = 10000;

            // Buffer to read bytes in chunk size specified above
            byte[] buffer = new Byte[bytesToRead];

            // The number of bytes read
            try
            {
                //Create a WebRequest to get the file
                HttpWebRequest fileReq = (HttpWebRequest)HttpWebRequest.Create(url);

                //Create a response for this request
                HttpWebResponse fileResp = (HttpWebResponse)fileReq.GetResponse();

                if (fileReq.ContentLength > 0)
                    fileResp.ContentLength = fileReq.ContentLength;

                //Get the Stream returned from the response
                stream = fileResp.GetResponseStream();

                // prepare the response to the client. resp is the client Response
                var resp = ControllerContext.HttpContext.Response;

                //Indicate the type of data being sent
                resp.ContentType = "application/octet-stream";

                //Name the file 
                resp.AddHeader("Content-Disposition", "attachment; filename=\"" + order.Product.Title + ext + "\"");
                resp.AddHeader("Content-Length", fileResp.ContentLength.ToString());

                int length;
                do
                {
                    // Verify that the client is connected.
                    if (resp.IsClientConnected)
                    {
                        // Read data into the buffer.
                        length = stream.Read(buffer, 0, bytesToRead);

                        // and write it out to the response's output stream
                        resp.OutputStream.Write(buffer, 0, length);

                        // Flush the data
                        resp.Flush();

                        //Clear the buffer
                        buffer = new Byte[bytesToRead];
                    }
                    else
                    {
                        // cancel the download if client has disconnected
                        length = -1;
                    }
                } while (length > 0); //Repeat until no data is read
            }
            finally
            {
                if (stream != null)
                {
                    //Close the input stream
                    stream.Close();
                }
            }
            return new EmptyResult();
        }

    }
}
