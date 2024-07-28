// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Analytics.Server.AnalyticsFeatureService
// Assembly: Microsoft.VisualStudio.Services.Analytics.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E5A0742E-601C-4AD5-8902-781963AA7C5A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Analytics.Sdk.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Analytics.WebApi.Contracts;

namespace Microsoft.TeamFoundation.Analytics.Server
{
  public class AnalyticsFeatureService : IAnalyticsFeatureService, IVssFrameworkService
  {
    private static readonly RegistryQuery FeatureStateQuery = new RegistryQuery("/Service/Analytics/Settings/AnalyticsState");

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public bool IsAnalyticsEnabled(
      IVssRequestContext requestContext,
      bool treatPausedAsEnabled = false,
      bool bypassCache = false)
    {
      AnalyticsState analyticsState = this.GetAnalyticsState(requestContext, bypassCache);
      switch (analyticsState)
      {
        case AnalyticsState.Enabled:
        case AnalyticsState.Preparing:
          return true;
        default:
          return analyticsState == AnalyticsState.Paused & treatPausedAsEnabled;
      }
    }

    public AnalyticsState GetAnalyticsState(IVssRequestContext requestContext, bool bypassCache = false) => !requestContext.ExecutionEnvironment.IsOnPremisesDeployment ? AnalyticsState.Enabled : this.GetOnPremState(requestContext, bypassCache);

    private AnalyticsState GetOnPremState(IVssRequestContext requestContext, bool bypassCache = false) => (!bypassCache ? requestContext.GetService<IVssRegistryService>() : (IVssRegistryService) requestContext.GetService<ISqlRegistryService>()).GetValue<AnalyticsState>(requestContext, new RegistryQuery("/Service/Analytics/Settings/AnalyticsState"), AnalyticsState.Disabled);

    public static bool SubStatusPreventsStaging(ServiceHostSubStatus hostSubStatus)
    {
      if (hostSubStatus <= ServiceHostSubStatus.Importing)
      {
        if (hostSubStatus != ServiceHostSubStatus.Deleting && hostSubStatus != ServiceHostSubStatus.Importing)
          goto label_4;
      }
      else if (hostSubStatus != ServiceHostSubStatus.Moving && hostSubStatus != ServiceHostSubStatus.UpgradeDuringImport)
        goto label_4;
      return true;
label_4:
      return false;
    }
  }
}
