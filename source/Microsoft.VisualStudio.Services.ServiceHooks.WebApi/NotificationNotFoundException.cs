// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceHooks.WebApi.NotificationNotFoundException
// Assembly: Microsoft.VisualStudio.Services.ServiceHooks.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8FEBD486-B6EA-43F6-B878-5BE1581FAD28
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ServiceHooks.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using System;

namespace Microsoft.VisualStudio.Services.ServiceHooks.WebApi
{
  [ExceptionMapping("0.0", "3.0", "NotificationNotFoundException", "Microsoft.VisualStudio.Services.ServiceHooks.WebApi.NotificationNotFoundException, Microsoft.VisualStudio.Services.ServiceHooks.WebApi, Version=14.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
  [Serializable]
  public class NotificationNotFoundException : ServiceHookException
  {
    public NotificationNotFoundException(int notificationId)
      : base(ServiceHooksWebApiResources.Error_NotificationNotFoundByIdFormat((object) notificationId))
    {
    }

    public NotificationNotFoundException(string message, Exception innerException)
      : base(message, innerException)
    {
    }
  }
}
