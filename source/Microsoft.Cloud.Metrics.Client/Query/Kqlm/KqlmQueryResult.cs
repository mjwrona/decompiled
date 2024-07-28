// Decompiled with JetBrains decompiler
// Type: Microsoft.Cloud.Metrics.Client.Query.Kqlm.KqlmQueryResult
// Assembly: Microsoft.Cloud.Metrics.Client, Version=2.2023.705.2051, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 06B39E1C-7DF0-4BC1-AFBA-9AD635E73CB0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Cloud.Metrics.Client.dll

using Microsoft.Cloud.Metrics.Client.Metrics;
using Microsoft.Online.Metrics.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Microsoft.Cloud.Metrics.Client.Query.Kqlm
{
  internal sealed class KqlmQueryResult : IKqlmQueryResult
  {
    public string RequestID { get; private set; }

    public QueryState State { get; private set; }

    public IEnumerable<ITimeSeriesSet> TimeSeriesSets { get; private set; }

    public IEnumerable<IExecutionMessage> ExecutionMessages { get; private set; }

    public void Deserialize(BinaryReader reader)
    {
      int num1 = (int) reader.ReadByte();
      if (reader.ReadBoolean())
      {
        byte version;
        DateTime startTimeUtc;
        DateTime endTimeUtc;
        TimeSpan dataResolution;
        uint resultTimeSeriesCount;
        Dictionary<int, string> stringTable;
        long stringTableLengthInByte;
        KqlmQueryResult.SeriesMetadata seriesMetadata;
        Dictionary<int, KqlmQueryResult.SeriesMetadata> metadataTable;
        long metadataTableLengthInByte;
        if (!this.ReadPreamble(reader, out version, out startTimeUtc, out endTimeUtc, out dataResolution, out resultTimeSeriesCount, out stringTable, out stringTableLengthInByte, out seriesMetadata, out metadataTable, out metadataTableLengthInByte))
        {
          this.State = QueryState.Failed;
          throw new MetricsClientException("Failed to deserialize KQL-M query result.");
        }
        List<ITimeSeriesData> timeSeriesDataList = new List<ITimeSeriesData>((int) resultTimeSeriesCount);
        for (int index = 0; (long) index < (long) resultTimeSeriesCount; ++index)
          timeSeriesDataList.Add((ITimeSeriesData) this.ReadTimeSeries(version, reader, seriesMetadata, stringTable, metadataTable));
        List<string> resultantDimensions = new List<string>();
        List<string> resultantSamplingTypes = new List<string>();
        if (timeSeriesDataList.Count > 0)
        {
          foreach (KeyValuePair<string, string> dimensionValue in timeSeriesDataList.FirstOrDefault<ITimeSeriesData>().DimensionValues)
            resultantDimensions.Add(dimensionValue.Key);
          foreach (KeyValuePair<string, IEnumerable<double>> keyValuePair in timeSeriesDataList.FirstOrDefault<ITimeSeriesData>().SamplingTypesData)
            resultantSamplingTypes.Add(keyValuePair.Key);
        }
        this.TimeSeriesSets = (IEnumerable<ITimeSeriesSet>) new List<ITimeSeriesSet>()
        {
          (ITimeSeriesSet) new TimeSeriesSet((IResultsMetadata) new ResultsMetadata(startTimeUtc, endTimeUtc, dataResolution, (IEnumerable<string>) resultantDimensions, (IEnumerable<string>) resultantSamplingTypes, (long) resultTimeSeriesCount), (IEnumerable<ITimeSeriesData>) timeSeriesDataList)
        };
        reader.BaseStream.Position += metadataTableLengthInByte + stringTableLengthInByte;
      }
      List<IExecutionMessage> source = new List<IExecutionMessage>();
      int num2 = reader.ReadInt32();
      for (int index = 0; index < num2; ++index)
      {
        reader.ReadString();
        int severity = reader.ReadInt32();
        string text = reader.ReadString();
        string documentationLink = reader.ReadString();
        int num3 = reader.ReadInt32();
        int num4 = reader.ReadInt32();
        int lineNumber = reader.ReadInt32();
        int num5 = reader.ReadInt32();
        int charPositionInLine = num4;
        int charPositionAbsolute = num3;
        int errorSectionLength = num5;
        StatementContextInformation statementContext = new StatementContextInformation(lineNumber, charPositionInLine, charPositionAbsolute, errorSectionLength);
        source.Add((IExecutionMessage) new ExecutionMessage((MessageSeverity) severity, text, documentationLink, statementContext));
      }
      this.ExecutionMessages = (IEnumerable<IExecutionMessage>) source;
      this.State = source.Any<IExecutionMessage>((Func<IExecutionMessage, bool>) (m => m.Severity == MessageSeverity.Error)) ? QueryState.Failed : QueryState.Success;
    }

    private bool ReadPreamble(
      BinaryReader reader,
      out byte version,
      out DateTime startTimeUtc,
      out DateTime endTimeUtc,
      out TimeSpan dataResolution,
      out uint resultTimeSeriesCount,
      out Dictionary<int, string> stringTable,
      out long stringTableLengthInByte,
      out KqlmQueryResult.SeriesMetadata seriesMetadata,
      out Dictionary<int, KqlmQueryResult.SeriesMetadata> metadataTable,
      out long metadataTableLengthInByte)
    {
      resultTimeSeriesCount = 0U;
      stringTable = (Dictionary<int, string>) null;
      stringTableLengthInByte = 0L;
      seriesMetadata = (KqlmQueryResult.SeriesMetadata) null;
      metadataTable = (Dictionary<int, KqlmQueryResult.SeriesMetadata>) null;
      metadataTableLengthInByte = 0L;
      version = reader.ReadByte();
      if (version == (byte) 0)
        throw new MetricsClientException("The server didn't respond with the right version of serialization - the initial version is 1 but the server responds with version 0.");
      if (version > (byte) 3)
        throw new MetricsClientException(string.Format("The server didn't respond with the right version of serialization. CurrentVersion : {0}, NextVersion : {1}, Responded: {2}.", (object) 3, (object) 3, (object) version));
      if (reader.ReadBoolean())
        new QueryResultQualityInfo().Deserialize(reader);
      startTimeUtc = new DateTime((long) SerializationUtils.ReadUInt64FromBase128(reader) * 600000000L, DateTimeKind.Utc);
      endTimeUtc = startTimeUtc.AddMinutes((double) SerializationUtils.ReadUInt32FromBase128(reader));
      dataResolution = TimeSpan.FromMinutes((double) (int) SerializationUtils.ReadUInt32FromBase128(reader));
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
        metadataTable = new Dictionary<int, KqlmQueryResult.SeriesMetadata>((int) capacity2);
        for (int key = 0; (long) key < (long) capacity2; ++key)
          metadataTable.Add(key, this.DeserializeTimeSeriesMetadata(reader, stringTable));
        metadataTableLengthInByte = reader.BaseStream.Position - position4;
        reader.BaseStream.Position = position3 + 8L;
      }
      if (resultTimeSeriesCount > 0U && version < (byte) 3)
        seriesMetadata = this.DeserializeTimeSeriesMetadata(reader, stringTable);
      return true;
    }

    private KqlmQueryResult.SeriesMetadata DeserializeTimeSeriesMetadata(
      BinaryReader reader,
      Dictionary<int, string> stringTable)
    {
      string monitoringAccount = this.DeserializeStringByIndex(reader, stringTable);
      string metricNamespace = this.DeserializeStringByIndex(reader, stringTable);
      string metricName = this.DeserializeStringByIndex(reader, stringTable);
      byte length = reader.ReadByte();
      string[] dimensionNames = new string[(int) length];
      for (int index = 0; index < (int) length; ++index)
        dimensionNames[index] = this.DeserializeStringByIndex(reader, stringTable);
      return new KqlmQueryResult.SeriesMetadata(monitoringAccount, metricNamespace, metricName, dimensionNames);
    }

    private TimeSeriesData ReadTimeSeries(
      byte version,
      BinaryReader reader,
      KqlmQueryResult.SeriesMetadata seriesMetadata,
      Dictionary<int, string> stringTable,
      Dictionary<int, KqlmQueryResult.SeriesMetadata> metadataTable)
    {
      if (version >= (byte) 3)
      {
        int key = SerializationUtils.ReadInt32FromBase128(reader);
        seriesMetadata = metadataTable[key];
      }
      string[] dimensionNames = seriesMetadata.DimensionNames;
      int length1 = dimensionNames.Length;
      Dictionary<string, string> dimensionValues = new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      for (int index = 0; index < length1; ++index)
        dimensionValues.Add(dimensionNames[index], this.DeserializeStringByIndex(reader, stringTable));
      byte length2 = reader.ReadByte();
      KeyValuePair<string, double>[] keyValuePairArray = new KeyValuePair<string, double>[(int) length2];
      for (int index = 0; index < (int) length2; ++index)
        keyValuePairArray[index] = new KeyValuePair<string, double>(this.DeserializeStringByIndex(reader, stringTable), reader.ReadDouble());
      byte num = reader.ReadByte();
      Dictionary<string, IEnumerable<double>> samplingTypesData = new Dictionary<string, IEnumerable<double>>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      for (int index = 0; index < (int) num; ++index)
      {
        string key = this.DeserializeStringByIndex(reader, stringTable);
        samplingTypesData.Add(key, (IEnumerable<double>) DoubleValueSerializer.Deserialize(reader));
      }
      return new TimeSeriesData((IEnumerable<KeyValuePair<string, string>>) dimensionValues, (IEnumerable<KeyValuePair<string, IEnumerable<double>>>) samplingTypesData);
    }

    private string DeserializeStringByIndex(
      BinaryReader reader,
      Dictionary<int, string> stringTable)
    {
      int key = (int) SerializationUtils.ReadUInt32FromBase128(reader);
      return stringTable[key];
    }

    private sealed class SeriesMetadata
    {
      public SeriesMetadata(
        string monitoringAccount,
        string metricNamespace,
        string metricName,
        string[] dimensionNames)
      {
        this.MonitoringAccount = monitoringAccount;
        this.MetricNamespace = metricNamespace;
        this.MetricName = metricName;
        this.DimensionNames = dimensionNames;
      }

      public string MonitoringAccount { get; }

      public string MetricNamespace { get; }

      public string MetricName { get; }

      public string[] DimensionNames { get; }
    }
  }
}
