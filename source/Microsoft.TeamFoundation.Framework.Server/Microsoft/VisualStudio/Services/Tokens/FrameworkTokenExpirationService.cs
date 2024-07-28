// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Tokens.FrameworkTokenExpirationService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.DelegatedAuthorization;
using Microsoft.VisualStudio.Services.Security;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;

namespace Microsoft.VisualStudio.Services.Tokens
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  internal class FrameworkTokenExpirationService : 
    TokenServiceBase,
    ITokenExpirationService,
    IDelegatedAuthorizationTokenExpirationService,
    IVssFrameworkService
  {
    private const string Area = "Token";
    private const string Layer = "FrameworkTokenExpirationService";

    private void ImpersonatePermissionValidation(
      IVssRequestContext deploymentRequestContext,
      int tracePoint,
      string message)
    {
      if (!deploymentRequestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(deploymentRequestContext, FrameworkSecurity.FrameworkNamespaceId).HasPermission(deploymentRequestContext, FrameworkSecurity.FrameworkNamespaceToken, 4, false))
      {
        deploymentRequestContext.Trace(tracePoint, TraceLevel.Warning, "Token", nameof (FrameworkTokenExpirationService), message);
        throw new AccessCheckException(message);
      }
    }

    public IList<ExpiringToken> GetExpiringTokens(
      IVssRequestContext requestContext,
      DateTime setToExpireOn)
    {
      throw new NotImplementedException();
    }

    public IList<ExpiringToken> GetExpiringTokens(
      IVssRequestContext requestContext,
      DateTime beginDate,
      DateTime endDate)
    {
      throw new NotImplementedException();
    }
  }
}
