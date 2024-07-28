// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.Egress.EgressParser
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D3AB5C9B-EB54-4477-A304-63BB297414A3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.dll

using Microsoft.Azure.Storage.Blob;
using Microsoft.VisualStudio.Services.Content.Common;
using Microsoft.VisualStudio.Services.Content.Server.Common;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.BlobStore.Server.Egress
{
  public class EgressParser
  {
    private readonly int HostIdLength = Guid.NewGuid().ToString("N").Length;
    private readonly DateTimeOffset startTime;
    private readonly DateTimeOffset endTime;
    private readonly Tracer tracer;
    private readonly ICloudAnalyticsClientAdapter cloudAnalyticsClient;

    public EgressParser(
      ICloudAnalyticsClientAdapter cloudAnalyticsClient,
      DateTimeOffset startTime,
      DateTimeOffset endTime,
      Tracer tracer)
    {
      this.cloudAnalyticsClient = cloudAnalyticsClient;
      this.startTime = startTime;
      this.endTime = endTime;
      this.tracer = tracer;
    }

    public EgressParserResult ProcessLogBlob(ICloudBlob cloudBlob)
    {
      EgressParserResult egressComputeShardResult = new EgressParserResult();
      foreach (LogRecordAdapter logRecord in this.cloudAnalyticsClient.GetLogRecords(cloudBlob))
      {
        try
        {
          if (this.IsBillableLogRecord(logRecord))
            this.ExtractMetric(egressComputeShardResult, logRecord);
        }
        catch (Exception ex)
        {
          if (string.IsNullOrWhiteSpace(egressComputeShardResult.Exception))
            egressComputeShardResult.Exception = "Exception: " + JobHelper.GetNestedExceptionMessage(ex);
        }
      }
      return egressComputeShardResult;
    }

    private bool IsRequestBillable(string requestStatus, string requesterIpAddress) => Enum.TryParse<EgressConstants.BillableRequestStatus>(requestStatus, true, out EgressConstants.BillableRequestStatus _);

    private bool IsBillableLogRecord(LogRecordAdapter logRecord) => "1.0".Equals(logRecord.VersionNumber, StringComparison.OrdinalIgnoreCase) && "GetBlob".Equals(logRecord.OperationType, StringComparison.OrdinalIgnoreCase) && this.IsRequestBillable(logRecord.RequestStatus, logRecord.RequesterIPAddress);

    private void ExtractMetric(
      EgressParserResult egressComputeShardResult,
      LogRecordAdapter logRecord)
    {
      string[] strArray = logRecord.RequestedObjectKey.Split('/');
      if (strArray.Length < 3)
      {
        this.tracer.TraceError(string.Format("[Egress Parser]: Could not retrieve host Id from the request object key property: {0} in the log record {1} during {2}:{3}.", (object) strArray, (object) logRecord, (object) this.startTime, (object) this.endTime));
      }
      else
      {
        string str = strArray[2].TrimEnd('"');
        if (str.Length < this.HostIdLength)
          return;
        string input = str.Substring(str.Length - this.HostIdLength);
        if (!Guid.TryParse(input, out Guid _))
          return;
        long? responsePacketSize = logRecord.ResponsePacketSize;
        if (!responsePacketSize.HasValue)
          return;
        responsePacketSize = logRecord.ResponsePacketSize;
        long num1 = 0;
        if (!(responsePacketSize.GetValueOrDefault() > num1 & responsePacketSize.HasValue))
          return;
        Dictionary<string, long> egressMetricPerHost = egressComputeShardResult.EgressMetricPerHost;
        string key = input;
        responsePacketSize = logRecord.ResponsePacketSize;
        long num2 = responsePacketSize.Value;
        EgressParser.AddOrUpdateDictionary(egressMetricPerHost, key, num2);
      }
    }

    public static void AddOrUpdateDictionary(
      Dictionary<string, long> dictionary,
      string key,
      long value)
    {
      if (dictionary.ContainsKey(key))
        dictionary[key] += value;
      else
        dictionary.Add(key, value);
    }
  }
}
