using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Merchello.Plugin.Payments.QuickPay.Models {
  public class QuickPayOperation {

    public int Id { get; set; }
    public string Type { get; set; }
    public int Amount { get; set; }
    public bool Pending { get; set; }
    public string Qp_Status_Code { get; set; }
    public string Qp_Status_Msg { get; set; }
    public string Aq_Status_Code { get; set; }
    public string Aq_Status_Msg { get; set; }
    public object Data { get; set; }
    public string Created_At { get; set; } // "2015-03-05T10:06:18+00:00"

  }
}
