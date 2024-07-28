// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.Common.Timestamp
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using System;
using System.Diagnostics;

namespace Microsoft.Azure.NotificationHubs.Common
{
  internal struct Timestamp : IComparable<Timestamp>, IEquatable<Timestamp>
  {
    private static readonly double TickFrequency = 10000000.0 / (double) Stopwatch.Frequency;
    private readonly long timestamp;

    private Timestamp(long timestamp) => this.timestamp = timestamp;

    public static Timestamp Now => new Timestamp(Stopwatch.GetTimestamp());

    public TimeSpan Elapsed => new TimeSpan(this.GetElapsedDateTimeTicks());

    public long ElapsedTicks => this.GetElapsedDateTimeTicks();

    private static long ConvertRawTicksToTicks(long rawTicks) => Stopwatch.IsHighResolution ? (long) ((double) rawTicks * Timestamp.TickFrequency) : rawTicks;

    private long GetRawElapsedTicks() => Stopwatch.GetTimestamp() - this.timestamp;

    private long GetElapsedDateTimeTicks() => Timestamp.ConvertRawTicksToTicks(this.GetRawElapsedTicks());

    public int CompareTo(Timestamp other) => this.timestamp.CompareTo(other.timestamp);

    public bool Equals(Timestamp other) => this.timestamp == other.timestamp;

    public override int GetHashCode() => this.timestamp.GetHashCode();

    public override bool Equals(object obj) => obj is Timestamp other && this.Equals(other);

    public static bool operator ==(Timestamp t1, Timestamp t2) => t1.timestamp == t2.timestamp;

    public static bool operator !=(Timestamp t1, Timestamp t2) => t1.timestamp != t2.timestamp;

    public static bool operator >(Timestamp t1, Timestamp t2) => t1.timestamp > t2.timestamp;

    public static bool operator <(Timestamp t1, Timestamp t2) => t1.timestamp < t2.timestamp;

    public static bool operator >=(Timestamp t1, Timestamp t2) => t1.timestamp >= t2.timestamp;

    public static bool operator <=(Timestamp t1, Timestamp t2) => t1.timestamp <= t2.timestamp;

    public static Timestamp operator +(Timestamp t, TimeSpan duration) => new Timestamp((long) ((double) t.timestamp + (double) duration.Ticks / Timestamp.TickFrequency));

    public static Timestamp operator -(Timestamp t, TimeSpan duration) => new Timestamp((long) ((double) t.timestamp - (double) duration.Ticks / Timestamp.TickFrequency));

    public static TimeSpan operator -(Timestamp t1, Timestamp t2) => new TimeSpan(Timestamp.ConvertRawTicksToTicks(t1.timestamp - t2.timestamp));
  }
}
