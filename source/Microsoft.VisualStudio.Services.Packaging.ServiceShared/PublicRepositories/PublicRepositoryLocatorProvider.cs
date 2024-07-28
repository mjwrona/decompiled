// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.PublicRepositories.PublicRepositoryLocatorProvider
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Content.Common;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.DocumentProvider;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;


#nullable enable
namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.PublicRepositories
{
  public class PublicRepositoryLocatorProvider : IAggregationDocumentLocatorProvider<IPackageName>
  {
    public PublicRepositoryLocatorProvider(ICacheUniverseProvider cacheUniverseProvider)
    {
      // ISSUE: reference to a compiler-generated field
      this.\u003CcacheUniverseProvider\u003EP = cacheUniverseProvider;
      // ISSUE: explicit constructor call
      base.\u002Ector();
    }

    public Locator GetLocator(IFeedRequest feedRequest, IPackageName specifier) => new Locator(new string[3]
    {
      this.\u003CcacheUniverseProvider\u003EP.GetCacheUniverseName(),
      "packages",
      specifier.NormalizedName + ".binpb"
    });
  }
}
