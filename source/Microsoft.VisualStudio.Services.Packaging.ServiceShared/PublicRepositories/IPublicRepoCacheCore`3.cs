// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.PublicRepositories.IPublicRepoCacheCore`3
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.Shared.Internal.WebApi.Types;
using System;
using System.Threading.Tasks;


#nullable enable
namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.PublicRepositories
{
  public interface IPublicRepoCacheCore<in TPackageName, TDoc, in TCursor>
    where TPackageName : class, IPackageName
    where TDoc : class, IVersionedDocument, IHaveGenerationCursor<TCursor>, IHaveIsDefaultInitialized
    where TCursor : class, IComparable<TCursor>
  {
    Task InvalidatePackageVersionDataAsync(
      TPackageName packageName,
      TCursor? minValidCursor,
      bool allowRefresh);

    Task<TDoc> GetPackageMetadataAsync(TPackageName packageName);

    Task<TDoc?> GetMetadataForDiagnosticsAsync(
      PublicRepositoryCacheType? cacheType,
      TPackageName typedName);

    Task<TDoc> UpdatePackageMetadataAsync(TPackageName packageName, TCursor? minValidCursor);
  }
}
