// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts.PackageRequest`2
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts
{
  public class PackageRequest<TPackageId, T> : 
    PackageRequest<TPackageId>,
    IPackageRequest<TPackageId, T>,
    IPackageRequest<TPackageId>,
    IPackageRequest,
    IFeedRequest,
    IProtocolAgnosticFeedRequest,
    IFeedRequest<T>
    where TPackageId : IPackageIdentity
  {
    public PackageRequest(IPackageRequest<TPackageId> originalRequest, T data)
      : base((IFeedRequest) originalRequest, originalRequest.PackageId)
    {
      this.AdditionalData = data;
    }

    public T AdditionalData { get; }
  }
}
