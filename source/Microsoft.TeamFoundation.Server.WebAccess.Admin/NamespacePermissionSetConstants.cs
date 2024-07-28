// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Admin.NamespacePermissionSetConstants
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Admin, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 31215F45-B8A9-42A7-99A7-F8CB77B7D405
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Admin.dll

using Microsoft.Azure.Boards.CssNodes;
using Microsoft.TeamFoundation.Build.Common;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.ReleaseManagement.Common;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.TeamFoundation.VersionControl.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Common;
using System;

namespace Microsoft.TeamFoundation.Server.WebAccess.Admin
{
  internal static class NamespacePermissionSetConstants
  {
    internal static readonly Guid OrganizationLevel = new Guid("706AE8C9-3370-404B-9CB1-DF6D0FAA6EDB");
    internal static readonly Guid CollectionLevel = new Guid("3D9D8F5B-9430-447F-89C1-20BF35758E08");
    internal static readonly Guid ProjectLevel = AuthorizationSecurityConstants.ProjectSecurityGuid;
    internal static readonly Guid VersionControl = SecurityConstants.RepositorySecurityNamespaceGuid;
    internal static readonly Guid Git = GitConstants.GitSecurityNamespaceId;
    internal static readonly Guid Build = BuildSecurity.BuildNamespaceId;
    internal static readonly Guid WitQueryFolders = QueryItemSecurityConstants.NamespaceGuid;
    internal static readonly Guid Iteration = AuthorizationSecurityConstants.IterationNodeSecurityGuid;
    internal static readonly Guid Area = AuthorizationSecurityConstants.CommonStructureNodeSecurityGuid;
    internal static readonly Guid ReleaseManagement = ReleaseManagementSecurity.SecurityNamespaceId;
    internal static readonly Guid Process = FrameworkSecurity.ProcessNamespaceId;
    internal static readonly Guid MetaTask = MetaTaskPermissionsManager.SecurityNamespaceId;
    internal static readonly Guid Plan = PlanSecurityGroupConstants.SecurityNamespaceId;
    internal static readonly Guid Dashboard = DashboardSecurityConstants.SecurityNamespaceId;
    internal static readonly Guid AnalyticsViews = AnalyticsViewsSecurityNamespace.SecurityNamespaceId;
  }
}
