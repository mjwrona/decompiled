// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.SignalR.IGroupManagerExtensions
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
  public static class IGroupManagerExtensions
  {
    public static async Task AddTrackedConnection(
      this IGroupManager groups,
      IVssRequestContext requestContext,
      string hubName,
      string groupName,
      string connectionId)
    {
      try
      {
        requestContext.GetService<IVssSignalRHubGroupService>().AddConnectionToGroup(requestContext, hubName, groupName, connectionId);
      }
      finally
      {
        await groups.Add(connectionId, groupName).ConfigureAwait(false);
      }
    }

    public static async Task RemoveTrackedConnection(
      this IGroupManager groups,
      IVssRequestContext requestContext,
      string hubName,
      string groupName,
      string connectionId)
    {
      try
      {
        requestContext.GetService<IVssSignalRHubGroupService>().RemoveConnectionFromGroup(requestContext, hubName, groupName, connectionId);
      }
      finally
      {
        await groups.Remove(connectionId, groupName).ConfigureAwait(false);
      }
    }
  }
}
