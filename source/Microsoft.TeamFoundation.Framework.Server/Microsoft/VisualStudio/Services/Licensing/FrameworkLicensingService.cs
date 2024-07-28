// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Licensing.FrameworkLicensingService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.Licensing.Utilities;
using Microsoft.VisualStudio.Services.CircuitBreaker;
using Microsoft.VisualStudio.Services.Commerce;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Licensing.Client;
using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Licensing
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  internal class FrameworkLicensingService : ILicensingService, IVssFrameworkService
  {
    private const string _testManagerExtensionId = "ms.vss-testmanager-web";
    private const string s_frameworkLicensingAllowExtensionSpecificShortCircuitFeatureFlag = "VisualStudio.Framework.Licensing.AllowExtensionSpecificShortCircuit";
    private static readonly RegistryQuery extensionRegistryQuery = new RegistryQuery("/Configuration/Licensing/ShortCircuitedExtensions/*");
    private const string s_frameworkLicensingShortCircuitExtensionRightsCheckFeatureFlag = "VisualStudio.Framework.Licensing.ShortCircuitExtensionRightsCheck";
    private const string s_frameworkLicensingUseNewExtensionRightsImplementation = "VisualStudio.Services.UseNewExtensionRightsImplementation";
    private const string s_frameworkLicensingBypassLicensingForNonTestManager = "VisualStudio.Framework.Licensing.BypassLicensingForNonTestManager";
    private const string s_frameworkLicensingUseComputeExtensionRights = "VisualStudio.Framework.Licensing.UseComputeExtensionRights";
    private const string s_area = "VisualStudio.Services.FrameworkLicensingService";
    private const string s_layer = "BusinessLogic";
    private CircuitBreakerSettings m_circuitBreakerSettings;

    public void ServiceStart(IVssRequestContext requestContext) => this.m_circuitBreakerSettings = CircuitBreakerSettings.Default;

    public void ServiceEnd(IVssRequestContext requestContext)
    {
    }

    public virtual ClientRightsContainer GetClientRightsContainer(
      IVssRequestContext requestContext,
      ClientRightsQueryContext queryContext,
      ClientRightsTelemetryContext telemetryContext = null)
    {
      requestContext.CheckDeploymentRequestContext();
      ArgumentUtility.CheckForNull<ClientRightsQueryContext>(queryContext, nameof (queryContext));
      requestContext.TraceEnter(1038010, "VisualStudio.Services.FrameworkLicensingService", "BusinessLogic", nameof (GetClientRightsContainer));
      try
      {
        return this.GetHttpClient(requestContext).GetClientRightsContainerAsync(queryContext, telemetryContext).SyncResult<ClientRightsContainer>();
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1038018, "VisualStudio.Services.FrameworkLicensingService", "BusinessLogic", ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(1038019, "VisualStudio.Services.FrameworkLicensingService", "BusinessLogic", nameof (GetClientRightsContainer));
      }
    }

    public IList<MsdnEntitlement> GetMsdnEntitlements(IVssRequestContext requestContext)
    {
      requestContext.CheckDeploymentRequestContext();
      requestContext.TraceEnter(1038100, "VisualStudio.Services.FrameworkLicensingService", "BusinessLogic", nameof (GetMsdnEntitlements));
      try
      {
        return (IList<MsdnEntitlement>) this.GetHttpClient(requestContext).GetMsdnEntitlementsAsync().SyncResult<IEnumerable<MsdnEntitlement>>().ToList<MsdnEntitlement>();
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1038108, "VisualStudio.Services.FrameworkLicensingService", "BusinessLogic", ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(1038109, "VisualStudio.Services.FrameworkLicensingService", "BusinessLogic", nameof (GetMsdnEntitlements));
      }
    }

    public IDictionary<string, bool> GetExtensionRights(
      IVssRequestContext requestContext,
      IEnumerable<string> extensionIds)
    {
      requestContext.CheckProjectCollectionRequestContext();
      requestContext.TraceEnter(1038150, "VisualStudio.Services.FrameworkLicensingService", "BusinessLogic", nameof (GetExtensionRights));
      requestContext.TraceConditionally(1038163, TraceLevel.Info, "VisualStudio.Services.FrameworkLicensingService", "BusinessLogic", (Func<string>) (() => "Called with " + string.Join(",", extensionIds)));
      try
      {
        if (requestContext.IsSystemContext || requestContext.RootContext != null && requestContext.RootContext.IsSystemContext)
          return this.GenerateExtensionRights(requestContext, extensionIds, true, 1038180);
        if (requestContext.IsFeatureEnabled("VisualStudio.Framework.Licensing.BypassLicensingForNonTestManager") && !extensionIds.Contains<string>("ms.vss-testmanager-web"))
          return ExtensionLicensingUtil.GetExtensionRightsWhenDisabled(requestContext, extensionIds);
        Guid userId = requestContext.GetUserId();
        IList<string> paidExtensionIds;
        IList<string> freeExtensionIds;
        this.DetermineFreeExtensions(requestContext, extensionIds, userId, out paidExtensionIds, out freeExtensionIds);
        if (userId == Guid.Empty)
          return (IDictionary<string, bool>) this.GenerateExtensionRights(requestContext, (IEnumerable<string>) paidExtensionIds, false, 1038181).Union<KeyValuePair<string, bool>>((IEnumerable<KeyValuePair<string, bool>>) this.GenerateExtensionRights(requestContext, (IEnumerable<string>) freeExtensionIds, true, 1038182)).ToDictionary<KeyValuePair<string, bool>, string, bool>((Func<KeyValuePair<string, bool>, string>) (pair => pair.Key), (Func<KeyValuePair<string, bool>, bool>) (pair => pair.Value));
        Guid accountId = requestContext.ServiceHost.InstanceId;
        requestContext.TraceConditionally(1038151, TraceLevel.Info, "VisualStudio.Services.FrameworkLicensingService", "BusinessLogic", (Func<string>) (() => string.Format("Filtered free extensions for AccountId: {0}, UserId: {1}, FreeExtensionIds: {2}", (object) accountId, (object) userId, (object) string.Join(",", (IEnumerable<string>) freeExtensionIds))));
        VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_FrameworkLicensingService_ExtensionRightsRequests").Increment();
        VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_FrameworkLicensingService_ExtensionRightsRequestsPerSec").Increment();
        if (requestContext.IsAnonymousPrincipal() || requestContext.IsPublicUser())
          return this.GenerateExtensionRights(requestContext, (IEnumerable<string>) freeExtensionIds, true, 1038190);
        if (!paidExtensionIds.Any<string>())
          return this.GenerateExtensionRights(requestContext, extensionIds, true, 1038183);
        if (requestContext.IsFeatureEnabled("VisualStudio.Framework.Licensing.ShortCircuitExtensionRightsCheck"))
          return this.GenerateExtensionRights(requestContext, extensionIds, true, 1038184);
        IDictionary<string, bool> listedExtensions = this.GetWhiteListedExtensions(requestContext, userId, accountId);
        List<string> onUserAccessLevel = LicensingExtensionEntitlementHelper.GetExtensionAssignmentBasedOnUserAccessLevel(requestContext, userId);
        requestContext.Trace(1034275, TraceLevel.Info, "VisualStudio.Services.FrameworkLicensingService", "BusinessLogic", string.Format("This user:{0} has access to the following extensions due to their license. ExtensionIds: {1}", (object) userId, (object) string.Join(",", (IEnumerable<string>) onUserAccessLevel)));
        List<string> filteredPaidExtensionIds = paidExtensionIds.Except<string>((IEnumerable<string>) listedExtensions.Keys, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase).Except<string>((IEnumerable<string>) onUserAccessLevel).ToList<string>();
        if (!filteredPaidExtensionIds.Any<string>())
          return this.GenerateExtensionRights(requestContext, extensionIds, true, 1038185);
        requestContext.TraceConditionally(1038153, TraceLevel.Info, "VisualStudio.Services.FrameworkLicensingService", "BusinessLogic", (Func<string>) (() => string.Format("Looking up cache for AccountId: {0}, UserId: {1}, ExtensionIds: {2}", (object) accountId, (object) userId, (object) string.Join(",", (IEnumerable<string>) filteredPaidExtensionIds))));
        IExtensionRightsCacheService service = requestContext.GetService<IExtensionRightsCacheService>();
        try
        {
          ExtensionRightsResult setExtensionRights = service.GetOrSetExtensionRights(requestContext, new ExtensionRightsCacheKey(accountId, userId, requestContext.IsPublicResourceLicense()), new Func<IVssRequestContext, IEnumerable<string>, ExtensionRightsResult>(this.EvaluateExtensionRights), extensionIds);
          if (setExtensionRights != null && setExtensionRights.ResultCode == ExtensionRightsResultCode.Normal && setExtensionRights?.EntitledExtensions == null)
            return this.GenerateExtensionRights(requestContext, extensionIds, true, 1038186);
          Dictionary<string, bool> extensionRights = this.TranslateExtensionRightsResult(requestContext, setExtensionRights, (IList<string>) filteredPaidExtensionIds).Union<KeyValuePair<string, bool>>((IEnumerable<KeyValuePair<string, bool>>) this.GenerateExtensionRights(requestContext, (IEnumerable<string>) freeExtensionIds, true, 1038187)).Union<KeyValuePair<string, bool>>((IEnumerable<KeyValuePair<string, bool>>) listedExtensions).ToDictionary<KeyValuePair<string, bool>, string, bool>((Func<KeyValuePair<string, bool>, string>) (pair => pair.Key), (Func<KeyValuePair<string, bool>, bool>) (pair => pair.Value));
          requestContext.TraceConditionally(1038157, TraceLevel.Info, "VisualStudio.Services.FrameworkLicensingService", "BusinessLogic", (Func<string>) (() => string.Join(" | ", extensionRights.Select<KeyValuePair<string, bool>, string>((Func<KeyValuePair<string, bool>, string>) (ext => string.Format("{0}:{1}", (object) ext.Key, (object) ext.Value))))));
          return (IDictionary<string, bool>) extensionRights;
        }
        catch (NotSupportedException ex)
        {
          return (IDictionary<string, bool>) this.GenerateExtensionRights(requestContext, (IEnumerable<string>) paidExtensionIds, false, 1038188).Union<KeyValuePair<string, bool>>((IEnumerable<KeyValuePair<string, bool>>) this.GenerateExtensionRights(requestContext, (IEnumerable<string>) freeExtensionIds, true, 1038189)).ToDictionary<KeyValuePair<string, bool>, string, bool>((Func<KeyValuePair<string, bool>, string>) (pair => pair.Key), (Func<KeyValuePair<string, bool>, bool>) (pair => pair.Value));
        }
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1038158, "VisualStudio.Services.FrameworkLicensingService", "BusinessLogic", ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(1038159, "VisualStudio.Services.FrameworkLicensingService", "BusinessLogic", nameof (GetExtensionRights));
      }
    }

    private IDictionary<string, bool> TranslateExtensionRightsResult(
      IVssRequestContext requestContext,
      ExtensionRightsResult paidExtensionRightsResult,
      IList<string> paidExtensionIds)
    {
      if (paidExtensionRightsResult.ResultCode == ExtensionRightsResultCode.AllFree)
        return this.GenerateExtensionRights(requestContext, (IEnumerable<string>) paidExtensionIds, true, 1038190);
      if (paidExtensionRightsResult.ResultCode == ExtensionRightsResultCode.FreeExtensionsFree)
        return this.GenerateExtensionRights(requestContext, (IEnumerable<string>) new List<string>(), true, 1038191);
      Dictionary<string, bool> translatedRights = paidExtensionIds.ToDictionary<string, string, bool>((Func<string, string>) (ext => ext), (Func<string, bool>) (ext => paidExtensionRightsResult.EntitledExtensions.Contains(ext)));
      requestContext.TraceConditionally(1038192, TraceLevel.Info, "VisualStudio.Services.FrameworkLicensingService", "BusinessLogic", (Func<string>) (() => string.Format("Translated ExtensionRights for AccountId: {0}, UserId: {1}, Rights: {2}", (object) requestContext.ServiceHost.InstanceId, (object) requestContext.GetUserId(), (object) string.Join(" | ", translatedRights.Select<KeyValuePair<string, bool>, string>((Func<KeyValuePair<string, bool>, string>) (ext => string.Format("{0}:{1}", (object) ext.Key, (object) ext.Value)))))));
      return (IDictionary<string, bool>) translatedRights;
    }

    private void DetermineFreeExtensions(
      IVssRequestContext requestContext,
      IEnumerable<string> extensionIds,
      Guid userId,
      out IList<string> paidExtensionIds,
      out IList<string> freeExtensionIds)
    {
      AccountEntitlement accountEntitlement = requestContext.GetAccountEntitlement(userId);
      if ((accountEntitlement?.License == (License) AccountLicense.EarlyAdopter || accountEntitlement?.License == (License) AccountLicense.Advanced || accountEntitlement?.License == (License) MsdnLicense.Enterprise ? 1 : (requestContext.IsPublicResourceLicense() ? 1 : 0)) != 0)
        OfferMeters.TrySplitExtensions(requestContext, extensionIds, out paidExtensionIds, out freeExtensionIds);
      else
        OfferMeters.TrySplitExtensionsToFreeAndPaid(requestContext, extensionIds, out paidExtensionIds, out freeExtensionIds);
    }

    private ExtensionRightsResult EvaluateExtensionRights(
      IVssRequestContext requestContext,
      IEnumerable<string> extensionIds)
    {
      if (ExtensionLicensingUtil.IsExtensionLicensingDisabled(requestContext))
      {
        IEnumerable<string> collection = ExtensionLicensingUtil.GetExtensionRightsWhenDisabled(requestContext, extensionIds).Where<KeyValuePair<string, bool>>((Func<KeyValuePair<string, bool>, bool>) (pair => pair.Value)).Select<KeyValuePair<string, bool>, string>((Func<KeyValuePair<string, bool>, string>) (pair => pair.Key));
        return new ExtensionRightsResult()
        {
          HostId = requestContext.ServiceHost.InstanceId,
          ReasonCode = ExtensionRightsReasonCode.Normal,
          ResultCode = ExtensionRightsResultCode.Normal,
          EntitledExtensions = new HashSet<string>(collection, (IEqualityComparer<string>) StringComparer.Ordinal)
        };
      }
      requestContext.Trace(1038160, TraceLevel.Info, "VisualStudio.Services.FrameworkLicensingService", "BusinessLogic", "Calling LicensingService through CircuitBreaker");
      CommandSetter setter = CommandSetter.WithGroupKey((CommandGroupKey) "Framework.").AndCommandKey((CommandKey) this.m_circuitBreakerSettings.CommandKeyForGetExtensionRights).AndCommandPropertiesDefaults(this.m_circuitBreakerSettings.CircuitBreakerSettingsForOutBoundCallToLicensing);
      return new CommandService<ExtensionRightsResult>(requestContext, setter, (Func<ExtensionRightsResult>) (() =>
      {
        IAccountLicensingHttpClient licensingHttpClient = this.GetAccountLicensingHttpClient(requestContext);
        ExtensionRightsResult extensionRightsResult;
        if (requestContext.IsFeatureEnabled("VisualStudio.Framework.Licensing.UseComputeExtensionRights"))
        {
          requestContext.Trace(1038164, TraceLevel.Info, "VisualStudio.Services.FrameworkLicensingService", "BusinessLogic", "Calling ComputeExtensionRights with extension ids: " + string.Join(", ", extensionIds));
          IDictionary<string, bool> result = licensingHttpClient.ComputeExtensionRightsAsync(extensionIds).Result;
          if (result == null)
          {
            requestContext.TraceAlways(1038166, TraceLevel.Error, "VisualStudio.Services.FrameworkLicensingService", "BusinessLogic", "Extension rights result dictionary returned null. Giving all extensions free.");
            return new ExtensionRightsResult()
            {
              ReasonCode = ExtensionRightsReasonCode.ErrorCallingService,
              ResultCode = ExtensionRightsResultCode.AllFree,
              Reason = "Tracepoint 1038166. Extension rights dictionary returned null."
            };
          }
          IEnumerable<string> collection = result.Where<KeyValuePair<string, bool>>((Func<KeyValuePair<string, bool>, bool>) (pair => pair.Value)).Select<KeyValuePair<string, bool>, string>((Func<KeyValuePair<string, bool>, string>) (pair => pair.Key));
          extensionRightsResult = new ExtensionRightsResult()
          {
            HostId = requestContext.ServiceHost.InstanceId,
            ReasonCode = ExtensionRightsReasonCode.Normal,
            ResultCode = ExtensionRightsResultCode.Normal,
            EntitledExtensions = new HashSet<string>(collection, (IEqualityComparer<string>) StringComparer.Ordinal)
          };
        }
        else
          extensionRightsResult = licensingHttpClient.GetExtensionRightsAsync((object) extensionIds).Result;
        requestContext.TraceConditionally(1038165, TraceLevel.Info, "VisualStudio.Services.FrameworkLicensingService", "BusinessLogic", (Func<string>) (() => JsonConvert.SerializeObject((object) extensionRightsResult)));
        return extensionRightsResult;
      }), (Func<ExtensionRightsResult>) (() =>
      {
        int tracepoint = 1038171;
        requestContext.TraceAlways(tracepoint, TraceLevel.Error, "VisualStudio.Services.FrameworkLicensingService", "BusinessLogic", string.Format("Failed Calling ExtensionRights. AccountId: {0}, UserId: {1}", (object) requestContext.ServiceHost.InstanceId, (object) requestContext.GetUserId()));
        return new ExtensionRightsResult()
        {
          ReasonCode = ExtensionRightsReasonCode.ErrorCallingService,
          ResultCode = ExtensionRightsResultCode.AllFree,
          Reason = string.Format("{0}. Circuit Breaker fallback.", (object) tracepoint)
        };
      })).Execute();
    }

    private IDictionary<string, bool> GenerateExtensionRights(
      IVssRequestContext requestContext,
      IEnumerable<string> extensionIds,
      bool defaultRight,
      int tracepoint)
    {
      requestContext.TraceConditionally(tracepoint, TraceLevel.Info, "VisualStudio.Services.FrameworkLicensingService", "BusinessLogic", (Func<string>) (() => string.Format("Generated ExtensionRights for AccountId: {0}, UserId: {1}, ExtensionIds: {2}, rights {3}", (object) requestContext.ServiceHost.InstanceId, (object) requestContext.GetUserId(), (object) string.Join(",", extensionIds), (object) defaultRight.ToString())));
      return (IDictionary<string, bool>) extensionIds.ToDictionary<string, string, bool>((Func<string, string>) (k => k), (Func<string, bool>) (v => defaultRight));
    }

    internal virtual LicensingHttpClient GetHttpClient(IVssRequestContext requestContext) => requestContext.RootContext != null && requestContext.RootContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection) ? requestContext.RootContext.GetClient<LicensingHttpClient>() : requestContext.GetClient<LicensingHttpClient>();

    private protected virtual IAccountLicensingHttpClient GetAccountLicensingHttpClient(
      IVssRequestContext requestContext)
    {
      return requestContext.RootContext != null && requestContext.RootContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection) ? requestContext.RootContext.GetAccountLicensingHttpClient() : requestContext.GetAccountLicensingHttpClient();
    }

    private IDictionary<string, bool> GetWhiteListedExtensions(
      IVssRequestContext requestContext,
      Guid userId,
      Guid accountId)
    {
      IDictionary<string, bool> whitelistedExtensionIds;
      if (requestContext.IsFeatureEnabled("VisualStudio.Framework.Licensing.AllowExtensionSpecificShortCircuit"))
      {
        whitelistedExtensionIds = this.GetWhitelistedExtensions(requestContext);
        requestContext.TraceConditionally(1038161, TraceLevel.Info, "VisualStudio.Services.FrameworkLicensingService", "BusinessLogic", (Func<string>) (() => string.Format("Short Circuiting Extension Rights Check AccountId: {0}, UserId: {1}, ExtensionIds: {2}", (object) accountId, (object) userId, (object) string.Join<KeyValuePair<string, bool>>(",", (IEnumerable<KeyValuePair<string, bool>>) whitelistedExtensionIds))));
      }
      else
        whitelistedExtensionIds = (IDictionary<string, bool>) new Dictionary<string, bool>();
      return whitelistedExtensionIds;
    }

    private IDictionary<string, bool> GetWhitelistedExtensions(IVssRequestContext requestContext)
    {
      IEnumerable<string> collectionExtensionWhitelist = this.GetShortCircuitedExtensionsFromRegistry(requestContext);
      IEnumerable<string> deploymentExtensionWhitelist = this.GetShortCircuitedExtensionsFromRegistry(requestContext.To(TeamFoundationHostType.Deployment));
      requestContext.TraceConditionally(1038162, TraceLevel.Info, "VisualStudio.Services.FrameworkLicensingService", "BusinessLogic", (Func<string>) (() => "Whitelisted at deployment level: " + string.Join(", ", deploymentExtensionWhitelist) + " | at collection level: " + string.Join(", ", collectionExtensionWhitelist)));
      return (IDictionary<string, bool>) collectionExtensionWhitelist.Union<string>(deploymentExtensionWhitelist, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase).ToDictionary<string, string, bool>((Func<string, string>) (k => k), (Func<string, bool>) (v => true));
    }

    private IEnumerable<string> GetShortCircuitedExtensionsFromRegistry(
      IVssRequestContext requestContext)
    {
      return requestContext.GetService<IVssRegistryService>().Read(requestContext, in FrameworkLicensingService.extensionRegistryQuery).Where<RegistryItem>((Func<RegistryItem, bool>) (v => v.Value == "1")).Select<RegistryItem, string>((Func<RegistryItem, string>) (v => v.Path)).Select<string, string>((Func<string, string>) (v => v.Substring(v.LastIndexOf('/') + 1)));
    }
  }
}
