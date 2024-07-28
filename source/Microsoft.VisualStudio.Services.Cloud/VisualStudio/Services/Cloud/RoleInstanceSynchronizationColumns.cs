// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.RoleInstanceSynchronizationColumns
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.VisualStudio.Services.Cloud
{
  internal class RoleInstanceSynchronizationColumns : ObjectBinder<RoleInstanceSynchronization>
  {
    private SqlColumnBinder roleInstanceColumn = new SqlColumnBinder("RoleInstance");
    private SqlColumnBinder hostIdColumn = new SqlColumnBinder("HostId");
    private SqlColumnBinder currentStatusColumn = new SqlColumnBinder("CurrentStatus");
    private SqlColumnBinder previousStatusColumn = new SqlColumnBinder("PreviousStatus");
    private SqlColumnBinder lastUpdatedColumn = new SqlColumnBinder("LastUpdated");

    protected override RoleInstanceSynchronization Bind() => new RoleInstanceSynchronization()
    {
      RoleInstance = this.roleInstanceColumn.GetString((IDataReader) this.Reader, false),
      HostId = this.hostIdColumn.GetNullableGuid((IDataReader) this.Reader),
      CurrentStatus = this.currentStatusColumn.GetString((IDataReader) this.Reader, false),
      PreviousStatus = this.previousStatusColumn.GetString((IDataReader) this.Reader, true),
      LastUpdated = this.lastUpdatedColumn.GetDateTime((IDataReader) this.Reader)
    };
  }
}
