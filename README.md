# MerchelloQuickPayProvider

This may be the first payment provider for Merchello, with support for the type of payment gateway so common in Scandinavia (QuickPay, ePay [DK], DIBS, Curanet/Wannafind, DanDomain). For this reason, it also differs from other available providers.

## How to use

The correct implementation of this payment provider, differs from the other already available payment providers.

Since QuickPay requires a unique numeric identifier for each payment to process, the order has to be converted into and persisted as an invoice before redirecting the customer to the payment window.

One of the parameters required by QuickPay is the MD5 check value, which must also be calculated and posted as part of the redirecting code.

You will also need 3 different API keys from QuickPay. 2 of them are inserted in the configuration of the Payment Provider within Merchello's Gateways section. The third API key need to be inserted in the HTML form submitted to the QuickPay payment window url. See the sample code below.

## Sample Code

Add the following form to your checkout flow. I may want to assign ID's to these fields, and insert the values with JavaScript and then submit the form with JavaScript as well. That is up to you, but you will need a form to POST the values to QuickPay.

```
<form id="formQuickPay" method="POST" action="https://payment.quickpay.net">
  <input type="hidden" name="version" value="v10">
  <input type="hidden" name="merchant_id" value="1234">
  <input type="hidden" name="agreement_id" value="1234">
  <input type="hidden" name="order_id" value="INSERT VALUE HERE">
  <input type="hidden" name="amount" value="INSERT VALUE HERE">
  <input type="hidden" name="currency" value="INSERT VALUE HERE">
  <input type="hidden" name="continueurl" value="INSERT VALUE HERE">
  <input type="hidden" name="cancelurl" value="INSERT VALUE HERE">
  <input type="hidden" name="callbackurl" value="INSERT VALUE HERE">
  <input type="hidden" name="checksum" value="INSERT VALUE HERE">
  <input type="hidden" name="language" value="en">
  <input type="hidden" name="autocapture" value="0">
  <input type="submit" value="Continue to QuickPay">
</form>
```

The value of the checksum field can be calculated with the method below.

```
public string GetChecksum(string orderId, string amount, string currency, string continueUrl, string cancelUrl, string callbackUrl) {
      var parameters = new Dictionary<string, string>
      {
        {"version", "v10"},
        {"merchant_id", "1234"},
        {"agreement_id", "1234"},
        {"order_id", orderId},
        {"amount", amount},
        {"currency", currency},
        {"continueurl", continueUrl},
        {"cancelurl", cancelUrl},
        {"callbackurl", callbackUrl},
        {"language", "en"},
        {"autocapture", "0"}
      };
      var checksum = Sign(parameters, "ABCDEF1234"); // QuickPay Payment Window API Key
      return checksum;
    }

private string Sign(Dictionary<string, string> parameters, string apiKey) {
      var result = String.Join(" ", parameters.OrderBy(c => c.Key).Select(c => c.Value).ToArray());
      var e = Encoding.UTF8;
      var hmac = new HMACSHA256(e.GetBytes(apiKey));
      byte[] b = hmac.ComputeHash(e.GetBytes(result));
      var s = new StringBuilder();
      for (int i = 0; i < b.Length; i++) {
        s.Append(b[i].ToString("x2"));
      }
      return s.ToString();
    }
```

The Callback URL need to be your website host address followed by this relative URL: /MerchelloQuickPay/Callback

## Credits

A special thank to the authors of the Stripe and PayPal providers. These two projects have been examined, copied and modified to a great extent while developing this provider.

See https://github.com/Merchello/Merchello/tree/merchello-dev/Plugin/Payments

## Known bugs

- Refund and Void payment buttons don't show
- Invoice Status in the order list is the same for orders both authorized and non-authorized. To fix this usability issue, some changes are needed in Merchello. See http://issues.merchello.com/youtrack/issue/M-811
