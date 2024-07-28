// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Admin.CollectionViewModel
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Admin, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 31215F45-B8A9-42A7-99A7-F8CB77B7D405
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Admin.dll

using Microsoft.Azure.Boards.ProcessTemplates;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.TeamFoundation.Server.WebAccess.Routing;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.WebAccess.Admin
{
  public class CollectionViewModel
  {
    private TeamFoundationServiceHostStatus m_status;

    public CollectionViewModel(TfsWebContext webContext)
      : this(webContext, new TfsServiceHostDescriptor(webContext.TfsRequestContext))
    {
    }

    public CollectionViewModel(
      TfsWebContext webContext,
      TfsServiceHostDescriptor collectionServiceHost)
    {
      CatalogNode collectionCatalogNode = webContext.GetCollectionCatalogNode(collectionServiceHost.Id);
      this.CollectionServiceHost = collectionServiceHost;
      this.CollectionId = collectionServiceHost.Id;
      this.DisplayName = collectionServiceHost.Name;
      this.Description = collectionCatalogNode != null ? collectionCatalogNode.Resource.Description : "";
      this.m_status = collectionServiceHost.Status;
      this.StatusReason = collectionServiceHost.StatusReason;
      this.TeamProjects = new List<TeamProjectModel>();
      if (webContext.TfsRequestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection))
      {
        this.PopulateTeamProjects(webContext);
      }
      else
      {
        IEnumerable<CatalogNode> projectCatalogNodes = webContext.GetProjectCatalogNodes(collectionServiceHost.Id);
        if (!projectCatalogNodes.Any<CatalogNode>())
          return;
        using (IVssRequestContext requestContext = webContext.TfsRequestContext.GetService<TeamFoundationHostManagementService>().BeginImpersonatedRequest(webContext.TfsRequestContext, collectionServiceHost.Id, RequestContextType.UserContext, webContext.TfsRequestContext.RequestContextInternal().Actors))
          this.PopulateTeamProjects(requestContext, projectCatalogNodes);
      }
    }

    private void PopulateTeamProjects(
      IVssRequestContext requestContext,
      IEnumerable<CatalogNode> nodes)
    {
      foreach (CatalogNode node in nodes)
      {
        try
        {
          this.TeamProjects.Add(new TeamProjectModel(requestContext, node));
        }
        catch (UnauthorizedAccessException ex)
        {
        }
      }
      this.TeamProjects.Sort();
    }

    private void PopulateTeamProjects(TfsWebContext webContext)
    {
      IVssRequestContext tfsRequestContext = webContext.TfsRequestContext;
      this.TeamProjects.Clear();
      ITeamFoundationProcessService service1 = tfsRequestContext.GetService<ITeamFoundationProcessService>();
      IWorkItemTrackingProcessService service2 = tfsRequestContext.GetService<IWorkItemTrackingProcessService>();
      IVssRequestContext requestContext = tfsRequestContext;
      if (service1.IsProcessEnabled(requestContext))
      {
        foreach (ProjectProcessDescriptorMapping descriptorMapping in (IEnumerable<ProjectProcessDescriptorMapping>) service2.GetProjectProcessDescriptorMappings(tfsRequestContext, expectUnmappedProjects: true, expectInvalidProcessIds: true))
          this.TeamProjects.Add(new TeamProjectModel(tfsRequestContext, descriptorMapping.Project, descriptorMapping.Descriptor));
      }
      else
      {
        foreach (ProjectInfo project in tfsRequestContext.GetService<IProjectService>().GetProjects(tfsRequestContext))
          this.TeamProjects.Add(new TeamProjectModel(tfsRequestContext, project));
      }
      this.TeamProjects.Sort();
    }

    public bool CanCreateProjects { get; internal set; }

    public Guid CollectionId { get; private set; }

    public string DisplayName { get; private set; }

    public string Description { get; private set; }

    public TfsServiceHostDescriptor CollectionServiceHost { get; private set; }

    public string Status
    {
      get
      {
        switch (this.m_status)
        {
          case TeamFoundationServiceHostStatus.Starting:
            return AdminServerResources.Starting;
          case TeamFoundationServiceHostStatus.Started:
            return AdminResources.Online;
          case TeamFoundationServiceHostStatus.Stopping:
            return AdminServerResources.Stopping;
          case TeamFoundationServiceHostStatus.Stopped:
            return AdminServerResources.Stopped;
          case TeamFoundationServiceHostStatus.Pausing:
            return AdminServerResources.Pausing;
          case TeamFoundationServiceHostStatus.Paused:
            return AdminServerResources.Paused;
          default:
            return string.Empty;
        }
      }
    }

    public bool IsOnline => this.m_status == TeamFoundationServiceHostStatus.Started;

    public string StatusReason { get; private set; }

    public List<TeamProjectModel> TeamProjects { get; private set; }
  }
}
