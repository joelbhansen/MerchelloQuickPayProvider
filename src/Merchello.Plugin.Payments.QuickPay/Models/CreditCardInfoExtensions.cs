using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Merchello.Core.Gateways.Payment;

namespace Merchello.Plugin.Payments.QuickPay.Models {
  public static class CreditCardInfoExtensions {

    public static QuickPayCallbackResponseModel AsQuickPayCallbackResponseModel(this ProcessorArgumentCollection args) {
      return new QuickPayCallbackResponseModel() {
        PaymentId = args.ArgValue(Constants.ExtendedDataKeys.QuickpayPaymentId),
        Currency = args.ArgValue(Constants.ExtendedDataKeys.PaymentCurrency),
        Amount = args.ArgValue(Constants.ExtendedDataKeys.PaymentAmount)
      };
    }
    
    private static string ArgValue(this ProcessorArgumentCollection args, string key) {
      return args.ContainsKey(key) ? args[key] : string.Empty;
    }

  }
}
