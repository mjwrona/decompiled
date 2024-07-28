// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.DelegatedAuthorization.PlatformDelegatedAuthorizationServiceBase
// Assembly: Microsoft.VisualStudio.Services.DelegatedAuthorization, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 76926D67-5A10-414E-AFAB-34A210884CEB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.DelegatedAuthorization.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.VisualStudio.Services.DelegatedAuthorization
{
  internal abstract class PlatformDelegatedAuthorizationServiceBase : 
    PlatformDelegatedAuthorizationBase
  {
    internal void HasImpersonatePermission(
      IVssRequestContext requestContext,
      int tracePoint,
      string area,
      string layer,
      string message)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      if (!vssRequestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(vssRequestContext, FrameworkSecurity.FrameworkNamespaceId).HasPermission(vssRequestContext, FrameworkSecurity.FrameworkNamespaceToken, 4, false))
      {
        requestContext.Trace(tracePoint, TraceLevel.Warning, area, layer, message);
        throw new PlatformDelegatedAuthorizationServiceException(PlatformDelegatedAuthorizationServiceError.NoImpersonatePermission);
      }
    }

    protected void TryAuthenticateTheCredentialsBeforeIssuingToken(
      IVssRequestContext requestContext,
      int tracePoint,
      string area,
      string layer,
      string message)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      if (this.IsCurrentRequestAuthenticatedUsingAlternateCredentials(vssRequestContext) && !vssRequestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(vssRequestContext, FrameworkSecurity.FrameworkNamespaceId).HasPermission(vssRequestContext, FrameworkSecurity.FrameworkNamespaceToken, 4, false))
      {
        requestContext.Trace(1430002, TraceLevel.Warning, area, layer, message);
        throw new PlatformDelegatedAuthorizationServiceException(PlatformDelegatedAuthorizationServiceError.NoImpersonatePermission);
      }
    }

    protected void CheckShardedIdentity(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Identity.Identity targetIdentity,
      int tracePoint,
      string area,
      string layer,
      string message)
    {
      if (IdentityHelper.IsShardedFrameworkIdentity(requestContext, targetIdentity.Descriptor))
        return;
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      IdentityService service = vssRequestContext.GetService<IdentityService>();
      Microsoft.VisualStudio.Services.Identity.Identity readIdentity = service.ReadIdentities(vssRequestContext.Elevate(), (IList<IdentityDescriptor>) new IdentityDescriptor[1]
      {
        GroupWellKnownIdentityDescriptors.NamespaceAdministratorsGroup
      }, QueryMembership.None, (IEnumerable<string>) null)[0];
      if (service.IsMember(vssRequestContext, readIdentity.Descriptor, targetIdentity.Descriptor) && requestContext.ExecutionEnvironment.IsHostedDeployment)
      {
        requestContext.Trace(tracePoint, TraceLevel.Warning, area, layer, message);
        throw new PlatformDelegatedAuthorizationServiceException(PlatformDelegatedAuthorizationServiceError.InvalidUserType);
      }
    }

    protected Guid GetUserIdCheckForEmpty(
      IVssRequestContext requestContext,
      Guid? userId,
      out Microsoft.VisualStudio.Services.Identity.Identity requestIdentity,
      out Microsoft.VisualStudio.Services.Identity.Identity targetIdentity,
      bool tryReadingIdentityAtDeploymentLevelAsWell = false)
    {
      targetIdentity = (Microsoft.VisualStudio.Services.Identity.Identity) null;
      requestIdentity = requestContext.GetUserIdentity();
      if (userId.HasValue)
      {
        Guid? nullable = userId;
        Guid empty = Guid.Empty;
        if ((nullable.HasValue ? (nullable.HasValue ? (nullable.GetValueOrDefault() == empty ? 1 : 0) : 1) : 0) != 0)
        {
          targetIdentity = requestIdentity;
          userId = new Guid?(targetIdentity.StorageKey(requestContext, TeamFoundationHostType.Deployment));
          goto label_5;
        }
      }
      IdentityService service1 = requestContext.GetService<IdentityService>();
      targetIdentity = service1.ReadIdentities(requestContext.Elevate(), (IList<Guid>) new Guid[1]
      {
        userId.Value
      }, QueryMembership.None, (IEnumerable<string>) null).SingleOrDefault<Microsoft.VisualStudio.Services.Identity.Identity>();
      if (targetIdentity == null)
      {
        IVssRequestContext context = requestContext.To(TeamFoundationHostType.Deployment);
        IdentityService service2 = context.GetService<IdentityService>();
        targetIdentity = service2.ReadIdentities(context.Elevate(), (IList<Guid>) new Guid[1]
        {
          userId.Value
        }, QueryMembership.None, (IEnumerable<string>) null).FirstOrDefault<Microsoft.VisualStudio.Services.Identity.Identity>();
      }
label_5:
      if (userId.HasValue)
      {
        Guid? nullable = userId;
        Guid empty = Guid.Empty;
        if ((nullable.HasValue ? (nullable.HasValue ? (nullable.GetValueOrDefault() == empty ? 1 : 0) : 1) : 0) == 0)
          return userId.Value;
      }
      return requestIdentity.StorageKey(requestContext, TeamFoundationHostType.Deployment);
    }

    protected bool IsServicePrincipal(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Identity.Identity targetIdentity,
      string area,
      string layer)
    {
      if (targetIdentity != null && IdentityHelper.IsUserIdentity(requestContext, (IReadOnlyVssIdentity) targetIdentity))
        return false;
      requestContext.Trace(1048007, TraceLevel.Warning, area, layer, "Request identity {0} is a Service Principal.", (object) targetIdentity?.Id);
      return true;
    }

    internal bool IsUserAuthorized(
      IVssRequestContext requestContext,
      Guid requestIdentityStorageKey,
      Guid userId,
      string area,
      string layer)
    {
      if (requestIdentityStorageKey != userId)
      {
        requestContext.Trace(1048007, TraceLevel.Warning, area, layer, "Request identity is not same as target identity for issue token.");
        IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
        if (!vssRequestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(vssRequestContext, FrameworkSecurity.FrameworkNamespaceId).HasPermission(vssRequestContext, FrameworkSecurity.FrameworkNamespaceToken, 4, false))
        {
          requestContext.Trace(1048007, TraceLevel.Warning, area, layer, "Request identity {0} does not have framework impersonate permissions, target identity {1}.", (object) requestIdentityStorageKey, (object) userId);
          return false;
        }
      }
      return true;
    }

    protected void TryValidatingUserForInitiateAuthorization(
      IVssRequestContext requestContext,
      Guid userId,
      string area,
      string layer)
    {
      Microsoft.VisualStudio.Services.Identity.Identity requestIdentity;
      Microsoft.VisualStudio.Services.Identity.Identity targetIdentity;
      userId = this.GetUserIdCheckForEmpty(requestContext, new Guid?(userId), out requestIdentity, out targetIdentity);
      if (!this.IsUserAuthorized(requestContext, requestIdentity.StorageKey(requestContext, TeamFoundationHostType.Deployment), userId, area, layer) || this.IsServicePrincipal(requestContext, targetIdentity, area, layer))
      {
        requestContext.Trace(1048007, TraceLevel.Warning, area, layer, "Request identity {0} is not authorized to InitiateAuthorization.", (object) userId);
        throw new PlatformDelegatedAuthorizationServiceException(PlatformDelegatedAuthorizationServiceError.AccessDenied);
      }
    }

    internal void TryValidatingIfDelegateAuthIsAllowedForUser(
      IVssRequestContext requestContext,
      Guid userId)
    {
      Microsoft.VisualStudio.Services.Identity.Identity requestIdentity;
      Microsoft.VisualStudio.Services.Identity.Identity targetIdentity;
      userId = this.GetUserIdCheckForEmpty(requestContext, new Guid?(userId), out requestIdentity, out targetIdentity);
      if (userId == Guid.Empty || targetIdentity == null)
        throw new PlatformDelegatedAuthorizationServiceException(PlatformDelegatedAuthorizationServiceError.InvalidUserId);
      if (!IdentityHelper.IsUserIdentity(requestContext, (IReadOnlyVssIdentity) targetIdentity))
        throw new PlatformDelegatedAuthorizationServiceException(PlatformDelegatedAuthorizationServiceError.InvalidUserType);
      if (ServicePrincipals.IsServicePrincipal(requestContext, requestContext.GetAuthenticatedDescriptor()))
        throw new PlatformDelegatedAuthorizationServiceException(PlatformDelegatedAuthorizationServiceError.InvalidUserType);
      if (requestIdentity.StorageKey(requestContext, TeamFoundationHostType.Deployment) != userId)
        throw new PlatformDelegatedAuthorizationServiceException(PlatformDelegatedAuthorizationServiceError.AccessDenied);
    }

    protected Guid TryValidateIdentityBeforeIssuingToken(
      IVssRequestContext requestContext,
      Guid? userId,
      string area,
      string layer)
    {
      if (!userId.HasValue || userId.Value == Guid.Empty)
        userId = new Guid?(requestContext.GetUserId());
      Microsoft.VisualStudio.Services.Identity.Identity identity = requestContext.GetService<IdentityService>().ReadIdentities(requestContext.Elevate(), (IList<Guid>) new Guid[1]
      {
        userId.Value
      }, QueryMembership.None, (IEnumerable<string>) null).FirstOrDefault<Microsoft.VisualStudio.Services.Identity.Identity>();
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      if (identity == null)
        identity = vssRequestContext.GetService<IdentityService>().ReadIdentities(vssRequestContext.Elevate(), (IList<Guid>) new Guid[1]
        {
          userId.Value
        }, QueryMembership.None, (IEnumerable<string>) null).FirstOrDefault<Microsoft.VisualStudio.Services.Identity.Identity>();
      if (identity == null)
        throw new PlatformDelegatedAuthorizationServiceException(PlatformDelegatedAuthorizationServiceError.InvalidUserId);
      if (!IdentityHelper.IsUserIdentity(vssRequestContext, (IReadOnlyVssIdentity) identity))
      {
        requestContext.Trace(1048025, TraceLevel.Warning, area, layer, "IssueAccessTokenKey is requested for non user identity.");
        throw new PlatformDelegatedAuthorizationServiceException(PlatformDelegatedAuthorizationServiceError.InvalidUserType);
      }
      if (!vssRequestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(vssRequestContext, FrameworkSecurity.FrameworkNamespaceId).HasPermission(vssRequestContext, FrameworkSecurity.FrameworkNamespaceToken, 4, false))
      {
        requestContext.Trace(1048026, TraceLevel.Warning, area, layer, "User does not have permission to issue application token.");
        throw new PlatformDelegatedAuthorizationServiceException(PlatformDelegatedAuthorizationServiceError.AccessDenied);
      }
      return identity.StorageKey(vssRequestContext, TeamFoundationHostType.Deployment);
    }
  }
}
