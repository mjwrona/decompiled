// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.MsalAzureAccessTokenHelper
// Assembly: Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 002C83BC-B53E-470A-8038-76E47B5E5BF3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.dll

using Microsoft.Identity.Client;
using Microsoft.TeamFoundation;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.Authentication;
using Microsoft.VisualStudio.Services.Cloud.AzureActiveDirectory.Tokens;
using Microsoft.VisualStudio.Services.CloudConfiguration;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.Identity.Client;
using Microsoft.VisualStudio.Services.Location;
using Microsoft.VisualStudio.Services.Location.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography.X509Certificates;

namespace Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server
{
  public static class MsalAzureAccessTokenHelper
  {
    private static X509Certificate2 s_aadCertificate;
    private static IConfidentialClientApplication s_confidentialClientApplication;
    private const string c_layer = "AzureAccessTokenHelperSdk";
    private const string c_area = "DistributedTask";
    private const string c_vstsResourceId = "499b84ac-1321-427f-aa17-267ca6975798";

    public static string GetArmAccessToken(
      IVssRequestContext requestContext,
      string tenantId,
      string vstsToken = null)
    {
      return MsalAzureAccessTokenHelper.GetResourceAccessToken(requestContext, tenantId, (string) null, vstsToken);
    }

    internal static X509Certificate2 GetCertificate(IVssRequestContext context)
    {
      IVssRequestContext vssRequestContext = context.To(TeamFoundationHostType.Deployment);
      ITeamFoundationStrongBoxService service = vssRequestContext.GetService<ITeamFoundationStrongBoxService>();
      string str = vssRequestContext.GetService<IVssRegistryService>().GetValue(vssRequestContext, (RegistryQuery) OAuth2RegistryConstants.AADCertThumbprint, string.Empty);
      X509Certificate2 certificate = MsalAzureAccessTokenHelper.s_aadCertificate;
      if (!string.Equals(certificate?.Thumbprint, str, StringComparison.OrdinalIgnoreCase))
      {
        Guid drawerId = service.UnlockDrawer(vssRequestContext, OAuth2RegistryConstants.S2SSigningCertDrawerName, true);
        certificate = service.RetrieveFileAsCertificate(vssRequestContext, drawerId, str);
        MsalAzureAccessTokenHelper.s_aadCertificate = certificate;
      }
      return certificate;
    }

    internal static string GetResourceAccessToken(
      IVssRequestContext requestContext,
      string tenantId,
      string resource,
      string vstsToken)
    {
      requestContext.TraceEnter(0, "AzureAccessTokenHelperSdk", nameof (GetResourceAccessToken));
      try
      {
        if (AzureAccessTokenProvider.UseProdAzureResourcesOnDevFabric(requestContext))
          return AzureAccessTokenProvider.GetAccessToken(resource);
        return string.IsNullOrEmpty(vstsToken) ? MsalAzureAccessTokenHelper.GetAccessTokenUsingRefreshToken(requestContext, tenantId, resource) : MsalAzureAccessTokenHelper.GetAccessTokenUsingClientAssertion(requestContext, tenantId, resource, vstsToken);
      }
      finally
      {
        requestContext.TraceLeave(0, "AzureAccessTokenHelperSdk", nameof (GetResourceAccessToken));
      }
    }

    private static string GetAccessTokenUsingRefreshToken(
      IVssRequestContext requestContext,
      string tenantId,
      string resource)
    {
      requestContext.TraceEnter(0, "AzureAccessTokenHelperSdk", nameof (GetAccessTokenUsingRefreshToken));
      try
      {
        IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment).Elevate();
        IAadTokenService service = vssRequestContext.GetService<IAadTokenService>();
        if (string.IsNullOrWhiteSpace(resource))
        {
          requestContext.TraceInfo(34000201, "AzureAccessTokenHelperSdk", "Resource requested is NullOrEmpty. Using default resource: {0}", (object) service.DefaultResource);
          resource = service.DefaultResource;
        }
        JwtSecurityToken jwtSecurityToken = service.AcquireToken(vssRequestContext, resource, tenantId, requestContext.UserContext);
        requestContext.TraceAlways(34000223, TraceLevel.Info, "DistributedTask", "AzureAccessTokenHelperSdk", "AdalToMsal: Acquired the user access token. Resource: {0}, TenantId: {1}", (object) resource, (object) tenantId);
        return jwtSecurityToken.RawData;
      }
      catch (Exception ex)
      {
        requestContext.TraceError(34000224, "DistributedTask", "AzureAccessTokenHelperSdk", (object) ("AdalToMsal: Failed to acquire access token using refresh token: " + ex.ToStringDemystified()));
        throw;
      }
      finally
      {
        requestContext.TraceLeave(0, "AzureAccessTokenHelperSdk", nameof (GetAccessTokenUsingRefreshToken));
      }
    }

    private static IConfidentialClientApplication GetConfidentialClientApplication(
      IVssRequestContext requestContext)
    {
      X509Certificate2 certificate = MsalAzureAccessTokenHelper.GetCertificate(requestContext.Elevate());
      IConfidentialClientApplication confidentialClientApplication = MsalAzureAccessTokenHelper.s_confidentialClientApplication;
      if ((confidentialClientApplication != null ? confidentialClientApplication.GetCertificate() : (X509Certificate2) null) != certificate)
      {
        ConfidentialClientApplicationBuilder applicationBuilder = ConfidentialClientApplicationBuilder.Create("499b84ac-1321-427f-aa17-267ca6975798").WithCertificate(certificate, true).WithAzureRegion();
        if (requestContext.ExecutionEnvironment.IsDevFabricDeployment && !AzureAccessTokenProvider.UseProdAzureResourcesOnDevFabric(requestContext))
          applicationBuilder = applicationBuilder.WithAuthority("https://login.windows-ppe.net/common");
        confidentialClientApplication = applicationBuilder.Build();
        MsalAzureAccessTokenHelper.s_confidentialClientApplication = confidentialClientApplication;
      }
      return confidentialClientApplication;
    }

    public static string GetAccessTokenUsingClientAssertion(
      IVssRequestContext requestContext,
      string tenantId,
      string resource,
      string vstsToken)
    {
      requestContext.TraceEnter(0, "AzureAccessTokenHelperSdk", nameof (GetAccessTokenUsingClientAssertion));
      if (string.IsNullOrEmpty(resource))
        resource = "https://management.core.windows.net/";
      string authorityUri = (requestContext.ExecutionEnvironment.IsDevFabricDeployment ? "https://login.windows-ppe.net/" : "https://login.microsoftonline.com/") + tenantId;
      try
      {
        IConfidentialClientApplication clientApplication = MsalAzureAccessTokenHelper.GetConfidentialClientApplication(requestContext);
        UserAssertion userAssertion1 = new UserAssertion(vstsToken, "urn:ietf:params:oauth:grant-type:jwt-bearer");
        string[] scopes = MsalUtility.GetScopes(resource);
        UserAssertion userAssertion2 = userAssertion1;
        AuthenticationResult authenticationResult = clientApplication.AcquireTokenOnBehalfOf((IEnumerable<string>) scopes, userAssertion2).WithAuthority(authorityUri, false).ExecuteAsync().SyncResultConfigured<AuthenticationResult>();
        AuthenticationResultMetadata authenticationResultMetadata = authenticationResult.AuthenticationResultMetadata;
        IVssRequestContext requestContext1 = requestContext;
        string[] strArray = new string[8]
        {
          string.Format("AdalToMsal: Acquired access token on behalf of user. TokenSource: {0}, Total duration: {1} ms, Http duration: {2} ms, ", (object) authenticationResultMetadata.TokenSource, (object) authenticationResultMetadata.DurationTotalInMs, (object) authenticationResultMetadata.DurationInHttpInMs),
          "TokenEndpoint: '",
          authenticationResultMetadata.TokenEndpoint,
          "', Region: '",
          authenticationResultMetadata.RegionDetails.RegionUsed,
          "', ",
          string.IsNullOrEmpty(authenticationResultMetadata.RegionDetails.AutoDetectionError) ? string.Empty : "AutoDetectionError: '" + authenticationResultMetadata.RegionDetails.AutoDetectionError + "', ",
          null
        };
        DateTimeOffset dateTimeOffset = authenticationResult.ExpiresOn;
        dateTimeOffset = dateTimeOffset.ToUniversalTime();
        strArray[7] = string.Format("Token expiration time: {0:s}", (object) dateTimeOffset.DateTime);
        string format = string.Concat(strArray);
        object[] objArray = Array.Empty<object>();
        requestContext1.TraceAlways(34000222, TraceLevel.Info, "DistributedTask", "AzureAccessTokenHelperSdk", format, objArray);
        return authenticationResult.AccessToken;
      }
      catch (Exception ex)
      {
        requestContext.TraceError(34000221, "AzureAccessTokenHelperSdk", "AdalToMsal: Failed to acquire access token on behalf of user: " + ex.ToStringDemystified());
        throw;
      }
      finally
      {
        requestContext.TraceLeave(0, "AzureAccessTokenHelperSdk", nameof (GetAccessTokenUsingClientAssertion));
      }
    }

    public static string GetAadOAuthRequestUrl(
      IVssRequestContext requestContext,
      string tenantId,
      string redirectUri,
      AadAuthUrlUtility.PromptOption promptOption = AadAuthUrlUtility.PromptOption.SelectAccount,
      string completeCallbackPayload = null,
      bool completeCallbackByAuthCode = false)
    {
      requestContext.TraceEnter(0, "AzureAccessTokenHelperSdk", nameof (GetAadOAuthRequestUrl));
      if (string.IsNullOrEmpty(redirectUri) || !Uri.IsWellFormedUriString(redirectUri, UriKind.Absolute))
      {
        requestContext.TraceError(34000211, "AzureAccessTokenHelperSdk", ServiceEndpointSdkResources.OAuthRedirectUrlIsInvalidError((object) redirectUri));
        throw new ArgumentException(ServiceEndpointSdkResources.OAuthRedirectUrlIsInvalidError((object) redirectUri));
      }
      if (string.IsNullOrEmpty(tenantId))
      {
        Guid identityTenantId = AadIdentityHelper.GetIdentityTenantId(requestContext.UserContext);
        tenantId = !(identityTenantId == Guid.Empty) ? identityTenantId.ToString() : "common";
      }
      try
      {
        IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment).Elevate();
        string str1 = new Uri(vssRequestContext.GetService<ILocationService>().GetLocationServiceUrl(vssRequestContext, ServiceInstanceTypes.SPS, AccessMappingConstants.ClientAccessMappingMoniker))?.ToString() + "oauth2/receive";
        string str2;
        if (completeCallbackByAuthCode)
          str2 = JsonUtility.ToString((object) new Dictionary<string, string>()
          {
            ["RedirectUri"] = redirectUri
          });
        else
          str2 = vssRequestContext.GetClient<CacheHttpClient>().CacheAsync((object) new Dictionary<string, string>()
          {
            [PropertyCacheServiceConstants.RedirectUri] = redirectUri
          }).SyncResultConfigured<string>();
        Dictionary<string, string> dictionary = new Dictionary<string, string>()
        {
          ["ReplyToUri"] = str1,
          ["TenantId"] = tenantId,
          ["Resource"] = "499b84ac-1321-427f-aa17-267ca6975798",
          ["State"] = str2,
          ["CompleteCallbackPayload"] = completeCallbackPayload ?? string.Empty
        };
        return new AadAuthUrlUtility.AuthUrlBuilder()
        {
          Resource = "499b84ac-1321-427f-aa17-267ca6975798",
          Tenant = tenantId,
          RedirectLocation = str1,
          QueryString = ((IDictionary<string, string>) dictionary),
          PromptOption = promptOption
        }.Build(requestContext);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(34000211, "AzureAccessTokenHelperSdk", ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(0, "AzureAccessTokenHelperSdk", nameof (GetAadOAuthRequestUrl));
      }
    }

    private static class AadOAuthUrlQueryConstants
    {
      public const string ReplyToUri = "ReplyToUri";
      public const string TenantId = "TenantId";
      public const string Resource = "Resource";
      public const string State = "State";
      public const string CompleteCallbackPayload = "CompleteCallbackPayload";
      public const string RedirectUri = "RedirectUri";
    }
  }
}
