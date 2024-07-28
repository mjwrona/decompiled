// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Authorization.AzureKeyCredentialAuthorizationTokenProvider
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Azure;
using Microsoft.Azure.Cosmos.Tracing;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Collections;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Cosmos.Authorization
{
  internal class AzureKeyCredentialAuthorizationTokenProvider : AuthorizationTokenProvider
  {
    private readonly object refreshLock = new object();
    private readonly AzureKeyCredential azureKeyCredential;
    private string currentKeyObject;
    internal AuthorizationTokenProvider authorizationTokenProvider;

    public AzureKeyCredentialAuthorizationTokenProvider(AzureKeyCredential azureKeyCredential)
    {
      this.azureKeyCredential = azureKeyCredential ?? throw new ArgumentNullException(nameof (azureKeyCredential));
      this.CheckAndRefreshTokenProvider();
    }

    public override ValueTask AddAuthorizationHeaderAsync(
      INameValueCollection headersCollection,
      Uri requestAddress,
      string verb,
      AuthorizationTokenType tokenType)
    {
      this.CheckAndRefreshTokenProvider();
      return this.authorizationTokenProvider.AddAuthorizationHeaderAsync(headersCollection, requestAddress, verb, tokenType);
    }

    public override void Dispose()
    {
      this.authorizationTokenProvider?.Dispose();
      this.authorizationTokenProvider = (AuthorizationTokenProvider) null;
    }

    public override ValueTask<(string token, string payload)> GetUserAuthorizationAsync(
      string resourceAddress,
      string resourceType,
      string requestVerb,
      INameValueCollection headers,
      AuthorizationTokenType tokenType)
    {
      this.CheckAndRefreshTokenProvider();
      return this.authorizationTokenProvider.GetUserAuthorizationAsync(resourceAddress, resourceType, requestVerb, headers, tokenType);
    }

    public override ValueTask<string> GetUserAuthorizationTokenAsync(
      string resourceAddress,
      string resourceType,
      string requestVerb,
      INameValueCollection headers,
      AuthorizationTokenType tokenType,
      ITrace trace)
    {
      this.CheckAndRefreshTokenProvider();
      return this.authorizationTokenProvider.GetUserAuthorizationTokenAsync(resourceAddress, resourceType, requestVerb, headers, tokenType, trace);
    }

    public override void TraceUnauthorized(
      DocumentClientException dce,
      string authorizationToken,
      string payload)
    {
      this.authorizationTokenProvider.TraceUnauthorized(dce, authorizationToken, payload);
    }

    public override TimeSpan GetAge() => this.authorizationTokenProvider.GetAge();

    private void CheckAndRefreshTokenProvider()
    {
      if ((object) this.currentKeyObject == (object) this.azureKeyCredential.Key)
        return;
      lock (this.refreshLock)
      {
        if ((object) this.currentKeyObject == (object) this.azureKeyCredential.Key)
          return;
        AuthorizationTokenProvider resourceTokenOrAuthKey = AuthorizationTokenProvider.CreateWithResourceTokenOrAuthKey(this.azureKeyCredential.Key);
        AuthorizationTokenProvider authorizationTokenProvider = Interlocked.Exchange<AuthorizationTokenProvider>(ref this.authorizationTokenProvider, resourceTokenOrAuthKey);
        if (resourceTokenOrAuthKey == authorizationTokenProvider)
          return;
        Interlocked.Exchange<string>(ref this.currentKeyObject, this.azureKeyCredential.Key);
      }
    }
  }
}
