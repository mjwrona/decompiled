// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.CatalogDependencyGroup
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.VisualStudio.Services.Common;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Server.Core
{
  public class CatalogDependencyGroup
  {
    public CatalogDependencyGroup()
    {
      this.Singletons = (IDictionary<string, CatalogNode>) new Dictionary<string, CatalogNode>((IEqualityComparer<string>) VssStringComparer.CatalogNodeDependency);
      this.Sets = (IDictionary<string, IList<CatalogNode>>) new Dictionary<string, IList<CatalogNode>>((IEqualityComparer<string>) VssStringComparer.CatalogNodeDependency);
    }

    public IDictionary<string, CatalogNode> Singletons { get; private set; }

    public IDictionary<string, IList<CatalogNode>> Sets { get; private set; }

    public IList<CatalogNode> GetDependencySet(string key)
    {
      IList<CatalogNode> dependencySet;
      if (!this.Sets.TryGetValue(key, out dependencySet))
        this.Sets[key] = dependencySet = (IList<CatalogNode>) new List<CatalogNode>();
      return dependencySet;
    }
  }
}
