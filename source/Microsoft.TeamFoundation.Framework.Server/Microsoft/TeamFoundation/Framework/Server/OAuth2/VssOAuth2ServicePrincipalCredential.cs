// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.OAuth2.VssOAuth2ServicePrincipalCredential
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.OAuth;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Jwt;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading;

namespace Microsoft.TeamFoundation.Framework.Server.OAuth2
{
  internal sealed class VssOAuth2ServicePrincipalCredential : FederatedCredential
  {
    private readonly Uri authorizationUrl;

    public VssOAuth2ServicePrincipalCredential(
      Uri authorizationUrl,
      string aadDomain,
      Guid sourceServicePrincipal,
      string targetServicePrincipalName,
      string issuer,
      VssSigningCredentials signingCredentials,
      string token = null,
      bool preAuthenticate = false)
      : base(string.IsNullOrEmpty(token) ? (IssuedToken) null : (IssuedToken) new VssOAuthAccessToken(token))
    {
      ArgumentUtility.CheckForNull<Uri>(authorizationUrl, nameof (authorizationUrl));
      ArgumentUtility.CheckStringForNullOrEmpty(aadDomain, nameof (aadDomain));
      ArgumentUtility.CheckForEmptyGuid(sourceServicePrincipal, nameof (sourceServicePrincipal));
      ArgumentUtility.CheckStringForNullOrEmpty(targetServicePrincipalName, nameof (targetServicePrincipalName));
      ArgumentUtility.CheckStringForNullOrEmpty(issuer, nameof (issuer));
      ArgumentUtility.CheckForNull<VssSigningCredentials>(signingCredentials, nameof (signingCredentials));
      this.authorizationUrl = authorizationUrl;
      this.AADDomain = aadDomain;
      this.SourceServicePrincipal = sourceServicePrincipal;
      this.TargetServicePrincipalName = targetServicePrincipalName;
      this.Issuer = issuer;
      this.SigningCredentials = signingCredentials;
      this.PreAuthenticate = preAuthenticate;
    }

    public override VssCredentialsType CredentialType => VssCredentialsType.S2S;

    public string AADDomain { get; }

    public Guid SourceServicePrincipal { get; }

    public string TargetServicePrincipalName { get; }

    public string Issuer { get; }

    public VssSigningCredentials SigningCredentials { get; }

    public bool PreAuthenticate { get; }

    public override bool IsAuthenticationChallenge(IHttpResponse webResponse) => webResponse != null && (webResponse.StatusCode == HttpStatusCode.Unauthorized || webResponse.StatusCode == HttpStatusCode.Found) && webResponse.Headers.GetValues("WWW-Authenticate").Any<string>((Func<string, bool>) (x => x.StartsWith("Bearer")));

    protected override IssuedTokenProvider OnCreateTokenProvider(
      Uri serverUrl,
      IHttpResponse response)
    {
      string targetService = response != null ? response.Headers.GetValues("X-VSS-S2STargetService").FirstOrDefault<string>() : (string) null;
      VssOAuthTokenParameters tokenParameters = new VssOAuthTokenParameters()
      {
        Resource = !string.IsNullOrEmpty(targetService) ? targetService : this.TargetServicePrincipalName
      };
      VssOAuthJwtBearerClientCredential clientCredential = new VssOAuthJwtBearerClientCredential(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}@{1}", (object) this.SourceServicePrincipal.ToString("D"), (object) this.AADDomain), this.Issuer, this.SigningCredentials);
      VssOAuth2ServicePrincipalCredential.VssOAuthServicePrincipalTokenProvider tokenProvider = new VssOAuth2ServicePrincipalCredential.VssOAuthServicePrincipalTokenProvider((IssuedTokenCredential) this, serverUrl, this.authorizationUrl, (VssOAuthGrant) VssOAuthGrant.ClientCredentials, (VssOAuthClientCredential) clientCredential, tokenParameters, targetService);
      if (this.PreAuthenticate)
        tokenProvider.GetTokenAsync((IssuedToken) null, CancellationToken.None).SyncResult<IssuedToken>();
      return (IssuedTokenProvider) tokenProvider;
    }

    private sealed class VssOAuthServicePrincipalTokenProvider : VssOAuthTokenProvider
    {
      private readonly string m_targetService;

      public VssOAuthServicePrincipalTokenProvider(
        IssuedTokenCredential credential,
        Uri serverUrl,
        Uri authorizationUrl,
        VssOAuthGrant grant,
        VssOAuthClientCredential clientCredential,
        VssOAuthTokenParameters tokenParameters,
        string targetService)
        : base(credential, serverUrl, authorizationUrl, grant, clientCredential, tokenParameters)
      {
        this.m_targetService = targetService;
      }

      protected internal override bool IsAuthenticationChallenge(IHttpResponse response) => base.IsAuthenticationChallenge(response) && string.Equals(this.m_targetService, response.Headers.GetValues("X-VSS-S2STargetService").FirstOrDefault<string>(), StringComparison.OrdinalIgnoreCase);

      protected override IssuedToken CreateIssuedToken(VssOAuthTokenResponse tokenResponse)
      {
        DateTime validTo = tokenResponse.ExpiresIn > 0 ? DateTime.UtcNow.AddSeconds((double) tokenResponse.ExpiresIn) : DateTime.MaxValue;
        return (IssuedToken) new VssOAuth2ServicePrincipalCredential.VssOAuthServicePrincipalAccessToken(tokenResponse.AccessToken, validTo, this.ClientCredential.ClientId, this.TokenParameters.Resource);
      }
    }

    public sealed class VssOAuthServicePrincipalAccessToken : ExpiringIssuedToken
    {
      private readonly string m_value;
      private readonly DateTime m_validTo;
      private readonly string m_delegatedIssuer;
      private readonly string m_resource;

      public VssOAuthServicePrincipalAccessToken(
        string value,
        DateTime validTo,
        string delegatedIssuer,
        string resource)
      {
        ArgumentUtility.CheckStringForNullOrEmpty(value, nameof (value));
        this.m_value = value;
        this.m_validTo = validTo;
        this.m_delegatedIssuer = delegatedIssuer;
        this.m_resource = resource;
      }

      public override DateTime ValidTo => this.m_validTo;

      public override string Value => this.m_value;

      protected internal override VssCredentialsType CredentialType => VssCredentialsType.OAuth;

      internal override void ApplyTo(IHttpRequest request)
      {
        string encodedToken = this.Value;
        ClaimsIdentity claimsIdentity;
        if (request.Properties.TryGetValue<ClaimsIdentity>(TfsApiPropertyKeys.DelegatedUser, out claimsIdentity))
          encodedToken = JsonWebToken.Create(this.m_delegatedIssuer, this.m_resource, DateTime.UtcNow, DateTime.UtcNow.AddMinutes(5.0), (IEnumerable<Claim>) claimsIdentity.Claims.ToDedupedDictionary<Claim, string, Claim>((Func<Claim, string>) (x => x.Type), (Func<Claim, Claim>) (x => x), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase).Values, encodedToken).EncodedToken;
        request.Headers.SetValue("Authorization", "Bearer " + encodedToken);
      }
    }
  }
}
