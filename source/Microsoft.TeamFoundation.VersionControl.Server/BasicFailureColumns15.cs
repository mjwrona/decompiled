// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.BasicFailureColumns15
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  internal class BasicFailureColumns15 : BasicFailureColumns
  {
    public BasicFailureColumns15(string sqlProcName, VersionControlSqlResourceComponent component)
      : base(sqlProcName, component)
    {
    }

    protected override string BindServerItem() => this.BestEffortGetServerItemProjectNamePath(this.serverItem.GetServerItem(this.Reader, false));
  }
}
