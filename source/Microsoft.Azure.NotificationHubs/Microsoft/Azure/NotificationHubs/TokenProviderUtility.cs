// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.TokenProviderUtility
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using Microsoft.Azure.NotificationHubs.Common;
using Microsoft.Azure.NotificationHubs.Messaging;
using System;
using System.IdentityModel.Tokens;

namespace Microsoft.Azure.NotificationHubs
{
  internal static class TokenProviderUtility
  {
    private static readonly TimeSpan MaxGetTokenSyncTimeout = TimeSpan.FromSeconds(15.15);

    internal static IAsyncResult BeginGetMessagingToken(
      this TokenProvider tokenProvider,
      Uri namespaceAddress,
      string appliesTo,
      string action,
      bool bypassCache,
      TimeSpan timeout,
      AsyncCallback callback,
      object state)
    {
      try
      {
        return tokenProvider.BeginGetToken(namespaceAddress, appliesTo, action, bypassCache, timeout, callback, state);
      }
      catch (Exception ex)
      {
        Exception exception;
        ref Exception local = ref exception;
        if (TokenProviderUtility.HandleTokenException(ex, out local))
          throw exception;
        throw;
      }
    }

    internal static SecurityToken EndGetMessagingToken(
      this TokenProvider tokenProvider,
      IAsyncResult result)
    {
      try
      {
        return tokenProvider.EndGetToken(result);
      }
      catch (Exception ex)
      {
        Exception exception;
        ref Exception local = ref exception;
        if (TokenProviderUtility.HandleTokenException(ex, out local))
          throw exception;
        throw;
      }
    }

    private static bool HandleTokenException(Exception exception, out Exception outException)
    {
      outException = (Exception) null;
      if (exception is TimeoutException || Fx.IsFatal(exception))
        return false;
      outException = !(exception is TokenProviderException) ? (Exception) new UnauthorizedAccessException(exception.Message, exception) : (Exception) new MessagingException(exception.Message, exception);
      return true;
    }

    internal static SecurityToken GetMessagingToken(
      this TokenProvider tokenProvider,
      Uri namespaceAddress,
      string appliesTo,
      string action,
      bool bypassCache,
      TimeSpan timeout)
    {
      try
      {
        TimeSpan timeout1 = TimeoutHelper.Min(timeout, TokenProviderUtility.MaxGetTokenSyncTimeout);
        TokenProvider tokenProvider1 = tokenProvider;
        return tokenProvider1.EndGetToken(tokenProvider1.BeginGetToken(namespaceAddress, appliesTo, action, bypassCache, timeout1, (AsyncCallback) null, (object) null));
      }
      catch (TimeoutException ex)
      {
        throw;
      }
      catch (TokenProviderException ex)
      {
        throw new MessagingException(ex.Message, (Exception) ex);
      }
      catch (Exception ex)
      {
        if (!Fx.IsFatal(ex))
          throw new UnauthorizedAccessException(ex.Message, ex);
        throw;
      }
    }

    internal static SecurityToken GetToken(
      this TokenProvider tokenProvider,
      string appliesTo,
      string action,
      bool bypassCache,
      TimeSpan timeout)
    {
      TimeSpan timeout1 = TimeoutHelper.Min(timeout, TokenProviderUtility.MaxGetTokenSyncTimeout);
      TokenProvider tokenProvider1 = tokenProvider;
      return tokenProvider1.EndGetToken(tokenProvider1.BeginGetToken(appliesTo, action, bypassCache, timeout1, (AsyncCallback) null, (object) null));
    }

    internal static string GetWebToken(
      this TokenProvider tokenProvider,
      string appliesTo,
      string action,
      bool bypassCache,
      TimeSpan timeout)
    {
      TokenProvider tokenProvider1 = tokenProvider;
      return tokenProvider1.EndGetWebToken(tokenProvider1.BeginGetWebToken(appliesTo, action, bypassCache, timeout, (AsyncCallback) null, (object) null));
    }

    internal static string GetMessagingWebToken(
      this TokenProvider tokenProvider,
      Uri namespaceAddress,
      string appliesTo,
      string action,
      bool bypassCache,
      TimeSpan timeout)
    {
      try
      {
        TokenProvider tokenProvider1 = tokenProvider;
        return tokenProvider1.EndGetWebToken(tokenProvider1.BeginGetWebToken(namespaceAddress, appliesTo, action, bypassCache, timeout, (AsyncCallback) null, (object) null));
      }
      catch (TimeoutException ex)
      {
        throw;
      }
      catch (TokenProviderException ex)
      {
        throw new MessagingException(ex.Message, (Exception) ex);
      }
      catch (Exception ex)
      {
        if (!Fx.IsFatal(ex))
          throw new UnauthorizedAccessException(ex.Message, ex);
        throw;
      }
    }
  }
}
