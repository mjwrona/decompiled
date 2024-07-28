// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Ssh.Server.ExtensibilityProviders.AuthenticationProvider
// Assembly: Microsoft.TeamFoundation.Ssh.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9CD799E8-FCEB-494E-B317-B5A120D16BCC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Ssh.Server.dll

using Microsoft.IdentityModel.Tokens;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Ssh.Server.Core;
using Microsoft.TeamFoundation.Ssh.Server.Core.ExtensibilityProviders;
using Microsoft.VisualStudio.Services.Account;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.DelegatedAuthorization;
using Microsoft.VisualStudio.Services.HostManagement;
using Microsoft.VisualStudio.Services.HostManagement.Server;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.Location.Server;
using Microsoft.VisualStudio.Services.Organization;
using Microsoft.VisualStudio.Services.Organization.Client;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Jwt;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading;

namespace Microsoft.TeamFoundation.Ssh.Server.ExtensibilityProviders
{
  public class AuthenticationProvider : IAuthenticationProvider
  {
    internal const string s_Area = "Ssh";
    internal const string s_Layer = "AuthenticationProvider";
    internal const string AudiencePartPrefix = "vso:";
    internal const string SshAuthenticationForwardingKeyNamespace = "ssh";
    internal const string s_clientIpClaimName = "client_ip";

    internal virtual bool TryGetAccount(
      IVssRequestContext requestContext,
      string accountName,
      out Guid accountId)
    {
      requestContext.TraceEnter(13000310, "Ssh", nameof (AuthenticationProvider), nameof (TryGetAccount));
      try
      {
        accountId = new Guid();
        if (HostNameResolver.TryGetCollectionServiceHostId(requestContext, accountName, out accountId))
          return true;
        requestContext.Trace(13000311, TraceLevel.Info, "Ssh", nameof (AuthenticationProvider), "Account \"{0}\" not found.", (object) accountName);
        return false;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(13000318, "Ssh", nameof (AuthenticationProvider), ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(13000319, "Ssh", nameof (AuthenticationProvider), nameof (TryGetAccount));
      }
    }

    private string GetCurrentInstanceId(IVssRequestContext requestContext) => requestContext.ServiceHost.DeploymentServiceHost.InstanceId.ToString();

    private string[] GetAllInstanceIds(IVssRequestContext requestContext)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      return vssRequestContext.GetService<IInstanceManagementService>().GetServiceInstances(vssRequestContext, ServiceInstanceTypes.TFS).Select<ServiceInstance, string>((Func<ServiceInstance, string>) (instance => instance.InstanceId.ToString())).ToArray<string>();
    }

    public string GenerateForwardCredentials(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Identity.Identity identity,
      string targetAudience)
    {
      string currentInstanceId = this.GetCurrentInstanceId(requestContext);
      IVssRequestContext context = requestContext.Elevate();
      ISecureTokenService service = context.GetService<ISecureTokenService>();
      TimeSpan timeSpan = new TimeSpan(1, 0, 0);
      List<Claim> claimList1 = new List<Claim>();
      claimList1.Add(new Claim("nameid", identity.Id.ToString()));
      claimList1.Add(new Claim("X-VSS-E2EID", requestContext.E2EId.ToString("D")));
      claimList1.Add(new Claim("client_ip", requestContext.RemoteIPAddress()));
      IVssRequestContext requestContext1 = context;
      string audience = targetAudience;
      string issuer = currentInstanceId;
      List<Claim> claimList2 = claimList1;
      TimeSpan tokenLifetime = timeSpan;
      DateTimeOffset? validFrom = new DateTimeOffset?();
      return service.IssueToken(requestContext1, audience, issuer, (IEnumerable<Claim>) claimList2, "ssh", tokenLifetime, validFrom).RawData;
    }

    public (Microsoft.VisualStudio.Services.Identity.Identity identity, Guid? e2eId, string clientIp) AuthenticateForwardedCredentials(
      IVssRequestContext requestContext,
      string username,
      string forwardedToken)
    {
      requestContext.TraceEnter(13000350, "Ssh", nameof (AuthenticationProvider), nameof (AuthenticateForwardedCredentials));
      try
      {
        if (requestContext.IsFeatureEnabled("VisualStudio.Services.Ssh.AllowForwardedAuthentication"))
        {
          string currentInstanceId = this.GetCurrentInstanceId(requestContext);
          ISecureTokenService service = requestContext.Elevate().GetService<ISecureTokenService>();
          TokenValidationParameters validationParameters1 = new TokenValidationParameters();
          validationParameters1.ValidIssuers = (IEnumerable<string>) this.GetAllInstanceIds(requestContext);
          validationParameters1.ValidAudience = currentInstanceId;
          IVssRequestContext requestContext1 = requestContext;
          string jwtString = forwardedToken;
          TokenValidationParameters validationParameters2 = validationParameters1;
          SecureTokenValidationResult validationResult = service.ValidateToken(requestContext1, jwtString, validationParameters2);
          if (validationResult != null && validationResult.validatedJwt != null)
            return (this.ReadIdentityFromToken(requestContext, validationResult.validatedJwt, username), this.ReadE2EIdFromToken(validationResult.validatedJwt), this.ReadClientIpFromToken(validationResult.validatedJwt));
          requestContext.Trace(13000352, TraceLevel.Info, "Ssh", nameof (AuthenticationProvider), "The token could not be validated.");
          return ((Microsoft.VisualStudio.Services.Identity.Identity) null, new Guid?(), (string) null);
        }
        requestContext.Trace(13000351, TraceLevel.Info, "Ssh", nameof (AuthenticationProvider), "Feature flag for forwarded authentication \"{0}\" is disabled.", (object) "VisualStudio.Services.Ssh.AllowForwardedAuthentication");
        return ((Microsoft.VisualStudio.Services.Identity.Identity) null, new Guid?(), (string) null);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(13000358, "Ssh", nameof (AuthenticationProvider), ex);
        return ((Microsoft.VisualStudio.Services.Identity.Identity) null, new Guid?(), (string) null);
      }
      finally
      {
        requestContext.TraceLeave(13000359, "Ssh", nameof (AuthenticationProvider), nameof (AuthenticateForwardedCredentials));
      }
    }

    internal Microsoft.VisualStudio.Services.Identity.Identity ReadIdentityFromToken(
      IVssRequestContext requestContext,
      JsonWebToken webToken,
      string accountName)
    {
      return this.GetIdentity(requestContext, webToken);
    }

    internal Microsoft.VisualStudio.Services.Identity.Identity ReadIdentityFromToken(
      IVssRequestContext requestContext,
      JwtSecurityToken webToken,
      string accountName)
    {
      if (!requestContext.ExecutionEnvironment.IsHostedDeployment)
        return this.GetIdentity(requestContext, webToken);
      Guid accountId;
      if (!this.TryGetAccount(requestContext, accountName, out accountId))
        throw new AccountNotFoundException("Could not find an account named " + accountName);
      using (IVssRequestContext requestContext1 = requestContext.CreateRequestContext(accountId))
        return this.GetIdentity(requestContext1.To(TeamFoundationHostType.Application), webToken);
    }

    private Microsoft.VisualStudio.Services.Identity.Identity GetIdentity(
      IVssRequestContext requestContext,
      Guid identityId)
    {
      bool castedValueOrDefault = requestContext.RootContext.Items.GetCastedValueOrDefault<string, bool>(RequestContextItemsKeys.IsProcessingAuthenticationModules);
      if (!castedValueOrDefault)
        requestContext.RootContext.Items[RequestContextItemsKeys.IsProcessingAuthenticationModules] = (object) true;
      Microsoft.VisualStudio.Services.Identity.Identity identity = (Microsoft.VisualStudio.Services.Identity.Identity) null;
      identity = requestContext.GetService<IVssIdentityRetrievalService>().ResolveEligibleActorByDeploymentId(requestContext, identityId);
      if (identity == null && requestContext.IsDeploymentFallbackIdentityReadAllowed())
      {
        identity = requestContext.GetService<IdentityService>().ReadIdentities(requestContext, (IList<Guid>) new Guid[1]
        {
          identityId
        }, QueryMembership.None, (IEnumerable<string>) null).SingleOrDefault<Microsoft.VisualStudio.Services.Identity.Identity>();
        if (identity != null)
          requestContext.TraceDataConditionally(34345434, TraceLevel.Error, "Ssh", nameof (AuthenticationProvider), "Unexpected SSH usage: Could not resolve eligible actor by master ID, but resolved with regular host-level identity read", (Func<object>) (() => (object) new
          {
            identityId = identityId,
            identity = identity
          }), nameof (GetIdentity));
      }
      if (!castedValueOrDefault)
        requestContext.RootContext.Items.Remove(RequestContextItemsKeys.IsProcessingAuthenticationModules);
      return identity != null ? identity : throw new IdentityNotFoundException(identityId);
    }

    internal Microsoft.VisualStudio.Services.Identity.Identity GetIdentity(
      IVssRequestContext requestContext,
      JsonWebToken userJsonWebToken)
    {
      if (userJsonWebToken == null)
        throw new ArgumentNullException(nameof (userJsonWebToken));
      Guid result;
      if (!Guid.TryParse(userJsonWebToken.NameIdentifier, out result))
        throw new FormatException("Name Identifier is not a valid Guid.");
      return this.GetIdentity(requestContext, result);
    }

    internal Microsoft.VisualStudio.Services.Identity.Identity GetIdentity(
      IVssRequestContext requestContext,
      JwtSecurityToken userJsonWebToken)
    {
      Guid result;
      if (!Guid.TryParse(userJsonWebToken.Claims.FirstOrDefault<Claim>((Func<Claim, bool>) (x => x.Type == "nameid"))?.Value, out result))
        throw new FormatException("Name Identifier is not a valid Guid.");
      return this.GetIdentity(requestContext, result);
    }

    private Guid? ReadE2EIdFromToken(JwtSecurityToken webToken)
    {
      Guid result;
      return !Guid.TryParse(webToken.Claims.FirstOrDefault<Claim>((Func<Claim, bool>) (x => x.Type == "X-VSS-E2EID"))?.Value, out result) ? new Guid?() : new Guid?(result);
    }

    private string ReadClientIpFromToken(JwtSecurityToken securityToken) => securityToken.Claims.FirstOrDefault<Claim>((Func<Claim, bool>) (x => x.Type == "client_ip"))?.Value;

    public Microsoft.VisualStudio.Services.Identity.Identity AuthenticatePublicKey(
      IVssRequestContext collectionRequestContext,
      string username,
      ICryptographicKeyPair publicKey)
    {
      collectionRequestContext.TraceEnter(13000340, "Ssh", nameof (AuthenticationProvider), nameof (AuthenticatePublicKey));
      try
      {
        string base64String = Convert.ToBase64String(publicKey.PublicKeyData);
        JsonWebToken webToken = this.AuthenticateDelegatedAuth(collectionRequestContext, base64String, true, username);
        return webToken == null ? (Microsoft.VisualStudio.Services.Identity.Identity) null : this.ReadIdentityFromToken(collectionRequestContext, webToken, username);
      }
      catch (Exception ex)
      {
        collectionRequestContext.TraceException(13000348, "Ssh", nameof (AuthenticationProvider), ex);
        return (Microsoft.VisualStudio.Services.Identity.Identity) null;
      }
      finally
      {
        collectionRequestContext.TraceLeave(13000349, "Ssh", nameof (AuthenticationProvider), nameof (AuthenticatePublicKey));
      }
    }

    internal byte[] GetFrameworkAccessTokenKeySecret(IVssRequestContext requestContext)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment).Elevate();
      ITeamFoundationStrongBoxService service = vssRequestContext.GetService<ITeamFoundationStrongBoxService>();
      Guid drawerId = service.UnlockDrawer(vssRequestContext, "DelegatedAuthorizationSecrets", true);
      return Convert.FromBase64String(service.GetString(vssRequestContext, drawerId, "FrameworkAccessTokenKeySecret"));
    }

    internal string CalculateHmac(
      IVssRequestContext requestContext,
      Guid accountId,
      string token,
      byte[] frameworkAccessTokenKeySecret)
    {
      using (HMACSHA256Hash hmacshA256Hash = new HMACSHA256Hash(string.Format("{0}:{1}", (object) accountId, (object) token), frameworkAccessTokenKeySecret))
        return hmacshA256Hash.HashBase32Encoded;
    }

    internal virtual bool ValidateAccount(
      IVssRequestContext requestContext,
      AuthenticationProvider.HostedAccountSshAuthInfo accountDetails,
      JsonWebToken webToken)
    {
      IList<Guid> list = (IList<Guid>) ((IEnumerable<string>) webToken.Audience.Split('|')).Where<string>((Func<string, bool>) (x => x.StartsWith("vso:"))).Select<string, string>((Func<string, string>) (x => x.Substring("vso:".Length))).Select<string, Guid>((Func<string, Guid>) (x =>
      {
        Guid result = new Guid();
        Guid.TryParse(x, out result);
        return result;
      })).ToList<Guid>();
      if (accountDetails == null)
        return !list.Any<Guid>();
      if (accountDetails.DisallowAuth)
        return false;
      if (requestContext.RequestTracer != null)
        requestContext.TraceSerializedConditionally(13000353, TraceLevel.Info, "Ssh", nameof (AuthenticationProvider), "Validating HostIds for specified account accountDetails = {0}, hostIds = {1}", (object) accountDetails, (object) list);
      return !list.Any<Guid>() || list.Contains(accountDetails.OrganizationId) || list.Contains(accountDetails.CollectionId);
    }

    private JsonWebToken AuthenticateDelegatedAuth(
      IVssRequestContext collectionRequestContext,
      string token,
      bool isPublic,
      string accountName)
    {
      collectionRequestContext.TraceEnter(13000330, "Ssh", nameof (AuthenticationProvider), nameof (AuthenticateDelegatedAuth));
      try
      {
        AuthenticationProvider.HostedAccountSshAuthInfo accountDetails = (AuthenticationProvider.HostedAccountSshAuthInfo) null;
        IVssRequestContext deploymentRequestContext = collectionRequestContext.To(TeamFoundationHostType.Deployment);
        if (collectionRequestContext.ExecutionEnvironment.IsHostedDeployment)
        {
          accountDetails = this.GetHostedAccountSshAuthInfo(collectionRequestContext, accountName);
          if (accountDetails != null && accountDetails.DisallowAuth)
            return (JsonWebToken) null;
        }
        byte[] accessTokenKeySecret = this.GetFrameworkAccessTokenKeySecret(collectionRequestContext);
        AccessTokenResult accessTokenResult;
        if (accountDetails == null)
        {
          collectionRequestContext.Trace(13000345, TraceLevel.Info, "Ssh", nameof (AuthenticationProvider), "Hosted Account is null");
          accessTokenResult = this.ExchangePublicKey(deploymentRequestContext, Guid.Empty, token, isPublic, accessTokenKeySecret);
        }
        else
        {
          try
          {
            collectionRequestContext.Trace(13000346, TraceLevel.Info, "Ssh", nameof (AuthenticationProvider), "Trying to authenticate using collection Id.");
            accessTokenResult = this.ExchangePublicKey(deploymentRequestContext, accountDetails.CollectionId, token, isPublic, accessTokenKeySecret);
          }
          catch (InvalidPublicKeyException ex)
          {
            accessTokenResult = (AccessTokenResult) null;
          }
          if (accessTokenResult == null)
          {
            collectionRequestContext.Trace(13000347, TraceLevel.Info, "Ssh", nameof (AuthenticationProvider), "Trying to authenticate using organization Id.");
            accessTokenResult = this.ExchangePublicKey(deploymentRequestContext, accountDetails.OrganizationId, token, isPublic, accessTokenKeySecret);
            if (accessTokenResult == null)
              return (JsonWebToken) null;
          }
        }
        if (accessTokenResult.HasError)
        {
          collectionRequestContext.Trace(13000337, TraceLevel.Warning, "Ssh", nameof (AuthenticationProvider), "Token exchange failed with error: {0}", (object) accessTokenResult.AccessTokenError);
          return (JsonWebToken) null;
        }
        if (accessTokenResult.AccessToken == null)
          return (JsonWebToken) null;
        if (!collectionRequestContext.ExecutionEnvironment.IsHostedDeployment || this.ValidateAccount(collectionRequestContext, accountDetails, accessTokenResult.AccessToken))
          return accessTokenResult.AccessToken;
        collectionRequestContext.Trace(13000336, TraceLevel.Warning, "Ssh", nameof (AuthenticationProvider), "Token used is not scoped for account {0}", (object) accountName);
        return (JsonWebToken) null;
      }
      catch (Exception ex)
      {
        collectionRequestContext.TraceException(13000338, "Ssh", nameof (AuthenticationProvider), ex);
        return (JsonWebToken) null;
      }
      finally
      {
        collectionRequestContext.TraceLeave(13000339, "Ssh", nameof (AuthenticationProvider), nameof (AuthenticateDelegatedAuth));
      }
    }

    private AccessTokenResult ExchangePublicKey(
      IVssRequestContext deploymentRequestContext,
      Guid hostId,
      string token,
      bool isPublic,
      byte[] frameworkAccessTokenKeySecret)
    {
      IDelegatedAuthorizationService service = deploymentRequestContext.GetService<IDelegatedAuthorizationService>();
      string tokenHash = this.CalculateHmac(deploymentRequestContext, hostId, token, frameworkAccessTokenKeySecret);
      deploymentRequestContext.TraceConditionally(13000337, TraceLevel.Info, "Ssh", nameof (AuthenticationProvider), (Func<string>) (() => string.Format("ExchangePublicKey, hostId: {0}, token: {1}, tokenHash: {2}", (object) hostId, (object) token, (object) tokenHash)));
      IVssRequestContext requestContext = deploymentRequestContext;
      string accessTokenKey = tokenHash;
      int num = isPublic ? 1 : 0;
      return service.Exchange(requestContext, accessTokenKey, num != 0);
    }

    private AuthenticationProvider.HostedAccountSshAuthInfo GetHostedAccountSshAuthInfo(
      IVssRequestContext collectionContext,
      string collectionName)
    {
      IVssRequestContext deploymentContext = collectionContext.To(TeamFoundationHostType.Deployment);
      Guid guid;
      Guid organizationId;
      bool flag;
      if (collectionContext.Items.ContainsKey(RequestContextItemsKeys.HostProxyData))
      {
        HostProxyData var = collectionContext.Items[RequestContextItemsKeys.HostProxyData] as HostProxyData;
        ArgumentUtility.CheckForNull<HostProxyData>(var, "proxyData", "SSH");
        ServiceHostProperties serviceHostProperties = this.GetServiceHostProperties(deploymentContext, var.HostId);
        guid = serviceHostProperties.HostId;
        organizationId = serviceHostProperties.ParentHostId;
        flag = this.GetEffectivePolicyValue(deploymentContext.Elevate(), var.HostId, organizationId);
      }
      else
      {
        guid = collectionContext.ServiceHost.InstanceId;
        organizationId = collectionContext.ServiceHost.OrganizationServiceHost.InstanceId;
        flag = collectionContext.GetService<IOrganizationPolicyService>().GetPolicy<bool>(collectionContext.Elevate(), "Policy.DisallowSecureShell", false).EffectiveValue;
      }
      return new AuthenticationProvider.HostedAccountSshAuthInfo()
      {
        CollectionId = guid,
        OrganizationId = organizationId,
        DisallowAuth = flag
      };
    }

    private ServiceHostProperties GetServiceHostProperties(
      IVssRequestContext deploymentContext,
      Guid hostId)
    {
      if (deploymentContext.IsFeatureEnabled("VisualStudio.Services.Ssh.UseCollectionLevelRouting"))
      {
        string locationServiceUrl = LocationServiceHelper.GetRootLocationServiceUrl(deploymentContext, hostId, forceDevOpsDomainUrls: true);
        using (HostManagementHttpClient client = HttpClientHelper.CreateClient<HostManagementHttpClient>(deploymentContext.Elevate(), new Uri(locationServiceUrl)))
          return client.GetServiceHostPropertiesAsync().SyncResult<ServiceHostProperties>() ?? throw new CollectionNotFoundException("GetServiceHostProperties: Collection-level GetServiceHostPropertiesAsync returned null");
      }
      else
        return deploymentContext.GetService<IHostManagementService>().GetServiceHostProperties(deploymentContext, hostId) ?? throw new CollectionNotFoundException("GetServiceHostProperties: Deployment-level GetServiceHostProperties returned null");
    }

    private bool GetEffectivePolicyValue(
      IVssRequestContext deploymentContext,
      Guid accountId,
      Guid organizationId)
    {
      deploymentContext.CheckDeploymentRequestContext();
      string policyName = "Policy.DisallowSecureShell";
      string enforceKey = OrganizationPolicyService.GetEnforceKey(policyName);
      OrganizationHttpClient organizationClient1 = this.GetOrganizationClient(deploymentContext, organizationId);
      OrganizationHttpClient organizationClient2 = this.GetOrganizationClient(deploymentContext, accountId);
      string[] propertyNames = new string[2]
      {
        policyName,
        enforceKey
      };
      CancellationToken cancellationToken = new CancellationToken();
      Microsoft.VisualStudio.Services.Organization.Client.Organization organization = organizationClient1.GetOrganizationAsync("Me", (IEnumerable<string>) propertyNames, cancellationToken: cancellationToken).SyncResult<Microsoft.VisualStudio.Services.Organization.Client.Organization>() ?? throw new OrganizationNotFoundException("GetEffectivePolicyValue: GetOrganizationAsync returned null");
      Microsoft.VisualStudio.Services.Organization.Client.Collection collection = organizationClient2.GetCollectionAsync("Me", (IEnumerable<string>) new string[2]
      {
        policyName,
        enforceKey
      }).SyncResult<Microsoft.VisualStudio.Services.Organization.Client.Collection>() ?? throw new CollectionNotFoundException("GetEffectivePolicyValue: GetCollectionAsync returned null");
      Policy<bool> policy = OrganizationPolicyService.ConvertToPolicy<bool>(deploymentContext, organization.Properties == null ? (PropertyBag) null : new PropertyBag((IDictionary<string, object>) organization.Properties), policyName, enforceKey, false, (Policy<bool>) null);
      return OrganizationPolicyService.ConvertToPolicy<bool>(deploymentContext, collection.Properties == null ? (PropertyBag) null : new PropertyBag((IDictionary<string, object>) collection.Properties), policyName, enforceKey, false, policy).EffectiveValue;
    }

    private OrganizationHttpClient GetOrganizationClient(
      IVssRequestContext deploymentContext,
      Guid hostId)
    {
      string locationServiceUrl = LocationServiceHelper.GetRootLocationServiceUrl(deploymentContext, hostId);
      if (!(deploymentContext.ClientProvider is ICreateClient clientProvider))
        throw new ArgumentException("Failed to get a client creator");
      IVssRequestContext requestContext = deploymentContext;
      Uri baseUri = new Uri(locationServiceUrl);
      Guid targetServicePrincipal = new Guid();
      return clientProvider.CreateClient<OrganizationHttpClient>(requestContext, baseUri, "SSH", (ApiResourceLocationCollection) null, targetServicePrincipal: targetServicePrincipal);
    }

    public bool IsForwardedAuthSupported(IVssRequestContext requestContext) => requestContext.IsFeatureEnabled("VisualStudio.Services.Ssh.AllowForwardedAuthentication");

    public bool IsPublicKeySupported(IVssRequestContext requestContext) => true;

    internal class HostedAccountSshAuthInfo
    {
      public Guid CollectionId { get; set; }

      public Guid OrganizationId { get; set; }

      public bool DisallowAuth { get; set; }
    }
  }
}
