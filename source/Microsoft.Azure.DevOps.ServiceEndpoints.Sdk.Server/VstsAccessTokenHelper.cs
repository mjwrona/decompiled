// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.VstsAccessTokenHelper
// Assembly: Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 002C83BC-B53E-470A-8038-76E47B5E5BF3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Identity.Client;
using Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server
{
  public static class VstsAccessTokenHelper
  {
    private const string AadOAuthStrongBoxDrawerName = "AadOauthCallbackDrawer";

    public static bool TryGetVstsAccessToken(
      IVssRequestContext requestContext,
      ServiceEndpoint serviceEndpoint,
      out string vstsToken,
      out string idToken,
      out string nonce,
      out string completeCallbackPayload,
      out string errorMessage,
      bool clearCache = true)
    {
      vstsToken = (string) null;
      idToken = (string) null;
      nonce = (string) null;
      completeCallbackPayload = (string) null;
      errorMessage = (string) null;
      if (serviceEndpoint == null || serviceEndpoint.Authorization == null || serviceEndpoint.Authorization.Parameters == null)
      {
        errorMessage = ServiceEndpointSdkResources.ServiceEndpointInvalid();
        return false;
      }
      string str;
      if (serviceEndpoint.Authorization.Parameters.TryGetValue("AccessToken", out str))
      {
        VstsAccessTokenHelper.CheckFlagAndDeleteAuthorizationParameter(serviceEndpoint, "AccessToken", clearCache);
        AccessTokenRequestType result = AccessTokenRequestType.None;
        if (serviceEndpoint.Authorization.Parameters.ContainsKey("AccessTokenFetchingMethod"))
        {
          if (!Enum.TryParse<AccessTokenRequestType>(serviceEndpoint.Authorization.Parameters["AccessTokenFetchingMethod"], out result))
          {
            errorMessage = ServiceEndpointSdkResources.AccessTokenStoreInvalid();
            return false;
          }
          VstsAccessTokenHelper.CheckFlagAndDeleteAuthorizationParameter(serviceEndpoint, "AccessTokenFetchingMethod", clearCache);
        }
        if (result == AccessTokenRequestType.Oauth)
          return VstsAccessTokenHelper.TryGetVstsAccessTokenFromStrongBox(requestContext, str, out vstsToken, out idToken, out nonce, out completeCallbackPayload, out errorMessage, clearCache);
        if (result == AccessTokenRequestType.None)
          return VstsAccessTokenHelper.TryGetVstsAccessTokenFromPropertyCacheWithRetries(requestContext, str, out vstsToken, out idToken, out nonce, out completeCallbackPayload, out errorMessage, clearCache);
        errorMessage = ServiceEndpointSdkResources.AccessTokenStoreInvalid();
        return false;
      }
      errorMessage = ServiceEndpointSdkResources.AccessTokenKeyNotFoundInServiceEndpointAuthorizationParameters();
      return false;
    }

    public static bool TryGetVstsAccessTokenFromStrongBox(
      IVssRequestContext requestContext,
      string strongBoxKey,
      out string vstsToken,
      out string idToken,
      out string nonce,
      out string completeCallbackPayload,
      out string errorMessage,
      bool clearCache = true)
    {
      vstsToken = (string) null;
      idToken = (string) null;
      nonce = (string) null;
      completeCallbackPayload = (string) null;
      errorMessage = (string) null;
      ITeamFoundationStrongBoxService service = requestContext.GetService<ITeamFoundationStrongBoxService>();
      IVssRequestContext requestContext1 = requestContext.Elevate();
      Guid drawerId = service.UnlockDrawer(requestContext1, "AadOauthCallbackDrawer", true);
      string toDeserialize;
      try
      {
        toDeserialize = service.GetString(requestContext1, drawerId, strongBoxKey);
      }
      catch (StrongBoxItemNotFoundException ex)
      {
        errorMessage = ServiceEndpointSdkResources.AzdevAccessTokenStrongBoxLookupKeyIsInvalid();
        requestContext.TraceError(34000205, "AadAuthentication", errorMessage);
        return false;
      }
      if (clearCache)
        service.DeleteItem(requestContext1, drawerId, strongBoxKey);
      IDictionary<string, string> dictionary = (IDictionary<string, string>) null;
      try
      {
        dictionary = JsonUtility.FromString<IDictionary<string, string>>(toDeserialize);
      }
      catch (JsonReaderException ex)
      {
      }
      if (dictionary == null)
      {
        errorMessage = ServiceEndpointSdkResources.VstsAccessTokenStrongBoxLookupValueIsInvalid();
        requestContext.TraceError(34000205, "AadAuthentication", errorMessage);
        return false;
      }
      if (!dictionary.TryGetValue("AccessToken", out vstsToken))
      {
        errorMessage = ServiceEndpointSdkResources.AzdevAccessTokenKeyNotFoundError();
        requestContext.TraceError(34000205, "AadAuthentication", errorMessage);
        return false;
      }
      if (string.IsNullOrEmpty(vstsToken))
      {
        errorMessage = ServiceEndpointSdkResources.AzdevAccessTokenIsNullError();
        requestContext.TraceError(34000205, "AadAuthentication", errorMessage);
        return false;
      }
      dictionary.TryGetValue("IdToken", out idToken);
      dictionary.TryGetValue(nameof (nonce), out nonce);
      dictionary.TryGetValue("CompleteCallbackPayload", out completeCallbackPayload);
      requestContext.TraceAlways(1048671, TraceLevel.Info, "ServiceEndpoints", "AadAuthentication", "Access token is not stored in SPS Property Cache");
      return true;
    }

    public static bool TryGetVstsAccessTokenFromPropertyCache(
      IVssRequestContext requestContext,
      string cacheKey,
      out string vstsToken,
      out string idToken,
      out string nonce,
      out string completeCallbackPayload,
      out string errorMessage,
      bool clearCache = true)
    {
      idToken = (string) null;
      vstsToken = (string) null;
      errorMessage = (string) null;
      nonce = (string) null;
      completeCallbackPayload = (string) null;
      CacheHttpClient propertyCacheHttpClient = (CacheHttpClient) null;
      try
      {
        propertyCacheHttpClient = requestContext.To(TeamFoundationHostType.Deployment).Elevate().GetClient<CacheHttpClient>();
        object o = (object) null;
        try
        {
          o = propertyCacheHttpClient.GetAsync(cacheKey).SyncResult<object>();
        }
        catch (Exception ex)
        {
          requestContext.TraceException(34000206, "AadAuthentication", ex);
        }
        if (o == null)
        {
          errorMessage = ServiceEndpointSdkResources.AzdevAccessTokenCacheKeyLookupResultIsNullError();
          requestContext.TraceError(34000207, "AadAuthentication", errorMessage);
          return false;
        }
        Dictionary<string, object> dictionary = JObject.FromObject(o).ToObject<Dictionary<string, object>>();
        if (dictionary == null)
        {
          errorMessage = ServiceEndpointSdkResources.AzdevAccessTokenCacheKeyLookupResultIsInvalidError();
          requestContext.TraceError(34000207, "AadAuthentication", errorMessage);
          return false;
        }
        if (!dictionary.ContainsKey("AccessToken"))
        {
          errorMessage = ServiceEndpointSdkResources.AzdevAccessTokenKeyNotFoundError();
          requestContext.TraceError(34000207, "AadAuthentication", errorMessage);
          return false;
        }
        object obj1 = dictionary["AccessToken"];
        if (obj1 == null)
        {
          errorMessage = ServiceEndpointSdkResources.AzdevAccessTokenIsNullError();
          requestContext.TraceError(34000207, "AadAuthentication", errorMessage);
          return false;
        }
        vstsToken = obj1.ToString();
        object obj2 = (object) null;
        if (!dictionary.TryGetValue("IdToken", out obj2))
        {
          errorMessage = ServiceEndpointSdkResources.AzdevIdTokenKeyNotFoundError();
          requestContext.TraceWarning(34000208, "AadAuthentication", errorMessage);
        }
        idToken = obj2?.ToString();
        object obj3 = (object) null;
        if (!dictionary.TryGetValue(nameof (nonce), out obj3))
        {
          errorMessage = ServiceEndpointSdkResources.AzdevNonceNotFoundError();
          requestContext.TraceWarning(34000209, "AadAuthentication", errorMessage);
        }
        nonce = obj3?.ToString();
        if (!dictionary.ContainsKey("CompleteCallbackPayload"))
        {
          errorMessage = ServiceEndpointSdkResources.AzdevAccessTokenKeyNotFoundError();
          requestContext.TraceWarning(34000207, "AadAuthentication", errorMessage);
        }
        else
          completeCallbackPayload = dictionary["CompleteCallbackPayload"].ToString();
      }
      finally
      {
        if (clearCache)
          VstsAccessTokenHelper.DeleteCacheEntry(requestContext, propertyCacheHttpClient, cacheKey);
      }
      requestContext.TraceAlways(1048670, TraceLevel.Info, "ServiceEndpoints", "AadAuthentication", "Access token is stored in SPS Property Cache");
      return true;
    }

    private static bool TryGetVstsAccessTokenFromPropertyCacheWithRetries(
      IVssRequestContext requestContext,
      string cacheKey,
      out string vstsToken,
      out string idToken,
      out string nonce,
      out string completeCallbackPayload,
      out string errorMessage,
      bool clearCache = true)
    {
      string cachedVstsToken = (string) null;
      string cachedIdToken = (string) null;
      string cachedNonce = (string) null;
      string cachedCompleteCallbackPayload = (string) null;
      string cachedErrorMessage = (string) null;
      bool cacheCallResult = false;
      try
      {
        new RetryManager(5, TimeSpan.FromSeconds(1.0)).Invoke((Action) (() => cacheCallResult = VstsAccessTokenHelper.TryGetVstsAccessTokenFromPropertyCache(requestContext, cacheKey, out cachedVstsToken, out cachedIdToken, out cachedNonce, out cachedCompleteCallbackPayload, out cachedErrorMessage, false)));
        vstsToken = cachedVstsToken;
        idToken = cachedIdToken;
        nonce = cachedNonce;
        completeCallbackPayload = cachedCompleteCallbackPayload;
        errorMessage = cachedErrorMessage;
        return cacheCallResult;
      }
      finally
      {
        CacheHttpClient client = requestContext.To(TeamFoundationHostType.Deployment).Elevate().GetClient<CacheHttpClient>();
        if (clearCache)
          VstsAccessTokenHelper.DeleteCacheEntry(requestContext, client, cacheKey);
      }
    }

    private static void DeleteCacheEntry(
      IVssRequestContext requestContext,
      CacheHttpClient propertyCacheHttpClient,
      string cacheKey)
    {
      try
      {
        if (propertyCacheHttpClient == null || string.IsNullOrEmpty(cacheKey))
          return;
        propertyCacheHttpClient.DeleteAsync(cacheKey).SyncResult();
      }
      catch (Exception ex)
      {
        requestContext.TraceException(34000210, "AadAuthentication", ex);
      }
    }

    private static void CheckFlagAndDeleteAuthorizationParameter(
      ServiceEndpoint serviceEndpoint,
      string parameterkey,
      bool clearCache)
    {
      if (!clearCache)
        return;
      serviceEndpoint.Authorization.Parameters.Remove(parameterkey);
    }
  }
}
