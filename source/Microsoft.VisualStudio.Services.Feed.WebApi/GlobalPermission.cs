// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.WebApi.GlobalPermission
// Assembly: Microsoft.VisualStudio.Services.Feed.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8DACB936-5231-4131-8ED8-082A1F46DC54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Feed.WebApi.dll

using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Feed.WebApi
{
  [DataContract]
  public class GlobalPermission : IEquatable<GlobalPermission>
  {
    [DataMember]
    public GlobalRole Role { get; set; }

    [DataMember]
    public IdentityDescriptor IdentityDescriptor { get; set; }

    [DataMember(EmitDefaultValue = false, IsRequired = false)]
    public Guid? IdentityId { get; set; }

    public static bool operator ==(GlobalPermission left, GlobalPermission right) => object.Equals((object) left, (object) right);

    public static bool operator !=(GlobalPermission left, GlobalPermission right) => !object.Equals((object) left, (object) right);

    public bool Equals(GlobalPermission other)
    {
      if ((object) other == null)
        return false;
      if ((object) this == (object) other)
        return true;
      return this.Role == other.Role && object.Equals((object) this.IdentityDescriptor, (object) other.IdentityDescriptor);
    }

    public override bool Equals(object obj)
    {
      if (obj == null)
        return false;
      if ((object) this == obj)
        return true;
      return !(obj.GetType() != this.GetType()) && this.Equals((GlobalPermission) obj);
    }

    public override int GetHashCode() => (int) this.Role * 397 ^ (this.IdentityDescriptor != (IdentityDescriptor) null ? this.IdentityDescriptor.GetHashCode() : 0);
  }
}
