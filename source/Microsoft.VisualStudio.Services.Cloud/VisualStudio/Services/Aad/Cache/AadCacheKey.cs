// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Aad.Cache.AadCacheKey
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Aad.Cache
{
  [DataContract]
  internal sealed class AadCacheKey
  {
    internal AadCacheKey()
    {
    }

    internal AadCacheKey(Guid tenantId, Guid objectId)
    {
      this.TenantId = tenantId;
      this.ObjectId = objectId;
    }

    [DataMember]
    internal Guid TenantId { get; private set; }

    [DataMember]
    internal Guid ObjectId { get; private set; }

    public bool Equals(AadCacheKey other)
    {
      if (this == other)
        return true;
      return other != null && object.Equals((object) this.TenantId, (object) other.TenantId) && object.Equals((object) this.ObjectId, (object) other.ObjectId);
    }

    public override bool Equals(object obj) => this == obj || this.Equals(obj as AadCacheKey);

    public override int GetHashCode() => (7001 * 8999 + this.TenantId.GetHashCode()) * 8999 + this.ObjectId.GetHashCode();

    public override string ToString() => string.Format("AadCacheKey{0}TenantId={1},ObjectId={2}{3}", (object) "{", (object) this.TenantId, (object) this.ObjectId, (object) "}");
  }
}
