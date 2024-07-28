// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts.PackageFileRequest`2
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using System;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts
{
  public class PackageFileRequest<TPackageId, T> : 
    PackageFileRequest<TPackageId>,
    IPackageFileRequest<TPackageId, T>,
    IPackageFileRequest<TPackageId>,
    IPackageRequest<TPackageId>,
    IPackageRequest,
    IFeedRequest,
    IProtocolAgnosticFeedRequest,
    IPackageFileRequest,
    IPackageRequest<TPackageId, T>,
    IFeedRequest<T>
    where TPackageId : IPackageIdentity
    where T : class
  {
    public PackageFileRequest(IPackageFileRequest<TPackageId> originalRequest, T data)
      : base((IFeedRequest) originalRequest, originalRequest.PackageId, originalRequest.FilePath)
    {
      this.AdditionalData = data ?? throw new ArgumentNullException(nameof (data));
    }

    public PackageFileRequest(IPackageRequest<TPackageId> originalRequest, string fileName, T data)
      : base((IFeedRequest) originalRequest, originalRequest.PackageId, fileName)
    {
      this.AdditionalData = data ?? throw new ArgumentNullException(nameof (data));
    }

    public T AdditionalData { get; }
  }
}
