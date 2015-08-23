using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Merchello.Plugin.Payments.QuickPay {
  class Constants {

    public static class ExtendedDataKeys {
      public static string ProcessorSettings = "QuickPayProcessorSettings";

      public static string QuickpayPaymentId = "quickpayPaymentId";
      public static string PaymentAmount = "quickpayPaymentAmount";
      public static string PaymentCurrency = "quickpayPaymentCurrency";

      public static string CaptureTransactionResult = "quickpayCaptureTransactionResult";
    }

    public static string ProviderId = "ef0af38c-a53a-49f8-b8bc-e80a5c4c70a9";

  }
}
