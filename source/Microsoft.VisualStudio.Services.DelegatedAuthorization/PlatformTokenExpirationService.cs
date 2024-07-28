// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.DelegatedAuthorization.PlatformTokenExpirationService
// Assembly: Microsoft.VisualStudio.Services.DelegatedAuthorization, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 76926D67-5A10-414E-AFAB-34A210884CEB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.DelegatedAuthorization.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.DelegatedAuthorization.DataAccess;
using Microsoft.VisualStudio.Services.Security;
using Microsoft.VisualStudio.Services.Tokens;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.VisualStudio.Services.DelegatedAuthorization
{
  public class PlatformTokenExpirationService : 
    PlatformDelegatedAuthorizationBase,
    IDelegatedAuthorizationTokenExpirationService,
    IVssFrameworkService
  {
    private const string Area = "DelegatedAuthorizationService";
    private const string Layer = "PlatformTokenExpirationService";

    public void ServiceStart(IVssRequestContext requestContext) => requestContext.CheckDeploymentRequestContext();

    public void ServiceEnd(IVssRequestContext requestContext)
    {
    }

    private void ImpersonatePermissionValidation(
      IVssRequestContext deploymentRequestContext,
      int tracePoint,
      string message)
    {
      if (!deploymentRequestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(deploymentRequestContext, FrameworkSecurity.FrameworkNamespaceId).HasPermission(deploymentRequestContext, FrameworkSecurity.FrameworkNamespaceToken, 4, false))
      {
        deploymentRequestContext.Trace(tracePoint, TraceLevel.Warning, "DelegatedAuthorizationService", nameof (PlatformTokenExpirationService), message);
        throw new AccessCheckException(message);
      }
    }

    public IList<ExpiringToken> GetExpiringTokens(
      IVssRequestContext requestContext,
      DateTime setToExpireOn)
    {
      requestContext.CheckDeploymentRequestContext();
      return this.ExecuteListServiceMethods<ExpiringToken>(requestContext, (Func<IVssRequestContext, IEnumerable<ExpiringToken>>) (context => (IEnumerable<ExpiringToken>) this.GetExpiringTokensInternal(context, setToExpireOn)), (Func<IVssRequestContext, IEnumerable<ExpiringToken>>) (context => (IEnumerable<ExpiringToken>) this.GetExpiringTokensRemote(context, setToExpireOn)), "DelegatedAuthorizationService", nameof (PlatformTokenExpirationService), nameof (GetExpiringTokens));
    }

    public IList<ExpiringToken> GetExpiringTokensRemote(
      IVssRequestContext requestContext,
      DateTime setToExpireOn)
    {
      return requestContext.GetService<ITokenExpirationService>().GetExpiringTokens(requestContext, setToExpireOn);
    }

    public IList<ExpiringToken> GetExpiringTokensInternal(
      IVssRequestContext requestContext,
      DateTime setToExpireOn)
    {
      requestContext.CheckDeploymentRequestContext();
      this.ImpersonatePermissionValidation(requestContext, 1030012, "User does not have permission to list expiring tokens.");
      using (TokenExpirationComponent component = requestContext.CreateComponent<TokenExpirationComponent>())
        return (IList<ExpiringToken>) component.GetExpiringTokens(setToExpireOn);
    }
  }
}
