// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.PendChangeExceptionBinder15
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Framework.Server;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  internal class PendChangeExceptionBinder15 : PendChangeExceptionBinder
  {
    public PendChangeExceptionBinder15(
      IVssRequestContext requestContext,
      VersionControlSqlResourceComponent component)
      : base(requestContext, component)
    {
    }

    protected override string BindOffendingServerItem() => this.BestEffortGetServerItemProjectNamePath(base.BindOffendingServerItem());

    protected override string BindTargetServerItem() => this.BestEffortGetServerItemProjectNamePath(base.BindTargetServerItem());
  }
}
