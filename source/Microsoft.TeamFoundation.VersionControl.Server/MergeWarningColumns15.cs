// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.MergeWarningColumns15
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using System.Data;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  internal class MergeWarningColumns15 : MergeWarningColumns
  {
    public MergeWarningColumns15(VersionControlSqlResourceComponent component)
      : base(component)
    {
    }

    protected override Failure Bind()
    {
      Failure failure = new Failure();
      failure.Severity = SeverityType.Warning;
      failure.ServerItem = this.BestEffortGetServerItemProjectNamePath(this.targetServerItem.GetServerItem(this.Reader, true));
      string itemProjectNamePath = this.BestEffortGetServerItemProjectNamePath(this.offendingServerItem.GetServerItem(this.Reader, false));
      if (this.errorCode.GetInt32((IDataReader) this.Reader) != 500138)
        return (Failure) null;
      failure.Message = Resources.Format("MergeEditDeleteException", (object) failure.ServerItem, (object) itemProjectNamePath);
      failure.Code = "MergeEditDeleteException";
      return failure;
    }
  }
}
