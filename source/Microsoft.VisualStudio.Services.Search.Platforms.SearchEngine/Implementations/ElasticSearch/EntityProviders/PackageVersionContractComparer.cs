// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Implementations.ElasticSearch.EntityProviders.PackageVersionContractComparer
// Assembly: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EE1DF96-C85D-457F-AAA1-93619829BFD4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.dll

using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.Package;
using Nest;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Implementations.ElasticSearch.EntityProviders
{
  public class PackageVersionContractComparer : IComparer<IHit<PackageVersionContract>>
  {
    public int Compare(IHit<PackageVersionContract> x, IHit<PackageVersionContract> y) => string.Compare(y.Source.SortableVersion, x.Source.SortableVersion, StringComparison.Ordinal);
  }
}
