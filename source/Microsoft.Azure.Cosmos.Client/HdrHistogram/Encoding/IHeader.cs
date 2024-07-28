// Decompiled with JetBrains decompiler
// Type: HdrHistogram.Encoding.IHeader
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

namespace HdrHistogram.Encoding
{
  internal interface IHeader
  {
    int Cookie { get; }

    int PayloadLengthInBytes { get; }

    int NormalizingIndexOffset { get; }

    int NumberOfSignificantValueDigits { get; }

    long LowestTrackableUnitValue { get; }

    long HighestTrackableValue { get; }

    double IntegerToDoubleValueConversionRatio { get; }

    int CapacityEstimateExcess { get; }
  }
}
