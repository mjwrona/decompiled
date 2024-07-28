// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceHooks.Server.WebApi.ServiceHooksSvcControllerBase
// Assembly: Microsoft.VisualStudio.Services.ServiceHooks.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 25B6D63E-3809-4A04-9AE1-1F77D8FFEE67
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ServiceHooks.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ServiceHooks.WebApi;
using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Authentication;

namespace Microsoft.VisualStudio.Services.ServiceHooks.Server.WebApi
{
  public abstract class ServiceHooksSvcControllerBase : TfsApiController
  {
    private static readonly IDictionary<Type, HttpStatusCode> baseHttpExceptions;
    private static readonly string s_layer = typeof (ServiceHooksSvcControllerBase).Name;
    private static readonly string s_area = typeof (ServiceHooksSvcControllerBase).Namespace;

    static ServiceHooksSvcControllerBase() => ServiceHooksSvcControllerBase.baseHttpExceptions = (IDictionary<Type, HttpStatusCode>) new Dictionary<Type, HttpStatusCode>()
    {
      {
        typeof (ArgumentException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (ArgumentNullException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (ArgumentOutOfRangeException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (SubscriptionInputException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (AuthenticationException),
        HttpStatusCode.Unauthorized
      },
      {
        typeof (ConsumerNotFoundException),
        HttpStatusCode.NotFound
      },
      {
        typeof (ConsumerActionNotFoundException),
        HttpStatusCode.NotFound
      },
      {
        typeof (SubscriptionNotFoundException),
        HttpStatusCode.NotFound
      },
      {
        typeof (NotificationNotFoundException),
        HttpStatusCode.NotFound
      }
    };

    public override IDictionary<Type, HttpStatusCode> HttpExceptions => ServiceHooksSvcControllerBase.baseHttpExceptions;

    protected bool CheckPermission(
      IVssRequestContext requestContext,
      int permission,
      bool throwIfNoPermission = true)
    {
      IVssSecurityNamespace securityNamespace = requestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(requestContext, ServiceHooksSecurityConstants.NamespaceId);
      string token = "ManagementSecurity";
      if (!throwIfNoPermission)
        return securityNamespace.HasPermission(requestContext, token, permission, false);
      securityNamespace.CheckPermission(requestContext, token, permission, false);
      return true;
    }

    public override string ActivityLogArea => "Service Hooks";
  }
}
