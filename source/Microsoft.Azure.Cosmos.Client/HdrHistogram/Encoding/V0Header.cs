// Decompiled with JetBrains decompiler
// Type: HdrHistogram.Encoding.V0Header
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using HdrHistogram.Utilities;

namespace HdrHistogram.Encoding
{
  internal sealed class V0Header : IHeader
  {
    public V0Header(int cookie, ByteBuffer buffer)
    {
      this.Cookie = cookie;
      this.NumberOfSignificantValueDigits = buffer.GetInt();
      this.LowestTrackableUnitValue = buffer.GetLong();
      this.HighestTrackableValue = buffer.GetLong();
      this.PayloadLengthInBytes = int.MaxValue;
      this.IntegerToDoubleValueConversionRatio = 1.0;
      this.NormalizingIndexOffset = 0;
    }

    public int Cookie { get; }

    public int PayloadLengthInBytes { get; }

    public int NormalizingIndexOffset { get; }

    public int NumberOfSignificantValueDigits { get; }

    public long LowestTrackableUnitValue { get; }

    public long HighestTrackableValue { get; }

    public double IntegerToDoubleValueConversionRatio { get; }

    public int CapacityEstimateExcess => 32;
  }
}
