// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts.PackageFileRequestExtensions
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts
{
  public static class PackageFileRequestExtensions
  {
    public static IPackageFileRequest<TPackageId, TData> WithData<TPackageId, TData>(
      this IPackageFileRequest<TPackageId> packageRequest,
      TData data)
      where TPackageId : IPackageIdentity
      where TData : class
    {
      return (IPackageFileRequest<TPackageId, TData>) new PackageFileRequest<TPackageId, TData>(packageRequest, data);
    }

    public static IPackageInnerFileRequest<TPackageId> WithInnerFile<TPackageId>(
      this IPackageFileRequest<TPackageId> request,
      string innerFilePath)
      where TPackageId : IPackageIdentity
    {
      return (IPackageInnerFileRequest<TPackageId>) new PackageInnerFileRequest<TPackageId>((IFeedRequest) request, request.PackageId, request.FilePath, innerFilePath);
    }
  }
}
