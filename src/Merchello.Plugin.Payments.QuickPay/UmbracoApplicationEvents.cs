using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using Merchello.Core.Models;
using Merchello.Core.Services;
using Merchello.Plugin.Payments.QuickPay.Models;
using Umbraco.Core;
using Umbraco.Core.Events;
using Umbraco.Core.Logging;

namespace Merchello.Plugin.Payments.QuickPay {
  public class UmbracoApplicationEvents : ApplicationEventHandler {

    protected override void ApplicationStarted(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext) {
      base.ApplicationStarted(umbracoApplication, applicationContext);

      LogHelper.Info<UmbracoApplicationEvents>("Initializing QuickPay Payment provider events");

      GatewayProviderService.Saving += GatewayProviderServiceOnSaving;

      RouteTable.Routes.MapRoute("MerchelloQuickpay", "MerchelloQuickPay/{action}/{id}", new { controller = "Callback", action = "index", id = UrlParameter.Optional });
    }

    private void GatewayProviderServiceOnSaving(IGatewayProviderService sender, SaveEventArgs<IGatewayProviderSettings> saveEventArgs) {
      var key = new Guid(Constants.ProviderId);

      var provider = saveEventArgs.SavedEntities.FirstOrDefault(x => key == x.Key && !x.HasIdentity);
      if (provider == null) return;

      provider.ExtendedData.SaveProviderSettings(new QuickPayProviderSettings());
    }
  }
}
