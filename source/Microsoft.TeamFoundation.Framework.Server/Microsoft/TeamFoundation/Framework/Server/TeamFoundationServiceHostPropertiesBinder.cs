// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.TeamFoundationServiceHostPropertiesBinder
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using System;
using System.Data;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal sealed class TeamFoundationServiceHostPropertiesBinder : 
    TeamFoundationServiceHostBinderBase<TeamFoundationServiceHostProperties>
  {
    private SqlColumnBinder IdColumn = new SqlColumnBinder("HostId");
    private SqlColumnBinder ParentIdColumn = new SqlColumnBinder("ParentHostId");
    private SqlColumnBinder NameColumn = new SqlColumnBinder("Name");
    private SqlColumnBinder DescriptionColumn = new SqlColumnBinder("Description");
    private SqlColumnBinder StatusColumn = new SqlColumnBinder("Status");
    private SqlColumnBinder StatusReasonColumn = new SqlColumnBinder("StatusReason");
    private SqlColumnBinder SubStatusColumn = new SqlColumnBinder("SubStatus");
    private SqlColumnBinder HostTypeColumn = new SqlColumnBinder("HostType");
    private SqlColumnBinder LastUserAccessColumn = new SqlColumnBinder("LastUserAccess");
    private SqlColumnBinder ServicingDetailsColumn = new SqlColumnBinder("ServicingDetails");
    private SqlColumnBinder DatabaseIdColumn = new SqlColumnBinder("DatabaseId");
    private SqlColumnBinder ServiceLevelColumn = new SqlColumnBinder("ServiceLevel");
    private SqlColumnBinder StorageAccountIdColumn = new SqlColumnBinder("StorageAccountId");

    protected override TeamFoundationServiceHostProperties Bind()
    {
      TeamFoundationServiceHostProperties serviceHostProperties = new TeamFoundationServiceHostProperties();
      serviceHostProperties.Id = this.IdColumn.GetGuid((IDataReader) this.Reader);
      serviceHostProperties.ParentId = this.ParentIdColumn.GetGuid((IDataReader) this.Reader, true);
      serviceHostProperties.Name = this.NameColumn.GetString((IDataReader) this.Reader, true);
      serviceHostProperties.Description = this.DescriptionColumn.GetString((IDataReader) this.Reader, true);
      serviceHostProperties.Status = (TeamFoundationServiceHostStatus) this.StatusColumn.GetInt32((IDataReader) this.Reader);
      serviceHostProperties.StatusReason = this.StatusReasonColumn.GetString((IDataReader) this.Reader, true);
      serviceHostProperties.SubStatus = (ServiceHostSubStatus) this.SubStatusColumn.GetInt32((IDataReader) this.Reader, -1, -1);
      serviceHostProperties.LastUserAccess = this.LastUserAccessColumn.GetDateTime((IDataReader) this.Reader, DateTime.MinValue);
      serviceHostProperties.HostType = (TeamFoundationHostType) this.HostTypeColumn.GetInt32((IDataReader) this.Reader);
      serviceHostProperties.Registered = true;
      serviceHostProperties.DatabaseId = this.DatabaseIdColumn.GetInt32((IDataReader) this.Reader, 0, 0);
      serviceHostProperties.ServiceLevel = this.ServiceLevelColumn.GetString((IDataReader) this.Reader, (string) null);
      serviceHostProperties.StorageAccountId = this.StorageAccountIdColumn.GetInt32((IDataReader) this.Reader, 0, 0);
      TeamFoundationServiceHostProperties hostProperties = serviceHostProperties;
      if (this.ServicingDetailsColumn.ColumnExists((IDataReader) this.Reader))
        this.ProcessServicingDetails(hostProperties, this.ServicingDetailsColumn.GetString((IDataReader) this.Reader, true));
      return hostProperties;
    }
  }
}
