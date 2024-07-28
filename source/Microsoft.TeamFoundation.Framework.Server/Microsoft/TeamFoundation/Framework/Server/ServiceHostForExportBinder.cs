// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ServiceHostForExportBinder
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using System;
using System.Data;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class ServiceHostForExportBinder : ObjectBinder<ServiceHostForExportEntry>
  {
    private SqlColumnBinder HostIdColumn = new SqlColumnBinder("HostId");
    private SqlColumnBinder ParentHostIdColumn = new SqlColumnBinder("ParentHostId");
    private SqlColumnBinder NameColumn = new SqlColumnBinder("Name");
    private SqlColumnBinder StatusColumn = new SqlColumnBinder("Status");
    private SqlColumnBinder StatusReasonColumn = new SqlColumnBinder("StatusReason");
    private SqlColumnBinder HostTypeColumn = new SqlColumnBinder("HostType");
    private SqlColumnBinder LastUserAccessColumn = new SqlColumnBinder("LastUserAccess");
    private SqlColumnBinder DatabaseIdColumn = new SqlColumnBinder("DatabaseId");

    protected override ServiceHostForExportEntry Bind() => new ServiceHostForExportEntry()
    {
      HostId = this.HostIdColumn.GetGuid((IDataReader) this.Reader),
      ParentHostId = this.ParentHostIdColumn.GetGuid((IDataReader) this.Reader, true, Guid.Empty),
      Name = this.NameColumn.GetString((IDataReader) this.Reader, true),
      Status = (TeamFoundationServiceHostStatus) this.StatusColumn.GetInt32((IDataReader) this.Reader, 0),
      StatusReason = this.StatusReasonColumn.GetString((IDataReader) this.Reader, true),
      HostType = (TeamFoundationHostType) this.HostTypeColumn.GetInt32((IDataReader) this.Reader, 0),
      LastUserAccess = this.LastUserAccessColumn.GetDateTime((IDataReader) this.Reader),
      DatabaseId = this.DatabaseIdColumn.ColumnExists((IDataReader) this.Reader) ? this.DatabaseIdColumn.GetInt32((IDataReader) this.Reader) : 0
    };
  }
}
