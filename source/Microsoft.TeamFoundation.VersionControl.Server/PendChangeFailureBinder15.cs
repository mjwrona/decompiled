// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.PendChangeFailureBinder15
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Framework.Server;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  internal class PendChangeFailureBinder15 : PendChangeFailureBinder
  {
    public PendChangeFailureBinder15(
      PendChangeFailureBinder.Caller caller,
      IVssRequestContext requestContext,
      VersionControlSqlResourceComponent component)
      : base(caller, requestContext, component)
    {
    }

    public PendChangeFailureBinder15(
      PendChangeFailureBinder.Caller caller,
      IVssRequestContext requestContext,
      SeverityType serverityType,
      VersionControlSqlResourceComponent component)
      : base(caller, requestContext, serverityType, component)
    {
    }

    protected override string BindTargetServerItem() => this.BestEffortGetServerItemProjectNamePath(this.targetServerItem.GetServerItem(this.Reader, true));

    protected override string BindOffendingServerItem() => this.BestEffortGetServerItemProjectNamePath(this.offendingServerItem.GetServerItem(this.Reader, true));
  }
}
