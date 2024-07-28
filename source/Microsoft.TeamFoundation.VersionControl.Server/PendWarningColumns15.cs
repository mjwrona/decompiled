// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.PendWarningColumns15
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using System.Data;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  internal class PendWarningColumns15 : PendWarningColumns
  {
    public PendWarningColumns15(VersionControlSqlResourceComponent component)
      : base(component)
    {
    }

    protected override Warning Bind() => new Warning()
    {
      yourServerItem = this.BestEffortGetServerItemProjectNamePath(this.YourServerItem.GetServerItem(this.Reader, true)),
      yourItemId = this.YourItemId.GetInt32((IDataReader) this.Reader, 0),
      yourChangeType = VersionControlSqlResourceComponent.GetChangeType(this.YourPendingCommand.GetInt32((IDataReader) this.Reader)),
      WarningType = (Microsoft.TeamFoundation.VersionControl.Server.WarningType) this.WarningType.GetInt32((IDataReader) this.Reader),
      ChangeType = VersionControlSqlResourceComponent.GetChangeType(this.TheirPendingCommand.GetInt32((IDataReader) this.Reader, 0)),
      ParentOrChildPath = this.BestEffortGetServerItemProjectNamePath(this.TheirServerItem.GetServerItem(this.Reader, true)),
      Workspace = this.WorkspaceName.GetString((IDataReader) this.Reader, true),
      ownerId = this.OwnerId.GetGuid((IDataReader) this.Reader, true)
    };
  }
}
