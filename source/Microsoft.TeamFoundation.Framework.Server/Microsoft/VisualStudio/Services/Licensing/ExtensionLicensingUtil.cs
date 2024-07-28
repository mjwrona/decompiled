// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Licensing.ExtensionLicensingUtil
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.AzComm.Contracts;
using Microsoft.TeamFoundation.Framework.Server.AzComm.HttpClients;
using Microsoft.VisualStudio.Services.Commerce;
using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Licensing
{
  public static class ExtensionLicensingUtil
  {
    private const string _disableExtensionLicensing = "AzureDevOps.Services.Licensing.DisableExtensionLicensing";
    private const string _useCommerceV2Apis = "AzureDevOps.Services.Licensing.UseCommerceV2Apis";
    private const string _readCommerceV2Apis = "AzureDevOps.Services.Licensing.ReadCommerceV2Apis";
    private const string _readCommerceV1Apis = "AzureDevOps.Services.Licensing.ReadCommerceV1Apis";
    private const string _testManagerExtensionId = "ms.vss-testmanager-web";
    private const string s_area = "Extensions";
    private const string s_layer = "ExtensionLicensingUtil";

    public static bool IsExtensionLicensingDisabled(IVssRequestContext requestContext) => requestContext.IsFeatureEnabled("AzureDevOps.Services.Licensing.DisableExtensionLicensing");

    public static IDictionary<string, bool> GetExtensionRightsWhenDisabled(
      IVssRequestContext requestContext,
      IEnumerable<string> extensionIds)
    {
      requestContext.TraceConditionally(6124901, TraceLevel.Info, "Extensions", nameof (ExtensionLicensingUtil), (Func<string>) (() => "Extensions requested: " + string.Join(",", extensionIds)));
      Dictionary<string, bool> dictionary = extensionIds.ToDictionary<string, string, bool>((Func<string, string>) (id => id), (Func<string, bool>) (id => true));
      try
      {
        if (extensionIds.Contains<string>("ms.vss-testmanager-web"))
        {
          bool flag = ExtensionLicensingUtil.GetExtensionRightsWhenDisabled(requestContext).SingleOrDefault<KeyValuePair<string, bool>>((Func<KeyValuePair<string, bool>, bool>) (pair => pair.Key == "ms.vss-testmanager-web")).Value;
          dictionary["ms.vss-testmanager-web"] = flag;
        }
      }
      catch (Exception ex)
      {
        requestContext.TraceException(7050772, TraceLevel.Error, "Extensions", nameof (ExtensionLicensingUtil), ex, "Exception occurred when trying to get extension rights for {0}. Returning true for all.", (object) string.Join(",", extensionIds));
        return (IDictionary<string, bool>) extensionIds.ToDictionary<string, string, bool>((Func<string, string>) (id => id), (Func<string, bool>) (id => true));
      }
      return (IDictionary<string, bool>) dictionary;
    }

    public static IDictionary<string, bool> GetExtensionRightsWhenDisabled(
      IVssRequestContext requestContext)
    {
      IDictionary<string, bool> rightsFromCommerceV1 = (IDictionary<string, bool>) null;
      IDictionary<string, bool> rightsFromCommerceV2 = (IDictionary<string, bool>) null;
      if (requestContext.IsFeatureEnabled("AzureDevOps.Services.Licensing.ReadCommerceV1Apis") || !requestContext.IsFeatureEnabled("AzureDevOps.Services.Licensing.ReadCommerceV2Apis"))
      {
        requestContext.Trace(580760, TraceLevel.Info, "Extensions", nameof (ExtensionLicensingUtil), "Reading from commerce V1");
        IEnumerable<IOfferSubscription> offerSubscriptions = requestContext.GetService<IOfferSubscriptionService>().GetOfferSubscriptions(requestContext);
        if (offerSubscriptions == null)
        {
          requestContext.TraceAlways(580762, TraceLevel.Info, "Extensions", nameof (ExtensionLicensingUtil), "OfferSubscriptions from commerce came back null.");
          return (IDictionary<string, bool>) new Dictionary<string, bool>();
        }
        requestContext.TraceConditionally(455805, TraceLevel.Info, "Extensions", nameof (ExtensionLicensingUtil), (Func<string>) (() => "Extensions returned from commerce: " + string.Join(",", offerSubscriptions.Select<IOfferSubscription, string>((Func<IOfferSubscription, string>) (o => o.OfferMeter.GalleryId)))));
        rightsFromCommerceV1 = (IDictionary<string, bool>) offerSubscriptions.Where<IOfferSubscription>((Func<IOfferSubscription, bool>) (o => o != null && o.OfferMeter != (OfferMeter) null)).Where<IOfferSubscription>((Func<IOfferSubscription, bool>) (o => o.OfferMeter.Category == MeterCategory.Extension && o.OfferMeter.GalleryId != "ms.vss-testmanager-web")).ToDictionary<IOfferSubscription, string, bool>((Func<IOfferSubscription, string>) (extension => extension.OfferMeter.GalleryId), (Func<IOfferSubscription, bool>) (extension => true));
        if (ExtensionLicensingUtil.IsTestPlansInTrial(requestContext, offerSubscriptions))
        {
          requestContext.Trace(3127509, TraceLevel.Info, "Extensions", nameof (ExtensionLicensingUtil), "Test Plans is in trial");
          rightsFromCommerceV1.Add("ms.vss-testmanager-web", true);
        }
        else
        {
          requestContext.Trace(8764710, TraceLevel.Info, "Extensions", nameof (ExtensionLicensingUtil), "Test Plans is not in trial");
          rightsFromCommerceV1.Add("ms.vss-testmanager-web", false);
        }
      }
      if (requestContext.IsFeatureEnabled("AzureDevOps.Services.Licensing.ReadCommerceV2Apis"))
      {
        requestContext.Trace(8764715, TraceLevel.Info, "Extensions", nameof (ExtensionLicensingUtil), "Reading from commerce V2");
        rightsFromCommerceV2 = ExtensionLicensingUtil.GetExtensionRightsWhenDisabledFromCommerceV2(requestContext);
      }
      if (requestContext.IsFeatureEnabled("AzureDevOps.Services.Licensing.ReadCommerceV1Apis") && requestContext.IsFeatureEnabled("AzureDevOps.Services.Licensing.ReadCommerceV2Apis"))
        ExtensionLicensingUtil.CompareExtensionRightsForTestPlans(requestContext, rightsFromCommerceV1, rightsFromCommerceV2);
      if (requestContext.IsFeatureEnabled("AzureDevOps.Services.Licensing.ReadCommerceV2Apis") && requestContext.IsFeatureEnabled("AzureDevOps.Services.Licensing.UseCommerceV2Apis"))
      {
        requestContext.Trace(8764720, TraceLevel.Info, "Extensions", nameof (ExtensionLicensingUtil), "Returned from commerce V2 values: " + JsonConvert.SerializeObject((object) rightsFromCommerceV2));
        return rightsFromCommerceV2;
      }
      requestContext.Trace(8764725, TraceLevel.Info, "Extensions", nameof (ExtensionLicensingUtil), "Returned from commerce V1 values: " + JsonConvert.SerializeObject((object) rightsFromCommerceV1));
      return rightsFromCommerceV1;
    }

    private static void CompareExtensionRightsForTestPlans(
      IVssRequestContext requestContext,
      IDictionary<string, bool> rightsFromCommerceV1,
      IDictionary<string, bool> rightsFromCommerceV2)
    {
      bool flag1 = rightsFromCommerceV1["ms.vss-testmanager-web"];
      bool flag2 = rightsFromCommerceV2["ms.vss-testmanager-web"];
      if (flag1 != flag2)
        requestContext.TraceAlways(1912137, TraceLevel.Error, "Extensions", nameof (ExtensionLicensingUtil), string.Format("Commerce V1 and V2 apis are not the same. V1 returned {0} and V2 returned {1}.", (object) flag1, (object) flag2));
      else
        requestContext.Trace(6974518, TraceLevel.Info, "Extensions", nameof (ExtensionLicensingUtil), string.Format("Commerce V1 and V2 apis are the same. Both returned {0}.", (object) flag1));
    }

    public static IDictionary<string, bool> GetExtensionRightsWhenDisabledFromCommerceV2(
      IVssRequestContext requestContext)
    {
      MeterUsage2GetResponse testPlansMeterUsage = ExtensionLicensingUtil.GetTestPlansMeterUsage(requestContext);
      if (testPlansMeterUsage == null)
      {
        requestContext.TraceAlways(1241816, TraceLevel.Info, "Extensions", nameof (ExtensionLicensingUtil), "MeterUsages from commerce came back null.");
        return (IDictionary<string, bool>) new Dictionary<string, bool>();
      }
      Dictionary<string, bool> disabledFromCommerceV2 = new Dictionary<string, bool>();
      if (testPlansMeterUsage.IsInTrial)
      {
        requestContext.Trace(8995332, TraceLevel.Info, "Extensions", nameof (ExtensionLicensingUtil), "Test Plans is in trial");
        disabledFromCommerceV2.Add("ms.vss-testmanager-web", true);
      }
      else
      {
        requestContext.Trace(4468702, TraceLevel.Info, "Extensions", nameof (ExtensionLicensingUtil), "Test Plans is not in trial");
        disabledFromCommerceV2.Add("ms.vss-testmanager-web", false);
      }
      return (IDictionary<string, bool>) disabledFromCommerceV2;
    }

    public static bool TryApplyingOnDemandLicense(
      IVssRequestContext requestContext,
      string extensionId)
    {
      try
      {
        if (requestContext.ExecutionEnvironment.IsOnPremisesDeployment)
          return false;
        if (requestContext.IsSystemContext)
        {
          requestContext.Trace(100136260, TraceLevel.Info, "Extensions", nameof (ExtensionLicensingUtil), "System context can not be assigned a license for " + extensionId);
          return false;
        }
        Guid userId = requestContext.GetUserId();
        if (userId.Equals(Guid.Empty))
        {
          requestContext.Trace(100136261, TraceLevel.Info, "Extensions", nameof (ExtensionLicensingUtil), "Empty User-id can not be assigned a license for " + extensionId);
          return false;
        }
        if (!requestContext.IsFeatureEnabled("VisualStudio.Framework.Licensing.EnableAutoExtensionAssignment"))
        {
          requestContext.Trace(100136262, TraceLevel.Info, "Extensions", nameof (ExtensionLicensingUtil), string.Format("Extension auto assignment feature is not enabled in {0} for {1}", (object) requestContext.ServiceHost.InstanceId, (object) extensionId));
          return false;
        }
        requestContext.Trace(100136263, TraceLevel.Info, "Extensions", nameof (ExtensionLicensingUtil), string.Format("Attempting on demand licensing for user {0} on extension {1}", (object) userId, (object) extensionId));
        ICollection<ExtensionOperationResult> users = requestContext.GetService<IExtensionEntitlementService>().AssignExtensionToUsers(requestContext.Elevate(), extensionId, (IList<Guid>) new List<Guid>()
        {
          userId
        }, true);
        if (users.Any<ExtensionOperationResult>())
        {
          requestContext.Trace(100136264, TraceLevel.Info, "Extensions", nameof (ExtensionLicensingUtil), string.Format("On demand licensing for user {0} on extension {1} failed with '{2}'", (object) userId, (object) extensionId, (object) users.First<ExtensionOperationResult>().Message));
          return false;
        }
        requestContext.Trace(100136265, TraceLevel.Info, "Extensions", nameof (ExtensionLicensingUtil), string.Format("On demand licensing for user {0} on extension {1} was successful.", (object) userId, (object) extensionId));
        return true;
      }
      catch (LicenseNotAvailableException ex)
      {
        requestContext.Trace(100136267, TraceLevel.Info, "Extensions", nameof (ExtensionLicensingUtil), "No licenses available for extension " + extensionId);
        return false;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(100136269, "Extensions", nameof (ExtensionLicensingUtil), ex);
        return false;
      }
    }

    private static bool IsTestPlansInTrial(
      IVssRequestContext requestContext,
      IEnumerable<IOfferSubscription> offerSubscriptions)
    {
      IOfferSubscription offerSubscription = offerSubscriptions.Where<IOfferSubscription>((Func<IOfferSubscription, bool>) (o => o != null && o.OfferMeter != (OfferMeter) null)).FirstOrDefault<IOfferSubscription>((Func<IOfferSubscription, bool>) (o => o.OfferMeter.GalleryId == "ms.vss-testmanager-web"));
      if (offerSubscription == null)
      {
        requestContext.Trace(2916653, TraceLevel.Info, "Extensions", nameof (ExtensionLicensingUtil), "Test Plans not returned from commerce.");
        return false;
      }
      if (!offerSubscription.IsTrialOrPreview)
        return false;
      DateTime? trialExpiryDate = offerSubscription.TrialExpiryDate;
      DateTime utcNow = DateTime.UtcNow;
      return trialExpiryDate.HasValue && trialExpiryDate.GetValueOrDefault() >= utcNow;
    }

    private static MeterUsage2GetResponse GetTestPlansMeterUsage(IVssRequestContext requestContext)
    {
      MeterUsage2GetResponse meterUsage = requestContext.To(TeamFoundationHostType.Deployment).GetClient<MeterUsage2HttpClient>().GetMeterUsageAsync(requestContext.ServiceHost.InstanceId, AzCommMeterIds.TestManagerMeterId).SyncResult<MeterUsage2GetResponse>();
      if (meterUsage != null)
        requestContext.TraceConditionally(2416768, TraceLevel.Info, "Extensions", nameof (ExtensionLicensingUtil), (Func<string>) (() => "Test Plans meter usage returned => " + meterUsage.Serialize<MeterUsage2GetResponse>()));
      return meterUsage;
    }
  }
}
