// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.PublicRepositories.DeploymentPackageUpstreamRefreshJobHandler`1
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Feed.Common;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.JobManagement;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils;
using System;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.PublicRepositories
{
  public class DeploymentPackageUpstreamRefreshJobHandler<TPackageName> : 
    IHandler<DeploymentPackageUpstreamRefreshJobData, JobResult>
    where TPackageName : class, IPackageName
  {
    private readonly IPublicRepositoryInterestTracker<TPackageName> interestTracker;
    private readonly IConverter<string, TPackageName> nameResolver;
    private readonly ITracerService tracerService;
    private readonly IFactory<CollectionId, IVssRequestContext> collectionContextFactory;
    private readonly IPerCollectionBootstrapper<ICollectionPackageUpstreamRefreshJobQueuer> perCollectionBootstrapper;

    public DeploymentPackageUpstreamRefreshJobHandler(
      IFactory<CollectionId, IVssRequestContext> collectionContextFactory,
      ITracerService tracerService,
      IConverter<string, TPackageName> nameResolver,
      IPublicRepositoryInterestTracker<TPackageName> interestTracker,
      IPerCollectionBootstrapper<ICollectionPackageUpstreamRefreshJobQueuer> perCollectionBootstrapper)
    {
      this.collectionContextFactory = collectionContextFactory;
      this.tracerService = tracerService;
      this.nameResolver = nameResolver;
      this.interestTracker = interestTracker;
      this.perCollectionBootstrapper = perCollectionBootstrapper;
    }

    public JobResult Handle(DeploymentPackageUpstreamRefreshJobData data)
    {
      using (ITracerBlock tracerBlock = this.tracerService.Enter((object) this, nameof (Handle)))
      {
        DeploymentPackageUpstreamRefreshJobTelemetry telemetry = new DeploymentPackageUpstreamRefreshJobTelemetry();
        string upstreamLocation = data.PushDrivenUpstreamsTelemetry.ExternalUpstreamLocation;
        WellKnownUpstreamSource source1 = upstreamLocation != null ? WellKnownSourceProvider.Instance.GetWellKnownSourceOrDefault(upstreamLocation) : throw new InvalidOperationException("Deployment-level package refresh job requires external upstream location");
        if ((object) source1 == null)
          throw new InvalidOperationException("Could not find well-known source for location " + upstreamLocation);
        TPackageName packageName = this.nameResolver.Convert(data.PackageName);
        foreach (IGrouping<CollectionId, FeedInterestedInPackage> source2 in this.interestTracker.GetFeedsInterestedInPackage(packageName, source1).GroupBy<FeedInterestedInPackage, CollectionId>((Func<FeedInterestedInPackage, CollectionId>) (x => x.Collection)))
        {
          ++telemetry.InterestedCollectionCount;
          telemetry.InterestedFeedCount += source2.Count<FeedInterestedInPackage>();
          try
          {
            using (IVssRequestContext collectionRequestContext = this.collectionContextFactory.Get(source2.Key))
            {
              this.perCollectionBootstrapper.Bootstrap(collectionRequestContext).QueuePackageJob(packageName.NormalizedName, source2.Select<FeedInterestedInPackage, Guid>((Func<FeedInterestedInPackage, Guid>) (x => x.Feed.Guid)), data.PushDrivenUpstreamsTelemetry);
              ++telemetry.SucceededCollectionCount;
            }
          }
          catch (HostShutdownException ex)
          {
            ++telemetry.ShutdownCollectionCount;
            tracerBlock.TraceInfoAlways(string.Format("Can't queue collection-level job on shut-down collection {0}: {1}", (object) source2.Key, (object) StackTraceCompressor.CompressStackTrace(ex.ToString())));
          }
          catch (HostDoesNotExistException ex)
          {
            ++telemetry.NonexistentCollectionCount;
            tracerBlock.TraceInfoAlways(string.Format("Can't queue collection-level job on nonexistent collection {0}: {1}", (object) source2.Key, (object) StackTraceCompressor.CompressStackTrace(ex.ToString())));
          }
          catch (Exception ex)
          {
            ++telemetry.FailedCollectionCount;
            telemetry.LogException(ex);
            tracerBlock.TraceException(ex);
          }
        }
        return telemetry.FailedCollectionCount == 0 ? JobResult.Succeeded((JobTelemetry) telemetry) : JobResult.Failed((JobTelemetry) telemetry);
      }
    }
  }
}
