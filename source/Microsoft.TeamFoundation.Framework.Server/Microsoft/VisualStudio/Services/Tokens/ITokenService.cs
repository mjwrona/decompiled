// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Tokens.ITokenService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.DelegatedAuthorization;
using Microsoft.VisualStudio.Services.WebApi.Jwt;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Tokens
{
  [DefaultServiceImplementation(typeof (FrameworkTokenService))]
  public interface ITokenService : IDelegatedAuthorizationService, IVssFrameworkService
  {
    AccessToken GetAccessToken(
      IVssRequestContext requestContext,
      string accessTokenKey,
      bool isPublic = false);

    AppSessionTokenResult IssueAppSessionToken(
      IVssRequestContext requestContext,
      SubjectDescriptor subjectDescriptor,
      Guid clientId,
      Guid? authorizationId = null);

    HostAuthorizationDecision AuthorizeHost(
      IVssRequestContext requestContext,
      Guid clientId,
      Guid hostId,
      Guid? newId = null);

    SessionTokenResult IssueSessionToken(
      IVssRequestContext requestContext,
      Guid hostId,
      Guid orgHostId,
      Guid deploymentHostId,
      Guid? clientId = null,
      Guid? userId = null,
      string name = null,
      DateTime? validTo = null,
      string scope = null,
      IList<Guid> targetAccounts = null,
      SessionTokenType tokenType = SessionTokenType.SelfDescribing,
      bool isPublic = false,
      string publicData = null,
      string source = null,
      bool isRequestedByTfsPatWebUI = false,
      Guid? authorizationId = null,
      Guid? accessId = null,
      bool isImpersonating = false,
      string token = null,
      IDictionary<string, string> customClaims = null,
      Guid? requestedById = null);

    AccessTokenResult Exchange(
      IVssRequestContext requestContext,
      GrantType grantType,
      JsonWebToken grant,
      JsonWebToken clientSecret,
      Guid hostId,
      Guid orgHostId,
      Uri audience = null,
      Uri redirectUri = null,
      Guid? accessId = null);

    AccessTokenResult Exchange(
      IVssRequestContext requestContext,
      Guid registrationId,
      string clientSecret,
      Guid hostId,
      Guid tenantId,
      string requestedScopes = null);
  }
}
