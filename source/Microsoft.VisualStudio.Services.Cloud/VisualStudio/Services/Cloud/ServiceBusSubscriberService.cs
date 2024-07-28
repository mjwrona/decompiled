// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.ServiceBusSubscriberService
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;

namespace Microsoft.VisualStudio.Services.Cloud
{
  internal sealed class ServiceBusSubscriberService : 
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
      requestContext.GetService<ServiceBusManagementService>().Subscribe(requestContext, subscription, action, acceptedHostTypes, exceptionNotification, invokeActionWithNoMessage);
    }

    public void Unsubscribe(
      IVssRequestContext requestContext,
      MessageBusSubscriptionInfo subscription)
    {
      requestContext.GetService<ServiceBusManagementService>().Unsubscribe(requestContext, subscription);
    }
  }
}
