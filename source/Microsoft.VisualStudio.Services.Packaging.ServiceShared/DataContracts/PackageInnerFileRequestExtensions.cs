// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts.PackageInnerFileRequestExtensions
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts
{
  public static class PackageInnerFileRequestExtensions
  {
    public static IPackageInnerFileRequest<TPackageId, TData> WithData<TPackageId, TData>(
      this IPackageInnerFileRequest<TPackageId> packageRequest,
      TData data)
      where TPackageId : IPackageIdentity
      where TData : class
    {
      return (IPackageInnerFileRequest<TPackageId, TData>) new PackageInnerFileRequest<TPackageId, TData>(packageRequest, data);
    }
  }
}
