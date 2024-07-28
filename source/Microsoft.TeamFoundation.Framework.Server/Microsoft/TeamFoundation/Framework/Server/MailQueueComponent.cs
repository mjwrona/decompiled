// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.MailQueueComponent
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Mail;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class MailQueueComponent : TeamFoundationSqlResourceComponent
  {
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[3]
    {
      (IComponentCreator) new ComponentCreator<MailQueueComponent>(1, true),
      (IComponentCreator) new ComponentCreator<MailQueueComponent2>(2),
      (IComponentCreator) new ComponentCreator<MailQueueComponent3>(3)
    }, "SendMail");
    private static readonly SqlMetaData[] typ_MailRequestTable = new SqlMetaData[13]
    {
      new SqlMetaData("Id", SqlDbType.UniqueIdentifier),
      new SqlMetaData("TfId", SqlDbType.UniqueIdentifier),
      new SqlMetaData("Status", SqlDbType.Int),
      new SqlMetaData("QueuedTime", SqlDbType.DateTime),
      new SqlMetaData("RetryCount", SqlDbType.Int),
      new SqlMetaData("MailSubject", SqlDbType.NVarChar, 998L),
      new SqlMetaData("MailBody", SqlDbType.NVarChar, -1L),
      new SqlMetaData("MailTo", SqlDbType.NVarChar, -1L),
      new SqlMetaData("MailCC", SqlDbType.NVarChar, -1L),
      new SqlMetaData("MailBCC", SqlDbType.NVarChar, -1L),
      new SqlMetaData("MailReplyTo", SqlDbType.NVarChar, -1L),
      new SqlMetaData("MailPriority", SqlDbType.Int),
      new SqlMetaData("MailHeaders", SqlDbType.NVarChar, -1L)
    };
    protected const int MaxSubjectLength = 998;

    protected SqlParameter BindMailRequestTable(
      string parameterName,
      IEnumerable<MailQueueEntry> rows)
    {
      rows = rows ?? Enumerable.Empty<MailQueueEntry>();
      System.Func<MailQueueEntry, SqlDataRecord> selector = (System.Func<MailQueueEntry, SqlDataRecord>) (entry =>
      {
        SqlDataRecord record = new SqlDataRecord(MailQueueComponent.typ_MailRequestTable);
        this.SetRecord(record, entry);
        return record;
      });
      return this.BindTable(parameterName, "typ_MailRequestTable", rows.Select<MailQueueEntry, SqlDataRecord>(selector));
    }

    protected void SetRecord(SqlDataRecord record, MailQueueEntry entry)
    {
      record.SetGuid(0, entry.Id);
      record.SetGuid(1, entry.TfId);
      record.SetInt32(2, entry.Status);
      record.SetDateTime(3, entry.QueuedTimeUtc);
      record.SetInt32(4, entry.RetryCount);
      record.SetString(5, entry.Subject);
      record.SetString(6, entry.Body);
      record.SetString(7, entry.To);
      record.SetString(8, entry.CC);
      record.SetString(9, entry.BCC);
      record.SetString(10, entry.ReplyTo);
      record.SetInt32(11, entry.Priority);
      record.SetString(12, entry.Headers);
    }

    public int MaxRetryCount { get; set; }

    public virtual ICollection<MailRequest> AddMailRequests(
      IEnumerable<MailMessage> messages,
      Guid requesterTfId)
    {
      if (!messages.Any<MailMessage>())
        return (ICollection<MailRequest>) new List<MailRequest>(0);
      this.PrepareStoredProcedure("prc_AddMailRequests");
      ICollection<MailRequest> list = (ICollection<MailRequest>) messages.Select<MailMessage, MailRequest>((System.Func<MailMessage, MailRequest>) (m => new MailRequest(m, Guid.NewGuid(), requesterTfId))).ToList<MailRequest>();
      this.BindToRequestTable(list);
      this.ExecuteNonQuery();
      return list;
    }

    public virtual void BindToRequestTable(ICollection<MailRequest> newRequestAndIds) => this.BindMailRequestTable("@newMailRequests", newRequestAndIds.Select<MailRequest, MailQueueEntry>((System.Func<MailRequest, MailQueueEntry>) (r => MailQueueEntry.Create(r, DateTime.UtcNow, this.MaxRetryCount))));

    public void DeleteMailRequests(IEnumerable<Guid> requestIdsForDeletion)
    {
      if (!requestIdsForDeletion.Any<Guid>())
        return;
      this.PrepareStoredProcedure("prc_DeleteMailRequests");
      this.BindGuidTable("@toBeDeletedMailRequestIds", requestIdsForDeletion);
      this.ExecuteNonQuery();
    }

    internal List<MailQueueEntry> GetPendingMailQueueEntries(bool includeFailedRequests)
    {
      this.PrepareStoredProcedure("prc_GetPendingMailRequests");
      this.BindBoolean("@includeFailedRequests", includeFailedRequests);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<MailQueueEntry>((ObjectBinder<MailQueueEntry>) this.CreateBinder());
      return resultCollection.GetCurrent<MailQueueEntry>().Items;
    }

    internal virtual MailQueueComponent.MailQueueBinder CreateBinder() => new MailQueueComponent.MailQueueBinder();

    public void RetryMailRequests(IEnumerable<Guid> retryMailRequestIds)
    {
      if (!retryMailRequestIds.Any<Guid>())
        return;
      this.PrepareStoredProcedure("prc_RetryMailRequests");
      this.BindGuidTable("@toBeRetriedMailRequestIds", retryMailRequestIds);
      this.ExecuteNonQuery();
    }

    internal class MailQueueBinder : ObjectBinder<MailQueueEntry>
    {
      private SqlColumnBinder IdColumn = new SqlColumnBinder("Id");
      private SqlColumnBinder TfIdColumn = new SqlColumnBinder("TfId");
      private SqlColumnBinder StatusColumn = new SqlColumnBinder("Status");
      private SqlColumnBinder QueuedTimeColumn = new SqlColumnBinder("QueuedTime");
      private SqlColumnBinder RetryCountColumn = new SqlColumnBinder("RetryCount");
      private SqlColumnBinder MailSubjectColumn = new SqlColumnBinder("MailSubject");
      private SqlColumnBinder MailBodyColumn = new SqlColumnBinder("MailBody");
      private SqlColumnBinder MailToColumn = new SqlColumnBinder("MailTo");
      private SqlColumnBinder MailCCColumn = new SqlColumnBinder("MailCC");
      private SqlColumnBinder MailBCCColumn = new SqlColumnBinder("MailBCC");
      private SqlColumnBinder MailReplyToColumn = new SqlColumnBinder("MailReplyTo");
      private SqlColumnBinder MailPriorityColumn = new SqlColumnBinder("MailPriority");
      private SqlColumnBinder MailHeadersColumn = new SqlColumnBinder("MailHeaders");

      protected override MailQueueEntry Bind() => new MailQueueEntry()
      {
        Id = this.IdColumn.GetGuid((IDataReader) this.Reader),
        TfId = this.TfIdColumn.GetGuid((IDataReader) this.Reader),
        Status = this.StatusColumn.GetInt32((IDataReader) this.Reader),
        QueuedTimeUtc = this.QueuedTimeColumn.GetDateTime((IDataReader) this.Reader),
        RetryCount = this.RetryCountColumn.GetInt32((IDataReader) this.Reader),
        Subject = this.MailSubjectColumn.GetString((IDataReader) this.Reader, true),
        Body = this.MailBodyColumn.GetString((IDataReader) this.Reader, true),
        To = this.MailToColumn.GetString((IDataReader) this.Reader, false),
        CC = this.MailCCColumn.GetString((IDataReader) this.Reader, true),
        BCC = this.MailBCCColumn.GetString((IDataReader) this.Reader, true),
        ReplyTo = this.MailReplyToColumn.GetString((IDataReader) this.Reader, true),
        Priority = this.MailPriorityColumn.GetInt32((IDataReader) this.Reader, 1),
        Headers = this.MailHeadersColumn.GetString((IDataReader) this.Reader, true)
      };
    }
  }
}
