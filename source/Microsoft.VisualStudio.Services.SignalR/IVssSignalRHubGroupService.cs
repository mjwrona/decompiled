// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.SignalR.IVssSignalRHubGroupService
// Assembly: Microsoft.VisualStudio.Services.SignalR, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: BD148864-3B8A-4D7D-BD16-EF04E9549DC9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.SignalR.dll

using Microsoft.TeamFoundation.Framework.Server;

namespace Microsoft.VisualStudio.Services.SignalR
{
  [DefaultServiceImplementation(typeof (VssSignalRHubGroupService))]
  public interface IVssSignalRHubGroupService : IVssFrameworkService
  {
    void AddConnectionToGroup(
      IVssRequestContext requestContext,
      string hubName,
      string groupName,
      string connectionId);

    void CleanupExpiredConnections(IVssRequestContext requestContext);

    VssSignalRHubGroup GetGroup(
      IVssRequestContext requestContext,
      string hubName,
      string groupName);

    void QueueConnectionCleanUpJob(IVssRequestContext requestContext);

    void RemoveConnectionFromAllGroups(
      IVssRequestContext requestContext,
      string hubName,
      string connectionId);

    void RemoveConnectionFromGroup(
      IVssRequestContext requestContext,
      string hubName,
      string groupName,
      string connectionId);
  }
}
