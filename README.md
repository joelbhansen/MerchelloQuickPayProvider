# MerchelloQuickPayProvider

This may be the first payment provider for Merchello, with support for the type of payment gateway so common in Scandinavia (QuickPay, ePay [DK], DIBS, Curanet/Wannafind, DanDomain). For this reason, it also differs from other available providers.

## How to use

The correct implementation of this payment provider, differs from the other already available payment providers.

Since QuickPay requires a unique numeric identifier for each payment to process, the order has to be converted into and persisted as an invoice before redirecting the customer to the payment window.

One of the parameters required by QuickPay is the MD5 check value, which must also be calculated and posted as part of the redirecting code.

## Credits

A special thank to the authors of the Stripe and PayPal providers. These two projects have been examined, copied and modified to a great extent while developing this provider.

See https://github.com/Merchello/Merchello/tree/merchello-dev/Plugin/Payments

## Known bugs

- Refund and Void payment buttons don't show
- Invoice Status in the order list is the same for orders both authorized and non-authorized. To fix this usability issue, some changes are needed in Merchello. See http://issues.merchello.com/youtrack/issue/M-811
