using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Merchello.Plugin.Payments.QuickPay.Models {
  public class QuickPayMetaData {

    public string Type { get; set; }
    public string Brand { get; set; }
    public string Last4 { get; set; }
    public int Exp_Month { get; set; }
    public int Exp_Year { get; set; }
    public string Country { get; set; }
    public bool Is_3d_Secure { get; set; }
    public string Customer_Ip { get; set; }
    public string Customer_Country { get; set; }

  }
}
