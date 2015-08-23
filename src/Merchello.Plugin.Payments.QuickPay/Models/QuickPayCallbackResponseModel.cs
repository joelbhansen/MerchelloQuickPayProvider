using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Merchello.Plugin.Payments.QuickPay.Models {
  public class QuickPayCallbackResponseModel {

    public string PaymentId { get; set; }
    public string Amount { get; set; }
    public string Currency { get; set; }

  }
}
