// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Dashboards.Services.DashboardService
// Assembly: Microsoft.TeamFoundation.Dashboards.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CC7F149F-95E9-4579-9C7B-BAEEA5A10ECA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Dashboards.Server.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Dashboards.DataAccess;
using Microsoft.TeamFoundation.Dashboards.Model;
using Microsoft.TeamFoundation.Dashboards.Security;
using Microsoft.TeamFoundation.Dashboards.Telemetry;
using Microsoft.TeamFoundation.Dashboards.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.TeamFoundation.Server.Types.Team;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Dashboards.Services
{
  public class DashboardService : IDashboardService, IVssFrameworkService
  {
    public const string AutoRefreshIntervalRegistryPath = "/Service/Dashboards/AutoRefreshInterval";
    private const string c_GetDashboardsKeyFromRequestContext = "Dashboards-GetDashboards";

    public DashboardService()
      : this((IDashboardSecurityManager) new Microsoft.TeamFoundation.Dashboards.DashboardSecurityManager(), new CopyWidgetAdapterFactory())
    {
    }

    public DashboardService(
      IDashboardSecurityManager securityManager,
      CopyWidgetAdapterFactory factory)
    {
      this.DashboardSecurityManager = securityManager;
      this.m_CopyWidgetAdapterFactory = factory;
    }

    private IDashboardSecurityManager DashboardSecurityManager { get; }

    private CopyWidgetAdapterFactory m_CopyWidgetAdapterFactory { get; }

    private IEnumerable<Widget> GetWidgetsByDashboardIdFromSqlComponent(
      IVssRequestContext requestContext,
      Guid dataspaceId,
      Guid dashboardId)
    {
      using (WidgetSqlResourceComponent component = requestContext.CreateComponent<WidgetSqlResourceComponent>())
        return component.GetWidgetsByDashboardId(dataspaceId, dashboardId);
    }

    private DashboardGroupEntry GetDashboardByDashboardIdFromSqlComponent(
      IVssRequestContext requestContext,
      Guid dataspaceId,
      Guid groupId,
      DashboardScope scope,
      Guid dashboardId)
    {
      using (DashboardGroupSqlResourceComponent component = requestContext.CreateComponent<DashboardGroupSqlResourceComponent>())
        return component.GetDashboardById(dataspaceId, groupId, scope, dashboardId) ?? throw new DashboardDoesNotExistException(dashboardId);
    }

    private void ValidateIfProjectAndTeamExists(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid teamId,
      out WebApiTeam defaultOrDestinationTeam)
    {
      if (teamId == Guid.Empty)
      {
        ITeamService teamService = requestContext.GetService<IProjectService>().TryGetProject(requestContext, projectId, out ProjectInfo _) ? requestContext.GetService<ITeamService>() : throw new ProjectDoesNotExistException(projectId.ToString());
        defaultOrDestinationTeam = teamService.GetDefaultTeam(requestContext, projectId);
      }
      else
        defaultOrDestinationTeam = requestContext.GetService<ITeamService>().GetTeamInProject(requestContext, projectId, teamId.ToString()) ?? throw new TeamNotFoundException(teamId.ToString());
    }

    internal virtual void ValidateDashboard(IVssRequestContext requestContext, Dashboard dashboard)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<Dashboard>(dashboard, nameof (dashboard));
      using (TelemetryCollector.TraceMonitor(requestContext, 10017056, nameof (DashboardService), "DashboardService.ValidateDashboard"))
        dashboard.Validate();
    }

    internal virtual void ValidateWidget(IVssRequestContext requestContext, Widget widget)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<Widget>(widget, nameof (widget));
      using (TelemetryCollector.TraceMonitor(requestContext, 10017378, "WidgetService", "DashboardService.ValidateWidget"))
        widget.Validate(requestContext);
    }

    internal virtual IEnumerable<WidgetDataModel> DetectGridCollision(
      IVssRequestContext requestContext,
      IDashboardConsumer dashboardConsumer,
      Dashboard dashboard,
      UpdateWidgetsBehavior behavior)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<IDashboardConsumer>(dashboardConsumer, nameof (dashboardConsumer));
      ArgumentUtility.CheckForNull<Dashboard>(dashboard, nameof (dashboard));
      ArgumentUtility.CheckForNull<IEnumerable<Widget>>(dashboard.Widgets, "Widgets");
      using (TelemetryCollector.TraceMonitor(requestContext, 10017380, "WidgetService", "DashboardService.DetectGridCollision"))
      {
        IEnumerable<Widget> widgets = dashboard.Widgets;
        List<WidgetDataModel> list;
        if (behavior == UpdateWidgetsBehavior.Replace)
        {
          list = dashboard.Widgets.Select<Widget, WidgetDataModel>((Func<Widget, WidgetDataModel>) (x => new WidgetDataModel(dashboard.Id.Value, x))).ToList<WidgetDataModel>();
        }
        else
        {
          DashboardWidgetsDataModel widgetsForDashboard;
          using (WidgetSqlResourceComponent component = requestContext.CreateComponent<WidgetSqlResourceComponent>())
            widgetsForDashboard = component.GetWidgetsForDashboard(dashboardConsumer.GetDataspaceId(), dashboard.Id.Value);
          if (widgetsForDashboard == null)
            throw new DashboardsDoNotExistException(dashboard.Id.Value.ToString());
          if (widgetsForDashboard.ETag != null && widgetsForDashboard.ETag != dashboard.ETag)
            throw new WidgetETagConflictException();
          list = widgetsForDashboard.Widgets.ToList<WidgetDataModel>();
          List<WidgetDataModel> collection = new List<WidgetDataModel>();
          foreach (Widget widget in widgets)
          {
            Widget newwidget = widget;
            int index = list.FindIndex((Predicate<WidgetDataModel>) (w =>
            {
              Guid? id1 = w.Id;
              Guid? id2 = newwidget.Id;
              if (id1.HasValue != id2.HasValue)
                return false;
              return !id1.HasValue || id1.GetValueOrDefault() == id2.GetValueOrDefault();
            }));
            if (index >= 0)
              list[index] = new WidgetDataModel(dashboard.Id.Value, newwidget);
            else
              collection.Add(new WidgetDataModel(dashboard.Id.Value, newwidget));
          }
          list.AddRange((IEnumerable<WidgetDataModel>) collection);
        }
        this.DetectWidgetCollisions((IEnumerable<WidgetDataModel>) list);
        return (IEnumerable<WidgetDataModel>) list;
      }
    }

    public void DetectWidgetCollisions(IEnumerable<WidgetDataModel> widgets)
    {
      if (widgets.Count<WidgetDataModel>() <= 0)
        return;
      string[,] strArray = new string[widgets.Max<WidgetDataModel>((Func<WidgetDataModel, int>) (w => w.Position.Row + w.Size.RowSpan)) + 1, widgets.Max<WidgetDataModel>((Func<WidgetDataModel, int>) (w => w.Position.Column + w.Size.ColumnSpan)) + 1];
      foreach (Widget widget in widgets)
      {
        if (widget.IsPositioned())
        {
          for (int row = widget.Position.Row; row < widget.Position.Row + widget.Size.RowSpan; ++row)
          {
            for (int column = widget.Position.Column; column < widget.Position.Column + widget.Size.ColumnSpan; ++column)
            {
              if (strArray[row, column] != null)
                throw new WidgetCollisionException();
              strArray[row, column] = widget.Name;
            }
          }
        }
      }
    }

    private int GetAutoRefreshInterval(
      IVssRequestContext requestContext,
      string refreshIntervalRegistryPath,
      int defaultValue)
    {
      CachedRegistryService service = requestContext.GetService<CachedRegistryService>();
      return service == null ? defaultValue : service.GetValue<int>(requestContext, (RegistryQuery) refreshIntervalRegistryPath, true, defaultValue);
    }

    private void UpdateRefreshIntervalFromRegistry(
      IVssRequestContext requestContext,
      Dashboard entry)
    {
      if (!entry.RefreshInterval.HasValue)
        return;
      int? refreshInterval = entry.RefreshInterval;
      int num = 0;
      if (!(refreshInterval.GetValueOrDefault() > num & refreshInterval.HasValue))
        return;
      refreshInterval = entry.RefreshInterval;
      int valueOrDefault = refreshInterval.GetValueOrDefault();
      int autoRefreshInterval = this.GetAutoRefreshInterval(requestContext, "/Service/Dashboards/AutoRefreshInterval", valueOrDefault);
      if (autoRefreshInterval > valueOrDefault)
      {
        entry.RefreshInterval = new int?(Math.Max(autoRefreshInterval, valueOrDefault));
      }
      else
      {
        if (autoRefreshInterval == 0 || autoRefreshInterval >= valueOrDefault)
          return;
        entry.RefreshInterval = new int?(Math.Min(autoRefreshInterval, valueOrDefault));
      }
    }

    private bool GetMemberClaim(IVssRequestContext requestContext) => requestContext.GetService<IContributionClaimService>().HasClaim(requestContext, "Member");

    public Dashboard AddDashboard(
      IVssRequestContext requestContext,
      IDashboardConsumer dashboardConsumer,
      Dashboard dashboard)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<IDashboardConsumer>(dashboardConsumer, nameof (dashboardConsumer));
      ArgumentUtility.CheckForNull<Dashboard>(dashboard, nameof (dashboard));
      using (TelemetryCollector.TraceMonitor(requestContext, 10017050, nameof (DashboardService), "DashboardService.AddDashboard"))
      {
        this.DashboardSecurityManager.CheckCreatePermission(requestContext, dashboardConsumer.GetDataspaceId(), dashboardConsumer.GetGroupId());
        if (!dashboard.IsNew())
          throw new CreateDashboardWithExistingIdException();
        List<DashboardGroupEntry> dashboardsByGroupId;
        using (DashboardGroupSqlResourceComponent component = requestContext.CreateComponent<DashboardGroupSqlResourceComponent>())
          dashboardsByGroupId = component.GetDashboardsByGroupId(dashboardConsumer.GetDataspaceId(), dashboardConsumer.GetGroupId(), dashboardConsumer.GetScope());
        int dashboardsPerGroup = DashboardSettings.GetMaxDashboardsPerGroup(requestContext);
        int count = dashboardsByGroupId.Count;
        if (count + 1 > dashboardsPerGroup)
          throw new DashboardCountLimitExceededException(dashboardsPerGroup, count, count + 1);
        if (dashboard.Position < 1)
          dashboard.Position = dashboardsByGroupId.Count == 0 ? 1 : dashboardsByGroupId.Select<DashboardGroupEntry, int>((Func<DashboardGroupEntry, int>) (x => x.Position)).Max() + 1;
        if (dashboard.Name != null)
          dashboard.Name = dashboard.Name.Trim();
        this.ValidateDashboard(requestContext, dashboard);
        Microsoft.VisualStudio.Services.Identity.Identity userIdentity = requestContext.GetUserIdentity();
        if (dashboardConsumer.GetScope() == DashboardScope.Project_Team)
        {
          dashboard.ValidateNameIsUnique(requestContext, (IEnumerable<Dashboard>) dashboardsByGroupId);
          dashboard.OwnerId = dashboardConsumer.GetGroupId();
        }
        else
          dashboard.OwnerId = userIdentity.Id;
        dashboard.ModifiedBy = userIdentity.Id;
        dashboard.Id = new Guid?(Guid.NewGuid());
        dashboard.ETag = "1";
        dashboard.LastAccessedDate = DateTime.UtcNow.Date;
        dashboard.ModifiedDate = DateTime.UtcNow;
        DashboardGroupEntryDataModel2 entry = new DashboardGroupEntryDataModel2(dashboardConsumer.GetGroupId(), dashboardConsumer.GetScope(), dashboard);
        DashboardGroupEntry dashboardGroupEntry;
        using (DashboardGroupSqlResourceComponent component = requestContext.CreateComponent<DashboardGroupSqlResourceComponent>())
          dashboardGroupEntry = component.AddDashboard(dashboardConsumer.GetDataspaceId(), entry);
        dashboard.ETag = dashboardGroupEntry.ETag;
        if (dashboardConsumer.GetScope() == DashboardScope.Project)
        {
          IDashboardSecurityManager dashboardSecurityManager = this.DashboardSecurityManager;
          IVssRequestContext requestContext1 = requestContext;
          Guid dataspaceId = dashboardConsumer.GetDataspaceId();
          Guid? nullable = dashboard.Id;
          Guid dashboardId = nullable.Value;
          nullable = new Guid?();
          Guid? ownerId = nullable;
          dashboardSecurityManager.SetDashboardAllPermissions(requestContext1, dataspaceId, dashboardId, ownerId);
        }
        if (dashboard.Widgets != null)
          dashboard = this.UpdateDashboardWidgets(requestContext, dashboardConsumer, dashboard);
        this.UpdateRefreshIntervalFromRegistry(requestContext, dashboard);
        return dashboard;
      }
    }

    public List<Dashboard> GetDashboards(
      IVssRequestContext requestContext,
      Guid projectId,
      IEnumerable<Guid> ids = null)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      using (TelemetryCollector.TraceMonitor(requestContext, 100170063, nameof (DashboardService), "DashboardService.GetDashboards"))
      {
        ProjectInfo projectInfo;
        if (!requestContext.GetService<IProjectService>().TryGetProject(requestContext, projectId, out projectInfo))
          throw new ProjectDoesNotExistException(projectId.ToString());
        List<Dashboard> dashboards = new List<Dashboard>();
        if (ids == null)
        {
          using (DashboardGroupSqlResourceComponent component = requestContext.CreateComponent<DashboardGroupSqlResourceComponent>())
            component.GetDashboardsByProjectId(projectId).ForEach((Action<DashboardGroupEntry>) (entry => dashboards.Add((Dashboard) new DashboardGroupEntry((Dashboard) entry))));
          if (this.DashboardSecurityManager.HasMaterializeDashboardsPermission(requestContext))
          {
            this.MaterializeUnvisitedDashboards(requestContext, projectInfo, dashboards);
            if (!dashboards.Any<Dashboard>() && this.IsProjectDashboardsEnabled(requestContext))
              this.DashboardSecurityManager.SetProjectDashboardsCreatePermission(requestContext, projectId);
          }
        }
        else
        {
          using (DashboardGroupSqlResourceComponent component = requestContext.CreateComponent<DashboardGroupSqlResourceComponent>())
            component.GetDashboardsByIds(projectId, ids).ForEach((Action<DashboardGroupEntry>) (entry => dashboards.Add((Dashboard) entry)));
        }
        List<Dashboard> accessibleDashboards = this.GetAccessibleDashboards<Dashboard>(requestContext, projectId, (IEnumerable<Dashboard>) dashboards);
        foreach (Dashboard entry in accessibleDashboards)
          this.UpdateRefreshIntervalFromRegistry(requestContext, entry);
        return accessibleDashboards;
      }
    }

    private void MaterializeUnvisitedDashboards(
      IVssRequestContext requestContext,
      ProjectInfo projectInfo,
      List<Dashboard> dashboards)
    {
      foreach (WebApiTeam webApiTeam in (IEnumerable<WebApiTeam>) requestContext.GetService<ITeamService>().QueryTeamsInProject(requestContext.Elevate(), projectInfo.Id))
      {
        WebApiTeam team = webApiTeam;
        if (dashboards.Find((Predicate<Dashboard>) (dashboard => dashboard.OwnerId == team.Id)) == null)
          this.GetDashboardGroup(requestContext, (IDashboardConsumer) new TeamDashboardConsumer(projectInfo.Id, team.Id)).DashboardEntries.ForEach<DashboardGroupEntry>((Action<DashboardGroupEntry>) (entry => dashboards.Add((Dashboard) entry)));
      }
    }

    public DashboardGroup GetDashboardGroup(
      IVssRequestContext requestContext,
      IDashboardConsumer dashboardConsumer)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<IDashboardConsumer>(dashboardConsumer, nameof (dashboardConsumer));
      using (TelemetryCollector.TraceMonitor(requestContext, 10017058, nameof (DashboardService), "DashboardService.GetDashboards"))
      {
        ISecuredObject securedObject = DashboardSecuredObjectFactory.GenerateSecuredObject(requestContext, this.DashboardSecurityManager, dashboardConsumer.GetDataspaceId(), dashboardConsumer.GetGroupId(), new Guid?());
        Guid? nullable = new Guid?();
        DashboardScope scope = dashboardConsumer.GetScope();
        IEnumerable<DashboardGroupEntry> dashboardGroupEntries;
        using (DashboardGroupSqlResourceComponent component = requestContext.CreateComponent<DashboardGroupSqlResourceComponent>())
          dashboardGroupEntries = (IEnumerable<DashboardGroupEntry>) component.GetDashboardsByGroupId(dashboardConsumer.GetDataspaceId(), dashboardConsumer.GetGroupId(), scope);
        if (!dashboardGroupEntries.Any<DashboardGroupEntry>() && this.DashboardSecurityManager.HasMaterializeDashboardsPermission(requestContext))
        {
          if (scope == DashboardScope.Project_Team)
          {
            DashboardGroupEntry entry1 = DefaultDashboardFactory.CreateDashboard(scope);
            entry1.ModifiedDate = DateTime.UtcNow;
            entry1.ModifiedBy = requestContext.GetUserIdentity().Id;
            entry1.LastAccessedDate = DateTime.UtcNow.Date;
            DashboardGroupEntryDataModel2 entry2 = new DashboardGroupEntryDataModel2(dashboardConsumer.GetGroupId(), scope, entry1);
            using (DashboardGroupSqlResourceComponent component = requestContext.CreateComponent<DashboardGroupSqlResourceComponent>())
              entry1 = component.AddDashboard(dashboardConsumer.GetDataspaceId(), entry2);
            dashboardGroupEntries = (IEnumerable<DashboardGroupEntry>) new List<DashboardGroupEntry>()
            {
              entry1
            };
            nullable = new Guid?(entry1.Id.Value);
          }
          if (this.IsProjectDashboardsEnabled(requestContext))
            this.DashboardSecurityManager.SetProjectDashboardsCreatePermission(requestContext, dashboardConsumer.GetDataspaceId());
        }
        if (nullable.HasValue)
          this.DashboardSecurityManager.SetDefaultDashboardWithManageGroupPermission(requestContext, dashboardConsumer.GetDataspaceId(), dashboardConsumer.GetGroupId());
        List<DashboardGroupEntry> accessibleDashboards = this.GetAccessibleDashboards<DashboardGroupEntry>(requestContext, dashboardConsumer.GetDataspaceId(), dashboardGroupEntries);
        foreach (DashboardGroupEntry entry in accessibleDashboards)
          this.UpdateRefreshIntervalFromRegistry(requestContext, (Dashboard) entry);
        DashboardGroup dashboardGroup = new DashboardGroup()
        {
          DashboardEntries = (IEnumerable<DashboardGroupEntry>) accessibleDashboards
        };
        if (dashboardConsumer.GetGroupId() != Guid.Empty)
        {
          int groupPermissions = this.DashboardSecurityManager.GetTeamMemberGroupPermissions(requestContext, dashboardConsumer.GetDataspaceId(), dashboardConsumer.GetGroupId());
          dashboardGroup.TeamDashboardPermission = DashboardsPrivileges.NamespaceToUserPermission(groupPermissions);
        }
        dashboardGroup.SetSecuredObject(securedObject);
        return dashboardGroup;
      }
    }

    public Dashboard UpdateDashboard(
      IVssRequestContext requestContext,
      IDashboardConsumer dashboardConsumer,
      Dashboard dashboard)
    {
      if (dashboard == null)
        throw new DashboardDoesNotExistException();
      DashboardGroup dashboardGroup = this.GetDashboardGroup(requestContext, dashboardConsumer);
      Dashboard dashboard1 = (Dashboard) dashboardGroup.DashboardEntries.FirstOrDefault<DashboardGroupEntry>((Func<DashboardGroupEntry, bool>) (o =>
      {
        Guid? id1 = o.Id;
        Guid? id2 = dashboard.Id;
        if (id1.HasValue != id2.HasValue)
          return false;
        return !id1.HasValue || id1.GetValueOrDefault() == id2.GetValueOrDefault();
      }));
      if (dashboard1 == null)
        throw new DashboardDoesNotExistException(dashboard.Id.Value);
      bool enforceUniqueNames = dashboardConsumer != null && dashboardConsumer.GetScope() == DashboardScope.Project_Team && !Convert.ToBoolean((object) dashboard?.Name?.Equals(dashboard1?.Name, StringComparison.InvariantCulture));
      dashboard1.Name = dashboard.Name;
      dashboard1.Description = dashboard.Description;
      dashboard1.RefreshInterval = dashboard.RefreshInterval;
      if (dashboardConsumer.GetScope() == DashboardScope.Project && dashboard1.OwnerId != dashboard.OwnerId && this.CanUserManagePermissions(requestContext, dashboardConsumer, dashboard))
      {
        dashboard1.OwnerId = dashboard.OwnerId;
        this.DashboardSecurityManager.SetDashboardAllPermissions(requestContext, dashboardConsumer.GetDataspaceId(), dashboard.Id.Value, new Guid?(dashboard.OwnerId));
      }
      Dashboard dashboard2 = (Dashboard) this.UpdateDashboards(requestContext, dashboardConsumer, dashboardGroup, enforceUniqueNames).DashboardEntries.First<DashboardGroupEntry>((Func<DashboardGroupEntry, bool>) (o =>
      {
        Guid? id3 = o.Id;
        Guid? id4 = dashboard.Id;
        if (id3.HasValue != id4.HasValue)
          return false;
        return !id3.HasValue || id3.GetValueOrDefault() == id4.GetValueOrDefault();
      }));
      if (dashboard.Widgets != null)
        dashboard2 = this.ReplaceDashboardWidgets(requestContext, dashboardConsumer, dashboard);
      return dashboard2;
    }

    public DashboardGroup UpdateDashboards(
      IVssRequestContext requestContext,
      IDashboardConsumer dashboardConsumer,
      DashboardGroup dashboardGroup,
      bool enforceUniqueNames = true)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<IDashboardConsumer>(dashboardConsumer, nameof (dashboardConsumer));
      ArgumentUtility.CheckForNull<DashboardGroup>(dashboardGroup, nameof (dashboardGroup));
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) dashboardGroup.DashboardEntries, nameof (dashboardGroup));
      using (TelemetryCollector.TraceMonitor(requestContext, 10017052, nameof (DashboardService), "DashboardService.UpdateDashboards"))
      {
        List<DashboardGroupEntry> list1 = dashboardGroup.DashboardEntries.ToList<DashboardGroupEntry>();
        int dashboardsPerGroup = DashboardSettings.GetMaxDashboardsPerGroup(requestContext);
        List<DashboardGroupEntry> list2 = list1.Where<DashboardGroupEntry>((Func<DashboardGroupEntry, bool>) (e => !e.IsNew())).ToList<DashboardGroupEntry>();
        IEnumerable<DashboardGroupEntry> dashboardsByGroupId;
        using (DashboardGroupSqlResourceComponent component = requestContext.CreateComponent<DashboardGroupSqlResourceComponent>())
          dashboardsByGroupId = (IEnumerable<DashboardGroupEntry>) component.GetDashboardsByGroupId(dashboardConsumer.GetDataspaceId(), dashboardConsumer.GetGroupId(), dashboardConsumer.GetScope());
        List<DashboardGroupEntry> list3 = list2.Except<DashboardGroupEntry>(dashboardsByGroupId, (IEqualityComparer<DashboardGroupEntry>) new DashboardGroupEntryComparer()).ToList<DashboardGroupEntry>();
        if (list3.Any<DashboardGroupEntry>())
          throw new DashboardsDoNotExistException(list3.Select<DashboardGroupEntry, string>((Func<DashboardGroupEntry, string>) (e => string.Format("{0} : {1}", (object) e.Id.ToString(), (object) e.Name))).Aggregate<string>((Func<string, string, string>) ((i, j) => i + ", " + j)));
        int requested = list1.Select<DashboardGroupEntry, int>((Func<DashboardGroupEntry, int>) (e => e.Position)).Count<int>();
        if (requested > dashboardsPerGroup)
          throw new DashboardCountLimitExceededException(dashboardsPerGroup, dashboardsByGroupId.Count<DashboardGroupEntry>(), requested);
        if (enforceUniqueNames)
          DashboardExtension.ValidateAllNamesAreUnique((IEnumerable<Dashboard>) list1);
        foreach (DashboardGroupEntry dashboardGroupEntry in dashboardsByGroupId.Except<DashboardGroupEntry>((IEnumerable<DashboardGroupEntry>) list1, (IEqualityComparer<DashboardGroupEntry>) new DashboardGroupEntryComparer()))
          this.DashboardSecurityManager.CheckDeletePermission(requestContext, dashboardConsumer.GetDataspaceId(), dashboardConsumer.GetGroupId(), dashboardGroupEntry.Id);
        List<DashboardGroupEntryDataModel2> groupEntryDataModel2List = new List<DashboardGroupEntryDataModel2>();
        foreach (DashboardGroupEntry dashboardGroupEntry in list1)
        {
          DashboardGroupEntry d = dashboardGroupEntry;
          this.ValidateDashboard(requestContext, (Dashboard) d);
          if (d.IsNew())
          {
            this.DashboardSecurityManager.CheckCreatePermission(requestContext, dashboardConsumer.GetDataspaceId(), dashboardConsumer.GetGroupId());
            d.Id = new Guid?(Guid.NewGuid());
            d.ETag = "1";
          }
          else
          {
            DashboardGroupEntry existing = dashboardsByGroupId.Single<DashboardGroupEntry>((Func<DashboardGroupEntry, bool>) (e =>
            {
              Guid? id1 = e.Id;
              Guid? id2 = d.Id;
              if (id1.HasValue != id2.HasValue)
                return false;
              return !id1.HasValue || id1.GetValueOrDefault() == id2.GetValueOrDefault();
            }));
            if (DashboardService.ChangesArePresent(existing, d))
            {
              this.DashboardSecurityManager.CheckEditPermission(requestContext, dashboardConsumer.GetDataspaceId(), dashboardConsumer.GetGroupId(), existing.Id);
              d.LastAccessedDate = DateTime.UtcNow.Date;
              d.ModifiedDate = DateTime.UtcNow;
              d.ModifiedBy = requestContext.GetUserIdentity().Id;
            }
          }
          this.UpdateRefreshIntervalFromRegistry(requestContext, (Dashboard) d);
          groupEntryDataModel2List.Add(new DashboardGroupEntryDataModel2(dashboardConsumer.GetGroupId(), dashboardConsumer.GetScope(), d));
        }
        using (DashboardGroupSqlResourceComponent component = requestContext.CreateComponent<DashboardGroupSqlResourceComponent>())
          component.UpdateDashboards(dashboardConsumer.GetDataspaceId(), dashboardConsumer.GetGroupId(), dashboardConsumer.GetScope(), (IEnumerable<DashboardGroupEntryDataModel2>) groupEntryDataModel2List.ToArray());
        return new DashboardGroup()
        {
          DashboardEntries = (IEnumerable<DashboardGroupEntry>) groupEntryDataModel2List
        };
      }
    }

    private List<T> GetAccessibleDashboards<T>(
      IVssRequestContext requestContext,
      Guid projectId,
      IEnumerable<T> entries)
      where T : Dashboard
    {
      List<T> accessibleDashboards = new List<T>();
      foreach (T entry in entries)
      {
        ISecuredObject securedDashboardObject;
        if (DashboardSecuredObjectFactory.TryGenerateSecuredObject(requestContext, this.DashboardSecurityManager, projectId, entry.OwnerId, entry.Id, out securedDashboardObject))
        {
          entry.SetSecuredObject(securedDashboardObject);
          accessibleDashboards.Add(entry);
        }
      }
      return accessibleDashboards;
    }

    private static bool ChangesArePresent(DashboardGroupEntry existing, DashboardGroupEntry @new)
    {
      int num1;
      if ((false ? 1 : (existing.Name != @new.Name ? 1 : 0)) == 0)
      {
        int? refreshInterval = existing.RefreshInterval;
        int num2 = 0;
        if (refreshInterval.GetValueOrDefault() == num2 & refreshInterval.HasValue)
        {
          refreshInterval = @new.RefreshInterval;
          int num3 = 0;
          if (refreshInterval.GetValueOrDefault() > num3 & refreshInterval.HasValue)
          {
            num1 = 1;
            goto label_8;
          }
        }
        refreshInterval = existing.RefreshInterval;
        int num4 = 0;
        if (refreshInterval.GetValueOrDefault() > num4 & refreshInterval.HasValue)
        {
          refreshInterval = @new.RefreshInterval;
          int num5 = 0;
          num1 = refreshInterval.GetValueOrDefault() == num5 & refreshInterval.HasValue ? 1 : 0;
        }
        else
          num1 = 0;
      }
      else
        num1 = 1;
label_8:
      return num1 != 0 || existing.Description != @new.Description;
    }

    private bool CanUserManagePermissions(
      IVssRequestContext requestContext,
      IDashboardConsumer dashboardConsumer,
      Dashboard dashboard)
    {
      return (this.DashboardSecurityManager.GetEffectivePermissions(requestContext, dashboardConsumer.GetDataspaceId(), dashboardConsumer.GetGroupId(), dashboard.Id) & 16) != 0;
    }

    private bool IsProjectDashboardsEnabled(IVssRequestContext requestContext) => requestContext.IsFeatureEnabled("WebAccess.Dashboards.TeamAgnosticDashboards");

    public void DeleteDashboard(
      IVssRequestContext requestContext,
      IDashboardConsumer dashboardConsumer,
      Guid dashboardId)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<IDashboardConsumer>(dashboardConsumer, nameof (dashboardConsumer));
      ArgumentUtility.CheckForEmptyGuid(dashboardId, nameof (dashboardId));
      using (TelemetryCollector.TraceMonitor(requestContext, 10017054, nameof (DashboardService), "DashboardService.DeleteDashboard"))
      {
        this.DashboardSecurityManager.CheckDeletePermission(requestContext, dashboardConsumer.GetDataspaceId(), dashboardConsumer.GetGroupId(), new Guid?(dashboardId));
        using (DashboardGroupSqlResourceComponent component = requestContext.CreateComponent<DashboardGroupSqlResourceComponent>())
        {
          DashboardScope scope = dashboardConsumer.GetScope();
          List<DashboardGroupEntry> dashboardsByGroupId = component.GetDashboardsByGroupId(dashboardConsumer.GetDataspaceId(), dashboardConsumer.GetGroupId(), scope);
          if (!dashboardsByGroupId.Where<DashboardGroupEntry>((Func<DashboardGroupEntry, bool>) (d =>
          {
            Guid? id = d.Id;
            Guid guid = dashboardId;
            if (!id.HasValue)
              return false;
            return !id.HasValue || id.GetValueOrDefault() == guid;
          })).Any<DashboardGroupEntry>())
            throw new DashboardDoesNotExistException(dashboardId);
          if (scope == DashboardScope.Project_Team && dashboardsByGroupId.Count == 1)
            throw new LastDashboardCannotBeDeletedException();
          component.DeleteDashboard(dashboardConsumer.GetDataspaceId(), dashboardConsumer.GetGroupId(), dashboardConsumer.GetScope(), dashboardId);
        }
      }
    }

    public CopyDashboardResponse CopyDashboard(
      IVssRequestContext requestContext,
      Guid sourceDashboardId,
      IDashboardConsumer sourceDashboardConsumer,
      IDashboardConsumer targetDashboardConsumer,
      CopyDashboardOptions options)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<IDashboardConsumer>(sourceDashboardConsumer, nameof (sourceDashboardConsumer));
      ArgumentUtility.CheckForNull<IDashboardConsumer>(targetDashboardConsumer, nameof (targetDashboardConsumer));
      using (TelemetryCollector.TraceMonitor(requestContext, 100170065, nameof (DashboardService), "DashboardService.CopyDashboard"))
      {
        WebApiTeam defaultOrDestinationTeam1;
        this.ValidateIfProjectAndTeamExists(requestContext, targetDashboardConsumer.GetDataspaceId(), targetDashboardConsumer.GetGroupId(), out defaultOrDestinationTeam1);
        this.DashboardSecurityManager.CheckCreatePermission(requestContext, targetDashboardConsumer.GetDataspaceId(), targetDashboardConsumer.GetGroupId());
        if (targetDashboardConsumer.GetScope() == DashboardScope.Project_Team)
          this.DashboardSecurityManager.CheckEditPermission(requestContext, targetDashboardConsumer.GetDataspaceId(), targetDashboardConsumer.GetGroupId(), new Guid?());
        List<DashboardGroupEntry> dashboardsByGroupId;
        using (DashboardGroupSqlResourceComponent component = requestContext.CreateComponent<DashboardGroupSqlResourceComponent>())
          dashboardsByGroupId = component.GetDashboardsByGroupId(targetDashboardConsumer.GetDataspaceId(), targetDashboardConsumer.GetGroupId(), targetDashboardConsumer.GetScope());
        int dashboardsPerGroup = DashboardSettings.GetMaxDashboardsPerGroup(requestContext);
        int count = dashboardsByGroupId.Count;
        if (count + 1 > dashboardsPerGroup)
          throw new DashboardCountLimitExceededException(dashboardsPerGroup, count, count + 1);
        DashboardGroupEntry dashboardById;
        using (DashboardGroupSqlResourceComponent component = requestContext.CreateComponent<DashboardGroupSqlResourceComponent>())
          dashboardById = component.GetDashboardById(sourceDashboardConsumer.GetDataspaceId(), sourceDashboardConsumer.GetGroupId(), sourceDashboardConsumer.GetScope(), sourceDashboardId);
        if (dashboardById == null)
          throw new DashboardDoesNotExistException(sourceDashboardId);
        DashboardWidgetsDataModel widgetsForDashboard;
        using (WidgetSqlResourceComponent component = requestContext.CreateComponent<WidgetSqlResourceComponent>())
          widgetsForDashboard = component.GetWidgetsForDashboard(sourceDashboardConsumer.GetDataspaceId(), dashboardById.Id.Value);
        dashboardById.Widgets = (IEnumerable<Widget>) widgetsForDashboard.Widgets;
        Dashboard dashboardForCopy = DefaultDashboardFactory.CreateDashboardForCopy(targetDashboardConsumer, dashboardById, options, dashboardsByGroupId.Count == 0 ? 1 : dashboardsByGroupId.Select<DashboardGroupEntry, int>((Func<DashboardGroupEntry, int>) (x => x.Position)).Max() + 1);
        this.ValidateDashboard(requestContext, dashboardForCopy);
        Microsoft.VisualStudio.Services.Identity.Identity userIdentity = requestContext.GetUserIdentity();
        if (dashboardForCopy.DashboardScope == DashboardScope.Project_Team)
        {
          dashboardForCopy.ValidateNameIsUnique(requestContext, (IEnumerable<Dashboard>) dashboardsByGroupId, "DashboardService.CopyDashboard", 100170065);
          dashboardForCopy.OwnerId = targetDashboardConsumer.GetGroupId();
        }
        else
          dashboardForCopy.OwnerId = userIdentity.Id;
        dashboardForCopy.ModifiedBy = userIdentity.Id;
        dashboardForCopy.ModifiedDate = DateTime.UtcNow;
        List<WidgetDataModel> source = new List<WidgetDataModel>();
        string name = dashboardForCopy.Name;
        Guid? id = dashboardForCopy.Id;
        Guid dashboardId1 = id.Value;
        WebApiTeam defaultOrDestinationTeam2 = defaultOrDestinationTeam1;
        Guid? queryFolderPath = options.QueryFolderPath;
        int num = options.CopyQueriesFlag.Value ? 1 : 0;
        CopyDashboardParameterHandler copyDashboardParameterHandler = new CopyDashboardParameterHandler(name, dashboardId1, defaultOrDestinationTeam2, queryFolderPath, num != 0);
        foreach (Widget widget1 in dashboardById.Widgets)
        {
          Widget widget2 = this.m_CopyWidgetAdapterFactory.GetWidgetAdapter(widget1.ContributionId).ProcessAndCopyWidget(widget1, requestContext, sourceDashboardConsumer, targetDashboardConsumer, copyDashboardParameterHandler: copyDashboardParameterHandler);
          this.ValidateWidget(requestContext, widget2);
          if (widget2.IsNew())
          {
            widget2.Id = new Guid?(Guid.NewGuid());
            widget2.ETag = "1";
          }
          List<WidgetDataModel> widgetDataModelList = source;
          id = dashboardForCopy.Id;
          WidgetDataModel widgetDataModel = new WidgetDataModel(id.Value, widget2);
          widgetDataModelList.Add(widgetDataModel);
        }
        DashboardGroupEntryDataModel2 groupEntryDataModel2 = new DashboardGroupEntryDataModel2(dashboardForCopy.GroupId, dashboardForCopy.DashboardScope, dashboardForCopy);
        groupEntryDataModel2.LastAccessedDate = DateTime.UtcNow.Date;
        groupEntryDataModel2.ModifiedDate = DateTime.UtcNow;
        groupEntryDataModel2.ModifiedBy = requestContext.GetUserIdentity().Id;
        if (source.Count == 0)
        {
          DashboardGroupEntry dashboardGroupEntry;
          using (DashboardGroupSqlResourceComponent component = requestContext.CreateComponent<DashboardGroupSqlResourceComponent>())
            dashboardGroupEntry = component.AddDashboard(targetDashboardConsumer.GetDataspaceId(), groupEntryDataModel2);
          dashboardForCopy.ETag = dashboardGroupEntry.ETag;
        }
        else
        {
          DashboardWidgetsDataModel dashboardWithWidgets;
          using (WidgetSqlResourceComponent component = requestContext.CreateComponent<WidgetSqlResourceComponent>())
            dashboardWithWidgets = component.CreateDashboardWithWidgets(targetDashboardConsumer.GetDataspaceId(), groupEntryDataModel2, new DashboardWidgetsDataModel()
            {
              DashboardId = dashboardForCopy.Id.Value,
              ETag = dashboardForCopy.ETag,
              Widgets = source.AsEnumerable<WidgetDataModel>(),
              ModifiedBy = dashboardForCopy.ModifiedBy,
              ModifiedDate = dashboardForCopy.ModifiedDate
            });
          dashboardForCopy.Widgets = (IEnumerable<Widget>) dashboardWithWidgets.Widgets;
        }
        if (dashboardForCopy.DashboardScope == DashboardScope.Project)
        {
          IDashboardSecurityManager dashboardSecurityManager = this.DashboardSecurityManager;
          IVssRequestContext requestContext1 = requestContext;
          Guid dataspaceId = targetDashboardConsumer.GetDataspaceId();
          Guid? nullable = dashboardForCopy.Id;
          Guid dashboardId2 = nullable.Value;
          nullable = new Guid?();
          Guid? ownerId = nullable;
          dashboardSecurityManager.SetDashboardAllPermissions(requestContext1, dataspaceId, dashboardId2, ownerId);
        }
        CopyDashboardResponse dashboardResponse = new CopyDashboardResponse(options, dashboardForCopy);
        dashboardResponse.SetSecuredObject(DashboardSecuredObjectFactory.GenerateSecuredObject(requestContext, this.DashboardSecurityManager, targetDashboardConsumer.GetDataspaceId(), targetDashboardConsumer.GetGroupId(), dashboardForCopy.Id));
        this.UpdateRefreshIntervalFromRegistry(requestContext, dashboardForCopy);
        return dashboardResponse;
      }
    }

    public Dashboard GetDashboard(
      IVssRequestContext requestContext,
      IDashboardConsumer dashboardConsumer,
      Guid dashboardId)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<IDashboardConsumer>(dashboardConsumer, nameof (dashboardConsumer));
      ArgumentUtility.CheckForEmptyGuid(dashboardId, nameof (dashboardId));
      using (TelemetryCollector.TraceMonitor(requestContext, 100170060, nameof (DashboardService), "DashboardService.GetDashboard"))
      {
        ISecuredObject securedObject = DashboardSecuredObjectFactory.GenerateSecuredObject(requestContext, this.DashboardSecurityManager, dashboardConsumer.GetDataspaceId(), dashboardConsumer.GetGroupId(), new Guid?(dashboardId));
        DashboardGroupEntry dashboardById;
        using (DashboardGroupSqlResourceComponent component = requestContext.CreateComponent<DashboardGroupSqlResourceComponent>())
          dashboardById = component.GetDashboardById(dashboardConsumer.GetDataspaceId(), dashboardConsumer.GetGroupId(), dashboardConsumer.GetScope(), dashboardId);
        if (dashboardById == null)
          throw new DashboardDoesNotExistException(dashboardId);
        this.UpdateRefreshIntervalFromRegistry(requestContext, (Dashboard) dashboardById);
        Dashboard dashboard = new Dashboard(dashboardById);
        using (WidgetSqlResourceComponent component = requestContext.CreateComponent<WidgetSqlResourceComponent>())
        {
          DashboardWidgetsDataModel widgetsForDashboard = component.GetWidgetsForDashboard(dashboardConsumer.GetDataspaceId(), dashboard.Id.Value);
          dashboard.Widgets = (IEnumerable<Widget>) widgetsForDashboard.Widgets;
        }
        Widget[] array = dashboard.Widgets.ToArray<Widget>();
        IEnumerable<WidgetMetadata> metaDataList = DashboardService.GetMetaDataList(requestContext, (IEnumerable<Widget>) array);
        bool memberClaim = this.GetMemberClaim(requestContext);
        foreach (Widget widget in array)
        {
          widget.SetMetaData(DashboardService.GetMetaData(requestContext, metaDataList, widget.ContributionId));
          if (!memberClaim && !widget.IsEnabled)
            widget.Settings = (string) null;
        }
        CrossProjectSettingsPayloadFilter.FilterInsecurePublicWidgetConfigurations(requestContext, dashboardConsumer.GetDataspaceId(), (IEnumerable<Widget>) array);
        dashboard.Widgets = (IEnumerable<Widget>) array;
        dashboard.SetSecuredObject(securedObject);
        DateTime dateTime = dashboard.LastAccessedDate;
        dateTime = dateTime.Date;
        DateTime universalTime = dateTime.ToUniversalTime();
        dateTime = DateTime.UtcNow;
        DateTime date = dateTime.Date;
        if (universalTime != date)
          this.UpdateDashboardLastAccessedDate(requestContext, dashboardId, dashboardConsumer.GetDataspaceId(), dashboardConsumer.GetGroupId());
        return dashboard;
      }
    }

    internal static IEnumerable<WidgetMetadata> GetMetaDataList(
      IVssRequestContext context,
      IEnumerable<Widget> widgets)
    {
      IWidgetTypesService service = context.GetService<IWidgetTypesService>();
      IEnumerable<string> contributionIdsInResponse = widgets.Select<Widget, string>((Func<Widget, string>) (r => r.ContributionId));
      IVssRequestContext requestContext = context;
      Func<WidgetMetadata, bool> filter = (Func<WidgetMetadata, bool>) (m => contributionIdsInResponse.Contains<string>(m.ContributionId));
      return service.GetFilteredWidgetsMetadata(requestContext, filter) ?? throw new NoMetaDataFoundInCatalogException();
    }

    internal static WidgetMetadata GetMetaData(
      IVssRequestContext context,
      IEnumerable<WidgetMetadata> metaDataList,
      string contributionId)
    {
      IEnumerable<WidgetMetadata> source = metaDataList.Where<WidgetMetadata>((Func<WidgetMetadata, bool>) (m => m.ContributionId == contributionId));
      return source == null || source.Count<WidgetMetadata>() != 1 ? WidgetMetadata.CreateDisabledInstance(contributionId) : source.First<WidgetMetadata>();
    }

    public Dashboard ReplaceDashboardWidgets(
      IVssRequestContext requestContext,
      IDashboardConsumer dashboardConsumer,
      Dashboard dashboard)
    {
      return this.UpdateDashboardWidgets(requestContext, dashboardConsumer, dashboard, UpdateWidgetsBehavior.Replace);
    }

    public Dashboard UpdateDashboardWidgets(
      IVssRequestContext requestContext,
      IDashboardConsumer dashboardConsumer,
      Dashboard dashboard)
    {
      return this.UpdateDashboardWidgets(requestContext, dashboardConsumer, dashboard, UpdateWidgetsBehavior.Patch);
    }

    public virtual Dashboard UpdateDashboardWidgets(
      IVssRequestContext requestContext,
      IDashboardConsumer dashboardConsumer,
      Dashboard dashboard,
      UpdateWidgetsBehavior behavior)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<IDashboardConsumer>(dashboardConsumer, nameof (dashboardConsumer));
      ArgumentUtility.CheckForNull<Dashboard>(dashboard, nameof (dashboard));
      ArgumentUtility.CheckForEmptyGuid(dashboard.Id.Value, "dashboardId");
      using (TelemetryCollector.TraceMonitor(requestContext, 100170062, nameof (DashboardService), "DashboardService.UpdateDashboard"))
      {
        this.DashboardSecurityManager.CheckEditPermission(requestContext, dashboardConsumer.GetDataspaceId(), dashboardConsumer.GetGroupId(), dashboard.Id);
        IVssRequestContext requestContext1 = requestContext;
        Guid dataspaceId = dashboardConsumer.GetDataspaceId();
        Guid groupId = dashboardConsumer.GetGroupId();
        int scope = (int) dashboardConsumer.GetScope();
        Guid? id = dashboard.Id;
        Guid dashboardId = id.Value;
        Dashboard entry = new Dashboard(this.GetDashboardByDashboardIdFromSqlComponent(requestContext1, dataspaceId, groupId, (DashboardScope) scope, dashboardId));
        int requested = dashboard.Widgets.Count<Widget>();
        int widgetsPerDashboard = DashboardSettings.GetMaxWidgetsPerDashboard(requestContext);
        if (requested > widgetsPerDashboard)
          throw new WidgetLimitExceededException(widgetsPerDashboard, requested - 1, requested);
        List<WidgetDataModel> widgetDataModelList1 = new List<WidgetDataModel>();
        foreach (Widget widget in dashboard.Widgets)
        {
          this.ValidateWidget(requestContext, widget);
          if (widget.IsNew())
            widget.Id = new Guid?(Guid.NewGuid());
          List<WidgetDataModel> widgetDataModelList2 = widgetDataModelList1;
          id = dashboard.Id;
          WidgetDataModel widgetDataModel = new WidgetDataModel(id.Value, widget);
          widgetDataModelList2.Add(widgetDataModel);
        }
        IEnumerable<WidgetDataModel> widgetDataModels = this.DetectGridCollision(requestContext, dashboardConsumer, dashboard, behavior);
        using (WidgetSqlResourceComponent component = requestContext.CreateComponent<WidgetSqlResourceComponent>())
        {
          dashboard.ModifiedBy = requestContext.GetUserIdentity().Id;
          dashboard.ModifiedDate = DateTime.UtcNow;
          dashboard.LastAccessedDate = DateTime.UtcNow.Date;
          DashboardWidgetsDataModel widgetsDataModel;
          if (behavior == UpdateWidgetsBehavior.Replace)
            widgetsDataModel = component.ReplaceWidgets(dashboardConsumer.GetDataspaceId(), new DashboardWidgetsDataModel()
            {
              DashboardId = dashboard.Id.Value,
              ETag = dashboard.ETag,
              Widgets = widgetDataModels,
              ModifiedBy = dashboard.ModifiedBy,
              ModifiedDate = dashboard.ModifiedDate
            });
          else
            widgetsDataModel = component.UpdateWidgets(dashboardConsumer.GetDataspaceId(), new DashboardWidgetsDataModel()
            {
              DashboardId = dashboard.Id.Value,
              ETag = dashboard.ETag,
              Widgets = widgetDataModels,
              ModifiedBy = dashboard.ModifiedBy,
              ModifiedDate = dashboard.ModifiedDate
            });
          entry.Widgets = (IEnumerable<Widget>) widgetsDataModel.Widgets;
          entry.ETag = widgetsDataModel.ETag;
          entry.LastAccessedDate = dashboard.LastAccessedDate;
          entry.ModifiedDate = dashboard.ModifiedDate;
          entry.ModifiedBy = dashboard.ModifiedBy;
        }
        this.UpdateRefreshIntervalFromRegistry(requestContext, entry);
        return entry;
      }
    }

    public Widget AddWidget(
      IVssRequestContext requestContext,
      IDashboardConsumer dashboardConsumer,
      Guid dashboardId,
      Widget widget)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<IDashboardConsumer>(dashboardConsumer, nameof (dashboardConsumer));
      ArgumentUtility.CheckForEmptyGuid(dashboardId, nameof (dashboardId));
      ArgumentUtility.CheckForNull<Widget>(widget, nameof (widget));
      using (TelemetryCollector telemetryCollector = TelemetryCollector.TraceMonitor(requestContext, 10017360, "WidgetService", "DashboardService.AddWidget"))
      {
        this.DashboardSecurityManager.CheckEditPermission(requestContext, dashboardConsumer.GetDataspaceId(), dashboardConsumer.GetGroupId(), new Guid?(dashboardId));
        if (!widget.IsNew())
          throw new CreateWidgetWithExistingIdException();
        widget.Id = new Guid?(Guid.NewGuid());
        widget.SettingsVersion = widget.SettingsVersion != null ? widget.SettingsVersion : SemanticVersion.Default;
        IWidgetTypesService service = requestContext.GetService<IWidgetTypesService>();
        service.ValidateWidgets(requestContext, (IEnumerable<Widget>) new List<Widget>()
        {
          widget
        });
        service.GetFilteredWidgetsMetadata(requestContext, (Func<WidgetMetadata, bool>) (m => widget.ContributionId.Equals(m.ContributionId))).FirstOrDefault<WidgetMetadata>();
        telemetryCollector.Properties["WidgetId"] = widget.Id.ToString();
        telemetryCollector.Properties["WidgetTypeId"] = widget.TypeId;
        telemetryCollector.Properties["WidgetContributionId"] = widget.ContributionId;
        telemetryCollector.Properties["DashboardId"] = dashboardId.ToString();
        DashboardWidgetsDataModel widgetsForDashboard;
        IEnumerable<Widget> widgets1;
        using (WidgetSqlResourceComponent component = requestContext.CreateComponent<WidgetSqlResourceComponent>())
        {
          widgetsForDashboard = component.GetWidgetsForDashboard(dashboardConsumer.GetDataspaceId(), dashboardId);
          widgets1 = (IEnumerable<Widget>) widgetsForDashboard.Widgets;
        }
        IEnumerable<Widget> widgets2 = widgets1.Concat<Widget>((IEnumerable<Widget>) new Widget[1]
        {
          widget
        });
        string etag = widgetsForDashboard.ETag;
        Dashboard dashboard1 = new Dashboard()
        {
          Id = new Guid?(widgetsForDashboard.DashboardId),
          Widgets = widgets2,
          ETag = etag,
          LastAccessedDate = DateTime.UtcNow.Date,
          ModifiedDate = DateTime.UtcNow,
          ModifiedBy = requestContext.GetUserIdentity().Id
        };
        Dashboard dashboard2 = this.UpdateDashboardWidgets(requestContext, dashboardConsumer, dashboard1, UpdateWidgetsBehavior.Replace);
        Widget widget1 = dashboard2.Widgets.FirstOrDefault<Widget>((Func<Widget, bool>) (w =>
        {
          Guid? id1 = w.Id;
          Guid? id2 = widget.Id;
          if (id1.HasValue != id2.HasValue)
            return false;
          return !id1.HasValue || id1.GetValueOrDefault() == id2.GetValueOrDefault();
        }));
        if (widget1 == null)
          throw new WidgetDoesNotExistException(widget.Id.Value);
        widget1.Dashboard = new Dashboard(dashboard2.ETag);
        return widget1;
      }
    }

    public Widget GetWidgetById(
      IVssRequestContext requestContext,
      IDashboardConsumer dashboardConsumer,
      Guid dashboardId,
      Guid widgetId)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<IDashboardConsumer>(dashboardConsumer, nameof (dashboardConsumer));
      ArgumentUtility.CheckForEmptyGuid(dashboardId, nameof (dashboardId));
      ArgumentUtility.CheckForEmptyGuid(widgetId, nameof (widgetId));
      using (TelemetryCollector.TraceMonitor(requestContext, 10017372, "WidgetService", "DashboardService.GetWidgetById"))
      {
        ISecuredObject securedObject = DashboardSecuredObjectFactory.GenerateSecuredObject(requestContext, this.DashboardSecurityManager, dashboardConsumer.GetDataspaceId(), dashboardConsumer.GetGroupId(), new Guid?());
        Widget widgetById;
        using (WidgetSqlResourceComponent component = requestContext.CreateComponent<WidgetSqlResourceComponent>())
          widgetById = component.GetWidgetById(dashboardConsumer.GetDataspaceId(), widgetId);
        IEnumerable<WidgetMetadata> metaDataList = widgetById != null ? DashboardService.GetMetaDataList(requestContext, (IEnumerable<Widget>) new Widget[1]
        {
          widgetById
        }) : throw new WidgetDoesNotExistException(widgetId);
        int num = this.GetMemberClaim(requestContext) ? 1 : 0;
        widgetById.SetMetaData(DashboardService.GetMetaData(requestContext, metaDataList, widgetById.ContributionId));
        if (num == 0 && !widgetById.IsEnabled)
          widgetById.Settings = (string) null;
        CrossProjectSettingsPayloadFilter.FilterWidget(requestContext, dashboardConsumer.GetDataspaceId(), widgetById);
        widgetById.SetSecuredObject(securedObject);
        return widgetById;
      }
    }

    public Widget ReplaceWidget(
      IVssRequestContext requestContext,
      IDashboardConsumer dashboardConsumer,
      Guid dashboardId,
      Widget widget)
    {
      return this.UpdateWidget(requestContext, dashboardConsumer, dashboardId, widget, true);
    }

    public Widget UpdateWidget(
      IVssRequestContext requestContext,
      IDashboardConsumer dashboardConsumer,
      Guid dashboardId,
      Widget widget)
    {
      return this.UpdateWidget(requestContext, dashboardConsumer, dashboardId, widget, true);
    }

    public Widget UpdateWidget(
      IVssRequestContext requestContext,
      IDashboardConsumer dashboardConsumer,
      Guid dashboardId,
      Widget widget,
      bool replaceWidget = false)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<IDashboardConsumer>(dashboardConsumer, nameof (dashboardConsumer));
      ArgumentUtility.CheckForEmptyGuid(dashboardId, nameof (dashboardId));
      ArgumentUtility.CheckForNull<Widget>(widget, nameof (widget));
      using (TelemetryCollector.TraceMonitor(requestContext, 10017362, "DashboardService.UpdateWidget", "WidgetService"))
      {
        this.DashboardSecurityManager.CheckEditPermission(requestContext, dashboardConsumer.GetDataspaceId(), dashboardConsumer.GetGroupId(), new Guid?(dashboardId));
        WidgetDataModel[] widgetDataModelArray = !widget.IsNew() ? new WidgetDataModel[1]
        {
          new WidgetDataModel(dashboardId, widget)
        } : throw new WidgetWithoutIdException();
        widgetDataModelArray[0].ReplaceWidget = replaceWidget;
        Dashboard dashboard1 = new Dashboard()
        {
          Id = new Guid?(dashboardId),
          Widgets = (IEnumerable<Widget>) widgetDataModelArray
        };
        if (widget.Dashboard != null)
          dashboard1.ETag = widget.Dashboard.ETag;
        Dashboard dashboard2 = this.UpdateDashboardWidgets(requestContext, dashboardConsumer, dashboard1);
        Widget widget1 = dashboard2.Widgets.FirstOrDefault<Widget>((Func<Widget, bool>) (x =>
        {
          Guid? id1 = x.Id;
          Guid? id2 = widget.Id;
          if (id1.HasValue != id2.HasValue)
            return false;
          return !id1.HasValue || id1.GetValueOrDefault() == id2.GetValueOrDefault();
        }));
        if (widget1 == null)
          throw new WidgetDoesNotExistException(widget.Id.Value);
        widget1.Dashboard = new Dashboard(dashboard2.ETag);
        return widget1;
      }
    }

    public Dashboard DeleteWidget(
      IVssRequestContext requestContext,
      IDashboardConsumer dashboardConsumer,
      Guid dashboardId,
      Guid widgetId)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<IDashboardConsumer>(dashboardConsumer, nameof (dashboardConsumer));
      ArgumentUtility.CheckForEmptyGuid(dashboardId, nameof (dashboardId));
      ArgumentUtility.CheckForEmptyGuid(widgetId, nameof (widgetId));
      using (TelemetryCollector.TraceMonitor(requestContext, 10017366, "DashboardService.DeleteWidget", "WidgetService"))
      {
        this.DashboardSecurityManager.CheckEditPermission(requestContext, dashboardConsumer.GetDataspaceId(), dashboardConsumer.GetGroupId(), new Guid?(dashboardId));
        Widget widget = (Widget) null;
        using (WidgetSqlResourceComponent component = requestContext.CreateComponent<WidgetSqlResourceComponent>())
          widget = component.GetWidgetById(dashboardConsumer.GetDataspaceId(), widgetId);
        if (widget == null)
          throw new WidgetDoesNotExistException(widgetId);
        using (WidgetSqlResourceComponent component = requestContext.CreateComponent<WidgetSqlResourceComponent>())
          component.DeleteWidget(dashboardConsumer.GetDataspaceId(), widgetId);
        return this.GetDashboard(requestContext, dashboardConsumer, dashboardId);
      }
    }

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public void UpdateDashboardLastAccessedDate(
      IVssRequestContext requestContext,
      Guid dashboardId,
      Guid projectId,
      Guid groupId)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(dashboardId, nameof (dashboardId));
      ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId));
      if (requestContext.GetProjectInfo(projectId.ToString()).Visibility == ProjectVisibility.Public)
        return;
      using (TelemetryCollector.TraceMonitor(requestContext, 100170066, nameof (DashboardService), "DashboardService.UpdateLastAccessedForDashboardId"))
      {
        using (DashboardGroupSqlResourceComponent component = requestContext.CreateComponent<DashboardGroupSqlResourceComponent>())
          component.UpdateDashboardLastAccessedDate(dashboardId, projectId, groupId);
      }
    }

    public (int DashboardCount, int MaxAllowedDashboards) GetDashboardCountByProjectId(
      IVssRequestContext requestContext,
      Guid projectId)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId));
      int num = requestContext.GetService<IProjectService>().TryGetProject(requestContext, projectId, out ProjectInfo _) ? DashboardSettings.GetMaxDashboardsPerGroup(requestContext) : throw new ProjectDoesNotExistException(projectId.ToString());
      using (DashboardGroupSqlResourceComponent component = requestContext.CreateComponent<DashboardGroupSqlResourceComponent>())
        return (component.GetDashboardsByProjectId(projectId).Where<DashboardGroupEntry>((Func<DashboardGroupEntry, bool>) (dashboard => dashboard.DashboardScope == DashboardScope.Project)).ToList<DashboardGroupEntry>().Count, num);
    }
  }
}
