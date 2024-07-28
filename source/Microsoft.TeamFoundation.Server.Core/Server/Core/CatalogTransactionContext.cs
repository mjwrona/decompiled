// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.CatalogTransactionContext
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Location;
using Microsoft.VisualStudio.Services.Location.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Server.Core
{
  public class CatalogTransactionContext
  {
    private bool m_runRulesOnSave;
    private bool m_allowEntireCatalogDeletion;
    private Dictionary<Guid, CatalogResource> m_resources = new Dictionary<Guid, CatalogResource>();
    private Dictionary<string, CatalogNode> m_nodes = new Dictionary<string, CatalogNode>((IEqualityComparer<string>) VssStringComparer.CatalogNodePath);
    private List<KeyValuePair<CatalogNode, bool>> m_deletes = new List<KeyValuePair<CatalogNode, bool>>();
    private List<KeyValuePair<CatalogNode, CatalogNode>> m_moves = new List<KeyValuePair<CatalogNode, CatalogNode>>();
    private static readonly string s_catalogLockingResource = "CatalogUpdates";

    internal CatalogTransactionContext(bool runRulesOnSave)
      : this(runRulesOnSave, false)
    {
    }

    internal CatalogTransactionContext(bool runRulesOnSave, bool allowEntireCatalogDeletion)
    {
      this.m_runRulesOnSave = runRulesOnSave;
      this.m_allowEntireCatalogDeletion = allowEntireCatalogDeletion;
    }

    public bool IsReadOnly(IVssRequestContext requestContext) => VirtualCatalogServiceHelper.IsCatalogVirtual(requestContext);

    public void AttachResource(CatalogResource catalogResource)
    {
      ArgumentUtility.CheckForNull<CatalogResource>(catalogResource, nameof (catalogResource));
      this.m_resources[catalogResource.TempCorrelationId] = catalogResource;
    }

    public void AttachNode(CatalogNode catalogNode)
    {
      ArgumentUtility.CheckForNull<CatalogNode>(catalogNode, nameof (catalogNode));
      ArgumentUtility.CheckForNull<CatalogResource>(catalogNode.Resource, "catalogNode.Resource");
      this.AttachResource(catalogNode.Resource);
      this.m_nodes[catalogNode.ParentPath + catalogNode.ChildItem] = catalogNode;
    }

    public void AttachDelete(CatalogNode catalogNode, bool recurse)
    {
      ArgumentUtility.CheckForNull<CatalogNode>(catalogNode, nameof (catalogNode));
      if (string.IsNullOrEmpty(catalogNode.FullPath))
        throw new CatalogNodeDoesNotExistException();
      this.m_deletes.Add(new KeyValuePair<CatalogNode, bool>(catalogNode, recurse));
    }

    public void AttachMove(CatalogNode nodeToMove, CatalogNode newParent)
    {
      ArgumentUtility.CheckForNull<CatalogNode>(nodeToMove, nameof (nodeToMove));
      ArgumentUtility.CheckForNull<CatalogNode>(newParent, nameof (newParent));
      this.AttachNode(nodeToMove);
      this.AttachNode(newParent);
      this.m_moves.Add(new KeyValuePair<CatalogNode, CatalogNode>(nodeToMove, newParent));
    }

    internal virtual List<CatalogResource> Save(IVssRequestContext requestContext, bool preview) => this.Save(requestContext, preview, out List<CatalogResource> _, out List<CatalogNode> _);

    internal List<CatalogResource> Save(
      IVssRequestContext requestContext,
      bool preview,
      out List<CatalogResource> deletedResources,
      out List<CatalogNode> deletedNodes)
    {
      return this.Save(requestContext, CatalogQueryOptions.None, preview, out deletedResources, out deletedNodes);
    }

    internal List<CatalogResource> Save(
      IVssRequestContext requestContext,
      CatalogQueryOptions queryOptions,
      bool preview,
      out List<CatalogResource> deletedResources,
      out List<CatalogNode> deletedNodes)
    {
      TeamFoundationTrace.Info("Entered CatalogTransactionContext.Save().");
      if (this.IsReadOnly(requestContext))
      {
        deletedResources = new List<CatalogResource>();
        deletedNodes = new List<CatalogNode>();
        return new List<CatalogResource>();
      }
      TeamFoundationCatalogService service1 = requestContext.GetService<TeamFoundationCatalogService>();
      Dictionary<string, object> newDefaults = new Dictionary<string, object>((IEqualityComparer<string>) VssStringComparer.CatalogNodePath);
      Dictionary<Guid, object> dictionary1 = new Dictionary<Guid, object>();
      foreach (CatalogNode node in this.m_nodes.Values)
      {
        CatalogNode.Validate(newDefaults, node);
        dictionary1[node.Resource.TempCorrelationId] = (object) null;
        queryOptions |= node.Dependencies != null ? CatalogQueryOptions.ExpandDependencies : CatalogQueryOptions.None;
        queryOptions |= node.ParentNode != null ? CatalogQueryOptions.IncludeParents : CatalogQueryOptions.None;
      }
      ILocationService service2 = requestContext.GetService<ILocationService>();
      List<ServiceDefinition> serviceDefinitionList = new List<ServiceDefinition>();
      foreach (CatalogResource catalogResource in this.m_resources.Values)
      {
        CatalogResource.Validate(catalogResource);
        if (catalogResource.Identifier == Guid.Empty && !dictionary1.ContainsKey(catalogResource.TempCorrelationId))
          throw new InvalidCatalogSaveResourceException(FrameworkResources.CatalogUnreferencedResourceCreateMessage((object) catalogResource.DisplayName));
        service1.QueryResourceType(requestContext, catalogResource.ResourceType.Identifier);
        foreach (ServiceDefinition serviceDefinition in (IEnumerable<ServiceDefinition>) catalogResource.ServiceReferences.Values)
        {
          if (serviceDefinition == null)
            throw new TeamFoundationValidationException(FrameworkResources.ArgumentPropertyCannotBeNull((object) "ServiceReferences.ServiceDefinition"), "catalogResource");
          if (!VssStringComparer.ServiceType.Equals(serviceDefinition.ServiceType, "LocationService") && !VssStringComparer.ToolId.Equals(serviceDefinition.ToolId, "TSWebAccess"))
            serviceDefinitionList.Add(serviceDefinition);
        }
      }
      Dictionary<string, string> dictionary2 = new Dictionary<string, string>((IEqualityComparer<string>) VssStringComparer.CatalogNodePath);
      foreach (KeyValuePair<CatalogNode, CatalogNode> move in this.m_moves)
      {
        string str1 = move.Key.ParentPath + move.Key.ChildItem;
        if (VssStringComparer.CatalogNodePath.StartsWith(move.Value.ParentPath + move.Value.ChildItem, str1))
          throw new InvalidCatalogNodeMoveException(FrameworkResources.CannotMoveCatalogNodeBelowItself());
        if (dictionary2.ContainsKey(str1))
          throw new InvalidCatalogNodeMoveException(FrameworkResources.InvalidDoubleCatalogNodeMove());
        string str2 = move.Value.FullPath + move.Key.ChildItem;
        dictionary2[str1] = str2;
      }
      List<string> stringList = new List<string>();
      foreach (KeyValuePair<CatalogNode, bool> delete in this.m_deletes)
      {
        CatalogPathSpec.ValidatePathSpec(delete.Key.FullPath, false, false);
        if (dictionary2.ContainsKey(delete.Key.FullPath))
          throw new InvalidCatalogNodeMoveException(FrameworkResources.InvalidCatalogNodeMoveDelete());
        if (!delete.Value)
          stringList.Add(delete.Key.FullPath);
      }
      Dictionary<string, CatalogNode> nodes = new Dictionary<string, CatalogNode>((IDictionary<string, CatalogNode>) this.m_nodes, (IEqualityComparer<string>) VssStringComparer.CatalogNodePath);
      List<CatalogResource> catalogResourceList1 = (List<CatalogResource>) null;
      TeamFoundationLockingService service3 = requestContext.GetService<TeamFoundationLockingService>();
      TeamFoundationTrace.Info("Asking for the Catalog database lock.");
      string str3 = CatalogTransactionContext.s_catalogLockingResource + requestContext.ServiceHost.InstanceId.ToString("n");
      IVssRequestContext requestContext1 = requestContext;
      string resource = str3;
      using (service3.AcquireLock(requestContext1, TeamFoundationLockMode.Exclusive, resource))
      {
        TeamFoundationTrace.Info("Got the Catalog database lock.");
        if (stringList.Count > 0)
        {
          stringList.Sort((IComparer<string>) VssStringComparer.CatalogNodePath);
          List<string> pathSpecs = new List<string>();
          foreach (string key in stringList)
          {
            pathSpecs.Add(key + CatalogConstants.SingleRecurseStar);
            string str4 = key.Substring(0, key.Length - CatalogConstants.MandatoryNodePathLength);
            for (int length = str4.Length; dictionary2.Count > 0 && length > CatalogConstants.MandatoryNodePathLength; length -= CatalogConstants.MandatoryNodePathLength)
            {
              string str5;
              if (dictionary2.TryGetValue(str4.Substring(0, length), out str5))
              {
                str4 = length != str4.Length ? str5 + str4.Substring(length) : str5;
                break;
              }
            }
            dictionary2[key] = str4;
          }
          foreach (CatalogNode queryNode in (IEnumerable<CatalogNode>) service1.QueryNodes(requestContext, (IEnumerable<string>) pathSpecs, (IEnumerable<Guid>) null, CatalogQueryOptions.ExpandDependencies))
            nodes[queryNode.FullPath] = queryNode;
        }
        if (this.m_runRulesOnSave)
        {
          CatalogRuleValidationUtility utility = new CatalogRuleValidationUtility((ITeamFoundationCatalogService) service1, this.m_resources, nodes, this.m_deletes, this.m_moves, dictionary2);
          TeamFoundationTrace.Info("Running the catalog rules.");
          service1.CatalogValidator.ValidateChanges(requestContext, (ITeamFoundationCatalogService) service1, utility, (IEnumerable<CatalogNode>) this.m_nodes.Values, (IEnumerable<CatalogResource>) this.m_resources.Values, (IEnumerable<KeyValuePair<CatalogNode, bool>>) this.m_deletes);
          TeamFoundationTrace.Info("Finished running the catalog rules.");
        }
        if (!preview)
          service2.SaveServiceDefinitions(requestContext, (IEnumerable<ServiceDefinition>) serviceDefinitionList);
        List<CatalogResource> databaseResources = (List<CatalogResource>) null;
        List<CatalogServiceReference> databaseServiceReferences = (List<CatalogServiceReference>) null;
        List<CatalogNode> databaseNodes = (List<CatalogNode>) null;
        List<CatalogNodeDependency> databaseDependencies = (List<CatalogNodeDependency>) null;
        List<CatalogResource> items1;
        List<CatalogNode> items2;
        List<CatalogResource> items3;
        using (CatalogComponent component = requestContext.CreateComponent<CatalogComponent>())
        {
          TeamFoundationTrace.Info("Calling the catalog database to save the changes.");
          ResultCollection rc = component.SaveCatalogChanges((IEnumerable<CatalogResource>) this.m_resources.Values, (IEnumerable<CatalogNode>) this.m_nodes.Values, (IEnumerable<KeyValuePair<CatalogNode, bool>>) this.m_deletes, (IEnumerable<KeyValuePair<string, string>>) dictionary2, queryOptions, preview, this.m_allowEntireCatalogDeletion);
          items1 = rc.GetCurrent<CatalogResource>().Items;
          rc.NextResult();
          items2 = rc.GetCurrent<CatalogNode>().Items;
          rc.NextResult();
          items3 = rc.GetCurrent<CatalogResource>().Items;
          if (!preview)
          {
            rc.NextResult();
            service1.GetResources(requestContext, rc, queryOptions, out databaseResources, out databaseServiceReferences, out databaseNodes, out databaseDependencies);
          }
          TeamFoundationTrace.Info("Returned from the catalog database after saving changes.");
        }
        if (this.m_deletes.Count > 0 || this.m_moves.Count > 0)
          requestContext.GetService<TeamFoundationSecurityService>().GetSecurityNamespace(requestContext, FrameworkSecurity.CatalogNamespaceId).OnDataChanged(requestContext);
        List<CatalogNode> matchingNodes;
        List<CatalogResource> matchingResources;
        service1.BuildCatalogObjects(requestContext, items1, new List<CatalogServiceReference>(), new List<CatalogNode>(), new List<CatalogNodeDependency>(), CatalogQueryOptions.None, out matchingNodes, out matchingResources);
        List<CatalogResource> catalogResourceList2 = new List<CatalogResource>();
        foreach (CatalogResource catalogResource in matchingResources)
          catalogResourceList2.Add(catalogResource);
        deletedResources = catalogResourceList2;
        service1.BuildCatalogObjects(requestContext, items3, new List<CatalogServiceReference>(), items2, new List<CatalogNodeDependency>(), CatalogQueryOptions.None, out matchingNodes, out matchingResources);
        TeamFoundationTrace.Info("Finished building the returned catalog objects.");
        List<CatalogNode> catalogNodeList = new List<CatalogNode>();
        foreach (CatalogNode catalogNode in matchingNodes)
          catalogNodeList.Add(catalogNode);
        deletedNodes = catalogNodeList;
        catalogResourceList1 = preview ? new List<CatalogResource>() : service1.BuildResources(requestContext, queryOptions, databaseResources, databaseServiceReferences, databaseNodes, databaseDependencies);
      }
      if (!preview)
      {
        foreach (CatalogResource updatedResource in catalogResourceList1)
        {
          CatalogResource catalogResource;
          if (this.m_resources.TryGetValue(updatedResource.Identifier, out catalogResource))
          {
            catalogResource.UpdateSelf(updatedResource);
            this.m_resources.Remove(updatedResource.Identifier);
          }
          foreach (CatalogNode nodeReference in updatedResource.NodeReferences)
          {
            string key = nodeReference.ParentPath + nodeReference.ChildItem;
            CatalogNode catalogNode;
            if (this.m_nodes.TryGetValue(key, out catalogNode))
            {
              catalogNode.UpdateSelf(nodeReference);
              this.m_nodes.Remove(key);
            }
          }
        }
        this.m_resources.Clear();
        this.m_nodes.Clear();
        this.m_moves.Clear();
        this.m_deletes.Clear();
      }
      TeamFoundationTrace.Info("Exiting CatalogTransactionContext.SaveCatalogChanges().");
      return catalogResourceList1;
    }

    internal IEnumerable<CatalogResource> ResourceChanges => (IEnumerable<CatalogResource>) this.m_resources.Values;

    internal IEnumerable<CatalogNode> NodeChanges => (IEnumerable<CatalogNode>) this.m_nodes.Values;

    internal IEnumerable<KeyValuePair<CatalogNode, bool>> Deletes => (IEnumerable<KeyValuePair<CatalogNode, bool>>) this.m_deletes;

    internal IEnumerable<KeyValuePair<CatalogNode, CatalogNode>> Moves => (IEnumerable<KeyValuePair<CatalogNode, CatalogNode>>) this.m_moves;
  }
}
