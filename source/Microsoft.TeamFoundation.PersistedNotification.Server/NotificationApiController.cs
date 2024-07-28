// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.PersistedNotification.Server.NotificationApiController
// Assembly: Microsoft.TeamFoundation.PersistedNotification.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 93EF6375-7D4B-4818-984E-834B7F34DA0F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.PersistedNotification.Server.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Notification;
using System;
using System.Collections.Generic;
using System.Net;

namespace Microsoft.TeamFoundation.PersistedNotification.Server
{
  public class NotificationApiController : TfsApiController
  {
    private static readonly Dictionary<Type, HttpStatusCode> s_httpExceptions = new Dictionary<Type, HttpStatusCode>()
    {
      {
        typeof (ArgumentNullException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (TeamFoundationServiceException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (ProjectException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (NotSupportedException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (UnauthorizedAccessException),
        HttpStatusCode.Forbidden
      },
      {
        typeof (FeatureDisabledException),
        HttpStatusCode.NotFound
      }
    };

    public override string ActivityLogArea => "Notification";

    public override IDictionary<Type, HttpStatusCode> HttpExceptions => (IDictionary<Type, HttpStatusCode>) NotificationApiController.s_httpExceptions;

    protected IPersistedNotificationService PersistedNotificationService => this.TfsRequestContext.GetService<IPersistedNotificationService>();
  }
}
