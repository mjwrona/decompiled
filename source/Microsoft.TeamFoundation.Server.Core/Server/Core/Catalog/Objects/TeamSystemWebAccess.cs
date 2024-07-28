// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.Catalog.Objects.TeamSystemWebAccess
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Location;
using System;
using System.ComponentModel;

namespace Microsoft.TeamFoundation.Server.Core.Catalog.Objects
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class TeamSystemWebAccess : CatalogObject
  {
    public static readonly Guid ResourceTypeIdentifier = CatalogResourceTypes.TeamSystemWebAccess;

    public ServiceDefinition HomeService
    {
      get => this.GetServiceReference("Home");
      set => this.SetServiceRefence("Home", value);
    }

    public Uri HomeServiceLocation => this.LocationAsUrl(this.HomeService);

    public ServiceDefinition WorkItemEditorService
    {
      get => this.GetServiceReference("WorkItemEditor");
      set => this.SetServiceRefence("WorkItemEditor", value);
    }

    public Uri WorkItemEditorServiceLocation => this.LocationAsUrl(this.WorkItemEditorService);

    public ServiceDefinition ChangesetDetailService
    {
      get => this.GetServiceReference("ChangesetDetail");
      set => this.SetServiceRefence("ChangesetDetail", value);
    }

    public Uri ChangesetDetailServiceLocation => this.LocationAsUrl(this.ChangesetDetailService);

    public ServiceDefinition DifferenceService
    {
      get => this.GetServiceReference("Difference");
      set => this.SetServiceRefence("Difference", value);
    }

    public Uri DifferenceServiceLocation => this.LocationAsUrl(this.DifferenceService);

    public ServiceDefinition ViewItemService
    {
      get => this.GetServiceReference("ViewItem");
      set => this.SetServiceRefence("ViewItem", value);
    }

    public Uri ViewItemServiceLocation => this.LocationAsUrl(this.ViewItemService);

    public ServiceDefinition ShelvesetDetailService
    {
      get => this.GetServiceReference("ShelvesetDetail");
      set => this.SetServiceRefence("ShelvesetDetail", value);
    }

    public Uri ShelvesetDetailServiceLocation => this.LocationAsUrl(this.ShelvesetDetailService);

    public ServiceDefinition QueryResultsService
    {
      get => this.GetServiceReference("QueryResults");
      set => this.SetServiceRefence("QueryResults", value);
    }

    public Uri QueryResultsServiceLocation => this.LocationAsUrl(this.QueryResultsService);

    public ServiceDefinition AnnotateService
    {
      get => this.GetServiceReference("Annotate");
      set => this.SetServiceRefence("Annotate", value);
    }

    public Uri AnnotateServiceLocation => this.LocationAsUrl(this.AnnotateService);

    public ServiceDefinition SourceExplorerService
    {
      get => this.GetServiceReference("SourceExplorer");
      set => this.SetServiceRefence("SourceExplorer", value);
    }

    public Uri SourceExplorerServiceLocation => this.LocationAsUrl(this.SourceExplorerService);

    public ServiceDefinition OpenWorkItemService
    {
      get => this.GetServiceReference("OpenWorkItem");
      set => this.SetServiceRefence("OpenWorkItem", value);
    }

    public Uri OpenWorkItemServiceLocation => this.LocationAsUrl(this.OpenWorkItemService);

    public ServiceDefinition CreateWorkItemService
    {
      get => this.GetServiceReference("CreateWorkItem");
      set => this.SetServiceRefence("CreateWorkItem", value);
    }

    public Uri CreateWorkItemServiceLocation => this.LocationAsUrl(this.CreateWorkItemService);

    public ServiceDefinition ViewServerQueryResultsService
    {
      get => this.GetServiceReference("ViewServerQueryResults");
      set => this.SetServiceRefence("ViewServerQueryResults", value);
    }

    public Uri ViewServerQueryResultsServiceLocation => this.LocationAsUrl(this.ViewServerQueryResultsService);

    public ServiceDefinition ViewWiqlQueryResultsService
    {
      get => this.GetServiceReference("ViewWiqlQueryResults");
      set => this.SetServiceRefence("ViewWiqlQueryResults", value);
    }

    public Uri ViewWiqlQueryResultsServiceLocation => this.LocationAsUrl(this.ViewWiqlQueryResultsService);

    public ServiceDefinition ExploreSourceControlPathService
    {
      get => this.GetServiceReference("ExploreSourceControlPath");
      set => this.SetServiceRefence("ExploreSourceControlPath", value);
    }

    public Uri ExploreSourceControlPathServiceLocation => this.LocationAsUrl(this.ExploreSourceControlPathService);

    public ServiceDefinition FindShelvesetService
    {
      get => this.GetServiceReference("FindShelveset");
      set => this.SetServiceRefence("FindShelveset", value);
    }

    public Uri FindShelvesetServiceLocation => this.LocationAsUrl(this.FindShelvesetService);

    public ServiceDefinition ViewShelvesetDetailsService
    {
      get => this.GetServiceReference("ViewShelvesetDetails");
      set => this.SetServiceRefence("ViewShelvesetDetails", value);
    }

    public Uri ViewShelvesetDetailsServiceLocation => this.LocationAsUrl(this.ViewShelvesetDetailsService);

    public ServiceDefinition FindChangesetService
    {
      get => this.GetServiceReference("FindChangeset");
      set => this.SetServiceRefence("FindChangeset", value);
    }

    public Uri FindChangesetServiceLocation => this.LocationAsUrl(this.FindChangesetService);

    public ServiceDefinition ViewChangesetDetailsService
    {
      get => this.GetServiceReference("ViewChangesetDetails");
      set => this.SetServiceRefence("ViewChangesetDetails", value);
    }

    public Uri ViewChangesetDetailsServiceLocation => this.LocationAsUrl(this.ViewChangesetDetailsService);

    public ServiceDefinition ViewSourceControlItemService
    {
      get => this.GetServiceReference("ViewSourceControlItem");
      set => this.SetServiceRefence("ViewSourceControlItem", value);
    }

    public Uri ViewSourceControlItemServiceLocation => this.LocationAsUrl(this.ViewSourceControlItemService);

    public ServiceDefinition DownloadSourceControlItemService
    {
      get => this.GetServiceReference("DownloadSourceControlItem");
      set => this.SetServiceRefence("DownloadSourceControlItem", value);
    }

    public Uri DownloadSourceControlItemServiceLocation => this.LocationAsUrl(this.DownloadSourceControlItemService);

    public ServiceDefinition DiffSourceControlItemsService
    {
      get => this.GetServiceReference("DiffSourceControlItems");
      set => this.SetServiceRefence("DiffSourceControlItems", value);
    }

    public Uri DiffSourceControlItemsServiceLocation => this.LocationAsUrl(this.DiffSourceControlItemsService);

    public ServiceDefinition AnnotateSourceControlItemService
    {
      get => this.GetServiceReference("AnnotateSourceControlItem");
      set => this.SetServiceRefence("AnnotateSourceControlItem", value);
    }

    public Uri AnnotateSourceControlItemServiceLocation => this.LocationAsUrl(this.AnnotateSourceControlItemService);

    public ServiceDefinition ViewSourceControlItemHistoryService
    {
      get => this.GetServiceReference("ViewSourceControlItemHistory");
      set => this.SetServiceRefence("ViewSourceControlItemHistory", value);
    }

    public Uri ViewSourceControlItemHistoryServiceLocation => this.LocationAsUrl(this.ViewSourceControlItemHistoryService);

    public ServiceDefinition ViewBuildDetailsService
    {
      get => this.GetServiceReference("ViewBuildDetails");
      set => this.SetServiceRefence("ViewBuildDetails", value);
    }

    public Uri ViewBuildDetailsServiceLocation => this.LocationAsUrl(this.ViewBuildDetailsService);

    public ServiceDefinition ViewSourceControlShelvedItemService
    {
      get => this.GetServiceReference("ViewSourceControlShelvedItem");
      set => this.SetServiceRefence("ViewSourceControlShelvedItem", value);
    }

    public Uri ViewSourceControlShelvedItemServiceLocation => this.LocationAsUrl(this.ViewSourceControlShelvedItemService);

    public ServiceDefinition DiffSourceControlShelvedItemService
    {
      get => this.GetServiceReference("DiffSourceControlShelvedItem");
      set => this.SetServiceRefence("DiffSourceControlShelvedItem", value);
    }

    public Uri DiffSourceControlShelvedItemServiceLocation => this.LocationAsUrl(this.DiffSourceControlShelvedItemService);

    public void ResetServiceDefinitions(IVssRequestContext requestContext)
    {
      this.HomeService = this.GetTswaServiceDefinition(requestContext, "TSWAHome", Microsoft.TeamFoundation.ServiceInterfaces.GuidTswaHome);
      this.WorkItemEditorService = this.GetTswaServiceDefinition(requestContext, "WorkItemEditor", Microsoft.TeamFoundation.ServiceInterfaces.GuidTswaWorkItemEditor);
      this.QueryResultsService = this.GetTswaServiceDefinition(requestContext, "QueryResults", Microsoft.TeamFoundation.ServiceInterfaces.GuidTswaQueryResults);
      this.ChangesetDetailService = this.GetTswaServiceDefinition(requestContext, "ChangesetDetail", Microsoft.TeamFoundation.ServiceInterfaces.GuidTswaChangesetDetail);
      this.ShelvesetDetailService = this.GetTswaServiceDefinition(requestContext, "ShelvesetDetail", Microsoft.TeamFoundation.ServiceInterfaces.GuidTswaShelvesetDetail);
      this.ViewItemService = this.GetTswaServiceDefinition(requestContext, "ViewItem", Microsoft.TeamFoundation.ServiceInterfaces.GuidTswaViewItem);
      this.DifferenceService = this.GetTswaServiceDefinition(requestContext, "Difference", Microsoft.TeamFoundation.ServiceInterfaces.GuidTswaDifference);
      this.AnnotateService = this.GetTswaServiceDefinition(requestContext, "Annotate", Microsoft.TeamFoundation.ServiceInterfaces.GuidTswaAnnotate);
      this.SourceExplorerService = this.GetTswaServiceDefinition(requestContext, "SourceExplorer", Microsoft.TeamFoundation.ServiceInterfaces.GuidTswaSourceExplorer);
      this.OpenWorkItemService = this.GetTswaServiceDefinition(requestContext, "OpenWorkItem", Microsoft.TeamFoundation.ServiceInterfaces.GuidTswaOpenWorkItem);
      this.CreateWorkItemService = this.GetTswaServiceDefinition(requestContext, "CreateWorkItem", Microsoft.TeamFoundation.ServiceInterfaces.GuidTswaCreateWorkItem);
      this.ViewServerQueryResultsService = this.GetTswaServiceDefinition(requestContext, "ViewServerQueryResults", Microsoft.TeamFoundation.ServiceInterfaces.GuidTswaViewServerQueryResults);
      this.ViewWiqlQueryResultsService = this.GetTswaServiceDefinition(requestContext, "ViewWiqlQueryResults", Microsoft.TeamFoundation.ServiceInterfaces.GuidTswaHome);
      this.ExploreSourceControlPathService = this.GetTswaServiceDefinition(requestContext, "ExploreSourceControlPath", Microsoft.TeamFoundation.ServiceInterfaces.GuidTswaExploreSourceControlPath);
      this.ViewShelvesetDetailsService = this.GetTswaServiceDefinition(requestContext, "ViewShelvesetDetails", Microsoft.TeamFoundation.ServiceInterfaces.GuidTswaViewShelvesetDetails);
      this.ViewChangesetDetailsService = this.GetTswaServiceDefinition(requestContext, "ViewChangesetDetails", Microsoft.TeamFoundation.ServiceInterfaces.GuidTswaViewChangesetDetails);
      this.ViewSourceControlItemService = this.GetTswaServiceDefinition(requestContext, "ViewSourceControlItem", Microsoft.TeamFoundation.ServiceInterfaces.GuidTswaViewSourceControlItem);
      this.DiffSourceControlItemsService = this.GetTswaServiceDefinition(requestContext, "DiffSourceControlItems", Microsoft.TeamFoundation.ServiceInterfaces.GuidTswaDiffSourceControlItems);
      this.AnnotateSourceControlItemService = this.GetTswaServiceDefinition(requestContext, "AnnotateSourceControlItem", Microsoft.TeamFoundation.ServiceInterfaces.GuidTswaAnnotateSourceControlItem);
      this.ViewSourceControlItemHistoryService = this.GetTswaServiceDefinition(requestContext, "ViewSourceControlItemHistory", Microsoft.TeamFoundation.ServiceInterfaces.GuidTswaViewSourceControlItemHistory);
      this.ViewBuildDetailsService = this.GetTswaServiceDefinition(requestContext, "ViewBuildDetails", Microsoft.TeamFoundation.ServiceInterfaces.GuidTswaViewBuildDetails);
      this.ViewSourceControlShelvedItemService = this.GetTswaServiceDefinition(requestContext, "ViewSourceControlShelvedItem", Microsoft.TeamFoundation.ServiceInterfaces.GuidTswaViewSourceControlShelvedItem);
      this.DiffSourceControlShelvedItemService = this.GetTswaServiceDefinition(requestContext, "DiffSourceControlShelvedItem", Microsoft.TeamFoundation.ServiceInterfaces.GuidTswaDiffSourceControlShelvedItem);
    }

    private ServiceDefinition GetTswaServiceDefinition(
      IVssRequestContext requestContext,
      string serviceType,
      Guid serviceGuid)
    {
      return this.Context.GetLocationService(requestContext).FindServiceDefinition(requestContext, serviceType, serviceGuid);
    }

    public static class Fields
    {
      public const string Home = "Home";
      public const string WorkItemEditor = "WorkItemEditor";
      public const string ChangesetDetail = "ChangesetDetail";
      public const string Difference = "Difference";
      public const string ViewItem = "ViewItem";
      public const string ShelvesetDetail = "ShelvesetDetail";
      public const string QueryResults = "QueryResults";
      public const string Annotate = "Annotate";
      public const string SourceExplorer = "SourceExplorer";
      public const string OpenWorkItem = "OpenWorkItem";
      public const string CreateWorkItem = "CreateWorkItem";
      public const string ViewServerQueryResults = "ViewServerQueryResults";
      public const string ViewWiqlQueryResults = "ViewWiqlQueryResults";
      public const string ExploreSourceControlPath = "ExploreSourceControlPath";
      public const string FindShelveset = "FindShelveset";
      public const string ViewShelvesetDetails = "ViewShelvesetDetails";
      public const string FindChangeset = "FindChangeset";
      public const string ViewChangesetDetails = "ViewChangesetDetails";
      public const string ViewSourceControlItem = "ViewSourceControlItem";
      public const string DownloadSourceControlItem = "DownloadSourceControlItem";
      public const string DiffSourceControlItems = "DiffSourceControlItems";
      public const string AnnotateSourceControlItem = "AnnotateSourceControlItem";
      public const string ViewSourceControlItemHistory = "ViewSourceControlItemHistory";
      public const string ViewBuildDetails = "ViewBuildDetails";
      public const string ViewSourceControlShelvedItem = "ViewSourceControlShelvedItem";
      public const string DiffSourceControlShelvedItem = "DiffSourceControlShelvedItem";
    }
  }
}
