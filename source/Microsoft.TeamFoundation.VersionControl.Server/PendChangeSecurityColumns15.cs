// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.PendChangeSecurityColumns15
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  internal class PendChangeSecurityColumns15 : PendChangeSecurityColumns
  {
    private SqlColumnBinder sourceItemDataspaceId = new SqlColumnBinder("SourceItemDataspaceId");
    private SqlColumnBinder targetItemDataspaceId = new SqlColumnBinder("TargetItemDataspaceId");

    public PendChangeSecurityColumns15(VersionControlSqlResourceComponent component)
      : base(component)
    {
    }

    protected override PendChangeSecurity Bind() => new PendChangeSecurity()
    {
      TargetItemPathPair = this.GetItemPathPair(this.targetVTServerItem.GetServerItem(this.Reader, false)),
      TargetSourceItemPathPair = this.GetItemPathPair(this.targetVFServerItem.GetServerItem(this.Reader, true)),
      SourceItemToPathPair = this.GetItemPathPair(this.sourceVTServerItem.GetServerItem(this.Reader, true)),
      SourceItemId = this.sourceItemId.GetInt32((IDataReader) this.Reader, 0),
      TargetItemId = this.targetItemId.GetInt32((IDataReader) this.Reader, 0),
      InputIndex = this.inputIndex.GetInt32((IDataReader) this.Reader),
      IsBranchObject = this.isBranchObject.GetBoolean((IDataReader) this.Reader),
      ChangeType = VersionControlSqlResourceComponent.GetChangeType(this.changeType.GetInt32((IDataReader) this.Reader, 0)),
      SourceItemDataspaceId = this.sourceItemDataspaceId.GetInt32((IDataReader) this.Reader, 0),
      TargetItemDataspaceId = this.targetItemDataspaceId.GetInt32((IDataReader) this.Reader, 0)
    };
  }
}
