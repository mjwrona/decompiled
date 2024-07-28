// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Boards.RecentActivity.ITeamFoundationRecentActivityService
// Assembly: Microsoft.Azure.Boards.RecentActivity, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 684DCFA4-4764-4794-94A6-960AF811434C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Boards.RecentActivity.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.Azure.Boards.RecentActivity
{
  [DefaultServiceImplementation(typeof (TeamFoundationRecentActivityService))]
  public interface ITeamFoundationRecentActivityService : IVssFrameworkService
  {
    IReadOnlyDictionary<string, Microsoft.Azure.Boards.RecentActivity.RecentActivity> GetUserActivities(
      IVssRequestContext requestContext,
      Guid identityId,
      Guid artifactKind);

    IReadOnlyDictionary<string, Microsoft.Azure.Boards.RecentActivity.RecentActivity> GetProjectActivities(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid artifactKind,
      int limit);

    void UpdateActivities(
      IVssRequestContext requestContext,
      IReadOnlyCollection<Microsoft.Azure.Boards.RecentActivity.RecentActivity> activities);

    int CleanupProjectActivities(IVssRequestContext requestContext, Guid artifactKind);

    void CleanupRecentUserActivities(IVssRequestContext requestContext);

    void CleanupRecentProjectUserActivities(IVssRequestContext requestContext);

    IReadOnlyList<Microsoft.Azure.Boards.RecentActivity.RecentActivity> GetProjectUserActivities(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid identityId,
      IEnumerable<Guid> artifactKindIds);

    void UpdateProjectUserActivities(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid identityId,
      DateTime activityDate,
      Guid artifactKind,
      string artifactId,
      IDictionary<string, string> activityDetails);
  }
}
