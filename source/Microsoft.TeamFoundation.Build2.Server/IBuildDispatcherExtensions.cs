// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.IBuildDispatcherExtensions
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.Build2.Server
{
  internal static class IBuildDispatcherExtensions
  {
    public static void SendArtifactAdded(
      this IBuildDispatcher dispatcher,
      IVssRequestContext requestContext,
      int buildId,
      string artifactName)
    {
      dispatcher.SendArtifactAdded(requestContext, (string) null, buildId, artifactName);
    }

    public static void SendChangesCalculated(
      this IBuildDispatcher dispatcher,
      IVssRequestContext requestContext,
      int buildId)
    {
      dispatcher.SendChangesCalculated(requestContext, (string) null, buildId);
    }

    public static void SendBuildUpdated(
      this IBuildDispatcher dispatcher,
      IVssRequestContext requestContext,
      Guid projectId,
      int definitionId,
      int buildId,
      string definitionScope)
    {
      dispatcher.SendBuildUpdated(requestContext, (string) null, projectId, definitionId, buildId, definitionScope);
    }

    public static Task SendBuildUpdatedAsync(
      this IBuildDispatcher dispatcher,
      IVssRequestContext requestContext,
      Guid projectId,
      int buildId,
      int definitionId,
      string definitionScope)
    {
      return dispatcher.SendBuildUpdatedAsync(requestContext, (string) null, projectId, definitionId, buildId, definitionScope);
    }

    public static void SendTagsAdded(
      this IBuildDispatcher dispatcher,
      IVssRequestContext requestContext,
      int buildId)
    {
      dispatcher.SendTagsAdded(requestContext, (string) null, buildId);
    }
  }
}
