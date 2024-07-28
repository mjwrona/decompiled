// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.Server.INotificationSubscriberService
// Assembly: Microsoft.VisualStudio.Services.Notifications.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8ED1F52D-E567-4EFA-AAED-F2D9262BE9C2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Notifications.WebApi;
using System;

namespace Microsoft.VisualStudio.Services.Notifications.Server
{
  [DefaultServiceImplementation(typeof (NotificationSubscriberService))]
  public interface INotificationSubscriberService : IVssFrameworkService
  {
    NotificationSubscriber GetSubscriber(IVssRequestContext requestContext, Guid subscriberId);

    NotificationSubscriber UpdateSubscriber(
      IVssRequestContext requestContext,
      Guid subscriberId,
      NotificationSubscriberUpdateParameters updateParameters);

    void InvalidateSubscriber(IVssRequestContext requestContext, Guid subscriberId);
  }
}
