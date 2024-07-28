// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Tokens.TokenServiceBase
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.Authorization;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Tokens
{
  internal abstract class TokenServiceBase
  {
    protected static Guid s_TokenServiceInstanceType = new Guid("00000052-0000-8888-8000-000000000000");
    private static readonly Guid GenevaServiceGuid = new Guid("0000005B-0000-8888-8000-000000000000");

    public void ServiceStart(IVssRequestContext requestContext)
    {
    }

    public void ServiceEnd(IVssRequestContext requestContext)
    {
    }

    internal virtual TClient GetHttpClient<TClient>(
      IVssRequestContext requestContext,
      bool projectCollectionLevelEnabled = false)
      where TClient : class, IVssHttpClient
    {
      IVssRequestContext context = requestContext;
      bool flag = projectCollectionLevelEnabled && requestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection);
      if (!requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment) && !flag)
      {
        context = requestContext.To(TeamFoundationHostType.Deployment);
        Microsoft.VisualStudio.Services.Identity.Identity userIdentity = requestContext.GetUserIdentity();
        string key = userIdentity?.Descriptor.ToString();
        if (!string.IsNullOrEmpty(key) && requestContext.Items.ContainsKey(key))
          context.Items[key] = (object) userIdentity;
      }
      context.RootContext.Items[RequestContextItemsKeys.UseDelegatedS2STokens] = (object) true;
      return !flag ? context.GetClient<TClient>(TokenServiceBase.s_TokenServiceInstanceType) : context.GetClient<TClient>();
    }

    protected void Cleanup(IVssRequestContext requestContext)
    {
      IVssRequestContext vssRequestContext = requestContext;
      if (!requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
        vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      vssRequestContext.RootContext.Items.Remove(RequestContextItemsKeys.UseDelegatedS2STokens);
    }

    internal static Microsoft.VisualStudio.Services.Identity.Identity ReadIdentity(
      IVssRequestContext requestContext,
      Guid? userId)
    {
      Microsoft.VisualStudio.Services.Identity.Identity identity = requestContext.GetService<IdentityService>().ReadIdentities(requestContext.Elevate(), (IList<Guid>) new Guid[1]
      {
        userId.Value
      }, QueryMembership.None, (IEnumerable<string>) null).FirstOrDefault<Microsoft.VisualStudio.Services.Identity.Identity>();
      if (identity == null)
      {
        IVssRequestContext context = requestContext.To(TeamFoundationHostType.Deployment);
        identity = context.GetService<IdentityService>().ReadIdentities(context.Elevate(), (IList<Guid>) new Guid[1]
        {
          userId.Value
        }, QueryMembership.None, (IEnumerable<string>) null).FirstOrDefault<Microsoft.VisualStudio.Services.Identity.Identity>();
      }
      return identity;
    }

    internal static (IVssRequestContext requestContext, bool isDisposable) CreateUserRequest(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Identity.Identity requestIdentity,
      Microsoft.VisualStudio.Services.Identity.Identity targetIdentity,
      string area,
      string layer,
      bool elevateCall,
      bool isImpersonating = false,
      bool addAuthorizationId = false)
    {
      IVssRequestContext vssRequestContext1 = requestContext.To(TeamFoundationHostType.Deployment);
      IVssSecurityNamespace securityNamespace1 = vssRequestContext1.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(vssRequestContext1, FrameworkSecurity.FrameworkNamespaceId);
      if (!isImpersonating && !securityNamespace1.HasPermission(vssRequestContext1, FrameworkSecurity.FrameworkNamespaceToken, 4, false))
      {
        requestContext.Trace(2048002, TraceLevel.Warning, area, layer, "Request identity does not have permissions {0}.", (object) requestIdentity.MasterId);
        return (elevateCall ? requestContext.Elevate() : requestContext, false);
      }
      if (isImpersonating && requestContext.RequestContextInternal().Actors.Count >= 2)
      {
        IVssSecurityNamespace securityNamespace2 = securityNamespace1;
        IVssRequestContext requestContext1 = vssRequestContext1;
        List<IRequestActor> actors = new List<IRequestActor>();
        actors.Add(requestContext.RequestContextInternal().Actors.FirstOrDefault<IRequestActor>());
        string frameworkNamespaceToken = FrameworkSecurity.FrameworkNamespaceToken;
        EvaluationPrincipal evaluationPrincipal;
        ref EvaluationPrincipal local = ref evaluationPrincipal;
        if (!securityNamespace2.HasPermissionOnActors(requestContext1, (IEnumerable<IRequestActor>) actors, frameworkNamespaceToken, 4, out local, false))
        {
          requestContext.Trace(2048002, TraceLevel.Warning, area, layer, "Request impersonating identity does not have permissions {0}.", (object) requestIdentity.MasterId);
          return (elevateCall ? requestContext.Elevate() : requestContext, false);
        }
      }
      else if (TokenServiceBase.IsCallerGenevaActionsServicePrincipal(requestContext))
        return (elevateCall ? requestContext.Elevate() : requestContext, false);
      requestContext.Trace(2048002, TraceLevel.Warning, area, layer, "Creating new impersonated request context for {0} - {1}.", (object) targetIdentity.MasterId, (object) targetIdentity.Descriptor);
      IVssRequestContext vssRequestContext2 = requestContext.GetService<ITeamFoundationHostManagementService>().BeginUserRequest(requestContext, requestContext.ServiceHost.InstanceId, targetIdentity);
      IRequestContextInternal requestContextInternal = (IRequestContextInternal) vssRequestContext2;
      requestContextInternal.SetE2EId(requestContext.E2EId);
      requestContextInternal.SetOrchestrationId(requestContext.OrchestrationId);
      if (requestContext.RootContext.Items.ContainsKey(RequestContextItemsKeys.AuthorizationId))
        vssRequestContext2.RootContext.Items[RequestContextItemsKeys.AuthorizationId] = requestContext.RootContext.Items[RequestContextItemsKeys.AuthorizationId];
      else if (addAuthorizationId)
        vssRequestContext2.RootContext.Items[RequestContextItemsKeys.AuthorizationId] = (object) "deadbeef-0000-0000-0000-000000000000";
      return (vssRequestContext2, true);
    }

    internal static (IVssRequestContext requestContext, bool isDisposable) GetUserRequestContext(
      IVssRequestContext requestContext,
      string area,
      string layer,
      Guid? userId = null,
      bool elevateCall = false,
      bool impersonateUserAllowed = false,
      bool addAuthorizationId = false)
    {
      requestContext.Trace(2048002, TraceLevel.Warning, area, layer, "Entering GetRequestContext");
      Microsoft.VisualStudio.Services.Identity.Identity authenticatedIdentity = requestContext.GetAuthenticatedIdentity();
      Microsoft.VisualStudio.Services.Identity.Identity userIdentity = requestContext.GetUserIdentity();
      if (authenticatedIdentity == null)
        return (requestContext, false);
      if (userIdentity != null && (authenticatedIdentity != userIdentity || userIdentity.IsServiceIdentity))
      {
        requestContext.Trace(2048002, TraceLevel.Warning, area, layer, "Impersonated call to SPS - request identity {0}, target identity is {1}", (object) authenticatedIdentity.MasterId, (object) userIdentity.MasterId);
        return TokenServiceBase.CreateUserRequest(requestContext, authenticatedIdentity, userIdentity, area, layer, elevateCall, authenticatedIdentity != userIdentity, addAuthorizationId);
      }
      if (userIdentity != null && userId.HasValue)
      {
        Guid guid = userIdentity.StorageKey(requestContext, TeamFoundationHostType.Deployment);
        Guid? nullable = userId;
        if ((nullable.HasValue ? (guid != nullable.GetValueOrDefault() ? 1 : 0) : 1) != 0)
        {
          requestContext.GetService<IdentityService>();
          Microsoft.VisualStudio.Services.Identity.Identity targetIdentity = TokenServiceBase.ReadIdentity(requestContext, userId);
          if (targetIdentity == null)
          {
            requestContext.Trace(2048002, TraceLevel.Warning, area, layer, "Did not find target identity for {0}.", (object) userId.Value);
            return (elevateCall ? requestContext.Elevate() : requestContext, false);
          }
          if (targetIdentity.IsServiceIdentity)
            impersonateUserAllowed = true;
          return TokenServiceBase.CreateUserRequest(requestContext, authenticatedIdentity, targetIdentity, area, layer, elevateCall, impersonateUserAllowed, addAuthorizationId);
        }
      }
      return (elevateCall ? requestContext.Elevate() : requestContext, false);
    }

    internal static T ExecuteTokenServiceResultRequest<T>(
      IVssRequestContext requestContext,
      string area,
      string layer,
      Func<IVssRequestContext, bool, T> lambdaFunction,
      Guid? userId = null,
      bool elevateCall = false,
      bool impersonateUserAllowed = false,
      bool addAuthorizationId = false)
    {
      (IVssRequestContext requestContext, bool isDisposable) userRequestContext = TokenServiceBase.GetUserRequestContext(requestContext, area, layer, userId, elevateCall, impersonateUserAllowed, addAuthorizationId);
      try
      {
        return lambdaFunction(userRequestContext.requestContext, userRequestContext.isDisposable);
      }
      finally
      {
        if (userRequestContext.isDisposable)
          userRequestContext.requestContext.Dispose();
      }
    }

    internal static void ExecuteTokenServiceVoidRequest(
      IVssRequestContext requestContext,
      string area,
      string layer,
      Action<IVssRequestContext> lambdaFunction,
      Guid? userId = null,
      bool elevateCall = false,
      bool addAuthorizationId = false)
    {
      (IVssRequestContext requestContext, bool isDisposable) userRequestContext = TokenServiceBase.GetUserRequestContext(requestContext, area, layer, userId, elevateCall, addAuthorizationId: addAuthorizationId);
      try
      {
        lambdaFunction(userRequestContext.requestContext);
      }
      finally
      {
        if (userRequestContext.isDisposable)
          userRequestContext.requestContext.Dispose();
      }
    }

    private static bool IsCallerGenevaActionsServicePrincipal(IVssRequestContext requestContext)
    {
      Guid spGuid;
      return ServicePrincipals.IsServicePrincipal(requestContext, requestContext.UserContext) && ServicePrincipals.TryParse(requestContext.UserContext, out spGuid, out Guid _) && spGuid == TokenServiceBase.GenevaServiceGuid;
    }
  }
}
