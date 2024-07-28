// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.WebApi.Internal.Contracts.PackageDependencyDetails
// Assembly: Microsoft.VisualStudio.Services.Feed.WebApi.Internal, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4BC34C1F-0F07-4DDD-8B37-907579B359F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Feed.WebApi.Internal.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Feed.WebApi.Internal.Contracts
{
  [DataContract]
  public class PackageDependencyDetails : FeedSecuredObject
  {
    [DataMember(EmitDefaultValue = false)]
    public string PackageName { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string Group { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string VersionRange { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public Guid PackageId { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public Guid PackageVersionId { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string NormalizedPackageName { get; set; }

    public bool Equals(PackageDependencyDetails other)
    {
      if (other == null)
        return false;
      if (this == other)
        return true;
      return object.Equals((object) this.PackageId, (object) other.PackageId) && object.Equals((object) this.PackageVersionId, (object) other.PackageVersionId) && string.Equals(this.PackageName, other.PackageName);
    }

    public override bool Equals(object obj)
    {
      if (obj == null)
        return false;
      if (this == obj)
        return true;
      return obj is PackageDependencyDetails other && this.Equals(other);
    }

    public override int GetHashCode()
    {
      Guid packageId = this.PackageId;
      Guid guid = this.PackageId;
      int num = guid.GetHashCode() * 397;
      Guid packageVersionId = this.PackageVersionId;
      guid = this.PackageVersionId;
      int hashCode = guid.GetHashCode();
      return (num ^ hashCode) * 397 ^ (this.PackageName != null ? this.PackageName.GetHashCode() : 0);
    }
  }
}
