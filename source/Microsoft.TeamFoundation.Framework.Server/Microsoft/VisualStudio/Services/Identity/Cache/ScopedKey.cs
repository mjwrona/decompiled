// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.Cache.ScopedKey
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Identity.Cache
{
  [DataContract]
  internal class ScopedKey : ICloneable
  {
    internal ScopedKey()
    {
    }

    internal ScopedKey(Guid scopeId, string key)
    {
      this.ScopeId = scopeId;
      this.Key = key;
    }

    [DataMember]
    internal Guid ScopeId { get; private set; }

    [DataMember]
    internal string Key { get; private set; }

    public bool Equals(ScopedKey other)
    {
      if (this == other)
        return true;
      return other != null && object.Equals((object) this.Key, (object) other.Key) && object.Equals((object) this.ScopeId, (object) other.ScopeId);
    }

    public override bool Equals(object obj)
    {
      if (this == obj)
        return true;
      return obj != null && this.GetType() == obj.GetType() && this.Equals((ScopedKey) obj);
    }

    public override int GetHashCode()
    {
      string key = this.Key;
      return (key != null ? key.GetHashCode() : -1) + 23 * this.ScopeId.GetHashCode();
    }

    public object Clone() => (object) new ScopedKey(this.ScopeId, this.Key);
  }
}
