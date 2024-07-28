// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TeamFoundationRequestContextExtension
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  internal static class TeamFoundationRequestContextExtension
  {
    internal static bool RouteThroughReadReplica(this IVssRequestContext requestContext) => requestContext.IsFeatureEnabled("TestManagement.Server.ReadReplica") && new ReadReplicaSettings(requestContext).EnabledCommands.Contains(requestContext.Method?.Name);
  }
}
