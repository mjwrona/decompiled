// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.ChangeFeed.ChangeFeedEstimatorIterator
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.ChangeFeed.LeaseManagement;
using Microsoft.Azure.Cosmos.ChangeFeed.Utils;
using Microsoft.Azure.Cosmos.Core.Trace;
using Microsoft.Azure.Cosmos.Diagnostics;
using Microsoft.Azure.Cosmos.Query.Core;
using Microsoft.Azure.Cosmos.Query.Core.Monads;
using Microsoft.Azure.Cosmos.Telemetry;
using Microsoft.Azure.Cosmos.Tracing;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Cosmos.ChangeFeed
{
  internal sealed class ChangeFeedEstimatorIterator : FeedIterator<ChangeFeedProcessorState>
  {
    private const string EstimatorDefaultHostName = "Estimator";
    private const char PKRangeIdSeparator = ':';
    private const char SegmentSeparator = '#';
    private const string LSNPropertyName = "_lsn";
    private readonly ContainerInternal monitoredContainer;
    private readonly ContainerInternal leaseContainer;
    private readonly string processorName;
    private readonly Func<DocumentServiceLease, string, bool, FeedIteratorInternal> monitoredContainerFeedCreator;
    private readonly ChangeFeedEstimatorRequestOptions changeFeedEstimatorRequestOptions;
    private readonly AsyncLazy<TryCatch<IReadOnlyList<DocumentServiceLease>>> lazyLeaseDocuments;
    private DocumentServiceLeaseContainer documentServiceLeaseContainer;
    private int currentPage;
    private int maxPage;
    private int pageSize;
    private bool hasMoreResults;

    public ChangeFeedEstimatorIterator(
      string processorName,
      ContainerInternal monitoredContainer,
      ContainerInternal leaseContainer,
      DocumentServiceLeaseContainer documentServiceLeaseContainer,
      ChangeFeedEstimatorRequestOptions changeFeedEstimatorRequestOptions)
      : this(processorName, monitoredContainer, leaseContainer, documentServiceLeaseContainer, changeFeedEstimatorRequestOptions, (Func<DocumentServiceLease, string, bool, FeedIteratorInternal>) ((lease, continuationToken, startFromBeginning) => (FeedIteratorInternal) ChangeFeedPartitionKeyResultSetIteratorCore.Create(lease, continuationToken, new int?(1), monitoredContainer, new DateTime?(), string.IsNullOrEmpty(continuationToken))))
    {
    }

    internal ChangeFeedEstimatorIterator(
      ContainerInternal monitoredContainer,
      ContainerInternal leaseContainer,
      DocumentServiceLeaseContainer documentServiceLeaseContainer,
      Func<DocumentServiceLease, string, bool, FeedIteratorInternal> monitoredContainerFeedCreator,
      ChangeFeedEstimatorRequestOptions changeFeedEstimatorRequestOptions)
      : this(string.Empty, monitoredContainer, leaseContainer, documentServiceLeaseContainer, changeFeedEstimatorRequestOptions, monitoredContainerFeedCreator)
    {
    }

    private ChangeFeedEstimatorIterator(
      string processorName,
      ContainerInternal monitoredContainer,
      ContainerInternal leaseContainer,
      DocumentServiceLeaseContainer documentServiceLeaseContainer,
      ChangeFeedEstimatorRequestOptions changeFeedEstimatorRequestOptions,
      Func<DocumentServiceLease, string, bool, FeedIteratorInternal> monitoredContainerFeedCreator)
    {
      this.processorName = processorName ?? throw new ArgumentNullException(nameof (processorName));
      this.monitoredContainer = monitoredContainer ?? throw new ArgumentNullException(nameof (monitoredContainer));
      this.leaseContainer = leaseContainer ?? throw new ArgumentNullException(nameof (leaseContainer));
      this.changeFeedEstimatorRequestOptions = changeFeedEstimatorRequestOptions ?? new ChangeFeedEstimatorRequestOptions();
      if (this.changeFeedEstimatorRequestOptions.MaxItemCount.HasValue && this.changeFeedEstimatorRequestOptions.MaxItemCount.Value <= 0)
        throw new ArgumentOutOfRangeException("MaxItemCount value should be a positive integer.");
      this.lazyLeaseDocuments = new AsyncLazy<TryCatch<IReadOnlyList<DocumentServiceLease>>>((Func<ITrace, CancellationToken, Task<TryCatch<IReadOnlyList<DocumentServiceLease>>>>) ((trace, innerCancellationToken) => this.TryInitializeLeaseDocumentsAsync(innerCancellationToken)));
      this.hasMoreResults = true;
      this.monitoredContainerFeedCreator = monitoredContainerFeedCreator;
      this.documentServiceLeaseContainer = documentServiceLeaseContainer;
    }

    public override bool HasMoreResults => this.hasMoreResults;

    public override Task<FeedResponse<ChangeFeedProcessorState>> ReadNextAsync(
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.monitoredContainer.ClientContext.OperationHelperAsync<FeedResponse<ChangeFeedProcessorState>>("Change Feed Estimator Read Next Async", (RequestOptions) null, (Func<ITrace, Task<FeedResponse<ChangeFeedProcessorState>>>) (trace => this.ReadNextAsync(trace, cancellationToken)), (Func<FeedResponse<ChangeFeedProcessorState>, OpenTelemetryAttributes>) (response => (OpenTelemetryAttributes) new OpenTelemetryResponse<ChangeFeedProcessorState>(response, this.monitoredContainer?.Id, this.monitoredContainer?.Database?.Id ?? this.databaseName)), TraceComponent.ChangeFeed);
    }

    public async Task<FeedResponse<ChangeFeedProcessorState>> ReadNextAsync(
      ITrace trace,
      CancellationToken cancellationToken)
    {
      ChangeFeedEstimatorIterator estimatorIterator1 = this;
      if (!estimatorIterator1.lazyLeaseDocuments.ValueInitialized)
      {
        await estimatorIterator1.InitializeLeaseStoreAsync(trace, cancellationToken);
        TryCatch<IReadOnlyList<DocumentServiceLease>> tryCatch = await estimatorIterator1.lazyLeaseDocuments.GetValueAsync(trace, cancellationToken).ConfigureAwait(false);
        if (!tryCatch.Succeeded)
        {
          if (!(tryCatch.Exception.InnerException is CosmosException innerException))
            throw new InvalidOperationException("Failed to convert to CosmosException.");
          throw innerException;
        }
        estimatorIterator1.currentPage = 0;
        int? maxItemCount = estimatorIterator1.changeFeedEstimatorRequestOptions.MaxItemCount;
        if (maxItemCount.HasValue)
        {
          ChangeFeedEstimatorIterator estimatorIterator2 = estimatorIterator1;
          maxItemCount = estimatorIterator1.changeFeedEstimatorRequestOptions.MaxItemCount;
          int num = maxItemCount.Value;
          estimatorIterator2.pageSize = num;
          estimatorIterator1.maxPage = (int) Math.Ceiling((double) estimatorIterator1.lazyLeaseDocuments.Result.Result.Count / (double) estimatorIterator1.pageSize);
        }
        else
        {
          estimatorIterator1.pageSize = estimatorIterator1.lazyLeaseDocuments.Result.Result.Count;
          estimatorIterator1.maxPage = 1;
        }
      }
      return await estimatorIterator1.ReadNextInternalAsync(trace, cancellationToken);
    }

    private async Task<FeedResponse<ChangeFeedProcessorState>> ReadNextInternalAsync(
      ITrace trace,
      CancellationToken cancellationToken)
    {
      ChangeFeedEstimatorIterator estimatorIterator1 = this;
      cancellationToken.ThrowIfCancellationRequested();
      if (estimatorIterator1.lazyLeaseDocuments.Result.Result.Count == 0)
      {
        estimatorIterator1.hasMoreResults = false;
        return (FeedResponse<ChangeFeedProcessorState>) new ChangeFeedEstimatorIterator.ChangeFeedEstimatorEmptyFeedResponse(trace);
      }
      (ChangeFeedProcessorState, ResponseMessage)[] valueTupleArray = await Task.WhenAll<(ChangeFeedProcessorState, ResponseMessage)>((IEnumerable<Task<(ChangeFeedProcessorState, ResponseMessage)>>) estimatorIterator1.lazyLeaseDocuments.Result.Result.Skip<DocumentServiceLease>(estimatorIterator1.currentPage * estimatorIterator1.pageSize).Take<DocumentServiceLease>(estimatorIterator1.pageSize).Select<DocumentServiceLease, Task<(ChangeFeedProcessorState, ResponseMessage)>>((Func<DocumentServiceLease, Task<(ChangeFeedProcessorState, ResponseMessage)>>) (lease => this.GetRemainingWorkAsync(lease, trace, cancellationToken))).ToArray<Task<(ChangeFeedProcessorState, ResponseMessage)>>());
      List<ChangeFeedProcessorState> feedProcessorStateList = new List<ChangeFeedProcessorState>();
      double ruCost = 0.0;
      foreach ((ChangeFeedProcessorState, ResponseMessage) valueTuple in (IEnumerable<(ChangeFeedProcessorState, ResponseMessage)>) valueTupleArray)
      {
        using (valueTuple.Item2)
          ruCost += valueTuple.Item2.Headers.RequestCharge;
        feedProcessorStateList.Add(valueTuple.Item1);
      }
      ChangeFeedEstimatorIterator estimatorIterator2 = estimatorIterator1;
      ChangeFeedEstimatorIterator estimatorIterator3 = estimatorIterator1;
      int num1 = estimatorIterator1.currentPage + 1;
      int num2 = num1;
      estimatorIterator3.currentPage = num2;
      int num3 = num1 != estimatorIterator1.maxPage ? 1 : 0;
      estimatorIterator2.hasMoreResults = num3 != 0;
      return (FeedResponse<ChangeFeedProcessorState>) new ChangeFeedEstimatorIterator.ChangeFeedEstimatorFeedResponse(trace, feedProcessorStateList.AsReadOnly(), ruCost);
    }

    internal static string ExtractLsnFromSessionToken(string sessionToken)
    {
      if (string.IsNullOrEmpty(sessionToken))
        return string.Empty;
      string[] strArray = sessionToken.Substring(sessionToken.IndexOf(':') + 1).Split('#');
      return strArray.Length < 2 ? strArray[0] : strArray[1];
    }

    private static string GetFirstItemLSN(IEnumerable<JObject> items)
    {
      JObject jobject = items.FirstOrDefault<JObject>();
      if (jobject == null)
        return (string) null;
      JToken jtoken;
      if (jobject.TryGetValue("_lsn", StringComparison.OrdinalIgnoreCase, out jtoken))
        return jtoken.Value<string>();
      DefaultTrace.TraceWarning("Change Feed response item does not include LSN.");
      return (string) null;
    }

    private static long TryConvertToNumber(string number)
    {
      long result;
      if (long.TryParse(number, NumberStyles.Number, (IFormatProvider) CultureInfo.InvariantCulture, out result))
        return result;
      DefaultTrace.TraceWarning("Cannot parse number '{0}'.", (object) number);
      return 0;
    }

    private static IEnumerable<JObject> GetItemsFromResponse(ResponseMessage response) => response.Content == null ? (IEnumerable<JObject>) new Collection<JObject>() : (IEnumerable<JObject>) CosmosFeedResponseSerializer.FromFeedResponseStream<JObject>(CosmosContainerExtensions.DefaultJsonSerializer, response.Content);

    private async Task<(ChangeFeedProcessorState, ResponseMessage)> GetRemainingWorkAsync(
      DocumentServiceLease existingLease,
      ITrace trace,
      CancellationToken cancellationToken)
    {
      using (FeedIteratorInternal iterator = this.monitoredContainerFeedCreator(existingLease, existingLease.ContinuationToken, string.IsNullOrEmpty(existingLease.ContinuationToken)))
      {
        try
        {
          ResponseMessage response = await iterator.ReadNextAsync(trace, cancellationToken).ConfigureAwait(false);
          if (response.StatusCode != HttpStatusCode.NotModified)
            response.EnsureSuccessStatusCode();
          long number = ChangeFeedEstimatorIterator.TryConvertToNumber(ChangeFeedEstimatorIterator.ExtractLsnFromSessionToken(response.Headers.Session));
          IEnumerable<JObject> itemsFromResponse = ChangeFeedEstimatorIterator.GetItemsFromResponse(response);
          long num1 = itemsFromResponse.Any<JObject>() ? ChangeFeedEstimatorIterator.TryConvertToNumber(ChangeFeedEstimatorIterator.GetFirstItemLSN(itemsFromResponse)) - 1L : number;
          if (num1 < 0L)
            return (new ChangeFeedProcessorState(existingLease.CurrentLeaseToken, 1L, existingLease.Owner), response);
          long num2 = number - num1;
          return (new ChangeFeedProcessorState(existingLease.CurrentLeaseToken, num2 < 0L ? 0L : num2, existingLease.Owner), response);
        }
        catch (Exception ex)
        {
          Microsoft.Azure.Cosmos.Extensions.TraceException(ex);
          DefaultTrace.TraceWarning("GetEstimateWork > exception: lease token '{0}'", (object) existingLease.CurrentLeaseToken);
          throw;
        }
      }
    }

    private async Task InitializeLeaseStoreAsync(ITrace trace, CancellationToken cancellationToken)
    {
      using (trace.StartChild("Initialize Lease Store", TraceComponent.ChangeFeed, TraceLevel.Info))
      {
        if (this.documentServiceLeaseContainer == null)
          this.documentServiceLeaseContainer = (await DocumentServiceLeaseStoreManagerBuilder.InitializeAsync(this.monitoredContainer, this.leaseContainer, this.monitoredContainer.GetLeasePrefix(this.processorName, await this.monitoredContainer.GetMonitoredDatabaseAndContainerRidAsync(cancellationToken)), "Estimator")).LeaseContainer;
      }
    }

    private async Task<TryCatch<IReadOnlyList<DocumentServiceLease>>> TryInitializeLeaseDocumentsAsync(
      CancellationToken cancellationToken)
    {
      cancellationToken.ThrowIfCancellationRequested();
      try
      {
        return TryCatch<IReadOnlyList<DocumentServiceLease>>.FromResult(await this.documentServiceLeaseContainer.GetAllLeasesAsync().ConfigureAwait(false));
      }
      catch (CosmosException ex)
      {
        return TryCatch<IReadOnlyList<DocumentServiceLease>>.FromException((Exception) ex);
      }
    }

    private sealed class ChangeFeedEstimatorFeedResponse : FeedResponse<ChangeFeedProcessorState>
    {
      private readonly ReadOnlyCollection<ChangeFeedProcessorState> remainingLeaseWorks;
      private readonly Microsoft.Azure.Cosmos.Headers headers;

      public ChangeFeedEstimatorFeedResponse(
        ITrace trace,
        ReadOnlyCollection<ChangeFeedProcessorState> remainingLeaseWorks,
        double ruCost)
      {
        this.Trace = trace ?? throw new ArgumentNullException(nameof (trace));
        this.remainingLeaseWorks = remainingLeaseWorks ?? throw new ArgumentNullException(nameof (remainingLeaseWorks));
        this.headers = new Microsoft.Azure.Cosmos.Headers()
        {
          RequestCharge = ruCost
        };
      }

      public ITrace Trace { get; }

      public override string ContinuationToken => throw new NotSupportedException();

      public override int Count => this.remainingLeaseWorks.Count;

      public override Microsoft.Azure.Cosmos.Headers Headers => this.headers;

      public override IEnumerable<ChangeFeedProcessorState> Resource => (IEnumerable<ChangeFeedProcessorState>) this.remainingLeaseWorks;

      public override HttpStatusCode StatusCode => HttpStatusCode.OK;

      public override CosmosDiagnostics Diagnostics => (CosmosDiagnostics) new CosmosTraceDiagnostics(this.Trace);

      public override string IndexMetrics => (string) null;

      internal override RequestMessage RequestMessage => (RequestMessage) null;

      public override IEnumerator<ChangeFeedProcessorState> GetEnumerator() => this.remainingLeaseWorks.GetEnumerator();
    }

    private sealed class ChangeFeedEstimatorEmptyFeedResponse : 
      FeedResponse<ChangeFeedProcessorState>
    {
      private static readonly IEnumerable<ChangeFeedProcessorState> remainingLeaseWorks = Enumerable.Empty<ChangeFeedProcessorState>();
      private readonly Microsoft.Azure.Cosmos.Headers headers;

      public ChangeFeedEstimatorEmptyFeedResponse(ITrace trace)
      {
        this.Trace = trace ?? throw new ArgumentNullException(nameof (trace));
        this.headers = new Microsoft.Azure.Cosmos.Headers();
      }

      public ITrace Trace { get; }

      public override string ContinuationToken => throw new NotSupportedException();

      public override int Count => 0;

      public override Microsoft.Azure.Cosmos.Headers Headers => throw new NotImplementedException();

      public override IEnumerable<ChangeFeedProcessorState> Resource => ChangeFeedEstimatorIterator.ChangeFeedEstimatorEmptyFeedResponse.remainingLeaseWorks;

      public override HttpStatusCode StatusCode => HttpStatusCode.OK;

      public override CosmosDiagnostics Diagnostics => (CosmosDiagnostics) new CosmosTraceDiagnostics(this.Trace);

      public override string IndexMetrics => string.Empty;

      internal override RequestMessage RequestMessage => (RequestMessage) null;

      public override IEnumerator<ChangeFeedProcessorState> GetEnumerator() => ChangeFeedEstimatorIterator.ChangeFeedEstimatorEmptyFeedResponse.remainingLeaseWorks.GetEnumerator();
    }
  }
}
