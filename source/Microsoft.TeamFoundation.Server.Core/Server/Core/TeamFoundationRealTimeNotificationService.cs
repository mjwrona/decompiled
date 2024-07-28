// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.TeamFoundationRealTimeNotificationService
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.Server.Core
{
  public class TeamFoundationRealTimeNotificationService : IVssFrameworkService
  {
    private IDisposableReadOnlyList<IRealTimeNotificationDispatcher> m_dispatchers;

    public void ServiceStart(IVssRequestContext systemRequestContext) => this.m_dispatchers = systemRequestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection) ? systemRequestContext.GetExtensions<IRealTimeNotificationDispatcher>() : throw new UnexpectedHostTypeException(systemRequestContext.ServiceHost.HostType);

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
      if (this.m_dispatchers == null)
        return;
      this.m_dispatchers.Dispose();
      this.m_dispatchers = (IDisposableReadOnlyList<IRealTimeNotificationDispatcher>) null;
    }

    public Task StartMonitor(
      IVssRequestContext requestContext,
      string collectionId,
      string clientId)
    {
      List<Task> taskList = new List<Task>();
      if (this.m_dispatchers != null)
      {
        foreach (IRealTimeNotificationDispatcher dispatcher in (IEnumerable<IRealTimeNotificationDispatcher>) this.m_dispatchers)
          taskList.Add(dispatcher.StartMonitor(requestContext, collectionId, clientId));
      }
      return Task.WhenAll((IEnumerable<Task>) taskList);
    }

    public void SendMessage(IVssRequestContext requestContext, RealTimeNotificationMessage message)
    {
      if (this.m_dispatchers == null)
        return;
      foreach (IRealTimeNotificationDispatcher dispatcher in (IEnumerable<IRealTimeNotificationDispatcher>) this.m_dispatchers)
        dispatcher.SendMessage(requestContext, message);
    }
  }
}
