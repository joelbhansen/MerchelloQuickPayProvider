using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Merchello.Plugin.Payments.QuickPay.Models {
  public class QuickPayResponseModel {

    public int Id { get; set; }
    public string Order_Id { get; set; }
    public bool Accepted { get; set; }
    public bool Test_Mode { get; set; }
    public string BrandingId { get; set; }
    public string Acquirer { get; set; }
    public List<QuickPayOperation> Operations { get; set; }
    public QuickPayMetaData MetaData { get; set; }
    public string Created_At { get; set; }
    public int Balance { get; set; }
    public string Currency { get; set; }

  }
}
