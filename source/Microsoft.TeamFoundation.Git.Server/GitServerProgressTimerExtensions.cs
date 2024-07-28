// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.GitServerProgressTimerExtensions
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core;
using System;
using System.Runtime.CompilerServices;

namespace Microsoft.TeamFoundation.Git.Server
{
  internal static class GitServerProgressTimerExtensions
  {
    public static IDisposable TimeRegion(
      this IVssRequestContext requestContext,
      string layer,
      [CallerMemberName] string regionName = null,
      int tracepoint = 0)
    {
      return requestContext.TimeRegion(GitServerUtils.TraceArea, layer, regionName: regionName, tracepoint: tracepoint);
    }

    public static ITimedOrchestrationRegion TimeOrchestration(
      this IVssRequestContext requestContext,
      string layer,
      Guid orchestrationId,
      long executionTimeThreshold,
      string orchestrationFeature,
      bool orchestrationAlreadyInProgress = false,
      [CallerMemberName] string regionName = null,
      int tracepoint = 0)
    {
      return requestContext.TimeOrchestration(GitServerUtils.TraceArea, layer, orchestrationId, executionTimeThreshold, orchestrationFeature, orchestrationAlreadyInProgress, regionName, tracepoint);
    }
  }
}
