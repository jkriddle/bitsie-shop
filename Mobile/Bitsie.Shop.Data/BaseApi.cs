using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Collections.Specialized;
using System;

namespace Bitsie.Shop.Data
{
	public class BaseApi
	{
		private readonly string _rootUrl;

		public BaseApi(string rootUrl) {
			_rootUrl = rootUrl;
		}

		public string GetUrl(string path) {
			return _rootUrl + path;
		}

		public HttpWebRequest CreateRequest(string path, string method = "GET", 
		                                NameValueCollection data = null,
										string authToken = null) {
			string postData = "";
			if (data != null) {
				postData = String.Join("&", Array.ConvertAll(data.AllKeys, 
					key => string.Format("{0}={1}", key, data[key])));
			}
				
			string url = path;

			// if URL is a relative URL, append the root URL
			if (path.Substring(0, 4) != "http") url = GetUrl(path);

			// Append data if GET request
			if (method == "GET" && !String.IsNullOrEmpty(postData)) {
				if (url.Contains ("?")) {
					url += "&" + postData;
				} else {
					url += "?" + postData;
				}
			}

			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
			request.Method = method;
			request.Timeout = 10000;
			request.UserAgent = "android";

			if (!String.IsNullOrEmpty(authToken)) {
				request.Headers.Add("AuthToken", authToken);
			}

			if (!String.IsNullOrEmpty (postData) && method == "POST") {
				byte[] byteArray = Encoding.UTF8.GetBytes (postData);
				request.ContentType = "application/x-www-form-urlencoded";
				request.ContentLength = byteArray.Length;
				Stream dataStream = request.GetRequestStream ();
				dataStream.Write (byteArray, 0, byteArray.Length);
				dataStream.Close ();
			}

			return request;
		}

		public string SendRequest(string path, string method = "GET", 
									NameValueCollection data = null,
									string authToken = null) {
			HttpWebRequest request = CreateRequest (path, method, data, authToken);
			return ExecuteRequest (request);
		}

		public string ExecuteRequest(HttpWebRequest request) {
			WebResponse response = request.GetResponse ();
			Stream dataStream = response.GetResponseStream ();
			StreamReader reader = new StreamReader (dataStream);
			string content = reader.ReadToEnd ();
			reader.Close ();
			dataStream.Close ();
			response.Close ();

			HttpWebResponse httpResponse = (HttpWebResponse)response;
			if (httpResponse.StatusCode != HttpStatusCode.OK) {
				throw new Exception(httpResponse.StatusCode.ToString() + " error: " + content);
			}
			return content;
		}
	}
}

