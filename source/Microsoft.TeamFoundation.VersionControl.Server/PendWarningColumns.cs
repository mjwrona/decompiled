// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.PendWarningColumns
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  internal class PendWarningColumns : VersionControlObjectBinder<Warning>
  {
    protected SqlColumnBinder YourPendingCommand = new SqlColumnBinder(nameof (YourPendingCommand));
    protected SqlColumnBinder YourServerItem = new SqlColumnBinder(nameof (YourServerItem));
    protected SqlColumnBinder YourItemId = new SqlColumnBinder(nameof (YourItemId));
    protected SqlColumnBinder WarningType = new SqlColumnBinder(nameof (WarningType));
    protected SqlColumnBinder TheirPendingCommand = new SqlColumnBinder(nameof (TheirPendingCommand));
    protected SqlColumnBinder TheirServerItem = new SqlColumnBinder(nameof (TheirServerItem));
    protected SqlColumnBinder WorkspaceName = new SqlColumnBinder(nameof (WorkspaceName));
    protected SqlColumnBinder OwnerId = new SqlColumnBinder(nameof (OwnerId));

    public PendWarningColumns()
    {
    }

    public PendWarningColumns(VersionControlSqlResourceComponent component)
      : base(component)
    {
    }

    protected override Warning Bind() => new Warning()
    {
      yourServerItem = this.YourServerItem.GetServerItem(this.Reader, true),
      yourItemId = this.YourItemId.GetInt32((IDataReader) this.Reader, 0),
      yourChangeType = VersionControlSqlResourceComponent.GetChangeType(this.YourPendingCommand.GetInt32((IDataReader) this.Reader)),
      WarningType = (Microsoft.TeamFoundation.VersionControl.Server.WarningType) this.WarningType.GetInt32((IDataReader) this.Reader),
      ChangeType = VersionControlSqlResourceComponent.GetChangeType(this.TheirPendingCommand.GetInt32((IDataReader) this.Reader, 0)),
      ParentOrChildPath = this.TheirServerItem.GetServerItem(this.Reader, true),
      Workspace = this.WorkspaceName.GetString((IDataReader) this.Reader, true),
      ownerId = this.OwnerId.GetGuid((IDataReader) this.Reader, true)
    };
  }
}
