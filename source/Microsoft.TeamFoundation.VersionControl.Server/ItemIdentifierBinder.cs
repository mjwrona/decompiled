// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.ItemIdentifierBinder
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  internal class ItemIdentifierBinder : VersionControlObjectBinder<ItemIdentifier>
  {
    protected SqlColumnBinder fullPath = new SqlColumnBinder("FullPath");
    protected SqlColumnBinder versionFrom = new SqlColumnBinder("VersionFrom");
    protected SqlColumnBinder deletionId = new SqlColumnBinder("DeletionId");
    protected SqlColumnBinder command = new SqlColumnBinder("Command");

    public ItemIdentifierBinder()
    {
    }

    public ItemIdentifierBinder(VersionControlSqlResourceComponent component)
      : base(component)
    {
    }

    protected override ItemIdentifier Bind() => new ItemIdentifier()
    {
      ItemPathPair = this.GetPreDataspaceItemPathPair(this.fullPath.GetServerItem(this.Reader, false)),
      DeletionId = this.deletionId.GetInt32((IDataReader) this.Reader),
      Version = (VersionSpec) new ChangesetVersionSpec(this.versionFrom.GetInt32((IDataReader) this.Reader)),
      ChangeType = VersionControlSqlResourceComponent.GetChangeType((int) this.command.GetInt16((IDataReader) this.Reader))
    };
  }
}
