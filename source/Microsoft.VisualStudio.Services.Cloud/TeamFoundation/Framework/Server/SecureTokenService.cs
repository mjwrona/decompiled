// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.SecureTokenService
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.IdentityModel.Tokens;
using Microsoft.TeamFoundation.Framework.Server.SecureToken;
using Microsoft.TeamFoundation.Framework.Server.SecureToken.Actions;
using Microsoft.VisualStudio.Services.TokenSigningKeyLifecycle;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class SecureTokenService : ISecureTokenService, IVssFrameworkService
  {
    private SecureTokenActionFactory actionFactory;
    private Guid serviceHostId = Guid.Empty;
    private const string c_Area = "Framework";
    private const string c_Layer = "SecureTokenService";

    public JwtSecurityToken IssueToken(
      IVssRequestContext requestContext,
      string audience,
      string issuer,
      IEnumerable<Claim> claims,
      string keyNamespace,
      TimeSpan tokenLifetime,
      DateTimeOffset? validFrom = null)
    {
      requestContext.TraceEnter(0, "Framework", nameof (SecureTokenService), nameof (IssueToken));
      try
      {
        this.ValidateRequestContext(requestContext);
        DateTimeOffset dateTimeOffset = validFrom ?? DateTimeOffset.UtcNow;
        dateTimeOffset = dateTimeOffset.Add(tokenLifetime);
        DateTime dateTime = dateTimeOffset.DateTime;
        IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
        return this.actionFactory.DefaultSecureTokenIssuer.IssueToken(vssRequestContext.GetService<ITokenSigningKeyLifecycleService>().GetSigningKey(vssRequestContext, keyNamespace, (DateTimeOffset) dateTime), audience, issuer, claims, tokenLifetime, validFrom);
      }
      finally
      {
        requestContext.TraceLeave(0, "Framework", nameof (SecureTokenService), nameof (IssueToken));
      }
    }

    public SecureTokenValidationResult ValidateToken(
      IVssRequestContext requestContext,
      string jwtString,
      TokenValidationParameters validationParameters)
    {
      requestContext.TraceEnter(0, "Framework", nameof (SecureTokenService), nameof (ValidateToken));
      SecureTokenValidationResult validationResult = (SecureTokenValidationResult) null;
      try
      {
        this.ValidateRequestContext(requestContext);
        ISecureTokenValidator secureTokenValidator = this.actionFactory.DefaultSecureTokenValidator;
        string signingKeyNamespace = (string) null;
        int keyId = int.MinValue;
        secureTokenValidator.ExtractKeyInformation(jwtString, out signingKeyNamespace, out keyId);
        IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
        TokenSigningKey validationKey = vssRequestContext.GetService<ITokenSigningKeyLifecycleService>().GetValidationKey(vssRequestContext, signingKeyNamespace, keyId);
        validationResult = secureTokenValidator.ValidateToken(jwtString, validationKey, validationParameters);
        return validationResult;
      }
      catch (Exception ex)
      {
        validationResult = new SecureTokenValidationResult()
        {
          FailureReason = SecureTokenValidationFailureReason.Unknown,
          Failure = ex
        };
        return validationResult;
      }
      finally
      {
        if (validationResult?.Failure != null && !(validationResult.Failure is SecureTokenServiceException))
          validationResult.Failure = (Exception) new SecureTokenServiceException(string.Empty, validationResult.Failure);
        requestContext.TraceLeave(0, "Framework", nameof (SecureTokenService), nameof (ValidateToken));
      }
    }

    public void ServiceStart(IVssRequestContext requestContext)
    {
      this.serviceHostId = requestContext?.ServiceHost?.InstanceId ?? Guid.Empty;
      this.actionFactory = new SecureTokenActionFactory();
    }

    public void ServiceEnd(IVssRequestContext requestContext)
    {
    }

    private void ValidateRequestContext(IVssRequestContext requestContext)
    {
      if (!object.Equals((object) this.serviceHostId, (object) requestContext?.ServiceHost?.InstanceId))
        throw new ArgumentException("Request context host" + " mismatch between service creation and api invocation");
    }
  }
}
