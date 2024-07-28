// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Dashboards.WebApi.DashboardsPrivileges
// Assembly: Microsoft.TeamFoundation.Dashboards.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 05786B5F-D2ED-4F72-80F1-EA5660131542
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Dashboards.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using System;

namespace Microsoft.TeamFoundation.Dashboards.WebApi
{
  internal class DashboardsPrivileges
  {
    public static readonly Guid NamespaceId = new Guid("8ADF73B7-389A-4276-B638-FE1653F7EFC7");
    public static readonly char NamespaceSeparator = '/';
    public static readonly string Root = "$";
    public static readonly string MaterializeDashboardsRoot = "/MaterializeDashboards";

    public static TeamDashboardPermission NamespaceToUserPermission(int permissions) => DashboardsPrivileges.TryParse(permissions);

    private static TeamDashboardPermission TryParse(int permissions)
    {
      TeamDashboardPermission dashboardPermission = TeamDashboardPermission.None;
      if ((permissions & 1) != 0)
        dashboardPermission |= TeamDashboardPermission.Read;
      if ((permissions & 2) != 0)
        dashboardPermission |= TeamDashboardPermission.Create;
      if ((permissions & 4) != 0)
        dashboardPermission |= TeamDashboardPermission.Edit;
      if ((permissions & 8) != 0)
        dashboardPermission |= TeamDashboardPermission.Delete;
      if ((permissions & 16) != 0)
        dashboardPermission |= TeamDashboardPermission.ManagePermissions;
      return dashboardPermission;
    }

    public static int UserToNamespacePermission(TeamDashboardPermission teamDashboardPermission)
    {
      int namespacePermission = 0;
      if ((teamDashboardPermission & TeamDashboardPermission.Read) != TeamDashboardPermission.None)
        namespacePermission |= 1;
      if ((teamDashboardPermission & TeamDashboardPermission.Create) != TeamDashboardPermission.None)
        namespacePermission |= 2;
      if ((teamDashboardPermission & TeamDashboardPermission.Edit) != TeamDashboardPermission.None)
        namespacePermission |= 4;
      if ((teamDashboardPermission & TeamDashboardPermission.Delete) != TeamDashboardPermission.None)
        namespacePermission |= 8;
      if ((teamDashboardPermission & TeamDashboardPermission.ManagePermissions) != TeamDashboardPermission.None)
        namespacePermission |= 16;
      return namespacePermission;
    }

    public static int AllPermissions() => 31;

    [GenerateAllConstants(null)]
    public class DashboardsPermissions
    {
      public const int Read = 1;
      public const int Create = 2;
      public const int Edit = 4;
      public const int Delete = 8;
      public const int ManagePermissions = 16;
      public const int MaterializeDashboards = 32;
    }
  }
}
