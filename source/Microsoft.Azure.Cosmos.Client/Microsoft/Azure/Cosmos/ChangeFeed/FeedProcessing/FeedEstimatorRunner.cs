// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.ChangeFeed.FeedProcessing.FeedEstimatorRunner
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Cosmos.ChangeFeed.FeedProcessing
{
  internal sealed class FeedEstimatorRunner
  {
    private static readonly string EstimationLeaseIdentifier = "Change Feed Estimator";
    private static TimeSpan defaultMonitoringDelay = TimeSpan.FromSeconds(5.0);
    private readonly ChangeFeedEstimator remainingWorkEstimator;
    private readonly TimeSpan monitoringDelay;
    private readonly Container.ChangesEstimationHandler dispatchEstimation;
    private readonly ChangeFeedProcessorHealthMonitor healthMonitor;

    public FeedEstimatorRunner(
      Container.ChangesEstimationHandler dispatchEstimation,
      ChangeFeedEstimator remainingWorkEstimator,
      ChangeFeedProcessorHealthMonitor healthMonitor,
      TimeSpan? estimationPeriod = null)
    {
      this.dispatchEstimation = dispatchEstimation;
      this.remainingWorkEstimator = remainingWorkEstimator;
      this.healthMonitor = healthMonitor;
      this.monitoringDelay = estimationPeriod ?? FeedEstimatorRunner.defaultMonitoringDelay;
    }

    public async Task RunAsync(CancellationToken cancellationToken)
    {
      while (!cancellationToken.IsCancellationRequested)
      {
        try
        {
          await this.EstimateAsync(cancellationToken);
        }
        catch (OperationCanceledException ex)
        {
          if (cancellationToken.IsCancellationRequested)
            throw;
          else
            Extensions.TraceException(new Exception("exception within estimator", (Exception) ex));
        }
        await Task.Delay(this.monitoringDelay, cancellationToken).ConfigureAwait(false);
      }
    }

    private async Task EstimateAsync(CancellationToken cancellationToken)
    {
      try
      {
        await this.dispatchEstimation(await this.GetEstimatedRemainingWorkAsync(cancellationToken).ConfigureAwait(false), cancellationToken).ConfigureAwait(false);
      }
      catch (Exception ex)
      {
        await this.healthMonitor.NotifyErrorAsync(FeedEstimatorRunner.EstimationLeaseIdentifier, ex);
      }
    }

    private async Task<long> GetEstimatedRemainingWorkAsync(CancellationToken cancellationToken)
    {
      long remainingWorkAsync;
      using (FeedIterator<ChangeFeedProcessorState> feedIterator = this.remainingWorkEstimator.GetCurrentStateIterator())
      {
        FeedResponse<ChangeFeedProcessorState> source = await feedIterator.ReadNextAsync(cancellationToken).ConfigureAwait(false);
        remainingWorkAsync = source.Count != 0 ? source.Sum<ChangeFeedProcessorState>((Func<ChangeFeedProcessorState, long>) (estimation => estimation.EstimatedLag)) : 1L;
      }
      return remainingWorkAsync;
    }
  }
}
