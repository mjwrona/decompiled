// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.DelegatedAuthorization.PersonalAccessTokensController
// Assembly: Microsoft.VisualStudio.Services.DelegatedAuthorization, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 76926D67-5A10-414E-AFAB-34A210884CEB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.DelegatedAuthorization.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.Authentication;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Security;
using Microsoft.VisualStudio.Services.WebApi.Contracts.DelegatedAuthorization;
using System;
using System.Collections.Generic;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.DelegatedAuthorization
{
  [ControllerApiVersion(6.1)]
  [VersionedApiControllerCustomName(Area = "Tokens", ResourceName = "Pats")]
  [RestrictAadServicePrincipals]
  public class PersonalAccessTokensController : TfsApiController
  {
    private Lazy<IDelegatedAuthorizationService> service;
    private Lazy<IDelegatedAuthorizationService> deploymentService;

    public PersonalAccessTokensController()
    {
      this.service = new Lazy<IDelegatedAuthorizationService>((Func<IDelegatedAuthorizationService>) (() => this.TfsRequestContext.GetService<IDelegatedAuthorizationService>()));
      this.deploymentService = new Lazy<IDelegatedAuthorizationService>((Func<IDelegatedAuthorizationService>) (() => this.TfsRequestContext.To(TeamFoundationHostType.Deployment).GetService<IDelegatedAuthorizationService>()));
    }

    internal PersonalAccessTokensController(
      IDelegatedAuthorizationService injectedDelegatedAuthService,
      IDelegatedAuthorizationService injectedDeploymentDelegatedAuthService,
      IVssRequestContext injectedRequestContext)
    {
      this.service = new Lazy<IDelegatedAuthorizationService>((Func<IDelegatedAuthorizationService>) (() => injectedDelegatedAuthService));
      this.deploymentService = new Lazy<IDelegatedAuthorizationService>((Func<IDelegatedAuthorizationService>) (() => injectedDeploymentDelegatedAuthService));
      this.TfsRequestContext = injectedRequestContext;
    }

    [HttpGet]
    [ClientExample("ListPats.json", "List personal access tokens", null, null)]
    public PagedPatTokens ListPats(
      DisplayFilterOptions displayFilterOption = DisplayFilterOptions.Active,
      SortByOptions sortByOption = SortByOptions.DisplayName,
      bool isSortAscending = true,
      string continuationToken = null,
      [FromUri(Name = "$top")] int top = 20)
    {
      this.VerifyAllowedAuthenticationMechanism();
      this.TfsRequestContext.CheckProjectCollectionRequestContext();
      int num = 1;
      if (continuationToken != null)
        num = PagedSessionTokens.DecodeContinuationToken(continuationToken);
      return (PagedPatTokens) this.service.Value.ListSessionTokensByPage(this.TfsRequestContext, new TokenPageRequest()
      {
        DisplayFilterOption = displayFilterOption,
        CreatedByOption = CreatedByOptions.All,
        SortByOption = sortByOption,
        IsSortAscending = isSortAscending,
        StartRowNumber = num,
        PageSize = top,
        PageRequestTimeStamp = PersonalAccessTokensController.GetCurrentDatetimeStringNeededForSproc()
      });
    }

    [HttpGet]
    [ClientExample("GetPat.json", "Get a personal access token by authorizationId", null, null)]
    public PatTokenResult GetPat(Guid authorizationId)
    {
      this.VerifyAllowedAuthenticationMechanism();
      this.TfsRequestContext.CheckProjectCollectionRequestContext();
      PatTokenResult pat1 = new PatTokenResult();
      SessionTokenResult pat2 = new SessionTokenResult();
      try
      {
        pat2.SessionToken = this.service.Value.GetSessionToken(this.TfsRequestContext, authorizationId);
        return (PatTokenResult) pat2;
      }
      catch (SessionTokenNotFoundException ex)
      {
        pat1.PatTokenError = SessionTokenError.TokenNotFound;
        return pat1;
      }
      catch (AccessCheckException ex)
      {
        pat1.PatTokenError = SessionTokenError.AccessDenied;
        return pat1;
      }
    }

    [HttpPost]
    [ClientExample("CreatePat.json", "Create a new personal access token", null, null)]
    public PatTokenResult CreatePat([FromBody] PatTokenCreateRequest patTokenCreateRequest)
    {
      this.VerifyAllowedAuthenticationMechanism();
      this.TfsRequestContext.CheckProjectCollectionRequestContext();
      ArgumentUtility.CheckForNull<PatTokenCreateRequest>(patTokenCreateRequest, nameof (patTokenCreateRequest));
      List<Guid> targetAccounts = new List<Guid>();
      if (!patTokenCreateRequest.AllOrgs)
        targetAccounts.Add(this.TfsRequestContext.ServiceHost.InstanceId);
      return (PatTokenResult) (patTokenCreateRequest.AllOrgs ? this.deploymentService : this.service).Value.IssueSessionToken(patTokenCreateRequest.AllOrgs ? this.TfsRequestContext.To(TeamFoundationHostType.Deployment) : this.TfsRequestContext, name: patTokenCreateRequest.DisplayName, validTo: patTokenCreateRequest.ValidTo, scope: patTokenCreateRequest.Scope, targetAccounts: (IList<Guid>) targetAccounts, tokenType: SessionTokenType.Compact, isRequestedByTfsPatWebUI: true);
    }

    [HttpPut]
    [ClientExample("UpdatePat.json", "Update a personal access token by authorizationId", null, null)]
    public PatTokenResult UpdatePat([FromBody] PatTokenUpdateRequest patTokenUpdateRequest)
    {
      this.VerifyAllowedAuthenticationMechanism();
      this.TfsRequestContext.CheckProjectCollectionRequestContext();
      ArgumentUtility.CheckForNull<PatTokenUpdateRequest>(patTokenUpdateRequest, nameof (patTokenUpdateRequest));
      List<Guid> guidList = (List<Guid>) null;
      bool hasValue;
      if (hasValue = patTokenUpdateRequest.AllOrgs.HasValue)
      {
        guidList = new List<Guid>();
        if (!patTokenUpdateRequest.AllOrgs.Value)
          guidList.Add(this.TfsRequestContext.ServiceHost.InstanceId);
        hasValue = patTokenUpdateRequest.AllOrgs.Value;
      }
      Lazy<IDelegatedAuthorizationService> lazy = hasValue ? this.deploymentService : this.service;
      PatTokenResult patTokenResult = new PatTokenResult();
      SessionTokenResult sessionTokenResult = new SessionTokenResult();
      SessionToken sessionToken;
      try
      {
        sessionToken = this.service.Value.GetSessionToken(this.TfsRequestContext, patTokenUpdateRequest.AuthorizationId);
      }
      catch (SessionTokenNotFoundException ex)
      {
        patTokenResult.PatTokenError = SessionTokenError.TokenNotFound;
        return patTokenResult;
      }
      catch (AccessCheckException ex)
      {
        patTokenResult.PatTokenError = SessionTokenError.AccessDenied;
        return patTokenResult;
      }
      catch (VssServiceException ex)
      {
        if (ex.Message.Equals("SessionToken AuthorizationId is not a valid Guid"))
        {
          patTokenResult.PatTokenError = SessionTokenError.InvalidAuthorizationId;
          return patTokenResult;
        }
        patTokenResult.PatTokenError = SessionTokenError.FailedToUpdateAccessToken;
        return patTokenResult;
      }
      if (sessionToken.IsValid)
      {
        string displayName = patTokenUpdateRequest.DisplayName ?? sessionToken.DisplayName;
        string scope = patTokenUpdateRequest.Scope ?? sessionToken.Scope;
        DateTime dateTime = patTokenUpdateRequest.ValidTo != new DateTime() ? patTokenUpdateRequest.ValidTo : sessionToken.ValidTo;
        IList<Guid> targetAccounts = (IList<Guid>) guidList ?? sessionToken.TargetAccounts ?? (IList<Guid>) new List<Guid>();
        try
        {
          sessionTokenResult.SessionToken = lazy.Value.UpdateSessionToken(hasValue ? this.TfsRequestContext.To(TeamFoundationHostType.Deployment) : this.TfsRequestContext, patTokenUpdateRequest.AuthorizationId, displayName, scope, new DateTime?(dateTime), targetAccounts);
          return (PatTokenResult) sessionTokenResult;
        }
        catch (FailedToReadTenantPoliciesException ex)
        {
          patTokenResult.PatTokenError = this.GetUpdateExceptionSessionTokenResult(ex.Message);
          return patTokenResult;
        }
        catch (SessionTokenUpdateException ex)
        {
          patTokenResult.PatTokenError = this.GetUpdateExceptionSessionTokenResult(ex.Message);
          return patTokenResult;
        }
      }
      else
      {
        patTokenResult.PatTokenError = SessionTokenError.InvalidToken;
        return patTokenResult;
      }
    }

    [HttpDelete]
    [ClientExample("RevokePat.json", "Revoke a personal access token by authorizationId", null, null)]
    public void Revoke(Guid authorizationId)
    {
      this.VerifyAllowedAuthenticationMechanism();
      this.TfsRequestContext.CheckProjectCollectionRequestContext();
      this.service.Value.RevokeSessionToken(this.TfsRequestContext, authorizationId);
    }

    private static string GetCurrentDatetimeStringNeededForSproc() => DateTime.UtcNow.ToString("ddd, dd MMM yyyy HH:mm:ss") + " GMT";

    private void VerifyAllowedAuthenticationMechanism()
    {
      if (!AuthenticationHelpers.IsRequestUsingAADAuthentication(this.TfsRequestContext))
        throw new InvalidAccessException(FrameworkResources.InvalidAccessException());
    }

    private SessionTokenError GetUpdateExceptionSessionTokenResult(string message)
    {
      if ("GlobalPatPolicyViolation".Equals(message))
        return SessionTokenError.GlobalPatPolicyViolation;
      if ("FullScopePatPolicyViolation".Equals(message))
        return SessionTokenError.FullScopePatPolicyViolation;
      if ("PatLifespanPolicyViolation".Equals(message))
        return SessionTokenError.PatLifespanPolicyViolation;
      return "FailedToReadTenantPolicy".Equals(message) ? SessionTokenError.FailedToReadTenantPolicy : SessionTokenError.InvalidToken;
    }
  }
}
