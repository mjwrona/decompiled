// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.PublicRepositories.PublicRepoPackageMemoryCacheFacade`1
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.BlobStore.Server.Common;
using Microsoft.VisualStudio.Services.Feed.Common;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;


#nullable enable
namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.PublicRepositories
{
  public class PublicRepoPackageMemoryCacheFacade<TDocument> : 
    IPublicRepoPackageMemoryCache<TDocument>
    where TDocument : class, IVersionedDocument
  {
    public PublicRepoPackageMemoryCacheFacade(
      IVssRequestContext requestContext,
      IPublicRepoPackageMemoryCacheService<TDocument> cacheService)
    {
      // ISSUE: reference to a compiler-generated field
      this.\u003CrequestContext\u003EP = requestContext;
      // ISSUE: reference to a compiler-generated field
      this.\u003CcacheService\u003EP = cacheService;
      // ISSUE: explicit constructor call
      base.\u002Ector();
    }

    public bool TryGet(
      string cacheUniverse,
      WellKnownUpstreamSource source,
      IPackageName packageName,
      out EtagValue<TDocument> document)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      return this.\u003CcacheService\u003EP.TryGet(this.\u003CrequestContext\u003EP, cacheUniverse, source, packageName, out document);
    }

    public bool TryAddOrReplace(
      string cacheUniverse,
      WellKnownUpstreamSource source,
      IPackageName packageName,
      EtagValue<TDocument> document,
      out EtagValue<TDocument> finalDocument)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      return this.\u003CcacheService\u003EP.TryAddOrReplace(this.\u003CrequestContext\u003EP, cacheUniverse, source, packageName, document, out finalDocument);
    }

    public void Invalidate(
      string cacheUniverse,
      WellKnownUpstreamSource source,
      IPackageName packageName)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      this.\u003CcacheService\u003EP.Invalidate(this.\u003CrequestContext\u003EP, cacheUniverse, source, packageName);
    }

    public void Invalidate(
      string cacheUniverse,
      WellKnownUpstreamSource source,
      IPackageName packageName,
      long documentVersion)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      this.\u003CcacheService\u003EP.Invalidate(this.\u003CrequestContext\u003EP, cacheUniverse, source, packageName, documentVersion);
    }
  }
}
