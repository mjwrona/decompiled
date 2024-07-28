// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.Cache.IdentityId
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Identity.Cache
{
  [DataContract]
  internal class IdentityId : ICloneable
  {
    internal IdentityId()
    {
    }

    internal IdentityId(Guid id, IdentityType type, IdentityDescriptor descriptor)
    {
      this.Id = id;
      this.Type = type;
      this.Descriptor = descriptor;
    }

    [DataMember]
    internal Guid Id { get; private set; }

    [DataMember]
    internal IdentityType Type { get; private set; }

    [DataMember]
    internal IdentityDescriptor Descriptor { get; private set; }

    public bool Equals(IdentityId other)
    {
      if (this == other)
        return true;
      return other != null && object.Equals((object) this.Id, (object) other.Id) && object.Equals((object) this.Type, (object) other.Type) && IdentityDescriptorComparer.Instance.Equals(this.Descriptor, other.Descriptor);
    }

    public override bool Equals(object obj)
    {
      if (this == obj)
        return true;
      return obj != null && this.GetType() == obj.GetType() && this.Equals((IdentityId) obj);
    }

    public override int GetHashCode() => this.Id.GetHashCode() + 23 * this.Type.GetHashCode();

    public object Clone() => (object) new IdentityId(this.Id, this.Type, this.Descriptor == (IdentityDescriptor) null ? (IdentityDescriptor) null : new IdentityDescriptor(this.Descriptor.IdentityType, this.Descriptor.Identifier));
  }
}
