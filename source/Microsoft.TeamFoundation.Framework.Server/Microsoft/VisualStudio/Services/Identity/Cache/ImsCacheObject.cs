// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.Cache.ImsCacheObject
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Identity.Cache
{
  [DataContract]
  internal abstract class ImsCacheObject : ICloneable
  {
    internal ImsCacheObject()
    {
    }

    internal ImsCacheObject(ImsCacheKey key, object value)
      : this(key, value, DateTimeOffset.UtcNow)
    {
    }

    internal ImsCacheObject(ImsCacheKey key, object value, DateTimeOffset time)
    {
      this.Key = key;
      this.Value = value;
      this.Time = time;
    }

    [DataMember]
    public ImsCacheKey Key { get; private set; }

    [IgnoreDataMember]
    public object Value { get; protected set; }

    [DataMember]
    public DateTimeOffset Time { get; private set; }

    public bool Equals(ImsCacheObject other)
    {
      if (this == other)
        return true;
      return other != null && object.Equals((object) this.Key, (object) other.Key) && this.ValueEquals(other.Value) && object.Equals((object) this.Time, (object) other.Time);
    }

    public override bool Equals(object obj)
    {
      if (this == obj)
        return true;
      return obj != null && this.GetType() == obj.GetType() && this.Equals((ImsCacheObject) obj);
    }

    public override int GetHashCode() => this.Key.GetHashCode() + 23 * this.Value.GetHashCode() + 23 * this.Time.GetHashCode();

    public abstract object Clone();

    protected abstract bool ValueEquals(object rhs);

    public override string ToString() => "(" + (object) this.Key + ", " + this.Value + ", " + (object) this.Time + ")";

    internal bool IsExpired(TimeSpan timeToLive) => this.IsExpired(timeToLive, DateTimeOffset.UtcNow);

    internal bool IsExpired(TimeSpan timeToLive, DateTimeOffset now) => this.Time + timeToLive < now;

    internal static bool IsNullOrExpired(ImsCacheObject obj, TimeSpan timeToLive) => ImsCacheObject.IsNullOrExpired(obj, timeToLive, DateTimeOffset.UtcNow);

    internal static bool IsNullOrExpired(
      ImsCacheObject obj,
      TimeSpan timeToLive,
      DateTimeOffset now)
    {
      return obj == null || obj.IsExpired(timeToLive, now);
    }

    internal bool IsMoreRecent(ImsCacheObject other) => other == null || other.Time < this.Time;

    internal static bool IsMoreRecent(ImsCacheObject obj, ImsCacheObject other) => obj != null && obj.IsMoreRecent(other);

    internal static T GetMostRecent<T>(params T[] objs) where T : ImsCacheObject => ((IEnumerable<T>) objs).Where<T>((Func<T, bool>) (obj => (object) obj != null)).OrderBy<T, DateTimeOffset>((Func<T, DateTimeOffset>) (obj => obj.Time)).LastOrDefault<T>();
  }
}
