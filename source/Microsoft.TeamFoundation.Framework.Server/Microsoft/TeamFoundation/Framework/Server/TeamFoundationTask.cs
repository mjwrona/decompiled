// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.TeamFoundationTask
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class TeamFoundationTask : TeamFoundationTask<Guid>
  {
    public TeamFoundationTask(TeamFoundationTaskCallback callback)
      : this(callback, (object) null, 0)
    {
    }

    public TeamFoundationTask(TeamFoundationTaskCallback callback, object taskArgs, int interval)
      : this(callback, taskArgs, DateTime.UtcNow.AddMilliseconds((double) interval), interval)
    {
    }

    public TeamFoundationTask(
      TeamFoundationTaskCallback callback,
      object taskArgs,
      DateTime startTime,
      int interval)
      : base(callback, taskArgs, startTime, interval)
    {
    }

    public override bool NeedsTargetRequestContext => true;

    public override IVssRequestContext GetRequestContext(
      IVssRequestContext deploymentContext,
      Guid identifier)
    {
      return deploymentContext.GetService<ITeamFoundationHostManagementService>().BeginRequest(deploymentContext, identifier, this.ServicingContext ? RequestContextType.ServicingContext : RequestContextType.SystemContext, false, false);
    }
  }
}
