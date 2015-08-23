using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Merchello.Core;
using Merchello.Core.Gateways.Payment;
using Merchello.Core.Models;
using Merchello.Plugin.Payments.QuickPay.Models;
using Newtonsoft.Json.Linq;
using Umbraco.Core;
using Umbraco.Core.Logging;

namespace Merchello.Plugin.Payments.QuickPay {
  internal class QuickPayPaymentProcessor {

    private readonly QuickPayProviderSettings _settings;

    public QuickPayPaymentProcessor(QuickPayProviderSettings settings) {
      _settings = settings;
    }

    /// <summary>
    ///     Processes the Authorize and AuthorizeAndCapture transactions
    /// </summary>
    /// <param name="invoice">The <see cref="IInvoice" /> to be paid</param>
    /// <param name="payment">The <see cref="IPayment" /> record</param>
    /// <param name="transactionMode">Authorize or AuthorizeAndCapture</param>
    /// <param name="amount">The money amount to be processed</param>
    /// <returns>The <see cref="IPaymentResult" /></returns>
    public IPaymentResult ProcessPayment(IInvoice invoice, IPayment payment, TransactionMode transactionMode, decimal amount) {

      if (!IsValidCurrencyCode(invoice.CurrencyCode()))
        return new PaymentResult(Attempt<IPayment>.Fail(payment, new Exception(string.Format("Invalid currency. Invoice Currency: '{0}'", invoice.CurrencyCode()))), invoice, false);

      if (transactionMode == TransactionMode.Authorize) {

        //var paymentId = payment.ExtendedData.GetValue(Constants.ExtendedDataKeys.QuickpayPaymentId);
        var currency = payment.ExtendedData.GetValue(Constants.ExtendedDataKeys.PaymentCurrency);
        var amountAuthorizedMinorString = payment.ExtendedData.GetValue(Constants.ExtendedDataKeys.PaymentAmount);
        var amountAuthorized = decimal.Parse(amountAuthorizedMinorString) / (IsZeroDecimalCurrency(currency) ? 100 : 1);

        if (invoice.CurrencyCode() != currency) {
          return new PaymentResult(Attempt<IPayment>.Fail(payment, new Exception(string.Format("Currency mismatch. Invoice Currency: {0}, Payment Currency: {1}", invoice.CurrencyCode(), currency))), invoice, false);
        }

        if (invoice.Total > amountAuthorized) {
          return new PaymentResult(Attempt<IPayment>.Fail(payment, new Exception(string.Format("Amount mismatch. Invoice Amount: {0}, Payment Amount: {1}", invoice.Total.ToString("F2"), amountAuthorized.ToString("F2")))), invoice, false);
        }

        payment.Authorized = true;

        return new PaymentResult(Attempt<IPayment>.Succeed(payment), invoice, invoice.ShippingLineItems().Any());
      }

      return new PaymentResult(Attempt<IPayment>.Fail(payment, new Exception(string.Format("{0}", "QuickPay Payment AuthorizeAndCapture Not Implemented Yet"))), invoice, false);
    }

    public IPaymentResult PriorAuthorizeCapturePayment(IInvoice invoice, IPayment payment, decimal amount) {
      var api = new QuickPayApi(_settings.ApiKey);
      var paymentId = payment.ExtendedData.GetValue(Constants.ExtendedDataKeys.QuickpayPaymentId);
      //var amountMinor = int.Parse(payment.ExtendedData.GetValue(Constants.ExtendedDataKeys.QuickpayPaymentAmount));
      var amountMinor = IsZeroDecimalCurrency(invoice.CurrencyCode()) ? amount.ToString("F0") : (amount * 100).ToString("F0");

      try {
        var captureResult = api.CapturePayment(paymentId, amountMinor);

        if (captureResult.Accepted && captureResult.Operations.Any(x => x.Type == "capture" && x.Amount.ToString("F0") == amountMinor)) {
          LogHelper.Info<QuickPayPaymentProcessor>(string.Format("QuickPay Capture PaymentId {0} Success:\r\n{1}", paymentId, captureResult));
          return new PaymentResult(Attempt<IPayment>.Succeed(payment), invoice, invoice.ShippingLineItems().Any());
        } else {
          LogHelper.Info<QuickPayPaymentProcessor>(string.Format("QuickPay Capture PaymentId {0} Error:\r\n{1}", paymentId, captureResult));
          return new PaymentResult(Attempt<IPayment>.Fail(payment, new Exception(string.Format("{0}", "QuickPay unknown error"))), invoice, false);
        }
      } catch (Exception x) {
        LogHelper.Error<QuickPayPaymentProcessor>(string.Format("QuickPay Capture PaymentId {0} Error:\r\n{1}", paymentId, x.Message), x);
        return new PaymentResult(Attempt<IPayment>.Fail(payment, new Exception(string.Format("{0}", "QuickPay unknown error"))), invoice, false);
      }
    }

    public IPaymentResult RefundPayment(IInvoice invoice, IPayment payment, decimal amount) {
      var api = new QuickPayApi(_settings.ApiKey);

      var paymentId = payment.ExtendedData.GetValue(Constants.ExtendedDataKeys.QuickpayPaymentId);
      var amountMinor = IsZeroDecimalCurrency(invoice.CurrencyCode()) ? amount.ToString("F0") : (amount * 100).ToString("F0");

      try {
        var refundResult = api.RefundPayment(paymentId, amountMinor);

        if (refundResult.Accepted && refundResult.Operations.Any(x => x.Type == "refund" && x.Amount.ToString("F0") == amountMinor)) {
          LogHelper.Info<QuickPayPaymentProcessor>(string.Format("QuickPay Refund PaymentId {0} Success:\r\n{1}", paymentId, refundResult));
          return new PaymentResult(Attempt<IPayment>.Succeed(payment), invoice, invoice.ShippingLineItems().Any());
        } else {
          LogHelper.Info<QuickPayPaymentProcessor>(string.Format("QuickPay Refund PaymentId {0} Error:\r\n{1}", paymentId, refundResult));
          return new PaymentResult(Attempt<IPayment>.Fail(payment, new Exception(string.Format("{0}", "QuickPay Refund Unknown Error"))), invoice, false);
        }
      } catch (Exception x) {
        LogHelper.Error<QuickPayPaymentProcessor>(string.Format("QuickPay Refund PaymentId {0} Error:\r\n{1}", paymentId, x.Message), x);
        return new PaymentResult(Attempt<IPayment>.Fail(payment, new Exception(string.Format("{0}", "QuickPay Refund Unknown Error"))), invoice, false);
      }
    }

    public IPaymentResult VoidPayment(IInvoice invoice, IPayment payment) {
      var api = new QuickPayApi(_settings.ApiKey);

      var paymentId = payment.ExtendedData.GetValue(Constants.ExtendedDataKeys.QuickpayPaymentId);

      try {
        var cancelResult = api.CancelPayment(paymentId);

        if (cancelResult.Accepted && cancelResult.Operations.Any(x => x.Type == "cancel")) {
          LogHelper.Info<QuickPayPaymentProcessor>(string.Format("QuickPay Cancel PaymentId {0} Success:\r\n{1}", paymentId, cancelResult));
          return new PaymentResult(Attempt<IPayment>.Succeed(payment), invoice, invoice.ShippingLineItems().Any());
        } else {
          LogHelper.Info<QuickPayPaymentProcessor>(string.Format("QuickPay Cancel PaymentId {0} Error:\r\n{1}", paymentId, cancelResult));
          return new PaymentResult(Attempt<IPayment>.Fail(payment, new Exception(string.Format("{0}", "QuickPay Cancel Unknown Error"))), invoice, false);
        }
      } catch (Exception x) {
        LogHelper.Error<QuickPayPaymentProcessor>(string.Format("QuickPay Cancel PaymentId {0} Error:\r\n{1}", paymentId, x.Message), x);
        return new PaymentResult(Attempt<IPayment>.Fail(payment, new Exception(string.Format("{0}", "QuickPay Cancel Unknown Error"))), invoice, false);
      }
    }

    /// <summary>
    /// </summary>
    /// <param name="currencyCode"></param>
    /// <returns></returns>
    private static bool IsValidCurrencyCode(string currencyCode) {
      switch (currencyCode) {
        case "USD": // TODO: add other valid codes
        case "GBP":
        case "EUR":
        case "DKK":
        case "SEK":
        case "NOK":
          return true;
      }
      return IsZeroDecimalCurrency(currencyCode);
    }

    /// <summary>
    /// </summary>
    /// <param name="currencyCode"></param>
    /// <returns></returns>
    private static bool IsZeroDecimalCurrency(string currencyCode) {
      switch (currencyCode) {
        case "BIF":
        case "DJF":
        case "JPY":
        case "KRW":
        case "PYG":
        case "VND":
        case "XAF":
        case "XPF":
        case "CLP":
        case "GNF":
        case "KMF":
        case "MGA":
        case "RWF":
        case "VUV":
        case "XOF":
          return true;
      }
      return false;
    }

  }
}
