// Decompiled with JetBrains decompiler
// Type: HdrHistogram.TimeStamp
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Documents;

namespace HdrHistogram
{
  internal static class TimeStamp
  {
    public static long Seconds(long seconds) => ValueStopwatch.Frequency * seconds;

    public static long Minutes(long minutes) => minutes * TimeStamp.Seconds(60L);

    public static long Hours(int hours) => (long) hours * TimeStamp.Minutes(60L);
  }
}
