// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Platform.WebSessionTokenController
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Platform, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A6A2C403-5081-466C-A570-9B50BFA8E213
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Platform.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.DelegatedAuthorization;
using Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using Microsoft.VisualStudio.Services.WebPlatform;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.TeamFoundation.Server.WebAccess.Platform
{
  [VersionedApiControllerCustomName(Area = "WebPlatformAuth", ResourceName = "SessionToken")]
  public class WebSessionTokenController : TfsApiController
  {
    public override string TraceArea => "WebPlatformAuth";

    public override string ActivityLogArea => "Framework";

    [HttpPost]
    [ClientResponseType(typeof (WebSessionToken), null, null)]
    public HttpResponseMessage CreateSessionToken(WebSessionToken sessionToken)
    {
      ArgumentUtility.CheckForNull<WebSessionToken>(sessionToken, nameof (sessionToken));
      if (!sessionToken.TokenType.HasValue)
        sessionToken.TokenType = new DelegatedAppTokenType?(DelegatedAppTokenType.Session);
      HttpResponseMessage response = this.Request.CreateResponse<WebSessionToken>(sessionToken);
      SessionTokens tokens = SessionTokenCookie.GetTokens((TfsApiController) this);
      Guid userId1 = this.TfsRequestContext.GetUserId();
      WebSessionTokenValue cookieValue;
      if (!string.IsNullOrEmpty(sessionToken.NamedTokenId))
      {
        cookieValue = tokens.GetNamedToken(userId1, sessionToken.NamedTokenId);
        if (sessionToken.Force || cookieValue == null)
          cookieValue = (this.TfsRequestContext.To(TeamFoundationHostType.Deployment).GetService<NamedWebSessionTokenProviderService>().GetTokenProvider(sessionToken.NamedTokenId) ?? throw new ArgumentException(string.Format(PlatformResources.NamedSessionTokenProviderNotFoundFormat, (object) sessionToken.NamedTokenId))).GenerateToken(this.TfsRequestContext);
      }
      else
      {
        if (!sessionToken.AppId.HasValue)
        {
          if (string.IsNullOrEmpty(sessionToken.PublisherName) || string.IsNullOrEmpty(sessionToken.ExtensionName))
            throw new ArgumentException(PlatformResources.SessionTokenArgumentsNotDefined);
          sessionToken.AppId = new Guid?((this.TfsRequestContext.GetService<IInstalledExtensionManager>().GetInstalledExtension(this.TfsRequestContext, sessionToken.PublisherName, sessionToken.ExtensionName) ?? throw new InstalledExtensionNotFoundException(sessionToken.PublisherName, sessionToken.ExtensionName)).RegistrationId);
        }
        Guid instanceId = this.TfsRequestContext.ServiceHost.InstanceId;
        cookieValue = tokens.GetToken(userId1, sessionToken.AppId.Value, instanceId, sessionToken.TokenType.Value);
        if (sessionToken.Force || cookieValue == null)
        {
          IVssRequestContext tfsRequestContext = this.TfsRequestContext;
          if (sessionToken.TokenType.Value == DelegatedAppTokenType.Session)
          {
            IDelegatedAuthorizationService service = tfsRequestContext.GetService<IDelegatedAuthorizationService>();
            Guid? nullable1 = new Guid?(Guid.Empty);
            nullable1 = sessionToken.AppId;
            IVssRequestContext requestContext = tfsRequestContext;
            Guid? clientId = nullable1;
            string str = string.IsNullOrEmpty(sessionToken.Name) ? "sessiontoken-webplatform" : sessionToken.Name;
            DateTime? nullable2 = new DateTime?(DateTime.UtcNow.AddHours(1.0));
            IList<Guid> guidList = (IList<Guid>) new Guid[1]
            {
              instanceId
            };
            Guid? userId2 = new Guid?();
            string name = str;
            DateTime? validTo = nullable2;
            IList<Guid> targetAccounts = guidList;
            Guid? authorizationId = new Guid?();
            Guid? accessId = new Guid?();
            SessionTokenResult sessionTokenResult = service.IssueSessionToken(requestContext, clientId, userId2, name, validTo, targetAccounts: targetAccounts, authorizationId: authorizationId, accessId: accessId);
            if (sessionTokenResult.HasError)
              throw new SessionTokenException(sessionTokenResult.SessionTokenError);
            cookieValue = new WebSessionTokenValue()
            {
              TokenValue = sessionTokenResult.SessionToken.Token,
              ValidTo = sessionTokenResult.SessionToken.ValidTo
            };
          }
          else
          {
            IVssRequestContext vssRequestContext = tfsRequestContext.Elevate();
            AppSessionTokenResult sessionTokenResult = vssRequestContext.GetService<IDelegatedAuthorizationService>().IssueAppSessionToken(vssRequestContext, sessionToken.AppId.Value, new Guid?(this.TfsRequestContext.GetUserId()));
            if (sessionTokenResult.HasError)
              throw new AppSessionTokenException(sessionTokenResult.AppSessionTokenError);
            cookieValue = new WebSessionTokenValue()
            {
              TokenValue = sessionTokenResult.AppSessionToken,
              ValidTo = sessionTokenResult.ExpirationDate
            };
          }
          tokens.SetToken(userId1, sessionToken.AppId.Value, instanceId, sessionToken.TokenType.Value, cookieValue);
          SessionTokenCookie.SetTokens((TfsApiController) this, response, tokens, cookieValue.ValidTo);
        }
      }
      sessionToken.Token = cookieValue.TokenValue;
      sessionToken.ValidTo = new DateTime?(cookieValue.ValidTo);
      return response;
    }
  }
}
