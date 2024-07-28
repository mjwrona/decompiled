// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.IntrinsicSettingsManager
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Commerce
{
  internal static class IntrinsicSettingsManager
  {
    private const string Layer = "Commerce";
    private const string Area = "IntrinsicSettingsManager";
    private const string MaxQuantityParamFormat = "{0}_MaxQuantity";

    public static void UpdateResourceSettings(
      IVssRequestContext requestContext,
      Guid hostId,
      Dictionary<string, string> intrinsicSettings)
    {
      requestContext.TraceEnter(5105390, nameof (IntrinsicSettingsManager), "Commerce", nameof (UpdateResourceSettings));
      try
      {
        IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
        IOfferMeterService service1 = vssRequestContext.GetService<IOfferMeterService>();
        foreach (KeyValuePair<string, string> intrinsicSetting in intrinsicSettings)
        {
          KeyValuePair<string, string> pair = intrinsicSetting;
          OfferMeter meter = (OfferMeter) service1.GetOfferMeter(vssRequestContext, pair.Key);
          if (!(meter == (OfferMeter) null))
            CollectionHelper.WithCollectionContext(requestContext, hostId, (Action<IVssRequestContext>) (collectionSystemContext =>
            {
              PlatformOfferSubscriptionService service2 = collectionSystemContext.GetService<PlatformOfferSubscriptionService>();
              if (meter.BillingMode == ResourceBillingMode.PayAsYouGo)
              {
                IntrinsicSettingsManager.UpdateSharedResourceSettings(collectionSystemContext, meter, intrinsicSettings);
              }
              else
              {
                int result;
                if (!int.TryParse(pair.Value, out result))
                  throw new ArgumentException("Intrinsic settings pair key \"" + pair.Key + "\" with value \"" + pair.Value + "\" must have a value that is parseable as an integer.");
                service2.ReportUsage(collectionSystemContext, Guid.NewGuid(), meter.Name, ResourceRenewalGroup.Monthly, result, meter.Name + " usage reported from AuxPortal", DateTime.UtcNow, false);
                collectionSystemContext.Trace(5105384, TraceLevel.Info, nameof (IntrinsicSettingsManager), "Commerce", string.Format("Report usage for resource {0} to {1} for accountId {2}", (object) meter.Name, (object) result, (object) hostId));
              }
            }), new RequestContextType?(RequestContextType.SystemContext), nameof (UpdateResourceSettings));
        }
      }
      catch (Exception ex)
      {
        requestContext.TraceException(5105399, nameof (IntrinsicSettingsManager), "Commerce", ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(5105400, nameof (IntrinsicSettingsManager), "Commerce", nameof (UpdateResourceSettings));
      }
    }

    private static void UpdateSharedResourceSettings(
      IVssRequestContext requestContext,
      OfferMeter meterConfig,
      Dictionary<string, string> intrinsicSettings)
    {
      bool result;
      if (bool.TryParse(intrinsicSettings[meterConfig.Name], out result))
      {
        int? maximumQuantity = new int?();
        string key = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}_MaxQuantity", (object) meterConfig.Name);
        if (intrinsicSettings.ContainsKey(key))
          maximumQuantity = CommerceUtil.ParseNullableInteger(intrinsicSettings[key]);
        requestContext.GetService<IResourceQuantityUpdaterService>().UpdateOfferSubscription(requestContext, maximumQuantity, new int?(), new bool?(result), meterConfig);
      }
      else
        throw new ArgumentException("Intrinsic settings pair key \"" + meterConfig.Name + "\" with value \"" + intrinsicSettings[meterConfig.Name] + "\" must have a value that is parsable as a boolean.");
    }

    public static void UpdateResourceSettings(
      IVssRequestContext requestContext,
      Guid accountId,
      CommerceIntrinsicSettings commerceIntrinsicSettings)
    {
      Dictionary<string, string> dictionary = commerceIntrinsicSettings.Items.ToDictionary<KeyValuePair, string, string>((Func<KeyValuePair, string>) (x => x.Key), (Func<KeyValuePair, string>) (y => y.Value), (IEqualityComparer<string>) StringComparer.InvariantCultureIgnoreCase);
      IntrinsicSettingsManager.UpdateResourceSettings(requestContext, accountId, dictionary);
    }
  }
}
