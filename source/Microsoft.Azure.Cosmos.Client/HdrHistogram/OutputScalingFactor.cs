// Decompiled with JetBrains decompiler
// Type: HdrHistogram.OutputScalingFactor
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Documents;

namespace HdrHistogram
{
  internal static class OutputScalingFactor
  {
    public const double None = 1.0;
    public static readonly double TimeStampToMicroseconds = (double) ValueStopwatch.Frequency / 1000000.0;
    public static readonly double TimeStampToMilliseconds = (double) ValueStopwatch.Frequency / 1000.0;
    public static readonly double TimeStampToSeconds = (double) ValueStopwatch.Frequency;
  }
}
