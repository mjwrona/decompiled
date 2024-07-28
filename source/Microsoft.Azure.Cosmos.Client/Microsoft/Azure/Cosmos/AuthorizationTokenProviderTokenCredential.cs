// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.AuthorizationTokenProviderTokenCredential
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Azure.Core;
using Microsoft.Azure.Cosmos.Core.Trace;
using Microsoft.Azure.Cosmos.Tracing;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Collections;
using System;
using System.Globalization;
using System.Threading.Tasks;


#nullable enable
namespace Microsoft.Azure.Cosmos
{
  internal sealed class AuthorizationTokenProviderTokenCredential : AuthorizationTokenProvider
  {
    internal readonly 
    #nullable disable
    TokenCredentialCache tokenCredentialCache;
    private bool isDisposed;

    public AuthorizationTokenProviderTokenCredential(
      TokenCredential tokenCredential,
      Uri accountEndpoint,
      TimeSpan? backgroundTokenCredentialRefreshInterval)
    {
      this.tokenCredentialCache = new TokenCredentialCache(tokenCredential, accountEndpoint, backgroundTokenCredentialRefreshInterval);
    }

    public override async ValueTask<(string token, string payload)> GetUserAuthorizationAsync(
      string resourceAddress,
      string resourceType,
      string requestVerb,
      INameValueCollection headers,
      AuthorizationTokenType tokenType)
    {
      (string, string) authorizationAsync;
      using (Microsoft.Azure.Cosmos.Tracing.Trace trace = Microsoft.Azure.Cosmos.Tracing.Trace.GetRootTrace("GetUserAuthorizationTokenAsync", TraceComponent.Authorization, TraceLevel.Info))
        authorizationAsync = (AuthorizationTokenProviderTokenCredential.GenerateAadAuthorizationSignature(await this.tokenCredentialCache.GetTokenAsync((ITrace) trace)), (string) null);
      return authorizationAsync;
    }

    public override async ValueTask<string> GetUserAuthorizationTokenAsync(
      string resourceAddress,
      string resourceType,
      string requestVerb,
      INameValueCollection headers,
      AuthorizationTokenType tokenType,
      ITrace trace)
    {
      return AuthorizationTokenProviderTokenCredential.GenerateAadAuthorizationSignature(await this.tokenCredentialCache.GetTokenAsync(trace));
    }

    public override async ValueTask AddAuthorizationHeaderAsync(
      INameValueCollection headersCollection,
      Uri requestAddress,
      string verb,
      AuthorizationTokenType tokenType)
    {
      using (Microsoft.Azure.Cosmos.Tracing.Trace trace = Microsoft.Azure.Cosmos.Tracing.Trace.GetRootTrace("GetUserAuthorizationTokenAsync", TraceComponent.Authorization, TraceLevel.Info))
        headersCollection.Add("authorization", AuthorizationTokenProviderTokenCredential.GenerateAadAuthorizationSignature(await this.tokenCredentialCache.GetTokenAsync((ITrace) trace)));
    }

    public override void TraceUnauthorized(
      DocumentClientException dce,
      string authorizationToken,
      string payload)
    {
      DefaultTrace.TraceError("Un-expected authorization for token credential. " + dce.Message);
    }

    public static string GenerateAadAuthorizationSignature(string aadToken) => HttpUtility.UrlEncode(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "type={0}&ver={1}&sig={2}", (object) "aad", (object) "1.0", (object) aadToken));

    public override void Dispose()
    {
      if (this.isDisposed)
        return;
      this.isDisposed = true;
      this.tokenCredentialCache.Dispose();
    }
  }
}
