// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.GitAsyncOperationComponent
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Microsoft.TeamFoundation.Git.Server
{
  internal class GitAsyncOperationComponent : TeamFoundationSqlResourceComponent
  {
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory((IComponentCreator[]) new ComponentCreator<GitAsyncOperationComponent>[3]
    {
      new ComponentCreator<GitAsyncOperationComponent>(1),
      new ComponentCreator<GitAsyncOperationComponent>(2),
      new ComponentCreator<GitAsyncOperationComponent>(3)
    }, "GitAsyncOperation", "Git");
    private static readonly Dictionary<int, SqlExceptionFactory> s_sqlExceptionFactories = new Dictionary<int, SqlExceptionFactory>();

    static GitAsyncOperationComponent()
    {
      GitAsyncOperationComponent.s_sqlExceptionFactories.Add(1200002, new SqlExceptionFactory(typeof (GenericDatabaseUpdateErrorException)));
      GitAsyncOperationComponent.s_sqlExceptionFactories.Add(1200022, new SqlExceptionFactory(typeof (GenericErrorException)));
      GitAsyncOperationComponent.s_sqlExceptionFactories.Add(1200029, new SqlExceptionFactory(typeof (GitAsyncOperationAlreadyExistsException)));
      GitAsyncOperationComponent.s_sqlExceptionFactories.Add(1200028, new SqlExceptionFactory(typeof (GitAsyncOperationNotFoundException)));
      GitAsyncOperationComponent.s_sqlExceptionFactories.Add(1200030, new SqlExceptionFactory(typeof (GitAsyncOperationUpdateFailedException)));
      GitAsyncOperationComponent.s_sqlExceptionFactories.Add(1200009, new SqlExceptionFactory(typeof (GitRepositoryNotFoundException)));
    }

    protected override IDictionary<int, SqlExceptionFactory> TranslatedExceptions => (IDictionary<int, SqlExceptionFactory>) GitAsyncOperationComponent.s_sqlExceptionFactories;

    public GitAsyncOperationComponent()
    {
      this.SelectedFeatures = SqlResourceComponentFeatures.BindPartitionIdByDefault;
      this.ContainerErrorCode = 50000;
    }

    public GitAsyncOperation CreateAsyncOperation(
      RepoKey repoKey,
      GitAsyncOperationType operationType,
      Guid creatorId,
      string parameters)
    {
      this.PrepareStoredProcedure("prc_CreateGitAsyncOperation");
      if (this.Version >= 3)
        this.BindDataspace((RepoScope) repoKey);
      this.BindGuid("@repositoryId", repoKey.RepoId);
      this.BindByte("@operationType", (byte) operationType);
      this.BindGuid("@creatorId", creatorId);
      this.BindString("@parameters", parameters, 4000, true, SqlDbType.NVarChar);
      this.BindByte("@operationStatus", (byte) 1);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        SqlColumnBinder operationIdCol = new SqlColumnBinder("OperationId");
        resultCollection.AddBinder<int>((ObjectBinder<int>) new SimpleObjectBinder<int>((System.Func<IDataReader, int>) (reader => operationIdCol.GetInt32(reader))));
        SqlColumnBinder dateTimeCol = new SqlColumnBinder("CreationDate");
        resultCollection.AddBinder<DateTime>((ObjectBinder<DateTime>) new SimpleObjectBinder<DateTime>((System.Func<IDataReader, DateTime>) (reader => dateTimeCol.GetDateTime(reader))));
        int operationId = resultCollection.GetCurrent<int>().First<int>();
        resultCollection.NextResult();
        DateTime creationDate = resultCollection.GetCurrent<DateTime>().First<DateTime>();
        return new GitAsyncOperation(repoKey.RepoId, operationId, operationType, creatorId, creationDate, parameters, GitAsyncOperationStatus.Queued, (string) null);
      }
    }

    public GitAsyncOperation GetAsyncOperationByOperationId(RepoKey repoKey, int operationId)
    {
      this.PrepareStoredProcedure("prc_GetGitAsyncOperationByOperationId");
      if (this.Version >= 3)
        this.BindDataspace((RepoScope) repoKey);
      this.BindGuid("@repositoryId", repoKey.RepoId);
      this.BindInt("@operationId", operationId);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<GitAsyncOperation>((ObjectBinder<GitAsyncOperation>) new GitAsyncOperationComponent.GitAsyncOperationBinder());
        return resultCollection.GetCurrent<GitAsyncOperation>().FirstOrDefault<GitAsyncOperation>();
      }
    }

    public void UpdateAsyncOperationStatus(
      RepoKey repoKey,
      int operationId,
      GitAsyncOperationStatus status,
      string detailedStatus,
      GitAsyncOperationStatus? previousStatus = null)
    {
      this.PrepareStoredProcedure("prc_UpdateGitAsyncOperationStatus");
      if (this.Version >= 3)
        this.BindDataspace((RepoScope) repoKey);
      this.BindGuid("@repositoryId", repoKey.RepoId);
      this.BindInt("@operationId", operationId);
      this.BindByte("@operationStatus", (byte) status);
      this.BindString("@detailedStatus", detailedStatus, 4000, true, SqlDbType.NVarChar);
      if (previousStatus.HasValue && this.Version >= 2)
        this.BindByte("@previousStatus", (byte) previousStatus.Value);
      this.ExecuteNonQuery();
    }

    public List<GitAsyncOperation> QueryAsyncOperationsByOperationType(
      RepoKey repoKey,
      GitAsyncOperationType operationType)
    {
      this.PrepareStoredProcedure("prc_QueryGitAsyncOperationsByOperationType");
      if (this.Version >= 3)
        this.BindDataspace((RepoScope) repoKey);
      this.BindNullableGuid("@repositoryId", repoKey.RepoId);
      this.BindByte("@operationType", (byte) operationType);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<GitAsyncOperation>((ObjectBinder<GitAsyncOperation>) new GitAsyncOperationComponent.GitAsyncOperationBinder());
        return resultCollection.GetCurrent<GitAsyncOperation>().Items;
      }
    }

    private void BindDataspace(RepoScope scopeToBind) => this.BindInt("@dataspaceId", this.GetDataspaceId(scopeToBind.ProjectId));

    private class GitAsyncOperationBinder : ObjectBinder<GitAsyncOperation>
    {
      internal SqlColumnBinder repositoryId = new SqlColumnBinder("RepositoryId");
      internal SqlColumnBinder operationId = new SqlColumnBinder("OperationId");
      internal SqlColumnBinder operationType = new SqlColumnBinder("OperationType");
      internal SqlColumnBinder creatorId = new SqlColumnBinder("CreatorId");
      internal SqlColumnBinder creationDate = new SqlColumnBinder("CreationDate");
      internal SqlColumnBinder parameters = new SqlColumnBinder("Parameters");
      internal SqlColumnBinder status = new SqlColumnBinder("Status");
      internal SqlColumnBinder detailedStatus = new SqlColumnBinder("DetailedStatus");

      protected override GitAsyncOperation Bind() => new GitAsyncOperation(this.repositoryId.GetGuid((IDataReader) this.Reader), this.operationId.GetInt32((IDataReader) this.Reader), (GitAsyncOperationType) this.operationType.GetByte((IDataReader) this.Reader), this.creatorId.GetGuid((IDataReader) this.Reader), this.creationDate.GetDateTime((IDataReader) this.Reader), this.parameters.GetString((IDataReader) this.Reader, true), (GitAsyncOperationStatus) this.status.GetByte((IDataReader) this.Reader), this.detailedStatus.GetString((IDataReader) this.Reader, true));
    }
  }
}
