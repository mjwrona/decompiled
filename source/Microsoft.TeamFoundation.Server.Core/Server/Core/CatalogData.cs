// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.CatalogData
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Server.Core
{
  [ClassVisibility(ClientVisibility.Internal)]
  public class CatalogData
  {
    private Dictionary<Guid, object> m_addedResourceTypes = new Dictionary<Guid, object>();
    private List<CatalogNode> m_deletedNodes = new List<CatalogNode>();
    private List<CatalogNode> m_catalogNodes = new List<CatalogNode>();
    private List<CatalogResource> m_catalogResources = new List<CatalogResource>();
    private List<CatalogResource> m_deletedResources = new List<CatalogResource>();
    private List<CatalogResource> m_deletedNodeResources = new List<CatalogResource>();
    private List<CatalogResourceType> m_catalogResourceTypes = new List<CatalogResourceType>();

    public CatalogData()
    {
    }

    public CatalogData(IEnumerable<CatalogResource> resources, long locationServiceLastChangeId)
    {
      this.LocationServiceLastChangeId = (int) locationServiceLastChangeId;
      this.ParseResources(resources, this.CatalogResources, this.CatalogNodes);
    }

    public CatalogData(
      IEnumerable<CatalogResource> resources,
      IEnumerable<CatalogResource> deletedResources,
      IEnumerable<CatalogNode> deletedNodes,
      long locationServiceLastChangeId)
    {
      this.LocationServiceLastChangeId = (int) locationServiceLastChangeId;
      this.ParseResources(resources, this.CatalogResources, this.CatalogNodes);
      this.ParseResources(deletedResources, this.DeletedResources, (List<CatalogNode>) null);
      this.ParseNodes(deletedNodes, this.DeletedNodes, this.DeletedNodeResources);
    }

    public CatalogData(IEnumerable<CatalogNode> nodes, long locationServiceLastChangeId)
    {
      this.LocationServiceLastChangeId = (int) locationServiceLastChangeId;
      this.ParseNodes(nodes, this.CatalogNodes, this.CatalogResources);
    }

    private void ParseNodes(
      IEnumerable<CatalogNode> nodes,
      List<CatalogNode> nodeDestination,
      List<CatalogResource> resourceDestination)
    {
      Dictionary<Guid, CatalogResource> addedResources = new Dictionary<Guid, CatalogResource>();
      Dictionary<string, CatalogNode> addedNodes = new Dictionary<string, CatalogNode>((IEqualityComparer<string>) VssStringComparer.CatalogNodePath);
      foreach (CatalogNode node in nodes)
        this.PrepareNodeForSerialization(nodeDestination, resourceDestination, addedResources, addedNodes, node, true);
    }

    private void ParseResources(
      IEnumerable<CatalogResource> resources,
      List<CatalogResource> resourceDestination,
      List<CatalogNode> nodeDestination)
    {
      Dictionary<Guid, CatalogResource> addedResources = new Dictionary<Guid, CatalogResource>();
      Dictionary<string, CatalogNode> addedNodes = new Dictionary<string, CatalogNode>((IEqualityComparer<string>) VssStringComparer.CatalogNodePath);
      foreach (CatalogResource resource in resources)
        this.PrepareResourceForSerialization(resourceDestination, nodeDestination, addedResources, addedNodes, resource, true);
    }

    private void PrepareNodeForSerialization(
      List<CatalogNode> nodeDestination,
      List<CatalogResource> resourceDestination,
      Dictionary<Guid, CatalogResource> addedResources,
      Dictionary<string, CatalogNode> addedNodes,
      CatalogNode node,
      bool matchedQuery)
    {
      if (!addedNodes.ContainsKey(node.FullPath))
      {
        node.PrepareForWebServiceSerialization(matchedQuery);
        nodeDestination.Add(node);
        addedNodes[node.FullPath] = node;
        this.PrepareResourceForSerialization(resourceDestination, nodeDestination, addedResources, addedNodes, node.Resource, matchedQuery);
        if (node.Dependencies != null)
        {
          foreach (CatalogNode node1 in (IEnumerable<CatalogNode>) node.Dependencies.Singletons.Values)
            this.PrepareNodeForSerialization(nodeDestination, resourceDestination, addedResources, addedNodes, node1, false);
          foreach (KeyValuePair<string, IList<CatalogNode>> set in (IEnumerable<KeyValuePair<string, IList<CatalogNode>>>) node.Dependencies.Sets)
          {
            foreach (CatalogNode node2 in (IEnumerable<CatalogNode>) set.Value)
              this.PrepareNodeForSerialization(nodeDestination, resourceDestination, addedResources, addedNodes, node2, false);
          }
        }
        if (node.ParentNode == null)
          return;
        this.PrepareNodeForSerialization(nodeDestination, resourceDestination, addedResources, addedNodes, node.ParentNode, false);
      }
      else
      {
        if (!matchedQuery)
          return;
        addedNodes[node.FullPath].MatchedQuery = matchedQuery;
      }
    }

    private void PrepareResourceForSerialization(
      List<CatalogResource> resourceDestination,
      List<CatalogNode> nodeDestination,
      Dictionary<Guid, CatalogResource> addedResources,
      Dictionary<string, CatalogNode> addedNodes,
      CatalogResource resource,
      bool matchedQuery)
    {
      if (!addedResources.ContainsKey(resource.Identifier))
      {
        resource.PrepareForWebServiceSerialization(matchedQuery);
        resourceDestination.Add(resource);
        addedResources[resource.Identifier] = resource;
        if (!this.m_addedResourceTypes.ContainsKey(resource.ResourceType.Identifier))
        {
          this.CatalogResourceTypes.Add(resource.ResourceType);
          this.m_addedResourceTypes[resource.ResourceType.Identifier] = (object) null;
        }
        foreach (CatalogNode nodeReference in resource.NodeReferences)
          this.PrepareNodeForSerialization(nodeDestination, resourceDestination, addedResources, addedNodes, nodeReference, false);
      }
      else
      {
        if (!matchedQuery)
          return;
        addedResources[resource.Identifier].MatchedQuery = true;
      }
    }

    public List<CatalogResourceType> CatalogResourceTypes => this.m_catalogResourceTypes;

    public List<CatalogResource> CatalogResources => this.m_catalogResources;

    public List<CatalogNode> CatalogNodes => this.m_catalogNodes;

    public List<CatalogResource> DeletedResources => this.m_deletedResources;

    public List<CatalogResource> DeletedNodeResources => this.m_deletedNodeResources;

    public List<CatalogNode> DeletedNodes => this.m_deletedNodes;

    public int LocationServiceLastChangeId { get; set; }
  }
}
