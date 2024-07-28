// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.Catalog.Objects.CatalogBulkData
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.Core.Catalog.Objects
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class CatalogBulkData
  {
    private ICollection<CatalogNode> m_nodes;
    private ICollection<Guid> m_queriedResourceTypes;

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

    private void ExpandRelationships(ICollection<CatalogNode> nodes)
    {
      Queue<CatalogNode> source = new Queue<CatalogNode>((IEnumerable<CatalogNode>) nodes);
      HashSet<CatalogNode> catalogNodeSet = new HashSet<CatalogNode>();
      while (source.Any<CatalogNode>())
      {
        CatalogNode catalogNode1 = source.Dequeue();
        catalogNodeSet.Add(catalogNode1);
        if (catalogNode1.ParentNode != null && !catalogNodeSet.Contains(catalogNode1.ParentNode))
          source.Enqueue(catalogNode1.ParentNode);
        if (catalogNode1.Dependencies != null && catalogNode1.Dependencies.Singletons != null)
        {
          foreach (KeyValuePair<string, CatalogNode> singleton in (IEnumerable<KeyValuePair<string, CatalogNode>>) catalogNode1.Dependencies.Singletons)
          {
            if (!catalogNodeSet.Contains(singleton.Value))
              source.Enqueue(singleton.Value);
          }
        }
        if (catalogNode1.Dependencies != null && catalogNode1.Dependencies.Sets != null)
        {
          foreach (KeyValuePair<string, IList<CatalogNode>> set in (IEnumerable<KeyValuePair<string, IList<CatalogNode>>>) catalogNode1.Dependencies.Sets)
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
