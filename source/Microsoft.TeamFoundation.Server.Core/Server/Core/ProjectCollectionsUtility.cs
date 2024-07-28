// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.ProjectCollectionsUtility
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core.CollectionManagement.WebServer;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.Core
{
  public class ProjectCollectionsUtility
  {
    public static IEnumerable<TeamProjectCollectionReference> GetProjectCollections(
      IVssRequestContext requestContext,
      int top,
      int skip)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Application);
      return vssRequestContext.GetService<ITeamProjectCollectionPropertiesService>().GetCollectionPropertiesCached(vssRequestContext).Select<TeamProjectCollectionProperties, TeamProjectCollectionReference>((Func<TeamProjectCollectionProperties, TeamProjectCollectionReference>) (properties => properties.ToTeamProjectCollectionReference(requestContext))).Skip<TeamProjectCollectionReference>(skip).Take<TeamProjectCollectionReference>(top);
    }

    public static IEnumerable<TeamProjectCollectionReference> GetCatalogProjectCollections(
      IVssRequestContext requestContext)
    {
      return ProjectCollectionsUtility.GetCatalogProjectNodes(requestContext).Select<CatalogNode, TeamProjectCollectionReference>((Func<CatalogNode, TeamProjectCollectionReference>) (c => c.ToTeamProjectCollectionReference(requestContext)));
    }

    private static IEnumerable<CatalogNode> GetCatalogProjectNodes(IVssRequestContext requestContext)
    {
      if (requestContext.ExecutionEnvironment.IsHostedDeployment)
        throw new InvalidOperationException("This method is not supported in hosted deployment.");
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Application);
      return (IEnumerable<CatalogNode>) vssRequestContext.GetService<ITeamFoundationCatalogService>().QueryNodes(vssRequestContext, (IEnumerable<string>) new string[1]
      {
        "**"
      }, (IEnumerable<Guid>) new Guid[1]
      {
        CatalogResourceTypes.ProjectCollection
      }, CatalogQueryOptions.None);
    }

    public static IEnumerable<WebApiProjectCollection> GetWebApiProjectCollections(
      IVssRequestContext requestContext,
      int top,
      int skip)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Application);
      return vssRequestContext.GetService<ITeamProjectCollectionPropertiesService>().GetCollectionPropertiesCached(vssRequestContext).Select<TeamProjectCollectionProperties, WebApiProjectCollection>((Func<TeamProjectCollectionProperties, WebApiProjectCollection>) (properties => properties.ToTeamProjectCollection(requestContext).ToWebApiProjectCollection())).Skip<WebApiProjectCollection>(skip).Take<WebApiProjectCollection>(top);
    }

    public static TeamProjectCollection GetProjectCollection(
      IVssRequestContext requestContext,
      string collectionId)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Application);
      Guid result;
      if (!Guid.TryParse(collectionId, out result))
      {
        if (requestContext.ExecutionEnvironment.IsHostedDeployment && requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
          throw new UnexpectedHostTypeException(requestContext.ServiceHost.HostType);
        HostProperties hostProperties = vssRequestContext.GetService<TeamFoundationHostManagementService>().QueryChildrenServiceHostPropertiesCached(vssRequestContext, vssRequestContext.ServiceHost.InstanceId).FirstOrDefault<HostProperties>((Func<HostProperties, bool>) (childHost => string.Equals(childHost.Name, collectionId, StringComparison.OrdinalIgnoreCase)));
        result = hostProperties != null && hostProperties.HostType == TeamFoundationHostType.ProjectCollection ? hostProperties.Id : throw new CollectionDoesNotExistException(collectionId);
      }
      return vssRequestContext.GetService<ITeamProjectCollectionPropertiesService>().GetCollectionPropertiesCached(vssRequestContext, result, true).ToTeamProjectCollection(requestContext);
    }
  }
}
