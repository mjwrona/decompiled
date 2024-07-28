// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Framework.SqlMessageBusSubscriberService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;

namespace Microsoft.VisualStudio.Services.Framework
{
  internal sealed class SqlMessageBusSubscriberService : 
    IMessageBusSubscriberService,
    IVssFrameworkService
  {
    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
      if (!systemRequestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
        throw new UnexpectedHostTypeException(systemRequestContext.ServiceHost.HostType);
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public void Subscribe(
      IVssRequestContext requestContext,
      MessageBusSubscriptionInfo subscription,
      Action<IVssRequestContext, IMessage> action,
      TeamFoundationHostType acceptedHostTypes,
      Action<Exception, string, IMessage> exceptionNotification = null,
      bool invokeActionWithNoMessage = false)
    {
      requestContext.GetService<SqlMessageBusManagementService>().Subscribe(requestContext, subscription, action, exceptionNotification, invokeActionWithNoMessage);
    }

    public void Unsubscribe(
      IVssRequestContext requestContext,
      MessageBusSubscriptionInfo subscription)
    {
      requestContext.GetService<SqlMessageBusManagementService>().Unsubscribe(requestContext, subscription);
    }
  }
}
