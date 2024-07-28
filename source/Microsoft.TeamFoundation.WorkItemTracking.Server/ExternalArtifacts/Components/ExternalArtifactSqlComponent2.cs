// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.ExternalArtifacts.Components.ExternalArtifactSqlComponent2
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.WorkItemTracking.Server.ExternalArtifacts.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.ExternalArtifacts.Components
{
  internal class ExternalArtifactSqlComponent2 : ExternalArtifactSqlComponent
  {
    public override void SaveArtifacts(
      IEnumerable<ExternalUserDataset> users,
      IEnumerable<ExternalArtifactUserDataset> artifactUsers,
      IEnumerable<ExternalCommitDataset> commits,
      IEnumerable<ExternalPullRequestDataset> pullRequests,
      IEnumerable<ExternalIssueDataset> issues)
    {
      this.PrepareStoredProcedure("prc_SaveExternalArtifacts");
      this.BindExternalUserTable("@users", users);
      this.BindExternalArtifactUsersTable("@artifactUsers", artifactUsers);
      this.BindExternalCommitTable("@commits", commits);
      this.BindExternalPullRequestTable("@pullRequests", pullRequests);
      this.ExecuteNonQuery();
    }

    protected override void BindExternalUserTable(
      string parameterName,
      IEnumerable<ExternalUserDataset> users)
    {
      new ExternalArtifactSqlComponent.ExternalUserTable2(parameterName, users).BindTable((ExternalArtifactSqlComponent) this);
    }

    protected override void BindExternalArtifactUsersTable(
      string parameterName,
      IEnumerable<ExternalArtifactUserDataset> artifactUsers)
    {
      new ExternalArtifactSqlComponent.ExternalArtifactUserTable2(parameterName, artifactUsers != null ? artifactUsers.Where<ExternalArtifactUserDataset>((Func<ExternalArtifactUserDataset, bool>) (u => !string.IsNullOrEmpty(u.UserId))) : (IEnumerable<ExternalArtifactUserDataset>) null).BindTable((ExternalArtifactSqlComponent) this);
    }

    protected override void BindExternalCommitTable(
      string parameterName,
      IEnumerable<ExternalCommitDataset> commits)
    {
      new ExternalArtifactSqlComponent.ExternalCommitTable2(parameterName, commits).BindTable((ExternalArtifactSqlComponent) this);
    }

    protected override void BindExternalPullRequestTable(
      string parameterName,
      IEnumerable<ExternalPullRequestDataset> pullRequests)
    {
      new ExternalArtifactSqlComponent.ExternalPullRequestTable2(parameterName, pullRequests).BindTable((ExternalArtifactSqlComponent) this);
    }
  }
}
