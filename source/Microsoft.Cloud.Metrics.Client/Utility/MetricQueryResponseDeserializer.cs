// Decompiled with JetBrains decompiler
// Type: Microsoft.Cloud.Metrics.Client.Utility.MetricQueryResponseDeserializer
// Assembly: Microsoft.Cloud.Metrics.Client, Version=2.2023.705.2051, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 06B39E1C-7DF0-4BC1-AFBA-9AD635E73CB0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Cloud.Metrics.Client.dll

using Microsoft.Cloud.Metrics.Client.Metrics;
using Microsoft.Cloud.Metrics.Client.Query;
using Microsoft.Online.Metrics.Serialization;
using Microsoft.Online.Metrics.Serialization.BitHelper;
using Microsoft.Online.Metrics.Serialization.Configuration;
using System;
using System.Collections.Generic;
using System.IO;

namespace Microsoft.Cloud.Metrics.Client.Utility
{
  public sealed class MetricQueryResponseDeserializer
  {
    public const string NumDataPointsHeader = "__NumDataPoints__";
    public const byte CurrentVersion = 3;
    public const byte NextVersion = 3;
    public const int NumBitsToEncodeNumMeaningfulBits = 6;
    private const int NumBitsToEncodeNumLeadingZeros = 5;
    private const int NumBitsToEncodeNumLeadingZerosCorrected = 6;
    private static readonly SamplingType[] SamplingTypesWithValueTypeOfLong = new SamplingType[9]
    {
      SamplingType.Count,
      SamplingType.Sum,
      SamplingType.Min,
      SamplingType.Max,
      SamplingType.Percentile50th,
      SamplingType.Percentile90th,
      SamplingType.Percentile95th,
      SamplingType.Percentile99th,
      SamplingType.Percentile999th
    };
    private static readonly DateTime DefaultDateTime = new DateTime();

    public static int GetNumBitsToEncodeNumLeadingZeros(int version) => version < 3 ? 5 : 6;

    public static Tuple<string[], TimeSeries<MetricIdentifier, double?>[]> Deserialize(
      Stream stream,
      IReadOnlyList<TimeSeriesDefinition<MetricIdentifier>> definitions,
      int numSamplingTypesRequested = -1)
    {
      if (!MultipleStampsQueryResponseSerializer.IsMergedResponseStream(stream))
        return MetricQueryResponseDeserializer.DeserializeImpl(stream, definitions, numSamplingTypesRequested);
      if (numSamplingTypesRequested == -1)
        throw new ArgumentException("Merged responses cannot be deserialized without numSamplingTypesRequested parameter.");
      using (BinaryReader reader = new BinaryReader(stream))
      {
        List<string> stringList = new List<string>();
        List<TimeSeries<MetricIdentifier, double?>> timeSeriesList = new List<TimeSeries<MetricIdentifier, double?>>();
        foreach (Stream enumerateMergeResponse in MultipleStampsQueryResponseSerializer.EnumerateMergeResponses(reader))
        {
          using (enumerateMergeResponse)
          {
            Tuple<string[], TimeSeries<MetricIdentifier, double?>[]> tuple = MetricQueryResponseDeserializer.DeserializeImpl(enumerateMergeResponse, definitions, numSamplingTypesRequested);
            if (tuple != null)
            {
              if (tuple.Item1 != null && tuple.Item1.Length != 0)
                stringList.AddRange((IEnumerable<string>) tuple.Item1);
              if (tuple.Item2 != null)
              {
                if (tuple.Item2.Length != 0)
                  timeSeriesList.AddRange((IEnumerable<TimeSeries<MetricIdentifier, double?>>) tuple.Item2);
              }
            }
          }
        }
        return Tuple.Create<string[], TimeSeries<MetricIdentifier, double?>[]>(stringList.ToArray(), timeSeriesList.ToArray());
      }
    }

    public static bool IsMetricValueTypeLong(
      SamplingType samplingType,
      int seriesResolutionInMinutes,
      AggregationType aggregationType)
    {
      if (aggregationType == AggregationType.None && seriesResolutionInMinutes > 1)
        return false;
      for (int index = 0; index < MetricQueryResponseDeserializer.SamplingTypesWithValueTypeOfLong.Length; ++index)
      {
        if (MetricQueryResponseDeserializer.SamplingTypesWithValueTypeOfLong[index].Equals(samplingType))
          return true;
      }
      return false;
    }

    private static Tuple<string[], TimeSeries<MetricIdentifier, double?>[]> DeserializeImpl(
      Stream stream,
      IReadOnlyList<TimeSeriesDefinition<MetricIdentifier>> definitions,
      int numSamplingTypesRequested = -1)
    {
      if (definitions == null && numSamplingTypesRequested <= 0)
        throw new ArgumentException(string.Format("{0} must be > 0 when {1} is null.", (object) numSamplingTypesRequested, (object) nameof (definitions)));
      using (BinaryReader reader = new BinaryReader(stream))
      {
        byte version = reader.ReadByte();
        if (version > (byte) 3)
          throw new MetricsClientException(string.Format("The server didn't respond with the right version of serialization. CurrentVersion : {0}, NextVersion : {1}, Responded: {2}.", (object) (byte) 3, (object) (byte) 3, (object) version));
        short length1 = reader.ReadInt16();
        string[] strArray;
        if (length1 == (short) 0)
        {
          strArray = (string[]) null;
        }
        else
        {
          strArray = new string[(int) length1];
          for (int index = 0; index < (int) length1; ++index)
            strArray[index] = reader.ReadString();
        }
        uint length2 = SerializationUtils.ReadUInt32FromBase128(reader);
        TimeSeries<MetricIdentifier, double?>[] timeSeriesArray = new TimeSeries<MetricIdentifier, double?>[(int) length2];
        for (int index = 0; (long) index < (long) length2; ++index)
        {
          MetricQueryResponseDeserializer.DeserializedRawData deserializedRawData = MetricQueryResponseDeserializer.DeserializeOneSeries(version, reader, definitions?[index], numSamplingTypesRequested);
          if (definitions == null)
          {
            timeSeriesArray[index] = new TimeSeries<MetricIdentifier, double?>(MetricQueryResponseDeserializer.DefaultDateTime, MetricQueryResponseDeserializer.DefaultDateTime, 1, (TimeSeriesDefinition<MetricIdentifier>) null, deserializedRawData.Values, deserializedRawData.ErrorCode);
          }
          else
          {
            TimeSeriesDefinition<MetricIdentifier> definition = definitions[index];
            if (deserializedRawData.Values != null)
            {
              DateTime startTimeUtc = definition.StartTimeUtc.AddMinutes((double) deserializedRawData.DeltaOfStartTimeInMinutes);
              int seriesResolutionInMinutes = definition.SeriesResolutionInMinutes + deserializedRawData.DeltaOfSeriesResolutionInMinutes;
              DateTime endTimeUtc = startTimeUtc + TimeSpan.FromMinutes((double) (seriesResolutionInMinutes * (deserializedRawData.Values[0].Count - 1)));
              timeSeriesArray[index] = new TimeSeries<MetricIdentifier, double?>(startTimeUtc, endTimeUtc, seriesResolutionInMinutes, definition, deserializedRawData.Values, deserializedRawData.ErrorCode);
            }
            else
              timeSeriesArray[index] = new TimeSeries<MetricIdentifier, double?>(definition.StartTimeUtc, definition.EndTimeUtc, definition.SeriesResolutionInMinutes, definition, deserializedRawData.Values, deserializedRawData.ErrorCode);
          }
        }
        return Tuple.Create<string[], TimeSeries<MetricIdentifier, double?>[]>(strArray, timeSeriesArray);
      }
    }

    private static MetricQueryResponseDeserializer.DeserializedRawData DeserializeOneSeries(
      byte version,
      BinaryReader reader,
      TimeSeriesDefinition<MetricIdentifier> definition,
      int numSamplingTypesRequested)
    {
      int num1 = SerializationUtils.ReadInt32FromBase128(reader);
      if (num1 < 0)
        return new MetricQueryResponseDeserializer.DeserializedRawData(0, 0, (List<List<double?>>) null, (TimeSeriesErrorCode) num1);
      uint num2 = 0;
      uint scalingFactor = 1;
      if (version == (byte) 1)
      {
        num2 = SerializationUtils.ReadUInt32FromBase128(reader);
        scalingFactor = SerializationUtils.ReadUInt32FromBase128(reader);
      }
      int num3 = num1 - (int) num2;
      int capacity = definition != null ? definition.SamplingTypes.Length : numSamplingTypesRequested;
      List<List<double?>> values = new List<List<double?>>(capacity);
      for (int index = 0; index < capacity; ++index)
        values.Add(new List<double?>(num1));
      double[] priorValidValues = new double[capacity];
      bool[] sampleTyesWithMetricValueTypeLong = new bool[capacity];
      sbyte[] currentBlockLeadingZeros = new sbyte[capacity];
      sbyte[] currentBlockTrailingZeros = new sbyte[capacity];
      for (int index = 0; index < capacity; ++index)
      {
        priorValidValues[index] = 0.0;
        currentBlockLeadingZeros[index] = (sbyte) -1;
        currentBlockTrailingZeros[index] = (sbyte) -1;
        if (definition == null || MetricQueryResponseDeserializer.IsMetricValueTypeLong(definition.SamplingTypes[index], definition.SeriesResolutionInMinutes, definition.AggregationType))
          sampleTyesWithMetricValueTypeLong[index] = true;
      }
      bool sparseData = num2 > 0U;
      BitBinaryReader reader1 = new BitBinaryReader(reader);
      for (int dataPointIndex = 0; dataPointIndex < num3; ++dataPointIndex)
      {
        if (version == (byte) 1)
          MetricQueryResponseDeserializer.DeserializeForOneTimestampV1(reader, definition, sparseData, values, sampleTyesWithMetricValueTypeLong, priorValidValues, scalingFactor);
        else
          MetricQueryResponseDeserializer.DeserializeForOneTimestampV2AndAbove(version, reader1, definition, values, dataPointIndex, currentBlockLeadingZeros, currentBlockTrailingZeros, priorValidValues);
      }
      if (sparseData && values[0].Count < num1)
        MetricQueryResponseDeserializer.FillNulls((uint) (num1 - values[0].Count), values);
      return new MetricQueryResponseDeserializer.DeserializedRawData(SerializationUtils.ReadInt32FromBase128(reader), SerializationUtils.ReadInt32FromBase128(reader), values, TimeSeriesErrorCode.Success);
    }

    private static void DeserializeForOneTimestampV1(
      BinaryReader reader,
      TimeSeriesDefinition<MetricIdentifier> definition,
      bool sparseData,
      List<List<double?>> values,
      bool[] sampleTyesWithMetricValueTypeLong,
      double[] priorValidValues,
      uint scalingFactor)
    {
      if (sparseData)
        MetricQueryResponseDeserializer.FillNulls(SerializationUtils.ReadUInt32FromBase128(reader), values);
      for (int index = 0; index < values.Count; ++index)
      {
        if (sampleTyesWithMetricValueTypeLong[index])
        {
          long num1 = SerializationUtils.ReadInt64FromBase128(reader);
          if (num1 == long.MaxValue)
          {
            values[index].Add(new double?());
          }
          else
          {
            double num2 = (double) num1 + priorValidValues[index];
            if (MetricQueryResponseDeserializer.IsCountSamplingType(definition, index))
              values[index].Add(new double?(num2));
            else
              values[index].Add(new double?(num2 / (double) scalingFactor));
            priorValidValues[index] = num2;
          }
        }
        else
        {
          double d = reader.ReadDouble();
          if (double.IsNaN(d))
          {
            values[index].Add(new double?());
          }
          else
          {
            double num = d + priorValidValues[index];
            if (MetricQueryResponseDeserializer.IsCountSamplingType(definition, index))
              values[index].Add(new double?(d + priorValidValues[index]));
            else
              values[index].Add(new double?((d + priorValidValues[index]) / (double) scalingFactor));
            priorValidValues[index] = num;
          }
        }
      }
    }

    private static void DeserializeForOneTimestampV2AndAbove(
      byte version,
      BitBinaryReader reader,
      TimeSeriesDefinition<MetricIdentifier> definition,
      List<List<double?>> values,
      int dataPointIndex,
      sbyte[] currentBlockLeadingZeros,
      sbyte[] currentBlockTrailingZeros,
      double[] priorValidValues)
    {
      int count = values.Count;
      for (int index = 0; index < count; ++index)
      {
        if (dataPointIndex == 0)
        {
          priorValidValues[index] = reader.BinaryReader.ReadDouble();
          values[index].Add(MetricQueryResponseDeserializer.GetNullableDouble(priorValidValues[index]));
        }
        else if (!reader.ReadBit())
        {
          values[index].Add(MetricQueryResponseDeserializer.GetNullableDouble(priorValidValues[index]));
        }
        else
        {
          long num1;
          if (!reader.ReadBit())
          {
            if (currentBlockLeadingZeros[index] < (sbyte) 0)
              throw new Exception("The block has not been set so it is a bug in serialization on server");
            int num2 = 64 - (int) currentBlockLeadingZeros[index] - (int) currentBlockTrailingZeros[index];
            num1 = reader.ReadBits(num2);
          }
          else
          {
            currentBlockLeadingZeros[index] = (sbyte) reader.ReadBits(MetricQueryResponseDeserializer.GetNumBitsToEncodeNumLeadingZeros((int) version));
            sbyte num3 = (sbyte) reader.ReadBits(6);
            if (num3 == (sbyte) 0)
              num3 = (sbyte) 64;
            currentBlockTrailingZeros[index] = (sbyte) (64 - (int) currentBlockLeadingZeros[index] - (int) num3);
            num1 = reader.ReadBits((int) num3);
          }
          long num4 = num1 << (int) currentBlockTrailingZeros[index];
          priorValidValues[index] = BitConverter.Int64BitsToDouble(num4 ^ BitConverter.DoubleToInt64Bits(priorValidValues[index]));
          values[index].Add(MetricQueryResponseDeserializer.GetNullableDouble(priorValidValues[index]));
        }
      }
    }

    private static double? GetNullableDouble(double priorValidValue) => !double.IsPositiveInfinity(priorValidValue) ? new double?(priorValidValue) : new double?();

    private static bool IsCountSamplingType(
      TimeSeriesDefinition<MetricIdentifier> definition,
      int samplingTypeIndex)
    {
      if (definition == null && samplingTypeIndex == 0)
        return true;
      return definition != null && definition.SamplingTypes[samplingTypeIndex].Equals(SamplingType.Count);
    }

    private static void FillNulls(uint numMissingDatapointsSinceLastOne, List<List<double?>> values)
    {
      int count = values.Count;
      for (int index1 = 0; (long) index1 < (long) numMissingDatapointsSinceLastOne; ++index1)
      {
        for (int index2 = 0; index2 < count; ++index2)
          values[index2].Add(new double?());
      }
    }

    private struct DeserializedRawData
    {
      internal DeserializedRawData(
        int deltaOfStartTimeInMinutes,
        int deltaOfSeriesResolutionInMinutes,
        List<List<double?>> values,
        TimeSeriesErrorCode errorCode)
      {
        this.DeltaOfStartTimeInMinutes = deltaOfStartTimeInMinutes;
        this.DeltaOfSeriesResolutionInMinutes = deltaOfSeriesResolutionInMinutes;
        this.Values = values;
        this.ErrorCode = errorCode;
      }

      internal int DeltaOfStartTimeInMinutes { get; }

      internal int DeltaOfSeriesResolutionInMinutes { get; }

      internal List<List<double?>> Values { get; }

      internal TimeSeriesErrorCode ErrorCode { get; }
    }
  }
}
