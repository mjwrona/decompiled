// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Tokens.TokenAdmin.FrameworkTokenAdminService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.DelegatedAuthorization;
using Microsoft.VisualStudio.Services.Graph;
using Microsoft.VisualStudio.Services.TokenAdmin.Client;
using Microsoft.VisualStudio.Services.Tokens.TokenAdmin.Client;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Tokens.TokenAdmin
{
  internal class FrameworkTokenAdminService : ITokenAdminService, IVssFrameworkService
  {
    private readonly IHostLevelConversionConfiguration hostLevelConversionConfiguration;
    private static Guid s_TokenServiceIntanceType = new Guid("00000052-0000-8888-8000-000000000000");
    private const string Area = "Token";
    private const string Layer = "FrameworkTokenAdminService";

    public FrameworkTokenAdminService()
      : this(HostLevelConversionConfiguration.Instance)
    {
    }

    public FrameworkTokenAdminService(
      IHostLevelConversionConfiguration hostLevelConversionConfiguration)
    {
      this.hostLevelConversionConfiguration = hostLevelConversionConfiguration;
    }

    public void ServiceStart(IVssRequestContext systemRequestContext) => systemRequestContext.CheckHostedDeployment();

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public IList<Guid> ListIdentitiesWithGlobalAccessTokens(
      IVssRequestContext requestContext,
      IEnumerable<Guid> authorizationIds,
      bool isPublic = false)
    {
      requestContext.CheckHostedDeployment();
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) authorizationIds, nameof (authorizationIds));
      List<TokenAdminRevocation> revocations = new List<TokenAdminRevocation>();
      foreach (Guid authorizationId in authorizationIds)
        revocations.Add(new TokenAdminRevocation()
        {
          AuthorizationId = authorizationId
        });
      try
      {
        return (IList<Guid>) this.GetHttpClient<FrameworkTokenAdminHttpClient>(requestContext).ListIdentitiesWithGlobalAccessTokensAsync((IEnumerable<TokenAdminRevocation>) revocations, new bool?(isPublic)).SyncResult<List<Guid>>();
      }
      finally
      {
        this.Cleanup(requestContext);
      }
    }

    public TokenAdminPagedSessionTokens ListPersonalAccessTokensByUser(
      IVssRequestContext requestContext,
      SubjectDescriptor userSubjectDescriptor,
      IEnumerable<string> audience,
      int? pageSize,
      Guid? continuationToken,
      bool isPublic = false)
    {
      requestContext.CheckHostedDeployment();
      GraphValidation.CheckDescriptor(userSubjectDescriptor, nameof (userSubjectDescriptor));
      try
      {
        return this.GetHttpClient<FrameworkTokenAdminHttpClient>(requestContext, this.hostLevelConversionConfiguration.IsEnabledForTokenAdminFlow(requestContext)).ListPersonalAccessTokensAsync(audience, userSubjectDescriptor, pageSize, continuationToken?.ToString(), new bool?(isPublic)).SyncResult<TokenAdminPagedSessionTokens>();
      }
      finally
      {
        this.Cleanup(requestContext);
      }
    }

    public IList<SessionToken> RevokeAuthorizations(
      IVssRequestContext requestContext,
      IEnumerable<Guid> authorizationIds,
      IEnumerable<string> audience,
      Guid hostId,
      bool isPublic = false)
    {
      requestContext.CheckHostedDeployment();
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) authorizationIds, nameof (authorizationIds));
      TokenAdministrationRevocation revocations = new TokenAdministrationRevocation()
      {
        AuthorizationIds = authorizationIds,
        Audience = audience
      };
      try
      {
        return (IList<SessionToken>) this.GetHttpClient<FrameworkTokenAdminHttpClient>(requestContext, this.hostLevelConversionConfiguration.IsEnabledForTokenAdminFlow(requestContext)).RevokeAuthorizationsAsync(revocations, hostId, new bool?(isPublic)).SyncResult<List<SessionToken>>();
      }
      finally
      {
        this.Cleanup(requestContext);
      }
    }

    public SessionTokenResult GetPersonalAccessToken(
      IVssRequestContext requestContext,
      string accessTokenKey,
      bool isPublic)
    {
      if (!this.hostLevelConversionConfiguration.IsEnabledForTokenAdminFlow(requestContext))
        requestContext.CheckDeploymentRequestContext();
      ArgumentUtility.CheckStringForNullOrEmpty(accessTokenKey, nameof (accessTokenKey));
      return TokenServiceBase.ExecuteTokenServiceResultRequest<SessionTokenResult>(requestContext, "Token", nameof (FrameworkTokenAdminService), (Func<IVssRequestContext, bool, SessionTokenResult>) ((context, isImpersonating) =>
      {
        try
        {
          return this.GetHttpClient<FrameworkTokenAdminHttpClient>(context, this.hostLevelConversionConfiguration.IsEnabledForTokenAdminFlow(requestContext)).GetPersonalAccessTokenAsync(accessTokenKey, isPublic).SyncResult<SessionTokenResult>();
        }
        finally
        {
          this.Cleanup(context);
        }
      }), elevateCall: true);
    }

    public void RevokePersonalAccessToken(
      IVssRequestContext requestContext,
      string accessTokenKey,
      bool isPublic = false)
    {
      if (!this.hostLevelConversionConfiguration.IsEnabledForTokenAdminFlow(requestContext))
        requestContext.CheckDeploymentRequestContext();
      ArgumentUtility.CheckStringForNullOrEmpty(accessTokenKey, nameof (accessTokenKey));
      TokenServiceBase.ExecuteTokenServiceVoidRequest(requestContext, "Token", nameof (FrameworkTokenAdminService), (Action<IVssRequestContext>) (context =>
      {
        try
        {
          this.GetHttpClient<FrameworkTokenAdminHttpClient>(context, this.hostLevelConversionConfiguration.IsEnabledForTokenAdminFlow(requestContext)).RevokePersonalAccessTokenAsync(accessTokenKey, isPublic).SyncResult();
        }
        finally
        {
          this.Cleanup(context);
        }
      }));
    }

    internal virtual TClient GetHttpClient<TClient>(
      IVssRequestContext requestContext,
      bool projectCollectionLevelEnabled = false)
      where TClient : class, IVssHttpClient
    {
      int num = requestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection) & projectCollectionLevelEnabled ? 1 : 0;
      IVssRequestContext context = requestContext;
      if (num == 0 && !requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
        context = requestContext.To(TeamFoundationHostType.Deployment);
      context.RootContext.Items[RequestContextItemsKeys.UseDelegatedS2STokens] = (object) true;
      return context.GetClient<TClient>(FrameworkTokenAdminService.s_TokenServiceIntanceType);
    }

    protected void Cleanup(IVssRequestContext requestContext)
    {
      IVssRequestContext vssRequestContext = requestContext;
      if (!requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
        vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      vssRequestContext.RootContext.Items.Remove(RequestContextItemsKeys.UseDelegatedS2STokens);
    }
  }
}
