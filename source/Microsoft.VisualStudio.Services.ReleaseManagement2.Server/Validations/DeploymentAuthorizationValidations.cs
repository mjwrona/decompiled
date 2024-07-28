// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.Validations.DeploymentAuthorizationValidations
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.OAuth2;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Exceptions;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Properties;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.Validations
{
  public static class DeploymentAuthorizationValidations
  {
    private const int DefaultRevalidateIdentityAuthTimeExpirationInSeconds = 300;
    private const string AuthTimeClaim = "auth_time";

    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", Justification = "This is not shown to the user and is for tracing")]
    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "This is not shown to the user and is for tracing")]
    public static void PerformValidation(
      IVssRequestContext requestContext,
      string identificationToken,
      string nonce,
      JwtSecurityToken jsonWebToken)
    {
      if (requestContext == null)
        throw new ArgumentNullException(nameof (requestContext));
      requestContext.Items.Add(nameof (nonce), (object) nonce);
      ClaimsPrincipal claimsPrincipal = requestContext.GetService<IOAuth2AuthenticationService>().ValidateToken(requestContext, identificationToken, OAuth2TokenValidators.AAD | OAuth2TokenValidators.DelegatedAuth, out jsonWebToken, out bool _, out bool _);
      requestContext.Items.Remove(nameof (nonce));
      if (claimsPrincipal == null)
        throw new InvalidRequestException(Resources.UpdateApprovalFailedToProcessVstsToken);
      if (jsonWebToken == null || jsonWebToken.Claims.IsNullOrEmpty<Claim>())
        throw new InvalidRequestException(Resources.UpdateApprovalFailedAsJsonWebTokenIsInvalid);
      List<Claim> list = jsonWebToken.Claims.Where<Claim>((Func<Claim, bool>) (claim => claim.Type == "auth_time")).ToList<Claim>();
      if (!list.Any<Claim>())
        throw new InvalidRequestException(Resources.UpdateApprovalFailedAsAuthorizationTimeNotPresent);
      string s = list.Select<Claim, string>((Func<Claim, string>) (claim => claim.Value)).FirstOrDefault<string>();
      long result;
      if (!long.TryParse(s, out result))
        throw new InvalidRequestException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.UpdateApprovalFailedDueToInvalidAuthorizationTime, (object) s));
      DeploymentAuthorizationValidations.ValidateAuthTime(requestContext, result);
      Microsoft.VisualStudio.Services.Identity.Identity identity = requestContext.GetService<IIdentityAuthenticationService>().ResolveIdentity(requestContext, claimsPrincipal.Identity);
      Microsoft.VisualStudio.Services.Identity.Identity userIdentity = requestContext.GetUserIdentity();
      bool flag = false;
      if (userIdentity.Id == identity.Id)
      {
        flag = true;
      }
      else
      {
        requestContext.Trace(1976394, TraceLevel.Info, "ReleaseManagementService", "Service", "Identity Id is not matching, trying to compare identity's PUID", (object) identity.DisplayName, (object) identity.Id);
        string str1;
        if (userIdentity.Properties.TryGetValue<string>("PUID", out str1))
        {
          string str2;
          if (identity.Properties.TryGetValue<string>("PUID", out str2))
            flag = str1.Equals(str2, StringComparison.OrdinalIgnoreCase);
          else
            requestContext.Trace(1976392, TraceLevel.Error, "ReleaseManagementService", "Service", "Cannot access PUID from the executor's identity {0} {1}", (object) identity.DisplayName, (object) identity.Id);
        }
        else
          requestContext.Trace(1976393, TraceLevel.Error, "ReleaseManagementService", "Service", "Cannot access PUID from the requestor's identity {0} {1}", (object) identity.DisplayName, (object) identity.Id);
      }
      if (!flag)
        throw new InvalidRequestException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, Resources.UpdateApprovalRevalidateIdentityEnabledButApproverNotMatching, (object) userIdentity.DisplayName, (object) identity.DisplayName));
    }

    private static void ValidateAuthTime(IVssRequestContext requestContext, long lastLoginTime)
    {
      int num1 = requestContext.GetService<IVssRegistryService>().GetValue<int>(requestContext, (RegistryQuery) "/Service/ReleaseManagement/Settings/RevalidateIdentityAuthorizationTimeExpirationInSeconds", true, 300);
      long num2 = DateTime.UtcNow.ToUnixEpochTime() - lastLoginTime;
      if (num2 > (long) num1)
        throw new InvalidRequestException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.UpdateApprovalFailedAsLastLoginTimeExcceeded, (object) num2));
    }
  }
}
