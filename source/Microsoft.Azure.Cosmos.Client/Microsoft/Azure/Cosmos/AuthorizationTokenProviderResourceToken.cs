// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.AuthorizationTokenProviderResourceToken
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.Core.Trace;
using Microsoft.Azure.Cosmos.Tracing;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Collections;
using System;
using System.Threading.Tasks;

namespace Microsoft.Azure.Cosmos
{
  internal sealed class AuthorizationTokenProviderResourceToken : AuthorizationTokenProvider
  {
    private readonly string urlEncodedAuthKeyResourceToken;
    private readonly ValueTask<string> urlEncodedAuthKeyResourceTokenValueTask;
    private readonly ValueTask<(string, string)> urlEncodedAuthKeyResourceTokenValueTaskWithPayload;
    private readonly ValueTask defaultValueTask;

    public AuthorizationTokenProviderResourceToken(string authKeyResourceToken)
    {
      this.urlEncodedAuthKeyResourceToken = HttpUtility.UrlEncode(authKeyResourceToken);
      this.urlEncodedAuthKeyResourceTokenValueTask = new ValueTask<string>(this.urlEncodedAuthKeyResourceToken);
      this.urlEncodedAuthKeyResourceTokenValueTaskWithPayload = new ValueTask<(string, string)>((this.urlEncodedAuthKeyResourceToken, (string) null));
      this.defaultValueTask = new ValueTask();
    }

    public override ValueTask<(string token, string payload)> GetUserAuthorizationAsync(
      string resourceAddress,
      string resourceType,
      string requestVerb,
      INameValueCollection headers,
      AuthorizationTokenType tokenType)
    {
      return this.urlEncodedAuthKeyResourceTokenValueTaskWithPayload;
    }

    public override ValueTask<string> GetUserAuthorizationTokenAsync(
      string resourceAddress,
      string resourceType,
      string requestVerb,
      INameValueCollection headers,
      AuthorizationTokenType tokenType,
      ITrace trace)
    {
      return this.urlEncodedAuthKeyResourceTokenValueTask;
    }

    public override ValueTask AddAuthorizationHeaderAsync(
      INameValueCollection headersCollection,
      Uri requestAddress,
      string verb,
      AuthorizationTokenType tokenType)
    {
      headersCollection.Add("authorization", this.urlEncodedAuthKeyResourceToken);
      return this.defaultValueTask;
    }

    public override void TraceUnauthorized(
      DocumentClientException dce,
      string authorizationToken,
      string payload)
    {
      DefaultTrace.TraceError("Un-expected authorization for resource token. " + dce.Message);
    }

    public override void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }

    private void Dispose(bool disposing)
    {
    }

    ~AuthorizationTokenProviderResourceToken() => this.Dispose(false);
  }
}
