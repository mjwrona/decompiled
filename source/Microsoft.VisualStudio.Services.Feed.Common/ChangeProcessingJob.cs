// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.Common.ChangeProcessingJob
// Assembly: Microsoft.VisualStudio.Services.Feed.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AAC6BCA4-7F6C-4DFE-8058-1CCDD886477F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Feed.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Feed.Common.ChangeProcessing.Telemetry;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Feed.Common
{
  public abstract class ChangeProcessingJob : VssAsyncJobExtension
  {
    public abstract IEnumerable<IChangeProcessorProvider> ChangeProcessors { get; }

    public abstract IChangeProvider ChangeProvider { get; }

    public abstract ITokenProvider TokenProvider { get; }

    public abstract DateTime JobStartTime { get; }

    public override async Task<VssJobResult> RunAsync(
      IVssRequestContext requestContext,
      TeamFoundationJobDefinition jobDefinition,
      DateTime queueTime)
    {
      if (this.ChangeProcessors == null)
        throw new ArgumentException("ChangeProcessors is null");
      if (this.ChangeProvider == null)
        throw new ArgumentException("ChangeProvider is null");
      if (this.TokenProvider == null)
        throw new ArgumentException("TokenProvider is null");
      if (!this.ChangeProcessors.Any<IChangeProcessorProvider>())
        throw new ArgumentException("ChangeProcessors must have at least one IChangeProcessorProvider.", "ChangeProcessors");
      JobTelemetryData jobTelemetryData = new JobTelemetryData()
      {
        JobStartTime = this.JobStartTime,
        CollectionId = requestContext.ServiceHost.InstanceId,
        FeedCountBatchSize = this.ChangeProvider.BatchSize,
        FeedUpdateGracePeriod = this.ChangeProvider.GracePeriod
      };
      string token = this.TokenProvider.GetToken();
      jobTelemetryData.InitialContinuationToken = token;
      Changes changes = this.ChangeProvider.GetChanges(token);
      if (!changes.ItemChanges.Any<IChange>())
        return new VssJobResult(TeamFoundationJobExecutionResult.Succeeded, string.Format("Changes are up to date. Nothing to do."));
      bool[] flagArray = await Task.WhenAll<bool>(this.ChangeProcessors.Select<IChangeProcessorProvider, Task<bool>>((Func<IChangeProcessorProvider, Task<bool>>) (cp => cp.ProcessChangesAsync(changes))));
      if (!((IEnumerable<bool>) flagArray).All<bool>((Func<bool, bool>) (r => r)))
        return new VssJobResult(TeamFoundationJobExecutionResult.Failed, string.Format("Failed to process the {0} changes. Change processors: '{1}' completed with the following status '{2}'.", (object) changes.ItemChanges.Count<IChange>(), (object) string.Join<IChangeProcessorProvider>(", ", this.ChangeProcessors), (object) string.Join<bool>(", ", (IEnumerable<bool>) flagArray)));
      this.TokenProvider.StoreToken(changes.Token);
      jobTelemetryData.FinalContinuationToken = changes.Token;
      jobTelemetryData.ChangesProcessed = changes.ItemChanges.LongCount<IChange>();
      jobTelemetryData.FeedsProcessed = new HashSet<Guid>(changes.ItemChanges.Select<IChange, Guid>((Func<IChange, Guid>) (c => c.Feed.Id))).ToArray<Guid>();
      return new VssJobResult(TeamFoundationJobExecutionResult.Succeeded, JsonConvert.SerializeObject((object) jobTelemetryData));
    }
  }
}
