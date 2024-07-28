// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.HostMigrationRequestBinder
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.VisualStudio.Services.Cloud
{
  internal class HostMigrationRequestBinder : ObjectBinder<HostMigrationRequest>
  {
    private SqlColumnBinder m_hostIdColumn = new SqlColumnBinder("HostId");
    private SqlColumnBinder m_targetInstanceNameColumn = new SqlColumnBinder("TargetInstanceName");
    private SqlColumnBinder m_targetDatabaseIdColumn = new SqlColumnBinder("TargetDatabaseId");
    private SqlColumnBinder m_hostsAffectedByTheMoveColumn = new SqlColumnBinder("HostsAffectedByTheMove");
    private SqlColumnBinder m_priorityColumn = new SqlColumnBinder("Priority");
    private SqlColumnBinder m_optionsColumnColumn = new SqlColumnBinder("Options");
    private SqlColumnBinder m_migrationIdColumn = new SqlColumnBinder("MigrationId");
    private SqlColumnBinder m_driverJobIdColumn = new SqlColumnBinder("DriverJobId");

    protected override HostMigrationRequest Bind() => new HostMigrationRequest()
    {
      HostId = this.m_hostIdColumn.GetGuid((IDataReader) this.Reader),
      TargetInstanceName = this.m_targetInstanceNameColumn.GetString((IDataReader) this.Reader, false),
      Priority = this.m_priorityColumn.GetByte((IDataReader) this.Reader),
      Options = (HostMigrationOptions) this.m_optionsColumnColumn.GetByte((IDataReader) this.Reader),
      MigrationId = this.m_migrationIdColumn.GetGuid((IDataReader) this.Reader, true),
      DriverJobId = this.m_driverJobIdColumn.GetGuid((IDataReader) this.Reader, true),
      TargetDatabaseId = this.m_targetDatabaseIdColumn.GetInt32((IDataReader) this.Reader, 0),
      HostsAffectedByTheMove = this.m_hostsAffectedByTheMoveColumn.GetInt32((IDataReader) this.Reader, 0, 0)
    };
  }
}
