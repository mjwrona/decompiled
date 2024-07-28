// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.AuthorizationTokenProvider
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.Tracing;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Collections;
using System;
using System.Threading.Tasks;

namespace Microsoft.Azure.Cosmos
{
  internal abstract class AuthorizationTokenProvider : 
    ICosmosAuthorizationTokenProvider,
    IAuthorizationTokenProvider,
    IDisposable
  {
    private readonly DateTime creationTime = DateTime.UtcNow;

    public async Task AddSystemAuthorizationHeaderAsync(
      DocumentServiceRequest request,
      string federationId,
      string verb,
      string resourceId)
    {
      request.Headers["x-ms-date"] = Rfc1123DateTimeCache.UtcNow();
      INameValueCollection nameValueCollection = request.Headers;
      nameValueCollection["authorization"] = (await this.GetUserAuthorizationAsync(resourceId ?? request.ResourceAddress, PathsHelper.GetResourcePath(request.ResourceType), verb, request.Headers, request.RequestAuthorizationTokenType)).Item1;
      nameValueCollection = (INameValueCollection) null;
    }

    public abstract ValueTask AddAuthorizationHeaderAsync(
      INameValueCollection headersCollection,
      Uri requestAddress,
      string verb,
      AuthorizationTokenType tokenType);

    public abstract ValueTask<(string token, string payload)> GetUserAuthorizationAsync(
      string resourceAddress,
      string resourceType,
      string requestVerb,
      INameValueCollection headers,
      AuthorizationTokenType tokenType);

    public abstract ValueTask<string> GetUserAuthorizationTokenAsync(
      string resourceAddress,
      string resourceType,
      string requestVerb,
      INameValueCollection headers,
      AuthorizationTokenType tokenType,
      ITrace trace);

    public abstract void TraceUnauthorized(
      DocumentClientException dce,
      string authorizationToken,
      string payload);

    public virtual TimeSpan GetAge() => DateTime.UtcNow.Subtract(this.creationTime);

    public static AuthorizationTokenProvider CreateWithResourceTokenOrAuthKey(
      string authKeyOrResourceToken)
    {
      if (string.IsNullOrEmpty(authKeyOrResourceToken))
        throw new ArgumentNullException(nameof (authKeyOrResourceToken));
      return AuthorizationHelper.IsResourceToken(authKeyOrResourceToken) ? (AuthorizationTokenProvider) new AuthorizationTokenProviderResourceToken(authKeyOrResourceToken) : (AuthorizationTokenProvider) new AuthorizationTokenProviderMasterKey(authKeyOrResourceToken);
    }

    public abstract void Dispose();
  }
}
