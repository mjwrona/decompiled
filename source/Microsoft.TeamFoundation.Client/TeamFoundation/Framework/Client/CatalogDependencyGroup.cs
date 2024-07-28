// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Client.CatalogDependencyGroup
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.VisualStudio.Services.Common;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Framework.Client
{
  public class CatalogDependencyGroup
  {
    private IDictionary<string, CatalogNode> m_singletons;
    private IDictionary<string, IList<CatalogNode>> m_sets;

    public CatalogDependencyGroup()
    {
      this.m_singletons = (IDictionary<string, CatalogNode>) new Dictionary<string, CatalogNode>((IEqualityComparer<string>) VssStringComparer.CatalogNodeDependency);
      this.m_sets = (IDictionary<string, IList<CatalogNode>>) new Dictionary<string, IList<CatalogNode>>((IEqualityComparer<string>) VssStringComparer.CatalogNodeDependency);
    }

    public CatalogDependencyGroup(CatalogDependencyGroup dependencies)
    {
      this.m_singletons = (IDictionary<string, CatalogNode>) new Dictionary<string, CatalogNode>(dependencies.m_singletons, (IEqualityComparer<string>) VssStringComparer.CatalogNodeDependency);
      this.m_sets = (IDictionary<string, IList<CatalogNode>>) new Dictionary<string, IList<CatalogNode>>(dependencies.m_sets, (IEqualityComparer<string>) VssStringComparer.CatalogNodeDependency);
    }

    public IEnumerable<CatalogNode> GetAllDependencies()
    {
      List<CatalogNode> allDependencies = new List<CatalogNode>();
      allDependencies.AddRange((IEnumerable<CatalogNode>) this.m_singletons.Values);
      foreach (KeyValuePair<string, IList<CatalogNode>> set in (IEnumerable<KeyValuePair<string, IList<CatalogNode>>>) this.m_sets)
        allDependencies.AddRange((IEnumerable<CatalogNode>) set.Value);
      return (IEnumerable<CatalogNode>) allDependencies;
    }

    public void SetSingletonDependency(string key, CatalogNode node) => this.m_singletons[key] = node;

    public void RemoveSingletonDependency(string key) => this.m_singletons.Remove(key);

    public CatalogNode GetSingletonDependency(string key)
    {
      CatalogNode singletonDependency;
      this.m_singletons.TryGetValue(key, out singletonDependency);
      return singletonDependency;
    }

    public IEnumerable<KeyValuePair<string, CatalogNode>> Singletons => (IEnumerable<KeyValuePair<string, CatalogNode>>) this.m_singletons;

    public void AddSetDependency(string key, CatalogNode node)
    {
      IList<CatalogNode> catalogNodeList;
      if (!this.m_sets.TryGetValue(key, out catalogNodeList))
      {
        catalogNodeList = (IList<CatalogNode>) new List<CatalogNode>();
        this.m_sets[key] = catalogNodeList;
      }
      catalogNodeList.Add(node);
    }

    public void RemoveSetDependency(string key) => this.m_sets.Remove(key);

    public void RemoveSetDependency(string key, CatalogNode node)
    {
      CatalogNode catalogNode = node;
      IList<CatalogNode> catalogNodeList;
      if (!this.m_sets.TryGetValue(key, out catalogNodeList))
        return;
      for (int index = catalogNodeList.Count - 1; index >= 0; --index)
      {
        if (VssStringComparer.CatalogNodePath.Equals(catalogNode.ChildItem, catalogNodeList[index].ChildItem))
          catalogNodeList.RemoveAt(index);
      }
      if (catalogNodeList.Count != 0)
        return;
      this.m_sets.Remove(key);
    }

    public IEnumerable<KeyValuePair<string, IList<CatalogNode>>> Sets => (IEnumerable<KeyValuePair<string, IList<CatalogNode>>>) this.m_sets;

    public IEnumerable<CatalogNode> GetDependencySet(string key)
    {
      IList<CatalogNode> dependencySet;
      if (!this.m_sets.TryGetValue(key, out dependencySet))
        this.m_sets[key] = dependencySet = (IList<CatalogNode>) new List<CatalogNode>();
      return (IEnumerable<CatalogNode>) dependencySet;
    }

    public void ClearSingletonDependencies() => this.m_singletons.Clear();

    public void ClearDependencySets() => this.m_sets.Clear();
  }
}
