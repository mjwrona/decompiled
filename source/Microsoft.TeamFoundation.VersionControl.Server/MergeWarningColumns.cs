// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.MergeWarningColumns
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  internal class MergeWarningColumns : VersionControlObjectBinder<Failure>
  {
    protected SqlColumnBinder targetServerItem = new SqlColumnBinder("TargetServerItem");
    protected SqlColumnBinder offendingServerItem = new SqlColumnBinder("OffendingServerItem");
    protected SqlColumnBinder offendingLocalItem = new SqlColumnBinder("OffendingLocalItem");
    protected SqlColumnBinder errorCode = new SqlColumnBinder("ErrorCode");
    protected SqlColumnBinder lockOwnerId = new SqlColumnBinder("LockOwnerId");
    protected SqlColumnBinder lockWorkspaceName = new SqlColumnBinder("LockWorkspaceName");

    public MergeWarningColumns()
    {
    }

    public MergeWarningColumns(VersionControlSqlResourceComponent component)
      : base(component)
    {
    }

    protected override Failure Bind()
    {
      Failure failure = new Failure();
      failure.Severity = SeverityType.Warning;
      failure.ServerItem = this.targetServerItem.GetServerItem(this.Reader, true);
      string serverItem = this.offendingServerItem.GetServerItem(this.Reader, false);
      if (this.errorCode.GetInt32((IDataReader) this.Reader) != 500138)
        return (Failure) null;
      failure.Message = Resources.Format("MergeEditDeleteException", (object) failure.ServerItem, (object) serverItem);
      failure.Code = "MergeEditDeleteException";
      return failure;
    }
  }
}
