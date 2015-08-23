using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using Merchello.Core;
using Merchello.Core.Gateways.Payment;
using Merchello.Core.Models;
using Merchello.Core.Sales;
using Merchello.Core.Services;
using Merchello.Plugin.Payments.QuickPay.Models;
using Newtonsoft.Json;
using Umbraco.Core.Logging;
using Umbraco.Web.Mvc;

namespace Merchello.Plugin.Payments.QuickPay.Controllers {

  public class CallbackController : Controller {
    
    public ActionResult Callback(string id) {

    var checkSum = Request.Headers["QuickPay-Checksum-SHA256"];

      // Get the Payload data
      Stream req = Request.InputStream;
      req.Seek(0, System.IO.SeekOrigin.Begin);
      var json = new StreamReader(req).ReadToEnd();

      LogHelper.Info<CallbackController>(() => "[BODY] : " + json);

      var gateway = MerchelloContext.Current.Gateways.Payment.GetProviderByKey(Guid.Parse(Constants.ProviderId));
      var gatewaySettings = gateway.ExtendedData.GetProviderSettings();
      var compute = Sign(json, gatewaySettings.PrivateKey); // Private Key for the Payment Window! Not the API key.


      if (!checkSum.Equals(compute)) {
        LogHelper.Warn<CallbackController>("checkSum did not compute : " + checkSum + "\r\n" + json);
        throw new Exception("MD5 Check does not compute");
      }

      QuickPayResponseModel callbackInput;
      try {
        callbackInput = JsonConvert.DeserializeObject<QuickPayResponseModel>(json);
      } catch (Exception ex) {
        LogHelper.Error<CallbackController>("Unable to deserialize json from QuickPay callback", ex);
        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
      }

      var invoiceNumber = int.Parse(callbackInput.Order_Id);
      var invoice = MerchelloContext.Current.Services.InvoiceService.GetByInvoiceNumber(invoiceNumber);

      var paymentGatewayMethod = MerchelloContext.Current.Gateways.Payment.GetPaymentGatewayMethods().Single(x => x.PaymentMethod.Name == "QuickPay");

      var args = new ProcessorArgumentCollection();
      args.Add(Constants.ExtendedDataKeys.PaymentCurrency, callbackInput.Currency);
      args.Add(Constants.ExtendedDataKeys.PaymentAmount, callbackInput.Operations.Where(x => !x.Pending).Sum(x => x.Amount).ToString("F0"));
      args.Add(Constants.ExtendedDataKeys.QuickpayPaymentId, callbackInput.Id.ToString("F0"));

      var paymentResult = invoice.AuthorizePayment(paymentGatewayMethod, args);

      Notification.Trigger("OrderConfirmation", paymentResult, new[] { invoice.BillToEmail });

      
      return Content("Hello QuickPay");
    }

    private string Sign(string Base, string apiKey) {
      var e = Encoding.UTF8;

      var hmac = new HMACSHA256(e.GetBytes(apiKey));
      byte[] b = hmac.ComputeHash(e.GetBytes(Base));

      var s = new StringBuilder();
      for (int i = 0; i < b.Length; i++) {
        s.Append(b[i].ToString("x2"));
      }

      return s.ToString();
    }
    
  }
}
