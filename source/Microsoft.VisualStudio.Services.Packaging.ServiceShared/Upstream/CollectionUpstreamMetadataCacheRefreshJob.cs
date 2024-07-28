// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Upstream.CollectionUpstreamMetadataCacheRefreshJob
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.JobManagement;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Framework;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils;
using System;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Upstream
{
  public abstract class CollectionUpstreamMetadataCacheRefreshJob : VssAsyncJobExtension
  {
    public const string RefreshJobPeriodSecondsRegistryKey = "/Configuration/Packaging/{0}/Upstreams/RefreshJobPeriodSeconds";
    public const string RefreshJobPeriodMaxSecondsRegistryKey = "/Configuration/Packaging/{0}/Upstreams/RefreshJobPeriodMaxSeconds";

    public abstract IProtocol Protocol { get; }

    public override sealed async Task<VssJobResult> RunAsync(
      IVssRequestContext requestContext,
      TeamFoundationJobDefinition jobDefinition,
      DateTime queueTime)
    {
      VssJobResult vssJobResult1;
      if (!requestContext.IsFeatureEnabledWithLogging("Packaging.SkipFeedLevelRefreshes"))
        vssJobResult1 = await this.RunInternalAsync(requestContext, jobDefinition, queueTime);
      else
        vssJobResult1 = JobResult.Blocked(new JobTelemetry()
        {
          Message = "Disabled by feature flag Packaging.SkipFeedLevelRefreshes"
        }).ToVssJobResult();
      VssJobResult vssJobResult2 = vssJobResult1;
      new OptionallyRescheduleJobTemplateOffRegistry((IJobQueuer) new JobServiceFacade(requestContext, requestContext.GetService<ITeamFoundationJobService>()), requestContext.GetFeatureFlagFacade(), (IRegistryService) new RegistryServiceFacade(requestContext), (IRandomProvider) new DefaultRandomProvider(), string.Format("/Configuration/Packaging/{0}/Upstreams/RefreshJobPeriodSeconds", (object) this.Protocol.LowercasedName), string.Format("/Configuration/Packaging/{0}/Upstreams/RefreshJobPeriodMaxSeconds", (object) this.Protocol.LowercasedName)).RescheduleJob(jobDefinition);
      return vssJobResult2;
    }

    public abstract Task<VssJobResult> RunInternalAsync(
      IVssRequestContext requestContext,
      TeamFoundationJobDefinition jobDefinition,
      DateTime queueTime);
  }
}
