// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.Server.IFeedMessageBus
// Assembly: Microsoft.VisualStudio.Services.Feed.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 55555083-3B79-4F4F-AA85-92D66019974E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Feed.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Feed.Common.Notification;
using System;

namespace Microsoft.VisualStudio.Services.Feed.Server
{
  public interface IFeedMessageBus
  {
    void SendCachedFeedNoLongerValidNotification(IVssRequestContext requestContext, Guid feedId);

    void SendPackagePublishNotification(IVssRequestContext requestContext, PackageChangedEvent data);

    void PublishFeedChanged(IVssRequestContext requestContext, FeedChangedEvent data);
  }
}
