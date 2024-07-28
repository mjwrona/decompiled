// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.ExternalIntegration.Utilities.GitHubSecretManagementHelper
// Assembly: Microsoft.TeamFoundation.ExternalIntegration.Utilities, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6309B6D0-0EEE-4299-AA79-F0B62882E0B1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.ExternalIntegration.Utilities.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.ExternalProviders.GitHub.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Jwt;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.TeamFoundation.ExternalIntegration.Utilities
{
  public class GitHubSecretManagementHelper
  {
    public static bool IsCredentialError(string errorId) => StringComparer.OrdinalIgnoreCase.Equals("incorrect_client_credentials", errorId);

    public static (string ClientId, string ClientSecret) GetApplicationSecretsForGitHub(
      IVssRequestContext requestContext,
      string usePrimaryCrendentialRegistryPath,
      string primarySecretKeyName,
      string secondarySecretKeyName)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      return vssRequestContext.GetService<ICachedRegistryService>().GetValue<bool>(vssRequestContext, (RegistryQuery) usePrimaryCrendentialRegistryPath, true) ? GitHubSecretManagementHelper.GetApplicationSecretsForGitHub(requestContext, primarySecretKeyName) : GitHubSecretManagementHelper.GetApplicationSecretsForGitHub(requestContext, secondarySecretKeyName);
    }

    public static (string ClientId, string ClientSecret) GetApplicationSecretsForGitHub(
      IVssRequestContext requestContext,
      string lookupKey)
    {
      try
      {
        IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment).Elevate();
        ITeamFoundationStrongBoxService service = vssRequestContext.GetService<ITeamFoundationStrongBoxService>();
        StrongBoxItemInfo itemInfo = service.GetItemInfo(vssRequestContext, "ConfigurationSecrets", lookupKey, true);
        if (string.IsNullOrWhiteSpace(itemInfo.CredentialName))
          throw new InvalidGitHubSecretsException("Invalid GitHub secrets retrieved - clientId is null or empty");
        string str = service.GetString(vssRequestContext, itemInfo);
        if (string.IsNullOrWhiteSpace(str))
          throw new InvalidGitHubSecretsException("Invalid GitHub secrets retrieved - clientSecret is null or empty");
        return (itemInfo.CredentialName, str);
      }
      catch (StrongBoxDrawerNotFoundException ex)
      {
        throw new InvalidGitHubSecretsException("This deployment is not registered with a GitHub account. ", (Exception) ex);
      }
      catch (StrongBoxItemNotFoundException ex)
      {
        throw new InvalidGitHubSecretsException("This deployment is not properly configured with a registered GitHub account. ", (Exception) ex);
      }
    }

    public static void ToggleUsePrimaryGitHubCredentials(
      IVssRequestContext requestContext,
      string usePrimaryCredentialRegistryPath)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      CachedRegistryService service = vssRequestContext.GetService<CachedRegistryService>();
      bool flag = service.GetValue<bool>(vssRequestContext, (RegistryQuery) usePrimaryCredentialRegistryPath, true);
      service.SetValue<bool>(vssRequestContext, usePrimaryCredentialRegistryPath, !flag);
    }

    public static void SaveUserAuthorizationToken(
      IVssRequestContext requestContext,
      string strongBoxDrawerName,
      string strongBoxKey,
      string secret)
    {
      IVssRequestContext vssRequestContext = requestContext.Elevate();
      ITeamFoundationStrongBoxService service = vssRequestContext.GetService<ITeamFoundationStrongBoxService>();
      Guid drawerId = service.UnlockDrawer(vssRequestContext, strongBoxDrawerName, false);
      if (drawerId == Guid.Empty)
        drawerId = service.CreateDrawer(vssRequestContext, strongBoxDrawerName);
      service.AddString(vssRequestContext, drawerId, strongBoxKey, secret);
    }

    public static string GetJwtSignature(
      IVssRequestContext requestContext,
      string content,
      string claimType = null,
      string issuer = null)
    {
      string generateStrongBoxSecret = requestContext.Elevate().GetOrGenerateStrongBoxSecret(GitHubConstants.StrongBoxKey.GitHubInstallationTokenSignatureSecret, GitHubConstants.StrongBoxKey.GitHubInstallationTokenSignatureSecretLookupKey);
      return !string.IsNullOrEmpty(claimType) ? content.ToJwtToken(claimType, Encoding.UTF8.GetBytes(generateStrongBoxSecret + content), issuer) : content.ToJwtToken(Encoding.UTF8.GetBytes(generateStrongBoxSecret + content));
    }

    public static void ValidateJwtSignature(
      IVssRequestContext requestContext,
      string signature,
      string content)
    {
      string signatureSecretLookupKey = GitHubConstants.StrongBoxKey.GitHubInstallationTokenSignatureSecretLookupKey;
      byte[] bytes = Encoding.UTF8.GetBytes(GitHubSecretManagementHelper.GetStrongBoxSecret(requestContext.Elevate(), GitHubConstants.StrongBoxKey.GitHubInstallationTokenSignatureSecret, signatureSecretLookupKey) + content);
      GitHubSecretManagementHelper.ValidateInstallationIdJwtToken(signature, bytes, requestContext.GetUserId().ToString());
    }

    private static void ValidateInstallationIdJwtToken(string token, byte[] secret, string issuer)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(token, nameof (token));
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) secret, nameof (secret));
      JsonWebToken.Create(token).ValidateToken(new JsonWebTokenValidationParameters()
      {
        ValidateActor = false,
        ValidateAudience = true,
        AllowedAudiences = (IEnumerable<string>) new string[1]
        {
          "vsts"
        },
        ValidateExpiration = false,
        ValidateIssuer = true,
        ValidIssuers = (IEnumerable<string>) new string[1]
        {
          issuer
        },
        ValidateNotBefore = false,
        ValidateSignature = true,
        SigningCredentials = VssSigningCredentials.Create(secret)
      });
    }

    private static string GetStrongBoxSecret(
      IVssRequestContext elevatedRequestContext,
      string strongBoxDrawerName,
      string strongBoxKey)
    {
      ITeamFoundationStrongBoxService service = elevatedRequestContext.GetService<ITeamFoundationStrongBoxService>();
      Guid drawerId = service.UnlockDrawer(elevatedRequestContext, strongBoxDrawerName, true);
      return service.GetString(elevatedRequestContext, drawerId, strongBoxKey);
    }
  }
}
