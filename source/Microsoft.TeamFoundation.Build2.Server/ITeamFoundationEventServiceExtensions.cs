// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.ITeamFoundationEventServiceExtensions
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Diagnostics;

namespace Microsoft.TeamFoundation.Build2.Server
{
  internal static class ITeamFoundationEventServiceExtensions
  {
    public static void PublishBuildCompletedEvent(
      this ITeamFoundationEventService eventService,
      IVssRequestContext requestContext,
      IReadOnlyBuildData build)
    {
      if (build == null)
        return;
      if (build.Definition == null)
        requestContext.TraceError(12030186, nameof (ITeamFoundationEventServiceExtensions), "Publishing a BuildCompletedEvent with a null definition. Project = {0}, buildId = {1}. Stack: {2}", (object) build.ProjectId, (object) build.Id, (object) new StackTrace().ToString());
      eventService.PublishNotification(requestContext, (object) new BuildCompletedEvent(build));
    }
  }
}
