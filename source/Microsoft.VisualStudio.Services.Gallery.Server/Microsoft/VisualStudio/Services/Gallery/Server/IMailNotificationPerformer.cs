// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.IMailNotificationPerformer
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Gallery.Server
{
  public interface IMailNotificationPerformer
  {
    IList<Guid> FetchAndValidateRecipients(
      IVssRequestContext requestContext,
      NotificationsData notificationsData);

    void SendNotifications(
      IVssRequestContext requestContext,
      IList<Guid> recipients,
      IDictionary<string, object> notificationsData);

    void PostNotificationsStep(
      IVssRequestContext requestContext,
      IList<Guid> recipients,
      IDictionary<string, object> notificationsData);
  }
}
