// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.UserSyncComponent
// Assembly: Microsoft.VisualStudio.Services.Identity, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1372DA81-8681-4BFE-8E91-D1AB4333F834
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Identity.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Identity
{
  internal class UserSyncComponent : TeamFoundationSqlResourceComponent
  {
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[1]
    {
      (IComponentCreator) new ComponentCreator<UserSyncComponent>(1)
    }, "UserSync");
    private static readonly SqlMetaData[] typ_UserSyncQueueDeletion = new SqlMetaData[2]
    {
      new SqlMetaData("UserId", SqlDbType.UniqueIdentifier),
      new SqlMetaData("Revision", SqlDbType.Binary, 8L)
    };

    protected SqlParameter BindUserSyncQueueDeletionTable(
      string parameterName,
      IEnumerable<UserSyncQueueEntry> rows)
    {
      rows = rows ?? Enumerable.Empty<UserSyncQueueEntry>();
      return this.BindTable(parameterName, "typ_UserSyncQueueDeletion", UserSyncComponent.BindUserSyncQueueDeletionRow(rows));
    }

    internal static IEnumerable<SqlDataRecord> BindUserSyncQueueDeletionRow(
      IEnumerable<UserSyncQueueEntry> rows)
    {
      if (rows != null)
      {
        foreach (UserSyncQueueEntry row in rows)
        {
          SqlDataRecord sqlDataRecord = new SqlDataRecord(UserSyncComponent.typ_UserSyncQueueDeletion);
          sqlDataRecord.SetGuid(0, row.UserId);
          sqlDataRecord.SetBytes(1, 0L, row.Revision, 0, row.Revision.Length);
          yield return sqlDataRecord;
        }
      }
    }

    public void AddUserToSyncQueue(Guid userId, int attemptsRemaining)
    {
      this.PrepareStoredProcedure("prc_AddUserToSyncQueue");
      this.BindGuid("@userId", userId);
      this.BindInt("@attemptsRemaining", attemptsRemaining);
      this.ExecuteNonQuery();
    }

    public List<UserSyncQueueEntry> QueryUserSyncQueue(int batchSize)
    {
      this.PrepareStoredProcedure("prc_QueryUserSyncQueue");
      this.BindInt("@batchSize", batchSize);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<UserSyncQueueEntry>((ObjectBinder<UserSyncQueueEntry>) new UserSyncComponent.UserSyncEntryColumns());
      return resultCollection.GetCurrent<UserSyncQueueEntry>().Items;
    }

    public void UpdateUserSyncQueue(
      IEnumerable<UserSyncQueueEntry> usersToDelete,
      IEnumerable<Guid> usersToRetry,
      int retryIntervalSeconds)
    {
      this.PrepareStoredProcedure("prc_UpdateUserSyncQueue");
      this.BindUserSyncQueueDeletionTable("@usersToDelete", usersToDelete);
      this.BindGuidTable("@usersToRetry", usersToRetry);
      this.BindInt("@retryIntervalSeconds", retryIntervalSeconds);
      this.ExecuteNonQuery();
    }

    private class UserSyncEntryColumns : ObjectBinder<UserSyncQueueEntry>
    {
      private SqlColumnBinder UserIdColumn = new SqlColumnBinder("UserId");
      private SqlColumnBinder AttemptsRemainingColumn = new SqlColumnBinder("AttemptsRemaining");
      private SqlColumnBinder RevisionColumn = new SqlColumnBinder("Revision");

      protected override UserSyncQueueEntry Bind() => new UserSyncQueueEntry()
      {
        UserId = this.UserIdColumn.GetGuid((IDataReader) this.Reader),
        AttemptsRemaining = this.AttemptsRemainingColumn.GetInt32((IDataReader) this.Reader),
        Revision = this.RevisionColumn.GetBytes((IDataReader) this.Reader, false)
      };
    }
  }
}
