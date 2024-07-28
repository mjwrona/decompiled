// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ServiceHostHistoryEntryBinder
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using System;
using System.Data;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class ServiceHostHistoryEntryBinder : ObjectBinder<ServiceHostHistoryEntry>
  {
    private SqlColumnBinder HistoryIdColumn = new SqlColumnBinder("HistoryId");
    private SqlColumnBinder InsertedDateColumn = new SqlColumnBinder("InsertedDate");
    private SqlColumnBinder HostIdColumn = new SqlColumnBinder("HostId");
    private SqlColumnBinder ChangeTypeColumn = new SqlColumnBinder("ChangeType");
    private SqlColumnBinder ParentHostIdColumn = new SqlColumnBinder("ParentHostId");
    private SqlColumnBinder DatabaseIdColumn = new SqlColumnBinder("DatabaseId");
    private SqlColumnBinder StatusColumn = new SqlColumnBinder("Status");
    private SqlColumnBinder StatusReasonColumn = new SqlColumnBinder("StatusReason");
    private SqlColumnBinder HostTypeColumn = new SqlColumnBinder("HostType");
    private SqlColumnBinder LastUserAccessColumn = new SqlColumnBinder("LastUserAccess");

    protected override ServiceHostHistoryEntry Bind() => new ServiceHostHistoryEntry()
    {
      HistoryId = this.HistoryIdColumn.GetInt32((IDataReader) this.Reader),
      InsertedDate = this.InsertedDateColumn.GetDateTime((IDataReader) this.Reader),
      HostId = this.HostIdColumn.GetGuid((IDataReader) this.Reader),
      ChangeType = (ServiceHostHistoryChangeType) this.ChangeTypeColumn.GetByte((IDataReader) this.Reader),
      ParentHostId = this.ParentHostIdColumn.GetGuid((IDataReader) this.Reader, true, Guid.Empty),
      DatabaseId = this.DatabaseIdColumn.GetInt32((IDataReader) this.Reader, 0),
      Status = (TeamFoundationServiceHostStatus) this.StatusColumn.GetByte((IDataReader) this.Reader, (byte) 0),
      StatusReason = this.StatusReasonColumn.GetString((IDataReader) this.Reader, true),
      HostType = (TeamFoundationHostType) this.HostTypeColumn.GetInt32((IDataReader) this.Reader, 0),
      LastUserAccess = this.LastUserAccessColumn.GetDateTime((IDataReader) this.Reader)
    };
  }
}
