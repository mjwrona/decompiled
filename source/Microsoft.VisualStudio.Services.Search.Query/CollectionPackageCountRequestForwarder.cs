// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Query.CollectionPackageCountRequestForwarder
// Assembly: Microsoft.VisualStudio.Services.Search.Query, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 71E00698-03D3-4C67-B313-A550333DA80C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Query.dll

using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.Package;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Implementations.ElasticSearch.EntityProviders;

namespace Microsoft.VisualStudio.Services.Search.Query
{
  public class CollectionPackageCountRequestForwarder : PackageCountRequestForwarder
  {
    public CollectionPackageCountRequestForwarder(
      string searchPlatformConnectionString,
      string searchPlatformSettings,
      bool isOnPrem)
      : base(searchPlatformConnectionString, searchPlatformSettings, isOnPrem)
    {
    }

    public CollectionPackageCountRequestForwarder(ISearchPlatform searchPlatform)
      : base(searchPlatform)
    {
    }

    internal override EntityIndexProvider<PackageVersionContract> GetIndexProvider() => (EntityIndexProvider<PackageVersionContract>) new CollectionPackageEntityCountIndexProvider<PackageVersionContract>();
  }
}
