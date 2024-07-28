// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.SignalR.IHubContextExtensions
// Assembly: Microsoft.VisualStudio.Services.SignalR, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: BD148864-3B8A-4D7D-BD16-EF04E9549DC9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.SignalR.dll

using Microsoft.AspNet.SignalR;
using Microsoft.TeamFoundation.Framework.Server;
using System.ComponentModel;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.SignalR
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public static class IHubContextExtensions
  {
    public static Task RemoveTrackedConnection(
      this IHubContext hubContext,
      IVssRequestContext requestContext,
      string hubName,
      string connectionId,
      bool isClientTimeout = false)
    {
      return IHubContextExtensions.RemoveTrackedConnection(requestContext, hubName, connectionId, isClientTimeout);
    }

    public static Task RemoveTrackedConnection<T>(
      this IHubContext<T> hubContext,
      IVssRequestContext requestContext,
      string hubName,
      string connectionId,
      bool isClientTimeout = false)
    {
      return IHubContextExtensions.RemoveTrackedConnection(requestContext, hubName, connectionId, isClientTimeout);
    }

    private static Task RemoveTrackedConnection(
      IVssRequestContext requestContext,
      string hubName,
      string connectionId,
      bool isClientTimeout = false)
    {
      IVssSignalRHubGroupService service = requestContext.GetService<IVssSignalRHubGroupService>();
      if (isClientTimeout && requestContext.IsFeatureEnabled("VisualStudio.Services.SignalR.DisconnectOnlyIfKeepAliveIsLost"))
      {
        CustomerIntelligenceData properties = new CustomerIntelligenceData();
        properties.Add("ConnectionId", connectionId);
        requestContext.GetService<CustomerIntelligenceService>().Publish(requestContext, "SignalR", "ConnectionCleanup", properties);
        service.QueueConnectionCleanUpJob(requestContext);
        return Task.CompletedTask;
      }
      service.RemoveConnectionFromAllGroups(requestContext, hubName, connectionId);
      return Task.CompletedTask;
    }
  }
}
