// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.DelegatedAuthorization.PlatformDelegatedAuthorizationBase
// Assembly: Microsoft.VisualStudio.Services.DelegatedAuthorization, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 76926D67-5A10-414E-AFAB-34A210884CEB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.DelegatedAuthorization.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Authentication;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.OrganizationTenant;
using Microsoft.VisualStudio.Services.WebApi.Jwt;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.VisualStudio.Services.DelegatedAuthorization
{
  public abstract class PlatformDelegatedAuthorizationBase
  {
    private IDisposableReadOnlyList<IDelegatedAuthorizationExtension> m_extensions;
    private static readonly Guid s_genevaActionServicePrincipal = Guid.Parse("0000005b-0000-8888-8000-000000000000");
    private const string c_DelegatedAuthorizationFeatureFlag = "VisualStudio.DelegatedAuthorizationService.L2TestOnly.DelegatedAuthorizationService";

    private static PlatformDelegatedAuthorizationBase.PreservedValues RemoveTokensFromResult(
      object result)
    {
      PlatformDelegatedAuthorizationBase.PreservedValues preservedValues = new PlatformDelegatedAuthorizationBase.PreservedValues();
      preservedValues.result = result;
      if (result == null)
        return preservedValues;
      Type type = result.GetType();
      if (type == typeof (SessionTokenResult))
      {
        SessionTokenResult sessionTokenResult = (SessionTokenResult) result;
        if (sessionTokenResult.SessionToken != null)
        {
          preservedValues.token = sessionTokenResult.SessionToken.Token;
          preservedValues.alternateToken = sessionTokenResult.SessionToken.AlternateToken;
          sessionTokenResult.SessionToken.Token = (string) null;
          sessionTokenResult.SessionToken.AlternateToken = (string) null;
        }
      }
      else if (type == typeof (SessionToken))
      {
        SessionToken sessionToken = (SessionToken) result;
        preservedValues.token = sessionToken.Token;
        preservedValues.alternateToken = sessionToken.AlternateToken;
        sessionToken.Token = (string) null;
        sessionToken.AlternateToken = (string) null;
      }
      else if (type == typeof (Registration))
      {
        Registration registration = (Registration) result;
        preservedValues.token = registration.Secret;
        registration.Secret = (string) null;
      }
      else if (type == typeof (AuthorizationDescription))
      {
        AuthorizationDescription authorizationDescription = (AuthorizationDescription) result;
        if (authorizationDescription.ClientRegistration != null)
        {
          preservedValues.token = authorizationDescription.ClientRegistration.Secret;
          authorizationDescription.ClientRegistration.Secret = (string) null;
        }
      }
      else if (type == typeof (AccessTokenResult))
      {
        AccessTokenResult accessTokenResult = (AccessTokenResult) result;
        preservedValues.jsonToken = accessTokenResult.AccessToken;
        preservedValues.grant = accessTokenResult.RefreshToken;
        accessTokenResult.AccessToken = (JsonWebToken) null;
        accessTokenResult.RefreshToken = (RefreshTokenGrant) null;
      }
      else if (type == typeof (AppSessionTokenResult))
      {
        AppSessionTokenResult sessionTokenResult = (AppSessionTokenResult) result;
        preservedValues.token = sessionTokenResult.AppSessionToken;
        sessionTokenResult.AppSessionToken = (string) null;
      }
      else if (type == typeof (AccessToken))
      {
        AccessToken accessToken = (AccessToken) result;
        preservedValues.jsonToken = accessToken.Token;
        preservedValues.jsonRefresh = accessToken.RefreshToken;
        accessToken.Token = (JsonWebToken) null;
        accessToken.RefreshToken = (JsonWebToken) null;
      }
      else if (type == typeof (AuthorizationDetails))
      {
        AuthorizationDetails authorizationDetails = (AuthorizationDetails) result;
        if (authorizationDetails.ClientRegistration != null)
        {
          preservedValues.token = authorizationDetails.ClientRegistration.Secret;
          authorizationDetails.ClientRegistration.Secret = (string) null;
        }
      }
      return preservedValues;
    }

    private static object AddTokensToResult(
      PlatformDelegatedAuthorizationBase.PreservedValues values)
    {
      if (values.result == null)
        return values.result;
      Type type = values.result.GetType();
      if (type == typeof (SessionTokenResult))
      {
        SessionTokenResult result = (SessionTokenResult) values.result;
        if (result.SessionToken != null)
        {
          result.SessionToken.Token = values.token;
          result.SessionToken.AlternateToken = values.alternateToken;
        }
      }
      else if (type == typeof (SessionToken))
      {
        SessionToken result = (SessionToken) values.result;
        result.Token = values.token;
        result.AlternateToken = values.alternateToken;
      }
      else if (type == typeof (Registration))
        ((Registration) values.result).Secret = values.token;
      else if (type == typeof (AuthorizationDescription))
      {
        AuthorizationDescription result = (AuthorizationDescription) values.result;
        if (result.ClientRegistration != null)
          result.ClientRegistration.Secret = values.token;
      }
      else if (type == typeof (AccessTokenResult))
      {
        AccessTokenResult result = (AccessTokenResult) values.result;
        result.AccessToken = values.jsonToken;
        result.RefreshToken = values.grant;
      }
      else if (type == typeof (AppSessionTokenResult))
        ((AppSessionTokenResult) values.result).AppSessionToken = values.token;
      else if (type == typeof (AccessToken))
      {
        AccessToken result = (AccessToken) values.result;
        result.Token = values.jsonToken;
        result.RefreshToken = values.jsonRefresh;
      }
      else if (type == typeof (AuthorizationDetails))
      {
        AuthorizationDetails result = (AuthorizationDetails) values.result;
        if (result.ClientRegistration != null)
          result.ClientRegistration.Secret = values.token;
      }
      return values.result;
    }

    protected static (T result, Exception exception) ExecuteMethod<T>(
      IVssRequestContext requestContext,
      Func<IVssRequestContext, T> serviceMethod,
      int tracepoint,
      int exTracePoint,
      string area,
      string layer,
      string message)
    {
      T result1 = default (T);
      T result2;
      try
      {
        result1 = serviceMethod(requestContext);
        PlatformDelegatedAuthorizationBase.PreservedValues values = PlatformDelegatedAuthorizationBase.RemoveTokensFromResult((object) result1);
        requestContext.TraceSerializedConditionally(tracepoint, TraceLevel.Verbose, area, layer, message, (object) result1);
        result2 = (T) PlatformDelegatedAuthorizationBase.AddTokensToResult(values);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(exTracePoint, area, layer, ex);
        return (result1, ex);
      }
      return (result2, (Exception) null);
    }

    private static (IList<T> result, Exception exception) ExecuteMethod<T>(
      IVssRequestContext requestContext,
      Func<IVssRequestContext, IEnumerable<T>> serviceMethod,
      int tracepoint,
      int exTracePoint,
      string area,
      string layer,
      string message)
    {
      IEnumerable<T> objs;
      try
      {
        objs = serviceMethod(requestContext);
        IList<PlatformDelegatedAuthorizationBase.PreservedValues> preservedValues = (IList<PlatformDelegatedAuthorizationBase.PreservedValues>) new List<PlatformDelegatedAuthorizationBase.PreservedValues>();
        objs.ForEach<T>((Action<T>) (r => preservedValues.Add(PlatformDelegatedAuthorizationBase.RemoveTokensFromResult((object) r))));
        requestContext.TraceSerializedConditionally(tracepoint, TraceLevel.Verbose, area, layer, message, (object) objs);
        preservedValues.ForEach<PlatformDelegatedAuthorizationBase.PreservedValues>((Action<PlatformDelegatedAuthorizationBase.PreservedValues>) (r => PlatformDelegatedAuthorizationBase.AddTokensToResult(r)));
      }
      catch (Exception ex)
      {
        requestContext.TraceException(exTracePoint, area, layer, ex);
        return ((IList<T>) null, ex);
      }
      return (objs == null ? (IList<T>) null : (IList<T>) objs.ToList<T>(), (Exception) null);
    }

    protected T ExecuteServiceMethods<T>(
      IVssRequestContext requestContext,
      Func<IVssRequestContext, T> localServiceMethod,
      Func<IVssRequestContext, T> tokenServiceMethod,
      int tracepoint,
      string area,
      string layer,
      string callerName)
    {
      (T, Exception) valueTuple = !PlatformDelegatedAuthorizationBase.CallTokenService(requestContext) ? PlatformDelegatedAuthorizationBase.ExecuteMethod<T>(requestContext, localServiceMethod, 1048144, 1048138, area, layer, "Sps Result = {0}") : PlatformDelegatedAuthorizationBase.ExecuteMethod<T>(requestContext, tokenServiceMethod, 1048145, 1048139, area, layer, "Token Result = {0}");
      return valueTuple.Item2 == null ? valueTuple.Item1 : throw valueTuple.Item2;
    }

    protected IList<T> ExecuteListServiceMethods<T>(
      IVssRequestContext requestContext,
      Func<IVssRequestContext, IEnumerable<T>> localServiceMethod,
      Func<IVssRequestContext, IEnumerable<T>> tokenServiceMethod,
      string area,
      string layer,
      string callerName)
    {
      (IList<T>, Exception) valueTuple = !PlatformDelegatedAuthorizationBase.CallTokenService(requestContext) ? PlatformDelegatedAuthorizationBase.ExecuteMethod<T>(requestContext, localServiceMethod, 1048146, 1048140, area, layer, "Sps Result = {0}") : PlatformDelegatedAuthorizationBase.ExecuteMethod<T>(requestContext, tokenServiceMethod, 1048147, 1048141, area, layer, "Token Result = {0}");
      return valueTuple.Item2 == null ? valueTuple.Item1 : throw valueTuple.Item2;
    }

    private static bool CallTokenService(IVssRequestContext requestContext)
    {
      if (!requestContext.ExecutionEnvironment.IsHostedDeployment)
        return false;
      return !requestContext.IsFeatureEnabled("VisualStudio.DelegatedAuthorizationService.L2TestOnly.DelegatedAuthorizationService") || !requestContext.ExecutionEnvironment.IsDevFabricDeployment;
    }

    protected bool AreTenantIdsValid(
      IVssRequestContext requestContext,
      IList<Guid> tenantIds,
      string area,
      string layer)
    {
      foreach (IDelegatedAuthorizationExtension extension in (IEnumerable<IDelegatedAuthorizationExtension>) this.m_extensions)
      {
        bool? nullable = extension.AreTenantIdsValid(requestContext, tenantIds);
        if (nullable.HasValue)
          return nullable.Value;
      }
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment).Elevate();
      IOrganizationTenantService service = vssRequestContext.GetService<IOrganizationTenantService>();
      foreach (Guid tenantId in (IEnumerable<Guid>) tenantIds)
      {
        if (((IAuthTenantService) service).GetTenant(vssRequestContext, tenantId) == null)
        {
          requestContext.Trace(1048111, TraceLevel.Info, area, layer, "Tenant Id {0} is not valid.", (object) tenantId);
          throw new RegistrationCreateException(DelegatedAuthorizationResources.RegistrationCreationFailureTenantId());
        }
      }
      return true;
    }

    protected bool IsApplicationRegistrationAllowed(IVssRequestContext requestContext)
    {
      foreach (IDelegatedAuthorizationExtension extension in (IEnumerable<IDelegatedAuthorizationExtension>) this.m_extensions)
      {
        bool? nullable = extension.IsApplicationRegistrationAllowed(requestContext);
        if (nullable.HasValue)
          return nullable.Value;
      }
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      return !(requestContext.GetUserIdentity().Id != PlatformDelegatedAuthorizationBase.s_genevaActionServicePrincipal) && vssRequestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(vssRequestContext, FrameworkSecurity.FrameworkNamespaceId).HasPermission(vssRequestContext, FrameworkSecurity.FrameworkNamespaceToken, 4, false);
    }

    protected bool IsCurrentRequestAuthenticatedUsingAlternateCredentials(
      IVssRequestContext requestContext)
    {
      foreach (IDelegatedAuthorizationExtension extension in (IEnumerable<IDelegatedAuthorizationExtension>) this.m_extensions)
      {
        bool? nullable = extension.IsCurrentRequestAuthenticatedUsingAlternateCredentials(requestContext);
        if (nullable.HasValue)
          return nullable.Value;
      }
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      bool flag1;
      bool flag2;
      return !vssRequestContext.IsImpersonating && !(vssRequestContext.Items.TryGetValue<bool>(RequestContextItemsKeys.AadAccessTokenUsedAsAlternateAuthCredentialsContextKey, out flag1) & flag1) && vssRequestContext.Items.TryGetValue<bool>(RequestContextItemsKeys.AlternateAuthCredentialsContextKey, out flag2) & flag2;
    }

    protected void LoadExtensions(IVssRequestContext requestContext) => this.m_extensions = requestContext.GetExtensions<IDelegatedAuthorizationExtension>();

    protected void DisposeExtensions()
    {
      this.m_extensions?.Dispose();
      this.m_extensions = (IDisposableReadOnlyList<IDelegatedAuthorizationExtension>) null;
    }

    private struct PreservedValues
    {
      public string token;
      public string alternateToken;
      public JsonWebToken jsonToken;
      public JsonWebToken jsonRefresh;
      public RefreshTokenGrant grant;
      public object result;
    }
  }
}
