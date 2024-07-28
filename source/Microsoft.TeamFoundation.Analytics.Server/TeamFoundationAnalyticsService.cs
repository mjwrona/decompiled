// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Analytics.Server.TeamFoundationAnalyticsService
// Assembly: Microsoft.TeamFoundation.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7A426D2C-9BEF-4A84-9FA2-D9A32F46BD7E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Analytics.Server.dll

using Microsoft.TeamFoundation.Analytics.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.TeamFoundation.Analytics.Server
{
  public class TeamFoundationAnalyticsService : ITeamFoundationAnalyticsService, IVssFrameworkService
  {
    public const string StagingFeatureFlagName = "Analytics.Jobs.Release";
    private const int FeatureFlagAnalyticsJobDelaySeconds = 60;
    private const int FeatureFlagAnalyticsJobDelaySecondsDevFabric = 30;
    private static readonly Guid FeatureFlagAnalyticsJobId = new Guid("B92E39DD-2A02-4044-B017-226E33C44187");

    void IVssFrameworkService.ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    void IVssFrameworkService.ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public void UpdateAnalyticsState(IVssRequestContext requestContext, AnalyticsState state)
    {
      requestContext.CheckProjectCollectionRequestContext();
      bool enable = state.Enable;
      bool flag = requestContext.IsFeatureEnabled("Analytics.Jobs.Release");
      requestContext.TraceAlways(15220003, TraceLevel.Info, "Analytics", nameof (UpdateAnalyticsState), string.Format("current feature flag on: {0} / desired feature flag on: {1}. Will update feature flag.", (object) flag, (object) enable));
      if (enable != flag)
      {
        IVssRequestContext vssRequestContext = requestContext.Elevate();
        vssRequestContext.GetService<ITeamFoundationFeatureAvailabilityService>().SetFeatureState(vssRequestContext, "Analytics.Jobs.Release", enable ? FeatureAvailabilityState.On : FeatureAvailabilityState.Off);
      }
      requestContext.GetService<ITeamFoundationJobService>().QueueDelayedJobs(requestContext, (IEnumerable<Guid>) new Guid[1]
      {
        TeamFoundationAnalyticsService.FeatureFlagAnalyticsJobId
      }, requestContext.ExecutionEnvironment.IsDevFabricDeployment ? 30 : 60);
    }
  }
}
