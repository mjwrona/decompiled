// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.CrossProtocol.Server.Controllers.PackagesIngestedUpstream.PackagesIngestedUpstreamControllerHandler
// Assembly: Microsoft.VisualStudio.Services.Packaging.CrossProtocol.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9BF15B3F-0578-4452-9C4B-B5237E218DF3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.CrossProtocol.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Packaging.CrossProtocol.Internal.WebApi.Types;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Packaging.CrossProtocol.Server.Controllers.PackagesIngestedUpstream
{
  public class PackagesIngestedUpstreamControllerHandler
  {
    public static readonly RegistryQuery FeedsPerJobRegistryQuery = (RegistryQuery) "/Configuration/Packaging/Upstreams/FeedsPerPackageJob";
    public const int DefaultFeedsPerJob = 10;
    private readonly IFactory<IProtocol, ICollectionPackageUpstreamRefreshJobQueuer> jobQueuerFactory;
    private readonly ITracerService tracerService;
    private readonly IPackagingTraces packagingTraces;
    private readonly ITimeProvider timeProvider;
    private readonly IExecutionEnvironment executionEnvironment;
    private readonly IRegistryService registryService;

    public PackagesIngestedUpstreamControllerHandler(
      IFactory<IProtocol, ICollectionPackageUpstreamRefreshJobQueuer> jobQueuerFactory,
      ITracerService tracerService,
      IPackagingTraces packagingTraces,
      ITimeProvider timeProvider,
      IExecutionEnvironment executionEnvironment,
      IRegistryService registryService)
    {
      this.jobQueuerFactory = jobQueuerFactory;
      this.tracerService = tracerService;
      this.packagingTraces = packagingTraces;
      this.timeProvider = timeProvider;
      this.executionEnvironment = executionEnvironment;
      this.registryService = registryService;
    }

    public void Handle(
      IProtocol proto,
      NotifyOfPackagesIngestedInUpstreamsParameters triggerParameters)
    {
      using (ITracerBlock tracerBlock = this.tracerService.Enter((object) this, nameof (Handle)))
      {
        int num1 = 0;
        int num2 = 0;
        ICollectionPackageUpstreamRefreshJobQueuer refreshJobQueuer = this.jobQueuerFactory.Get(proto);
        PushDrivenUpstreamsNotificationTelemetry upstreamsTelemetry = triggerParameters.PushDrivenUpstreamsTelemetry;
        upstreamsTelemetry.FeedToDownstreamTriggerActivityId = new Guid?(this.executionEnvironment.ActivityId);
        upstreamsTelemetry.FeedToDownstreamTriggerTimestamp = new DateTime?(this.timeProvider.Now);
        int pageSize = this.registryService.GetValue<int>(PackagesIngestedUpstreamControllerHandler.FeedsPerJobRegistryQuery, 10);
        foreach (List<Guid> page in triggerParameters.DownstreamFeeds.GetPages<Guid>(pageSize))
        {
          try
          {
            refreshJobQueuer.QueuePackageJob(triggerParameters.PackageName, (IEnumerable<Guid>) page, upstreamsTelemetry);
            ++num1;
          }
          catch (Exception ex)
          {
            ++num2;
            tracerBlock.TraceException(ex);
          }
        }
        this.packagingTraces.AddProperty("SucceededDownstreamJobCreation", (object) num1);
        this.packagingTraces.AddProperty("FailedDownstreamJobCreation", (object) num2);
      }
    }
  }
}
