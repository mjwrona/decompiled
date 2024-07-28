// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.AzureActiveDirectory.Tokens.FrameworkAadTokenService
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.Partitioning.Server;
using Microsoft.VisualStudio.Services.WebApi.HttpClients;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Cloud.AzureActiveDirectory.Tokens
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  internal class FrameworkAadTokenService : AadTokenServiceBase
  {
    protected internal override JwtSecurityToken GetUserAccessToken(
      IVssRequestContext requestContext,
      string resource,
      string tenantId,
      Microsoft.VisualStudio.Services.Identity.Identity identity)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<Microsoft.VisualStudio.Services.Identity.Identity>(identity, nameof (identity));
      IdentityDescriptor descriptor = identity.Descriptor;
      requestContext.TraceEnter(9002101, "AzureActiveDirectory", "AadTokenService", nameof (GetUserAccessToken));
      try
      {
        ArgumentUtility.CheckStringForNullOrEmpty(resource, nameof (resource));
        ArgumentUtility.CheckStringForNullOrEmpty(tenantId, nameof (tenantId));
        ArgumentUtility.CheckForNull<IdentityDescriptor>(descriptor, "identityDescriptor");
        Guid result;
        if (Guid.TryParse(tenantId, out result) && result == Guid.Empty)
          throw new ArgumentException("The TenantId must not be an empty guid.");
        requestContext.Trace(9002103, TraceLevel.Info, "AzureActiveDirectory", "AadTokenService", "Getting user access token for identity {0}, resource {1} and tenant {2}.", (object) identity.SubjectDescriptor.Identifier, (object) resource, (object) tenantId);
        string jwtEncodedString = this.GetClient(requestContext).GetAadUserAccessToken(resource, tenantId, descriptor).SyncResult<string>();
        if (string.IsNullOrEmpty(jwtEncodedString))
        {
          requestContext.Trace(9002105, TraceLevel.Error, "AzureActiveDirectory", "AadTokenService", "No user access token for identity {0}, resource {1} and tenant {2} was returned.", (object) identity.SubjectDescriptor.Identifier, (object) resource, (object) tenantId);
          return (JwtSecurityToken) null;
        }
        JwtSecurityToken userAccessToken = new JwtSecurityToken(jwtEncodedString);
        requestContext.Trace(9002104, TraceLevel.Info, "AzureActiveDirectory", "AadTokenService", "Successfully got user access token for identity {0}, resource {1} and tenant {2}. Token valid to {3}", (object) identity.SubjectDescriptor.Identifier, (object) resource, (object) tenantId, (object) userAccessToken.ValidTo);
        return userAccessToken;
      }
      finally
      {
        requestContext.TraceLeave(9002102, "AzureActiveDirectory", "AadTokenService", nameof (GetUserAccessToken));
      }
    }

    protected internal override JwtSecurityToken GetAppAccessToken(
      IVssRequestContext requestContext,
      string resource,
      string tenantId)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckStringForNullOrEmpty(resource, nameof (resource));
      ArgumentUtility.CheckStringForNullOrEmpty(tenantId, nameof (tenantId));
      Guid result;
      if (Guid.TryParse(tenantId, out result) && result == Guid.Empty)
        throw new ArgumentException("The TenantId must not be an empty guid.");
      requestContext.TraceEnter(9002106, "AzureActiveDirectory", "AadTokenService", nameof (GetAppAccessToken));
      requestContext.Trace(9002108, TraceLevel.Info, "AzureActiveDirectory", "AadTokenService", "Getting app access token for resource {0} and tenant {1}", (object) resource, (object) tenantId);
      string jwtEncodedString = this.GetClient(requestContext).GetAadAppAccessToken(resource, tenantId).SyncResult<string>();
      if (string.IsNullOrEmpty(jwtEncodedString))
      {
        requestContext.Trace(9002110, TraceLevel.Error, "AzureActiveDirectory", "AadTokenService", "Failed to get an app access token for resource {0} and tenant {1}", (object) resource, (object) tenantId);
        requestContext.TraceLeave(9002107, "AzureActiveDirectory", "AadTokenService", nameof (GetAppAccessToken));
        return (JwtSecurityToken) null;
      }
      JwtSecurityToken appAccessToken = new JwtSecurityToken(jwtEncodedString);
      requestContext.Trace(9002109, TraceLevel.Info, "AzureActiveDirectory", "AadTokenService", "Successfully got an app access token for resource {0} and tenant {1}. Token valid to {2}", (object) resource, (object) tenantId, (object) appAccessToken.ValidTo);
      requestContext.TraceLeave(9002107, "AzureActiveDirectory", "AadTokenService", nameof (GetAppAccessToken));
      return appAccessToken;
    }

    protected internal override JwtSecurityToken GetUserAccessTokenFromAuthCode(
      IVssRequestContext requestContext,
      string authCode,
      string resource,
      string tenantId,
      Uri replyToUri,
      Microsoft.VisualStudio.Services.Identity.Identity identity)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckStringForNullOrEmpty(authCode, nameof (authCode));
      ArgumentUtility.CheckStringForNullOrEmpty(resource, nameof (resource));
      ArgumentUtility.CheckStringForNullOrEmpty(tenantId, nameof (tenantId));
      ArgumentUtility.CheckForNull<Uri>(replyToUri, nameof (replyToUri));
      ArgumentUtility.CheckForNull<Microsoft.VisualStudio.Services.Identity.Identity>(identity, nameof (identity));
      Guid result;
      if (Guid.TryParse(tenantId, out result) && result == Guid.Empty)
        throw new ArgumentException("The TenantId must not be an empty guid.");
      requestContext.TraceEnter(9002116, "AzureActiveDirectory", "AadTokenService", nameof (GetUserAccessTokenFromAuthCode));
      requestContext.Trace(9002118, TraceLevel.Info, "AzureActiveDirectory", "AadTokenService", "Getting a user access token using auth code for identity {0}, resource {1} and tenant {2}", (object) identity.Descriptor.Identifier, (object) resource, (object) tenantId);
      string jwtEncodedString = this.GetClient(requestContext).GetUserAccessTokenFromAuthCode(authCode, resource, tenantId, replyToUri.AbsoluteUri, identity.Descriptor).SyncResult<string>();
      if (string.IsNullOrEmpty(jwtEncodedString))
      {
        requestContext.Trace(9002120, TraceLevel.Error, "AzureActiveDirectory", "AadTokenService", "Failed to get a user access token using auth code for identity {0}, resource {1} and tenant {2}", (object) identity.Descriptor.Identifier, (object) resource, (object) tenantId);
        requestContext.TraceLeave(9002107, "AzureActiveDirectory", "AadTokenService", nameof (GetUserAccessTokenFromAuthCode));
        return (JwtSecurityToken) null;
      }
      JwtSecurityToken tokenFromAuthCode = new JwtSecurityToken(jwtEncodedString);
      requestContext.Trace(9002119, TraceLevel.Info, "AzureActiveDirectory", "AadTokenService", "Successfully obtained a user access token using auth code for identity {0}, resource {1} and tenant {2}. Token valid to {3}", (object) identity.Descriptor.Identifier, (object) resource, (object) tenantId, (object) tokenFromAuthCode.ValidTo);
      requestContext.TraceLeave(9002117, "AzureActiveDirectory", "AadTokenService", nameof (GetUserAccessTokenFromAuthCode));
      return tokenFromAuthCode;
    }

    protected internal override string UpdateRefreshToken(
      IVssRequestContext requestContext,
      string accessToken,
      string resource,
      string tenantId,
      Microsoft.VisualStudio.Services.Identity.Identity identity)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckStringForNullOrEmpty(accessToken, nameof (accessToken));
      ArgumentUtility.CheckStringForNullOrEmpty(resource, nameof (resource));
      ArgumentUtility.CheckStringForNullOrEmpty(tenantId, nameof (tenantId));
      ArgumentUtility.CheckForNull<Microsoft.VisualStudio.Services.Identity.Identity>(identity, nameof (identity));
      Guid result;
      if (Guid.TryParse(tenantId, out result) && result == Guid.Empty)
        throw new ArgumentException("The TenantId must not be an empty guid.");
      requestContext.TraceEnter(9002111, "AzureActiveDirectory", "AadTokenService", nameof (UpdateRefreshToken));
      requestContext.Trace(9002113, TraceLevel.Info, "AzureActiveDirectory", "AadTokenService", "Updating refresh token on behalf of identity {0}, resource {1}, tenant {2} and access token {3}", (object) identity.Descriptor.Identifier, (object) resource, (object) tenantId, (object) accessToken);
      string str = this.GetClient(requestContext).UpdateRefreshTokenOnBehalfOf(accessToken, resource, tenantId, identity.Descriptor).SyncResult<string>();
      if (string.IsNullOrEmpty(str))
      {
        requestContext.Trace(9002115, TraceLevel.Error, "AzureActiveDirectory", "AadTokenService", "No user access token for identity {0}, resource {1} and tenant {2} was returned.", (object) identity.Descriptor.Identifier, (object) resource, (object) tenantId);
        requestContext.TraceLeave(9002112, "AzureActiveDirectory", "AadTokenService", nameof (UpdateRefreshToken));
        return (string) null;
      }
      requestContext.Trace(9002114, TraceLevel.Info, "AzureActiveDirectory", "AadTokenService", "Successfully update refresh token for identity {0}, resource {1} and tenant {2}.", (object) identity.Descriptor.Identifier, (object) resource, (object) tenantId);
      requestContext.TraceLeave(9002112, "AzureActiveDirectory", "AadTokenService", nameof (UpdateRefreshToken));
      return str;
    }

    protected internal override Task<string> GetUserAccessTokenAsync(
      IVssRequestContext requestContext,
      string resource,
      string tenantId,
      IdentityDescriptor identityDescriptor)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<IdentityDescriptor>(identityDescriptor, nameof (identityDescriptor));
      return this.GetClient(requestContext).GetAadUserAccessToken(resource, tenantId, identityDescriptor);
    }

    private TokenHttpClient GetClient(IVssRequestContext requestContext) => !requestContext.RootContext.ServiceHost.Is(TeamFoundationHostType.Deployment) ? PartitionedClientHelper.GetSpsClientForHostId<TokenHttpClient>(requestContext.Elevate(), requestContext.RootContext.ServiceHost.InstanceId) : requestContext.Elevate().GetClient<TokenHttpClient>();
  }
}
