// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.Services.PullRequestReleaseService
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Common.Diagnostics;
using Microsoft.VisualStudio.Services.ReleaseManagement.Common.Extensions;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.Services
{
  public class PullRequestReleaseService : ReleaseManagement2ServiceBase
  {
    public virtual IList<int> GetAndUpdateCancelableReleaseIdsForPullRequest(
      IVssRequestContext requestContext,
      Guid projectId,
      int pullRequestId,
      DateTime mergedAt,
      int iterationId)
    {
      if (requestContext == null)
        throw new ArgumentNullException(nameof (requestContext));
      using (ReleaseManagementTimer.Create(requestContext, "DataAccessLayer", "PullRequestReleaseService.GetCancelableReleaseIdsForPullRequest", 1976453))
      {
        Func<PullRequestReleaseSqlComponent, IList<int>> action = (Func<PullRequestReleaseSqlComponent, IList<int>>) (component => component.UpdateCancelablePullRequestReleases(projectId, pullRequestId, iterationId, mergedAt));
        return (IList<int>) requestContext.ExecuteWithinUsingWithComponent<PullRequestReleaseSqlComponent, IList<int>>(action).ToList<int>();
      }
    }

    public virtual IList<PullRequestRelease> ListPullRequestReleases(
      IVssRequestContext requestContext,
      Guid projectId,
      int pullRequestId,
      bool isActive)
    {
      if (requestContext == null)
        throw new ArgumentNullException(nameof (requestContext));
      using (ReleaseManagementTimer.Create(requestContext, "DataAccessLayer", "PullRequestReleaseService.GetCancelableReleaseIdsForPullRequest", 1976453))
      {
        Func<PullRequestReleaseSqlComponent, IList<PullRequestRelease>> action = (Func<PullRequestReleaseSqlComponent, IList<PullRequestRelease>>) (component => component.ListPullRequestReleases(projectId, pullRequestId, isActive));
        return (IList<PullRequestRelease>) requestContext.ExecuteWithinUsingWithComponent<PullRequestReleaseSqlComponent, IList<PullRequestRelease>>(action).ToList<PullRequestRelease>();
      }
    }

    public virtual void CreatePullRequestRelease(
      IVssRequestContext requestContext,
      PullRequestRelease pullRequestRelease)
    {
      if (requestContext == null)
        throw new ArgumentNullException(nameof (requestContext));
      using (ReleaseManagementTimer.Create(requestContext, "DataAccessLayer", "PullRequestReleaseService.CreatePullRequestRelease", 1976454))
      {
        Action<PullRequestReleaseSqlComponent> action = (Action<PullRequestReleaseSqlComponent>) (component => component.CreatePullRequestRelease(pullRequestRelease));
        requestContext.ExecuteWithinUsingWithComponent<PullRequestReleaseSqlComponent>(action);
      }
    }

    public virtual void DeletePullRequestReleases(
      IVssRequestContext requestContext,
      Guid projectId,
      IList<int> pullRequestIds)
    {
      if (requestContext == null)
        throw new ArgumentNullException(nameof (requestContext));
      using (ReleaseManagementTimer.Create(requestContext, "DataAccessLayer", "PullRequestReleaseService.DeletePullRequestReleases", 1976455))
      {
        Action<PullRequestReleaseSqlComponent> action = (Action<PullRequestReleaseSqlComponent>) (component => component.DeletePullReleases(projectId, pullRequestIds));
        requestContext.ExecuteWithinUsingWithComponent<PullRequestReleaseSqlComponent>(action);
      }
    }
  }
}
