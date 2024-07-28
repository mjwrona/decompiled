// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.ItemIdentifierBinder15
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using System.Data;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  internal class ItemIdentifierBinder15 : ItemIdentifierBinder
  {
    public ItemIdentifierBinder15(VersionControlSqlResourceComponent component)
      : base(component)
    {
    }

    protected override ItemIdentifier Bind() => new ItemIdentifier()
    {
      ItemPathPair = this.GetItemPathPair(this.fullPath.GetServerItem(this.Reader, false)),
      DeletionId = this.deletionId.GetInt32((IDataReader) this.Reader),
      Version = (VersionSpec) new ChangesetVersionSpec(this.versionFrom.GetInt32((IDataReader) this.Reader)),
      ChangeType = VersionControlSqlResourceComponent.GetChangeType((int) this.command.GetInt16((IDataReader) this.Reader))
    };
  }
}
