// Decompiled with JetBrains decompiler
// Type: HdrHistogram.Encoding.RecordedData
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

namespace HdrHistogram.Encoding
{
  internal sealed class RecordedData : IRecordedData
  {
    public RecordedData(
      int cookie,
      int normalizingIndexOffset,
      int numberOfSignificantValueDigits,
      long lowestDiscernibleValue,
      long highestTrackableValue,
      double integerToDoubleValueConversionRatio,
      long[] counts)
    {
      this.Cookie = cookie;
      this.NormalizingIndexOffset = normalizingIndexOffset;
      this.NumberOfSignificantValueDigits = numberOfSignificantValueDigits;
      this.LowestDiscernibleValue = lowestDiscernibleValue;
      this.HighestTrackableValue = highestTrackableValue;
      this.IntegerToDoubleValueConversionRatio = integerToDoubleValueConversionRatio;
      this.Counts = counts;
    }

    public int Cookie { get; }

    public int NormalizingIndexOffset { get; }

    public int NumberOfSignificantValueDigits { get; }

    public long LowestDiscernibleValue { get; }

    public long HighestTrackableValue { get; }

    public double IntegerToDoubleValueConversionRatio { get; }

    public long[] Counts { get; }
  }
}
