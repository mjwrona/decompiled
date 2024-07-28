// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Aad.Cache.AadCacheLookup`1
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using System;

namespace Microsoft.VisualStudio.Services.Aad.Cache
{
  internal class AadCacheLookup<T> where T : AadCacheObject
  {
    private readonly AadCacheKey key;
    private readonly AadCacheLookupStatus status;
    private readonly T result;
    private readonly Exception exception;

    internal AadCacheLookup(AadCacheKey key, AadCacheLookupStatus status, Exception exception)
      : this(key, status, default (T), exception)
    {
    }

    internal AadCacheLookup(AadCacheKey key, T result)
      : this(key, (object) result != null ? AadCacheLookupStatus.Hit : AadCacheLookupStatus.Miss, result, (Exception) null)
    {
    }

    internal AadCacheLookup(
      AadCacheKey key,
      AadCacheLookupStatus status,
      T result,
      Exception exception)
    {
      this.key = key;
      this.status = status;
      this.result = result;
      this.exception = exception;
    }

    public AadCacheKey Key => this.key;

    public AadCacheLookupStatus Status => this.status;

    public T Result => this.result;

    public Exception Exception => this.exception;

    public override bool Equals(object obj) => this == obj || this.Equals(obj as AadCacheLookup<T>);

    public bool Equals(AadCacheLookup<T> other)
    {
      if (this == other)
        return true;
      return other != null && object.Equals((object) this.Key, (object) other.key) && object.Equals((object) this.status, (object) other.status) && object.Equals((object) this.result, (object) other.result);
    }

    public override int GetHashCode()
    {
      int hashCode = (5059 * 4099 + this.key.GetHashCode()) * 4099 + this.status.GetHashCode();
      if ((object) this.result != null)
        hashCode = hashCode * 4099 + this.result.GetHashCode();
      return hashCode;
    }
  }
}
