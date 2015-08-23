using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Merchello.Plugin.Payments.QuickPay {
  /// <summary>
  /// Represents QuickPay payment processor transaction mode
  /// </summary>
  public enum TransactionMode {
    /// <summary>
    /// An Authorize transaction
    /// </summary>
    Authorize,

    /// <summary>
    /// An Authorize and Capture transaction
    /// </summary>
    AuthorizeAndCapture
  }
}
