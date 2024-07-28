// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.GitCommitStatusComponent
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Microsoft.TeamFoundation.Git.Server
{
  internal class GitCommitStatusComponent : TeamFoundationSqlResourceComponent
  {
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[2]
    {
      (IComponentCreator) new ComponentCreator<GitCommitStatusComponent>(1),
      (IComponentCreator) new ComponentCreator<GitCommitStatusComponent>(1290)
    }, "GitCommitStatus", "Git");
    private readonly SqlMetaData[] typ_OrderedObjectIdTable = new SqlMetaData[1]
    {
      new SqlMetaData("Data", SqlDbType.Binary, 20L)
    };

    public GitCommitStatusComponent()
    {
      this.SelectedFeatures = SqlResourceComponentFeatures.BindPartitionIdByDefault;
      this.ContainerErrorCode = 50000;
    }

    public GitStatus AddStatus(RepoKey repoKey, Sha1Id commitId, GitStatus commitStatus)
    {
      this.PrepareStoredProcedure("prc_AddGitCommitStatus");
      this.BindGuid("@repositoryId", repoKey.RepoId);
      this.BindBinary("@commitId", commitId.ToByteArray(), 20, SqlDbType.Binary);
      this.BindByte("@state", (byte) commitStatus.State);
      this.BindString("@context", commitStatus.Context.Name, 128, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.BindString("@genre", commitStatus.Context.Genre, 128, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindString("@description", commitStatus.Description, 2048, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindString("@targetUrl", commitStatus.TargetUrl, 2048, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindGuid("@createdBy", new Guid(commitStatus.CreatedBy.Id));
      if (this.Version >= 1290)
        this.BindDataspace((RepoScope) repoKey);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<KeyValuePair<Sha1Id, GitStatus>>((ObjectBinder<KeyValuePair<Sha1Id, GitStatus>>) new GitCommitStatusComponent.GitCommitStatusBinder());
      return resultCollection.GetCurrent<KeyValuePair<Sha1Id, GitStatus>>().First<KeyValuePair<Sha1Id, GitStatus>>().Value;
    }

    public virtual ILookup<Sha1Id, GitStatus> QueryStatuses(
      RepoKey repoKey,
      IEnumerable<Sha1Id> commits,
      int top,
      int skip)
    {
      this.PrepareStoredProcedure("prc_QueryGitCommitStatus");
      this.BindGuid("@repositoryId", repoKey.RepoId);
      this.BindObjectIdTable("@commits", commits);
      this.BindInt("@top", top);
      this.BindInt("@skip", skip);
      if (this.Version >= 1290)
        this.BindDataspace((RepoScope) repoKey);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<KeyValuePair<Sha1Id, GitStatus>>((ObjectBinder<KeyValuePair<Sha1Id, GitStatus>>) new GitCommitStatusComponent.GitCommitStatusBinder());
      return resultCollection.GetCurrent<KeyValuePair<Sha1Id, GitStatus>>().ToLookup<KeyValuePair<Sha1Id, GitStatus>, Sha1Id, GitStatus>((System.Func<KeyValuePair<Sha1Id, GitStatus>, Sha1Id>) (kvp => kvp.Key), (System.Func<KeyValuePair<Sha1Id, GitStatus>, GitStatus>) (kvp => kvp.Value));
    }

    private SqlParameter BindObjectIdTable(string parameterName, IEnumerable<Sha1Id> rows)
    {
      rows = rows ?? Enumerable.Empty<Sha1Id>();
      System.Func<Sha1Id, SqlDataRecord> selector = (System.Func<Sha1Id, SqlDataRecord>) (hash =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(this.typ_OrderedObjectIdTable);
        sqlDataRecord.SetBytes(0, 0L, hash.ToByteArray(), 0, 20);
        return sqlDataRecord;
      });
      return this.BindTable(parameterName, "typ_OrderedObjectIdTable", rows.Select<Sha1Id, SqlDataRecord>(selector));
    }

    private void BindDataspace(RepoScope scopeToBind) => this.BindInt("@dataspaceId", this.GetDataspaceId(scopeToBind.ProjectId));

    private class GitCommitStatusBinder : ObjectBinder<KeyValuePair<Sha1Id, GitStatus>>
    {
      private SqlColumnBinder commitId = new SqlColumnBinder("CommitId");
      private SqlColumnBinder statusId = new SqlColumnBinder("StatusId");
      private SqlColumnBinder state = new SqlColumnBinder("State");
      private SqlColumnBinder context = new SqlColumnBinder("Context");
      private SqlColumnBinder genre = new SqlColumnBinder("Genre");
      private SqlColumnBinder description = new SqlColumnBinder("Description");
      private SqlColumnBinder targetUrl = new SqlColumnBinder("TargetUrl");
      private SqlColumnBinder createdOn = new SqlColumnBinder("CreatedOn");
      private SqlColumnBinder createdBy = new SqlColumnBinder("CreatedBy");

      protected override KeyValuePair<Sha1Id, GitStatus> Bind() => new KeyValuePair<Sha1Id, GitStatus>(this.commitId.GetSha1Id((IDataReader) this.Reader), new GitStatus()
      {
        Id = this.statusId.GetInt32((IDataReader) this.Reader),
        State = (GitStatusState) this.state.GetByte((IDataReader) this.Reader),
        Context = new GitStatusContext()
        {
          Name = this.context.GetString((IDataReader) this.Reader, false),
          Genre = this.genre.GetString((IDataReader) this.Reader, true)
        },
        Description = this.description.GetString((IDataReader) this.Reader, true),
        TargetUrl = this.targetUrl.GetString((IDataReader) this.Reader, true),
        CreationDate = this.createdOn.GetDateTime((IDataReader) this.Reader),
        CreatedBy = new IdentityRef()
        {
          Id = this.createdBy.GetGuid((IDataReader) this.Reader).ToString()
        }
      });
    }
  }
}
