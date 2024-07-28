// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.CatalogRuleValidationUtility
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Location;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.Core
{
  public class CatalogRuleValidationUtility
  {
    private ITeamFoundationCatalogService m_catalogService;
    private Dictionary<Guid, CatalogResource> m_resources;
    private Dictionary<string, CatalogNode> m_nodes;
    private List<KeyValuePair<CatalogNode, bool>> m_deletes;
    private Dictionary<string, bool> m_deleteByPath;
    private List<KeyValuePair<CatalogNode, CatalogNode>> m_moves;
    private Dictionary<string, string> m_moveSourceToTarget;
    private Dictionary<Guid, List<CatalogNode>> m_nodeReferenceCache = new Dictionary<Guid, List<CatalogNode>>();

    public CatalogRuleValidationUtility(
      ITeamFoundationCatalogService catalogService,
      Dictionary<Guid, CatalogResource> resources,
      Dictionary<string, CatalogNode> nodes,
      List<KeyValuePair<CatalogNode, bool>> deletes,
      List<KeyValuePair<CatalogNode, CatalogNode>> moves,
      Dictionary<string, string> moveSourceToTarget)
    {
      this.m_catalogService = catalogService;
      this.m_resources = resources;
      this.m_nodes = nodes;
      this.m_deletes = deletes;
      this.m_moves = moves;
      this.m_moveSourceToTarget = moveSourceToTarget;
      this.m_deleteByPath = new Dictionary<string, bool>();
      foreach (KeyValuePair<CatalogNode, bool> delete in deletes)
        this.m_deleteByPath[delete.Key.FullPath] = delete.Value;
      this.m_moveSourceToTarget = new Dictionary<string, string>((IEqualityComparer<string>) VssStringComparer.CatalogNodePath);
      foreach (KeyValuePair<CatalogNode, CatalogNode> move in moves)
        this.m_moveSourceToTarget[move.Key.ParentPath + move.Key.ChildItem] = move.Value.ParentPath + move.Value.ChildItem + move.Key.ChildItem;
    }

    public CatalogNode FindParent(IVssRequestContext requestContext, CatalogNode node)
    {
      foreach (KeyValuePair<CatalogNode, CatalogNode> move in this.m_moves)
      {
        if (VssStringComparer.CatalogNodePath.Equals(move.Key.FullPath, node.FullPath))
          return move.Value;
      }
      if (node.ParentPath.Length == 0)
        return (CatalogNode) null;
      bool flag1 = true;
      string key = node.ParentPath;
      while (flag1)
      {
        flag1 = false;
        bool flag2;
        if (this.m_deleteByPath.TryGetValue(key, out flag2) && !flag2)
        {
          key = key.Substring(0, key.Length - CatalogConstants.MandatoryNodePathLength);
          flag1 = true;
        }
      }
      CatalogNode parent;
      if (this.m_nodes.TryGetValue(key, out parent))
        return parent;
      List<CatalogNode> catalogNodeList = this.m_catalogService.QueryNodes(requestContext, (IEnumerable<string>) new string[1]
      {
        node.ParentPath
      }, (IEnumerable<Guid>) null, CatalogQueryOptions.None);
      return catalogNodeList.Count != 0 ? catalogNodeList[0] : throw new InvalidCatalogSaveNodeException(FrameworkResources.CatalogNodeParentDoesNotExist((object) node.Resource.DisplayName));
    }

    public void CheckDependenciesConstraints(
      IVssRequestContext requestContext,
      CatalogNode node,
      DependencyConstraint[] dependencyConstraints)
    {
      if (dependencyConstraints == null)
        return;
      if (node.Dependencies == null)
        node.ExpandDependencies(requestContext);
      foreach (DependencyConstraint dependencyConstraint in dependencyConstraints)
      {
        CatalogNode catalogNode;
        if (dependencyConstraint.IsSingleton && node.Dependencies.Singletons.TryGetValue(dependencyConstraint.DependencyName, out catalogNode))
        {
          this.ValidateNodeDependencies(requestContext, node, (IEnumerable<CatalogNode>) new CatalogNode[1]
          {
            catalogNode
          }, dependencyConstraint);
        }
        else
        {
          if (dependencyConstraint.IsSingleton)
            throw new InvalidCatalogSaveNodeException(FrameworkResources.CatalogNodeDependencyMissing((object) node.Resource.ResourceType.DisplayName, (object) dependencyConstraint.DependencyName));
          foreach (KeyValuePair<string, IList<CatalogNode>> set in (IEnumerable<KeyValuePair<string, IList<CatalogNode>>>) node.Dependencies.Sets)
            this.ValidateNodeDependencies(requestContext, node, (IEnumerable<CatalogNode>) set.Value, dependencyConstraint);
        }
      }
    }

    private void ValidateNodeDependencies(
      IVssRequestContext requestContext,
      CatalogNode node,
      IEnumerable<CatalogNode> dependencies,
      DependencyConstraint constraint)
    {
      foreach (CatalogNode dependency in dependencies)
      {
        if (!this.WillBeDeleted(requestContext, dependency, out string _))
        {
          if (constraint.ResourceTypes == null || constraint.ResourceTypes.Length == 0)
            break;
          bool flag = false;
          foreach (Guid resourceType in constraint.ResourceTypes)
          {
            if (object.Equals((object) dependency.Resource.ResourceType.Identifier, (object) resourceType))
            {
              flag = true;
              break;
            }
          }
          if (flag)
            break;
          throw new InvalidCatalogSaveNodeException(FrameworkResources.CatalogNodeDependencyIncorrectType((object) node.Resource.DisplayName, (object) constraint.DependencyName));
        }
      }
    }

    public void CheckPropertyConstraints(
      IVssRequestContext requestContext,
      CatalogResource resource,
      PropertyConstraint[] constraints)
    {
      if (constraints == null)
        return;
      foreach (PropertyConstraint constraint in constraints)
      {
        string propertyValue;
        if (!resource.Properties.TryGetValue(constraint.PropertyName, out propertyValue) && constraint.PropertyExistence != PropertyExistence.Optional)
          throw new InvalidCatalogSaveResourceException(FrameworkResources.MissingCatalogResourcePropertyMessage((object) resource.ResourceType.DisplayName, (object) constraint.PropertyName));
        if (constraint.ValueExclusivity != PropertyValueExclusivity.None && propertyValue != null)
          this.CheckForDuplicatePropertyValues(requestContext, resource, constraint.ValueExclusivity, constraint.PropertyName, propertyValue);
      }
    }

    public void CheckServiceReferenceConstraints(
      CatalogResource resource,
      ServiceReferenceConstraint[] constraints)
    {
      if (constraints == null)
        return;
      foreach (ServiceReferenceConstraint constraint in constraints)
      {
        ServiceDefinition serviceDefinition;
        if (!resource.ServiceReferences.TryGetValue(constraint.ReferenceName, out serviceDefinition) || !string.IsNullOrEmpty(constraint.ServiceType) && !VssStringComparer.ServiceType.Equals(serviceDefinition.ServiceType, constraint.ServiceType))
          throw new InvalidCatalogSaveResourceException(FrameworkResources.MissingCatalogServiceReferenceMessage((object) resource.ResourceType.DisplayName, (object) constraint.ReferenceName, (object) constraint.ServiceType));
      }
    }

    public void CheckExclusiveNodeReference(
      IVssRequestContext requestContext,
      CatalogNode node,
      bool singleNodeReference)
    {
      if (!singleNodeReference)
        return;
      foreach (CatalogNode catalogNode in this.m_nodes.Values)
      {
        if (node.Resource.TempCorrelationId.Equals(catalogNode.Resource.TempCorrelationId) && !VssStringComparer.CatalogNodePath.Equals(node.ChildItem, catalogNode.ChildItem))
          throw new InvalidCatalogSaveNodeException(FrameworkResources.ExclusiveNodeReferenceFailure((object) node.Resource.DisplayName, (object) node.Resource.ResourceType.DisplayName));
      }
      if (node.Resource.Identifier.Equals(Guid.Empty))
        return;
      List<CatalogResource> catalogResourceList = this.m_catalogService.QueryResources(requestContext, (IEnumerable<Guid>) new Guid[1]
      {
        node.Resource.Identifier
      }, CatalogQueryOptions.ExpandDependencies);
      if (catalogResourceList.Count != 1)
        return;
      foreach (CatalogNode nodeReference in catalogResourceList[0].NodeReferences)
      {
        if (!VssStringComparer.CatalogNodePath.Equals(node.ChildItem, nodeReference.ChildItem) && !this.WillBeDeleted(requestContext, nodeReference, out string _))
          throw new InvalidCatalogSaveNodeException(FrameworkResources.ExclusiveNodeReferenceFailure((object) node.Resource.DisplayName, (object) node.Resource.ResourceType.DisplayName));
      }
    }

    public void CheckExclusiveTypePerParentExistence(
      IVssRequestContext requestContext,
      CatalogNode node,
      ExistencePerParentTypeConstraint[] constraints)
    {
      if (constraints == null)
        return;
      foreach (ExistencePerParentTypeConstraint constraint in constraints)
      {
        CatalogNode parent = this.FindParent(requestContext, node);
        if (!parent.Resource.ResourceType.Identifier.Equals(constraint.ResourceType))
          break;
        string str = parent.ParentPath + parent.ChildItem;
        this.CheckForTypeAndPathCollision(requestContext, node, str, false, (IEnumerable<CatalogNode>) this.m_nodes.Values.ToArray<CatalogNode>());
        List<string> pathSpecs = new List<string>();
        foreach (KeyValuePair<string, string> keyValuePair in this.m_moveSourceToTarget)
        {
          if (VssStringComparer.CatalogNodePath.StartsWith(keyValuePair.Value, str) && !VssStringComparer.CatalogNodePath.StartsWith(keyValuePair.Key, str))
            pathSpecs.Add(keyValuePair.Key + CatalogConstants.SingleRecurseStar);
        }
        if (!string.IsNullOrEmpty(parent.FullPath))
          pathSpecs.Add(str + CatalogConstants.SingleRecurseStar);
        if (pathSpecs.Count == 0)
          break;
        List<CatalogNode> nodes = this.m_catalogService.QueryNodes(requestContext, (IEnumerable<string>) pathSpecs, (IEnumerable<Guid>) new Guid[1]
        {
          node.Resource.ResourceType.Identifier
        }, CatalogQueryOptions.ExpandDependencies);
        this.CheckForTypeAndPathCollision(requestContext, node, str, false, (IEnumerable<CatalogNode>) nodes);
      }
    }

    public void CheckExclusiveTypePerRootExistence(
      IVssRequestContext requestContext,
      CatalogNode node,
      ExistencePerRootConstraint[] constraints)
    {
      if (constraints == null)
        return;
      foreach (ExistencePerRootConstraint constraint in constraints)
      {
        if (!string.IsNullOrEmpty(node.ParentPath))
        {
          string str = node.ParentPath.Substring(0, CatalogConstants.MandatoryNodePathLength);
          this.CheckForTypeAndPathCollision(requestContext, node, str, true, (IEnumerable<CatalogNode>) this.m_nodes.Values.ToArray<CatalogNode>());
          List<string> pathSpecs = new List<string>();
          foreach (KeyValuePair<string, string> keyValuePair in this.m_moveSourceToTarget)
          {
            if (VssStringComparer.CatalogNodePath.StartsWith(keyValuePair.Value, str) && !VssStringComparer.CatalogNodePath.StartsWith(keyValuePair.Key, str))
              pathSpecs.Add(keyValuePair.Key + CatalogConstants.FullRecurseStars);
          }
          pathSpecs.Add(str + CatalogConstants.FullRecurseStars);
          List<CatalogNode> nodes = this.m_catalogService.QueryNodes(requestContext, (IEnumerable<string>) pathSpecs, (IEnumerable<Guid>) new Guid[1]
          {
            node.Resource.ResourceType.Identifier
          }, CatalogQueryOptions.ExpandDependencies);
          this.CheckForTypeAndPathCollision(requestContext, node, str, true, (IEnumerable<CatalogNode>) nodes);
        }
      }
    }

    public void CheckDeleteConstraints(
      IVssRequestContext requestContext,
      CatalogNode nodeToDelete,
      bool recursive,
      DeleteConstraints constraints)
    {
      if (constraints == null)
        return;
      if (!constraints.CanDeleteRecursively && !constraints.CanDeleteNonRecursively)
        throw new InvalidCatalogDeleteNodeException(FrameworkResources.CatalogInvalidDeleteResourceType((object) nodeToDelete.Resource.ResourceType.DisplayName));
      if (recursive && !constraints.CanDeleteRecursively)
        throw new InvalidCatalogDeleteNodeException(FrameworkResources.CatalogInvalidRecursiveDelete((object) nodeToDelete.Resource.ResourceType.DisplayName));
      if (!recursive && !constraints.CanDeleteNonRecursively)
        throw new InvalidCatalogDeleteNodeException(FrameworkResources.CatalogInvalidNonRecursiveDelete((object) nodeToDelete.Resource.ResourceType.DisplayName));
    }

    public void CheckForDuplicatePropertyValues(
      IVssRequestContext requestContext,
      CatalogResource catalogResource,
      PropertyValueExclusivity valueExclusivity,
      string propertyName,
      string propertyValue)
    {
      List<string> pathScopes = new List<string>();
      bool recurse;
      switch (valueExclusivity)
      {
        case PropertyValueExclusivity.Parent:
          foreach (CatalogNode allNodeReference in this.DetermineAllNodeReferences(catalogResource))
          {
            string finalPath;
            if (!this.WillBeDeleted(requestContext, allNodeReference, out finalPath))
            {
              string str = finalPath.Substring(0, finalPath.Length - CatalogConstants.MandatoryNodePathLength);
              pathScopes.Add(str);
            }
          }
          recurse = false;
          break;
        case PropertyValueExclusivity.Subtree:
          foreach (CatalogNode allNodeReference in this.DetermineAllNodeReferences(catalogResource))
          {
            string finalPath;
            if (!this.WillBeDeleted(requestContext, allNodeReference, out finalPath))
            {
              string str = finalPath.Substring(0, CatalogConstants.MandatoryNodePathLength);
              pathScopes.Add(str);
            }
          }
          recurse = true;
          break;
        case PropertyValueExclusivity.EntireCatalog:
          pathScopes.Add(string.Empty);
          recurse = true;
          break;
        default:
          return;
      }
      foreach (CatalogResource duplicateResource in this.m_resources.Values)
      {
        Guid guid = duplicateResource.ResourceType.Identifier;
        if (guid.Equals(catalogResource.ResourceType.Identifier))
        {
          guid = duplicateResource.TempCorrelationId;
          string y;
          if (!guid.Equals(catalogResource.TempCorrelationId) && duplicateResource.Properties.TryGetValue(propertyName, out y) && StringComparer.OrdinalIgnoreCase.Equals(propertyValue, y) && !this.ResourceDeletedOrMovedOutOfScope(requestContext, pathScopes, recurse, duplicateResource))
            throw new InvalidCatalogSaveResourceException(FrameworkResources.Catalog_Validation_Exception_DuplicateProperty((object) propertyName, (object) propertyValue));
        }
      }
      foreach (CatalogResource queryResource in this.m_catalogService.QueryResources(requestContext, (IEnumerable<Guid>) new Guid[1]
      {
        catalogResource.ResourceType.Identifier
      }, (IEnumerable<KeyValuePair<string, string>>) new List<KeyValuePair<string, string>>()
      {
        new KeyValuePair<string, string>(propertyName, propertyValue)
      }, CatalogQueryOptions.None))
      {
        if (!(queryResource.TempCorrelationId == catalogResource.TempCorrelationId) && !this.m_resources.ContainsKey(queryResource.TempCorrelationId) && !this.ResourceDeletedOrMovedOutOfScope(requestContext, pathScopes, recurse, queryResource))
          throw new InvalidCatalogSaveResourceException(FrameworkResources.Catalog_Validation_Exception_DuplicateProperty((object) propertyName, (object) propertyValue));
      }
    }

    public List<CatalogNode> DetermineAllNodeReferences(CatalogResource resource)
    {
      List<CatalogNode> allNodeReferences1;
      if (this.m_nodeReferenceCache.TryGetValue(resource.TempCorrelationId, out allNodeReferences1))
        return allNodeReferences1;
      List<CatalogNode> allNodeReferences2 = new List<CatalogNode>((IEnumerable<CatalogNode>) resource.NodeReferences);
      foreach (CatalogNode catalogNode in this.m_nodes.Values)
      {
        if (catalogNode.Resource.TempCorrelationId.Equals(resource.TempCorrelationId))
          allNodeReferences2.Add(catalogNode);
      }
      this.m_nodeReferenceCache.Add(resource.TempCorrelationId, allNodeReferences2);
      return allNodeReferences2;
    }

    public bool ResourceDeletedOrMovedOutOfScope(
      IVssRequestContext requestContext,
      List<string> pathScopes,
      bool recurse,
      CatalogResource duplicateResource)
    {
      List<string> finalPaths;
      if (this.WillBeDeleted(requestContext, duplicateResource, out finalPaths))
        return true;
      bool flag = false;
      foreach (string itemPath in finalPaths)
      {
        foreach (string pathScope in pathScopes)
        {
          if (flag = this.UnderPathScope(itemPath, pathScope, recurse))
            break;
        }
        if (flag)
          break;
      }
      return !flag;
    }

    public bool UnderPathScope(string itemPath, string pathScope, bool recurse) => VssStringComparer.CatalogNodePath.Equals(itemPath.Substring(0, itemPath.Length - CatalogConstants.MandatoryNodePathLength), pathScope) || recurse && VssStringComparer.CatalogNodePath.StartsWith(itemPath, pathScope);

    public void CheckForTypeAndPathCollision(
      IVssRequestContext requestContext,
      CatalogNode singletonNode,
      string pathScope,
      bool recurse,
      IEnumerable<CatalogNode> nodes)
    {
      foreach (CatalogNode node in nodes)
      {
        string finalPath;
        if (node.Resource.ResourceType.Identifier.Equals(singletonNode.Resource.ResourceType.Identifier) && !VssStringComparer.CatalogNodePath.Equals(node.ChildItem, singletonNode.ChildItem) && !this.WillBeDeleted(requestContext, node, out finalPath))
        {
          string x = finalPath.Substring(0, finalPath.Length - CatalogConstants.MandatoryNodePathLength);
          if ((!recurse || VssStringComparer.CatalogNodePath.StartsWith(finalPath, pathScope)) && (recurse || VssStringComparer.CatalogNodePath.Equals(x, pathScope)))
          {
            CatalogNode queryNode;
            if (!this.m_nodes.TryGetValue(pathScope, out queryNode))
              queryNode = this.m_catalogService.QueryNodes(requestContext, (IEnumerable<string>) new string[1]
              {
                pathScope
              }, (IEnumerable<Guid>) null, CatalogQueryOptions.None)[0];
            throw new InvalidCatalogSaveResourceException(FrameworkResources.ExclusiveResourceTypeExistenceFailure((object) singletonNode.Resource.ResourceType.DisplayName, (object) queryNode.Resource.DisplayName));
          }
        }
      }
    }

    public string DeterminePathAfterMoves(string path)
    {
      for (int mandatoryNodePathLength = CatalogConstants.MandatoryNodePathLength; mandatoryNodePathLength <= path.Length; mandatoryNodePathLength += CatalogConstants.MandatoryNodePathLength)
      {
        string str;
        if (this.m_moveSourceToTarget.TryGetValue(path.Substring(0, mandatoryNodePathLength), out str))
          path = str + path.Substring(mandatoryNodePathLength);
      }
      return path;
    }

    public bool WillBeDeleted(
      IVssRequestContext requestContext,
      CatalogResource resource,
      out List<string> finalPaths)
    {
      finalPaths = new List<string>();
      foreach (CatalogNode allNodeReference in this.DetermineAllNodeReferences(resource))
      {
        string finalPath;
        if (!this.WillBeDeleted(requestContext, allNodeReference, out finalPath))
          finalPaths.Add(finalPath);
      }
      return finalPaths.Count == 0;
    }

    public bool WillBeDeleted(
      IVssRequestContext requestContext,
      CatalogNode node,
      out string finalPath)
    {
      Dictionary<string, object> seenDependencies = new Dictionary<string, object>();
      return this.WillBeDeletedHelper(requestContext, node, seenDependencies, out finalPath);
    }

    public bool WillBeDeletedHelper(
      IVssRequestContext requestContext,
      CatalogNode node,
      Dictionary<string, object> seenDependencies,
      out string finalPath)
    {
      string str = node.ParentPath + node.ChildItem;
      finalPath = this.DeterminePathAfterMoves(str);
      for (int mandatoryNodePathLength = CatalogConstants.MandatoryNodePathLength; mandatoryNodePathLength <= finalPath.Length; mandatoryNodePathLength += CatalogConstants.MandatoryNodePathLength)
      {
        bool flag;
        if (this.m_deleteByPath.TryGetValue(finalPath.Substring(0, mandatoryNodePathLength), out flag) && (flag || mandatoryNodePathLength == finalPath.Length))
          return true;
      }
      CatalogNode catalogNode;
      if (this.m_nodes.TryGetValue(str, out catalogNode) && catalogNode.Dependencies != null)
        node = catalogNode;
      if (node.Dependencies == null)
        node.ExpandDependencies(requestContext);
      List<CatalogNode> catalogNodeList = new List<CatalogNode>();
      catalogNodeList.AddRange((IEnumerable<CatalogNode>) node.Dependencies.Singletons.Values);
      foreach (KeyValuePair<string, IList<CatalogNode>> set in (IEnumerable<KeyValuePair<string, IList<CatalogNode>>>) node.Dependencies.Sets)
        catalogNodeList.AddRange((IEnumerable<CatalogNode>) set.Value);
      foreach (CatalogNode node1 in catalogNodeList)
      {
        if (!seenDependencies.ContainsKey(node1.ChildItem))
        {
          seenDependencies[node1.ChildItem] = (object) null;
          if (this.WillBeDeletedHelper(requestContext, node1, seenDependencies, out string _))
            return true;
        }
      }
      return false;
    }
  }
}
