// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.ExternalProviders.Common.BitbucketApiHelper
// Assembly: Microsoft.VisualStudio.ExternalProviders.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7E34B318-B0E9-49BD-88C0-4A425E8D0753
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.ExternalProviders.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.ExternalProviders.Bitbucket.WebApi;
using Microsoft.VisualStudio.Services.Common;
using System.Diagnostics;
using System.Net;

namespace Microsoft.VisualStudio.ExternalProviders.Common
{
  public static class BitbucketApiHelper
  {
    private const string c_area = "ExternalProviders";
    private const string c_layer = "BitbucketApiHelper";

    public static BitbucketResult<BitbucketData.Authorization> RefreshAccessToken(
      IVssRequestContext requestContext,
      string clientId,
      string clientSecret,
      string refreshToken)
    {
      ArgumentUtility.CheckForNull<string>(clientId, nameof (clientId));
      ArgumentUtility.CheckForNull<string>(clientSecret, nameof (clientSecret));
      ArgumentUtility.CheckForNull<string>(refreshToken, nameof (refreshToken));
      return BitbucketApiHelper.RefreshAccessTokenInternal(requestContext, clientId, clientSecret, refreshToken, new BitbucketApiHelper.GetAuthorizationFromToken(BitbucketHttpClientFactory.Create(requestContext).GetAuthorizationFromToken));
    }

    internal static BitbucketResult<BitbucketData.Authorization> RefreshAccessTokenInternal(
      IVssRequestContext requestContext,
      string clientId,
      string clientSecret,
      string refreshToken,
      BitbucketApiHelper.GetAuthorizationFromToken getAuthorizationFromToken)
    {
      if (!requestContext.IsFeatureEnabled(BitbucketFeatureFlags.CacheAccessTokens))
        return getAuthorizationFromToken(clientId, clientSecret, refreshToken);
      IBitbucketAccessTokenCacheService service = requestContext.GetService<IBitbucketAccessTokenCacheService>();
      BitbucketData.Authorization authorization;
      if (service.TryGetValue(refreshToken, out authorization))
        return BitbucketResult<BitbucketData.Authorization>.Success(authorization, HttpStatusCode.OK);
      BitbucketResult<BitbucketData.Authorization> bitbucketResult = getAuthorizationFromToken(clientId, clientSecret, refreshToken);
      if (bitbucketResult.IsSuccessful)
      {
        if (!service.AddValue(refreshToken, bitbucketResult.Result))
          requestContext.Trace(0, TraceLevel.Error, "ExternalProviders", nameof (BitbucketApiHelper), "Could not cache Bitbucket access token");
      }
      else
        requestContext.Trace(0, TraceLevel.Error, "ExternalProviders", nameof (BitbucketApiHelper), "Could not get auth data from Bitbucket by refreshToken");
      return bitbucketResult;
    }

    internal delegate BitbucketResult<BitbucketData.Authorization> GetAuthorizationFromToken(
      string clientId,
      string clientSecret,
      string refreshToken);
  }
}
