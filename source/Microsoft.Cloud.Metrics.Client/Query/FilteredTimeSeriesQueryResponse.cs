// Decompiled with JetBrains decompiler
// Type: Microsoft.Cloud.Metrics.Client.Query.FilteredTimeSeriesQueryResponse
// Assembly: Microsoft.Cloud.Metrics.Client, Version=2.2023.705.2051, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 06B39E1C-7DF0-4BC1-AFBA-9AD635E73CB0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Cloud.Metrics.Client.dll

using Microsoft.Cloud.Metrics.Client.Metrics;
using Microsoft.Online.Metrics.Serialization;
using Microsoft.Online.Metrics.Serialization.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Microsoft.Cloud.Metrics.Client.Query
{
  public sealed class FilteredTimeSeriesQueryResponse : IFilteredTimeSeriesQueryResponse
  {
    public const byte VersionToIndicateCompleteFailure = 255;
    public const byte CurrentVersion = 3;
    public const byte NextVersion = 4;

    public FilteredTimeSeriesQueryResponse()
    {
    }

    internal FilteredTimeSeriesQueryResponse(
      DateTime startTime,
      DateTime endTime,
      int timeResolutionInMinutes,
      IReadOnlyList<FilteredTimeSeries> filteredTimeSeriesList,
      string errorMessage = null)
    {
      this.StartTimeUtc = startTime;
      this.EndTimeUtc = endTime;
      this.TimeResolutionInMinutes = timeResolutionInMinutes;
      this.FilteredTimeSeriesList = (IReadOnlyList<IFilteredTimeSeries>) filteredTimeSeriesList;
      this.DiagnosticInfo = new DiagnosticInfo()
      {
        ErrorMessage = errorMessage
      };
    }

    public FilteredTimeSeriesQueryRequest QueryRequest { get; private set; }

    public DateTime EndTimeUtc { get; private set; }

    public DateTime StartTimeUtc { get; private set; }

    public int TimeResolutionInMinutes { get; private set; }

    public IReadOnlyList<IFilteredTimeSeries> FilteredTimeSeriesList { get; private set; }

    public FilteredTimeSeriesQueryResponseErrorCode ErrorCode { get; private set; }

    public DiagnosticInfo DiagnosticInfo { get; private set; }

    public void Deserialize(BinaryReader reader)
    {
      byte version;
      uint resultTimeSeriesCount;
      Dictionary<int, string> stringTable;
      long stringTableLengthInByte;
      FilteredTimeSeriesQueryResponse.SeriesMetadata seriesMetadata;
      Dictionary<int, FilteredTimeSeriesQueryResponse.SeriesMetadata> metadataTable;
      long metadataTableLengthInByte;
      if (!this.ReadPreamble(reader, out version, out resultTimeSeriesCount, out stringTable, out stringTableLengthInByte, out seriesMetadata, out metadataTable, out metadataTableLengthInByte))
        return;
      FilteredTimeSeries[] filteredTimeSeriesArray = new FilteredTimeSeries[(int) resultTimeSeriesCount];
      for (int index = 0; (long) index < (long) resultTimeSeriesCount; ++index)
        filteredTimeSeriesArray[index] = FilteredTimeSeriesQueryResponse.ReadTimeSeries(version, reader, seriesMetadata, stringTable, metadataTable);
      this.FilteredTimeSeriesList = (IReadOnlyList<IFilteredTimeSeries>) filteredTimeSeriesArray;
      reader.BaseStream.Position += metadataTableLengthInByte + stringTableLengthInByte;
      if (version <= (byte) 1)
        return;
      int num1 = (int) reader.ReadByte();
      for (int index = 0; index < num1; ++index)
      {
        int num2 = (int) reader.ReadByte();
        int num3 = (int) reader.ReadByte();
        int num4 = (int) reader.ReadByte();
        reader.ReadString();
        if (version > (byte) 3)
          reader.ReadString();
      }
    }

    public IEnumerable<FilteredTimeSeries> ReadFilteredTimeSeries(BinaryReader reader)
    {
      int numOfResponses = (int) SerializationUtils.ReadUInt32FromBase128(reader);
      for (int j = 0; j < numOfResponses; ++j)
      {
        int num = (int) reader.ReadByte();
        int numOfQueryResults = reader.ReadInt32();
        for (int k = 0; k < numOfQueryResults; ++k)
        {
          byte version;
          uint resultTimeSeriesCount;
          Dictionary<int, string> stringTable;
          FilteredTimeSeriesQueryResponse.SeriesMetadata seriesMetadata;
          Dictionary<int, FilteredTimeSeriesQueryResponse.SeriesMetadata> metadataTable;
          if (!this.ReadPreamble(reader, out version, out resultTimeSeriesCount, out stringTable, out long _, out seriesMetadata, out metadataTable, out long _))
          {
            yield break;
          }
          else
          {
            for (int i = 0; (long) i < (long) resultTimeSeriesCount; ++i)
              yield return FilteredTimeSeriesQueryResponse.ReadTimeSeries(version, reader, seriesMetadata, stringTable, metadataTable);
            stringTable = (Dictionary<int, string>) null;
            seriesMetadata = (FilteredTimeSeriesQueryResponse.SeriesMetadata) null;
            metadataTable = (Dictionary<int, FilteredTimeSeriesQueryResponse.SeriesMetadata>) null;
          }
        }
      }
    }

    public IEnumerable<FilteredTimeSeries> ReadFilteredTimeSeriesFromKqlQuery(BinaryReader reader)
    {
      int num = (int) reader.ReadByte();
      if (!reader.ReadBoolean())
        throw new MetricsClientException("The query failed and please try the query with Jarvis to see the error messages.");
      byte version;
      uint resultTimeSeriesCount;
      Dictionary<int, string> stringTable;
      FilteredTimeSeriesQueryResponse.SeriesMetadata seriesMetadata;
      Dictionary<int, FilteredTimeSeriesQueryResponse.SeriesMetadata> metadataTable;
      if (this.ReadPreamble(reader, out version, out resultTimeSeriesCount, out stringTable, out long _, out seriesMetadata, out metadataTable, out long _))
      {
        for (int i = 0; (long) i < (long) resultTimeSeriesCount; ++i)
          yield return FilteredTimeSeriesQueryResponse.ReadTimeSeries(version, reader, seriesMetadata, stringTable, metadataTable);
      }
    }

    private static FilteredTimeSeriesQueryResponse.SeriesMetadata DeserializeTimeSeriesMetadata(
      BinaryReader reader,
      Dictionary<int, string> stringTable)
    {
      MetricIdentifier metricIdentifier;
      // ISSUE: explicit constructor call
      ((MetricIdentifier) ref metricIdentifier).\u002Ector(FilteredTimeSeriesQueryResponse.DeserializeStringByIndex(reader, stringTable), FilteredTimeSeriesQueryResponse.DeserializeStringByIndex(reader, stringTable), FilteredTimeSeriesQueryResponse.DeserializeStringByIndex(reader, stringTable));
      byte length = reader.ReadByte();
      string[] dimensionNames = new string[(int) length];
      for (int index = 0; index < (int) length; ++index)
        dimensionNames[index] = FilteredTimeSeriesQueryResponse.DeserializeStringByIndex(reader, stringTable);
      return new FilteredTimeSeriesQueryResponse.SeriesMetadata(metricIdentifier, dimensionNames);
    }

    private static FilteredTimeSeries ReadTimeSeries(
      byte version,
      BinaryReader reader,
      FilteredTimeSeriesQueryResponse.SeriesMetadata seriesMetadata,
      Dictionary<int, string> stringTable,
      Dictionary<int, FilteredTimeSeriesQueryResponse.SeriesMetadata> metadataTable)
    {
      if (version >= (byte) 3)
      {
        int key = (int) SerializationUtils.ReadUInt32FromBase128(reader);
        seriesMetadata = metadataTable[key];
      }
      MetricIdentifier metricIdentifier = seriesMetadata.MetricIdentifier;
      string[] dimensionNames = seriesMetadata.DimensionNames;
      int length1 = dimensionNames.Length;
      KeyValuePair<string, string>[] dimensionList = new KeyValuePair<string, string>[length1];
      for (int index = 0; index < length1; ++index)
        dimensionList[index] = new KeyValuePair<string, string>(dimensionNames[index], FilteredTimeSeriesQueryResponse.DeserializeStringByIndex(reader, stringTable));
      byte length2 = reader.ReadByte();
      KeyValuePair<string, double>[] keyValuePairArray = new KeyValuePair<string, double>[(int) length2];
      for (int index = 0; index < (int) length2; ++index)
        keyValuePairArray[index] = new KeyValuePair<string, double>(FilteredTimeSeriesQueryResponse.DeserializeStringByIndex(reader, stringTable), reader.ReadDouble());
      byte length3 = reader.ReadByte();
      KeyValuePair<SamplingType, double[]>[] seriesValues = new KeyValuePair<SamplingType, double[]>[(int) length3];
      for (int index = 0; index < (int) length3; ++index)
      {
        string str = FilteredTimeSeriesQueryResponse.DeserializeStringByIndex(reader, stringTable);
        SamplingType key = SamplingType.BuiltInSamplingTypes.ContainsKey(str) ? SamplingType.BuiltInSamplingTypes[str] : new SamplingType(str);
        seriesValues[index] = new KeyValuePair<SamplingType, double[]>(key, DoubleValueSerializer.Deserialize(reader));
      }
      double evaluatedResult = length2 == (byte) 0 ? double.NaN : keyValuePairArray[0].Value;
      return new FilteredTimeSeries(metricIdentifier, (IReadOnlyList<KeyValuePair<string, string>>) dimensionList, evaluatedResult, (IReadOnlyList<KeyValuePair<SamplingType, double[]>>) seriesValues);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static string DeserializeStringByIndex(
      BinaryReader reader,
      Dictionary<int, string> stringTable)
    {
      int key = (int) SerializationUtils.ReadUInt32FromBase128(reader);
      return stringTable[key];
    }

    private bool ReadPreamble(
      BinaryReader reader,
      out byte version,
      out uint resultTimeSeriesCount,
      out Dictionary<int, string> stringTable,
      out long stringTableLengthInByte,
      out FilteredTimeSeriesQueryResponse.SeriesMetadata seriesMetadata,
      out Dictionary<int, FilteredTimeSeriesQueryResponse.SeriesMetadata> metadataTable,
      out long metadataTableLengthInByte)
    {
      resultTimeSeriesCount = 0U;
      stringTable = (Dictionary<int, string>) null;
      stringTableLengthInByte = 0L;
      seriesMetadata = (FilteredTimeSeriesQueryResponse.SeriesMetadata) null;
      metadataTable = (Dictionary<int, FilteredTimeSeriesQueryResponse.SeriesMetadata>) null;
      metadataTableLengthInByte = 0L;
      version = reader.ReadByte();
      if (version == (byte) 0)
        throw new MetricsClientException("The server didn't respond with the right version of serialization - the initial version is 1 but the server responds with version 0.");
      this.DiagnosticInfo = new DiagnosticInfo();
      if (version == byte.MaxValue)
      {
        this.ErrorCode = (FilteredTimeSeriesQueryResponseErrorCode) reader.ReadInt16();
        this.DiagnosticInfo.ErrorMessage = reader.ReadString();
        if (reader.ReadBoolean())
          this.QueryRequest = JsonConvert.DeserializeObject<FilteredTimeSeriesQueryRequest>(reader.ReadString());
        return false;
      }
      if (version > (byte) 4)
        throw new MetricsClientException(string.Format("The server didn't respond with the right version of serialization. CurrentVersion : {0}, NextVersion : {1}, VersionInResponse: {2}.", (object) (byte) 3, (object) (byte) 4, (object) version));
      if (reader.ReadBoolean())
        new QueryResultQualityInfo().Deserialize(reader);
      this.StartTimeUtc = new DateTime((long) SerializationUtils.ReadUInt64FromBase128(reader) * 600000000L, DateTimeKind.Utc);
      this.EndTimeUtc = this.StartTimeUtc.AddMinutes((double) SerializationUtils.ReadUInt32FromBase128(reader));
      this.TimeResolutionInMinutes = (int) SerializationUtils.ReadUInt32FromBase128(reader);
      resultTimeSeriesCount = SerializationUtils.ReadUInt32FromBase128(reader);
      long position1 = reader.BaseStream.Position;
      ulong num1 = reader.ReadUInt64();
      reader.BaseStream.Position = position1 + (long) num1;
      long position2 = reader.BaseStream.Position;
      uint capacity1 = SerializationUtils.ReadUInt32FromBase128(reader);
      stringTable = new Dictionary<int, string>((int) capacity1);
      for (int key = 0; (long) key < (long) capacity1; ++key)
        stringTable.Add(key, reader.ReadString());
      stringTableLengthInByte = reader.BaseStream.Position - position2;
      reader.BaseStream.Position = position1 + 8L;
      if (version >= (byte) 3)
      {
        long position3 = reader.BaseStream.Position;
        ulong num2 = reader.ReadUInt64();
        reader.BaseStream.Position = position3 + (long) num2;
        long position4 = reader.BaseStream.Position;
        uint capacity2 = SerializationUtils.ReadUInt32FromBase128(reader);
        metadataTable = new Dictionary<int, FilteredTimeSeriesQueryResponse.SeriesMetadata>((int) capacity2);
        for (int key = 0; (long) key < (long) capacity2; ++key)
          metadataTable.Add(key, FilteredTimeSeriesQueryResponse.DeserializeTimeSeriesMetadata(reader, stringTable));
        metadataTableLengthInByte = reader.BaseStream.Position - position4;
        reader.BaseStream.Position = position3 + 8L;
      }
      if (resultTimeSeriesCount > 0U)
      {
        if (version < (byte) 3)
        {
          seriesMetadata = FilteredTimeSeriesQueryResponse.DeserializeTimeSeriesMetadata(reader, stringTable);
          this.QueryRequest = new FilteredTimeSeriesQueryRequest(seriesMetadata.MetricIdentifier);
        }
        else
          this.QueryRequest = new FilteredTimeSeriesQueryRequest(metadataTable.Values.First<FilteredTimeSeriesQueryResponse.SeriesMetadata>().MetricIdentifier);
      }
      return true;
    }

    private sealed class SeriesMetadata
    {
      public SeriesMetadata(MetricIdentifier metricIdentifier, string[] dimensionNames)
      {
        this.MetricIdentifier = metricIdentifier;
        this.DimensionNames = dimensionNames;
      }

      public MetricIdentifier MetricIdentifier { get; }

      public string[] DimensionNames { get; }
    }
  }
}
