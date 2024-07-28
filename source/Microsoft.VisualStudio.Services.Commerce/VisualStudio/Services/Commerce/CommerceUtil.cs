// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.CommerceUtil
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.WindowsAzure.CloudServiceManagement.ResourceProviderCommunication;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Runtime.Serialization;
using System.Text;
using System.Web;
using System.Web.Security.AntiXss;

namespace Microsoft.VisualStudio.Services.Commerce
{
  public static class CommerceUtil
  {
    internal static Dictionary<Guid, string> KnownServicePrincipals = new Dictionary<Guid, string>();
    private const string Area = "Commerce";
    private const string Layer = "CommerceUtil";

    internal static string CheckForRequestSource(IVssRequestContext requestContext)
    {
      string source;
      if (requestContext.TryGetItem<string>("Commerce.RequestSource", out source) || requestContext.RootContext.TryGetItem<string>("Commerce.RequestSource", out source) || CommerceUtil.TryGetSourceFromServicePrincipal(requestContext, out source))
        return source;
      Microsoft.VisualStudio.Services.Identity.Identity identity = CommerceIdentityHelper.GetIdentity(requestContext);
      if (identity == null || string.IsNullOrEmpty(identity.DisplayName))
        return string.Empty;
      return identity.DisplayName.IndexOf("team foundation service", StringComparison.OrdinalIgnoreCase) > 0 ? "Azure" : "Ibiza";
    }

    internal static void SetRequestSource(IVssRequestContext requestContext, string source)
    {
      string source1;
      if (!CommerceUtil.TryGetSourceFromServicePrincipal(requestContext, out source1))
        source1 = source;
      if (!requestContext.Items.ContainsKey("Commerce.RequestSource"))
        requestContext.Items.Add("Commerce.RequestSource", (object) source1);
      else
        requestContext.Items["Commerce.RequestSource"] = (object) source1;
    }

    internal static bool IsBillingTabUrl(Uri referrer) => !(referrer == (Uri) null) && referrer.AbsolutePath.EndsWith("_settings/billing", StringComparison.OrdinalIgnoreCase);

    private static bool TryGetSourceFromServicePrincipal(
      IVssRequestContext requestContext,
      out string source)
    {
      source = string.Empty;
      Guid spGuid;
      return ServicePrincipals.IsServicePrincipal(requestContext, requestContext.UserContext) && ServicePrincipals.TryParse(requestContext.UserContext, out spGuid, out Guid _) && CommerceUtil.KnownServicePrincipals.TryGetValue(spGuid, out source);
    }

    internal static ExtendedUsageMeterCollection GetDefaultUsageMeters(
      IVssRequestContext requestContext,
      MediaTypeFormatter formatter = null)
    {
      requestContext.TraceEnter(5106341, "Commerce", nameof (CommerceUtil), nameof (GetDefaultUsageMeters));
      try
      {
        if (formatter == null)
          formatter = (MediaTypeFormatter) new XmlMediaTypeFormatter();
        ExtendedUsageMeterCollection defaultUsageMeters = new ExtendedUsageMeterCollection();
        Array values = System.Enum.GetValues(typeof (ResourceName));
        List<List<KeyValuePair>> keyValuePairListList = new List<List<KeyValuePair>>(values.Length);
        foreach (object obj in values)
        {
          defaultUsageMeters.Add(new UsageMeter()
          {
            Name = obj.ToString(),
            Unit = "Seats",
            Used = 0.ToString((IFormatProvider) CultureInfo.InvariantCulture),
            Included = 5.ToString((IFormatProvider) CultureInfo.InvariantCulture)
          });
          List<KeyValuePair> keyValuePairList = new List<KeyValuePair>()
          {
            new KeyValuePair("Name", obj.ToString()),
            new KeyValuePair("MaximumQuantity", int.MaxValue.ToString((IFormatProvider) CultureInfo.InvariantCulture)),
            new KeyValuePair("IsPaidBillingEnabled", false.ToString((IFormatProvider) CultureInfo.InvariantCulture))
          };
          keyValuePairListList.Add(keyValuePairList);
        }
        using (MemoryStream writeStream = new MemoryStream())
        {
          formatter.WriteToStreamAsync(typeof (List<List<KeyValuePair>>), (object) keyValuePairListList, (Stream) writeStream, (HttpContent) null, (TransportContext) null).Wait();
          defaultUsageMeters.AdditionalMeterData = Encoding.Default.GetString(writeStream.ToArray());
        }
        return defaultUsageMeters;
      }
      finally
      {
        requestContext.TraceLeave(5106350, "Commerce", nameof (CommerceUtil), nameof (GetDefaultUsageMeters));
      }
    }

    internal static ExtendedUsageMeterCollection GetUsageMeters(
      IVssRequestContext requestContext,
      Guid accountId,
      MediaTypeFormatter formatter = null)
    {
      requestContext.TraceEnter(5106351, "Commerce", nameof (CommerceUtil), nameof (GetUsageMeters));
      Dictionary<ResourceName, bool> dictionary = new Dictionary<ResourceName, bool>()
      {
        {
          ResourceName.AdvancedLicense,
          true
        },
        {
          ResourceName.ProfessionalLicense,
          true
        },
        {
          ResourceName.StandardLicense,
          true
        },
        {
          ResourceName.Build,
          false
        },
        {
          ResourceName.LoadTest,
          false
        }
      };
      try
      {
        IVssRequestContext context = requestContext.Elevate();
        if (formatter == null)
          formatter = (MediaTypeFormatter) new XmlMediaTypeFormatter();
        ExtendedUsageMeterCollection usageMeters = new ExtendedUsageMeterCollection();
        IOfferSubscriptionService service = context.GetService<IOfferSubscriptionService>();
        List<List<KeyValuePair>> keyValuePairListList = new List<List<KeyValuePair>>(dictionary.Count);
        using (IVssRequestContext requestContext1 = requestContext.GetService<ITeamFoundationHostManagementService>().BeginRequest(requestContext, accountId, RequestContextType.SystemContext))
        {
          List<ISubscriptionResource> list1 = service.GetOfferSubscriptions(requestContext1, true).Select<IOfferSubscription, ISubscriptionResource>((Func<IOfferSubscription, ISubscriptionResource>) (m => m.ToSubscriptionResource())).Where<ISubscriptionResource>((Func<ISubscriptionResource, bool>) (m => m != null)).ToList<ISubscriptionResource>();
          List<ISubscriptionResource> list2 = service.GetOfferSubscriptions(requestContext1).Select<IOfferSubscription, ISubscriptionResource>((Func<IOfferSubscription, ISubscriptionResource>) (m => m.ToSubscriptionResource())).Where<ISubscriptionResource>((Func<ISubscriptionResource, bool>) (m => m != null)).ToList<ISubscriptionResource>();
          foreach (KeyValuePair<ResourceName, bool> keyValuePair in dictionary)
          {
            KeyValuePair<ResourceName, bool> subscriptionResourceName = keyValuePair;
            ISubscriptionResource subscriptionResource = subscriptionResourceName.Value ? list1.First<ISubscriptionResource>((Func<ISubscriptionResource, bool>) (x => x.Name == subscriptionResourceName.Key)) : list2.First<ISubscriptionResource>((Func<ISubscriptionResource, bool>) (x => x.Name == subscriptionResourceName.Key));
            ExtendedUsageMeterCollection usageMeterCollection = usageMeters;
            UsageMeter usageMeter = new UsageMeter();
            usageMeter.Name = subscriptionResource.Name.ToString();
            usageMeter.Unit = "Seats";
            int num = subscriptionResource.CommittedQuantity;
            usageMeter.Used = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
            num = subscriptionResource.IncludedQuantity;
            usageMeter.Included = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
            usageMeterCollection.Add(usageMeter);
            List<KeyValuePair> keyValuePairList = new List<KeyValuePair>()
            {
              new KeyValuePair("Name", subscriptionResource.Name.ToString()),
              new KeyValuePair("MaximumQuantity", subscriptionResource.MaximumQuantity.ToString((IFormatProvider) CultureInfo.InvariantCulture)),
              new KeyValuePair("IsPaidBillingEnabled", subscriptionResource.IsPaidBillingEnabled.ToString((IFormatProvider) CultureInfo.InvariantCulture))
            };
            keyValuePairListList.Add(keyValuePairList);
          }
        }
        using (MemoryStream writeStream = new MemoryStream())
        {
          formatter.WriteToStreamAsync(typeof (List<List<KeyValuePair>>), (object) keyValuePairListList, (Stream) writeStream, (HttpContent) null, (TransportContext) null).Wait();
          usageMeters.AdditionalMeterData = Encoding.Default.GetString(writeStream.ToArray());
        }
        return usageMeters;
      }
      finally
      {
        requestContext.TraceLeave(5106360, "Commerce", nameof (CommerceUtil), nameof (GetUsageMeters));
      }
    }

    public static T Deserialize<T>(string inputString) where T : class
    {
      using (Stream stream = (Stream) new MemoryStream())
      {
        byte[] bytes = Encoding.UTF8.GetBytes(inputString);
        stream.Write(bytes, 0, bytes.Length);
        stream.Position = 0L;
        return (T) new DataContractSerializer(typeof (T)).ReadObject(stream);
      }
    }

    public static string Serialize<T>(T obj) where T : class
    {
      using (Stream stream = (Stream) new MemoryStream())
      {
        new DataContractSerializer(typeof (T)).WriteObject(stream, (object) obj);
        stream.Position = 0L;
        return new StreamReader(stream).ReadToEnd();
      }
    }

    public static int? ParseNullableInteger(string value)
    {
      int result;
      return !int.TryParse(value, out result) ? new int?() : new int?(result);
    }

    public static string TryGetParseQueryStringToken(string queryUri, string token)
    {
      string input = string.Empty;
      if (!string.IsNullOrEmpty(queryUri) && !string.IsNullOrEmpty(token))
        input = HttpUtility.ParseQueryString(queryUri)?.Get(token) ?? string.Empty;
      return AntiXssEncoder.HtmlEncode(input, false);
    }

    public static Guid? GetSubscriptionTenantId(
      IVssRequestContext requestContext,
      Guid subscriptionId)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      return vssRequestContext.GetService<IAzureBillingService>().GetBillingContextForSubscription(vssRequestContext, subscriptionId)?.TenantId;
    }

    public static string EncodeToBase64(string stringToEncode) => stringToEncode == null ? (string) null : Convert.ToBase64String(Encoding.ASCII.GetBytes(stringToEncode));

    public static bool IsRunningOnCommerceServiceAsBackup(IVssRequestContext requestContext) => requestContext.IsCommerceService() && !requestContext.IsFeatureEnabled("VisualStudio.Services.Commerce.EnableCommerceMaster");

    public static void MeasureTime(Action action, out int totalMilliseconds)
    {
      DateTime utcNow = DateTime.UtcNow;
      action();
      TimeSpan timeSpan = DateTime.UtcNow - utcNow;
      totalMilliseconds = Convert.ToInt32(timeSpan.TotalMilliseconds);
    }

    public static T MeasureTime<T>(Func<T> action, out int totalMilliseconds)
    {
      DateTime utcNow = DateTime.UtcNow;
      try
      {
        return action();
      }
      finally
      {
        TimeSpan timeSpan = DateTime.UtcNow - utcNow;
        totalMilliseconds = Convert.ToInt32(timeSpan.TotalMilliseconds);
      }
    }

    public static int GetExecutionTimeForForwardingThreshold(
      IVssRequestContext requestContext,
      string layer)
    {
      RegistryEntry registryEntry = requestContext.To(TeamFoundationHostType.Deployment).GetService<IVssRegistryService>().ReadEntries(requestContext, (RegistryQuery) ("/Service/Commerce/Forwarding/" + layer + "/ExecutionTimeThresholdInMs")).SingleOrDefault<RegistryEntry>();
      int result;
      return registryEntry != null && int.TryParse(registryEntry.Value, out result) ? result : 20000;
    }

    public static void TryParsePuidToDecimalFormat(
      IVssRequestContext requestContext,
      string puid,
      out string formattedPuid,
      out string idNamespace)
    {
      idNamespace = "Other";
      long result;
      if (long.TryParse(puid, NumberStyles.HexNumber, (IFormatProvider) CultureInfo.InvariantCulture, out result))
      {
        long int64 = Convert.ToInt64(result);
        formattedPuid = int64.ToString();
        idNamespace = "Puid";
      }
      else
        formattedPuid = puid;
      requestContext.TraceAlways(5109238, TraceLevel.Info, "Commerce", nameof (CommerceUtil), "Puid \"" + puid + "\" is parsed to decimal format \"" + formattedPuid + "\". idNamespace is \"" + idNamespace + "\".");
    }

    public static bool IsBundleEligibleForPurchase(
      IVssRequestContext requestContext,
      string galleryId,
      Guid azureSubscriptionId)
    {
      requestContext.TraceEnter(5109264, "Commerce", nameof (CommerceUtil), new object[2]
      {
        (object) galleryId,
        (object) azureSubscriptionId
      }, nameof (IsBundleEligibleForPurchase));
      try
      {
        if (!CommerceDeploymentHelper.IsBlockVisualStudioAnnualPurchaseEnabled(requestContext) || !CommerceDeploymentHelper.IsBlockingYearlyBundleEnabledForSubscription(requestContext, azureSubscriptionId) || !(galleryId == "ms.vs-enterprise-annual") && !(galleryId == "ms.vs-professional-annual"))
          return true;
        IEnumerable<IOfferSubscription> subscriptionsForGalleryItem = requestContext.GetService<PlatformOfferSubscriptionService>().GetOfferSubscriptionsForGalleryItem(requestContext.Elevate(), azureSubscriptionId, galleryId, false);
        int? nullable1 = subscriptionsForGalleryItem != null ? new int?(subscriptionsForGalleryItem.Count<IOfferSubscription>()) : new int?();
        requestContext.TraceAlways(5109267, TraceLevel.Info, "Commerce", nameof (CommerceUtil), string.Format("Azure Subscription: {0} Gallery ID: {1} Count of offer subscriptions: {2}", (object) azureSubscriptionId, (object) galleryId, (object) nullable1));
        int num1;
        if (subscriptionsForGalleryItem != null)
        {
          int? nullable2 = nullable1;
          int num2 = 0;
          num1 = nullable2.GetValueOrDefault() > num2 & nullable2.HasValue ? 1 : 0;
        }
        else
          num1 = 0;
        return num1 != 0;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(5109265, "Commerce", nameof (CommerceUtil), ex);
        return true;
      }
      finally
      {
        requestContext.TraceLeave(5109266, "Commerce", nameof (CommerceUtil), nameof (IsBundleEligibleForPurchase));
      }
    }

    public static bool IsExistingPurchase(IVssRequestContext requestContext, Guid subscriptionId)
    {
      if (!requestContext.IsFeatureEnabled("VisualStudio.Services.Commerce.EnableOfferTypeValidationForBundle"))
        return false;
      ISubscriptionAccount subscriptionAccount = requestContext.GetService<PlatformSubscriptionService>().GetAccounts(requestContext.Elevate(), subscriptionId, AccountProviderNamespace.Marketplace).SingleOrDefault<ISubscriptionAccount>();
      Guid? nullable1;
      int num;
      if (subscriptionAccount != null)
      {
        nullable1 = subscriptionAccount.SubscriptionId;
        if (nullable1.HasValue)
        {
          nullable1 = subscriptionAccount.SubscriptionId;
          num = nullable1.Equals((object) subscriptionId) ? 1 : 0;
          goto label_6;
        }
      }
      num = 0;
label_6:
      bool flag = num != 0;
      IVssRequestContext requestContext1 = requestContext;
      int existingPurchase = CommerceTracePoints.IsExistingPurchase;
      // ISSUE: variable of a boxed type
      __Boxed<bool> local1 = (ValueType) flag;
      Guid? nullable2;
      if (subscriptionAccount == null)
      {
        nullable1 = new Guid?();
        nullable2 = nullable1;
      }
      else
        nullable2 = subscriptionAccount.SubscriptionId;
      // ISSUE: variable of a boxed type
      __Boxed<Guid?> local2 = (ValueType) nullable2;
      string message = string.Format("isExistingPurchase: {0}. AzureResourceAccount's subscription Id: {1}.", (object) local1, (object) local2);
      requestContext1.Trace(existingPurchase, TraceLevel.Info, "Commerce", nameof (CommerceUtil), message);
      return flag;
    }

    public static bool IsSubscriptionEligibleForBundlePurchase(
      IVssRequestContext requestContext,
      AzureSubscriptionInfo subscriptionInfo)
    {
      if (!requestContext.IsFeatureEnabled("VisualStudio.Services.Commerce.EnableOfferTypeValidationForBundle"))
        return true;
      int num = subscriptionInfo.QuotaId.Equals("DreamSpark_2015-02-01") || subscriptionInfo.QuotaId.Equals("MSDN_2014-09-01") || subscriptionInfo.QuotaId.Equals("BizSpark_2014-09-01") ? 0 : (!subscriptionInfo.QuotaId.Equals("FreeTrial_2014-09-01") ? 1 : 0);
      if (num != 0)
        return num != 0;
      requestContext.TraceAlways(CommerceTracePoints.IsSubscriptionEligibleForBundlePurchase, TraceLevel.Info, "Commerce", nameof (CommerceUtil), string.Format("Feature Flag {0} is turned ON. SubscriptionInfo: {1}.", (object) "VisualStudio.Services.Commerce.EnableOfferTypeValidationForBundle", (object) subscriptionInfo));
      return num != 0;
    }

    public static bool isValidMeterForTrialNotificationEmail(
      IVssRequestContext requestContext,
      string galleryId)
    {
      return !requestContext.IsFeatureEnabled("VisualStudio.Services.Commerce.DisableTrialNotificationEmail.All") && (!string.Equals(galleryId, "Berichthaus.TfsTimetracker", StringComparison.OrdinalIgnoreCase) || !requestContext.IsFeatureEnabled("VisualStudio.Services.Commerce.DisableTrialNotificationEmail.TimeTracker")) && (!string.Equals(galleryId, "mskold.mskold-PRO-EnhancedExport", StringComparison.OrdinalIgnoreCase) || !requestContext.IsFeatureEnabled("VisualStudio.Services.Commerce.DisableTrialNotificationEmail.EnhancedExport")) && (!string.Equals(galleryId, "ms.feed", StringComparison.OrdinalIgnoreCase) || !requestContext.IsFeatureEnabled("VisualStudio.Services.Commerce.DisableTrialNotificationEmail.PackageManagement"));
    }
  }
}
