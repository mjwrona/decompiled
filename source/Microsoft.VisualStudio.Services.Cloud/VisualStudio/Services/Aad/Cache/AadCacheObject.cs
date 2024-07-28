// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Aad.Cache.AadCacheObject
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Aad.Cache
{
  [DataContract]
  internal abstract class AadCacheObject
  {
    internal AadCacheObject()
    {
    }

    internal AadCacheObject(AadCacheKey key, object value)
      : this(key, value, DateTimeOffset.UtcNow)
    {
    }

    internal AadCacheObject(AadCacheKey key, object value, DateTimeOffset time)
    {
      this.Key = key;
      this.Value = value;
      this.Time = time;
    }

    internal abstract AadCacheObject WithTime(DateTimeOffset time);

    [DataMember]
    public AadCacheKey Key { get; private set; }

    [IgnoreDataMember]
    public object Value { get; protected set; }

    [DataMember]
    public DateTimeOffset Time { get; private set; }

    public bool Equals(AadCacheObject other)
    {
      if (this == other)
        return true;
      return other != null && object.Equals((object) this.Key, (object) other.Key) && this.ValueEquals(other.Value) && object.Equals((object) this.Time, (object) other.Time);
    }

    public override bool Equals(object obj) => this == obj || this.Equals(obj as AadCacheObject);

    protected virtual bool ValueEquals(object rhs) => this.Value.Equals(rhs);

    public override int GetHashCode() => ((1231 * 3037 + this.Key.GetHashCode()) * 3037 + this.Value.GetHashCode()) * 3037 + this.Time.GetHashCode();

    public override string ToString() => string.Format("AadCacheObject{0}Key={1},Value={2},Time={3}{4}", (object) "{", (object) this.Key, this.Value, (object) this.Time, (object) "}");

    internal bool IsOlderThan(TimeSpan threshold) => this.IsOlderThan(threshold, DateTimeOffset.UtcNow);

    internal bool IsOlderThan(TimeSpan threshold, DateTimeOffset now) => this.Time + threshold < now;

    internal static bool IsNullOrOlderThan(AadCacheObject obj, TimeSpan threshold) => AadCacheObject.IsNullOrOlderThan(obj, threshold, DateTimeOffset.UtcNow);

    internal static bool IsNullOrOlderThan(
      AadCacheObject obj,
      TimeSpan threshold,
      DateTimeOffset now)
    {
      return obj == null || obj.IsOlderThan(threshold, now);
    }

    internal bool IsMoreRecent(AadCacheObject other) => other == null || other.Time < this.Time;

    internal static bool IsMoreRecent(AadCacheObject obj, AadCacheObject other) => obj != null && obj.IsMoreRecent(other);

    internal static T GetMostRecent<T>(params T[] objs) where T : AadCacheObject
    {
      T other = default (T);
      foreach (T obj in objs)
      {
        if (AadCacheObject.IsMoreRecent((AadCacheObject) obj, (AadCacheObject) other))
          other = obj;
      }
      return other;
    }
  }
}
