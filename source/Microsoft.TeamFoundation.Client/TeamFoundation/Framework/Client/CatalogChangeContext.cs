// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Client.CatalogChangeContext
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.TeamFoundation.Common.Internal;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Framework.Client
{
  public class CatalogChangeContext
  {
    private Dictionary<Guid, CatalogResource> m_resources = new Dictionary<Guid, CatalogResource>();
    private Dictionary<string, CatalogNode> m_nodes = new Dictionary<string, CatalogNode>((IEqualityComparer<string>) VssStringComparer.CatalogNodePath);
    private List<KeyValueOfStringString> m_moves = new List<KeyValueOfStringString>();
    private CatalogService m_catalogService;
    private CatalogWebService m_catalogProxy;

    internal CatalogChangeContext(CatalogService catalogService, CatalogWebService catalogProxy)
    {
      this.m_catalogService = catalogService;
      this.m_catalogProxy = catalogProxy;
    }

    public void AttachResource(CatalogResource resource)
    {
      ArgumentUtility.CheckForNull<CatalogResource>(resource, nameof (resource));
      resource.ChangeType = CatalogChangeType.CreateOrUpdate;
      this.m_resources[resource.TempCorrelationId] = resource;
    }

    public void AttachNode(CatalogNode node)
    {
      ArgumentUtility.CheckForNull<CatalogNode>(node, nameof (node));
      ArgumentUtility.CheckForNull<CatalogResource>(node.Resource, "CatalogNode.Resource");
      node.ChangeType = CatalogChangeType.CreateOrUpdate;
      this.m_nodes[node.ChildItem] = node;
    }

    public void AttachDelete(CatalogNode node, bool recurse)
    {
      ArgumentUtility.CheckForNull<CatalogNode>(node, nameof (node));
      if (string.IsNullOrEmpty(node.FullPath))
        throw new CatalogNodeDoesNotExistException(TFCommonResources.CatalogNodeDoesNotExist());
      node.ChangeType = recurse ? CatalogChangeType.RecursiveDelete : CatalogChangeType.NonRecursiveDelete;
      this.m_nodes[node.ChildItem] = node;
    }

    public void AttachMove(CatalogNode nodeToMove, CatalogNode newParent)
    {
      ArgumentUtility.CheckForNull<CatalogNode>(nodeToMove, nameof (nodeToMove));
      ArgumentUtility.CheckForNull<CatalogNode>(newParent, nameof (newParent));
      this.AttachNode(nodeToMove);
      this.AttachNode(newParent);
      this.m_moves.Add(new KeyValueOfStringString(new KeyValuePair<string, string>(nodeToMove.ParentPath + nodeToMove.ChildItem, newParent.ParentPath + newParent.ChildItem)));
    }

    public CatalogChangeResult Save() => this.SaveInternal(false);

    public CatalogChangeResult Preview() => this.SaveInternal(true);

    private CatalogChangeResult SaveInternal(bool preview)
    {
      if (this.m_moves.Count == 0 && this.m_nodes.Count == 0 && this.m_resources.Count == 0)
        return new CatalogChangeResult();
      List<CatalogNode> catalogNodeList = new List<CatalogNode>((IEnumerable<CatalogNode>) this.m_nodes.Values);
      CatalogQueryOptions queryOptions = CatalogQueryOptions.None;
      for (int index = 0; index < catalogNodeList.Count; ++index)
      {
        CatalogNode node = catalogNodeList[index];
        if (node.ChangeType == CatalogChangeType.CreateOrUpdate || !this.m_resources.ContainsKey(node.Resource.TempCorrelationId))
        {
          node.Resource.ChangeType = node.ChangeType == CatalogChangeType.CreateOrUpdate ? CatalogChangeType.CreateOrUpdate : CatalogChangeType.None;
          this.m_resources[node.Resource.TempCorrelationId] = node.Resource;
        }
        if (node.Dependencies != null)
        {
          queryOptions |= CatalogQueryOptions.ExpandDependencies;
          foreach (CatalogNode allDependency in node.Dependencies.GetAllDependencies())
          {
            if (!this.m_nodes.ContainsKey(allDependency.ChildItem))
            {
              allDependency.ChangeType = CatalogChangeType.None;
              catalogNodeList.Add(allDependency);
              this.m_nodes[allDependency.ChildItem] = allDependency;
            }
          }
        }
        if (node.ParentNode != null)
          queryOptions |= CatalogQueryOptions.IncludeParents;
        catalogNodeList[index] = CatalogNode.PrepareForWebServiceSerialization(node);
      }
      List<CatalogResource> catalogResourceList = new List<CatalogResource>();
      foreach (CatalogResource resource in this.m_resources.Values)
      {
        if (resource.Identifier == Guid.Empty)
        {
          bool flag = false;
          foreach (CatalogNode catalogNode in this.m_nodes.Values)
          {
            if (catalogNode.Resource.TempCorrelationId == resource.TempCorrelationId)
            {
              flag = true;
              break;
            }
          }
          if (!flag)
            throw new InvalidCatalogSaveResourceException(TFCommonResources.CannotCreateOrphanedResource());
        }
        catalogResourceList.Add(CatalogResource.PrepareForWebServiceSerialization(resource));
      }
      CatalogData catalogData = this.m_catalogProxy.SaveCatalogChanges(catalogResourceList.ToArray(), catalogNodeList.ToArray(), this.m_moves.ToArray(), (int) queryOptions, preview);
      CatalogResourceType[] catalogResourceTypes = catalogData.CatalogResourceTypes;
      if (!preview)
      {
        List<CatalogResource> matchingResources;
        this.m_catalogService.BuildCatalogObjects(catalogData.LocationServiceLastChangeId, catalogData.CatalogResources, catalogData.CatalogNodes, catalogResourceTypes, queryOptions, out matchingResources, out List<CatalogNode> _);
        foreach (CatalogResource updatedResource in matchingResources)
        {
          CatalogResource catalogResource;
          if (this.m_resources.TryGetValue(updatedResource.Identifier, out catalogResource))
            catalogResource.UpdateSelf(updatedResource);
          foreach (CatalogNode nodeReference in updatedResource.NodeReferences)
          {
            CatalogNode catalogNode;
            if (this.m_nodes.TryGetValue(nodeReference.FullPath.Substring(nodeReference.FullPath.Length - CatalogConstants.MandatoryNodePathLength), out catalogNode))
              catalogNode.UpdateSelf(nodeReference);
          }
        }
        this.m_nodes.Clear();
        this.m_resources.Clear();
        this.m_moves.Clear();
      }
      List<CatalogResource> matchingResources1;
      this.m_catalogService.BuildCatalogObjects(catalogData.LocationServiceLastChangeId, catalogData.DeletedResources, Array.Empty<CatalogNode>(), catalogResourceTypes, CatalogQueryOptions.None, out matchingResources1, out List<CatalogNode> _);
      List<CatalogNode> matchingNodes;
      this.m_catalogService.BuildCatalogObjects(catalogData.LocationServiceLastChangeId, catalogData.DeletedNodeResources, catalogData.DeletedNodes, catalogResourceTypes, CatalogQueryOptions.None, out List<CatalogResource> _, out matchingNodes);
      return new CatalogChangeResult()
      {
        DeletedNodes = matchingNodes.AsReadOnly(),
        DeletedResources = matchingResources1.AsReadOnly()
      };
    }
  }
}
