// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.Authentication.AuthenticationAudit
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server.AuditLog;
using Microsoft.TeamFoundation.Framework.Server.Serialization;
using System;
using System.Collections.Generic;
using System.Net.Http;

namespace Microsoft.TeamFoundation.Framework.Server.Authentication
{
  public class AuthenticationAudit : IAuthenticationAudit
  {
    public const string SignedInFf = "VisualStudio.Services.Identity.SignedInAuditing.Enable";
    public const string PatAccessFf = "VisualStudio.Services.Identity.PatAccessAuditing.Enable";
    private readonly IEventSerializer m_eventSerializer;
    private static readonly ISet<string> m_patAuthenticationMechanisms = (ISet<string>) new HashSet<string>((IEnumerable<string>) new string[4]
    {
      "PAT",
      "PAT_Unscoped",
      "PAT_Global",
      "PAT_UnscopedGlobal"
    });
    private static readonly ISet<string> m_loggedMethods = (ISet<string>) new HashSet<string>((IEnumerable<string>) new string[5]
    {
      HttpMethod.Get.Method,
      HttpMethod.Post.Method,
      HttpMethod.Put.Method,
      HttpMethod.Delete.Method,
      new HttpMethod("PATCH").Method
    });

    public AuthenticationAudit(IEventSerializer eventSerializer) => this.m_eventSerializer = eventSerializer;

    public void AuditAuthorizedRequest(IVssRequestContext requestContext, bool isSignIn)
    {
      if (!requestContext.IsFeatureEnabled("VisualStudio.Services.Identity.SignedInAuditing.Enable") && !requestContext.IsFeatureEnabled("VisualStudio.Services.Identity.PatAccessAuditing.Enable") || !requestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection))
        return;
      if (isSignIn)
      {
        if (!requestContext.IsFeatureEnabled("VisualStudio.Services.Identity.SignedInAuditing.Enable"))
          return;
        this.LogSuccessfulAccess(requestContext, "User.SignInEvent");
      }
      else
      {
        if (!requestContext.IsFeatureEnabled("VisualStudio.Services.Identity.PatAccessAuditing.Enable") || !AuthenticationAudit.IsPatAuthenticatedRequest(requestContext) || !AuthenticationAudit.m_loggedMethods.Contains(requestContext.HttpMethod()))
          return;
        this.LogSuccessfulAccess(requestContext, "User.PatAccessEvent");
      }
    }

    private void LogSuccessfulAccess(IVssRequestContext requestContext, string eventName)
    {
      string action = requestContext.HttpMethod();
      string status = "OK";
      string reason = "";
      this.CreateAndLogEvent(requestContext, eventName, action, status, reason);
    }

    private void CreateAndLogEvent(
      IVssRequestContext requestContext,
      string eventName,
      string action,
      string status,
      string reason)
    {
      try
      {
        AuthenticationAuditEvent e = AuthenticationAuditEvent.Create(requestContext, status, reason, action);
        requestContext.LogAuditEvent(eventName, this.m_eventSerializer.ToObjectDict<AuthenticationAuditEvent>(e));
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1420000, "VssfAuthenticationModule", nameof (AuthenticationAudit), ex);
      }
    }

    private static bool IsPatAuthenticatedRequest(IVssRequestContext requestContext) => AuthenticationAudit.m_patAuthenticationMechanisms.Contains(requestContext.RootContext.GetAuthenticationMechanism());
  }
}
