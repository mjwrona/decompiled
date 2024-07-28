// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.CollectionManagement.WebServer.ProjectCollectionApiExtensions
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Location.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;

namespace Microsoft.TeamFoundation.Server.Core.CollectionManagement.WebServer
{
  public static class ProjectCollectionApiExtensions
  {
    private const string s_editableOrgAvatarFeatureName = "VisualStudio.Services.AdminEngagement.OrganizationOverview.EditableOrganizationAvatar";

    public static TeamProjectCollection ToTeamProjectCollection(
      this TeamProjectCollectionProperties properties,
      IVssRequestContext requestContext)
    {
      TeamProjectCollection projectCollection = new TeamProjectCollection(properties.ToTeamProjectCollectionReference(requestContext))
      {
        Description = properties.Description,
        State = properties.State.ToString()
      };
      if (properties is TfsTeamProjectCollectionProperties)
        projectCollection.ProcessCustomizationType = (properties as TfsTeamProjectCollectionProperties).EnableInheritedProcessCustomization;
      string href;
      if (requestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection) && requestContext.ServiceHost.InstanceId == properties.Id)
      {
        ILocationService service = requestContext.GetService<ILocationService>();
        href = service.GetSelfReferenceUrl(requestContext, service.DetermineAccessMapping(requestContext));
      }
      else
      {
        IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Application);
        ILocationService service = vssRequestContext.GetService<ILocationService>();
        href = service.LocationForAccessMapping(vssRequestContext, "LocationService2", properties.Id, service.DetermineAccessMapping(vssRequestContext));
      }
      projectCollection.Links.AddLink("web", href);
      return projectCollection;
    }

    public static TeamProjectCollection ToTeamProjectCollection(
      this IVssRequestContext requestContext)
    {
      requestContext.CheckProjectCollectionRequestContext();
      IVssRequestContext context = requestContext.To(TeamFoundationHostType.Deployment);
      ITeamFoundationHostManagementService service1 = context.GetService<ITeamFoundationHostManagementService>();
      ILocationService service2 = requestContext.GetService<ILocationService>();
      IVssRequestContext requestContext1 = context;
      Guid instanceId = requestContext.ServiceHost.InstanceId;
      HostProperties hostProperties = (HostProperties) service1.QueryServiceHostProperties(requestContext1, instanceId);
      TeamProjectCollection projectCollection1 = new TeamProjectCollection();
      projectCollection1.Id = hostProperties.Id;
      projectCollection1.Name = hostProperties.Name;
      projectCollection1.Description = hostProperties.Description;
      projectCollection1.State = hostProperties.Status.ToString();
      projectCollection1.Url = service2.GetResourceUri(requestContext, "core", CoreConstants.ProjectCollectionsLocationId, (object) new
      {
        collectionId = hostProperties.Id
      }).ToString();
      TeamProjectCollection projectCollection2 = projectCollection1;
      string href = service2.LocationForAccessMapping(requestContext, "LocationService2", hostProperties.Id, service2.DetermineAccessMapping(requestContext));
      projectCollection2.Links = new ReferenceLinks();
      projectCollection2.Links.AddLink("self", projectCollection2.Url);
      projectCollection2.Links.AddLink("web", href);
      return projectCollection2;
    }

    public static TeamProjectCollectionReference ToTeamProjectCollectionReference(
      this TeamProjectCollectionProperties properties,
      IVssRequestContext requestContext)
    {
      return new TeamProjectCollectionReference()
      {
        Id = properties.Id,
        Name = properties.Name,
        Url = ProjectCollectionApiExtensions.GetCollectionResourceUrl(requestContext, properties.Id)
      };
    }

    public static TeamProjectCollectionReference ToTeamProjectCollectionReference(
      this CatalogNode collection,
      IVssRequestContext requestContext)
    {
      Guid id = new Guid(collection.Resource.Properties["InstanceId"]);
      return new TeamProjectCollectionReference()
      {
        Id = id,
        Name = collection.Resource.DisplayName,
        Url = ProjectCollectionApiExtensions.GetCollectionResourceUrl(requestContext, id)
      };
    }

    public static WebApiProjectCollection ToWebApiProjectCollection(
      this TeamProjectCollection collection)
    {
      return new WebApiProjectCollection(collection.ToWebApiProjectCollectionRef())
      {
        Description = collection.Description,
        State = collection.State
      };
    }

    public static WebApiProjectCollectionRef ToWebApiProjectCollectionRef(
      this TeamProjectCollection collection)
    {
      return new WebApiProjectCollectionRef()
      {
        Id = collection.Id,
        Name = collection.Name,
        Url = collection.Url,
        CollectionUrl = ((ReferenceLink) collection.Links.Links["web"]).Href
      };
    }

    public static string GetCollectionResourceUrl(IVssRequestContext requestContext, Guid id)
    {
      if (requestContext.ExecutionEnvironment.IsOnPremisesDeployment)
        requestContext = requestContext.To(TeamFoundationHostType.Application);
      return ServerCoreApiExtensions.GetCoreResourceUriString(requestContext, CoreConstants.ProjectCollectionsLocationId, (object) new
      {
        collectionId = id
      });
    }
  }
}
