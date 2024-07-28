// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.ExternalArtifacts.Components.ExternalArtifactSqlComponent
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccess;
using Microsoft.TeamFoundation.WorkItemTracking.Server.ExternalArtifacts.DataModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.ExternalArtifacts.Components
{
  internal class ExternalArtifactSqlComponent : WorkItemTrackingResourceComponent
  {
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[4]
    {
      (IComponentCreator) new ComponentCreator<ExternalArtifactSqlComponent>(1, false),
      (IComponentCreator) new ComponentCreator<ExternalArtifactSqlComponent2>(2, false),
      (IComponentCreator) new ComponentCreator<ExternalArtifactSqlComponent3>(3, false),
      (IComponentCreator) new ComponentCreator<ExternalArtifactSqlComponent4>(4, false)
    }, "ExternalArtifact", "WorkItem");

    public virtual ExternalArtifactResult GetArtifacts(
      IEnumerable<(Guid repositoryId, string sha)> repositoryAndShas,
      IEnumerable<(Guid repositoryId, string prId)> repositoryAndPrIds,
      IEnumerable<(Guid repositoryId, string issueId)> repositoryAndIssueIds)
    {
      this.PrepareStoredProcedure("prc_GetExternalArtifacts");
      this.BindKeyValuePairGuidStringTable("@repositoryAndCommitArtifactIds", repositoryAndShas != null ? repositoryAndShas.Select<(Guid, string), KeyValuePair<Guid, string>>((System.Func<(Guid, string), KeyValuePair<Guid, string>>) (kvp => new KeyValuePair<Guid, string>(kvp.repositoryId, kvp.sha))) : (IEnumerable<KeyValuePair<Guid, string>>) null);
      this.BindKeyValuePairGuidStringTable("@repositoryAndPrArtifactIds", repositoryAndPrIds != null ? repositoryAndPrIds.Select<(Guid, string), KeyValuePair<Guid, string>>((System.Func<(Guid, string), KeyValuePair<Guid, string>>) (kvp => new KeyValuePair<Guid, string>(kvp.repositoryId, kvp.prId))) : (IEnumerable<KeyValuePair<Guid, string>>) null);
      return this.ExecuteUnknown<ExternalArtifactResult>((System.Func<IDataReader, ExternalArtifactResult>) (reader =>
      {
        List<ExternalCommitDataset> list1 = this.GetExternalCommitBinder().BindAll(reader).ToList<ExternalCommitDataset>();
        reader.NextResult();
        List<ExternalPullRequestDataset> list2 = this.GetExternalPullRequestBinder().BindAll(reader).ToList<ExternalPullRequestDataset>();
        reader.NextResult();
        List<ExternalArtifactUserResultSet> list3 = this.GetExternalUserBinder().BindAll(reader).ToList<ExternalArtifactUserResultSet>();
        return new ExternalArtifactResult()
        {
          Commits = (IEnumerable<ExternalCommitDataset>) list1,
          PullRequests = (IEnumerable<ExternalPullRequestDataset>) list2,
          Users = (IEnumerable<ExternalArtifactUserResultSet>) list3
        };
      }));
    }

    public virtual PendingExternalArtifactResult GetPendingArtifacts()
    {
      this.PrepareStoredProcedure("prc_GetPendingExternalArtifacts");
      this.BindInt("@batchSize", 200);
      return this.ExecuteUnknown<PendingExternalArtifactResult>((System.Func<IDataReader, PendingExternalArtifactResult>) (reader =>
      {
        List<PendingExternalArtifactDataSet> list1 = this.GetPendingExternalArtifactBinder().BindAll(reader).ToList<PendingExternalArtifactDataSet>();
        reader.NextResult();
        List<PendingExternalArtifactDataSet> list2 = this.GetPendingExternalArtifactBinder().BindAll(reader).ToList<PendingExternalArtifactDataSet>();
        return new PendingExternalArtifactResult()
        {
          Commits = (IEnumerable<PendingExternalArtifactDataSet>) list1,
          PullRequests = (IEnumerable<PendingExternalArtifactDataSet>) list2
        };
      }));
    }

    public virtual void SaveArtifacts(
      IEnumerable<ExternalUserDataset> users,
      IEnumerable<ExternalArtifactUserDataset> artifactUsers,
      IEnumerable<ExternalCommitDataset> commits,
      IEnumerable<ExternalPullRequestDataset> pullRequests,
      IEnumerable<ExternalIssueDataset> issues)
    {
      throw new NotImplementedException();
    }

    protected virtual void BindExternalUserTable(
      string parameterName,
      IEnumerable<ExternalUserDataset> users)
    {
      new ExternalArtifactSqlComponent.ExternalUserTable(parameterName, users).BindTable(this);
    }

    protected virtual void BindExternalArtifactUsersTable(
      string parameterName,
      IEnumerable<ExternalArtifactUserDataset> artifactUsers)
    {
      new ExternalArtifactSqlComponent.ExternalArtifactUserTable(parameterName, artifactUsers).BindTable(this);
    }

    protected virtual void BindExternalCommitTable(
      string parameterName,
      IEnumerable<ExternalCommitDataset> commits)
    {
      new ExternalArtifactSqlComponent.ExternalCommitTable(parameterName, commits).BindTable(this);
    }

    protected virtual void BindExternalPullRequestTable(
      string parameterName,
      IEnumerable<ExternalPullRequestDataset> pullRequests)
    {
      new ExternalArtifactSqlComponent.ExternalPullRequestTable(parameterName, pullRequests).BindTable(this);
    }

    protected virtual ExternalArtifactSqlComponent.ExternalCommitBinder GetExternalCommitBinder() => new ExternalArtifactSqlComponent.ExternalCommitBinder();

    protected virtual ExternalArtifactSqlComponent.ExternalPullRequestBinder GetExternalPullRequestBinder() => new ExternalArtifactSqlComponent.ExternalPullRequestBinder();

    protected virtual ExternalArtifactSqlComponent.ExternalArtifactUserBinder GetExternalUserBinder() => new ExternalArtifactSqlComponent.ExternalArtifactUserBinder();

    protected virtual ExternalArtifactSqlComponent.PendingExternalArtifactBinder GetPendingExternalArtifactBinder() => new ExternalArtifactSqlComponent.PendingExternalArtifactBinder();

    public virtual IEnumerable<ExternalDeploymentDataset> GetDeploymentArtifactLinks(int workitemId) => throw new NotImplementedException();

    public virtual Guid CreateDeploymentArtifactLinks(
      IEnumerable<int> workitemIds,
      ExternalDeploymentDataset deployment)
    {
      throw new NotImplementedException();
    }

    protected virtual ExternalArtifactSqlComponent.ExternalDeploymentBinder GetExternalDeploymentBinder() => new ExternalArtifactSqlComponent.ExternalDeploymentBinder();

    protected class PendingExternalArtifactBinder : 
      WorkItemTrackingObjectBinder<PendingExternalArtifactDataSet>
    {
      protected SqlColumnBinder m_providerKey = new SqlColumnBinder("ProviderKey");
      protected SqlColumnBinder m_repositoryId = new SqlColumnBinder("RepositoryId");
      protected SqlColumnBinder m_externalId = new SqlColumnBinder("ExternalId");
      protected SqlColumnBinder m_artifactId = new SqlColumnBinder("ArtifactId");
      protected SqlColumnBinder m_statusDetails = new SqlColumnBinder("StatusDetails");

      public override PendingExternalArtifactDataSet Bind(IDataReader reader) => new PendingExternalArtifactDataSet()
      {
        ProviderKey = this.m_providerKey.GetString(reader, false),
        ExternalRepositoryId = this.m_externalId.GetString(reader, false),
        InternalRepositoryId = this.m_repositoryId.GetGuid(reader),
        ArtifactId = this.m_artifactId.GetString(reader, false),
        HydrationStatusDetails = this.m_statusDetails.GetString(reader, true)
      };
    }

    protected abstract class ExternalArtifactBinder<T> : WorkItemTrackingObjectBinder<T> where T : ExternalArtifactDataSet, new()
    {
      protected SqlColumnBinder m_repositoryId = new SqlColumnBinder("RepositoryId");
      protected SqlColumnBinder m_repositoryExternalId = new SqlColumnBinder("RepositoryExternalId");
      protected SqlColumnBinder m_repositoryName = new SqlColumnBinder("RepositoryName");
      protected SqlColumnBinder m_repositoryUrl = new SqlColumnBinder("RepositoryUrl");
      protected SqlColumnBinder m_artifactId = new SqlColumnBinder("ArtifactId");
      protected SqlColumnBinder m_secondaryId = new SqlColumnBinder("SecondaryId");
      protected SqlColumnBinder m_url = new SqlColumnBinder("Url");

      public override T Bind(IDataReader reader)
      {
        T obj = new T();
        obj.InternalRepositoryId = this.m_repositoryId.GetGuid(reader);
        obj.ExternalRepositoryId = this.m_repositoryExternalId.GetString(reader, false);
        obj.RepositoryNameWithOwner = this.m_repositoryName.GetString(reader, false);
        obj.RepositoryUrl = this.m_repositoryUrl.GetString(reader, false);
        obj.ArtifactId = this.m_artifactId.GetString(reader, false);
        obj.SecondaryId = this.m_secondaryId.GetString(reader, true);
        obj.Url = this.m_url.GetString(reader, true);
        return obj;
      }
    }

    protected class ExternalCommitBinder : 
      ExternalArtifactSqlComponent.ExternalArtifactBinder<ExternalCommitDataset>
    {
      protected SqlColumnBinder m_message = new SqlColumnBinder("Message");
      protected SqlColumnBinder m_committedDate = new SqlColumnBinder("CommittedDate");
      protected SqlColumnBinder m_pushedDate = new SqlColumnBinder("PushedDate");
      protected SqlColumnBinder m_authorId = new SqlColumnBinder("AuthorId");
      protected SqlColumnBinder m_authorName = new SqlColumnBinder("AuthorName");
      protected SqlColumnBinder m_authorLogin = new SqlColumnBinder("AuthorLogin");
      protected SqlColumnBinder m_authorEmail = new SqlColumnBinder("AuthorEmail");
      protected SqlColumnBinder m_authorAvatarUrl = new SqlColumnBinder("AuthorAvatarUrl");

      public override ExternalCommitDataset Bind(IDataReader reader)
      {
        ExternalCommitDataset externalCommitDataset = base.Bind(reader);
        externalCommitDataset.Message = this.m_message.GetString(reader, true);
        externalCommitDataset.CommittedDate = this.m_committedDate.GetNullableDateTime(reader);
        externalCommitDataset.PushedDate = this.m_pushedDate.GetNullableDateTime(reader);
        externalCommitDataset.AuthorId = this.m_authorId.GetString(reader, true);
        externalCommitDataset.AuthorName = this.m_authorName.GetString(reader, true);
        externalCommitDataset.AuthorLogin = this.m_authorLogin.GetString(reader, true);
        externalCommitDataset.AuthorEmail = this.m_authorEmail.GetString(reader, true);
        externalCommitDataset.AuthorAvatarUrl = this.m_authorAvatarUrl.GetString(reader, true);
        return externalCommitDataset;
      }
    }

    protected class ExternalPullRequestBinder : 
      ExternalArtifactSqlComponent.ExternalArtifactBinder<ExternalPullRequestDataset>
    {
      protected SqlColumnBinder m_title = new SqlColumnBinder("Title");
      protected SqlColumnBinder m_userId = new SqlColumnBinder("UserId");
      protected SqlColumnBinder m_userName = new SqlColumnBinder("UserName");
      protected SqlColumnBinder m_userLogin = new SqlColumnBinder("UserLogin");
      protected SqlColumnBinder m_userAvatarUrl = new SqlColumnBinder("UserAvatarUrl");
      protected SqlColumnBinder m_createdDate = new SqlColumnBinder("CreatedDate");
      protected SqlColumnBinder m_updatedDate = new SqlColumnBinder("UpdatedDate");
      protected SqlColumnBinder m_mergedDate = new SqlColumnBinder("MergedDate");
      protected SqlColumnBinder m_closedDate = new SqlColumnBinder("ClosedDate");
      protected SqlColumnBinder m_target = new SqlColumnBinder("Target");
      protected SqlColumnBinder m_state = new SqlColumnBinder("State");

      public override ExternalPullRequestDataset Bind(IDataReader reader)
      {
        ExternalPullRequestDataset pullRequestDataset = base.Bind(reader);
        pullRequestDataset.UserId = this.m_userId.GetString(reader, true);
        pullRequestDataset.UserName = this.m_userName.GetString(reader, true);
        pullRequestDataset.UserLogin = this.m_userLogin.GetString(reader, true);
        pullRequestDataset.UserAvatarUrl = this.m_userAvatarUrl.GetString(reader, true);
        pullRequestDataset.CreatedDate = this.m_createdDate.GetNullableDateTime(reader);
        pullRequestDataset.UpdatedDate = this.m_updatedDate.GetNullableDateTime(reader);
        pullRequestDataset.MergedDate = this.m_mergedDate.GetNullableDateTime(reader);
        pullRequestDataset.ClosedDate = this.m_closedDate.GetNullableDateTime(reader);
        pullRequestDataset.Title = this.m_title.GetString(reader, true);
        pullRequestDataset.Target = this.m_target.GetString(reader, true);
        pullRequestDataset.State = this.m_state.GetString(reader, true);
        return pullRequestDataset;
      }
    }

    protected class ExternalIssueBinder : 
      ExternalArtifactSqlComponent.ExternalArtifactBinder<ExternalIssueDataset>
    {
      protected SqlColumnBinder m_title = new SqlColumnBinder("Title");
      protected SqlColumnBinder m_userId = new SqlColumnBinder("UserId");
      protected SqlColumnBinder m_userName = new SqlColumnBinder("UserName");
      protected SqlColumnBinder m_userLogin = new SqlColumnBinder("UserLogin");
      protected SqlColumnBinder m_userAvatarUrl = new SqlColumnBinder("UserAvatarUrl");
      protected SqlColumnBinder m_createdDate = new SqlColumnBinder("CreatedDate");
      protected SqlColumnBinder m_updatedDate = new SqlColumnBinder("UpdatedDate");
      protected SqlColumnBinder m_closedDate = new SqlColumnBinder("ClosedDate");
      protected SqlColumnBinder m_state = new SqlColumnBinder("State");

      public override ExternalIssueDataset Bind(IDataReader reader)
      {
        ExternalIssueDataset externalIssueDataset = base.Bind(reader);
        externalIssueDataset.UserId = this.m_userId.GetString(reader, true);
        externalIssueDataset.UserName = this.m_userName.GetString(reader, true);
        externalIssueDataset.UserLogin = this.m_userLogin.GetString(reader, true);
        externalIssueDataset.UserAvatarUrl = this.m_userAvatarUrl.GetString(reader, true);
        externalIssueDataset.CreatedDate = this.m_createdDate.GetNullableDateTime(reader);
        externalIssueDataset.UpdatedDate = this.m_updatedDate.GetNullableDateTime(reader);
        externalIssueDataset.ClosedDate = this.m_closedDate.GetNullableDateTime(reader);
        externalIssueDataset.Title = this.m_title.GetString(reader, true);
        externalIssueDataset.State = this.m_state.GetString(reader, true);
        return externalIssueDataset;
      }
    }

    protected class ExternalArtifactUserBinder : 
      WorkItemTrackingObjectBinder<ExternalArtifactUserResultSet>
    {
      protected SqlColumnBinder m_repositoryId = new SqlColumnBinder("RepositoryId");
      protected SqlColumnBinder m_artifactId = new SqlColumnBinder("ArtifactId");
      protected SqlColumnBinder m_userId = new SqlColumnBinder("UserId");
      protected SqlColumnBinder m_userName = new SqlColumnBinder("UserName");
      protected SqlColumnBinder m_userLogin = new SqlColumnBinder("UserLogin");
      protected SqlColumnBinder m_userAvatarUrl = new SqlColumnBinder("UserAvatarUrl");
      protected SqlColumnBinder m_relationshipType = new SqlColumnBinder("RelationshipType");

      public override ExternalArtifactUserResultSet Bind(IDataReader reader) => new ExternalArtifactUserResultSet()
      {
        InternalRepositoryId = this.m_repositoryId.GetGuid(reader),
        UserId = this.m_userId.GetString(reader, true),
        UserName = this.m_userName.GetString(reader, true),
        UserLogin = this.m_userLogin.GetString(reader, true),
        UserAvatarUrl = this.m_userAvatarUrl.GetString(reader, true),
        RelationshipType = (ExternalArtifactRelationshipType) this.m_relationshipType.GetByte(reader)
      };
    }

    protected class ExternalDeploymentBinder : 
      WorkItemTrackingObjectBinder<ExternalDeploymentDataset>
    {
      protected SqlColumnBinder m_artifactId = new SqlColumnBinder("ArtifactId");
      protected SqlColumnBinder m_pipelineId = new SqlColumnBinder("PipelineId");
      protected SqlColumnBinder m_pipelineDispalyName = new SqlColumnBinder("PipelineDisplayName");
      protected SqlColumnBinder m_pipelineUrl = new SqlColumnBinder("PipelineUrl");
      protected SqlColumnBinder m_environmentId = new SqlColumnBinder("EnvironmentId");
      protected SqlColumnBinder m_environmentDisplayName = new SqlColumnBinder("EnvironmentDisplayName");
      protected SqlColumnBinder m_environmentType = new SqlColumnBinder("EnvironmentType");
      protected SqlColumnBinder m_runId = new SqlColumnBinder("RunId");
      protected SqlColumnBinder m_sequenceNumber = new SqlColumnBinder("SequenceNumber");
      protected SqlColumnBinder m_displayName = new SqlColumnBinder("DisplayName");
      protected SqlColumnBinder m_description = new SqlColumnBinder("Description");
      protected SqlColumnBinder m_status = new SqlColumnBinder("Status");
      protected SqlColumnBinder m_group = new SqlColumnBinder("Group");
      protected SqlColumnBinder m_url = new SqlColumnBinder("Url");
      protected SqlColumnBinder m_statusDate = new SqlColumnBinder("StatusDate");
      protected SqlColumnBinder m_createdDate = new SqlColumnBinder("CreatedDate");
      protected SqlColumnBinder m_createdBy = new SqlColumnBinder("CreatedBy");

      public override ExternalDeploymentDataset Bind(IDataReader reader) => new ExternalDeploymentDataset()
      {
        ArtifactId = this.m_artifactId.GetGuid(reader, false),
        PipelineId = this.m_pipelineId.GetInt32(reader),
        PipelineDisplayName = this.m_pipelineDispalyName.GetString(reader, false),
        PipelineUrl = this.m_pipelineUrl.GetString(reader, false),
        EnvironmentId = this.m_environmentId.GetInt32(reader),
        EnvironmentDisplayName = this.m_environmentDisplayName.GetString(reader, false),
        EnvironmentType = this.m_environmentType.GetString(reader, false),
        RunId = this.m_runId.GetInt32(reader),
        SequenceNumber = this.m_sequenceNumber.GetInt32(reader),
        DisplayName = this.m_displayName.GetString(reader, false),
        Description = this.m_description.GetString(reader, true),
        Status = this.m_status.GetString(reader, false),
        Group = this.m_group.GetString(reader, true),
        Url = this.m_url.GetString(reader, false),
        StatusDate = this.m_statusDate.GetDateTime(reader),
        CreatedDate = this.m_createdDate.GetDateTime(reader),
        CreatedBy = this.m_createdBy.GetGuid(reader, false)
      };
    }

    public class ExternalUserTable
    {
      private string parameterName;
      private IEnumerable<ExternalUserDataset> users;
      private static readonly SqlMetaData[] typ_ExternalUserTable = new SqlMetaData[5]
      {
        new SqlMetaData("ProviderKey", SqlDbType.NVarChar, 400L),
        new SqlMetaData("Id", SqlDbType.NVarChar, 400L),
        new SqlMetaData("Name", SqlDbType.NVarChar, (long) byte.MaxValue),
        new SqlMetaData("Login", SqlDbType.NVarChar, (long) byte.MaxValue),
        new SqlMetaData("AvatarUrl", SqlDbType.NVarChar, 4000L)
      };

      public ExternalUserTable(string parameterName, IEnumerable<ExternalUserDataset> users)
      {
        this.parameterName = parameterName;
        this.users = users;
      }

      public void BindTable(ExternalArtifactSqlComponent component) => component.Bind<ExternalUserDataset>(this.parameterName, this.users, ExternalArtifactSqlComponent.ExternalUserTable.typ_ExternalUserTable, "typ_ExternalUserTable", (Action<SqlDataRecord, ExternalUserDataset>) ((record, user) =>
      {
        int num1 = 0;
        SqlDataRecord sqlDataRecord1 = record;
        int ordinal1 = num1;
        int num2 = ordinal1 + 1;
        string providerKey = user.ProviderKey;
        sqlDataRecord1.SetString(ordinal1, providerKey);
        SqlDataRecord sqlDataRecord2 = record;
        int ordinal2 = num2;
        int num3 = ordinal2 + 1;
        string userId = user.UserId;
        sqlDataRecord2.SetString(ordinal2, userId);
        SqlDataRecord record1 = record;
        int ordinal3 = num3;
        int num4 = ordinal3 + 1;
        string userName = user.UserName;
        record1.SetNullableString(ordinal3, userName);
        SqlDataRecord record2 = record;
        int ordinal4 = num4;
        int num5 = ordinal4 + 1;
        string userLogin = user.UserLogin;
        record2.SetNullableString(ordinal4, userLogin);
        SqlDataRecord record3 = record;
        int ordinal5 = num5;
        int num6 = ordinal5 + 1;
        string userAvatarUrl = user.UserAvatarUrl;
        record3.SetNullableString(ordinal5, userAvatarUrl);
      }));
    }

    public class ExternalUserTable2
    {
      private string parameterName;
      private IEnumerable<ExternalUserDataset> users;
      private static readonly SqlMetaData[] typ_ExternalUserTable2 = new SqlMetaData[9]
      {
        new SqlMetaData("ProviderKey", SqlDbType.NVarChar, 400L),
        new SqlMetaData("Id", SqlDbType.NVarChar, 400L),
        new SqlMetaData("Name", SqlDbType.NVarChar, (long) byte.MaxValue),
        new SqlMetaData("Login", SqlDbType.NVarChar, (long) byte.MaxValue),
        new SqlMetaData("AvatarUrl", SqlDbType.NVarChar, 4000L),
        new SqlMetaData("UpdateOnly", SqlDbType.Bit),
        new SqlMetaData("RepositoryId", SqlDbType.UniqueIdentifier),
        new SqlMetaData("ArtifactId", SqlDbType.NVarChar, 400L),
        new SqlMetaData("ArtifactType", SqlDbType.TinyInt)
      };

      public ExternalUserTable2(string parameterName, IEnumerable<ExternalUserDataset> users)
      {
        this.parameterName = parameterName;
        this.users = users;
      }

      public void BindTable(ExternalArtifactSqlComponent component) => component.Bind<ExternalUserDataset>(this.parameterName, this.users, ExternalArtifactSqlComponent.ExternalUserTable2.typ_ExternalUserTable2, "typ_ExternalUserTable2", (Action<SqlDataRecord, ExternalUserDataset>) ((record, user) =>
      {
        int num1 = 0;
        SqlDataRecord sqlDataRecord1 = record;
        int ordinal1 = num1;
        int num2 = ordinal1 + 1;
        string providerKey = user.ProviderKey;
        sqlDataRecord1.SetString(ordinal1, providerKey);
        SqlDataRecord sqlDataRecord2 = record;
        int ordinal2 = num2;
        int num3 = ordinal2 + 1;
        string userId = user.UserId;
        sqlDataRecord2.SetString(ordinal2, userId);
        SqlDataRecord record1 = record;
        int ordinal3 = num3;
        int num4 = ordinal3 + 1;
        string userName = user.UserName;
        record1.SetNullableString(ordinal3, userName);
        SqlDataRecord record2 = record;
        int ordinal4 = num4;
        int num5 = ordinal4 + 1;
        string userLogin = user.UserLogin;
        record2.SetNullableString(ordinal4, userLogin);
        SqlDataRecord record3 = record;
        int ordinal5 = num5;
        int num6 = ordinal5 + 1;
        string userAvatarUrl = user.UserAvatarUrl;
        record3.SetNullableString(ordinal5, userAvatarUrl);
        SqlDataRecord sqlDataRecord3 = record;
        int ordinal6 = num6;
        int num7 = ordinal6 + 1;
        int num8 = user.UpdateOnly ? 1 : 0;
        sqlDataRecord3.SetBoolean(ordinal6, num8 != 0);
        SqlDataRecord sqlDataRecord4 = record;
        int ordinal7 = num7;
        int num9 = ordinal7 + 1;
        Guid internalRepositoryId = user.RelatedInternalRepositoryId;
        sqlDataRecord4.SetGuid(ordinal7, internalRepositoryId);
        SqlDataRecord sqlDataRecord5 = record;
        int ordinal8 = num9;
        int num10 = ordinal8 + 1;
        string relatedArtifactId = user.RelatedArtifactId;
        sqlDataRecord5.SetString(ordinal8, relatedArtifactId);
        SqlDataRecord sqlDataRecord6 = record;
        int ordinal9 = num10;
        int num11 = ordinal9 + 1;
        int relatedArtifactType = (int) user.RelatedArtifactType;
        sqlDataRecord6.SetByte(ordinal9, (byte) relatedArtifactType);
      }));
    }

    public class ExternalArtifactUserTable
    {
      private string parameterName;
      private IEnumerable<ExternalArtifactUserDataset> artifactUsers;
      private static readonly SqlMetaData[] typ_ExternalArtifactUserTable = new SqlMetaData[4]
      {
        new SqlMetaData("RepositoryId", SqlDbType.UniqueIdentifier),
        new SqlMetaData("ArtifactId", SqlDbType.NVarChar, 400L),
        new SqlMetaData("UserId", SqlDbType.NVarChar, 400L),
        new SqlMetaData("RelationshipType", SqlDbType.TinyInt)
      };

      public ExternalArtifactUserTable(
        string parameterName,
        IEnumerable<ExternalArtifactUserDataset> artifactUsers)
      {
        this.parameterName = parameterName;
        this.artifactUsers = artifactUsers;
      }

      public void BindTable(ExternalArtifactSqlComponent component) => component.Bind<ExternalArtifactUserDataset>(this.parameterName, this.artifactUsers, ExternalArtifactSqlComponent.ExternalArtifactUserTable.typ_ExternalArtifactUserTable, "typ_ExternalArtifactUserTable", (Action<SqlDataRecord, ExternalArtifactUserDataset>) ((record, artifactUser) =>
      {
        int num1 = 0;
        SqlDataRecord sqlDataRecord1 = record;
        int ordinal1 = num1;
        int num2 = ordinal1 + 1;
        Guid internalRepositoryId = artifactUser.InternalRepositoryId;
        sqlDataRecord1.SetGuid(ordinal1, internalRepositoryId);
        SqlDataRecord sqlDataRecord2 = record;
        int ordinal2 = num2;
        int num3 = ordinal2 + 1;
        string artifactId = artifactUser.ArtifactId;
        sqlDataRecord2.SetString(ordinal2, artifactId);
        SqlDataRecord sqlDataRecord3 = record;
        int ordinal3 = num3;
        int num4 = ordinal3 + 1;
        string userId = artifactUser.UserId;
        sqlDataRecord3.SetString(ordinal3, userId);
        SqlDataRecord sqlDataRecord4 = record;
        int ordinal4 = num4;
        int num5 = ordinal4 + 1;
        int relationshipType = (int) artifactUser.RelationshipType;
        sqlDataRecord4.SetByte(ordinal4, (byte) relationshipType);
      }));
    }

    public class ExternalArtifactUserTable2
    {
      private string parameterName;
      private IEnumerable<ExternalArtifactUserDataset> artifactUsers;
      private static readonly SqlMetaData[] typ_ExternalArtifactUserTable2 = new SqlMetaData[5]
      {
        new SqlMetaData("RepositoryId", SqlDbType.UniqueIdentifier),
        new SqlMetaData("ArtifactId", SqlDbType.NVarChar, 400L),
        new SqlMetaData("UserId", SqlDbType.NVarChar, 400L),
        new SqlMetaData("RelationshipType", SqlDbType.TinyInt),
        new SqlMetaData("UpdateOnly", SqlDbType.Bit)
      };

      public ExternalArtifactUserTable2(
        string parameterName,
        IEnumerable<ExternalArtifactUserDataset> artifactUsers)
      {
        this.parameterName = parameterName;
        this.artifactUsers = artifactUsers;
      }

      public void BindTable(ExternalArtifactSqlComponent component) => component.Bind<ExternalArtifactUserDataset>(this.parameterName, this.artifactUsers, ExternalArtifactSqlComponent.ExternalArtifactUserTable2.typ_ExternalArtifactUserTable2, "typ_ExternalArtifactUserTable2", (Action<SqlDataRecord, ExternalArtifactUserDataset>) ((record, artifactUser) =>
      {
        int num1 = 0;
        SqlDataRecord sqlDataRecord1 = record;
        int ordinal1 = num1;
        int num2 = ordinal1 + 1;
        Guid internalRepositoryId = artifactUser.InternalRepositoryId;
        sqlDataRecord1.SetGuid(ordinal1, internalRepositoryId);
        SqlDataRecord sqlDataRecord2 = record;
        int ordinal2 = num2;
        int num3 = ordinal2 + 1;
        string artifactId = artifactUser.ArtifactId;
        sqlDataRecord2.SetString(ordinal2, artifactId);
        SqlDataRecord sqlDataRecord3 = record;
        int ordinal3 = num3;
        int num4 = ordinal3 + 1;
        string userId = artifactUser.UserId;
        sqlDataRecord3.SetString(ordinal3, userId);
        SqlDataRecord sqlDataRecord4 = record;
        int ordinal4 = num4;
        int num5 = ordinal4 + 1;
        int relationshipType = (int) artifactUser.RelationshipType;
        sqlDataRecord4.SetByte(ordinal4, (byte) relationshipType);
        SqlDataRecord sqlDataRecord5 = record;
        int ordinal5 = num5;
        int num6 = ordinal5 + 1;
        int num7 = artifactUser.UpdateOnly ? 1 : 0;
        sqlDataRecord5.SetBoolean(ordinal5, num7 != 0);
      }));
    }

    public class ExternalArtifactUserTable3
    {
      private string parameterName;
      private IEnumerable<ExternalArtifactUserDataset> artifactUsers;
      private static readonly SqlMetaData[] typ_ExternalArtifactUserTable3 = new SqlMetaData[5]
      {
        new SqlMetaData("RepositoryId", SqlDbType.UniqueIdentifier),
        new SqlMetaData("ArtifactId", SqlDbType.NVarChar, 400L),
        new SqlMetaData("UserId", SqlDbType.NVarChar, 400L),
        new SqlMetaData("RelationshipType", SqlDbType.TinyInt),
        new SqlMetaData("UpdateOnly", SqlDbType.Bit)
      };

      public ExternalArtifactUserTable3(
        string parameterName,
        IEnumerable<ExternalArtifactUserDataset> artifactUsers)
      {
        this.parameterName = parameterName;
        this.artifactUsers = artifactUsers;
      }

      public void BindTable(ExternalArtifactSqlComponent component) => component.Bind<ExternalArtifactUserDataset>(this.parameterName, this.artifactUsers, ExternalArtifactSqlComponent.ExternalArtifactUserTable3.typ_ExternalArtifactUserTable3, "typ_ExternalArtifactUserTable3", (Action<SqlDataRecord, ExternalArtifactUserDataset>) ((record, artifactUser) =>
      {
        int num1 = 0;
        SqlDataRecord sqlDataRecord1 = record;
        int ordinal1 = num1;
        int num2 = ordinal1 + 1;
        Guid internalRepositoryId = artifactUser.InternalRepositoryId;
        sqlDataRecord1.SetGuid(ordinal1, internalRepositoryId);
        SqlDataRecord sqlDataRecord2 = record;
        int ordinal2 = num2;
        int num3 = ordinal2 + 1;
        string artifactId = artifactUser.ArtifactId;
        sqlDataRecord2.SetString(ordinal2, artifactId);
        SqlDataRecord record1 = record;
        int ordinal3 = num3;
        int num4 = ordinal3 + 1;
        string userId = artifactUser.UserId;
        record1.SetNullableString(ordinal3, userId);
        SqlDataRecord sqlDataRecord3 = record;
        int ordinal4 = num4;
        int num5 = ordinal4 + 1;
        int relationshipType = (int) artifactUser.RelationshipType;
        sqlDataRecord3.SetByte(ordinal4, (byte) relationshipType);
        SqlDataRecord sqlDataRecord4 = record;
        int ordinal5 = num5;
        int num6 = ordinal5 + 1;
        int num7 = artifactUser.UpdateOnly ? 1 : 0;
        sqlDataRecord4.SetBoolean(ordinal5, num7 != 0);
      }));
    }

    public class ExternalCommitTable
    {
      private string parameterName;
      private IEnumerable<ExternalCommitDataset> commits;
      private static readonly SqlMetaData[] typ_ExternalCommitTable = new SqlMetaData[14]
      {
        new SqlMetaData("RepositoryId", SqlDbType.UniqueIdentifier),
        new SqlMetaData("ArtifactId", SqlDbType.NVarChar, 400L),
        new SqlMetaData("SecondaryId", SqlDbType.NVarChar, 400L),
        new SqlMetaData("AuthorId", SqlDbType.NVarChar, 400L),
        new SqlMetaData("AuthorName", SqlDbType.NVarChar, (long) byte.MaxValue),
        new SqlMetaData("AuthorLogin", SqlDbType.NVarChar, (long) byte.MaxValue),
        new SqlMetaData("AuthorEmail", SqlDbType.NVarChar, 400L),
        new SqlMetaData("AuthorAvatarUrl", SqlDbType.NVarChar, 4000L),
        new SqlMetaData("CommittedDate", SqlDbType.DateTime),
        new SqlMetaData("PushedDate", SqlDbType.DateTime),
        new SqlMetaData("Message", SqlDbType.NVarChar, 4000L),
        new SqlMetaData("Url", SqlDbType.NVarChar, 4000L),
        new SqlMetaData("Status", SqlDbType.TinyInt),
        new SqlMetaData("StatusDetails", SqlDbType.NVarChar, -1L)
      };

      public ExternalCommitTable(string parameterName, IEnumerable<ExternalCommitDataset> commits)
      {
        this.parameterName = parameterName;
        this.commits = commits;
      }

      public void BindTable(ExternalArtifactSqlComponent component) => component.Bind<ExternalCommitDataset>(this.parameterName, this.commits, ExternalArtifactSqlComponent.ExternalCommitTable.typ_ExternalCommitTable, "typ_ExternalCommitTable", (Action<SqlDataRecord, ExternalCommitDataset>) ((record, commit) =>
      {
        int num1 = 0;
        SqlDataRecord sqlDataRecord1 = record;
        int ordinal1 = num1;
        int num2 = ordinal1 + 1;
        Guid internalRepositoryId = commit.InternalRepositoryId;
        sqlDataRecord1.SetGuid(ordinal1, internalRepositoryId);
        SqlDataRecord sqlDataRecord2 = record;
        int ordinal2 = num2;
        int num3 = ordinal2 + 1;
        string artifactId = commit.ArtifactId;
        sqlDataRecord2.SetString(ordinal2, artifactId);
        SqlDataRecord record1 = record;
        int ordinal3 = num3;
        int num4 = ordinal3 + 1;
        string secondaryId = commit.SecondaryId;
        record1.SetNullableString(ordinal3, secondaryId);
        SqlDataRecord record2 = record;
        int ordinal4 = num4;
        int num5 = ordinal4 + 1;
        string authorId = commit.AuthorId;
        record2.SetNullableString(ordinal4, authorId);
        SqlDataRecord record3 = record;
        int ordinal5 = num5;
        int num6 = ordinal5 + 1;
        string authorName = commit.AuthorName;
        record3.SetNullableString(ordinal5, authorName);
        SqlDataRecord record4 = record;
        int ordinal6 = num6;
        int num7 = ordinal6 + 1;
        string authorLogin = commit.AuthorLogin;
        record4.SetNullableString(ordinal6, authorLogin);
        SqlDataRecord record5 = record;
        int ordinal7 = num7;
        int num8 = ordinal7 + 1;
        string authorEmail = commit.AuthorEmail;
        record5.SetNullableString(ordinal7, authorEmail);
        SqlDataRecord record6 = record;
        int ordinal8 = num8;
        int num9 = ordinal8 + 1;
        string authorAvatarUrl = commit.AuthorAvatarUrl;
        record6.SetNullableString(ordinal8, authorAvatarUrl);
        SqlDataRecord record7 = record;
        int ordinal9 = num9;
        int num10 = ordinal9 + 1;
        DateTime? committedDate = commit.CommittedDate;
        record7.SetNullableDateTime(ordinal9, committedDate);
        SqlDataRecord record8 = record;
        int ordinal10 = num10;
        int num11 = ordinal10 + 1;
        DateTime? pushedDate = commit.PushedDate;
        record8.SetNullableDateTime(ordinal10, pushedDate);
        SqlDataRecord record9 = record;
        int ordinal11 = num11;
        int num12 = ordinal11 + 1;
        string message = commit.Message;
        record9.SetNullableString(ordinal11, message);
        SqlDataRecord record10 = record;
        int ordinal12 = num12;
        int num13 = ordinal12 + 1;
        string url = commit.Url;
        record10.SetNullableString(ordinal12, url);
        SqlDataRecord record11 = record;
        int ordinal13 = num13;
        int num14 = ordinal13 + 1;
        byte? nullable = new byte?(commit.HydrationStatus);
        record11.SetNullableByte(ordinal13, nullable);
        SqlDataRecord record12 = record;
        int ordinal14 = num14;
        int num15 = ordinal14 + 1;
        string hydrationStatusDetails = commit.HydrationStatusDetails;
        record12.SetNullableString(ordinal14, hydrationStatusDetails);
      }));
    }

    public class ExternalCommitTable2
    {
      private string parameterName;
      private IEnumerable<ExternalCommitDataset> commits;
      private static readonly SqlMetaData[] typ_ExternalCommitTable2 = new SqlMetaData[15]
      {
        new SqlMetaData("RepositoryId", SqlDbType.UniqueIdentifier),
        new SqlMetaData("ArtifactId", SqlDbType.NVarChar, 400L),
        new SqlMetaData("SecondaryId", SqlDbType.NVarChar, 400L),
        new SqlMetaData("AuthorId", SqlDbType.NVarChar, 400L),
        new SqlMetaData("AuthorName", SqlDbType.NVarChar, (long) byte.MaxValue),
        new SqlMetaData("AuthorLogin", SqlDbType.NVarChar, (long) byte.MaxValue),
        new SqlMetaData("AuthorEmail", SqlDbType.NVarChar, 400L),
        new SqlMetaData("AuthorAvatarUrl", SqlDbType.NVarChar, 4000L),
        new SqlMetaData("CommittedDate", SqlDbType.DateTime),
        new SqlMetaData("PushedDate", SqlDbType.DateTime),
        new SqlMetaData("Message", SqlDbType.NVarChar, 4000L),
        new SqlMetaData("Url", SqlDbType.NVarChar, 4000L),
        new SqlMetaData("Status", SqlDbType.TinyInt),
        new SqlMetaData("StatusDetails", SqlDbType.NVarChar, -1L),
        new SqlMetaData("UpdateOnly", SqlDbType.Bit)
      };

      public ExternalCommitTable2(string parameterName, IEnumerable<ExternalCommitDataset> commits)
      {
        this.parameterName = parameterName;
        this.commits = commits;
      }

      public void BindTable(ExternalArtifactSqlComponent component) => component.Bind<ExternalCommitDataset>(this.parameterName, this.commits, ExternalArtifactSqlComponent.ExternalCommitTable2.typ_ExternalCommitTable2, "typ_ExternalCommitTable2", (Action<SqlDataRecord, ExternalCommitDataset>) ((record, commit) =>
      {
        int num1 = 0;
        SqlDataRecord sqlDataRecord1 = record;
        int ordinal1 = num1;
        int num2 = ordinal1 + 1;
        Guid internalRepositoryId = commit.InternalRepositoryId;
        sqlDataRecord1.SetGuid(ordinal1, internalRepositoryId);
        SqlDataRecord sqlDataRecord2 = record;
        int ordinal2 = num2;
        int num3 = ordinal2 + 1;
        string artifactId = commit.ArtifactId;
        sqlDataRecord2.SetString(ordinal2, artifactId);
        SqlDataRecord record1 = record;
        int ordinal3 = num3;
        int num4 = ordinal3 + 1;
        string secondaryId = commit.SecondaryId;
        record1.SetNullableString(ordinal3, secondaryId);
        SqlDataRecord record2 = record;
        int ordinal4 = num4;
        int num5 = ordinal4 + 1;
        string authorId = commit.AuthorId;
        record2.SetNullableString(ordinal4, authorId);
        SqlDataRecord record3 = record;
        int ordinal5 = num5;
        int num6 = ordinal5 + 1;
        string authorName = commit.AuthorName;
        record3.SetNullableString(ordinal5, authorName);
        SqlDataRecord record4 = record;
        int ordinal6 = num6;
        int num7 = ordinal6 + 1;
        string authorLogin = commit.AuthorLogin;
        record4.SetNullableString(ordinal6, authorLogin);
        SqlDataRecord record5 = record;
        int ordinal7 = num7;
        int num8 = ordinal7 + 1;
        string authorEmail = commit.AuthorEmail;
        record5.SetNullableString(ordinal7, authorEmail);
        SqlDataRecord record6 = record;
        int ordinal8 = num8;
        int num9 = ordinal8 + 1;
        string authorAvatarUrl = commit.AuthorAvatarUrl;
        record6.SetNullableString(ordinal8, authorAvatarUrl);
        SqlDataRecord record7 = record;
        int ordinal9 = num9;
        int num10 = ordinal9 + 1;
        DateTime? committedDate = commit.CommittedDate;
        record7.SetNullableDateTime(ordinal9, committedDate);
        SqlDataRecord record8 = record;
        int ordinal10 = num10;
        int num11 = ordinal10 + 1;
        DateTime? pushedDate = commit.PushedDate;
        record8.SetNullableDateTime(ordinal10, pushedDate);
        SqlDataRecord record9 = record;
        int ordinal11 = num11;
        int num12 = ordinal11 + 1;
        string message = commit.Message;
        record9.SetNullableString(ordinal11, message);
        SqlDataRecord record10 = record;
        int ordinal12 = num12;
        int num13 = ordinal12 + 1;
        string url = commit.Url;
        record10.SetNullableString(ordinal12, url);
        SqlDataRecord record11 = record;
        int ordinal13 = num13;
        int num14 = ordinal13 + 1;
        byte? nullable = new byte?(commit.HydrationStatus);
        record11.SetNullableByte(ordinal13, nullable);
        SqlDataRecord record12 = record;
        int ordinal14 = num14;
        int num15 = ordinal14 + 1;
        string hydrationStatusDetails = commit.HydrationStatusDetails;
        record12.SetNullableString(ordinal14, hydrationStatusDetails);
        SqlDataRecord sqlDataRecord3 = record;
        int ordinal15 = num15;
        int num16 = ordinal15 + 1;
        int num17 = commit.UpdateOnly ? 1 : 0;
        sqlDataRecord3.SetBoolean(ordinal15, num17 != 0);
      }));
    }

    public class ExternalPullRequestTable
    {
      private string parameterName;
      private IEnumerable<ExternalPullRequestDataset> pullRequests;
      private static readonly SqlMetaData[] typ_ExternalPullRequestTable = new SqlMetaData[14]
      {
        new SqlMetaData("RepositoryId", SqlDbType.UniqueIdentifier),
        new SqlMetaData("ArtifactId", SqlDbType.NVarChar, 400L),
        new SqlMetaData("SecondaryId", SqlDbType.NVarChar, 400L),
        new SqlMetaData("UserId", SqlDbType.NVarChar, 400L),
        new SqlMetaData("CreatedDate", SqlDbType.DateTime),
        new SqlMetaData("UpdatedDate", SqlDbType.DateTime),
        new SqlMetaData("MergedDate", SqlDbType.DateTime),
        new SqlMetaData("ClosedDate", SqlDbType.DateTime),
        new SqlMetaData("Title", SqlDbType.NVarChar, 4000L),
        new SqlMetaData("Target", SqlDbType.NVarChar, 400L),
        new SqlMetaData("State", SqlDbType.NVarChar, 64L),
        new SqlMetaData("Url", SqlDbType.NVarChar, 4000L),
        new SqlMetaData("Status", SqlDbType.TinyInt),
        new SqlMetaData("StatusDetails", SqlDbType.NVarChar, -1L)
      };

      public ExternalPullRequestTable(
        string parameterName,
        IEnumerable<ExternalPullRequestDataset> pullRequests)
      {
        this.parameterName = parameterName;
        this.pullRequests = pullRequests;
      }

      public void BindTable(ExternalArtifactSqlComponent component) => component.Bind<ExternalPullRequestDataset>(this.parameterName, this.pullRequests, ExternalArtifactSqlComponent.ExternalPullRequestTable.typ_ExternalPullRequestTable, "typ_ExternalPullRequestTable", (Action<SqlDataRecord, ExternalPullRequestDataset>) ((record, pullRequest) =>
      {
        int num1 = 0;
        SqlDataRecord sqlDataRecord1 = record;
        int ordinal1 = num1;
        int num2 = ordinal1 + 1;
        Guid internalRepositoryId = pullRequest.InternalRepositoryId;
        sqlDataRecord1.SetGuid(ordinal1, internalRepositoryId);
        SqlDataRecord sqlDataRecord2 = record;
        int ordinal2 = num2;
        int num3 = ordinal2 + 1;
        string artifactId = pullRequest.ArtifactId;
        sqlDataRecord2.SetString(ordinal2, artifactId);
        SqlDataRecord record1 = record;
        int ordinal3 = num3;
        int num4 = ordinal3 + 1;
        string secondaryId = pullRequest.SecondaryId;
        record1.SetNullableString(ordinal3, secondaryId);
        SqlDataRecord record2 = record;
        int ordinal4 = num4;
        int num5 = ordinal4 + 1;
        string userId = pullRequest.UserId;
        record2.SetNullableString(ordinal4, userId);
        SqlDataRecord record3 = record;
        int ordinal5 = num5;
        int num6 = ordinal5 + 1;
        DateTime? createdDate = pullRequest.CreatedDate;
        record3.SetNullableDateTime(ordinal5, createdDate);
        SqlDataRecord record4 = record;
        int ordinal6 = num6;
        int num7 = ordinal6 + 1;
        DateTime? updatedDate = pullRequest.UpdatedDate;
        record4.SetNullableDateTime(ordinal6, updatedDate);
        SqlDataRecord record5 = record;
        int ordinal7 = num7;
        int num8 = ordinal7 + 1;
        DateTime? mergedDate = pullRequest.MergedDate;
        record5.SetNullableDateTime(ordinal7, mergedDate);
        SqlDataRecord record6 = record;
        int ordinal8 = num8;
        int num9 = ordinal8 + 1;
        DateTime? closedDate = pullRequest.ClosedDate;
        record6.SetNullableDateTime(ordinal8, closedDate);
        SqlDataRecord record7 = record;
        int ordinal9 = num9;
        int num10 = ordinal9 + 1;
        string title = pullRequest.Title;
        record7.SetNullableString(ordinal9, title);
        SqlDataRecord record8 = record;
        int ordinal10 = num10;
        int num11 = ordinal10 + 1;
        string target = pullRequest.Target;
        record8.SetNullableString(ordinal10, target);
        SqlDataRecord record9 = record;
        int ordinal11 = num11;
        int num12 = ordinal11 + 1;
        string state = pullRequest.State;
        record9.SetNullableString(ordinal11, state);
        SqlDataRecord record10 = record;
        int ordinal12 = num12;
        int num13 = ordinal12 + 1;
        string url = pullRequest.Url;
        record10.SetNullableString(ordinal12, url);
        SqlDataRecord record11 = record;
        int ordinal13 = num13;
        int num14 = ordinal13 + 1;
        byte? nullable = new byte?(pullRequest.HydrationStatus);
        record11.SetNullableByte(ordinal13, nullable);
        SqlDataRecord record12 = record;
        int ordinal14 = num14;
        int num15 = ordinal14 + 1;
        string hydrationStatusDetails = pullRequest.HydrationStatusDetails;
        record12.SetNullableString(ordinal14, hydrationStatusDetails);
      }));
    }

    public class ExternalPullRequestTable2
    {
      private string parameterName;
      private IEnumerable<ExternalPullRequestDataset> pullRequests;
      private static readonly SqlMetaData[] typ_ExternalPullRequestTable2 = new SqlMetaData[15]
      {
        new SqlMetaData("RepositoryId", SqlDbType.UniqueIdentifier),
        new SqlMetaData("ArtifactId", SqlDbType.NVarChar, 400L),
        new SqlMetaData("SecondaryId", SqlDbType.NVarChar, 400L),
        new SqlMetaData("UserId", SqlDbType.NVarChar, 400L),
        new SqlMetaData("CreatedDate", SqlDbType.DateTime),
        new SqlMetaData("UpdatedDate", SqlDbType.DateTime),
        new SqlMetaData("MergedDate", SqlDbType.DateTime),
        new SqlMetaData("ClosedDate", SqlDbType.DateTime),
        new SqlMetaData("Title", SqlDbType.NVarChar, 4000L),
        new SqlMetaData("Target", SqlDbType.NVarChar, 400L),
        new SqlMetaData("State", SqlDbType.NVarChar, 64L),
        new SqlMetaData("Url", SqlDbType.NVarChar, 4000L),
        new SqlMetaData("Status", SqlDbType.TinyInt),
        new SqlMetaData("StatusDetails", SqlDbType.NVarChar, -1L),
        new SqlMetaData("UpdateOnly", SqlDbType.Bit)
      };

      public ExternalPullRequestTable2(
        string parameterName,
        IEnumerable<ExternalPullRequestDataset> pullRequests)
      {
        this.parameterName = parameterName;
        this.pullRequests = pullRequests;
      }

      public void BindTable(ExternalArtifactSqlComponent component) => component.Bind<ExternalPullRequestDataset>(this.parameterName, this.pullRequests, ExternalArtifactSqlComponent.ExternalPullRequestTable2.typ_ExternalPullRequestTable2, "typ_ExternalPullRequestTable2", (Action<SqlDataRecord, ExternalPullRequestDataset>) ((record, pullRequest) =>
      {
        int num1 = 0;
        SqlDataRecord sqlDataRecord1 = record;
        int ordinal1 = num1;
        int num2 = ordinal1 + 1;
        Guid internalRepositoryId = pullRequest.InternalRepositoryId;
        sqlDataRecord1.SetGuid(ordinal1, internalRepositoryId);
        SqlDataRecord sqlDataRecord2 = record;
        int ordinal2 = num2;
        int num3 = ordinal2 + 1;
        string artifactId = pullRequest.ArtifactId;
        sqlDataRecord2.SetString(ordinal2, artifactId);
        SqlDataRecord record1 = record;
        int ordinal3 = num3;
        int num4 = ordinal3 + 1;
        string secondaryId = pullRequest.SecondaryId;
        record1.SetNullableString(ordinal3, secondaryId);
        SqlDataRecord record2 = record;
        int ordinal4 = num4;
        int num5 = ordinal4 + 1;
        string userId = pullRequest.UserId;
        record2.SetNullableString(ordinal4, userId);
        SqlDataRecord record3 = record;
        int ordinal5 = num5;
        int num6 = ordinal5 + 1;
        DateTime? createdDate = pullRequest.CreatedDate;
        record3.SetNullableDateTime(ordinal5, createdDate);
        SqlDataRecord record4 = record;
        int ordinal6 = num6;
        int num7 = ordinal6 + 1;
        DateTime? updatedDate = pullRequest.UpdatedDate;
        record4.SetNullableDateTime(ordinal6, updatedDate);
        SqlDataRecord record5 = record;
        int ordinal7 = num7;
        int num8 = ordinal7 + 1;
        DateTime? mergedDate = pullRequest.MergedDate;
        record5.SetNullableDateTime(ordinal7, mergedDate);
        SqlDataRecord record6 = record;
        int ordinal8 = num8;
        int num9 = ordinal8 + 1;
        DateTime? closedDate = pullRequest.ClosedDate;
        record6.SetNullableDateTime(ordinal8, closedDate);
        SqlDataRecord record7 = record;
        int ordinal9 = num9;
        int num10 = ordinal9 + 1;
        string title = pullRequest.Title;
        record7.SetNullableString(ordinal9, title);
        SqlDataRecord record8 = record;
        int ordinal10 = num10;
        int num11 = ordinal10 + 1;
        string target = pullRequest.Target;
        record8.SetNullableString(ordinal10, target);
        SqlDataRecord record9 = record;
        int ordinal11 = num11;
        int num12 = ordinal11 + 1;
        string state = pullRequest.State;
        record9.SetNullableString(ordinal11, state);
        SqlDataRecord record10 = record;
        int ordinal12 = num12;
        int num13 = ordinal12 + 1;
        string url = pullRequest.Url;
        record10.SetNullableString(ordinal12, url);
        SqlDataRecord record11 = record;
        int ordinal13 = num13;
        int num14 = ordinal13 + 1;
        byte? nullable = new byte?(pullRequest.HydrationStatus);
        record11.SetNullableByte(ordinal13, nullable);
        SqlDataRecord record12 = record;
        int ordinal14 = num14;
        int num15 = ordinal14 + 1;
        string hydrationStatusDetails = pullRequest.HydrationStatusDetails;
        record12.SetNullableString(ordinal14, hydrationStatusDetails);
        SqlDataRecord sqlDataRecord3 = record;
        int ordinal15 = num15;
        int num16 = ordinal15 + 1;
        int num17 = pullRequest.UpdateOnly ? 1 : 0;
        sqlDataRecord3.SetBoolean(ordinal15, num17 != 0);
      }));
    }

    public class ExternalIssueTable
    {
      private string parameterName;
      private IEnumerable<ExternalIssueDataset> issues;
      private static readonly SqlMetaData[] typ_ExternalIssueTable = new SqlMetaData[12]
      {
        new SqlMetaData("RepositoryId", SqlDbType.UniqueIdentifier),
        new SqlMetaData("ArtifactId", SqlDbType.NVarChar, 400L),
        new SqlMetaData("SecondaryId", SqlDbType.NVarChar, 400L),
        new SqlMetaData("UserId", SqlDbType.NVarChar, 400L),
        new SqlMetaData("CreatedDate", SqlDbType.DateTime),
        new SqlMetaData("UpdatedDate", SqlDbType.DateTime),
        new SqlMetaData("ClosedDate", SqlDbType.DateTime),
        new SqlMetaData("Title", SqlDbType.NVarChar, 4000L),
        new SqlMetaData("State", SqlDbType.NVarChar, 64L),
        new SqlMetaData("Url", SqlDbType.NVarChar, 4000L),
        new SqlMetaData("Status", SqlDbType.TinyInt),
        new SqlMetaData("StatusDetails", SqlDbType.NVarChar, -1L)
      };

      public ExternalIssueTable(string parameterName, IEnumerable<ExternalIssueDataset> issues)
      {
        this.parameterName = parameterName;
        this.issues = issues;
      }

      public void BindTable(ExternalArtifactSqlComponent component) => component.Bind<ExternalIssueDataset>(this.parameterName, this.issues, ExternalArtifactSqlComponent.ExternalIssueTable.typ_ExternalIssueTable, "typ_ExternalIssueTable", (Action<SqlDataRecord, ExternalIssueDataset>) ((record, issue) =>
      {
        int num1 = 0;
        SqlDataRecord sqlDataRecord1 = record;
        int ordinal1 = num1;
        int num2 = ordinal1 + 1;
        Guid internalRepositoryId = issue.InternalRepositoryId;
        sqlDataRecord1.SetGuid(ordinal1, internalRepositoryId);
        SqlDataRecord sqlDataRecord2 = record;
        int ordinal2 = num2;
        int num3 = ordinal2 + 1;
        string artifactId = issue.ArtifactId;
        sqlDataRecord2.SetString(ordinal2, artifactId);
        SqlDataRecord record1 = record;
        int ordinal3 = num3;
        int num4 = ordinal3 + 1;
        string secondaryId = issue.SecondaryId;
        record1.SetNullableString(ordinal3, secondaryId);
        SqlDataRecord record2 = record;
        int ordinal4 = num4;
        int num5 = ordinal4 + 1;
        string userId = issue.UserId;
        record2.SetNullableString(ordinal4, userId);
        SqlDataRecord record3 = record;
        int ordinal5 = num5;
        int num6 = ordinal5 + 1;
        DateTime? createdDate = issue.CreatedDate;
        record3.SetNullableDateTime(ordinal5, createdDate);
        SqlDataRecord record4 = record;
        int ordinal6 = num6;
        int num7 = ordinal6 + 1;
        DateTime? updatedDate = issue.UpdatedDate;
        record4.SetNullableDateTime(ordinal6, updatedDate);
        SqlDataRecord record5 = record;
        int ordinal7 = num7;
        int num8 = ordinal7 + 1;
        DateTime? closedDate = issue.ClosedDate;
        record5.SetNullableDateTime(ordinal7, closedDate);
        SqlDataRecord record6 = record;
        int ordinal8 = num8;
        int num9 = ordinal8 + 1;
        string title = issue.Title;
        record6.SetNullableString(ordinal8, title);
        SqlDataRecord record7 = record;
        int ordinal9 = num9;
        int num10 = ordinal9 + 1;
        string state = issue.State;
        record7.SetNullableString(ordinal9, state);
        SqlDataRecord record8 = record;
        int ordinal10 = num10;
        int num11 = ordinal10 + 1;
        string url = issue.Url;
        record8.SetNullableString(ordinal10, url);
        SqlDataRecord record9 = record;
        int ordinal11 = num11;
        int num12 = ordinal11 + 1;
        byte? nullable = new byte?(issue.HydrationStatus);
        record9.SetNullableByte(ordinal11, nullable);
        SqlDataRecord record10 = record;
        int ordinal12 = num12;
        int num13 = ordinal12 + 1;
        string hydrationStatusDetails = issue.HydrationStatusDetails;
        record10.SetNullableString(ordinal12, hydrationStatusDetails);
      }));
    }

    public class ExternalIssueTable2
    {
      private string parameterName;
      private IEnumerable<ExternalIssueDataset> issues;
      private static readonly SqlMetaData[] typ_ExternalIssueTable2 = new SqlMetaData[13]
      {
        new SqlMetaData("RepositoryId", SqlDbType.UniqueIdentifier),
        new SqlMetaData("ArtifactId", SqlDbType.NVarChar, 400L),
        new SqlMetaData("SecondaryId", SqlDbType.NVarChar, 400L),
        new SqlMetaData("UserId", SqlDbType.NVarChar, 400L),
        new SqlMetaData("CreatedDate", SqlDbType.DateTime),
        new SqlMetaData("UpdatedDate", SqlDbType.DateTime),
        new SqlMetaData("ClosedDate", SqlDbType.DateTime),
        new SqlMetaData("Title", SqlDbType.NVarChar, 4000L),
        new SqlMetaData("State", SqlDbType.NVarChar, 64L),
        new SqlMetaData("Url", SqlDbType.NVarChar, 4000L),
        new SqlMetaData("Status", SqlDbType.TinyInt),
        new SqlMetaData("StatusDetails", SqlDbType.NVarChar, -1L),
        new SqlMetaData("UpdateOnly", SqlDbType.Bit)
      };

      public ExternalIssueTable2(string parameterName, IEnumerable<ExternalIssueDataset> issues)
      {
        this.parameterName = parameterName;
        this.issues = issues;
      }

      public void BindTable(ExternalArtifactSqlComponent component) => component.Bind<ExternalIssueDataset>(this.parameterName, this.issues, ExternalArtifactSqlComponent.ExternalIssueTable2.typ_ExternalIssueTable2, "typ_ExternalIssueTable2", (Action<SqlDataRecord, ExternalIssueDataset>) ((record, issue) =>
      {
        int num1 = 0;
        SqlDataRecord sqlDataRecord1 = record;
        int ordinal1 = num1;
        int num2 = ordinal1 + 1;
        Guid internalRepositoryId = issue.InternalRepositoryId;
        sqlDataRecord1.SetGuid(ordinal1, internalRepositoryId);
        SqlDataRecord sqlDataRecord2 = record;
        int ordinal2 = num2;
        int num3 = ordinal2 + 1;
        string artifactId = issue.ArtifactId;
        sqlDataRecord2.SetString(ordinal2, artifactId);
        SqlDataRecord record1 = record;
        int ordinal3 = num3;
        int num4 = ordinal3 + 1;
        string secondaryId = issue.SecondaryId;
        record1.SetNullableString(ordinal3, secondaryId);
        SqlDataRecord record2 = record;
        int ordinal4 = num4;
        int num5 = ordinal4 + 1;
        string userId = issue.UserId;
        record2.SetNullableString(ordinal4, userId);
        SqlDataRecord record3 = record;
        int ordinal5 = num5;
        int num6 = ordinal5 + 1;
        DateTime? createdDate = issue.CreatedDate;
        record3.SetNullableDateTime(ordinal5, createdDate);
        SqlDataRecord record4 = record;
        int ordinal6 = num6;
        int num7 = ordinal6 + 1;
        DateTime? updatedDate = issue.UpdatedDate;
        record4.SetNullableDateTime(ordinal6, updatedDate);
        SqlDataRecord record5 = record;
        int ordinal7 = num7;
        int num8 = ordinal7 + 1;
        DateTime? closedDate = issue.ClosedDate;
        record5.SetNullableDateTime(ordinal7, closedDate);
        SqlDataRecord record6 = record;
        int ordinal8 = num8;
        int num9 = ordinal8 + 1;
        string title = issue.Title;
        record6.SetNullableString(ordinal8, title);
        SqlDataRecord record7 = record;
        int ordinal9 = num9;
        int num10 = ordinal9 + 1;
        string state = issue.State;
        record7.SetNullableString(ordinal9, state);
        SqlDataRecord record8 = record;
        int ordinal10 = num10;
        int num11 = ordinal10 + 1;
        string url = issue.Url;
        record8.SetNullableString(ordinal10, url);
        SqlDataRecord record9 = record;
        int ordinal11 = num11;
        int num12 = ordinal11 + 1;
        byte? nullable = new byte?(issue.HydrationStatus);
        record9.SetNullableByte(ordinal11, nullable);
        SqlDataRecord record10 = record;
        int ordinal12 = num12;
        int num13 = ordinal12 + 1;
        string hydrationStatusDetails = issue.HydrationStatusDetails;
        record10.SetNullableString(ordinal12, hydrationStatusDetails);
        SqlDataRecord sqlDataRecord3 = record;
        int ordinal13 = num13;
        int num14 = ordinal13 + 1;
        int num15 = issue.UpdateOnly ? 1 : 0;
        sqlDataRecord3.SetBoolean(ordinal13, num15 != 0);
      }));
    }
  }
}
