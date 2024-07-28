// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.Cache.ScopedId
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Identity.Cache
{
  [DataContract]
  internal class ScopedId : ICloneable
  {
    internal ScopedId()
    {
    }

    internal ScopedId(Guid scopeId, Guid id)
    {
      this.ScopeId = scopeId;
      this.Id = id;
    }

    [DataMember]
    internal Guid ScopeId { get; private set; }

    [DataMember]
    internal Guid Id { get; private set; }

    public bool Equals(ScopedId other)
    {
      if (this == other)
        return true;
      return other != null && object.Equals((object) this.Id, (object) other.Id) && object.Equals((object) this.ScopeId, (object) other.ScopeId);
    }

    public override bool Equals(object obj)
    {
      if (this == obj)
        return true;
      return obj != null && this.GetType() == obj.GetType() && this.Equals((ScopedId) obj);
    }

    public override int GetHashCode()
    {
      Guid guid = this.Id;
      int hashCode = guid.GetHashCode();
      guid = this.ScopeId;
      int num = 23 * guid.GetHashCode();
      return hashCode + num;
    }

    public object Clone() => (object) new ScopedId(this.ScopeId, this.Id);
  }
}
