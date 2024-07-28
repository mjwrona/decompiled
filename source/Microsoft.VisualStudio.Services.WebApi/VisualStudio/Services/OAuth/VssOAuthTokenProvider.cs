// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.OAuth.VssOAuthTokenProvider
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Common.Diagnostics;
using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.OAuth
{
  public class VssOAuthTokenProvider : IssuedTokenProvider
  {
    private readonly VssOAuthGrant m_grant;
    private readonly VssOAuthCredential m_credential;
    private readonly VssOAuthTokenParameters m_tokenParameters;
    private readonly VssOAuthClientCredential m_clientCredential;
    private static readonly Lazy<JsonSerializerSettings> s_traceSettings = new Lazy<JsonSerializerSettings>(new Func<JsonSerializerSettings>(VssOAuthTokenProvider.CreateTraceSettings));

    public VssOAuthTokenProvider(VssOAuthCredential credential, Uri serverUrl)
      : this((IssuedTokenCredential) credential, serverUrl, credential.AuthorizationUrl, credential.Grant, credential.ClientCredential, credential.TokenParameters)
    {
      this.m_credential = credential;
    }

    protected VssOAuthTokenProvider(
      IssuedTokenCredential credential,
      Uri serverUrl,
      Uri authorizationUrl,
      VssOAuthGrant grant,
      VssOAuthClientCredential clientCrential,
      VssOAuthTokenParameters tokenParameters)
      : base(credential, serverUrl, authorizationUrl)
    {
      this.m_grant = grant;
      this.m_tokenParameters = tokenParameters;
      this.m_clientCredential = clientCrential;
    }

    public VssOAuthGrant Grant => this.m_grant;

    public VssOAuthClientCredential ClientCredential => this.m_clientCredential;

    public VssOAuthTokenParameters TokenParameters => this.m_tokenParameters;

    public override bool GetTokenIsInteractive => false;

    protected override string AuthenticationParameter => this.ClientCredential == null ? (string) null : string.Format((IFormatProvider) CultureInfo.InvariantCulture, "client_id=\"{0}\" audience=\"{1}\"", (object) this.ClientCredential.ClientId, (object) this.SignInUrl.AbsoluteUri);

    protected override string AuthenticationScheme => "Bearer";

    protected override async Task<IssuedToken> OnGetTokenAsync(
      IssuedToken failedToken,
      CancellationToken cancellationToken)
    {
      VssOAuthTokenProvider provider = this;
      if (provider.SignInUrl == (Uri) null || provider.Grant == null || provider.ClientCredential == null)
        return (IssuedToken) null;
      IssuedToken issuedToken = (IssuedToken) null;
      VssTraceActivity traceActivity = VssTraceActivity.Current;
      try
      {
        VssOAuthTokenResponse tokenResponse = await new VssOAuthTokenHttpClient(provider.SignInUrl).GetTokenAsync(provider.Grant, provider.ClientCredential, provider.TokenParameters, cancellationToken).ConfigureAwait(false);
        if (!string.IsNullOrEmpty(tokenResponse.AccessToken))
        {
          issuedToken = provider.CreateIssuedToken(tokenResponse);
          if (string.IsNullOrEmpty(tokenResponse.RefreshToken))
            ;
        }
        else
        {
          if (!string.IsNullOrEmpty(tokenResponse.Error))
          {
            string message = tokenResponse.ErrorDescription;
            if (string.IsNullOrEmpty(message))
              message = "Error '" + tokenResponse.Error + "' was reported";
            throw new VssOAuthTokenRequestException(message)
            {
              Error = tokenResponse.Error
            };
          }
          StringBuilder sb = new StringBuilder();
          using (StringWriter stringWriter = new StringWriter(sb))
            JsonSerializer.Create(VssOAuthTokenProvider.s_traceSettings.Value).Serialize((TextWriter) stringWriter, (object) tokenResponse);
          VssHttpEventSource.Log.AuthenticationError(traceActivity, (IssuedTokenProvider) provider, sb.ToString());
        }
      }
      catch (VssServiceResponseException ex)
      {
        VssHttpEventSource.Log.AuthenticationError(traceActivity, (IssuedTokenProvider) provider, (Exception) ex);
      }
      return issuedToken;
    }

    protected virtual IssuedToken CreateIssuedToken(VssOAuthTokenResponse tokenResponse) => tokenResponse.ExpiresIn > 0 ? (IssuedToken) new VssOAuthAccessToken(tokenResponse.AccessToken, DateTime.UtcNow.AddSeconds((double) tokenResponse.ExpiresIn)) : (IssuedToken) new VssOAuthAccessToken(tokenResponse.AccessToken);

    private static JsonSerializerSettings CreateTraceSettings()
    {
      JsonSerializerSettings serializerSettings = new VssJsonMediaTypeFormatter().SerializerSettings;
      serializerSettings.Formatting = Formatting.Indented;
      return serializerSettings;
    }
  }
}
