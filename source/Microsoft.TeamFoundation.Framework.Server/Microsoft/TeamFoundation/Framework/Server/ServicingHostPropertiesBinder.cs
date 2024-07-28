// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ServicingHostPropertiesBinder
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using System;
using System.Data;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal sealed class ServicingHostPropertiesBinder : ObjectBinder<ServicingHostProperties>
  {
    private SqlColumnBinder IdColumn = new SqlColumnBinder("HostId");
    private SqlColumnBinder ParentIdColumn = new SqlColumnBinder("ParentHostId");
    private SqlColumnBinder DatabaseIdColumn = new SqlColumnBinder("DatabaseId");
    private SqlColumnBinder ServiceLevelColumn = new SqlColumnBinder("ServiceLevel");
    private SqlColumnBinder StatusColumn = new SqlColumnBinder("Status");
    private SqlColumnBinder LastUserAccessColumn = new SqlColumnBinder("LastUserAccess");
    private SqlColumnBinder SubStatus = new SqlColumnBinder(nameof (SubStatus));

    internal ServicingHostPropertiesBinder()
    {
    }

    protected override ServicingHostProperties Bind() => new ServicingHostProperties()
    {
      Id = this.IdColumn.GetGuid((IDataReader) this.Reader),
      ParentId = this.ParentIdColumn.GetGuid((IDataReader) this.Reader, true),
      DatabaseId = this.DatabaseIdColumn.ColumnExists((IDataReader) this.Reader) ? this.DatabaseIdColumn.GetInt32((IDataReader) this.Reader) : DatabaseManagementConstants.InvalidDatabaseId,
      ServiceLevel = string.Intern(this.ServiceLevelColumn.GetString((IDataReader) this.Reader, (string) null)),
      Status = (TeamFoundationServiceHostStatus) this.StatusColumn.GetInt32((IDataReader) this.Reader),
      LastUserAccess = this.LastUserAccessColumn.GetDateTime((IDataReader) this.Reader, DateTime.MinValue),
      SubStatus = (ServiceHostSubStatus) this.SubStatus.GetInt32((IDataReader) this.Reader, 0, 0)
    };
  }
}
