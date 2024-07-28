// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.MailQueueComponent3
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class MailQueueComponent3 : MailQueueComponent2
  {
    private static readonly SqlMetaData[] typ_MailRequestTable2 = new SqlMetaData[15]
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
      new SqlMetaData("MailHeaders", SqlDbType.NVarChar, -1L),
      new SqlMetaData("MailFrom", SqlDbType.NVarChar, -1L),
      new SqlMetaData("MailSender", SqlDbType.NVarChar, -1L)
    };

    protected SqlParameter BindMailRequestTable2(
      string parameterName,
      IEnumerable<MailQueueEntry> rows)
    {
      rows = rows ?? Enumerable.Empty<MailQueueEntry>();
      return this.BindTable(parameterName, "typ_MailRequestTable2", rows.Select<MailQueueEntry, SqlDataRecord>((System.Func<MailQueueEntry, SqlDataRecord>) (entry =>
      {
        SqlDataRecord record = new SqlDataRecord(MailQueueComponent3.typ_MailRequestTable2);
        this.SetRecord(record, entry);
        record.SetString(13, entry.From);
        record.SetString(14, entry.Sender);
        return record;
      })));
    }

    public override void BindToRequestTable(ICollection<MailRequest> newRequestAndIds) => this.BindMailRequestTable2("@newMailRequests", newRequestAndIds.Select<MailRequest, MailQueueEntry>((System.Func<MailRequest, MailQueueEntry>) (r => MailQueueEntry.Create(r, DateTime.UtcNow, this.MaxRetryCount))));

    internal override MailQueueComponent.MailQueueBinder CreateBinder() => (MailQueueComponent.MailQueueBinder) new MailQueueComponent3.MailQueueBinder2();

    internal class MailQueueBinder2 : MailQueueComponent.MailQueueBinder
    {
      private SqlColumnBinder MailFromColumn = new SqlColumnBinder("MailFrom");
      private SqlColumnBinder MailSenderColumn = new SqlColumnBinder("MailSender");

      protected override MailQueueEntry Bind()
      {
        MailQueueEntry mailQueueEntry = base.Bind();
        mailQueueEntry.From = this.MailFromColumn.GetString((IDataReader) this.Reader, true);
        mailQueueEntry.Sender = this.MailSenderColumn.GetString((IDataReader) this.Reader, true);
        return mailQueueEntry;
      }
    }
  }
}
