// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notification.IPersistedNotificationService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Notification
{
  [DefaultServiceImplementation(typeof (FrameworkPersistedNotificationService))]
  public interface IPersistedNotificationService : IVssFrameworkService
  {
    void SaveNotifications(
      IVssRequestContext requestContext,
      IEnumerable<Microsoft.VisualStudio.Services.Notification.Notification> notifications);

    IList<Microsoft.VisualStudio.Services.Notification.Notification> GetRecipientNotifications(
      IVssRequestContext requestContext);

    RecipientMetadata GetRecipientMetadata(IVssRequestContext requestContext);

    RecipientMetadata UpdateRecipientMetadata(
      IVssRequestContext requestContext,
      RecipientMetadata metadata);
  }
}
