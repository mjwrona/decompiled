// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Npm.Server.Aggregations.StorageIdCache.NpmStorageIdCacheAggregation
// Assembly: Microsoft.VisualStudio.Services.Npm.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2F4F0262-1C1B-42F0-BCA7-1385424A0D51
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Npm.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Npm.Server.Migration;
using Microsoft.VisualStudio.Services.Npm.Server.PackageDownload;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Download;

namespace Microsoft.VisualStudio.Services.Npm.Server.Aggregations.StorageIdCache
{
  public class NpmStorageIdCacheAggregation : 
    IAggregation<NpmStorageIdCacheAggregation, INpmStorageIdCacheAggregationAccessor>,
    IAggregation,
    IAggregationAccessorBootstrapper
  {
    public static readonly NpmStorageIdCacheAggregation NpmBlobMetadataStorageIdCache = new NpmStorageIdCacheAggregation(nameof (NpmBlobMetadataStorageIdCache));

    public NpmStorageIdCacheAggregation(string name) => this.VersionName = name;

    public AggregationDefinition Definition { get; } = NpmAggregationDefinitions.NpmStorageIdCacheAggregationDefinition;

    public string VersionName { get; }

    public IAggregationAccessor Bootstrap(IVssRequestContext requestContext) => (IAggregationAccessor) new NpmStorageIdCacheAggregationAccessor(this, (IMetadataCacheService) new PackageMetadataCacheServiceFacade<INpmPackageMetadataCacheService>(requestContext));
  }
}
