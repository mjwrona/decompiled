// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Maven.Server.MavenStorageIdCacheAggregation
// Assembly: Microsoft.VisualStudio.Services.Maven.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AEBE02E-FDD2-41D8-89F7-5C54445DBFA7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Maven.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Download;

namespace Microsoft.VisualStudio.Services.Maven.Server
{
  public class MavenStorageIdCacheAggregation : 
    IAggregation<MavenStorageIdCacheAggregation, IMavenStorageIdCacheAggregationAccessor>,
    IAggregation,
    IAggregationAccessorBootstrapper
  {
    public static readonly MavenStorageIdCacheAggregation MavenBlobMetadataStorageIdCache = new MavenStorageIdCacheAggregation(nameof (MavenBlobMetadataStorageIdCache));

    public MavenStorageIdCacheAggregation(string name) => this.VersionName = name;

    public AggregationDefinition Definition { get; } = AggregationDefinitions.MavenStorageIdCacheAggregationDefinition;

    public string VersionName { get; }

    public IAggregationAccessor Bootstrap(IVssRequestContext requestContext) => (IAggregationAccessor) new MavenStorageIdCacheAggregationAccessor(this, (IMetadataCacheService) new PackageMetadataCacheServiceFacade<IMavenPackageMetadataCacheService>(requestContext));
  }
}
