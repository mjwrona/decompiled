// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.ExternalArtifacts.Components.ExternalArtifactSqlComponent3
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.ExternalArtifacts.DataModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.ExternalArtifacts.Components
{
  internal class ExternalArtifactSqlComponent3 : ExternalArtifactSqlComponent2
  {
    public override ExternalArtifactResult GetArtifacts(
      IEnumerable<(Guid repositoryId, string sha)> repositoryAndShas,
      IEnumerable<(Guid repositoryId, string prId)> repositoryAndPrIds,
      IEnumerable<(Guid repositoryId, string issueId)> repositoryAndIssueIds)
    {
      this.PrepareStoredProcedure("prc_GetExternalArtifacts");
      this.BindKeyValuePairGuidStringTable("@repositoryAndCommitArtifactIds", repositoryAndShas != null ? repositoryAndShas.Select<(Guid, string), KeyValuePair<Guid, string>>((System.Func<(Guid, string), KeyValuePair<Guid, string>>) (kvp => new KeyValuePair<Guid, string>(kvp.repositoryId, kvp.sha))) : (IEnumerable<KeyValuePair<Guid, string>>) null);
      this.BindKeyValuePairGuidStringTable("@repositoryAndPrArtifactIds", repositoryAndPrIds != null ? repositoryAndPrIds.Select<(Guid, string), KeyValuePair<Guid, string>>((System.Func<(Guid, string), KeyValuePair<Guid, string>>) (kvp => new KeyValuePair<Guid, string>(kvp.repositoryId, kvp.prId))) : (IEnumerable<KeyValuePair<Guid, string>>) null);
      this.BindKeyValuePairGuidStringTable("@repositoryAndIssueArtifactIds", repositoryAndIssueIds != null ? repositoryAndIssueIds.Select<(Guid, string), KeyValuePair<Guid, string>>((System.Func<(Guid, string), KeyValuePair<Guid, string>>) (kvp => new KeyValuePair<Guid, string>(kvp.repositoryId, kvp.issueId))) : (IEnumerable<KeyValuePair<Guid, string>>) null);
      return this.ExecuteUnknown<ExternalArtifactResult>((System.Func<IDataReader, ExternalArtifactResult>) (reader =>
      {
        List<ExternalCommitDataset> list1 = this.GetExternalCommitBinder().BindAll(reader).ToList<ExternalCommitDataset>();
        reader.NextResult();
        List<ExternalPullRequestDataset> list2 = this.GetExternalPullRequestBinder().BindAll(reader).ToList<ExternalPullRequestDataset>();
        reader.NextResult();
        List<ExternalArtifactUserResultSet> list3 = this.GetExternalUserBinder().BindAll(reader).ToList<ExternalArtifactUserResultSet>();
        reader.NextResult();
        List<ExternalIssueDataset> list4 = this.GetExternalIssueBinder().BindAll(reader).ToList<ExternalIssueDataset>();
        reader.NextResult();
        List<ExternalArtifactUserResultSet> list5 = this.GetExternalUserBinder().BindAll(reader).ToList<ExternalArtifactUserResultSet>();
        IEnumerable<ExternalArtifactUserResultSet> artifactUserResultSets = list3.Concat<ExternalArtifactUserResultSet>((IEnumerable<ExternalArtifactUserResultSet>) list5);
        return new ExternalArtifactResult()
        {
          Commits = (IEnumerable<ExternalCommitDataset>) list1,
          PullRequests = (IEnumerable<ExternalPullRequestDataset>) list2,
          Issues = (IEnumerable<ExternalIssueDataset>) list4,
          Users = artifactUserResultSets
        };
      }));
    }

    public override PendingExternalArtifactResult GetPendingArtifacts()
    {
      this.PrepareStoredProcedure("prc_GetPendingExternalArtifacts");
      this.BindInt("@batchSize", 200);
      return this.ExecuteUnknown<PendingExternalArtifactResult>((System.Func<IDataReader, PendingExternalArtifactResult>) (reader =>
      {
        List<PendingExternalArtifactDataSet> list1 = this.GetPendingExternalArtifactBinder().BindAll(reader).ToList<PendingExternalArtifactDataSet>();
        reader.NextResult();
        List<PendingExternalArtifactDataSet> list2 = this.GetPendingExternalArtifactBinder().BindAll(reader).ToList<PendingExternalArtifactDataSet>();
        reader.NextResult();
        List<PendingExternalArtifactDataSet> list3 = this.GetPendingExternalArtifactBinder().BindAll(reader).ToList<PendingExternalArtifactDataSet>();
        return new PendingExternalArtifactResult()
        {
          Commits = (IEnumerable<PendingExternalArtifactDataSet>) list1,
          PullRequests = (IEnumerable<PendingExternalArtifactDataSet>) list2,
          Issues = (IEnumerable<PendingExternalArtifactDataSet>) list3
        };
      }));
    }

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
      this.BindExternalIssueTable("@issues", issues);
      this.ExecuteNonQuery();
    }

    protected override void BindExternalArtifactUsersTable(
      string parameterName,
      IEnumerable<ExternalArtifactUserDataset> artifactUsers)
    {
      new ExternalArtifactSqlComponent.ExternalArtifactUserTable3(parameterName, artifactUsers).BindTable((ExternalArtifactSqlComponent) this);
    }

    protected virtual ExternalArtifactSqlComponent.ExternalIssueBinder GetExternalIssueBinder() => new ExternalArtifactSqlComponent.ExternalIssueBinder();

    protected virtual void BindExternalIssueTable(
      string parameterName,
      IEnumerable<ExternalIssueDataset> issues)
    {
      new ExternalArtifactSqlComponent.ExternalIssueTable2(parameterName, issues).BindTable((ExternalArtifactSqlComponent) this);
    }
  }
}
