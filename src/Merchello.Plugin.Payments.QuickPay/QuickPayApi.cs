using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Merchello.Plugin.Payments.QuickPay.Models;
using Newtonsoft.Json;

namespace Merchello.Plugin.Payments.QuickPay {
  public class QuickPayApi {

    private string apiKey { get; set; }

    public QuickPayApi(string apiKey) {
      this.apiKey = apiKey;
    }

    public string Ping() {
      return Generic("/ping", Method.GET);
    }

    public string PaymentList() {
      return Generic(string.Format("/payments"), Method.GET);
    }

    public string PaymentRenew(string id) {
      return Generic(string.Format("/payments/{0}/renew", id), Method.POST);
    }

    public string CallBackList() {
      return Generic(string.Format("/callbacks?failed=false"), Method.GET);
    }

    public string CallBackRetry(string id) {
      return Generic(string.Format("/callbacks/{0}/retry", id), Method.PATCH);
    }

    public QuickPayResponseModel CapturePayment(string paymentId, string amountMinor) {
      var parameters = new NameValueCollection { { "amount", amountMinor } };
      var result = Generic(string.Format("/payments/{0}/capture", paymentId), Method.POST, parameters);
      return JsonConvert.DeserializeObject<QuickPayResponseModel>(result);
    }

    public QuickPayResponseModel RefundPayment(string paymentId, string amountMinor) {
      var parameters = new NameValueCollection { { "amount", amountMinor } };
      var result = Generic(string.Format("/payments/{0}/refund", paymentId), Method.POST, parameters);
      return JsonConvert.DeserializeObject<QuickPayResponseModel>(result);
    }

    public QuickPayResponseModel CancelPayment(string paymentId) {
      var result = Generic(string.Format("/payments/{0}/cancel", paymentId), Method.POST);
      return JsonConvert.DeserializeObject<QuickPayResponseModel>(result);
    }

    public string Generic(string relativeUrl, Method method, NameValueCollection requestParameters = null) {
      var apiUrl = string.Format("https://api.quickpay.net{0}", relativeUrl);

      var request = (HttpWebRequest)WebRequest.Create(apiUrl);
      request.Method = method.ToString();
      request.Accept = "application/json";

      request.Headers.Add("Authorization", BasicAuthToken(apiKey));
      request.Headers.Add("Accept-Version", "v10");

      if (requestParameters != null) {
        var postData = requestParameters.AllKeys.Aggregate("",
                     (current, key) => current + (key + "=" + HttpUtility.UrlEncode(requestParameters[key]) + "&"))
                     .TrimEnd('&');
        request.ContentLength = postData.Length;
        request.ContentType = "application/x-www-form-urlencoded";

        using (var writer = new StreamWriter(request.GetRequestStream())) {
          writer.Write(postData);
        }
      }


      var response = (HttpWebResponse)request.GetResponse();

      var sr = new StreamReader(response.GetResponseStream());

      var responseContent = sr.ReadToEnd();

      return responseContent;
    }

    #region Methods

    private static string BasicAuthToken(string apiKey) {
      string token = Convert.ToBase64String(Encoding.UTF8.GetBytes(string.Format(":{0}", apiKey)));
      return string.Format("Basic {0}", token);
    }

    #endregion

    #region Enums

    public enum Method {
      GET, POST, PUT, PATCH, DELETE
    }

    #endregion

  }
}
