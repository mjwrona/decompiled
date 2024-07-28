// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts.PackageNameRequest`2
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts
{
  public class PackageNameRequest<TPackageName, TData> : 
    PackageNameRequest<TPackageName>,
    IPackageNameRequest<TPackageName, TData>,
    IPackageNameRequest<TPackageName>,
    IPackageNameRequest,
    IFeedRequest,
    IProtocolAgnosticFeedRequest
    where TPackageName : IPackageName
  {
    public PackageNameRequest(IPackageNameRequest<TPackageName> packageRequest, TData data)
      : base((IFeedRequest) packageRequest, packageRequest.PackageName)
    {
      this.AdditionalData = data;
    }

    public TData AdditionalData { get; set; }

    protected bool Equals(PackageNameRequest<TPackageName, TData> other) => this.Equals((PackageNameRequest<TPackageName>) other) && EqualityComparer<TData>.Default.Equals(this.AdditionalData, other.AdditionalData);

    public override bool Equals(object obj)
    {
      if (obj == null)
        return false;
      if (this == obj)
        return true;
      return !(obj.GetType() != this.GetType()) && this.Equals((PackageNameRequest<TPackageName, TData>) obj);
    }

    public override int GetHashCode() => base.GetHashCode() * 397 ^ EqualityComparer<TData>.Default.GetHashCode(this.AdditionalData);
  }
}
