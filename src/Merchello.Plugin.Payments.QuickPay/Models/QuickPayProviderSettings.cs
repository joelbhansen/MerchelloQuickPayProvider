using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Merchello.Plugin.Payments.QuickPay.Models {
  public class QuickPayProviderSettings {
    public string ApiKey { get; set; }
    public string PrivateKey { get; set; }
    public string MerchantNumber { get; set; }
  }
}
