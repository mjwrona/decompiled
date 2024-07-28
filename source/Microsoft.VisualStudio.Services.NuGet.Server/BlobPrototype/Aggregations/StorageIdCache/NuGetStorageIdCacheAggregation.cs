// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype.Aggregations.StorageIdCache.NuGetStorageIdCacheAggregation
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.NuGet.Server.PackageDownload;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Download;

namespace Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype.Aggregations.StorageIdCache
{
  public class NuGetStorageIdCacheAggregation : 
    IAggregation<NuGetStorageIdCacheAggregation, INuGetStorageIdCacheAggregationAccessor>,
    IAggregation,
    IAggregationAccessorBootstrapper
  {
    public static readonly NuGetStorageIdCacheAggregation NuGetBlobMetadataStorageIdCache = new NuGetStorageIdCacheAggregation(nameof (NuGetBlobMetadataStorageIdCache));

    public NuGetStorageIdCacheAggregation(string name) => this.VersionName = name;

    public AggregationDefinition Definition { get; } = NuGetAggregationDefinitions.NuGetStorageIdCacheAggregationDefinition;

    public string VersionName { get; }

    public IAggregationAccessor Bootstrap(IVssRequestContext requestContext) => (IAggregationAccessor) new NuGetStorageIdCacheAggregationAccessor(this, (IMetadataCacheService) new PackageMetadataCacheServiceFacade<INuGetPackageMetadataCacheService>(requestContext));
  }
}
