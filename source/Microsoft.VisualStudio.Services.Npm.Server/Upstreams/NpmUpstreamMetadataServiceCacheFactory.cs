// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Npm.Server.Upstreams.NpmUpstreamMetadataServiceCacheFactory
// Assembly: Microsoft.VisualStudio.Services.Npm.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2F4F0262-1C1B-42F0-BCA7-1385424A0D51
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Npm.Server.dll

using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Npm.Server.CommitLog;
using Microsoft.VisualStudio.Services.Npm.Server.NpmPackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Versioning;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Npm.Server.Upstreams
{
  public class NpmUpstreamMetadataServiceCacheFactory : 
    IFactory<UpstreamSource, Task<IUpstreamMetadataService<NpmPackageName, SemanticVersion, NpmPackageIdentity, INpmMetadataEntry>>>
  {
    private readonly IFactory<UpstreamSource, Task<IUpstreamMetadataService<NpmPackageName, SemanticVersion, NpmPackageIdentity, INpmMetadataEntry>>> factory;
    private readonly Dictionary<UpstreamSource, IUpstreamMetadataService<NpmPackageName, SemanticVersion, NpmPackageIdentity, INpmMetadataEntry>> cache;

    public NpmUpstreamMetadataServiceCacheFactory(
      IFactory<UpstreamSource, Task<IUpstreamMetadataService<NpmPackageName, SemanticVersion, NpmPackageIdentity, INpmMetadataEntry>>> factory)
    {
      this.factory = factory;
      this.cache = new Dictionary<UpstreamSource, IUpstreamMetadataService<NpmPackageName, SemanticVersion, NpmPackageIdentity, INpmMetadataEntry>>();
    }

    public async Task<IUpstreamMetadataService<NpmPackageName, SemanticVersion, NpmPackageIdentity, INpmMetadataEntry>> Get(
      UpstreamSource input)
    {
      IUpstreamMetadataService<NpmPackageName, SemanticVersion, NpmPackageIdentity, INpmMetadataEntry> upstreamMetadataService1;
      if (this.cache.TryGetValue(input, out upstreamMetadataService1))
        return upstreamMetadataService1;
      IUpstreamMetadataService<NpmPackageName, SemanticVersion, NpmPackageIdentity, INpmMetadataEntry> upstreamMetadataService2 = await this.factory.Get(input);
      this.cache[input] = upstreamMetadataService2;
      return upstreamMetadataService2;
    }
  }
}
