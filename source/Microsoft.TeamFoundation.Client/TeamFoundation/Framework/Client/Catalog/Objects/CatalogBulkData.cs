// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Client.Catalog.Objects.CatalogBulkData
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Microsoft.TeamFoundation.Framework.Client.Catalog.Objects
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class CatalogBulkData
  {
    private ICollection<CatalogNode> m_nodes;
    private ICollection<Guid> m_queriedResourceTypes;
    private IDictionary<string, List<CatalogNode>> m_nodeTree;

    public CatalogBulkData(ICollection<CatalogNode> nodes)
      : this(nodes, (ICollection<Guid>) null)
    {
    }

    public CatalogBulkData(ICollection<CatalogNode> nodes, ICollection<Guid> queriedResourceTypes)
    {
      this.ExpandRelationships(nodes);
      this.m_queriedResourceTypes = (ICollection<Guid>) ((object) queriedResourceTypes ?? (object) Array.Empty<Guid>());
    }

    public bool QueriedForResourceType(Guid guid) => this.QueriedAllResourceTypes || this.m_queriedResourceTypes.Contains(guid);

    public bool QueriedAllResourceTypes => !this.m_queriedResourceTypes.Any<Guid>();

    public ICollection<Guid> QueriedResourceTypes => this.m_queriedResourceTypes;

    public ICollection<CatalogNode> Nodes => this.m_nodes;

    public IEnumerable<CatalogNode> FindChildren(string path)
    {
      List<CatalogNode> catalogNodeList;
      return this.m_nodeTree.TryGetValue(path, out catalogNodeList) ? (IEnumerable<CatalogNode>) catalogNodeList : Enumerable.Empty<CatalogNode>();
    }

    private void ExpandRelationships(ICollection<CatalogNode> nodes)
    {
      Queue<CatalogNode> source = new Queue<CatalogNode>((IEnumerable<CatalogNode>) nodes);
      HashSet<CatalogNode> catalogNodeSet = new HashSet<CatalogNode>();
      this.m_nodeTree = (IDictionary<string, List<CatalogNode>>) new Dictionary<string, List<CatalogNode>>();
      while (source.Any<CatalogNode>())
      {
        CatalogNode catalogNode1 = source.Dequeue();
        catalogNodeSet.Add(catalogNode1);
        string key = catalogNode1.ParentPath ?? string.Empty;
        List<CatalogNode> catalogNodeList;
        if (!this.m_nodeTree.TryGetValue(key, out catalogNodeList))
        {
          catalogNodeList = new List<CatalogNode>();
          this.m_nodeTree[key] = catalogNodeList;
        }
        catalogNodeList.Add(catalogNode1);
        if (catalogNode1.ParentNode != null && !catalogNodeSet.Contains(catalogNode1.ParentNode))
          source.Enqueue(catalogNode1.ParentNode);
        if (catalogNode1.Dependencies != null && catalogNode1.Dependencies.Singletons != null)
        {
          foreach (KeyValuePair<string, CatalogNode> singleton in catalogNode1.Dependencies.Singletons)
          {
            if (!catalogNodeSet.Contains(singleton.Value))
              source.Enqueue(singleton.Value);
          }
        }
        if (catalogNode1.Dependencies != null && catalogNode1.Dependencies.Sets != null)
        {
          foreach (KeyValuePair<string, IList<CatalogNode>> set in catalogNode1.Dependencies.Sets)
          {
            foreach (CatalogNode catalogNode2 in (IEnumerable<CatalogNode>) set.Value)
            {
              if (!catalogNodeSet.Contains(catalogNode2))
                source.Enqueue(catalogNode2);
            }
          }
        }
      }
      this.m_nodes = (ICollection<CatalogNode>) catalogNodeSet;
    }
  }
}
