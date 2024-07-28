// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.Egress.EgressWorker
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D3AB5C9B-EB54-4477-A304-63BB297414A3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.dll

using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Content.Common;
using Microsoft.VisualStudio.Services.Content.Server.Azure;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Microsoft.VisualStudio.Services.BlobStore.Server.Egress
{
  public class EgressWorker
  {
    private readonly IEgressWorkerUtilities egressWorkerUtilities;
    private readonly IStorageAccountAdapter storageAccountAdapter;
    private readonly ICloudAnalyticsClientAdapter cloudAnalyticsClientAdapter;
    private readonly EgressWorker.WorkingWindow window;
    private readonly Dictionary<string, string> shardConnectionMap;
    private readonly Dictionary<string, int> shardToLogBlobsProcessedMap;
    private readonly int lowerLimitForDelayAfterTask;
    private readonly int upperLimitForDelayAfterTask;
    private readonly Microsoft.VisualStudio.Services.Content.Server.Common.Tracer tracer;
    private readonly string serviceScope;
    private readonly EgressConstants.AzureBlobStorageLogs parseScope;
    private ConcurrentDictionary<EgressWorker.EgressLogBlob, EgressParserResult> egressLogBlobToParserResultMap;
    public static readonly string RegistryEgressCheckpoint = "/Configuration/BlobStore/EgressComputeJob/EgressCheckpoint";

    public EgressWorker(
      EgressConstants.AzureBlobStorageLogs parseScope,
      IEnumerable<string> connectionStrings,
      IEgressWorkerUtilities egressWorkerUtilities,
      IStorageAccountAdapter storageAccountAdapter,
      ICloudAnalyticsClientAdapter cloudAnalyticsClientAdapter,
      EgressWorker.WorkingWindow window,
      Microsoft.VisualStudio.Services.Content.Server.Common.Tracer tracer)
    {
      this.parseScope = parseScope;
      this.egressWorkerUtilities = egressWorkerUtilities;
      this.storageAccountAdapter = storageAccountAdapter;
      this.cloudAnalyticsClientAdapter = cloudAnalyticsClientAdapter;
      this.window = window;
      this.tracer = tracer;
      this.lowerLimitForDelayAfterTask = egressWorkerUtilities.GetLowerLimitForDelayAfterTask();
      this.upperLimitForDelayAfterTask = egressWorkerUtilities.GetUpperLimitForDelayAfterTask();
      this.serviceScope = Enum.GetName(typeof (EgressConstants.AzureBlobStorageLogs), (object) this.parseScope)?.ToLowerInvariant();
      this.shardToLogBlobsProcessedMap = new Dictionary<string, int>();
      this.shardConnectionMap = new Dictionary<string, string>();
      foreach (string connectionString in connectionStrings)
      {
        string shardName = egressWorkerUtilities.GetShardName(connectionString);
        this.shardConnectionMap.Add(connectionString, shardName);
      }
      this.egressLogBlobToParserResultMap = new ConcurrentDictionary<EgressWorker.EgressLogBlob, EgressParserResult>();
    }

    public async Task<EgressWorkerResult> DispatchWorkAsync(
      IVssRequestContext requestContext,
      EgressWorkerResult egressWorkerResult)
    {
      while (true)
      {
        DateTimeOffset now = this.egressWorkerUtilities.Now;
        if (!(egressWorkerResult.LifeSpan < now))
        {
          this.ComputeWorkingWindow(requestContext, this.window, now);
          if (!egressWorkerResult.ShardMetricMap.ContainsKey(this.window.TimeSlot))
          {
            egressWorkerResult.ShardMetricMap[this.window.TimeSlot] = new Dictionary<string, EgressParserResult>();
            foreach (string str in this.shardConnectionMap.Values)
              egressWorkerResult.ShardMetricMap[this.window.TimeSlot].Add(str, new EgressParserResult(str));
          }
          this.GenerateSearchPrefix(this.window);
          await this.ParseAndProcess(requestContext, this.storageAccountAdapter, this.cloudAnalyticsClientAdapter, egressWorkerResult, this.window).ConfigureAwait(true);
        }
        else
          break;
      }
      return egressWorkerResult;
    }

    public int GetLogBlobsProcessedFromCheckpoint(
      IVssRequestContext requestContext,
      string shardName,
      EgressWorkerResult egressWorkerResult,
      EgressWorker.WorkingWindow window)
    {
      string query = EgressWorker.RegistryEgressCheckpoint + "/" + shardName;
      string checkpoint = requestContext.GetService<IVssRegistryService>().GetValue(requestContext, (RegistryQuery) query, false, (string) null);
      if (!string.IsNullOrWhiteSpace(checkpoint))
      {
        DateTimeOffset dateTime;
        if (!EgressCheckpoint.TryGetDateTime(checkpoint, out dateTime))
        {
          this.tracer.TraceWarning(string.Format("[Egress Worker] Couldn't render prefix: {0} based on checkpoint on {1} at {2} for {3} - DateTimeParsed: {4}. Aborting.", (object) window.Prefix, (object) shardName, (object) window.StartTime, (object) this.serviceScope, (object) dateTime));
          return 0;
        }
        if (dateTime.Hour == window.StartTime.Hour)
        {
          egressWorkerResult.ShardMetricMap[window.TimeSlot][shardName].LogBlobProcessedCount = EgressCheckpoint.GetLogBlobProcessedCount(checkpoint);
          return egressWorkerResult.ShardMetricMap[window.TimeSlot][shardName].LogBlobProcessedCount;
        }
      }
      return 0;
    }

    private void ComputeWorkingWindow(
      IVssRequestContext requestContext,
      EgressWorker.WorkingWindow window,
      DateTimeOffset currUtcTime)
    {
      TimeSpan timeSpan = EgressConstants.DefaultLookBackWindowSize;
      short result;
      if (short.TryParse(requestContext.GetService<IVssRegistryService>().GetValue(requestContext, (RegistryQuery) EgressConstants.RegistryEgressComputeLookbackWindowInHrs, false, (string) null), out result) && (int) result > (int) EgressConstants.DefaultLookBackWindowInHrs)
        timeSpan = TimeSpan.FromHours((double) result);
      if (currUtcTime.Subtract(timeSpan) < window.StartTime || currUtcTime.Subtract(timeSpan) > window.EndTime)
      {
        window.StartTime = currUtcTime.AddMinutes((double) -currUtcTime.Minute).Subtract(EgressConstants.DefaultLookBackWindowSize);
        window.EndTime = window.StartTime.AddHours((double) EgressConstants.DefaultWindowSize);
      }
      window.TimeSlot = window.StartTime.UtcDateTime.ToString("yyyy/MM/dd/") + string.Format("{0:D2}00", (object) window.StartTime.Hour);
    }

    private async Task ParseAndProcess(
      IVssRequestContext requestContext,
      IStorageAccountAdapter storageAccountAdapter,
      ICloudAnalyticsClientAdapter cloudAnalyticsClientAdapter,
      EgressWorkerResult egressWorkerResult,
      EgressWorker.WorkingWindow window)
    {
      this.UpdateProgressForAllShards(requestContext, (IEnumerable<string>) this.shardConnectionMap.Values, egressWorkerResult, window);
      Action<string> action1 = (Action<string>) (connectionString =>
      {
        string shardConnection = this.shardConnectionMap[connectionString];
        IEnumerable<IListBlobItem> cloudBlobs = storageAccountAdapter.GetCloudBlobs(connectionString, window.Prefix);
        if (!cloudBlobs.Any<IListBlobItem>())
          return;
        int num = 0;
        foreach (IListBlobItem listBlobItem in cloudBlobs)
        {
          if (!(listBlobItem is ICloudBlob cloudBlob2))
            this.tracer.TraceWarning("[Egress Parser] Invalid cloud blob encountered.");
          else if (!cloudBlob2.Metadata.ContainsKey("StartTime") || !cloudBlob2.Metadata.ContainsKey("EndTime") || !cloudBlob2.Metadata.ContainsKey("LogType"))
          {
            this.tracer.TraceWarning(string.Format("[Egress Parser] Invalid metadata entry found for cloud blob. {0}", (object) cloudBlob2.Metadata));
          }
          else
          {
            DateTimeOffset universalTime = DateTimeOffset.Parse(cloudBlob2.Metadata["StartTime"]).ToUniversalTime();
            DateTimeOffset.Parse(cloudBlob2.Metadata["EndTime"]).ToUniversalTime();
            if (((IEnumerable<string>) cloudBlob2.Metadata["LogType"].ToLower().Split(',')).Any<string>((Func<string, bool>) (t => t.Equals(Enum.GetName(typeof (EgressConstants.LogBlobType), (object) EgressConstants.LogBlobType.Read), StringComparison.OrdinalIgnoreCase))) && num++ >= this.shardToLogBlobsProcessedMap[shardConnection])
              this.egressLogBlobToParserResultMap.TryAdd(new EgressWorker.EgressLogBlob()
              {
                CloudBlob = cloudBlob2,
                ConnectionString = connectionString,
                LogStartTime = universalTime
              }, (EgressParserResult) null);
          }
        }
      });
      ExecutionDataflowBlockOptions dataflowBlockOptions1 = new ExecutionDataflowBlockOptions();
      dataflowBlockOptions1.MaxDegreeOfParallelism = this.shardConnectionMap.Count;
      dataflowBlockOptions1.CancellationToken = requestContext.CancellationToken;
      await NonSwallowingActionBlock.Create<string>(action1, dataflowBlockOptions1).PostAllToUnboundedAndCompleteAsync<string>((IEnumerable<string>) this.shardConnectionMap.Keys, requestContext.CancellationToken).ConfigureAwait(true);
      ConcurrentBag<EgressWorker.EgressLogBlob> inputs = new ConcurrentBag<EgressWorker.EgressLogBlob>((IEnumerable<EgressWorker.EgressLogBlob>) this.egressLogBlobToParserResultMap.Keys.OrderByDescending<EgressWorker.EgressLogBlob, DateTimeOffset>((Func<EgressWorker.EgressLogBlob, DateTimeOffset>) (logBlob => logBlob.LogStartTime)));
      if (this.egressLogBlobToParserResultMap.Count > 0)
      {
        EgressParser egressParser = new EgressParser(cloudAnalyticsClientAdapter, window.StartTime, window.EndTime, this.tracer);
        try
        {
          Action<EgressWorker.EgressLogBlob> action2 = (Action<EgressWorker.EgressLogBlob>) (egressLogBlob =>
          {
            string shardConnection = this.shardConnectionMap[egressLogBlob.ConnectionString];
            EgressParserResult egressParserResult = egressParser.ProcessLogBlob(egressLogBlob.CloudBlob);
            egressParserResult.ShardName = shardConnection;
            this.egressLogBlobToParserResultMap.AddOrUpdate(egressLogBlob, egressParserResult, (Func<EgressWorker.EgressLogBlob, EgressParserResult, EgressParserResult>) ((key, value) => egressParserResult));
          });
          ExecutionDataflowBlockOptions dataflowBlockOptions2 = new ExecutionDataflowBlockOptions();
          dataflowBlockOptions2.MaxDegreeOfParallelism = this.egressLogBlobToParserResultMap.Count;
          dataflowBlockOptions2.CancellationToken = requestContext.CancellationToken;
          await NonSwallowingActionBlock.Create<EgressWorker.EgressLogBlob>(action2, dataflowBlockOptions2).PostAllToUnboundedAndCompleteAsync<EgressWorker.EgressLogBlob>((IEnumerable<EgressWorker.EgressLogBlob>) inputs, requestContext.CancellationToken).ConfigureAwait(true);
          this.UpdateEgressComputeWorkerResult(this.egressLogBlobToParserResultMap, egressWorkerResult, window);
          this.SetCheckpointForAllShards(requestContext, (IEnumerable<string>) this.shardConnectionMap.Values, egressWorkerResult, window);
        }
        catch (Exception ex)
        {
          egressWorkerResult.ExceptionSet.Add("Exception: " + JobHelper.GetNestedExceptionMessage(ex));
          this.SetCheckpointForAllShards(requestContext, (IEnumerable<string>) this.shardConnectionMap.Values, egressWorkerResult, window);
          throw;
        }
      }
      await Task.Delay(TimeSpan.FromSeconds((double) ThreadLocalRandom.Generator.Next(this.lowerLimitForDelayAfterTask, this.upperLimitForDelayAfterTask)), requestContext.CancellationToken).ConfigureAwait(true);
    }

    private void UpdateEgressComputeWorkerResult(
      ConcurrentDictionary<EgressWorker.EgressLogBlob, EgressParserResult> egressLogBlobToParserResultMap,
      EgressWorkerResult egressWorkerResult,
      EgressWorker.WorkingWindow window)
    {
      foreach (KeyValuePair<EgressWorker.EgressLogBlob, EgressParserResult> blobToParserResult in egressLogBlobToParserResultMap)
      {
        foreach (KeyValuePair<string, long> keyValuePair in blobToParserResult.Value.EgressMetricPerHost)
          EgressParser.AddOrUpdateDictionary(egressWorkerResult.ShardMetricMap[window.TimeSlot][blobToParserResult.Value.ShardName].EgressMetricPerHost, keyValuePair.Key, keyValuePair.Value);
        ++egressWorkerResult.ShardMetricMap[window.TimeSlot][blobToParserResult.Value.ShardName].LogBlobProcessedCount;
        egressLogBlobToParserResultMap.TryRemove(blobToParserResult.Key, out EgressParserResult _);
      }
      egressWorkerResult.ShardMetricMap[window.TimeSlot].Sum<KeyValuePair<string, EgressParserResult>>((Func<KeyValuePair<string, EgressParserResult>, int>) (x => x.Value.LogBlobProcessedCount));
    }

    private void UpdateProgressForAllShards(
      IVssRequestContext requestContext,
      IEnumerable<string> shardNames,
      EgressWorkerResult egressWorkerResult,
      EgressWorker.WorkingWindow window)
    {
      foreach (string shardName in shardNames)
        this.shardToLogBlobsProcessedMap[shardName] = this.GetLogBlobsProcessedFromCheckpoint(requestContext, shardName, egressWorkerResult, window);
    }

    private void SetCheckpointForAllShards(
      IVssRequestContext requestContext,
      IEnumerable<string> shardNames,
      EgressWorkerResult egressWorkerResult,
      EgressWorker.WorkingWindow window)
    {
      foreach (string shardName in shardNames)
        this.SetCheckPointForShard(requestContext, shardName, egressWorkerResult, window);
    }

    private void SetCheckPointForShard(
      IVssRequestContext requestContext,
      string shardName,
      EgressWorkerResult egressWorkerResult,
      EgressWorker.WorkingWindow window)
    {
      string path = EgressWorker.RegistryEgressCheckpoint + "/" + shardName;
      string str = EgressCheckpoint.RenderCheckpoint(this.serviceScope, (long) egressWorkerResult.ShardMetricMap[window.TimeSlot][shardName].LogBlobProcessedCount, window.StartTime);
      if (string.IsNullOrWhiteSpace(str))
        this.tracer.TraceError(string.Format("[Egress Worker] Checkpoint couldn't be rendered on {0} at {1} for {2}. Skipping.", (object) shardName, (object) window.StartTime, (object) this.serviceScope));
      else
        requestContext.GetService<IVssRegistryService>().SetValue<string>(requestContext, path, str);
    }

    private void GenerateSearchPrefix(EgressWorker.WorkingWindow window)
    {
      if (string.IsNullOrWhiteSpace(this.serviceScope))
        return;
      StringBuilder stringBuilder1 = new StringBuilder();
      StringBuilder stringBuilder2 = stringBuilder1;
      object[] objArray = new object[5]
      {
        (object) this.serviceScope,
        null,
        null,
        null,
        null
      };
      DateTimeOffset startTime = window.StartTime;
      objArray[1] = (object) startTime.Year;
      startTime = window.StartTime;
      objArray[2] = (object) startTime.Month;
      startTime = window.StartTime;
      objArray[3] = (object) startTime.Day;
      startTime = window.StartTime;
      objArray[4] = (object) startTime.Hour;
      stringBuilder2.AppendFormat("{0}/{1}/{2:D2}/{3:D2}/{4:D2}00", objArray);
      window.Prefix = !string.IsNullOrWhiteSpace(stringBuilder1.ToString()) ? stringBuilder1.ToString() : throw new InvalidOperationException(string.Format("[Egress Worker] Invalid prefix: {0} encountered at {1} for {2}. Aborting.", (object) stringBuilder1, (object) window.StartTime, (object) this.serviceScope));
    }

    public class EgressWorkerUtilities : IEgressWorkerUtilities, IClock
    {
      public string GetShardName(string connectionString) => StorageAccountUtilities.GetAccountInfo(connectionString).Name;

      public int GetLowerLimitForDelayAfterTask() => 30;

      public int GetUpperLimitForDelayAfterTask() => 60;

      public DateTimeOffset Now => UtcClock.Instance.Now;
    }

    public class StorageAccountAdapter : IStorageAccountAdapter
    {
      public IEnumerable<IListBlobItem> GetCloudBlobs(string connectionString, string prefix) => CloudStorageAccount.Parse(connectionString).CreateCloudBlobClient().GetContainerReference("$logs").ListBlobs(prefix, true, BlobListingDetails.All);
    }

    public class WorkingWindow
    {
      public DateTimeOffset StartTime { get; set; }

      public DateTimeOffset EndTime { get; set; }

      public string Prefix { get; set; }

      public string TimeSlot { get; set; }
    }

    private class EgressLogBlob
    {
      public string ConnectionString { get; set; }

      public ICloudBlob CloudBlob { get; set; }

      public DateTimeOffset LogStartTime { get; set; }
    }
  }
}
