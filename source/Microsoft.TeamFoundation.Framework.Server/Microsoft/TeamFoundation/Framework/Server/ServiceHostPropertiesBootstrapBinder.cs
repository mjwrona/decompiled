// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ServiceHostPropertiesBootstrapBinder
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using System;
using System.Data;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal sealed class ServiceHostPropertiesBootstrapBinder : 
    ObjectBinder<TeamFoundationServiceHostProperties>
  {
    private SqlColumnBinder m_idColumn = new SqlColumnBinder("HostId");
    private SqlColumnBinder m_nameColumn = new SqlColumnBinder("Name");
    private SqlColumnBinder m_descriptionColumn = new SqlColumnBinder("Description");
    private SqlColumnBinder m_statusColumn = new SqlColumnBinder("Status");
    private SqlColumnBinder m_statusReasonColumn = new SqlColumnBinder("StatusReason");
    private SqlColumnBinder m_connectionStringColumn = new SqlColumnBinder("ConnectionString");
    private SqlColumnBinder m_userIdColumn = new SqlColumnBinder("UserId");
    private SqlColumnBinder m_serviceLevelColumn = new SqlColumnBinder("ServiceLevel");

    internal ServiceHostPropertiesBootstrapBinder()
    {
    }

    protected override TeamFoundationServiceHostProperties Bind()
    {
      TeamFoundationServiceHostProperties serviceHostProperties = new TeamFoundationServiceHostProperties();
      serviceHostProperties.Id = this.m_idColumn.GetGuid((IDataReader) this.Reader);
      serviceHostProperties.Name = this.m_nameColumn.GetString((IDataReader) this.Reader, true);
      serviceHostProperties.Description = this.m_descriptionColumn.GetString((IDataReader) this.Reader, true);
      serviceHostProperties.Status = (TeamFoundationServiceHostStatus) this.m_statusColumn.GetInt32((IDataReader) this.Reader);
      serviceHostProperties.StatusReason = this.m_statusReasonColumn.GetString((IDataReader) this.Reader, true);
      serviceHostProperties.LastUserAccess = DateTime.UtcNow;
      serviceHostProperties.ServiceLevel = this.m_serviceLevelColumn.GetString((IDataReader) this.Reader, (string) null);
      return serviceHostProperties;
    }
  }
}
