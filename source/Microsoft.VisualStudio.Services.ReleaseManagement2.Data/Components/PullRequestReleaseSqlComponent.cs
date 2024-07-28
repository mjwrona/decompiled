// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components.PullRequestReleaseSqlComponent
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataAccess;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataBinders;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components
{
  public class PullRequestReleaseSqlComponent : ReleaseManagementSqlResourceComponentBase
  {
    [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "This signature is required by VSSF. Don't change.")]
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[1]
    {
      (IComponentCreator) new ComponentCreator<PullRequestReleaseSqlComponent>(1)
    }, "ReleaseManagementPullRequestRelease", "ReleaseManagement");

    public virtual void CreatePullRequestRelease(PullRequestRelease pullRequestRelease)
    {
      if (pullRequestRelease == null)
        throw new ArgumentNullException(nameof (pullRequestRelease));
      this.PrepareStoredProcedure("Release.prc_CreatePullRequestRelease", pullRequestRelease.ProjectId);
      this.BindPullRequestReleaseTable("@pullRequestReleases", (IEnumerable<PullRequestRelease>) new List<PullRequestRelease>()
      {
        pullRequestRelease
      });
      this.ExecuteNonQuery();
    }

    public virtual void UpdatePullRequestReleaseStatus(
      Guid projectId,
      int pullRequestId,
      int releaseId,
      bool isActive)
    {
      this.PrepareStoredProcedure("Release.prc_UpdatePullRequestRelease", projectId);
      this.BindInt(nameof (pullRequestId), pullRequestId);
      this.BindInt(nameof (releaseId), releaseId);
      this.BindBoolean(nameof (isActive), isActive);
      this.ExecuteNonQuery();
    }

    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "These are optional parameters")]
    public virtual IList<PullRequestRelease> ListPullRequestReleases(
      Guid projectId,
      int pullRequestId,
      bool isActive = true)
    {
      this.PrepareStoredProcedure("Release.prc_ListPullRequestRelease", projectId);
      this.BindInt(nameof (pullRequestId), pullRequestId);
      this.BindBoolean(nameof (isActive), isActive);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<PullRequestRelease>((ObjectBinder<PullRequestRelease>) this.GetPullRequestReleaseBinder());
        return (IList<PullRequestRelease>) resultCollection.GetCurrent<PullRequestRelease>().Items;
      }
    }

    public void DeletePullReleases(Guid projectId, IList<int> pullRequestIds)
    {
      this.PrepareStoredProcedure("Release.prc_BulkDeletePullRequestRelease", projectId);
      this.BindInt32Table(nameof (pullRequestIds), (IEnumerable<int>) pullRequestIds);
      this.ExecuteNonQuery();
    }

    public IList<int> UpdateCancelablePullRequestReleases(
      Guid projectId,
      int pullRequestId,
      int iterationId,
      DateTime mergedAt)
    {
      this.PrepareStoredProcedure("Release.prc_GetAndUpdateStatusForCancelableReleases", projectId);
      this.BindInt(nameof (pullRequestId), pullRequestId);
      this.BindInt(nameof (iterationId), iterationId);
      this.BindDateTime(nameof (mergedAt), mergedAt);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<int>((ObjectBinder<int>) new ReleaseIdListBinder());
        return (IList<int>) resultCollection.GetCurrent<int>().Items;
      }
    }

    [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "Override of base class method.")]
    protected virtual PullRequestReleaseBinder GetPullRequestReleaseBinder() => new PullRequestReleaseBinder((ReleaseManagementSqlResourceComponentBase) this);
  }
}
