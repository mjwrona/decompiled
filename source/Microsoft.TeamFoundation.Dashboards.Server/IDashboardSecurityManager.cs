// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Dashboards.IDashboardSecurityManager
// Assembly: Microsoft.TeamFoundation.Dashboards.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CC7F149F-95E9-4579-9C7B-BAEEA5A10ECA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Dashboards.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;

namespace Microsoft.TeamFoundation.Dashboards
{
  public interface IDashboardSecurityManager
  {
    int GetEffectivePermissions(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid teamId,
      Guid? dashboardId);

    void CheckReadPermission(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid teamId,
      Guid? dashboardId);

    bool HasReadPermission(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid teamId,
      Guid? dashboardId);

    void CheckCreatePermission(IVssRequestContext requestContext, Guid projectId, Guid teamId);

    void CheckEditPermission(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid teamId,
      Guid? dashboardId);

    void CheckDeletePermission(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid teamId,
      Guid? dashboardId);

    void SetDefaultDashboardWithManageGroupPermission(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid teamId);

    void SetDashboardWithReadGroupPermission(IVssRequestContext requestContext, Guid teamId);

    int GetTeamMemberGroupPermissions(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid teamId);

    bool HasMaterializeDashboardsPermission(IVssRequestContext requestContext);

    void SetDashboardAllPermissions(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid dashboardId,
      Guid? ownerId);

    void SetProjectDashboardsCreatePermission(IVssRequestContext requestContext, Guid projectId);
  }
}
