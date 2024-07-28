// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Tracing.TraceWriter
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.Json;
using Microsoft.Azure.Cosmos.Query.Core.Metrics;
using Microsoft.Azure.Cosmos.Telemetry;
using Microsoft.Azure.Cosmos.Tracing.TraceData;
using Microsoft.Azure.Documents;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace Microsoft.Azure.Cosmos.Tracing
{
  internal static class TraceWriter
  {
    private static readonly ConcurrentDictionary<string, string> FilePathToName = new ConcurrentDictionary<string, string>();

    public static void WriteTrace(
      TextWriter writer,
      ITrace trace,
      TraceLevel level = TraceLevel.Verbose,
      TraceWriter.AsciiType asciiType = TraceWriter.AsciiType.Default)
    {
      TraceWriter.TraceTextWriter.WriteTrace(writer, trace, level, asciiType);
    }

    public static void WriteTrace(IJsonWriter writer, ITrace trace) => TraceWriter.TraceJsonWriter.WriteTrace(writer, trace, true);

    public static string TraceToText(
      ITrace trace,
      TraceLevel level = TraceLevel.Verbose,
      TraceWriter.AsciiType asciiType = TraceWriter.AsciiType.Default)
    {
      StringWriter writer = new StringWriter();
      TraceWriter.WriteTrace((TextWriter) writer, trace, level, asciiType);
      return writer.ToString();
    }

    public static string TraceToJson(ITrace trace)
    {
      IJsonWriter writer = JsonWriter.Create(JsonSerializationFormat.Text);
      TraceWriter.WriteTrace(writer, trace);
      return Encoding.UTF8.GetString(writer.GetResult().ToArray());
    }

    public static string GetFileNameFromPath(string filePath)
    {
      string fileNameFromPath;
      if (!TraceWriter.FilePathToName.TryGetValue(filePath, out fileNameFromPath))
      {
        fileNameFromPath = ((IEnumerable<string>) filePath.Split('\\')).Last<string>();
        TraceWriter.FilePathToName[filePath] = fileNameFromPath;
      }
      return fileNameFromPath;
    }

    private static void WriteTraceDatum(IJsonWriter writer, object value)
    {
      switch (value)
      {
        case TraceDatum traceDatum:
          TraceWriter.TraceDatumJsonWriter traceDatumJsonWriter = new TraceWriter.TraceDatumJsonWriter(writer);
          traceDatum.Accept((ITraceDatumVisitor) traceDatumJsonWriter);
          break;
        case double num1:
          writer.WriteNumber64Value((Number64) num1);
          break;
        case long num2:
          writer.WriteNumber64Value((Number64) num2);
          break;
        case IEnumerable<object> objects:
          writer.WriteArrayStart();
          foreach (object obj in objects)
            TraceWriter.WriteTraceDatum(writer, obj);
          writer.WriteArrayEnd();
          break;
        case IDictionary<string, object> dictionary:
          writer.WriteObjectStart();
          foreach (KeyValuePair<string, object> keyValuePair in (IEnumerable<KeyValuePair<string, object>>) dictionary)
          {
            writer.WriteFieldName(keyValuePair.Key);
            TraceWriter.WriteTraceDatum(writer, keyValuePair.Value);
          }
          writer.WriteObjectEnd();
          break;
        case string str:
          writer.WriteStringValue(str);
          break;
        default:
          writer.WriteStringValue(value.ToString());
          break;
      }
    }

    public enum AsciiType
    {
      Default,
      DoubleLine,
      Classic,
      ClassicRounded,
      ExclamationMarks,
    }

    private static class TraceJsonWriter
    {
      public static void WriteTrace(IJsonWriter writer, ITrace trace, bool isRootTrace)
      {
        if (writer == null)
          throw new ArgumentNullException(nameof (writer));
        if (trace == null)
          throw new ArgumentNullException(nameof (trace));
        writer.WriteObjectStart();
        if (isRootTrace)
        {
          writer.WriteFieldName("Summary");
          new SummaryDiagnostics(trace).WriteSummaryDiagnostics(writer);
        }
        writer.WriteFieldName("name");
        writer.WriteStringValue(trace.Name);
        writer.WriteFieldName("id");
        writer.WriteStringValue(trace.Id.ToString());
        writer.WriteFieldName("start time");
        writer.WriteStringValue(trace.StartTime.ToString("hh:mm:ss:fff"));
        writer.WriteFieldName("duration in milliseconds");
        writer.WriteNumber64Value((Number64) trace.Duration.TotalMilliseconds);
        if (trace.Data.Any<KeyValuePair<string, object>>())
        {
          writer.WriteFieldName("data");
          writer.WriteObjectStart();
          foreach (KeyValuePair<string, object> keyValuePair in (IEnumerable<KeyValuePair<string, object>>) trace.Data)
          {
            string key = keyValuePair.Key;
            object obj = keyValuePair.Value;
            writer.WriteFieldName(key);
            TraceWriter.WriteTraceDatum(writer, obj);
          }
          writer.WriteObjectEnd();
        }
        if (trace.Children.Any<ITrace>())
        {
          writer.WriteFieldName("children");
          writer.WriteArrayStart();
          foreach (ITrace child in (IEnumerable<ITrace>) trace.Children)
            TraceWriter.TraceJsonWriter.WriteTrace(writer, child, false);
          writer.WriteArrayEnd();
        }
        writer.WriteObjectEnd();
      }
    }

    private sealed class TraceDatumJsonWriter : ITraceDatumVisitor
    {
      private readonly IJsonWriter jsonWriter;

      public TraceDatumJsonWriter(IJsonWriter jsonWriter) => this.jsonWriter = jsonWriter ?? throw new ArgumentNullException(nameof (jsonWriter));

      public void Visit(QueryMetricsTraceDatum queryMetricsTraceDatum) => this.jsonWriter.WriteStringValue(queryMetricsTraceDatum.QueryMetrics.ToString());

      public void Visit(
        PointOperationStatisticsTraceDatum pointOperationStatisticsTraceDatum)
      {
        this.jsonWriter.WriteObjectStart();
        this.jsonWriter.WriteFieldName("Id");
        this.jsonWriter.WriteStringValue("PointOperationStatistics");
        this.jsonWriter.WriteFieldName("ActivityId");
        this.WriteStringValueOrNull(pointOperationStatisticsTraceDatum.ActivityId);
        this.jsonWriter.WriteFieldName("ResponseTimeUtc");
        this.WriteDateTimeStringValue(pointOperationStatisticsTraceDatum.ResponseTimeUtc);
        this.jsonWriter.WriteFieldName("StatusCode");
        this.jsonWriter.WriteNumber64Value((Number64) (long) pointOperationStatisticsTraceDatum.StatusCode);
        this.jsonWriter.WriteFieldName("SubStatusCode");
        this.jsonWriter.WriteNumber64Value((Number64) (long) pointOperationStatisticsTraceDatum.SubStatusCode);
        this.jsonWriter.WriteFieldName("RequestCharge");
        this.jsonWriter.WriteNumber64Value((Number64) pointOperationStatisticsTraceDatum.RequestCharge);
        this.jsonWriter.WriteFieldName("RequestUri");
        this.WriteStringValueOrNull(pointOperationStatisticsTraceDatum.RequestUri);
        this.jsonWriter.WriteFieldName("ErrorMessage");
        this.WriteStringValueOrNull(pointOperationStatisticsTraceDatum.ErrorMessage);
        this.jsonWriter.WriteFieldName("RequestSessionToken");
        this.WriteStringValueOrNull(pointOperationStatisticsTraceDatum.RequestSessionToken);
        this.jsonWriter.WriteFieldName("ResponseSessionToken");
        this.WriteStringValueOrNull(pointOperationStatisticsTraceDatum.ResponseSessionToken);
        this.jsonWriter.WriteFieldName("BELatencyInMs");
        this.WriteStringValueOrNull(pointOperationStatisticsTraceDatum.BELatencyInMs);
        this.jsonWriter.WriteObjectEnd();
      }

      public void Visit(
        ClientSideRequestStatisticsTraceDatum clientSideRequestStatisticsTraceDatum)
      {
        this.jsonWriter.WriteObjectStart();
        this.jsonWriter.WriteFieldName("Id");
        this.jsonWriter.WriteStringValue("AggregatedClientSideRequestStatistics");
        this.WriteJsonUriArrayWithDuplicatesCounted("ContactedReplicas", (IReadOnlyList<TransportAddressUri>) clientSideRequestStatisticsTraceDatum.ContactedReplicas);
        this.WriteRegionsContactedArray("RegionsContacted", (IEnumerable<(string, Uri)>) clientSideRequestStatisticsTraceDatum.RegionsContacted);
        this.WriteJsonUriArray("FailedReplicas", (IEnumerable<TransportAddressUri>) clientSideRequestStatisticsTraceDatum.FailedReplicas);
        clientSideRequestStatisticsTraceDatum.WriteAddressCachRefreshContent(this.jsonWriter);
        this.jsonWriter.WriteFieldName("AddressResolutionStatistics");
        this.jsonWriter.WriteArrayStart();
        foreach (KeyValuePair<string, ClientSideRequestStatisticsTraceDatum.AddressResolutionStatistics> resolutionStatistic in (IEnumerable<KeyValuePair<string, ClientSideRequestStatisticsTraceDatum.AddressResolutionStatistics>>) clientSideRequestStatisticsTraceDatum.EndpointToAddressResolutionStatistics)
          this.VisitAddressResolutionStatistics(resolutionStatistic.Value);
        this.jsonWriter.WriteArrayEnd();
        this.jsonWriter.WriteFieldName("StoreResponseStatistics");
        this.jsonWriter.WriteArrayStart();
        foreach (ClientSideRequestStatisticsTraceDatum.StoreResponseStatistics responseStatistics in (IEnumerable<ClientSideRequestStatisticsTraceDatum.StoreResponseStatistics>) clientSideRequestStatisticsTraceDatum.StoreResponseStatisticsList)
          this.VisitStoreResponseStatistics(responseStatistics);
        this.jsonWriter.WriteArrayEnd();
        if (clientSideRequestStatisticsTraceDatum.HttpResponseStatisticsList.Count > 0)
        {
          this.jsonWriter.WriteFieldName("HttpResponseStats");
          this.jsonWriter.WriteArrayStart();
          foreach (ClientSideRequestStatisticsTraceDatum.HttpResponseStatistics responseStatistics in (IEnumerable<ClientSideRequestStatisticsTraceDatum.HttpResponseStatistics>) clientSideRequestStatisticsTraceDatum.HttpResponseStatisticsList)
            this.VisitHttpResponseStatistics(responseStatistics, this.jsonWriter);
          this.jsonWriter.WriteArrayEnd();
        }
        this.jsonWriter.WriteObjectEnd();
      }

      private void VisitHttpResponseStatistics(
        ClientSideRequestStatisticsTraceDatum.HttpResponseStatistics stat,
        IJsonWriter jsonWriter)
      {
        jsonWriter.WriteObjectStart();
        jsonWriter.WriteFieldName("StartTimeUTC");
        this.WriteDateTimeStringValue(stat.RequestStartTime);
        jsonWriter.WriteFieldName("DurationInMs");
        jsonWriter.WriteNumber64Value((Number64) stat.Duration.TotalMilliseconds);
        jsonWriter.WriteFieldName("RequestUri");
        jsonWriter.WriteStringValue(stat.RequestUri.ToString());
        jsonWriter.WriteFieldName("ResourceType");
        jsonWriter.WriteStringValue(stat.ResourceType.ToString());
        jsonWriter.WriteFieldName("HttpMethod");
        jsonWriter.WriteStringValue(stat.HttpMethod.ToString());
        jsonWriter.WriteFieldName("ActivityId");
        this.WriteStringValueOrNull(stat.ActivityId);
        if (stat.Exception != null)
        {
          jsonWriter.WriteFieldName("ExceptionType");
          jsonWriter.WriteStringValue(stat.Exception.GetType().ToString());
          jsonWriter.WriteFieldName("ExceptionMessage");
          jsonWriter.WriteStringValue(stat.Exception.Message);
        }
        if (stat.HttpResponseMessage != null)
        {
          jsonWriter.WriteFieldName("StatusCode");
          jsonWriter.WriteStringValue(stat.HttpResponseMessage.StatusCode.ToString());
          if (!stat.HttpResponseMessage.IsSuccessStatusCode)
          {
            jsonWriter.WriteFieldName("ReasonPhrase");
            jsonWriter.WriteStringValue(stat.HttpResponseMessage.ReasonPhrase);
          }
        }
        jsonWriter.WriteObjectEnd();
      }

      private void VisitAddressResolutionStatistics(
        ClientSideRequestStatisticsTraceDatum.AddressResolutionStatistics addressResolutionStatistics)
      {
        this.jsonWriter.WriteObjectStart();
        this.jsonWriter.WriteFieldName("StartTimeUTC");
        this.WriteDateTimeStringValue(addressResolutionStatistics.StartTime);
        this.jsonWriter.WriteFieldName("EndTimeUTC");
        if (addressResolutionStatistics.EndTime.HasValue)
          this.WriteDateTimeStringValue(addressResolutionStatistics.EndTime.Value);
        else
          this.jsonWriter.WriteStringValue("EndTime Never Set.");
        this.jsonWriter.WriteFieldName("TargetEndpoint");
        if (addressResolutionStatistics.TargetEndpoint == null)
          this.jsonWriter.WriteNullValue();
        else
          this.jsonWriter.WriteStringValue(addressResolutionStatistics.TargetEndpoint);
        this.jsonWriter.WriteObjectEnd();
      }

      private void VisitStoreResponseStatistics(
        ClientSideRequestStatisticsTraceDatum.StoreResponseStatistics storeResponseStatistics)
      {
        this.jsonWriter.WriteObjectStart();
        this.jsonWriter.WriteFieldName("ResponseTimeUTC");
        this.WriteDateTimeStringValue(storeResponseStatistics.RequestResponseTime);
        this.jsonWriter.WriteFieldName("ResourceType");
        this.jsonWriter.WriteStringValue(storeResponseStatistics.RequestResourceType.ToString());
        this.jsonWriter.WriteFieldName("OperationType");
        this.jsonWriter.WriteStringValue(storeResponseStatistics.RequestOperationType.ToString());
        if (!string.IsNullOrEmpty(storeResponseStatistics.RequestSessionToken))
        {
          this.jsonWriter.WriteFieldName("RequestSessionToken");
          this.jsonWriter.WriteStringValue(storeResponseStatistics.RequestSessionToken);
        }
        this.jsonWriter.WriteFieldName("LocationEndpoint");
        this.WriteStringValueOrNull(storeResponseStatistics.LocationEndpoint?.ToString());
        if (storeResponseStatistics.StoreResult != null)
        {
          this.jsonWriter.WriteFieldName("StoreResult");
          this.Visit(storeResponseStatistics.StoreResult);
        }
        this.jsonWriter.WriteObjectEnd();
      }

      public void Visit(CpuHistoryTraceDatum cpuHistoryTraceDatum)
      {
        if (!(this.jsonWriter is IJsonTextWriterExtensions jsonWriter))
          throw new NotImplementedException("Writing Raw Json directly to the buffer is currently only supported for text and not for binary, hybridrow");
        jsonWriter.WriteRawJsonValue((ReadOnlyMemory<byte>) Encoding.UTF8.GetBytes(cpuHistoryTraceDatum.Value.ToString()), false);
      }

      public void Visit(
        ClientConfigurationTraceDatum clientConfigurationTraceDatum)
      {
        if (!(this.jsonWriter is IJsonTextWriterExtensions jsonWriter))
          throw new NotImplementedException("Writing Raw Json directly to the buffer is currently only supported for text and not for binary, hybridrow");
        jsonWriter.WriteRawJsonValue(clientConfigurationTraceDatum.SerializedJson, false);
      }

      public void Visit(StoreResult storeResult)
      {
        this.jsonWriter.WriteObjectStart();
        this.jsonWriter.WriteFieldName("ActivityId");
        this.WriteStringValueOrNull(storeResult.ActivityId);
        this.jsonWriter.WriteFieldName("StatusCode");
        this.jsonWriter.WriteStringValue(storeResult.StatusCode.ToString());
        this.jsonWriter.WriteFieldName("SubStatusCode");
        this.jsonWriter.WriteStringValue(storeResult.SubStatusCode.ToString());
        this.jsonWriter.WriteFieldName("LSN");
        this.jsonWriter.WriteNumber64Value((Number64) storeResult.LSN);
        this.jsonWriter.WriteFieldName("PartitionKeyRangeId");
        this.WriteStringValueOrNull(storeResult.PartitionKeyRangeId);
        this.jsonWriter.WriteFieldName("GlobalCommittedLSN");
        this.jsonWriter.WriteNumber64Value((Number64) storeResult.GlobalCommittedLSN);
        this.jsonWriter.WriteFieldName("ItemLSN");
        this.jsonWriter.WriteNumber64Value((Number64) storeResult.ItemLSN);
        this.jsonWriter.WriteFieldName("UsingLocalLSN");
        this.jsonWriter.WriteBoolValue(storeResult.UsingLocalLSN);
        this.jsonWriter.WriteFieldName("QuorumAckedLSN");
        this.jsonWriter.WriteNumber64Value((Number64) storeResult.QuorumAckedLSN);
        this.jsonWriter.WriteFieldName("SessionToken");
        this.WriteStringValueOrNull(storeResult.SessionToken?.ConvertToString());
        this.jsonWriter.WriteFieldName("CurrentWriteQuorum");
        this.jsonWriter.WriteNumber64Value((Number64) (long) storeResult.CurrentWriteQuorum);
        this.jsonWriter.WriteFieldName("CurrentReplicaSetSize");
        this.jsonWriter.WriteNumber64Value((Number64) (long) storeResult.CurrentReplicaSetSize);
        this.jsonWriter.WriteFieldName("NumberOfReadRegions");
        this.jsonWriter.WriteNumber64Value((Number64) storeResult.NumberOfReadRegions);
        this.jsonWriter.WriteFieldName("IsValid");
        this.jsonWriter.WriteBoolValue(storeResult.IsValid);
        this.jsonWriter.WriteFieldName("StorePhysicalAddress");
        this.WriteStringValueOrNull(storeResult.StorePhysicalAddress?.ToString());
        this.jsonWriter.WriteFieldName("RequestCharge");
        this.jsonWriter.WriteNumber64Value((Number64) storeResult.RequestCharge);
        this.jsonWriter.WriteFieldName("RetryAfterInMs");
        this.WriteStringValueOrNull(storeResult.RetryAfterInMs);
        this.jsonWriter.WriteFieldName("BELatencyInMs");
        this.WriteStringValueOrNull(storeResult.BackendRequestDurationInMs);
        this.VisitTransportRequestStats(storeResult.TransportRequestStats);
        this.jsonWriter.WriteFieldName("TransportException");
        this.WriteStringValueOrNull(storeResult.Exception?.InnerException is TransportException innerException ? innerException.Message : (string) null);
        this.jsonWriter.WriteObjectEnd();
      }

      public void Visit(
        PartitionKeyRangeCacheTraceDatum partitionKeyRangeCacheTraceDatum)
      {
        this.jsonWriter.WriteObjectStart();
        this.jsonWriter.WriteFieldName("Previous Continuation Token");
        this.WriteStringValueOrNull(partitionKeyRangeCacheTraceDatum.PreviousContinuationToken);
        this.jsonWriter.WriteFieldName("Continuation Token");
        this.WriteStringValueOrNull(partitionKeyRangeCacheTraceDatum.ContinuationToken);
        this.jsonWriter.WriteObjectEnd();
      }

      private void WriteJsonUriArray(string propertyName, IEnumerable<TransportAddressUri> uris)
      {
        this.jsonWriter.WriteFieldName(propertyName);
        this.jsonWriter.WriteArrayStart();
        if (uris != null)
        {
          foreach (object uri in uris)
            this.WriteStringValueOrNull(uri?.ToString());
        }
        this.jsonWriter.WriteArrayEnd();
      }

      private void WriteRegionsContactedArray(string propertyName, IEnumerable<(string, Uri)> uris)
      {
        this.jsonWriter.WriteFieldName(propertyName);
        this.jsonWriter.WriteArrayStart();
        if (uris != null)
        {
          foreach ((string, Uri) uri in uris)
            this.WriteStringValueOrNull(uri.Item2?.ToString());
        }
        this.jsonWriter.WriteArrayEnd();
      }

      private void VisitTransportRequestStats(TransportRequestStats transportRequestStats)
      {
        this.jsonWriter.WriteFieldName("transportRequestTimeline");
        if (transportRequestStats == null)
        {
          this.jsonWriter.WriteNullValue();
        }
        else
        {
          if (!(this.jsonWriter is IJsonTextWriterExtensions jsonWriter))
            throw new NotImplementedException("Writing Raw Json directly to the buffer is currently only supported for text and not for binary, hybridrow");
          jsonWriter.WriteRawJsonValue((ReadOnlyMemory<byte>) Encoding.UTF8.GetBytes(transportRequestStats.ToString()), false);
        }
      }

      private void WriteJsonUriArrayWithDuplicatesCounted(
        string propertyName,
        IReadOnlyList<TransportAddressUri> uris)
      {
        this.jsonWriter.WriteFieldName(propertyName);
        this.jsonWriter.WriteArrayStart();
        if (uris != null)
        {
          Dictionary<TransportAddressUri, int> dictionary = new Dictionary<TransportAddressUri, int>();
          foreach (TransportAddressUri uri in (IEnumerable<TransportAddressUri>) uris)
          {
            if (uri != null)
            {
              if (dictionary.ContainsKey(uri))
                dictionary[uri]++;
              else
                dictionary.Add(uri, 1);
            }
          }
          foreach (KeyValuePair<TransportAddressUri, int> keyValuePair in dictionary)
          {
            this.jsonWriter.WriteObjectStart();
            this.jsonWriter.WriteFieldName("Count");
            this.jsonWriter.WriteNumber64Value((Number64) (long) keyValuePair.Value);
            this.jsonWriter.WriteFieldName("Uri");
            this.WriteStringValueOrNull(keyValuePair.Key.ToString());
            this.jsonWriter.WriteObjectEnd();
          }
        }
        this.jsonWriter.WriteArrayEnd();
      }

      private void WriteStringValueOrNull(string value)
      {
        if (value == null)
          this.jsonWriter.WriteNullValue();
        else
          this.jsonWriter.WriteStringValue(value);
      }

      private void WriteDateTimeStringValue(DateTime value) => this.jsonWriter.WriteStringValue(value.ToString("o", (IFormatProvider) CultureInfo.InvariantCulture));
    }

    private static class TraceTextWriter
    {
      private const string space = "  ";
      private static readonly Dictionary<TraceWriter.AsciiType, TraceWriter.TraceTextWriter.AsciiTreeCharacters> asciiTreeCharactersMap = new Dictionary<TraceWriter.AsciiType, TraceWriter.TraceTextWriter.AsciiTreeCharacters>()
      {
        {
          TraceWriter.AsciiType.Default,
          new TraceWriter.TraceTextWriter.AsciiTreeCharacters(' ', '├', '─', '└', '│', '.')
        },
        {
          TraceWriter.AsciiType.DoubleLine,
          new TraceWriter.TraceTextWriter.AsciiTreeCharacters(' ', '╠', '═', '╚', '║', '╗')
        },
        {
          TraceWriter.AsciiType.Classic,
          new TraceWriter.TraceTextWriter.AsciiTreeCharacters(' ', '|', '-', '+', '|', '+')
        },
        {
          TraceWriter.AsciiType.ClassicRounded,
          new TraceWriter.TraceTextWriter.AsciiTreeCharacters(' ', '|', '-', '`', '|', '+')
        },
        {
          TraceWriter.AsciiType.ExclamationMarks,
          new TraceWriter.TraceTextWriter.AsciiTreeCharacters(' ', '#', '=', '*', '!', '#')
        }
      };
      private static readonly Dictionary<TraceWriter.AsciiType, TraceWriter.TraceTextWriter.AsciiTreeIndents> asciiTreeIndentsMap = new Dictionary<TraceWriter.AsciiType, TraceWriter.TraceTextWriter.AsciiTreeIndents>()
      {
        {
          TraceWriter.AsciiType.Default,
          TraceWriter.TraceTextWriter.AsciiTreeIndents.Create(TraceWriter.TraceTextWriter.asciiTreeCharactersMap[TraceWriter.AsciiType.Default])
        },
        {
          TraceWriter.AsciiType.DoubleLine,
          TraceWriter.TraceTextWriter.AsciiTreeIndents.Create(TraceWriter.TraceTextWriter.asciiTreeCharactersMap[TraceWriter.AsciiType.DoubleLine])
        },
        {
          TraceWriter.AsciiType.Classic,
          TraceWriter.TraceTextWriter.AsciiTreeIndents.Create(TraceWriter.TraceTextWriter.asciiTreeCharactersMap[TraceWriter.AsciiType.Classic])
        },
        {
          TraceWriter.AsciiType.ClassicRounded,
          TraceWriter.TraceTextWriter.AsciiTreeIndents.Create(TraceWriter.TraceTextWriter.asciiTreeCharactersMap[TraceWriter.AsciiType.ClassicRounded])
        },
        {
          TraceWriter.AsciiType.ExclamationMarks,
          TraceWriter.TraceTextWriter.AsciiTreeIndents.Create(TraceWriter.TraceTextWriter.asciiTreeCharactersMap[TraceWriter.AsciiType.ExclamationMarks])
        }
      };
      private static readonly string[] newLines = new string[1]
      {
        Environment.NewLine
      };
      private static readonly char[] newLineCharacters = Environment.NewLine.ToCharArray();

      public static void WriteTrace(
        TextWriter writer,
        ITrace trace,
        TraceLevel level = TraceLevel.Verbose,
        TraceWriter.AsciiType asciiType = TraceWriter.AsciiType.Default)
      {
        if (writer == null)
          throw new ArgumentNullException(nameof (writer));
        if (trace == null)
          throw new ArgumentNullException(nameof (trace));
        if (trace.Level > level)
          return;
        TraceWriter.TraceTextWriter.AsciiTreeCharacters asciiTreeCharacters = TraceWriter.TraceTextWriter.asciiTreeCharactersMap[asciiType];
        TraceWriter.TraceTextWriter.AsciiTreeIndents asciiTreeIndents = TraceWriter.TraceTextWriter.asciiTreeIndentsMap[asciiType];
        writer.WriteLine(asciiTreeCharacters.Root);
        TraceWriter.TraceTextWriter.WriteTraceRecursive(writer, trace, level, asciiTreeIndents, true);
      }

      private static void WriteTraceRecursive(
        TextWriter writer,
        ITrace trace,
        TraceLevel level,
        TraceWriter.TraceTextWriter.AsciiTreeIndents asciiTreeIndents,
        bool isLastChild)
      {
        ITrace parent = trace.Parent;
        Stack<string> indentStack = new Stack<string>();
        for (; parent != null; parent = parent.Parent)
        {
          if ((parent.Parent == null ? 1 : (parent.Equals((object) parent.Parent.Children.Last<ITrace>()) ? 1 : 0)) != 0)
            indentStack.Push(asciiTreeIndents.Blank);
          else
            indentStack.Push(asciiTreeIndents.Parent);
        }
        TraceWriter.TraceTextWriter.WriteIndents(writer, indentStack, asciiTreeIndents, isLastChild);
        writer.Write(trace.Name);
        writer.Write('(');
        writer.Write((object) trace.Id);
        writer.Write(')');
        writer.Write("  ");
        writer.Write((object) trace.Component);
        writer.Write('-');
        writer.Write("Component");
        writer.Write("  ");
        writer.Write(trace.StartTime.ToString("hh:mm:ss:fff", (IFormatProvider) CultureInfo.InvariantCulture));
        writer.Write("  ");
        writer.Write(trace.Duration.TotalMilliseconds.ToString("0.00"));
        writer.Write(" milliseconds");
        writer.Write("  ");
        writer.WriteLine();
        if (trace.Data.Count > 0)
        {
          bool isLeaf = trace.Children.Count == 0;
          TraceWriter.TraceTextWriter.WriteInfoIndents(writer, indentStack, asciiTreeIndents, isLastChild, isLeaf);
          writer.WriteLine('(');
          foreach (KeyValuePair<string, object> keyValuePair in (IEnumerable<KeyValuePair<string, object>>) trace.Data)
          {
            string key = keyValuePair.Key;
            object obj = keyValuePair.Value;
            TraceWriter.TraceTextWriter.WriteInfoIndents(writer, indentStack, asciiTreeIndents, isLastChild, isLeaf);
            writer.Write(asciiTreeIndents.Blank);
            writer.Write('[');
            writer.Write(key);
            writer.Write(']');
            writer.WriteLine();
            string str1;
            if (obj is TraceDatum traceDatum)
            {
              TraceWriter.TraceTextWriter.TraceDatumTextWriter traceDatumTextWriter = new TraceWriter.TraceTextWriter.TraceDatumTextWriter();
              traceDatum.Accept((ITraceDatumVisitor) traceDatumTextWriter);
              str1 = traceDatumTextWriter.ToString();
            }
            else
              str1 = obj.ToString();
            foreach (string str2 in str1.TrimEnd(TraceWriter.TraceTextWriter.newLineCharacters).Split(TraceWriter.TraceTextWriter.newLines, StringSplitOptions.None))
            {
              TraceWriter.TraceTextWriter.WriteInfoIndents(writer, indentStack, asciiTreeIndents, isLastChild, isLeaf);
              writer.Write(asciiTreeIndents.Blank);
              writer.WriteLine(str2);
            }
          }
          TraceWriter.TraceTextWriter.WriteInfoIndents(writer, indentStack, asciiTreeIndents, isLastChild, isLeaf);
          writer.WriteLine(')');
        }
        for (int index = 0; index < trace.Children.Count - 1; ++index)
        {
          ITrace child = trace.Children[index];
          TraceWriter.TraceTextWriter.WriteTraceRecursive(writer, child, level, asciiTreeIndents, false);
        }
        if (trace.Children.Count == 0)
          return;
        ITrace child1 = trace.Children[trace.Children.Count - 1];
        TraceWriter.TraceTextWriter.WriteTraceRecursive(writer, child1, level, asciiTreeIndents, true);
      }

      private static void WriteIndents(
        TextWriter writer,
        Stack<string> indentStack,
        TraceWriter.TraceTextWriter.AsciiTreeIndents asciiTreeIndents,
        bool isLastChild)
      {
        foreach (string indent in indentStack)
          writer.Write(indent);
        if (isLastChild)
          writer.Write(asciiTreeIndents.Last);
        else
          writer.Write(asciiTreeIndents.Child);
      }

      private static void WriteInfoIndents(
        TextWriter writer,
        Stack<string> indentStack,
        TraceWriter.TraceTextWriter.AsciiTreeIndents asciiTreeIndents,
        bool isLastChild,
        bool isLeaf)
      {
        foreach (string indent in indentStack)
          writer.Write(indent);
        if (isLastChild)
          writer.Write(asciiTreeIndents.Blank);
        else
          writer.Write(asciiTreeIndents.Parent);
        if (isLeaf)
          writer.Write(asciiTreeIndents.Blank);
        else
          writer.Write(asciiTreeIndents.Parent);
      }

      private static class AddressResolutionStatisticsTextTable
      {
        private static readonly TextTable.Column[] Columns = new TextTable.Column[3]
        {
          new TextTable.Column("Start Time (utc)", TraceWriter.TraceTextWriter.AddressResolutionStatisticsTextTable.HeaderLengths.StartTime),
          new TextTable.Column("End Time (utc)", TraceWriter.TraceTextWriter.AddressResolutionStatisticsTextTable.HeaderLengths.EndTime),
          new TextTable.Column("Endpoint", TraceWriter.TraceTextWriter.AddressResolutionStatisticsTextTable.HeaderLengths.Endpoint)
        };
        public static readonly TextTable Singleton = new TextTable(TraceWriter.TraceTextWriter.AddressResolutionStatisticsTextTable.Columns);

        private static class Headers
        {
          public const string StartTime = "Start Time (utc)";
          public const string EndTime = "End Time (utc)";
          public const string Endpoint = "Endpoint";
        }

        private static class HeaderLengths
        {
          public static readonly int StartTime = Math.Max("Start Time (utc)".Length, DateTime.MaxValue.ToString("hh:mm:ss:fff", (IFormatProvider) CultureInfo.InvariantCulture).Length);
          public static readonly int EndTime = Math.Max("End Time (utc)".Length, DateTime.MaxValue.ToString("hh:mm:ss:fff", (IFormatProvider) CultureInfo.InvariantCulture).Length);
          public static readonly int Endpoint = 80 - (TraceWriter.TraceTextWriter.AddressResolutionStatisticsTextTable.HeaderLengths.StartTime + TraceWriter.TraceTextWriter.AddressResolutionStatisticsTextTable.HeaderLengths.EndTime);
        }
      }

      private sealed class TraceDatumTextWriter : ITraceDatumVisitor
      {
        private string toStringValue;

        public void Visit(QueryMetricsTraceDatum queryMetricsTraceDatum) => this.toStringValue = queryMetricsTraceDatum.QueryMetrics.ToString();

        public void Visit(
          PointOperationStatisticsTraceDatum pointOperationStatisticsTraceDatum)
        {
          StringBuilder stringBuilder = new StringBuilder();
          stringBuilder.AppendLine("Activity ID: " + (pointOperationStatisticsTraceDatum.ActivityId ?? "<null>"));
          stringBuilder.AppendLine(string.Format("Status Code: {0}/{1}", (object) pointOperationStatisticsTraceDatum.StatusCode, (object) pointOperationStatisticsTraceDatum.SubStatusCode));
          stringBuilder.AppendLine("Response Time: " + pointOperationStatisticsTraceDatum.ResponseTimeUtc.ToString("hh:mm:ss:fff", (IFormatProvider) CultureInfo.InvariantCulture));
          stringBuilder.AppendLine(string.Format("Request Charge: {0}", (object) pointOperationStatisticsTraceDatum.RequestCharge));
          stringBuilder.AppendLine("Request URI: " + (pointOperationStatisticsTraceDatum.RequestUri ?? "<null>"));
          stringBuilder.AppendLine("Session Tokens: " + (pointOperationStatisticsTraceDatum.RequestSessionToken ?? "<null>") + " / " + (pointOperationStatisticsTraceDatum.ResponseSessionToken ?? "<null>"));
          if (pointOperationStatisticsTraceDatum.ErrorMessage != null)
            stringBuilder.AppendLine("Error Message: " + pointOperationStatisticsTraceDatum.ErrorMessage);
          this.toStringValue = stringBuilder.ToString();
        }

        public void Visit(
          ClientSideRequestStatisticsTraceDatum clientSideRequestStatisticsTraceDatum)
        {
          StringBuilder stringBuilder1 = new StringBuilder();
          stringBuilder1.AppendLine("Start Time: " + clientSideRequestStatisticsTraceDatum.RequestStartTimeUtc.ToString("hh:mm:ss:fff", (IFormatProvider) CultureInfo.InvariantCulture));
          if (clientSideRequestStatisticsTraceDatum.RequestEndTimeUtc.HasValue)
            stringBuilder1.AppendLine("End Time: " + clientSideRequestStatisticsTraceDatum.RequestEndTimeUtc.Value.ToString("hh:mm:ss:fff", (IFormatProvider) CultureInfo.InvariantCulture));
          stringBuilder1.AppendLine("Contacted Replicas");
          Dictionary<TransportAddressUri, int> dictionary = new Dictionary<TransportAddressUri, int>();
          foreach (TransportAddressUri contactedReplica in clientSideRequestStatisticsTraceDatum.ContactedReplicas)
          {
            if (contactedReplica != null)
            {
              int num;
              if (!dictionary.TryGetValue(contactedReplica, out num))
                num = 0;
              dictionary[contactedReplica] = ++num;
            }
          }
          foreach (KeyValuePair<TransportAddressUri, int> keyValuePair in dictionary)
            stringBuilder1.AppendLine(string.Format("{0}{1}: {2}", (object) "  ", (object) (keyValuePair.Key?.ToString() ?? "<null>"), (object) keyValuePair.Value));
          stringBuilder1.AppendLine("Failed to Contact Replicas");
          foreach (TransportAddressUri failedReplica in clientSideRequestStatisticsTraceDatum.FailedReplicas)
            stringBuilder1.AppendLine("  " + (failedReplica?.ToString() ?? "<null>"));
          stringBuilder1.AppendLine("Regions Contacted");
          foreach (TransportAddressUri contactedReplica in clientSideRequestStatisticsTraceDatum.ContactedReplicas)
            stringBuilder1.AppendLine("  " + (contactedReplica?.ToString() ?? "<null>"));
          stringBuilder1.AppendLine("Address Resolution Statistics");
          stringBuilder1.AppendLine(TraceWriter.TraceTextWriter.AddressResolutionStatisticsTextTable.Singleton.TopLine);
          stringBuilder1.AppendLine(TraceWriter.TraceTextWriter.AddressResolutionStatisticsTextTable.Singleton.Header);
          stringBuilder1.AppendLine(TraceWriter.TraceTextWriter.AddressResolutionStatisticsTextTable.Singleton.MiddleLine);
          foreach (KeyValuePair<string, ClientSideRequestStatisticsTraceDatum.AddressResolutionStatistics> resolutionStatistic in (IEnumerable<KeyValuePair<string, ClientSideRequestStatisticsTraceDatum.AddressResolutionStatistics>>) clientSideRequestStatisticsTraceDatum.EndpointToAddressResolutionStatistics)
          {
            TextTable singleton = TraceWriter.TraceTextWriter.AddressResolutionStatisticsTextTable.Singleton;
            object[] objArray = new object[3];
            ClientSideRequestStatisticsTraceDatum.AddressResolutionStatistics resolutionStatistics = resolutionStatistic.Value;
            objArray[0] = (object) resolutionStatistics.StartTime.ToString("hh:mm:ss:fff", (IFormatProvider) CultureInfo.InvariantCulture);
            resolutionStatistics = resolutionStatistic.Value;
            DateTime? endTime = resolutionStatistics.EndTime;
            string str;
            if (!endTime.HasValue)
            {
              str = "NO END TIME";
            }
            else
            {
              resolutionStatistics = resolutionStatistic.Value;
              endTime = resolutionStatistics.EndTime;
              str = endTime.Value.ToString("hh:mm:ss:fff", (IFormatProvider) CultureInfo.InvariantCulture);
            }
            objArray[1] = (object) str;
            resolutionStatistics = resolutionStatistic.Value;
            objArray[2] = (object) resolutionStatistics.TargetEndpoint;
            string row = singleton.GetRow(objArray);
            stringBuilder1.AppendLine(row);
          }
          stringBuilder1.AppendLine(TraceWriter.TraceTextWriter.AddressResolutionStatisticsTextTable.Singleton.BottomLine);
          stringBuilder1.AppendLine("Store Response Statistics");
          foreach (ClientSideRequestStatisticsTraceDatum.StoreResponseStatistics responseStatistics in (IEnumerable<ClientSideRequestStatisticsTraceDatum.StoreResponseStatistics>) clientSideRequestStatisticsTraceDatum.StoreResponseStatisticsList)
          {
            DateTime? requestStartTime = responseStatistics.RequestStartTime;
            if (requestStartTime.HasValue)
            {
              StringBuilder stringBuilder2 = stringBuilder1;
              requestStartTime = responseStatistics.RequestStartTime;
              string str = "  Start Time: " + requestStartTime.Value.ToString("hh:mm:ss:fff", (IFormatProvider) CultureInfo.InvariantCulture);
              stringBuilder2.AppendLine(str);
            }
            else
              stringBuilder1.AppendLine("{space}Start Time Not Found");
            stringBuilder1.AppendLine("  End Time: " + responseStatistics.RequestResponseTime.ToString("hh:mm:ss:fff", (IFormatProvider) CultureInfo.InvariantCulture));
            stringBuilder1.AppendLine(string.Format("{0}Resource Type: {1}", (object) "  ", (object) responseStatistics.RequestResourceType));
            stringBuilder1.AppendLine(string.Format("{0}Operation Type: {1}", (object) "  ", (object) responseStatistics.RequestOperationType));
            if (responseStatistics.StoreResult != null)
            {
              stringBuilder1.AppendLine("  Store Result");
              stringBuilder1.AppendLine("    Activity Id: " + (responseStatistics.StoreResult.ActivityId ?? "<null>"));
              stringBuilder1.AppendLine("    Store Physical Address: " + (responseStatistics.StoreResult.StorePhysicalAddress?.ToString() ?? "<null>"));
              stringBuilder1.AppendLine(string.Format("{0}{1}Status Code: {2}/{3}", (object) "  ", (object) "  ", (object) responseStatistics.StoreResult.StatusCode, (object) responseStatistics.StoreResult.SubStatusCode));
              stringBuilder1.AppendLine(string.Format("{0}{1}Is Valid: {2}", (object) "  ", (object) "  ", (object) responseStatistics.StoreResult.IsValid));
              stringBuilder1.AppendLine("    LSN Info");
              stringBuilder1.AppendLine(string.Format("{0}{1}{2}LSN: {3}", (object) "  ", (object) "  ", (object) "  ", (object) responseStatistics.StoreResult.LSN));
              stringBuilder1.AppendLine(string.Format("{0}{1}{2}Item LSN: {3}", (object) "  ", (object) "  ", (object) "  ", (object) responseStatistics.StoreResult.ItemLSN));
              stringBuilder1.AppendLine(string.Format("{0}{1}{2}Global LSN: {3}", (object) "  ", (object) "  ", (object) "  ", (object) responseStatistics.StoreResult.GlobalCommittedLSN));
              stringBuilder1.AppendLine(string.Format("{0}{1}{2}Quorum Acked LSN: {3}", (object) "  ", (object) "  ", (object) "  ", (object) responseStatistics.StoreResult.QuorumAckedLSN));
              stringBuilder1.AppendLine(string.Format("{0}{1}{2}Using LSN: {3}", (object) "  ", (object) "  ", (object) "  ", (object) responseStatistics.StoreResult.UsingLocalLSN));
              stringBuilder1.AppendLine("    Session Token: " + (responseStatistics.StoreResult.SessionToken?.ConvertToString() ?? "<null>"));
              stringBuilder1.AppendLine("    Quorum Info");
              stringBuilder1.AppendLine(string.Format("{0}{1}{2}Current Replica Set Size: {3}", (object) "  ", (object) "  ", (object) "  ", (object) responseStatistics.StoreResult.CurrentReplicaSetSize));
              stringBuilder1.AppendLine(string.Format("{0}{1}{2}Current Write Quorum: {3}", (object) "  ", (object) "  ", (object) "  ", (object) responseStatistics.StoreResult.CurrentWriteQuorum));
              stringBuilder1.AppendLine("    Exception");
              try
              {
                stringBuilder1.AppendLine(string.Format("{0}{1}{2}", (object) "  ", (object) "  ", (object) responseStatistics.StoreResult.GetException()));
              }
              catch (Exception ex)
              {
              }
            }
          }
          if (clientSideRequestStatisticsTraceDatum.HttpResponseStatisticsList.Any<ClientSideRequestStatisticsTraceDatum.HttpResponseStatistics>())
          {
            stringBuilder1.AppendLine("Http Response Statistics");
            foreach (ClientSideRequestStatisticsTraceDatum.HttpResponseStatistics responseStatistics in (IEnumerable<ClientSideRequestStatisticsTraceDatum.HttpResponseStatistics>) clientSideRequestStatisticsTraceDatum.HttpResponseStatisticsList)
            {
              stringBuilder1.AppendLine("  HttpResponse");
              stringBuilder1.AppendLine("    RequestStartTime: " + responseStatistics.RequestStartTime.ToString("o", (IFormatProvider) CultureInfo.InvariantCulture));
              stringBuilder1.AppendLine(string.Format("{0}{1}DurationInMs: {2:0.00}", (object) "  ", (object) "  ", (object) responseStatistics.Duration.TotalMilliseconds));
              stringBuilder1.AppendLine(string.Format("{0}{1}RequestUri: {2}", (object) "  ", (object) "  ", (object) responseStatistics.RequestUri));
              stringBuilder1.AppendLine(string.Format("{0}{1}ResourceType: {2}", (object) "  ", (object) "  ", (object) responseStatistics.ResourceType));
              stringBuilder1.AppendLine(string.Format("{0}{1}HttpMethod: {2}", (object) "  ", (object) "  ", (object) responseStatistics.HttpMethod));
              if (responseStatistics.Exception != null)
              {
                stringBuilder1.AppendLine(string.Format("{0}{1}ExceptionType: {2}", (object) "  ", (object) "  ", (object) responseStatistics.Exception.GetType()));
                stringBuilder1.AppendLine("    ExceptionMessage: " + responseStatistics.Exception.Message);
              }
              if (responseStatistics.HttpResponseMessage != null)
              {
                stringBuilder1.AppendLine(string.Format("{0}{1}StatusCode: {2}", (object) "  ", (object) "  ", (object) responseStatistics.HttpResponseMessage.StatusCode));
                if (!responseStatistics.HttpResponseMessage.IsSuccessStatusCode)
                  stringBuilder1.AppendLine("    ReasonPhrase: " + responseStatistics.HttpResponseMessage.ReasonPhrase);
              }
            }
          }
          this.toStringValue = stringBuilder1.ToString();
        }

        public void Visit(CpuHistoryTraceDatum cpuHistoryTraceDatum)
        {
          StringBuilder stringBuilder = new StringBuilder();
          stringBuilder.AppendLine(cpuHistoryTraceDatum.Value.ToString());
          this.toStringValue = stringBuilder.ToString();
        }

        public void Visit(
          ClientConfigurationTraceDatum clientConfigurationTraceDatum)
        {
          StringBuilder stringBuilder = new StringBuilder();
          stringBuilder.AppendLine("Client Configuration");
          stringBuilder.AppendLine("Client Created Time: " + clientConfigurationTraceDatum.ClientCreatedDateTimeUtc.ToString("o", (IFormatProvider) CultureInfo.InvariantCulture));
          stringBuilder.AppendLine("Machine Id: " + VmMetadataApiHandler.GetMachineId());
          stringBuilder.AppendLine(string.Format("Number Of Clients Created: {0}", (object) CosmosClient.numberOfClientsCreated));
          stringBuilder.AppendLine(string.Format("Number Of Active Clients: {0}", (object) CosmosClient.NumberOfActiveClients));
          stringBuilder.AppendLine(string.Format("Connection Mode: {0}", (object) clientConfigurationTraceDatum.ConnectionMode));
          stringBuilder.AppendLine("User Agent: " + clientConfigurationTraceDatum.UserAgentContainer.UserAgent);
          stringBuilder.AppendLine("Connection Config:");
          stringBuilder.AppendLine(string.Format("{0}'gw': {1}", (object) "  ", (object) clientConfigurationTraceDatum.GatewayConnectionConfig));
          stringBuilder.AppendLine(string.Format("{0}'rntbd': {1}", (object) "  ", (object) clientConfigurationTraceDatum.RntbdConnectionConfig));
          stringBuilder.AppendLine(string.Format("{0}'other': {1}", (object) "  ", (object) clientConfigurationTraceDatum.OtherConnectionConfig));
          stringBuilder.AppendLine(string.Format("Consistency Config: {0}", (object) clientConfigurationTraceDatum.ConsistencyConfig));
          this.toStringValue = stringBuilder.ToString();
        }

        public override string ToString() => this.toStringValue;

        public void Visit(
          PartitionKeyRangeCacheTraceDatum partitionKeyRangeCacheTraceDatum)
        {
          StringBuilder stringBuilder = new StringBuilder();
          stringBuilder.AppendLine("Previous Continuation Token: " + (partitionKeyRangeCacheTraceDatum.PreviousContinuationToken ?? "<null>"));
          stringBuilder.AppendLine("Continuation Token: " + (partitionKeyRangeCacheTraceDatum.ContinuationToken ?? "<null>"));
          this.toStringValue = stringBuilder.ToString();
        }
      }

      private readonly struct AsciiTreeCharacters
      {
        public AsciiTreeCharacters(
          char blank,
          char child,
          char dash,
          char last,
          char parent,
          char root)
        {
          this.Blank = blank;
          this.Child = child;
          this.Dash = dash;
          this.Last = last;
          this.Parent = parent;
          this.Root = root;
        }

        public char Blank { get; }

        public char Child { get; }

        public char Dash { get; }

        public char Last { get; }

        public char Parent { get; }

        public char Root { get; }
      }

      private readonly struct AsciiTreeIndents
      {
        private AsciiTreeIndents(string child, string parent, string last, string blank)
        {
          this.Child = child;
          this.Parent = parent;
          this.Last = last;
          this.Blank = blank;
        }

        public string Child { get; }

        public string Parent { get; }

        public string Last { get; }

        public string Blank { get; }

        public static TraceWriter.TraceTextWriter.AsciiTreeIndents Create(
          TraceWriter.TraceTextWriter.AsciiTreeCharacters asciiTreeCharacters)
        {
          return new TraceWriter.TraceTextWriter.AsciiTreeIndents(new string(new char[4]
          {
            asciiTreeCharacters.Child,
            asciiTreeCharacters.Dash,
            asciiTreeCharacters.Dash,
            asciiTreeCharacters.Blank
          }), new string(new char[4]
          {
            asciiTreeCharacters.Parent,
            asciiTreeCharacters.Blank,
            asciiTreeCharacters.Blank,
            asciiTreeCharacters.Blank
          }), new string(new char[4]
          {
            asciiTreeCharacters.Last,
            asciiTreeCharacters.Dash,
            asciiTreeCharacters.Dash,
            asciiTreeCharacters.Blank
          }), new string(new char[4]
          {
            asciiTreeCharacters.Blank,
            asciiTreeCharacters.Blank,
            asciiTreeCharacters.Blank,
            asciiTreeCharacters.Blank
          }));
        }
      }
    }
  }
}
