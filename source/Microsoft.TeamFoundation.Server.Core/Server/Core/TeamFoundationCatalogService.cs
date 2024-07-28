// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.TeamFoundationCatalogService
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.TeamFoundation.Common.Internal;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.Location;
using Microsoft.VisualStudio.Services.Location.Server;
using Microsoft.VisualStudio.Services.Security;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.Core
{
  public sealed class TeamFoundationCatalogService : 
    VssBaseService,
    ILegacyTeamFoundationCatalogService,
    ITeamFoundationCatalogService,
    IVssFrameworkService,
    IDisposable
  {
    private Dictionary<Guid, CatalogResourceType> m_resourceTypes;
    private bool m_resourceTypesLoaded;
    private int m_cacheVersion = -1;
    private ILockName m_lock;
    private Guid m_instanceId;
    private bool m_runRulesOnSave = true;
    private static readonly string s_area = "Catalog";
    private static readonly string s_layer = "IVssFrameworkService";

    internal TeamFoundationCatalogService()
    {
    }

    void IVssFrameworkService.ServiceStart(IVssRequestContext requestContext)
    {
      if (requestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection))
      {
        bool flag = false;
        object x;
        if (requestContext.IsServicingContext && requestContext.Items.TryGetValue(RequestContextItemsKeys.CurrentServicingOperation, out x) && (VssStringComparer.ServicingOperation.Equals(x, (object) ServicingOperationConstants.AttachCollection) || VssStringComparer.ServicingOperation.Equals(x, (object) ServicingOperationConstants.Snapshot) || VssStringComparer.ServicingOperation.StartsWith(x.ToString(), ServicingOperationConstants.DataImport)))
        {
          flag = true;
          this.m_runRulesOnSave = false;
        }
        if (!flag)
          throw new UnexpectedHostTypeException(requestContext.ServiceHost.HostType);
      }
      this.m_resourceTypes = new Dictionary<Guid, CatalogResourceType>();
      this.m_lock = this.CreateLockName(requestContext, "resourcelock");
      this.m_instanceId = requestContext.ServiceHost.InstanceId;
      requestContext.GetService<TeamFoundationSqlNotificationService>().RegisterNotification(requestContext, "Default", SqlNotificationEventClasses.CatalogResourceChanged, new SqlNotificationCallback(this.OnCatalogResourceTypesChanged), false);
      this.CatalogValidator = new CatalogValidator();
      this.CatalogValidator.Initialize(requestContext);
    }

    void IVssFrameworkService.ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    void IDisposable.Dispose()
    {
      if (this.CatalogValidator == null)
        return;
      this.CatalogValidator.Dispose();
      this.CatalogValidator = (CatalogValidator) null;
    }

    public List<CatalogResourceType> QueryResourceTypes(
      IVssRequestContext requestContext,
      IEnumerable<Guid> resourceTypeIdentifiers)
    {
      this.ValidateRequestContext(requestContext);
      this.EnsureResourceTypesLoaded(requestContext);
      List<CatalogResourceType> catalogResourceTypeList = new List<CatalogResourceType>();
      if (resourceTypeIdentifiers != null)
      {
        bool flag = true;
        foreach (Guid resourceTypeIdentifier in resourceTypeIdentifiers)
        {
          CatalogResourceType catalogResourceType;
          if (!this.m_resourceTypes.TryGetValue(resourceTypeIdentifier, out catalogResourceType))
          {
            requestContext.Trace(55001, TraceLevel.Warning, TeamFoundationCatalogService.s_area, TeamFoundationCatalogService.s_layer, "Throwing CatalogResourceTypeDoesNotExistException for resourceTypeIdentifier {0}", (object) resourceTypeIdentifier);
            throw new CatalogResourceTypeDoesNotExistException(resourceTypeIdentifier);
          }
          catalogResourceTypeList.Add(catalogResourceType);
          flag = false;
        }
        if (!flag)
          return catalogResourceTypeList;
      }
      foreach (CatalogResourceType catalogResourceType in this.m_resourceTypes.Values)
        catalogResourceTypeList.Add(catalogResourceType);
      return catalogResourceTypeList;
    }

    public CatalogResourceType QueryResourceType(
      IVssRequestContext requestContext,
      Guid resourceTypeIdentifier)
    {
      this.ValidateRequestContext(requestContext);
      this.EnsureResourceTypesLoaded(requestContext);
      CatalogResourceType catalogResourceType;
      if (!this.m_resourceTypes.TryGetValue(resourceTypeIdentifier, out catalogResourceType))
      {
        requestContext.Trace(55002, TraceLevel.Warning, TeamFoundationCatalogService.s_area, TeamFoundationCatalogService.s_layer, "Throwing CatalogResourceTypeDoesNotExistException for resourceTypeIdentifier {0}", (object) resourceTypeIdentifier);
        throw new CatalogResourceTypeDoesNotExistException(resourceTypeIdentifier);
      }
      return catalogResourceType;
    }

    public List<CatalogNode> QueryRootNodes(IVssRequestContext requestContext)
    {
      this.ValidateRequestContext(requestContext);
      return this.QueryNodes(requestContext, (IEnumerable<string>) new string[1]
      {
        CatalogConstants.SingleRecurseStar
      }, (IEnumerable<Guid>) null, CatalogQueryOptions.None);
    }

    public CatalogNode QueryRootNode(IVssRequestContext requestContext, CatalogTree tree)
    {
      this.ValidateRequestContext(requestContext);
      IEnumerable<CatalogNode> source = (IEnumerable<CatalogNode>) this.QueryNodes(requestContext, (IEnumerable<string>) new string[1]
      {
        CatalogRoots.DeterminePath(tree)
      }, (IEnumerable<Guid>) null, CatalogQueryOptions.None);
      if (source.Count<CatalogNode>() != 1)
      {
        requestContext.Trace(55003, TraceLevel.Warning, TeamFoundationCatalogService.s_area, TeamFoundationCatalogService.s_layer, "Found {0} node(s) for {1}", (object) source.Count<CatalogNode>(), (object) tree);
        throw new CatalogNodeDoesNotExistException();
      }
      return source.First<CatalogNode>();
    }

    public List<CatalogResource> QueryResources(
      IVssRequestContext requestContext,
      IEnumerable<Guid> resourceIdentifiers,
      CatalogQueryOptions queryOptions)
    {
      this.ValidateRequestContext(requestContext);
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) resourceIdentifiers, nameof (resourceIdentifiers));
      List<CatalogResource> databaseResources;
      List<CatalogServiceReference> databaseServiceReferences;
      List<CatalogNode> databaseNodes;
      List<CatalogNodeDependency> databaseDependencies;
      using (CatalogComponent component = requestContext.CreateComponent<CatalogComponent>())
      {
        ResultCollection rc = component.QueryCatalogResources(resourceIdentifiers, queryOptions);
        this.GetResources(requestContext, rc, queryOptions, out databaseResources, out databaseServiceReferences, out databaseNodes, out databaseDependencies);
      }
      List<CatalogResource> resources = this.BuildResources(requestContext, queryOptions, databaseResources, databaseServiceReferences, databaseNodes, databaseDependencies);
      return this.GetReadableResources(requestContext, resources);
    }

    public List<CatalogResource> QueryResourcesByType(
      IVssRequestContext requestContext,
      IEnumerable<Guid> resourceTypes,
      CatalogQueryOptions queryOptions)
    {
      this.ValidateRequestContext(requestContext);
      List<CatalogResource> resources = this.QueryResourcesByTypeAndArtifactIdInternal(requestContext, resourceTypes, (IEnumerable<int>) null, queryOptions);
      return this.GetReadableResources(requestContext, resources);
    }

    public List<CatalogResource> QueryResources(
      IVssRequestContext requestContext,
      IEnumerable<Guid> resourceTypes,
      IEnumerable<KeyValuePair<string, string>> propertyFilters,
      CatalogQueryOptions queryOptions)
    {
      this.ValidateRequestContext(requestContext);
      List<CatalogResource> resources1 = this.QueryResourcesByTypeAndArtifactIdInternal(requestContext, resourceTypes, (IEnumerable<int>) null, queryOptions);
      List<CatalogResource> resources2 = this.FilterCatalogResources(requestContext, resources1, propertyFilters);
      return this.GetReadableResources(requestContext, resources2);
    }

    private List<CatalogResource> FilterCatalogResources(
      IVssRequestContext requestContext,
      List<CatalogResource> resources,
      IEnumerable<KeyValuePair<string, string>> propertyFilters)
    {
      if (propertyFilters == null || resources == null || resources.Count == 0)
        return resources;
      List<CatalogResource> catalogResourceList = new List<CatalogResource>(resources.Count);
      foreach (CatalogResource resource in resources)
      {
        bool flag = true;
        foreach (KeyValuePair<string, string> propertyFilter in propertyFilters)
        {
          string x;
          if (!resource.Properties.TryGetValue(propertyFilter.Key, out x) || propertyFilter.Value != null && !StringComparer.OrdinalIgnoreCase.Equals(x, propertyFilter.Value))
          {
            flag = false;
            break;
          }
        }
        if (flag)
          catalogResourceList.Add(resource);
      }
      return catalogResourceList;
    }

    public List<CatalogNode> QueryNodes(
      IVssRequestContext requestContext,
      string pathSpec,
      Guid resourceTypeFilter)
    {
      return this.QueryNodes(requestContext, pathSpec, resourceTypeFilter, (IEnumerable<KeyValuePair<string, string>>) null);
    }

    public List<CatalogNode> QueryNodes(
      IVssRequestContext requestContext,
      string pathSpec,
      Guid resourceTypeFilter,
      IEnumerable<KeyValuePair<string, string>> propertyFilters)
    {
      return this.QueryNodes(requestContext, pathSpec, resourceTypeFilter, propertyFilters, CatalogQueryOptions.None);
    }

    public List<CatalogNode> QueryNodes(
      IVssRequestContext requestContext,
      string pathSpec,
      Guid resourceTypeFilter,
      IEnumerable<KeyValuePair<string, string>> propertyFilters,
      CatalogQueryOptions queryOptions)
    {
      return this.QueryNodes(requestContext, (IEnumerable<string>) new string[1]
      {
        pathSpec
      }, (IEnumerable<Guid>) new Guid[1]
      {
        resourceTypeFilter
      }, propertyFilters, queryOptions);
    }

    public List<CatalogNode> QueryNodes(
      IVssRequestContext requestContext,
      IEnumerable<string> pathSpecs,
      IEnumerable<Guid> resourceTypeFilters,
      CatalogQueryOptions queryOptions)
    {
      this.ValidateRequestContext(requestContext);
      List<CatalogNode> nodes = this.QueryNodesInternal(requestContext, pathSpecs, resourceTypeFilters, (IEnumerable<int>) null, queryOptions);
      return this.GetReadableNodes(requestContext, nodes);
    }

    public List<CatalogNode> QueryNodes(
      IVssRequestContext requestContext,
      IEnumerable<string> pathSpecs,
      IEnumerable<Guid> resourceTypeFilters,
      IEnumerable<KeyValuePair<string, string>> propertyFilters,
      CatalogQueryOptions queryOptions)
    {
      this.ValidateRequestContext(requestContext);
      List<CatalogNode> nodes = TeamFoundationCatalogService.FilterCatalogNodes(this.QueryNodesInternal(requestContext, pathSpecs, resourceTypeFilters, (IEnumerable<int>) null, queryOptions), propertyFilters);
      return this.GetReadableNodes(requestContext, nodes);
    }

    internal static List<CatalogNode> FilterCatalogNodes(
      List<CatalogNode> nodes,
      IEnumerable<KeyValuePair<string, string>> propertyFilters)
    {
      if (propertyFilters == null || nodes == null || nodes.Count == 0)
        return nodes;
      List<CatalogNode> catalogNodeList = new List<CatalogNode>(nodes.Count);
      foreach (CatalogNode node in nodes)
      {
        bool flag = true;
        foreach (KeyValuePair<string, string> propertyFilter in propertyFilters)
        {
          string x;
          if (!node.Resource.Properties.TryGetValue(propertyFilter.Key, out x) || propertyFilter.Value != null && !StringComparer.OrdinalIgnoreCase.Equals(x, propertyFilter.Value))
          {
            flag = false;
            break;
          }
        }
        if (flag)
          catalogNodeList.Add(node);
      }
      return catalogNodeList;
    }

    public List<CatalogNode> QueryCatalogDependents(
      IVssRequestContext requestContext,
      string path,
      CatalogQueryOptions queryOptions)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(path, nameof (path));
      if (!this.HasNodeReadPermission(requestContext, path))
      {
        requestContext.Trace(55004, TraceLevel.Warning, TeamFoundationCatalogService.s_area, TeamFoundationCatalogService.s_layer, "Throwing CatalogNodeDoesNotExistException");
        throw new CatalogNodeDoesNotExistException();
      }
      this.ValidateRequestContext(requestContext);
      CatalogPathSpec.ValidatePathSpec(path, false, false);
      List<CatalogResource> databaseResources;
      List<CatalogServiceReference> databaseServiceReferences;
      List<CatalogNode> databaseNodes;
      List<CatalogNodeDependency> databaseDependencies;
      using (CatalogComponent component = requestContext.CreateComponent<CatalogComponent>())
      {
        ResultCollection rc = component.QueryCatalogDependents(path, queryOptions);
        this.GetResources(requestContext, rc, queryOptions, out databaseResources, out databaseServiceReferences, out databaseNodes, out databaseDependencies);
      }
      return this.BuildNodes(requestContext, queryOptions, databaseResources, databaseServiceReferences, databaseNodes, databaseDependencies);
    }

    public List<CatalogNode> QueryParents(
      IVssRequestContext requestContext,
      Guid resourceIdentifier,
      IEnumerable<string> pathFilters,
      IEnumerable<Guid> resourceTypeFilters,
      bool recurseToRoot,
      CatalogQueryOptions queryOptions)
    {
      this.ValidateRequestContext(requestContext);
      ArgumentUtility.CheckForEmptyGuid(resourceIdentifier, nameof (resourceIdentifier));
      if (pathFilters != null)
      {
        foreach (string pathFilter in pathFilters)
          CatalogPathSpec.ValidatePathSpec(pathFilter, false, false);
      }
      List<CatalogResource> databaseResources;
      List<CatalogServiceReference> databaseServiceReferences;
      List<CatalogNode> databaseNodes;
      List<CatalogNodeDependency> databaseDependencies;
      using (CatalogComponent component = requestContext.CreateComponent<CatalogComponent>())
      {
        ResultCollection rc = component.QueryCatalogParents(resourceIdentifier, pathFilters, resourceTypeFilters, recurseToRoot, queryOptions);
        this.GetResources(requestContext, rc, queryOptions, out databaseResources, out databaseServiceReferences, out databaseNodes, out databaseDependencies);
      }
      List<CatalogNode> nodes = this.BuildNodes(requestContext, queryOptions, databaseResources, databaseServiceReferences, databaseNodes, databaseDependencies);
      return this.GetReadableNodes(requestContext, nodes);
    }

    public CatalogTransactionContext CreateTransactionContext() => new CatalogTransactionContext(this.m_runRulesOnSave);

    public List<CatalogResource> SaveTransactionContextChanges(
      IVssRequestContext requestContext,
      CatalogTransactionContext context,
      CatalogQueryOptions queryOptions,
      bool preview,
      out List<CatalogResource> deletedResources,
      out List<CatalogNode> deletedNodes)
    {
      IVssSecurityNamespace securityNamespace = requestContext.GetService<TeamFoundationSecurityService>().GetSecurityNamespace(requestContext, FrameworkSecurity.CatalogNamespaceId);
      Dictionary<Guid, CatalogResource> dictionary = new Dictionary<Guid, CatalogResource>();
      List<CatalogResource> catalogResourceList1 = new List<CatalogResource>();
      foreach (CatalogResource resourceChange in context.ResourceChanges)
      {
        if (resourceChange.Identifier != Guid.Empty)
          dictionary[resourceChange.Identifier] = (CatalogResource) null;
        if (resourceChange.ResourceType.Identifier == CatalogResourceTypes.TeamProject)
          catalogResourceList1.Add(resourceChange);
      }
      if (dictionary.Count > 0)
      {
        foreach (CatalogResource queryResource in this.QueryResources(requestContext, (IEnumerable<Guid>) dictionary.Keys, CatalogQueryOptions.None))
          dictionary[queryResource.Identifier] = queryResource;
      }
      foreach (CatalogResource catalogResource1 in catalogResourceList1)
      {
        CatalogResource catalogResource2;
        if (catalogResource1.Identifier != Guid.Empty && dictionary.TryGetValue(catalogResource1.Identifier, out catalogResource2) && !string.Equals(string.IsNullOrEmpty(catalogResource2.Description) ? (string) null : catalogResource2.Description, string.IsNullOrEmpty(catalogResource1.Description) ? (string) null : catalogResource1.Description, StringComparison.Ordinal))
        {
          requestContext.Trace(55029, TraceLevel.Warning, TeamFoundationCatalogService.s_area, TeamFoundationCatalogService.s_layer, "Attempt to update a project catalog node with a changed description, which is not allowed.  Descriptions must be set via ProjectService.");
          throw new InvalidCatalogSaveNodeException(Resources.CatalogProjectDescriptionChange());
        }
      }
      foreach (CatalogNode nodeChange in context.NodeChanges)
      {
        if (string.IsNullOrEmpty(nodeChange.FullPath))
        {
          securityNamespace.CheckPermission(requestContext, nodeChange.ParentPath, 2, false);
          if (nodeChange.Resource.Identifier != Guid.Empty)
          {
            CatalogResource catalogResource;
            if (!dictionary.TryGetValue(nodeChange.Resource.Identifier, out catalogResource))
            {
              requestContext.Trace(55005, TraceLevel.Warning, TeamFoundationCatalogService.s_area, TeamFoundationCatalogService.s_layer, "Throwing CatalogResourceDoesNotExistException {0}", (object) nodeChange.Resource.DisplayName);
              throw new CatalogResourceDoesNotExistException(nodeChange.Resource.DisplayName);
            }
            bool flag1 = false;
            bool flag2 = false;
            foreach (CatalogNode nodeReference in catalogResource.NodeReferences)
            {
              flag2 |= securityNamespace.HasPermission(requestContext, nodeReference.FullPath, 4, false);
              flag1 = this.HasNodeReadPermission(requestContext, nodeReference.FullPath);
            }
            if (!flag1 || !flag2)
            {
              if (!flag1)
              {
                requestContext.Trace(55006, TraceLevel.Warning, TeamFoundationCatalogService.s_area, TeamFoundationCatalogService.s_layer, "Throwing CatalogResourceDoesNotExistException {0}", (object) nodeChange.Resource.DisplayName);
                throw new CatalogResourceDoesNotExistException(nodeChange.Resource.DisplayName);
              }
              Microsoft.VisualStudio.Services.Identity.Identity readIdentity = requestContext.GetService<IdentityService>().ReadIdentities(requestContext, (IList<IdentityDescriptor>) new IdentityDescriptor[1]
              {
                requestContext.UserContext
              }, QueryMembership.None, (IEnumerable<string>) null)[0];
              if (readIdentity == null)
              {
                requestContext.Trace(55007, TraceLevel.Warning, TeamFoundationCatalogService.s_area, TeamFoundationCatalogService.s_layer, "Throwing IdentityNotFoundException {0}", (object) requestContext.UserContext);
                throw new IdentityNotFoundException(requestContext.UserContext);
              }
              Exception exception = (Exception) new AccessCheckException(readIdentity.Descriptor, readIdentity.DisplayName, nodeChange.FullPath, 4, FrameworkSecurity.CatalogNamespaceId, TFCommonResources.AccessCheckExceptionTokenFormat((object) requestContext.GetUserId().ToString(), (object) nodeChange.Resource.DisplayName, (object) securityNamespace.Description.GetLocalizedActions(4).FirstOrDefault<string>()));
              requestContext.Trace(55008, TraceLevel.Warning, TeamFoundationCatalogService.s_area, TeamFoundationCatalogService.s_layer, "Throwing AccessCheckException {0}", (object) exception);
              throw exception;
            }
          }
          this.CheckNodeDependencyPermissions(requestContext, nodeChange, true);
        }
        else
        {
          securityNamespace.CheckPermission(requestContext, nodeChange.FullPath, 4, false);
          this.CheckNodeDependencyPermissions(requestContext, nodeChange, false);
        }
      }
      foreach (CatalogResource catalogResource in dictionary.Values)
      {
        if (catalogResource != null)
        {
          bool flag3 = false;
          bool flag4 = false;
          string token = string.Empty;
          foreach (CatalogNode nodeReference in catalogResource.NodeReferences)
          {
            flag3 |= securityNamespace.HasPermission(requestContext, nodeReference.FullPath, 4, false);
            flag4 |= this.HasNodeReadPermission(requestContext, nodeReference.FullPath);
            token = nodeReference.FullPath;
          }
          if (!flag3)
          {
            if (!flag4)
            {
              requestContext.Trace(55009, TraceLevel.Warning, TeamFoundationCatalogService.s_area, TeamFoundationCatalogService.s_layer, "Throwing CatalogResourceDoesNotExistException {0}", (object) catalogResource.DisplayName);
              throw new CatalogResourceDoesNotExistException(catalogResource.DisplayName);
            }
            Microsoft.VisualStudio.Services.Identity.Identity readIdentity = requestContext.GetService<IdentityService>().ReadIdentities(requestContext, (IList<IdentityDescriptor>) new IdentityDescriptor[1]
            {
              requestContext.UserContext
            }, QueryMembership.None, (IEnumerable<string>) null)[0];
            if (readIdentity == null)
            {
              requestContext.Trace(55010, TraceLevel.Warning, TeamFoundationCatalogService.s_area, TeamFoundationCatalogService.s_layer, "Throwing IdentityNotFoundException {0}", (object) requestContext.UserContext);
              throw new IdentityNotFoundException(requestContext.UserContext);
            }
            AccessCheckException accessCheckException = new AccessCheckException(readIdentity.Descriptor, readIdentity.DisplayName, token, 4, FrameworkSecurity.CatalogNamespaceId, TFCommonResources.AccessCheckExceptionTokenFormat((object) requestContext.GetUserId().ToString(), (object) catalogResource.DisplayName, (object) securityNamespace.Description.GetLocalizedActions(4).FirstOrDefault<string>()));
            requestContext.Trace(55011, TraceLevel.Warning, TeamFoundationCatalogService.s_area, TeamFoundationCatalogService.s_layer, "Throwing AccessCheckException {0}", (object) accessCheckException);
            throw accessCheckException;
          }
        }
      }
      List<string> tokens = new List<string>();
      foreach (KeyValuePair<CatalogNode, bool> delete in context.Deletes)
      {
        securityNamespace.CheckPermission(requestContext, delete.Key.FullPath, 8, false);
        tokens.Add(delete.Key.FullPath);
      }
      List<Microsoft.TeamFoundation.Framework.Server.TokenRename> renameTokens = new List<Microsoft.TeamFoundation.Framework.Server.TokenRename>();
      foreach (KeyValuePair<CatalogNode, CatalogNode> move in context.Moves)
      {
        if (!string.IsNullOrEmpty(move.Key.FullPath))
          securityNamespace.CheckPermission(requestContext, move.Key.FullPath, 12, false);
        if (!string.IsNullOrEmpty(move.Value.FullPath))
          securityNamespace.CheckPermission(requestContext, move.Value.FullPath, 2, false);
        renameTokens.Add(new Microsoft.TeamFoundation.Framework.Server.TokenRename()
        {
          OldToken = move.Key.FullPath,
          NewToken = move.Value.FullPath,
          Copy = true,
          Recurse = true
        });
        tokens.Add(move.Key.FullPath);
      }
      securityNamespace.RenameTokens(requestContext, (IEnumerable<Microsoft.TeamFoundation.Framework.Server.TokenRename>) renameTokens);
      List<CatalogResource> catalogResourceList2 = context.Save(requestContext, queryOptions, preview, out deletedResources, out deletedNodes);
      securityNamespace.RemoveAccessControlLists(requestContext, (IEnumerable<string>) tokens, true);
      return catalogResourceList2;
    }

    internal CatalogValidator CatalogValidator { get; private set; }

    internal void GetResources(
      IVssRequestContext requestContext,
      ResultCollection rc,
      CatalogQueryOptions queryOptions,
      out List<CatalogResource> databaseResources,
      out List<CatalogServiceReference> databaseServiceReferences,
      out List<CatalogNode> databaseNodes,
      out List<CatalogNodeDependency> databaseDependencies)
    {
      this.ValidateRequestContext(requestContext);
      databaseResources = rc.GetCurrent<CatalogResource>().Items;
      rc.NextResult();
      databaseServiceReferences = rc.GetCurrent<CatalogServiceReference>().Items;
      rc.NextResult();
      databaseNodes = rc.GetCurrent<CatalogNode>().Items;
      databaseDependencies = (List<CatalogNodeDependency>) null;
      if ((queryOptions & CatalogQueryOptions.ExpandDependencies) != CatalogQueryOptions.ExpandDependencies)
        return;
      rc.NextResult();
      databaseDependencies = rc.GetCurrent<CatalogNodeDependency>().Items;
    }

    internal List<CatalogResource> BuildResources(
      IVssRequestContext requestContext,
      CatalogQueryOptions queryOptions,
      List<CatalogResource> databaseResources,
      List<CatalogServiceReference> databaseServiceReferences,
      List<CatalogNode> databaseNodes,
      List<CatalogNodeDependency> databaseDependencies)
    {
      this.ValidateRequestContext(requestContext);
      List<CatalogResource> matchingResources;
      this.BuildCatalogObjects(requestContext, databaseResources, databaseServiceReferences, databaseNodes, databaseDependencies, queryOptions, out List<CatalogNode> _, out matchingResources);
      return matchingResources;
    }

    internal void BuildCatalogObjects(
      IVssRequestContext requestContext,
      List<CatalogResource> databaseResources,
      List<CatalogServiceReference> databaseServiceReferences,
      List<CatalogNode> databaseNodes,
      List<CatalogNodeDependency> databaseDependencies,
      CatalogQueryOptions queryOptions,
      out List<CatalogNode> matchingNodes,
      out List<CatalogResource> matchingResources)
    {
      this.ValidateRequestContext(requestContext);
      this.EnsureResourceTypesLoaded(requestContext);
      IVssRequestContext vssRequestContext = requestContext;
      TeamFoundationPropertyService service1 = vssRequestContext.GetService<TeamFoundationPropertyService>();
      ILocationService service2 = requestContext.GetService<ILocationService>();
      matchingNodes = new List<CatalogNode>();
      matchingResources = new List<CatalogResource>();
      Dictionary<string, CatalogNode> dictionary1 = new Dictionary<string, CatalogNode>((IEqualityComparer<string>) VssStringComparer.CatalogNodePath);
      int index1 = 0;
      int index2 = 0;
      Dictionary<int, CatalogResource> dictionary2 = new Dictionary<int, CatalogResource>();
      List<ArtifactSpec> artifactSpecList = new List<ArtifactSpec>();
      foreach (CatalogResource databaseResource in databaseResources)
      {
        CatalogResourceType catalogResourceType;
        if (!this.m_resourceTypes.TryGetValue(databaseResource.ResourceTypeIdentifier, out catalogResourceType))
        {
          string str = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Resource {0} does not have a valid type, skipping...", (object) databaseResource.DisplayName);
          TeamFoundationTrace.Error(str);
          requestContext.Trace(55012, TraceLevel.Error, TeamFoundationCatalogService.s_area, TeamFoundationCatalogService.s_layer, str);
        }
        else
        {
          databaseResource.ResourceType = catalogResourceType;
          for (; index1 < databaseServiceReferences.Count && databaseServiceReferences[index1].ResourceIdentifier == databaseResource.Identifier; ++index1)
          {
            CatalogServiceReference serviceReference = databaseServiceReferences[index1];
            ServiceDefinition serviceDefinition = service2.FindServiceDefinition(requestContext, serviceReference.ServiceType, serviceReference.ServiceIdentifier);
            databaseResource.ServiceReferences[serviceReference.AssociationKey] = serviceDefinition;
          }
          while (index2 < databaseNodes.Count && databaseNodes[index2].ResourceIdentifier == databaseResource.Identifier)
          {
            CatalogNode databaseNode = databaseNodes[index2];
            databaseResource.NodeReferences.Add(databaseNode);
            databaseNode.Resource = databaseResource;
            databaseNode.CatalogService = (ITeamFoundationCatalogService) this;
            ++index2;
            dictionary1[databaseNode.FullPath] = databaseNode;
            if (databaseNode.MatchedQuery)
              matchingNodes.Add(databaseNode);
          }
          if (databaseResource.PropertyId != -1)
          {
            dictionary2[databaseResource.PropertyId] = databaseResource;
            artifactSpecList.Add(new ArtifactSpec(ArtifactKinds.Catalog, databaseResource.PropertyId, 0));
          }
          if (databaseResource.MatchedQuery)
            matchingResources.Add(databaseResource);
        }
      }
      if (artifactSpecList.Count > 0)
      {
        using (TeamFoundationDataReader properties = service1.GetProperties(vssRequestContext, (IEnumerable<ArtifactSpec>) artifactSpecList, (IEnumerable<string>) null))
        {
          foreach (ArtifactPropertyValue artifactPropertyValue in properties)
          {
            int key = (int) artifactPropertyValue.Spec.Id[0] << 24 | (int) artifactPropertyValue.Spec.Id[1] << 16 | (int) artifactPropertyValue.Spec.Id[2] << 8 | (int) artifactPropertyValue.Spec.Id[3];
            CatalogResource catalogResource = dictionary2[key];
            foreach (PropertyValue propertyValue in artifactPropertyValue.PropertyValues)
            {
              if (propertyValue.Value != null)
                catalogResource.Properties[propertyValue.PropertyName] = propertyValue.Value.ToString();
            }
          }
        }
      }
      if (databaseDependencies != null)
      {
        foreach (CatalogNodeDependency databaseDependency in databaseDependencies)
        {
          CatalogNode catalogNode1;
          if (!dictionary1.TryGetValue(databaseDependency.FullPath, out catalogNode1))
          {
            TeamFoundationEventLog.Default.Log(requestContext, FrameworkResources.InvalidCatalogNodeDependencyLoaded((object) databaseDependency.FullPath), TeamFoundationEventId.InvalidCatalogNodeDependencyLoaded, EventLogEntryType.Warning);
            requestContext.Trace(55013, TraceLevel.Error, TeamFoundationCatalogService.s_area, TeamFoundationCatalogService.s_layer, "Loaded an invalid catalog node dependency: {0}", (object) databaseDependency.FullPath);
          }
          else
          {
            CatalogNode catalogNode2;
            if (!dictionary1.TryGetValue(databaseDependency.RequiredNodeFullPath, out catalogNode2))
            {
              TeamFoundationEventLog.Default.Log(requestContext, FrameworkResources.InvalidCatalogNodeDependencyLoaded((object) databaseDependency.RequiredNodeFullPath), TeamFoundationEventId.InvalidCatalogNodeDependencyLoaded, EventLogEntryType.Warning);
              requestContext.Trace(55014, TraceLevel.Error, TeamFoundationCatalogService.s_area, TeamFoundationCatalogService.s_layer, "Loaded an invalid catalog node dependency: {0}", (object) databaseDependency.RequiredNodeFullPath);
            }
            else if (databaseDependency.IsSingleton)
            {
              catalogNode1.Dependencies.Singletons[databaseDependency.AssociationKey] = catalogNode2;
            }
            else
            {
              IList<CatalogNode> catalogNodeList;
              if (!catalogNode1.Dependencies.Sets.TryGetValue(databaseDependency.AssociationKey, out catalogNodeList))
              {
                catalogNodeList = (IList<CatalogNode>) new List<CatalogNode>();
                catalogNode1.Dependencies.Sets[databaseDependency.AssociationKey] = catalogNodeList;
              }
              catalogNodeList.Add(catalogNode2);
            }
          }
        }
      }
      if ((queryOptions & CatalogQueryOptions.IncludeParents) != CatalogQueryOptions.IncludeParents)
        return;
      foreach (CatalogNode catalogNode3 in dictionary1.Values)
      {
        if (!string.IsNullOrEmpty(catalogNode3.ParentPath))
        {
          CatalogNode catalogNode4;
          if (!dictionary1.TryGetValue(catalogNode3.ParentPath, out catalogNode4))
          {
            TeamFoundationEventLog.Default.Log(requestContext, FrameworkResources.CatalogMissingParentNode((object) catalogNode3.ParentPath), TeamFoundationEventId.MissingCatalogNodeParent, EventLogEntryType.Error);
            requestContext.Trace(55015, TraceLevel.Error, TeamFoundationCatalogService.s_area, TeamFoundationCatalogService.s_layer, FrameworkResources.CatalogMissingParentNode((object) catalogNode3.ParentPath));
          }
          else
            catalogNode3.ParentNode = catalogNode4;
        }
      }
    }

    internal List<CatalogNode> BuildNodes(
      IVssRequestContext requestContext,
      CatalogQueryOptions queryOptions,
      List<CatalogResource> databaseResources,
      List<CatalogServiceReference> databaseServiceReferences,
      List<CatalogNode> databaseNodes,
      List<CatalogNodeDependency> databaseDependencies)
    {
      this.ValidateRequestContext(requestContext);
      List<CatalogNode> matchingNodes;
      this.BuildCatalogObjects(requestContext, databaseResources, databaseServiceReferences, databaseNodes, databaseDependencies, queryOptions, out matchingNodes, out List<CatalogResource> _);
      return matchingNodes;
    }

    internal void SaveCatalogResourceTypes(
      IVssRequestContext requestContext,
      IEnumerable<CatalogResourceType> resourceTypes)
    {
      this.ValidateRequestContext(requestContext);
      foreach (CatalogResourceType resourceType in resourceTypes)
      {
        ArgumentUtility.CheckForEmptyGuid(resourceType.Identifier, "resourceType.Identifier");
        ArgumentUtility.CheckStringForNullOrEmpty(resourceType.DisplayName, "resourceType.DisplayName");
      }
      using (CatalogComponent component = requestContext.CreateComponent<CatalogComponent>())
        component.SaveCatalogResourceTypes(resourceTypes);
      this.OnCatalogResourceTypesChanged(requestContext, Guid.Empty, (string) null);
    }

    private List<CatalogResource> QueryResourcesByTypeAndArtifactIdInternal(
      IVssRequestContext requestContext,
      IEnumerable<Guid> resourceTypes,
      IEnumerable<int> artifactIds,
      CatalogQueryOptions queryOptions)
    {
      List<CatalogResource> databaseResources;
      List<CatalogServiceReference> databaseServiceReferences;
      List<CatalogNode> databaseNodes;
      List<CatalogNodeDependency> databaseDependencies;
      using (CatalogComponent component = requestContext.CreateComponent<CatalogComponent>())
      {
        ResultCollection rc = component.QueryCatalogResourcesByTypeAndArtifactId(resourceTypes, artifactIds, queryOptions);
        this.GetResources(requestContext, rc, queryOptions, out databaseResources, out databaseServiceReferences, out databaseNodes, out databaseDependencies);
      }
      return this.BuildResources(requestContext, queryOptions, databaseResources, databaseServiceReferences, databaseNodes, databaseDependencies);
    }

    private void OnCatalogResourceTypesChanged(
      IVssRequestContext requestContext,
      Guid eventClass,
      string eventData)
    {
      using (requestContext.Lock(this.m_lock))
      {
        this.m_resourceTypesLoaded = false;
        ++this.m_cacheVersion;
      }
    }

    private List<CatalogNode> QueryNodesInternal(
      IVssRequestContext requestContext,
      IEnumerable<string> pathSpecs,
      IEnumerable<Guid> resourceTypeFilters,
      IEnumerable<int> artifactIdFilters,
      CatalogQueryOptions queryOptions)
    {
      if (pathSpecs != null)
      {
        foreach (string pathSpec in pathSpecs)
          CatalogPathSpec.ValidatePathSpec(pathSpec, true, false);
      }
      List<CatalogResource> databaseResources;
      List<CatalogServiceReference> databaseServiceReferences;
      List<CatalogNode> databaseNodes;
      List<CatalogNodeDependency> databaseDependencies;
      using (CatalogComponent component = requestContext.CreateComponent<CatalogComponent>())
      {
        ResultCollection rc = component.QueryCatalogNodes(pathSpecs, resourceTypeFilters, artifactIdFilters, queryOptions);
        this.GetResources(requestContext, rc, queryOptions, out databaseResources, out databaseServiceReferences, out databaseNodes, out databaseDependencies);
      }
      return this.BuildNodes(requestContext, queryOptions, databaseResources, databaseServiceReferences, databaseNodes, databaseDependencies);
    }

    private void EnsureResourceTypesLoaded(IVssRequestContext requestContext)
    {
      if (this.m_resourceTypesLoaded)
        return;
      int cacheVersion = this.m_cacheVersion;
      List<CatalogResourceType> catalogResourceTypeList = (List<CatalogResourceType>) null;
      using (CatalogComponent component = requestContext.Elevate().CreateComponent<CatalogComponent>())
        catalogResourceTypeList = component.QueryCatalogResourceTypes((IEnumerable<Guid>) null).GetCurrent<CatalogResourceType>().Items;
      Dictionary<Guid, CatalogResourceType> dictionary = new Dictionary<Guid, CatalogResourceType>();
      foreach (CatalogResourceType catalogResourceType in catalogResourceTypeList)
        dictionary[catalogResourceType.Identifier] = catalogResourceType;
      using (requestContext.Lock(this.m_lock))
      {
        if (this.m_resourceTypesLoaded)
          return;
        this.m_resourceTypes = dictionary;
        if (cacheVersion != this.m_cacheVersion)
          return;
        this.m_resourceTypesLoaded = true;
      }
    }

    private void ValidateRequestContext(IVssRequestContext requestContext)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      if (this.m_instanceId != requestContext.ServiceHost.InstanceId)
      {
        requestContext.Trace(55016, TraceLevel.Error, TeamFoundationCatalogService.s_area, TeamFoundationCatalogService.s_layer, "Throwing InvalidRequestContextHostException {0}", (object) FrameworkResources.CatalogServiceRequestContextHostMessage((object) this.m_instanceId, (object) requestContext.ServiceHost.InstanceId));
        throw new InvalidRequestContextHostException(FrameworkResources.CatalogServiceRequestContextHostMessage((object) this.m_instanceId, (object) requestContext.ServiceHost.InstanceId));
      }
    }

    private void CheckNodeDependencyPermissions(
      IVssRequestContext requestContext,
      CatalogNode node,
      bool isCreate)
    {
      if (node.Dependencies == null || node.Dependencies.Singletons.Count == 0 && node.Dependencies.Sets.Count == 0)
        return;
      List<CatalogNode> unreadableDependencies = this.GetUnreadableDependencies(requestContext, node);
      if (unreadableDependencies.Count != 0)
      {
        if (!isCreate)
        {
          bool flag1 = false;
          CatalogNode catalogNode1 = this.QueryNodes(requestContext, (IEnumerable<string>) new string[1]
          {
            node.FullPath
          }, (IEnumerable<Guid>) null, CatalogQueryOptions.ExpandDependencies).FirstOrDefault<CatalogNode>();
          if (catalogNode1 != null)
          {
            List<CatalogNode> catalogNodeList = new List<CatalogNode>();
            catalogNodeList.AddRange((IEnumerable<CatalogNode>) catalogNode1.Dependencies.Singletons.Values);
            foreach (KeyValuePair<string, IList<CatalogNode>> set in (IEnumerable<KeyValuePair<string, IList<CatalogNode>>>) catalogNode1.Dependencies.Sets)
              catalogNodeList.AddRange((IEnumerable<CatalogNode>) set.Value);
            foreach (CatalogNode catalogNode2 in unreadableDependencies)
            {
              bool flag2 = false;
              foreach (CatalogNode catalogNode3 in catalogNodeList)
              {
                if (VssStringComparer.CatalogNodePath.Equals(catalogNode2.FullPath, catalogNode3.FullPath))
                {
                  flag2 = true;
                  break;
                }
              }
              if (!flag2)
              {
                flag1 = true;
                break;
              }
            }
            if (!flag1)
              return;
          }
        }
        requestContext.Trace(55017, TraceLevel.Error, TeamFoundationCatalogService.s_area, TeamFoundationCatalogService.s_layer, "Throwing CatalogNodeDoesNotExistException {0}", (object) node);
        throw new CatalogNodeDoesNotExistException();
      }
    }

    private List<CatalogNode> GetUnreadableDependencies(
      IVssRequestContext requestContext,
      CatalogNode node)
    {
      List<CatalogNode> catalogNodeList = new List<CatalogNode>();
      catalogNodeList.AddRange((IEnumerable<CatalogNode>) node.Dependencies.Singletons.Values);
      foreach (KeyValuePair<string, IList<CatalogNode>> set in (IEnumerable<KeyValuePair<string, IList<CatalogNode>>>) node.Dependencies.Sets)
        catalogNodeList.AddRange((IEnumerable<CatalogNode>) set.Value);
      List<CatalogNode> unreadableDependencies = new List<CatalogNode>();
      foreach (CatalogNode catalogNode in catalogNodeList)
      {
        if (!string.IsNullOrEmpty(catalogNode.FullPath) && !this.HasNodeReadPermission(requestContext, catalogNode.FullPath))
          unreadableDependencies.Add(catalogNode);
      }
      return unreadableDependencies;
    }

    private List<CatalogResource> GetReadableResources(
      IVssRequestContext requestContext,
      List<CatalogResource> resources)
    {
      List<CatalogResource> readableResources = new List<CatalogResource>();
      foreach (CatalogResource resource in resources)
      {
        List<CatalogNode> catalogNodeList = new List<CatalogNode>();
        foreach (CatalogNode nodeReference in resource.NodeReferences)
        {
          if (this.HasNodeReadPermission(requestContext, nodeReference.FullPath))
            catalogNodeList.Add(nodeReference);
          else
            requestContext.Trace(54025, TraceLevel.Info, TeamFoundationCatalogService.s_area, TeamFoundationCatalogService.s_layer, "GetReadableResources: filtering nodeReference ({0})", (object) nodeReference.FullPath);
        }
        if (catalogNodeList.Count > 0)
        {
          resource.NodeReferences = catalogNodeList;
          readableResources.Add(resource);
        }
        else
          requestContext.Trace(54026, TraceLevel.Info, TeamFoundationCatalogService.s_area, TeamFoundationCatalogService.s_layer, "GetReadableResources: filtering resource ({0})", (object) resource.Identifier);
      }
      requestContext.Trace(55028, TraceLevel.Info, TeamFoundationCatalogService.s_area, TeamFoundationCatalogService.s_layer, "GetReadableResources: Filtered out {0}/{1} resources.", (object) (resources.Count - readableResources.Count), (object) resources.Count);
      return readableResources;
    }

    private List<CatalogNode> GetReadableNodes(
      IVssRequestContext requestContext,
      List<CatalogNode> nodes)
    {
      List<CatalogNode> readableNodes = new List<CatalogNode>();
      foreach (CatalogNode node in nodes)
      {
        if (this.HasNodeReadPermission(requestContext, node.FullPath))
        {
          readableNodes.Add(node);
          List<CatalogNode> catalogNodeList = new List<CatalogNode>();
          foreach (CatalogNode nodeReference in node.Resource.NodeReferences)
          {
            if (node == nodeReference || this.HasNodeReadPermission(requestContext, nodeReference.FullPath))
              catalogNodeList.Add(nodeReference);
            else
              requestContext.Trace(54021, TraceLevel.Info, TeamFoundationCatalogService.s_area, TeamFoundationCatalogService.s_layer, "GetReadableNodes: filtering nodeReference ({0})", (object) nodeReference.FullPath);
          }
          node.Resource.NodeReferences = catalogNodeList;
        }
        else
          requestContext.Trace(54022, TraceLevel.Info, TeamFoundationCatalogService.s_area, TeamFoundationCatalogService.s_layer, "GetReadableNodes: filtering node ({0})", (object) node.FullPath);
      }
      if (nodes.Count == readableNodes.Count)
        requestContext.Trace(55023, TraceLevel.Verbose, TeamFoundationCatalogService.s_area, TeamFoundationCatalogService.s_layer, "GetReadableNodes: Filtered out 0/{0} nodes.", (object) nodes.Count);
      else
        requestContext.Trace(55024, TraceLevel.Info, TeamFoundationCatalogService.s_area, TeamFoundationCatalogService.s_layer, "GetReadableNodes: Filtered out 0/{0} nodes.", (object) (nodes.Count - readableNodes.Count), (object) nodes.Count);
      return readableNodes;
    }

    private bool HasNodeReadPermission(IVssRequestContext requestContext, string nodeFullPath)
    {
      if (CatalogRoots.DetermineTree(nodeFullPath) == CatalogTree.Infrastructure && (requestContext.IsUserContext || requestContext.RootContext.IsUserContext) && requestContext.ExecutionEnvironment.IsHostedDeployment)
        return false;
      IVssSecurityNamespace securityNamespace = requestContext.GetService<TeamFoundationSecurityService>().GetSecurityNamespace(requestContext, FrameworkSecurity.CatalogNamespaceId);
      return requestContext.IsSystemContext || securityNamespace.HasPermission(requestContext, nodeFullPath, 1) || securityNamespace.HasPermissionForAnyChildren(requestContext, nodeFullPath, 1);
    }

    internal static int GetCatalogPermissionsFromFrameworkPermissions(int frameworkPermissions)
    {
      int frameworkPermissions1 = 0;
      if ((frameworkPermissions & 2) != 0)
        frameworkPermissions1 |= 14;
      if ((frameworkPermissions & 1) != 0)
        frameworkPermissions1 |= 1;
      return frameworkPermissions1;
    }
  }
}
