using Merchello.Core.Models;
using Merchello.Plugin.Payments.QuickPay.Models;
using Newtonsoft.Json;

namespace Merchello.Plugin.Payments.QuickPay {
  /// <summary>
  /// Extended data utiltity extensions
  /// </summary>
  public static class ExtendedDataExtensions {
    /// <summary>
    /// Saves the processor settings to an extended data collection
    /// </summary>
    /// <param name="extendedData">The <see cref="ExtendedDataCollection"/></param>
    /// <param name="providerSettings">The <see cref="QuickPayProviderSettings"/> to be serialized and saved</param>
    public static void SaveProviderSettings(this ExtendedDataCollection extendedData, QuickPayProviderSettings providerSettings) {
      var settingsJson = JsonConvert.SerializeObject(providerSettings);

      extendedData.SetValue(Constants.ExtendedDataKeys.ProcessorSettings, settingsJson);
    }

    /// <summary>
    /// Get the processor settings from the extended data collection
    /// </summary>
    /// <param name="extendedData">The <see cref="ExtendedDataCollection"/></param>
    /// <returns>The deserialized <see cref="QuickPayProviderSettings"/></returns>
    public static QuickPayProviderSettings GetProviderSettings(this ExtendedDataCollection extendedData) {
      if (!extendedData.ContainsKey(Constants.ExtendedDataKeys.ProcessorSettings)) return new QuickPayProviderSettings();

      return JsonConvert.DeserializeObject<QuickPayProviderSettings>(extendedData.GetValue(Constants.ExtendedDataKeys.ProcessorSettings));
    }
  }
}
