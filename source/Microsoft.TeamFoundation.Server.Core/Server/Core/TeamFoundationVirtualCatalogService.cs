// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.TeamFoundationVirtualCatalogService
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Location;
using Microsoft.VisualStudio.Services.Location.Server;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.Core
{
  internal class TeamFoundationVirtualCatalogService : 
    ITeamFoundationCatalogService,
    IVssFrameworkService
  {
    private static readonly string[] s_requiredProjectProperties = new string[3]
    {
      "System.SourceControlCapabilityFlags",
      "System.SourceControlTfvcEnabled",
      "System.SourceControlGitEnabled"
    };
    private static readonly Dictionary<string, string> s_projectPropertiesToCatalogProperties = new Dictionary<string, string>()
    {
      {
        "System.SourceControlCapabilityFlags",
        "SourceControlCapabilityFlags"
      },
      {
        "System.SourceControlTfvcEnabled",
        "SourceControlTfvcEnabled"
      },
      {
        "System.SourceControlGitEnabled",
        "SourceControlGitEnabled"
      }
    };
    private static readonly HashSet<Guid> s_filterableResourceTypes = new HashSet<Guid>()
    {
      CatalogResourceTypes.ProjectCollection,
      CatalogResourceTypes.TeamProject
    };
    private const string c_area = "Catalog";
    private const string c_layer = "VirtualService";
    private static readonly int s_lengthOfBase64OfGuidByteArray = 24;
    private static readonly Dictionary<Guid, CatalogResourceType> s_catalogResourceTypes = TeamFoundationVirtualCatalogService.BuildCatalogResourceTypes();

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
      if (systemRequestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection))
        throw new UnexpectedHostTypeException(systemRequestContext.ServiceHost.HostType);
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public List<CatalogNode> QueryCatalogDependents(
      IVssRequestContext requestContext,
      string path,
      CatalogQueryOptions queryOptions)
    {
      return this.QueryNodes(requestContext, (IEnumerable<string>) new string[1]
      {
        path
      }, (IEnumerable<Guid>) new Guid[0], (IEnumerable<KeyValuePair<string, string>>) new KeyValuePair<string, string>[0], queryOptions);
    }

    public List<CatalogNode> QueryNodes(
      IVssRequestContext requestContext,
      string pathSpec,
      Guid resourceTypeFilter)
    {
      return this.QueryNodes(requestContext, pathSpec, resourceTypeFilter, (IEnumerable<KeyValuePair<string, string>>) new KeyValuePair<string, string>[0]);
    }

    public List<CatalogNode> QueryNodes(
      IVssRequestContext requestContext,
      IEnumerable<string> pathSpecs,
      IEnumerable<Guid> resourceTypeFilters,
      CatalogQueryOptions queryOptions)
    {
      return this.QueryNodes(requestContext, pathSpecs, resourceTypeFilters, (IEnumerable<KeyValuePair<string, string>>) new KeyValuePair<string, string>[0], queryOptions);
    }

    public List<CatalogNode> QueryNodes(
      IVssRequestContext requestContext,
      string pathSpec,
      Guid resourceTypeFilter,
      IEnumerable<KeyValuePair<string, string>> propertyFilters)
    {
      return this.QueryNodes(requestContext, (IEnumerable<string>) new string[1]
      {
        pathSpec
      }, (IEnumerable<Guid>) new Guid[1]
      {
        resourceTypeFilter
      }, propertyFilters, CatalogQueryOptions.None);
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
      IEnumerable<KeyValuePair<string, string>> propertyFilters,
      CatalogQueryOptions queryOptions)
    {
      requestContext.TraceEnter(47000570, "Catalog", "VirtualService", nameof (QueryNodes));
      try
      {
        List<CatalogNode> catalogNodeList = new List<CatalogNode>();
        bool includeParents = queryOptions.HasFlag((Enum) CatalogQueryOptions.IncludeParents);
        TeamFoundationVirtualCatalogService.CatalogSearchFilter catalogSearchFilter = new TeamFoundationVirtualCatalogService.CatalogSearchFilter(pathSpecs, resourceTypeFilters ?? (IEnumerable<Guid>) TeamFoundationVirtualCatalogService.s_catalogResourceTypes.Keys);
        if (catalogSearchFilter.IsMatch(4, CatalogResourceTypes.TeamProject, false))
        {
          foreach (HostProperties collectionHostProperty in TeamFoundationVirtualCatalogService.GetScopedCollectionHostProperties(requestContext))
          {
            bool dispose = false;
            IVssRequestContext vssRequestContext = (IVssRequestContext) null;
            try
            {
              vssRequestContext = TeamFoundationVirtualCatalogService.BeginOrGetCollectionRequestContext(requestContext, collectionHostProperty.Id, out dispose);
              IEnumerable<ProjectInfo> projectInfos = vssRequestContext.GetService<IProjectService>().GetProjects(vssRequestContext).PopulateProperties(vssRequestContext, TeamFoundationVirtualCatalogService.s_requiredProjectProperties);
              string parentPath = this.GetServerInstanceNode(requestContext).FullPath + Convert.ToBase64String(collectionHostProperty.Id.ToByteArray());
              foreach (ProjectInfo project in projectInfos)
                catalogNodeList.Add(this.CreateProjectCatalogNode(project, parentPath));
            }
            finally
            {
              if (dispose && vssRequestContext != null)
                vssRequestContext.Dispose();
            }
          }
        }
        IEnumerable<CatalogNode> childNodes1 = this.GetChildNodes(catalogNodeList, 3, includeParents);
        if (catalogSearchFilter.IsMatch(3, CatalogResourceTypes.ProjectCollection, childNodes1 != null && childNodes1.Count<CatalogNode>() > 0))
          catalogNodeList.AddRange(this.GetScopedCollectionCatalogNodes(requestContext, true, childNodes1));
        IEnumerable<CatalogNode> childNodes2 = this.GetChildNodes(catalogNodeList, 2, includeParents);
        if (catalogSearchFilter.IsMatch(2, CatalogResourceTypes.TeamFoundationServerInstance, childNodes2 != null && childNodes2.Count<CatalogNode>() > 0))
        {
          CatalogNode serverInstanceNode = this.GetServerInstanceNode(requestContext);
          catalogNodeList.Add(serverInstanceNode);
          this.ParentCatalogNodes(serverInstanceNode, childNodes2);
        }
        IEnumerable<CatalogNode> childNodes3 = this.GetChildNodes(catalogNodeList, 1, includeParents);
        if (catalogSearchFilter.IsMatch(1, CatalogResourceTypes.OrganizationalRoot, childNodes3 != null && childNodes3.Count<CatalogNode>() > 0))
        {
          CatalogNode organizationalRootNode = this.GetOrganizationalRootNode(requestContext);
          catalogNodeList.Add(organizationalRootNode);
          this.ParentCatalogNodes(organizationalRootNode, childNodes3);
        }
        return TeamFoundationCatalogService.FilterCatalogNodes(catalogNodeList, propertyFilters);
      }
      finally
      {
        requestContext.TraceLeave(153237272, "Catalog", "VirtualService", nameof (QueryNodes));
      }
    }

    internal static IVssRequestContext BeginOrGetCollectionRequestContext(
      IVssRequestContext requestContext,
      Guid collectionHostId,
      out bool dispose)
    {
      if (requestContext.RootContext.ServiceHost.InstanceId == collectionHostId)
      {
        dispose = false;
        return requestContext.RootContext;
      }
      dispose = true;
      ITeamFoundationHostManagementService service = requestContext.GetService<ITeamFoundationHostManagementService>();
      RequestContextType requestContextType = RequestContextType.UserContext;
      if (requestContext.IsServicingContext)
        requestContextType = RequestContextType.ServicingContext;
      else if (requestContext.IsSystemContext)
        requestContextType = RequestContextType.SystemContext;
      IVssRequestContext requestContext1 = requestContext;
      Guid instanceId = collectionHostId;
      int contextType = (int) requestContextType;
      return service.BeginRequest(requestContext1, instanceId, (RequestContextType) contextType, throwIfShutdown: false);
    }

    public List<CatalogResource> QueryResources(
      IVssRequestContext requestContext,
      IEnumerable<Guid> resourceIdentifiers,
      CatalogQueryOptions queryOptions)
    {
      requestContext.TraceEnter(197069176, "Catalog", "VirtualService", nameof (QueryResources));
      try
      {
        List<CatalogResource> catalogResourceList = new List<CatalogResource>();
        if (resourceIdentifiers.Contains<Guid>(this.GetServerInstanceId(requestContext)))
          catalogResourceList.Add(this.GetServerInstanceNode(requestContext).Resource);
        catalogResourceList.AddRange(this.GetScopedCollectionCatalogNodes(requestContext, false, (IEnumerable<CatalogNode>) null, resourceIdentifiers).Select<CatalogNode, CatalogResource>((Func<CatalogNode, CatalogResource>) (cn => cn.Resource)));
        return catalogResourceList.Count > 0 ? catalogResourceList : new List<CatalogResource>();
      }
      finally
      {
        requestContext.TraceLeave(180363327, "Catalog", "VirtualService", nameof (QueryResources));
      }
    }

    public List<CatalogResource> QueryResources(
      IVssRequestContext requestContext,
      IEnumerable<Guid> resourceTypes,
      IEnumerable<KeyValuePair<string, string>> propertyFilters,
      CatalogQueryOptions queryOptions)
    {
      requestContext.TraceEnter(190843156, "Catalog", "VirtualService", nameof (QueryResources));
      try
      {
        List<CatalogNode> catalogNodeList = this.QueryNodes(requestContext, (IEnumerable<string>) new string[1]
        {
          CatalogConstants.FullRecurseStars
        }, resourceTypes, queryOptions);
        List<CatalogNode> source = new List<CatalogNode>();
        foreach (CatalogNode catalogNode in catalogNodeList)
        {
          bool flag = true;
          foreach (KeyValuePair<string, string> propertyFilter in propertyFilters)
          {
            string y;
            if (!catalogNode.Resource.Properties.TryGetValue(propertyFilter.Key, out y) || propertyFilter.Value != null && !StringComparer.OrdinalIgnoreCase.Equals(propertyFilter.Value, y))
            {
              flag = false;
              break;
            }
          }
          if (flag)
          {
            catalogNode.MatchedQuery = false;
            source.Add(catalogNode);
          }
        }
        if (source.Count > 0)
          return source.Select<CatalogNode, CatalogResource>((Func<CatalogNode, CatalogResource>) (cn => cn.Resource)).ToList<CatalogResource>();
        if (this.HasFilterableResourceTypes(resourceTypes))
          return new List<CatalogResource>();
        throw new DeprecatedCatalogMethodException();
      }
      finally
      {
        requestContext.TraceLeave(167224791, "Catalog", "VirtualService", nameof (QueryResources));
      }
    }

    public CatalogNode QueryRootNode(IVssRequestContext requestContext, CatalogTree tree)
    {
      if (tree == CatalogTree.Organizational)
        return this.GetOrganizationalRootNode(requestContext);
      throw new DeprecatedCatalogMethodException();
    }

    public List<CatalogNode> QueryRootNodes(IVssRequestContext requestContext) => new List<CatalogNode>()
    {
      this.GetOrganizationalRootNode(requestContext)
    };

    public List<CatalogResourceType> QueryResourceTypes(
      IVssRequestContext requestContext,
      IEnumerable<Guid> resourceTypeIdentifiers)
    {
      return TeamFoundationVirtualCatalogService.s_catalogResourceTypes.Values.ToList<CatalogResourceType>();
    }

    public List<CatalogResource> QueryResourcesByType(
      IVssRequestContext requestContext,
      IEnumerable<Guid> resourceTypes,
      CatalogQueryOptions queryOptions)
    {
      return this.QueryResources(requestContext, resourceTypes, (IEnumerable<KeyValuePair<string, string>>) new KeyValuePair<string, string>[0], queryOptions);
    }

    public CatalogTransactionContext CreateTransactionContext() => new TeamFoundationCatalogService().CreateTransactionContext();

    public List<CatalogNode> QueryParents(
      IVssRequestContext requestContext,
      Guid resourceIdentifier,
      IEnumerable<string> pathFilters,
      IEnumerable<Guid> resourceTypeFilters,
      bool recurseToRoot,
      CatalogQueryOptions queryOptions)
    {
      throw new DeprecatedCatalogMethodException();
    }

    public CatalogResourceType QueryResourceType(
      IVssRequestContext requestContext,
      Guid resourceTypeIdentifier)
    {
      throw new DeprecatedCatalogMethodException();
    }

    public List<CatalogResource> SaveTransactionContextChanges(
      IVssRequestContext requestContext,
      CatalogTransactionContext context,
      CatalogQueryOptions queryOptions,
      bool preview,
      out List<CatalogResource> deletedResources,
      out List<CatalogNode> deletedNodes)
    {
      deletedResources = new List<CatalogResource>();
      deletedNodes = new List<CatalogNode>();
      return new List<CatalogResource>();
    }

    private void ParentCatalogNodes(
      CatalogNode parentNode,
      IEnumerable<CatalogNode> childNodes,
      Func<CatalogNode, CatalogNode, bool> filter = null)
    {
      if (childNodes == null)
        return;
      foreach (CatalogNode childNode in childNodes)
      {
        if (filter == null || filter(parentNode, childNode))
          childNode.ParentNode = parentNode;
      }
    }

    internal static IEnumerable<HostProperties> GetScopedCollectionHostProperties(
      IVssRequestContext requestContext)
    {
      ITeamFoundationHostManagementService service = requestContext.GetService<ITeamFoundationHostManagementService>();
      if (!requestContext.RootContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection))
        return service.QueryChildrenServiceHostPropertiesCached(requestContext, requestContext.ServiceHost.InstanceId);
      return (IEnumerable<HostProperties>) new HostProperties[1]
      {
        service.QueryServiceHostPropertiesCached(requestContext, requestContext.RootContext.ServiceHost.InstanceId)
      };
    }

    internal virtual IEnumerable<CatalogNode> GetScopedCollectionCatalogNodes(
      IVssRequestContext requestContext,
      bool nodeMatchedQuery,
      IEnumerable<CatalogNode> childNodes,
      IEnumerable<Guid> resultFilter = null)
    {
      IEnumerable<HostProperties> collectionHostProperties1 = TeamFoundationVirtualCatalogService.GetScopedCollectionHostProperties(requestContext);
      List<CatalogNode> collectionCatalogNodes = new List<CatalogNode>();
      foreach (HostProperties collectionHostProperties2 in collectionHostProperties1)
      {
        CatalogNode collectionCatalogNode;
        if ((resultFilter == null || resultFilter.Contains<Guid>(collectionHostProperties2.Id)) && this.TryCreateCollectionCatalogNode(requestContext, collectionHostProperties2, nodeMatchedQuery, out collectionCatalogNode))
        {
          collectionCatalogNodes.Add(collectionCatalogNode);
          this.ParentCatalogNodes(collectionCatalogNode, childNodes, (Func<CatalogNode, CatalogNode, bool>) ((parent, child) => VssStringComparer.CatalogNodePath.Equals(parent.FullPath, child.ParentPath)));
        }
      }
      return (IEnumerable<CatalogNode>) collectionCatalogNodes;
    }

    private bool HasFilterableResourceTypes(IEnumerable<Guid> resourceTypes)
    {
      foreach (Guid resourceType in resourceTypes)
      {
        if (TeamFoundationVirtualCatalogService.s_filterableResourceTypes.Contains(resourceType))
          return true;
      }
      return false;
    }

    private CatalogNode CreateProjectCatalogNode(ProjectInfo project, string parentPath)
    {
      CatalogResource catalogResource = new CatalogResource(TeamFoundationVirtualCatalogService.s_catalogResourceTypes[CatalogResourceTypes.TeamProject], FrameworkResources.TeamProject())
      {
        Identifier = project.Id,
        ResourceTypeIdentifier = CatalogResourceTypes.TeamProject,
        Description = project.Description,
        DisplayName = project.Name,
        MatchedQuery = true,
        TempCorrelationId = project.Id
      };
      catalogResource.Properties["ProjectId"] = project.Id.ToString();
      catalogResource.Properties["ProjectName"] = project.Name;
      catalogResource.Properties["ProjectState"] = project.State.ToString();
      catalogResource.Properties["ProjectUri"] = project.Uri;
      foreach (ProjectProperty property in (IEnumerable<ProjectProperty>) project.Properties)
      {
        string toCatalogProperty = TeamFoundationVirtualCatalogService.s_projectPropertiesToCatalogProperties[property.Name];
        catalogResource.Properties[toCatalogProperty] = (string) property.Value;
      }
      string base64String = Convert.ToBase64String(project.Id.ToByteArray());
      CatalogNode projectCatalogNode = new CatalogNode((ITeamFoundationCatalogService) this, parentPath, base64String, catalogResource)
      {
        FullPath = parentPath + base64String,
        ResourceIdentifier = catalogResource.Identifier,
        MatchedQuery = true,
        NodeDependenciesIncluded = true
      };
      catalogResource.NodeReferences.Add(projectCatalogNode);
      return projectCatalogNode;
    }

    private bool TryCreateCollectionCatalogNode(
      IVssRequestContext requestContext,
      HostProperties collectionHostProperties,
      bool catalogNodeMatchedQuery,
      out CatalogNode collectionCatalogNode)
    {
      bool dispose = false;
      IVssRequestContext vssRequestContext = (IVssRequestContext) null;
      try
      {
        vssRequestContext = TeamFoundationVirtualCatalogService.BeginOrGetCollectionRequestContext(requestContext, collectionHostProperties.Id, out dispose);
        if (!vssRequestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(vssRequestContext, FrameworkSecurity.FrameworkNamespaceId).HasPermission(vssRequestContext, FrameworkSecurity.FrameworkNamespaceToken, 1))
        {
          collectionCatalogNode = (CatalogNode) null;
          return false;
        }
      }
      finally
      {
        if (dispose && vssRequestContext != null)
          vssRequestContext.Dispose();
      }
      CatalogResource catalogResource = new CatalogResource(TeamFoundationVirtualCatalogService.s_catalogResourceTypes[CatalogResourceTypes.ProjectCollection], FrameworkResources.TeamProjectCollection())
      {
        Identifier = collectionHostProperties.Id,
        ResourceTypeIdentifier = CatalogResourceTypes.ProjectCollection,
        DisplayName = LocationServiceHelper.UseLegacyDefaultCollectionRouting(requestContext) ? "DefaultCollection" : collectionHostProperties.Name,
        Description = collectionHostProperties.Description,
        TempCorrelationId = collectionHostProperties.Id,
        MatchedQuery = true,
        ChangeTypeValue = 0
      };
      ServiceDefinition serviceDefinition = requestContext.GetService<ILocationService>().FindServiceDefinition(requestContext, "LocationService", collectionHostProperties.Id);
      catalogResource.ServiceReferences["Location"] = serviceDefinition;
      IDictionary<string, string> properties = catalogResource.Properties;
      Guid id = collectionHostProperties.Id;
      string str = id.ToString("D", (IFormatProvider) CultureInfo.InvariantCulture);
      properties.Add("InstanceId", str);
      id = collectionHostProperties.Id;
      string base64String = Convert.ToBase64String(id.ToByteArray());
      CatalogNode serverInstanceNode = this.GetServerInstanceNode(requestContext);
      collectionCatalogNode = new CatalogNode((ITeamFoundationCatalogService) this, serverInstanceNode.FullPath, base64String, catalogResource)
      {
        FullPath = serverInstanceNode.FullPath + base64String,
        ResourceIdentifier = catalogResource.Identifier,
        MatchedQuery = catalogNodeMatchedQuery,
        NodeDependenciesIncluded = true
      };
      catalogResource.NodeReferences.Add(collectionCatalogNode);
      return true;
    }

    private IEnumerable<CatalogNode> GetChildNodes(
      List<CatalogNode> potentialChildNodes,
      int length,
      bool includeParents)
    {
      IEnumerable<CatalogNode> childNodes = (IEnumerable<CatalogNode>) null;
      if (includeParents)
        childNodes = potentialChildNodes.Where<CatalogNode>((Func<CatalogNode, bool>) (node => node.ParentPath.Length / TeamFoundationVirtualCatalogService.s_lengthOfBase64OfGuidByteArray == length));
      return childNodes;
    }

    private CatalogNode GetOrganizationalRootNode(IVssRequestContext requestContext)
    {
      CatalogResource catalogResource = new CatalogResource(TeamFoundationVirtualCatalogService.s_catalogResourceTypes[CatalogResourceTypes.OrganizationalRoot], FrameworkResources.OrganizationalRoot())
      {
        Identifier = requestContext.ServiceHost.DeploymentServiceHost.InstanceId,
        ResourceTypeIdentifier = CatalogResourceTypes.OrganizationalRoot,
        DisplayName = FrameworkResources.OrganizationalRoot(),
        TempCorrelationId = requestContext.ServiceHost.DeploymentServiceHost.InstanceId,
        MatchedQuery = false,
        ChangeTypeValue = 0
      };
      CatalogNode organizationalRootNode = new CatalogNode((ITeamFoundationCatalogService) this, string.Empty, CatalogRoots.OrganizationalPath, catalogResource)
      {
        FullPath = CatalogRoots.OrganizationalPath,
        ResourceIdentifier = catalogResource.Identifier,
        MatchedQuery = false,
        NodeDependenciesIncluded = true
      };
      catalogResource.NodeReferences.Add(organizationalRootNode);
      return organizationalRootNode;
    }

    internal unsafe Guid GetServerInstanceId(IVssRequestContext requestContext)
    {
      if (!requestContext.RootContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection))
        return requestContext.ServiceHost.InstanceId;
      Guid instanceId = requestContext.RootContext.ServiceHost.InstanceId;
      uint* numPtr = (uint*) &instanceId;
      uint num1 = *numPtr;
      *numPtr = numPtr[3];
      numPtr[3] = num1;
      uint num2 = numPtr[1];
      numPtr[1] = numPtr[2];
      numPtr[2] = num2;
      return instanceId;
    }

    private CatalogNode GetServerInstanceNode(IVssRequestContext requestContext)
    {
      Guid serverInstanceId = this.GetServerInstanceId(requestContext);
      CatalogResource catalogResource = new CatalogResource(TeamFoundationVirtualCatalogService.s_catalogResourceTypes[CatalogResourceTypes.TeamFoundationServerInstance], FrameworkResources.TeamFoundationServerInstance())
      {
        Identifier = serverInstanceId,
        ResourceTypeIdentifier = CatalogResourceTypes.TeamFoundationServerInstance,
        DisplayName = FrameworkResources.TeamFoundationServerInstance(),
        TempCorrelationId = serverInstanceId,
        MatchedQuery = true,
        ChangeTypeValue = 0
      };
      ILocationService service = requestContext.GetService<ILocationService>();
      catalogResource.ServiceReferences["Location"] = service.FindServiceDefinition(requestContext, "LocationService", Microsoft.TeamFoundation.Framework.Common.LocationServiceConstants.SelfReferenceLocationServiceIdentifier);
      string base64String = Convert.ToBase64String(serverInstanceId.ToByteArray());
      CatalogNode serverInstanceNode = new CatalogNode((ITeamFoundationCatalogService) this, CatalogRoots.OrganizationalPath, base64String, catalogResource)
      {
        FullPath = CatalogRoots.OrganizationalPath + base64String,
        ResourceIdentifier = catalogResource.Identifier,
        MatchedQuery = false,
        NodeDependenciesIncluded = true
      };
      catalogResource.NodeReferences.Add(serverInstanceNode);
      return serverInstanceNode;
    }

    private static Dictionary<Guid, CatalogResourceType> BuildCatalogResourceTypes() => new Dictionary<Guid, CatalogResourceType>()
    {
      {
        CatalogResourceTypes.OrganizationalRoot,
        new CatalogResourceType(CatalogResourceTypes.OrganizationalRoot, FrameworkResources.OrganizationalRoot(), FrameworkResources.OrganizationalRootDescription())
      },
      {
        CatalogResourceTypes.TeamFoundationServerInstance,
        new CatalogResourceType(CatalogResourceTypes.TeamFoundationServerInstance, FrameworkResources.TeamFoundationServerInstance(), FrameworkResources.TeamFoundationServerInstanceDescription())
      },
      {
        CatalogResourceTypes.TeamSystemWebAccess,
        new CatalogResourceType(CatalogResourceTypes.TeamSystemWebAccess, FrameworkResources.TeamSystemWebAccess(), FrameworkResources.TeamSystemWebAccessDescription())
      },
      {
        CatalogResourceTypes.ProjectCollection,
        new CatalogResourceType(CatalogResourceTypes.ProjectCollection, FrameworkResources.TeamProjectCollection(), FrameworkResources.TeamProjectCollectionDescription())
      },
      {
        CatalogResourceTypes.TeamProject,
        new CatalogResourceType(CatalogResourceTypes.TeamProject, FrameworkResources.TeamProject(), FrameworkResources.TeamProjectDescription())
      },
      {
        CatalogResourceTypes.ProcessGuidanceSite,
        new CatalogResourceType(CatalogResourceTypes.ProcessGuidanceSite, FrameworkResources.ProcessGuidanceSite(), FrameworkResources.ProcessGuidanceSiteDescription())
      },
      {
        CatalogResourceTypes.ProjectPortal,
        new CatalogResourceType(CatalogResourceTypes.ProjectPortal, FrameworkResources.ProjectPortal(), FrameworkResources.ProjectPortalDescription())
      }
    };

    private class CatalogSearchFilter
    {
      private static readonly int s_maxDepth = 3;
      private static readonly int s_minDepth = 0;

      public bool[] SearchDepths { get; set; }

      public HashSet<Guid> ResourceIdentifiers { get; private set; }

      public CatalogSearchFilter(IEnumerable<string> searchStrings, IEnumerable<Guid> resourceTypes)
      {
        this.ResourceIdentifiers = resourceTypes != null ? new HashSet<Guid>(resourceTypes) : new HashSet<Guid>();
        this.SearchDepths = new bool[TeamFoundationVirtualCatalogService.CatalogSearchFilter.s_maxDepth + 1];
        if (searchStrings == null || searchStrings.Count<string>() == 0)
        {
          this.AddFilter(TeamFoundationVirtualCatalogService.CatalogSearchFilter.s_minDepth, true);
        }
        else
        {
          foreach (string searchString in searchStrings)
          {
            ArgumentUtility.CheckStringForNullOrEmpty(searchString, "path");
            int length = searchString.Length / TeamFoundationVirtualCatalogService.s_lengthOfBase64OfGuidByteArray;
            bool recursive = searchString.EndsWith(CatalogConstants.FullRecurseStars) || searchString.EndsWith(CatalogConstants.FullRecurseDots);
            if (!recursive && searchString.EndsWith(CatalogConstants.SingleRecurseStar))
              ++length;
            this.AddFilter(length, recursive);
          }
        }
      }

      public void AddFilter(int length, bool recursive)
      {
        int index1 = length - 1;
        if (index1 < TeamFoundationVirtualCatalogService.CatalogSearchFilter.s_minDepth)
          index1 = TeamFoundationVirtualCatalogService.CatalogSearchFilter.s_minDepth;
        if (index1 > TeamFoundationVirtualCatalogService.CatalogSearchFilter.s_maxDepth)
          return;
        this.SearchDepths[index1] = true;
        if (!recursive)
          return;
        for (int index2 = index1; index2 <= TeamFoundationVirtualCatalogService.CatalogSearchFilter.s_maxDepth; ++index2)
          this.SearchDepths[index2] = true;
      }

      public bool IsMatch(int length, Guid resourceIdentifier, bool needsParentNodes)
      {
        if (needsParentNodes)
          return true;
        return this.SearchDepths[length - 1] && this.ResourceIdentifiers.Contains(resourceIdentifier);
      }
    }
  }
}
