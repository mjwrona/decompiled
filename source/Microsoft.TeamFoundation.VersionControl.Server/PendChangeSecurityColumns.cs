// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.PendChangeSecurityColumns
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  internal class PendChangeSecurityColumns : VersionControlObjectBinder<PendChangeSecurity>
  {
    protected SqlColumnBinder targetVTServerItem = new SqlColumnBinder("TargetVTServerItem");
    protected SqlColumnBinder targetVFServerItem = new SqlColumnBinder("TargetVFServerItem");
    protected SqlColumnBinder sourceVTServerItem = new SqlColumnBinder("SourceVTServerItem");
    protected SqlColumnBinder sourceItemId = new SqlColumnBinder("SourceItemId");
    protected SqlColumnBinder targetItemId = new SqlColumnBinder("TargetItemId");
    protected SqlColumnBinder inputIndex = new SqlColumnBinder("InputIndex");
    protected SqlColumnBinder isBranchObject = new SqlColumnBinder("IsBranchObject");
    protected SqlColumnBinder changeType = new SqlColumnBinder("ChangeType");

    public PendChangeSecurityColumns()
    {
    }

    public PendChangeSecurityColumns(VersionControlSqlResourceComponent component)
      : base(component)
    {
    }

    protected override PendChangeSecurity Bind() => new PendChangeSecurity()
    {
      TargetItemPathPair = this.GetPreDataspaceItemPathPair(this.targetVTServerItem.GetServerItem(this.Reader, false)),
      TargetSourceItemPathPair = this.GetPreDataspaceItemPathPair(this.targetVFServerItem.GetServerItem(this.Reader, true)),
      SourceItemToPathPair = this.GetPreDataspaceItemPathPair(this.sourceVTServerItem.GetServerItem(this.Reader, true)),
      SourceItemId = this.sourceItemId.GetInt32((IDataReader) this.Reader, 0),
      TargetItemId = this.targetItemId.GetInt32((IDataReader) this.Reader, 0),
      InputIndex = this.inputIndex.GetInt32((IDataReader) this.Reader),
      IsBranchObject = this.isBranchObject.GetBoolean((IDataReader) this.Reader),
      ChangeType = VersionControlSqlResourceComponent.GetChangeType(this.changeType.GetInt32((IDataReader) this.Reader, 0))
    };
  }
}
