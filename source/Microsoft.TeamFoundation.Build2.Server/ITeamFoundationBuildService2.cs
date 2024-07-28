// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.ITeamFoundationBuildService2
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Build2.Server
{
  [DefaultServiceImplementation(typeof (TeamFoundationBuildService2))]
  internal interface ITeamFoundationBuildService2 : IVssFrameworkService
  {
    IEnumerable<BuildMetric> GetProjectMetrics(
      IVssRequestContext requestContext,
      Guid projectId,
      string metricAggregationType,
      DateTime? minMetricsTime);

    void CreateTeamProject(IVssRequestContext requestContext, Guid projectId);

    void DeleteTeamProject(IVssRequestContext requestContext, Guid projectId);

    Microsoft.VisualStudio.Services.Identity.Identity ProvisionServiceIdentity(
      IVssRequestContext requestContext,
      BuildAuthorizationScope scope,
      Guid projectId = default (Guid),
      bool setPermissions = false,
      IServicingContext servicingContext = null);

    void SetServiceIdentityPermissions(
      IVssRequestContext requestContext,
      Guid projectId,
      Microsoft.VisualStudio.Services.Identity.Identity serviceIdentity,
      int? definitionId = null);

    Microsoft.VisualStudio.Services.Identity.Identity GetServiceIdentity(
      IVssRequestContext requestContext,
      BuildAuthorizationScope authorizationScope,
      Guid projectId = default (Guid));

    IEnumerable<string> GetTags(IVssRequestContext requestContext, Guid projectId);

    IEnumerable<string> GetFilterBuildTags(
      IVssRequestContext requestContext,
      Guid projectId,
      MinimalBuildDefinition definition);

    RetentionPolicy GetDefaultRetentionPolicy(IVssRequestContext requestContext);

    RetentionPolicy SetDefaultRetentionPolicy(
      IVssRequestContext requestContext,
      RetentionPolicy retentionPolicy);

    RetentionPolicy GetMaximumRetentionPolicy(IVssRequestContext requestContext);

    RetentionPolicy SetMaximumRetentionPolicy(
      IVssRequestContext requestContext,
      RetentionPolicy retentionPolicy);

    int GetDaysToKeepDeletedBuildsBeforeDestroy(IVssRequestContext requestContext);

    int SetDaysToKeepDeletedBuildsBeforeDestroy(IVssRequestContext requestContext, int daysToKeep);

    IReportGenerator GetReportGenerator(
      IVssRequestContext requestContext,
      string reportType = "Html",
      bool throwIfNotFound = true);

    BuildResourceUsage GetBuildResourceUsage(IVssRequestContext requestContext);

    void SetWorkItemQueryFoldersPermission(
      IVssRequestContext requestContext,
      Guid projectId,
      Microsoft.VisualStudio.Services.Identity.Identity serviceIdentity,
      ITeamFoundationSecurityService securityService);
  }
}
