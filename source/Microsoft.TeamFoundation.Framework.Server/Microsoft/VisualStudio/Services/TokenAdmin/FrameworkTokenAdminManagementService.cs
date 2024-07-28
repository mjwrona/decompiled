// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.TokenAdmin.FrameworkTokenAdminManagementService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.DelegatedAuthorization;
using Microsoft.VisualStudio.Services.Graph;
using Microsoft.VisualStudio.Services.TokenAdmin.Client;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.TokenAdmin
{
  internal class FrameworkTokenAdminManagementService : 
    ITokenAdminManagementService,
    IVssFrameworkService
  {
    private const string c_area = "TokenAdmin";
    private const string c_layer = "FrameworkTokenAdminManagementService";

    public void ServiceStart(IVssRequestContext systemRequestContext) => this.ValidateRequestContext(systemRequestContext);

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    private void ValidateRequestContext(IVssRequestContext context) => context.CheckServiceHostType(TeamFoundationHostType.ProjectCollection);

    public TokenAdminPagedSessionTokens ListPersonalAccessTokensByUser(
      IVssRequestContext requestContext,
      SubjectDescriptor userSubjectDescriptor,
      int? pageSize,
      Guid? continuationToken,
      bool isPublic = false)
    {
      this.ValidateRequestContext(requestContext);
      GraphValidation.CheckDescriptor(userSubjectDescriptor, nameof (userSubjectDescriptor));
      return requestContext.GetClient<TokenAdminHttpClient>().ListPersonalAccessTokensAsync(userSubjectDescriptor, pageSize, continuationToken?.ToString(), new bool?(isPublic)).SyncResult<TokenAdminPagedSessionTokens>();
    }

    public void RevokeAuthorizations(
      IVssRequestContext requestContext,
      IEnumerable<Guid> authorizationIds,
      bool isPublic = false)
    {
      this.ValidateRequestContext(requestContext);
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) authorizationIds, nameof (authorizationIds));
      List<TokenAdminRevocation> revocations = new List<TokenAdminRevocation>();
      foreach (Guid authorizationId in authorizationIds)
        revocations.Add(new TokenAdminRevocation()
        {
          AuthorizationId = authorizationId
        });
      requestContext.GetClient<TokenAdminHttpClient>().RevokeAuthorizationsAsync((IEnumerable<TokenAdminRevocation>) revocations, new bool?(isPublic)).SyncResult();
    }

    public SessionTokenResult GetPersonalAccessToken(
      IVssRequestContext requestContext,
      string accessTokenKey,
      bool isPublic)
    {
      requestContext.CheckDeploymentRequestContext();
      ArgumentUtility.CheckStringForNullOrEmpty(accessTokenKey, nameof (accessTokenKey));
      return requestContext.GetClient<TokenAdminHttpClient>().GetPersonalAccessTokenAsync(accessTokenKey, isPublic).SyncResult<SessionTokenResult>();
    }

    public void RevokePersonalAccessToken(
      IVssRequestContext requestContext,
      string accessTokenKey,
      bool isPublic = false)
    {
      requestContext.CheckDeploymentRequestContext();
      requestContext.GetClient<TokenAdminHttpClient>().RevokePersonalAccessTokenAsync(accessTokenKey, isPublic).SyncResult();
    }
  }
}
