// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.WebServices.CatalogWebService
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Location.Server;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Web.Services;

namespace Microsoft.TeamFoundation.Server.Core.WebServices
{
  [WebService(Namespace = "http://microsoft.com/webservices/")]
  [ClientService(ComponentName = "Framework", ServiceName = "CatalogService", ServerConfiguration = ServerConfiguration.TfsConnection, ConfigurationServiceIdentifier = "C2F9106F-127A-45B7-B0A3-E0AD8239A2A7")]
  public class CatalogWebService : FrameworkWebService
  {
    private const string c_area = "CatalogService";
    private const string c_layer = "CatalogWebService";

    [WebMethod]
    public List<CatalogResourceType> QueryResourceTypes(Guid[] resourceTypeIdentifiers)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (QueryResourceTypes), MethodType.ReadWrite, EstimatedMethodCost.Low);
        methodInformation.AddArrayParameter<Guid>(nameof (resourceTypeIdentifiers), (IList<Guid>) resourceTypeIdentifiers);
        this.EnterMethod(methodInformation);
        IVssRequestContext vssRequestContext = this.RequestContext.To(TeamFoundationHostType.Application);
        return vssRequestContext.GetService<ITeamFoundationCatalogService>().QueryResourceTypes(vssRequestContext, (IEnumerable<Guid>) resourceTypeIdentifiers);
      }
      catch (Exception ex)
      {
        this.TraceFailedCatalogCommand(6421132, nameof (QueryResourceTypes), "resource types", resourceTypeIdentifiers);
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    [WebMethod]
    public CatalogData QueryResources(Guid[] resourceIdentifiers, int queryOptions)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation("QueryResource", MethodType.ReadWrite, EstimatedMethodCost.Moderate);
        methodInformation.AddArrayParameter<Guid>(nameof (resourceIdentifiers), (IList<Guid>) resourceIdentifiers);
        methodInformation.AddParameter(nameof (queryOptions), (object) queryOptions);
        this.EnterMethod(methodInformation);
        IVssRequestContext vssRequestContext = this.RequestContext.To(TeamFoundationHostType.Application);
        return new CatalogData((IEnumerable<CatalogResource>) vssRequestContext.GetService<ITeamFoundationCatalogService>().QueryResources(vssRequestContext, (IEnumerable<Guid>) resourceIdentifiers, (CatalogQueryOptions) queryOptions), this.GetLastChangeId(vssRequestContext));
      }
      catch (Exception ex)
      {
        this.TraceFailedCatalogCommand(7976576, nameof (QueryResources), "resources", resourceIdentifiers);
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    [WebMethod]
    public CatalogData QueryResourcesByType(
      Guid[] resourceTypes,
      KeyValue<string, string>[] propertyFilters,
      int queryOptions)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (QueryResourcesByType), MethodType.ReadWrite, EstimatedMethodCost.Moderate);
        methodInformation.AddArrayParameter<Guid>(nameof (resourceTypes), (IList<Guid>) resourceTypes);
        methodInformation.AddArrayParameter<KeyValue<string, string>>(nameof (propertyFilters), (IList<KeyValue<string, string>>) propertyFilters);
        methodInformation.AddParameter(nameof (queryOptions), (object) queryOptions);
        this.EnterMethod(methodInformation);
        IVssRequestContext vssRequestContext = this.RequestContext.To(TeamFoundationHostType.Application);
        ITeamFoundationCatalogService service = vssRequestContext.GetService<ITeamFoundationCatalogService>();
        return new CatalogData(propertyFilters == null || propertyFilters.Length == 0 ? (IEnumerable<CatalogResource>) service.QueryResourcesByType(vssRequestContext, (IEnumerable<Guid>) resourceTypes, (CatalogQueryOptions) queryOptions) : (IEnumerable<CatalogResource>) service.QueryResources(vssRequestContext, (IEnumerable<Guid>) resourceTypes, (IEnumerable<KeyValuePair<string, string>>) KeyValue<string, string>.Convert((IEnumerable<KeyValue<string, string>>) propertyFilters), (CatalogQueryOptions) queryOptions), this.GetLastChangeId(vssRequestContext));
      }
      catch (Exception ex)
      {
        this.TraceFailedCatalogCommand(6428687, nameof (QueryResourcesByType), "resource types", resourceTypes);
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    [WebMethod]
    public CatalogData QueryNodes(
      string[] pathSpecs,
      Guid[] resourceTypeFilters,
      KeyValue<string, string>[] propertyFilters,
      int queryOptions)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (QueryNodes), MethodType.ReadWrite, EstimatedMethodCost.Moderate);
        methodInformation.AddArrayParameter<string>(nameof (pathSpecs), (IList<string>) pathSpecs);
        methodInformation.AddArrayParameter<Guid>(nameof (resourceTypeFilters), (IList<Guid>) resourceTypeFilters);
        methodInformation.AddParameter(nameof (queryOptions), (object) queryOptions);
        this.EnterMethod(methodInformation);
        IVssRequestContext vssRequestContext = this.RequestContext.To(TeamFoundationHostType.Application);
        ITeamFoundationCatalogService service = vssRequestContext.GetService<ITeamFoundationCatalogService>();
        return new CatalogData(propertyFilters == null || propertyFilters.Length == 0 ? (IEnumerable<CatalogNode>) service.QueryNodes(vssRequestContext, (IEnumerable<string>) pathSpecs, (IEnumerable<Guid>) resourceTypeFilters, (CatalogQueryOptions) queryOptions) : (IEnumerable<CatalogNode>) service.QueryNodes(vssRequestContext, (IEnumerable<string>) pathSpecs, (IEnumerable<Guid>) resourceTypeFilters, (IEnumerable<KeyValuePair<string, string>>) KeyValue<string, string>.Convert((IEnumerable<KeyValue<string, string>>) propertyFilters), (CatalogQueryOptions) queryOptions), this.GetLastChangeId(vssRequestContext));
      }
      catch (Exception ex)
      {
        this.TraceFailedCatalogCommand(5013618, nameof (QueryNodes), "resource types", resourceTypeFilters);
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    [WebMethod]
    public CatalogData QueryParents(
      Guid resourceIdentifier,
      string[] pathFilters,
      Guid[] resourceTypeFilters,
      bool recurseToRoot,
      int queryOptions)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (QueryParents), MethodType.ReadWrite, EstimatedMethodCost.Moderate);
        methodInformation.AddParameter(nameof (resourceIdentifier), (object) resourceIdentifier);
        methodInformation.AddArrayParameter<string>(nameof (pathFilters), (IList<string>) pathFilters);
        methodInformation.AddArrayParameter<Guid>(nameof (resourceTypeFilters), (IList<Guid>) resourceTypeFilters);
        methodInformation.AddParameter(nameof (recurseToRoot), (object) recurseToRoot);
        methodInformation.AddParameter(nameof (queryOptions), (object) queryOptions);
        this.EnterMethod(methodInformation);
        IVssRequestContext vssRequestContext = this.RequestContext.To(TeamFoundationHostType.Application);
        return new CatalogData((IEnumerable<CatalogNode>) vssRequestContext.GetService<ITeamFoundationCatalogService>().QueryParents(vssRequestContext, resourceIdentifier, (IEnumerable<string>) pathFilters, (IEnumerable<Guid>) resourceTypeFilters, recurseToRoot, (CatalogQueryOptions) queryOptions), this.GetLastChangeId(vssRequestContext));
      }
      catch (Exception ex)
      {
        this.TraceFailedCatalogCommand(2116554, nameof (QueryParents), "resource types", resourceTypeFilters);
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    [WebMethod]
    public CatalogData QueryDependents(string path, int queryOptions)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (QueryDependents), MethodType.ReadWrite, EstimatedMethodCost.Moderate);
        methodInformation.AddParameter(nameof (path), (object) path);
        methodInformation.AddParameter(nameof (queryOptions), (object) queryOptions);
        this.EnterMethod(methodInformation);
        IVssRequestContext vssRequestContext = this.RequestContext.To(TeamFoundationHostType.Application);
        return new CatalogData((IEnumerable<CatalogNode>) vssRequestContext.GetService<ITeamFoundationCatalogService>().QueryCatalogDependents(vssRequestContext, path, (CatalogQueryOptions) queryOptions), this.GetLastChangeId(vssRequestContext));
      }
      catch (Exception ex)
      {
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    [WebMethod]
    public CatalogData SaveCatalogChanges(
      CatalogResource[] resources,
      CatalogNode[] nodes,
      KeyValue<string, string>[] nodeMoves,
      int queryOptions,
      bool preview)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (SaveCatalogChanges), MethodType.ReadWrite, EstimatedMethodCost.High);
        methodInformation.AddArrayParameter<CatalogResource>(nameof (resources), (IList<CatalogResource>) resources);
        methodInformation.AddArrayParameter<CatalogNode>(nameof (nodes), (IList<CatalogNode>) nodes);
        methodInformation.AddArrayParameter<KeyValue<string, string>>(nameof (nodeMoves), (IList<KeyValue<string, string>>) nodeMoves);
        methodInformation.AddParameter(nameof (queryOptions), (object) queryOptions);
        methodInformation.AddParameter(nameof (preview), (object) preview);
        this.EnterMethod(methodInformation);
        this.RequestContext.CheckOnPremisesDeployment(true);
        IVssRequestContext vssRequestContext = this.RequestContext.To(TeamFoundationHostType.Application);
        ITeamFoundationCatalogService service = vssRequestContext.GetService<ITeamFoundationCatalogService>();
        CatalogTransactionContext transactionContext = service.CreateTransactionContext();
        Dictionary<Guid, CatalogResource> resources1 = new Dictionary<Guid, CatalogResource>();
        if (resources != null)
        {
          TeamFoundationTrace.Info(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Calling SaveCatalogChanges with {0} resources", (object) resources.Length));
          foreach (CatalogResource resource in resources)
          {
            TeamFoundationTrace.Info("     Saving resource " + resource.DisplayName);
            Guid key = resource.Identifier == Guid.Empty ? resource.TempCorrelationId : resource.Identifier;
            resource.InitializeFromWebService(vssRequestContext, service);
            resources1[key] = resource;
            switch (resource.ChangeType)
            {
              case CatalogChangeType.None:
                continue;
              case CatalogChangeType.CreateOrUpdate:
                transactionContext.AttachResource(resource);
                continue;
              default:
                throw new InvalidCatalogSaveResourceException(FrameworkResources.CatalogInvalidChangeTypeResource((object) resource.ChangeType));
            }
          }
        }
        Dictionary<string, CatalogNode> dictionary = new Dictionary<string, CatalogNode>((IEqualityComparer<string>) VssStringComparer.CatalogNodePath);
        if (nodes != null)
        {
          foreach (CatalogNode node in nodes)
          {
            node.InitializeFromWebService(service, resources1);
            dictionary[node.ParentPath + node.ChildItem] = node;
            switch (node.ChangeType)
            {
              case CatalogChangeType.None:
                continue;
              case CatalogChangeType.CreateOrUpdate:
                transactionContext.AttachNode(node);
                continue;
              case CatalogChangeType.RecursiveDelete:
                transactionContext.AttachDelete(node, true);
                continue;
              case CatalogChangeType.NonRecursiveDelete:
                transactionContext.AttachDelete(node, false);
                continue;
              default:
                throw new InvalidCatalogSaveNodeException(FrameworkResources.CatalogInvalidChangeTypeNode((object) node.ChangeType));
            }
          }
          foreach (CatalogNode node in nodes)
          {
            if (node.NodeDependenciesIncluded)
            {
              foreach (CatalogNodeDependency nodeDependency in node.NodeDependencies)
              {
                CatalogNode catalogNode;
                if (!dictionary.TryGetValue(nodeDependency.RequiredNodeFullPath, out catalogNode))
                  throw new InvalidCatalogSaveNodeException(FrameworkResources.MustIncludeDependentNodeOverWebService());
                if (nodeDependency.IsSingleton)
                {
                  node.Dependencies.Singletons[nodeDependency.AssociationKey] = catalogNode;
                }
                else
                {
                  IList<CatalogNode> catalogNodeList;
                  if (!node.Dependencies.Sets.TryGetValue(nodeDependency.AssociationKey, out catalogNodeList))
                  {
                    catalogNodeList = (IList<CatalogNode>) new List<CatalogNode>();
                    node.Dependencies.Sets[nodeDependency.AssociationKey] = catalogNodeList;
                  }
                  catalogNodeList.Add(catalogNode);
                }
              }
            }
          }
        }
        if (nodeMoves != null)
        {
          foreach (KeyValue<string, string> nodeMove in nodeMoves)
          {
            ArgumentUtility.CheckStringForNullOrEmpty(nodeMove.Key, "nodeToMove.FullPath");
            ArgumentUtility.CheckStringForNullOrEmpty(nodeMove.Value, "newParent.FullPath");
            CatalogNode nodeToMove;
            if (!dictionary.TryGetValue(nodeMove.Key, out nodeToMove))
              throw new InvalidCatalogNodeMoveException(FrameworkResources.MustPassMovedAndParentNodeOnMove());
            CatalogNode newParent;
            if (!dictionary.TryGetValue(nodeMove.Value, out newParent))
              throw new InvalidCatalogNodeMoveException(FrameworkResources.MustPassMovedAndParentNodeOnMove());
            transactionContext.AttachMove(nodeToMove, newParent);
          }
        }
        List<CatalogResource> deletedResources;
        List<CatalogNode> deletedNodes;
        return new CatalogData((IEnumerable<CatalogResource>) service.SaveTransactionContextChanges(vssRequestContext, transactionContext, (CatalogQueryOptions) queryOptions, preview, out deletedResources, out deletedNodes), (IEnumerable<CatalogResource>) deletedResources, (IEnumerable<CatalogNode>) deletedNodes, this.GetLastChangeId(vssRequestContext));
      }
      catch (Exception ex)
      {
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    private long GetLastChangeId(IVssRequestContext organizationRequestContext)
    {
      IVssRequestContext rootContext = organizationRequestContext.RootContext;
      return organizationRequestContext.ExecutionEnvironment.IsHostedDeployment && rootContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection) ? rootContext.GetService<ILocationService>().GetLastChangeId(rootContext) : organizationRequestContext.GetService<ILocationService>().GetLastChangeId(organizationRequestContext);
    }

    private void TraceFailedCatalogCommand(
      int tracepoint,
      string methodName,
      string dataTypeName,
      Guid[] data)
    {
      string str = data != null ? string.Join<Guid>(", ", (IEnumerable<Guid>) data) : string.Empty;
      this.RequestContext.Trace(tracepoint, TraceLevel.Error, "CatalogService", nameof (CatalogWebService), "Command " + methodName + " failed with the following resource " + dataTypeName + ": " + str);
    }
  }
}
