// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.ReleaseManagement.Server.ReleaseManagementEventListener
// Assembly: Microsoft.TeamFoundation.ReleaseManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4E542756-E4AD-41D1-B4C6-D5898225A0D0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.ReleaseManagement.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;

namespace Microsoft.TeamFoundation.ReleaseManagement.Server
{
  public class ReleaseManagementEventListener : IMessageBusSubscriberJobExtensionReceiver
  {
    public TeamFoundationHostType AcceptedHostTypes => TeamFoundationHostType.ProjectCollection;

    public void Receive(IVssRequestContext requestContext, IMessage message)
    {
      if (message.ContentType == null || !message.ContentType.Equals(typeof (ServiceEvent).ToString(), StringComparison.Ordinal))
        return;
      ServiceEvent body = message.GetBody<ServiceEvent>();
      if (body == null || body.EventType == null || !body.EventType.Equals("Release.DeploymentCompletedEvent", StringComparison.OrdinalIgnoreCase) && !body.EventType.Equals("Release.ReleasesDeletedEvent", StringComparison.OrdinalIgnoreCase))
        return;
      requestContext.GetService<ITeamFoundationEventService>().PublishNotification(requestContext, body.Resource);
    }
  }
}
