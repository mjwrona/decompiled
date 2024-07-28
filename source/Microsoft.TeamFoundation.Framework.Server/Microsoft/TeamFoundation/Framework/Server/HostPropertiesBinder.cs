// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.HostPropertiesBinder
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using System;
using System.Data;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal sealed class HostPropertiesBinder : TeamFoundationServiceHostBinderBase<HostProperties>
  {
    private const string DefaultCollection = "DefaultCollection";
    private SqlColumnBinder IdColumn = new SqlColumnBinder("HostId");
    private SqlColumnBinder ParentIdColumn = new SqlColumnBinder("ParentHostId");
    private SqlColumnBinder NameColumn = new SqlColumnBinder("Name");
    private SqlColumnBinder DescriptionColumn = new SqlColumnBinder("Description");
    private SqlColumnBinder StatusColumn = new SqlColumnBinder("Status");
    private SqlColumnBinder StatusReasonColumn = new SqlColumnBinder("StatusReason");
    private SqlColumnBinder SubStatusColumn = new SqlColumnBinder("SubStatus");
    private SqlColumnBinder HostTypeColumn = new SqlColumnBinder("HostType");
    private SqlColumnBinder LastUserAccessColumn = new SqlColumnBinder("LastUserAccess");
    private SqlColumnBinder DatabaseIdColumn = new SqlColumnBinder("DatabaseId");
    private SqlColumnBinder ServiceLevelColumn = new SqlColumnBinder("ServiceLevel");
    private SqlColumnBinder StorageAccountIdColumn = new SqlColumnBinder("StorageAccountId");

    protected override HostProperties Bind()
    {
      HostProperties hostProperties = new HostProperties()
      {
        Id = this.IdColumn.GetGuid((IDataReader) this.Reader),
        ParentId = this.ParentIdColumn.GetGuid((IDataReader) this.Reader, true),
        Name = this.NameColumn.GetString((IDataReader) this.Reader, true),
        Description = this.DescriptionColumn.GetString((IDataReader) this.Reader, true),
        Status = (TeamFoundationServiceHostStatus) this.StatusColumn.GetInt32((IDataReader) this.Reader),
        StatusReason = this.StatusReasonColumn.GetString((IDataReader) this.Reader, true),
        SubStatus = (ServiceHostSubStatus) this.SubStatusColumn.GetInt32((IDataReader) this.Reader, 0, -1),
        LastUserAccess = this.LastUserAccessColumn.GetDateTime((IDataReader) this.Reader, DateTime.MinValue),
        HostType = (TeamFoundationHostType) this.HostTypeColumn.GetInt32((IDataReader) this.Reader),
        Registered = true,
        DatabaseId = this.DatabaseIdColumn.ColumnExists((IDataReader) this.Reader) ? this.DatabaseIdColumn.GetInt32((IDataReader) this.Reader) : 0,
        ServiceLevel = this.ServiceLevelColumn.GetString((IDataReader) this.Reader, (string) null),
        StorageAccountId = this.StorageAccountIdColumn.GetInt32((IDataReader) this.Reader, 0, 0)
      };
      if (hostProperties.HostType == TeamFoundationHostType.ProjectCollection && string.CompareOrdinal(hostProperties.Name, "DefaultCollection") == 0)
        hostProperties.Name = "DefaultCollection";
      if (!string.IsNullOrEmpty(hostProperties.ServiceLevel))
        hostProperties.ServiceLevel = string.Intern(hostProperties.ServiceLevel);
      return hostProperties;
    }
  }
}
