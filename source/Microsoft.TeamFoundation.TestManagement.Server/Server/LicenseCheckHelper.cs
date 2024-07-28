// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.LicenseCheckHelper
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.BusinessIntelligence;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using Microsoft.VisualStudio.Services.AzComm.SharedContracts;
using Microsoft.VisualStudio.Services.AzComm.WebApi.Contracts;
using Microsoft.VisualStudio.Services.AzComm.WebApi.HttpClients;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  public static class LicenseCheckHelper
  {
    private const string c_fetchTrialInfoFromCommerce = "TestManagement.Server.FetchTrialInfoFromCommerce";
    private const string c_testManagerExtensionRightsKey = "testmanager-extension-rights";
    private const string c_area = "Licensing";
    private const string c_layer = "LicensingHelpers";

    public static void ValidateIfUserHasTestManagementAdvancedAccess(
      IVssRequestContext requestContext)
    {
      if (!LicenseCheckHelper.DoesUserHaveTestManagementAdvancedAccess(requestContext))
        throw new UnauthorizedAccessException(ServerResources.NotAuthorizedToAccessApi);
    }

    public static void ValidateTestSessionSource(
      IVssRequestContext requestContext,
      TestSessionSource testSessionSource)
    {
      switch (testSessionSource)
      {
        case TestSessionSource.FeedbackDesktop:
        case TestSessionSource.FeedbackWeb:
          if (CommonLicenseCheckHelper.IsStakeholder(requestContext))
            break;
          throw new UnauthorizedAccessException(ServerResources.NotAuthorizedToAccessApi);
        case TestSessionSource.XTWeb:
          if (!CommonLicenseCheckHelper.IsStakeholder(requestContext))
            break;
          throw new UnauthorizedAccessException(ServerResources.NotAuthorizedToAccessApi);
        default:
          if (LicenseCheckHelper.DoesUserHaveTestManagementBasicAccess(requestContext))
            break;
          throw new UnauthorizedAccessException(ServerResources.NotAuthorizedToAccessApi);
      }
    }

    public static void ValidateRightsToGetSession(
      IVssRequestContext requestContext,
      TestSessionSource testSessionSource)
    {
      if (testSessionSource != TestSessionSource.FeedbackDesktop && testSessionSource != TestSessionSource.FeedbackWeb && !LicenseCheckHelper.DoesUserHaveTestManagementBasicAccess(requestContext))
        throw new UnauthorizedAccessException(ServerResources.NotAuthorizedToAccessApi);
    }

    private static bool DoesUserHaveTestManagementAdvancedAccess(IVssRequestContext requestContext)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      if (LicenseCheckHelper.CheckIfUserIsServiceIdentity(requestContext))
        return true;
      if (!LicenseCheckHelper.IsAdvancedTestExtensionEnabled(requestContext))
        return requestContext.To(TeamFoundationHostType.Application).GetService<ITeamFoundationLicensingService>().IsFeatureSupported(requestContext, LicenseFeatures.TestManagementId);
      return !requestContext.IsFeatureEnabled("TestManagement.Server.DisableTestPlanAccessWithTestManagerExtension");
    }

    private static bool DoesUserHaveTestManagementBasicAccess(IVssRequestContext requestContext)
    {
      if (requestContext == null)
        throw new ArgumentNullException(nameof (requestContext));
      return LicenseCheckHelper.CheckIfUserIsServiceIdentity(requestContext) || requestContext.To(TeamFoundationHostType.Application).GetService<ITeamFoundationLicensingService>().IsFeatureSupported(requestContext, LicenseFeatures.TestManagementForBasicUsersId);
    }

    private static bool CheckIfUserIsServiceIdentity(IVssRequestContext requestContext)
    {
      Microsoft.VisualStudio.Services.Identity.Identity userIdentity = requestContext.GetUserIdentity();
      return userIdentity != null && userIdentity.Descriptor != (IdentityDescriptor) null && userIdentity.Descriptor.IdentityType == "Microsoft.TeamFoundation.ServiceIdentity";
    }

    public static bool IsAdvancedTestExtensionEnabled(IVssRequestContext requestContext)
    {
      ClientTraceData properties = new ClientTraceData();
      ClientTraceService service = requestContext.GetService<ClientTraceService>();
      Stopwatch stopwatch = Stopwatch.StartNew();
      Dictionary<string, object> data = new Dictionary<string, object>();
      bool flag;
      if (!requestContext.IsFeatureEnabled("TestManagement.Server.FetchTrialInfoFromCommerce"))
      {
        flag = requestContext.GetService<IInstalledExtensionManager>().IsExtensionActive(requestContext, "ms", "vss-testmanager-web");
        properties.Add("TotalTimeForLicensingCallMs", (object) stopwatch.ElapsedMilliseconds);
        service.Publish(requestContext, "TestManagement", "CommerceApiPerf", properties);
        data.Add("AdvancedTestExtensionEnabler", (object) "ActiveExtension");
        data.Add("AdvancedTestExtensionEnablerState", (object) flag);
      }
      else if (!requestContext.Items.TryGetValue<bool>("testmanager-extension-rights", out flag))
      {
        MeterUsage2HttpClient client = requestContext.To(TeamFoundationHostType.Deployment).GetClient<MeterUsage2HttpClient>();
        Guid testManagerMeterId = AzCommMeterIds.TestManagerMeterId;
        Guid instanceId = requestContext.ServiceHost.InstanceId;
        Guid meterId = testManagerMeterId;
        CancellationToken cancellationToken = new CancellationToken();
        flag = client.GetMeterUsageAsync(instanceId, meterId, cancellationToken: cancellationToken).SyncResult<MeterUsage2GetResponse>().IsInTrial;
        requestContext.Items["testmanager-extension-rights"] = (object) flag;
        properties.Add("TotalTimeForCommerceV2CallMs", (object) stopwatch.ElapsedMilliseconds);
        service.Publish(requestContext, "TestManagement", "CommerceApiPerf", properties);
        data.Add("AdvancedTestExtensionEnabler", (object) "TrialExtension");
        data.Add("AdvancedTestExtensionEnablerState", (object) flag);
      }
      CustomerIntelligenceData cid = new CustomerIntelligenceData((IDictionary<string, object>) data);
      TelemetryLogger.Instance.PublishData(requestContext, nameof (IsAdvancedTestExtensionEnabled), cid);
      return flag;
    }
  }
}
