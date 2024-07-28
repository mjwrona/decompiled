// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.TokenProviderHelper
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using Microsoft.Azure.NotificationHubs.Common;
using Microsoft.Azure.NotificationHubs.Diagnostics;
using Microsoft.Azure.NotificationHubs.Messaging;
using Microsoft.Azure.NotificationHubs.Properties;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IdentityModel.Tokens;
using System.IO;
using System.Net;
using System.Text;
using System.Web;

namespace Microsoft.Azure.NotificationHubs
{
  internal static class TokenProviderHelper
  {
    internal static string ExtractSolutionFromHostname(string hostname)
    {
      string str = !string.IsNullOrEmpty(hostname) ? hostname.ToLowerInvariant() : throw new ArgumentException(SRClient.NullHostname);
      string lowerInvariant = RelayEnvironment.RelayHostRootName.ToLowerInvariant();
      if (!str.EndsWith(lowerInvariant, StringComparison.Ordinal))
        throw new ArgumentException(SRClient.MismatchServiceBusDomain((object) str, (object) lowerInvariant));
      string[] strArray = str.Replace(lowerInvariant, string.Empty).Split(new char[1]
      {
        '.'
      }, StringSplitOptions.RemoveEmptyEntries);
      if (strArray.Length > 1)
        throw new ArgumentException(SRClient.UnsupportedServiceBusDomainPrefix((object) str, (object) lowerInvariant));
      string empty = string.Empty;
      if (strArray.Length == 1)
        empty = strArray[0];
      return empty;
    }

    internal static IAsyncResult BeginGetAccessTokenByAssertion(
      Uri requestUri,
      string appliesTo,
      string requestToken,
      string simpleAuthAssertionFormat,
      TimeSpan timeout,
      AsyncCallback callback,
      object state)
    {
      return TokenProviderHelper.BeginGetAccessTokenCore(requestUri, appliesTo, requestToken, simpleAuthAssertionFormat, timeout, callback, state);
    }

    internal static TokenProviderHelper.TokenResult<SecurityToken> EndGetAccessTokenByAssertion(
      IAsyncResult result)
    {
      string expiresIn;
      string audience1;
      string accessTokenCore = TokenProviderHelper.EndGetAccessTokenCore(result, out expiresIn, out audience1);
      DateTime dateTime = TokenProviderHelper.ConvertExpiry(expiresIn);
      DateTime expiry = dateTime;
      string audience2 = audience1;
      SimpleWebSecurityToken webSecurityToken = new SimpleWebSecurityToken(accessTokenCore, expiry, audience2);
      return new TokenProviderHelper.TokenResult<SecurityToken>()
      {
        CacheUntil = dateTime,
        Token = (SecurityToken) webSecurityToken
      };
    }

    internal static TokenProviderHelper.TokenResult<SecurityToken> GetAccessTokenByAssertion(
      Uri requestUri,
      string appliesTo,
      string requestToken,
      string simpleAuthAssertionFormat,
      TimeSpan timeout)
    {
      string expiresIn;
      string audience1;
      string accessTokenCore = TokenProviderHelper.GetAccessTokenCore(requestUri, appliesTo, requestToken, simpleAuthAssertionFormat, timeout, out expiresIn, out audience1);
      DateTime dateTime = TokenProviderHelper.ConvertExpiry(expiresIn);
      DateTime expiry = dateTime;
      string audience2 = audience1;
      SimpleWebSecurityToken webSecurityToken = new SimpleWebSecurityToken(accessTokenCore, expiry, audience2);
      return new TokenProviderHelper.TokenResult<SecurityToken>()
      {
        CacheUntil = dateTime,
        Token = (SecurityToken) webSecurityToken
      };
    }

    internal static TokenProviderHelper.TokenResult<string> GetHttpAuthAccessTokenByAssertion(
      Uri requestUri,
      string appliesTo,
      string requestToken,
      string simpleAuthAssertionFormat,
      TimeSpan timeout)
    {
      string expiresIn;
      string accessTokenCore = TokenProviderHelper.GetAccessTokenCore(requestUri, appliesTo, requestToken, simpleAuthAssertionFormat, timeout, out expiresIn, out string _);
      DateTime dateTime = TokenProviderHelper.ConvertExpiry(expiresIn);
      string str = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0} {1}=\"{2}\"", new object[3]
      {
        (object) "WRAP",
        (object) "access_token",
        (object) accessTokenCore
      });
      return new TokenProviderHelper.TokenResult<string>()
      {
        CacheUntil = dateTime,
        Token = str
      };
    }

    internal static Uri GetStsUri(Uri stsUri, string appliesTo) => stsUri == (Uri) null ? ServiceBusEnvironment.CreateAccessControlUri(TokenProviderHelper.ExtractSolutionFromHostname(new Uri(appliesTo).Host)) : new Uri(stsUri, "WRAPv0.9/");

    private static DateTime ConvertExpiry(string expiresIn) => DateTime.UtcNow + TimeSpan.FromSeconds(Convert.ToDouble(expiresIn, (IFormatProvider) CultureInfo.InvariantCulture));

    private static void ExtractExpiresInAndAudience(
      string simpleWebToken,
      out DateTime expiresIn,
      out string audience)
    {
      expiresIn = DateTime.MinValue;
      audience = (string) null;
      if (!string.IsNullOrWhiteSpace(simpleWebToken))
      {
        IDictionary<string, string> dictionary = TokenProviderHelper.Decode(simpleWebToken);
        string str = dictionary["ExpiresOn"];
        if (string.IsNullOrWhiteSpace(str))
          TokenProviderHelper.ThrowException(SR.GetString(Resources.TokenProviderEmptyExpiration));
        expiresIn = TokenConstants.WrapBaseTime + TimeSpan.FromSeconds(double.Parse(HttpUtility.UrlDecode(str.Trim()), (IFormatProvider) CultureInfo.InvariantCulture));
        audience = dictionary["Audience"];
        if (!string.IsNullOrWhiteSpace(str))
          return;
        TokenProviderHelper.ThrowException(SR.GetString(Resources.TokenProviderEmptyAudience));
      }
      else
        TokenProviderHelper.ThrowException(SR.GetString(Resources.TokenProviderEmptyToken));
    }

    private static IDictionary<string, string> Decode(string encodedString)
    {
      IDictionary<string, string> dictionary = (IDictionary<string, string>) new Dictionary<string, string>();
      if (!string.IsNullOrWhiteSpace(encodedString))
      {
        foreach (string str in (IEnumerable<string>) encodedString.Split(new char[1]
        {
          '&'
        }, StringSplitOptions.None))
        {
          string[] strArray = str.Split(new char[1]{ '=' }, StringSplitOptions.None);
          if (strArray.Length != 2)
            throw new FormatException(SRClient.InvalidEncoding);
          dictionary.Add(HttpUtility.UrlDecode(strArray[0]), HttpUtility.UrlDecode(strArray[1]));
        }
      }
      return dictionary;
    }

    private static void ExtractAccessToken(
      string urlParameters,
      out string token,
      out string expiresIn,
      out string audience)
    {
      token = (string) null;
      expiresIn = (string) null;
      audience = (string) null;
      if (urlParameters != null)
      {
        string str1 = urlParameters;
        char[] chArray = new char[1]{ '&' };
        foreach (string str2 in str1.Split(chArray))
        {
          string[] strArray = str2.Split('=');
          if (strArray.Length == 2)
          {
            switch (strArray[0].Trim())
            {
              case "wrap_access_token":
                token = HttpUtility.UrlDecode(strArray[1].Trim());
                audience = TokenProviderHelper.ExtractAudience(token);
                continue;
              case "wrap_access_token_expires_in":
                expiresIn = HttpUtility.UrlDecode(strArray[1].Trim());
                continue;
              default:
                continue;
            }
          }
          else
            TokenProviderHelper.ThrowException(SR.GetString(Resources.TokenProviderInvalidTokenParameter, (object) str2));
        }
      }
      if (string.IsNullOrEmpty(token))
        TokenProviderHelper.ThrowException(SR.GetString(Resources.TokenProviderEmptyToken));
      if (string.IsNullOrEmpty(expiresIn))
        TokenProviderHelper.ThrowException(SR.GetString(Resources.TokenProviderEmptyExpiration));
      if (!string.IsNullOrEmpty(audience))
        return;
      TokenProviderHelper.ThrowException(SR.GetString(Resources.TokenProviderEmptyAudience));
    }

    private static string ExtractAudience(string token)
    {
      string audience = (string) null;
      if (token != null)
      {
        string str1 = token;
        char[] chArray = new char[1]{ '&' };
        foreach (string str2 in str1.Split(chArray))
        {
          string[] strArray = str2.Split('=');
          if (strArray.Length == 2)
          {
            if (strArray[0].Trim() == "Audience")
              audience = HttpUtility.UrlDecode(strArray[1].Trim());
          }
          else
            TokenProviderHelper.ThrowException(SR.GetString(Resources.TokenProviderInvalidTokenParameter, (object) str2));
        }
      }
      if (string.IsNullOrEmpty(audience))
        TokenProviderHelper.ThrowException(SR.GetString(Resources.TokenProviderEmptyAudience));
      return audience;
    }

    internal static IAsyncResult BeginGetAccessTokenCore(
      Uri requestUri,
      string appliesTo,
      string requestToken,
      string simpleAuthAssertionFormat,
      TimeSpan timeout,
      AsyncCallback callback,
      object state)
    {
      return (IAsyncResult) new TokenProviderHelper.TokenRequestAsyncResult(requestUri, appliesTo, requestToken, simpleAuthAssertionFormat, timeout, callback, state);
    }

    internal static string EndGetAccessTokenCore(
      IAsyncResult result,
      out string expiresIn,
      out string audience)
    {
      TokenProviderHelper.TokenRequestAsyncResult requestAsyncResult = AsyncResult<TokenProviderHelper.TokenRequestAsyncResult>.End(result);
      expiresIn = requestAsyncResult.ExpiresIn;
      audience = requestAsyncResult.Audience;
      return requestAsyncResult.AccessToken;
    }

    private static string GetAccessTokenCore(
      Uri requestUri,
      string appliesTo,
      string requestToken,
      string simpleAuthAssertionFormat,
      TimeSpan timeout,
      out string expiresIn,
      out string audience)
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, "{0}={1}", new object[2]
      {
        (object) "wrap_scope",
        (object) HttpUtility.UrlEncode(appliesTo)
      });
      stringBuilder.Append('&');
      stringBuilder.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, "{0}={1}", new object[2]
      {
        (object) "wrap_assertion_format",
        (object) simpleAuthAssertionFormat
      });
      stringBuilder.Append('&');
      stringBuilder.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, "{0}={1}", new object[2]
      {
        (object) "wrap_assertion",
        (object) HttpUtility.UrlEncode(requestToken)
      });
      byte[] bytes = Encoding.UTF8.GetBytes(stringBuilder.ToString());
      string token = (string) null;
      expiresIn = (string) null;
      audience = (string) null;
      try
      {
        HttpWebRequest httpWebRequest = WebRequest.Create(requestUri) as HttpWebRequest;
        if (ServiceBusEnvironment.Proxy != null)
          httpWebRequest.Proxy = ServiceBusEnvironment.Proxy;
        httpWebRequest.ServicePoint.MaxIdleTime = Constants.ServicePointMaxIdleTimeMilliSeconds;
        httpWebRequest.AllowAutoRedirect = true;
        httpWebRequest.MaximumAutomaticRedirections = 1;
        httpWebRequest.Method = "POST";
        httpWebRequest.ContentType = "application/x-www-form-urlencoded";
        httpWebRequest.ContentLength = (long) bytes.Length;
        try
        {
          httpWebRequest.Timeout = Convert.ToInt32((object) timeout.TotalMilliseconds, (IFormatProvider) CultureInfo.InvariantCulture);
        }
        catch (OverflowException ex)
        {
          throw new ArgumentException(SRClient.TimeoutExceeded, (Exception) ex);
        }
        using (Stream requestStream = httpWebRequest.GetRequestStream())
          requestStream.Write(bytes, 0, bytes.Length);
        using (HttpWebResponse response = (HttpWebResponse) httpWebRequest.GetResponse())
        {
          using (Stream responseStream = response.GetResponseStream())
          {
            using (StreamReader streamReader = new StreamReader(responseStream, Encoding.UTF8))
              TokenProviderHelper.ExtractAccessToken(streamReader.ReadToEnd(), out token, out expiresIn, out audience);
          }
        }
      }
      catch (ArgumentException ex)
      {
        TokenProviderHelper.ThrowException(requestUri, (Exception) ex);
      }
      catch (WebException ex)
      {
        TokenProviderHelper.ThrowException(requestUri, ex);
      }
      return token;
    }

    public static string GetWindowsAccessTokenCore(
      IEnumerator<Uri> stsUris,
      Func<Uri, Uri> uriBuilder,
      string requestToken,
      TimeSpan timeout,
      out DateTime expiresIn,
      out string audience)
    {
      byte[] bytes = Encoding.UTF8.GetBytes(requestToken);
      string simpleWebToken = (string) null;
      expiresIn = DateTime.MinValue;
      audience = (string) null;
      bool flag = stsUris.MoveNext();
      while (flag)
      {
        Uri requestUri = uriBuilder(stsUris.Current);
        try
        {
          HttpWebRequest httpWebRequest = WebRequest.Create(requestUri) as HttpWebRequest;
          if (ServiceBusEnvironment.Proxy != null)
            httpWebRequest.Proxy = ServiceBusEnvironment.Proxy;
          httpWebRequest.ServicePoint.MaxIdleTime = Constants.ServicePointMaxIdleTimeMilliSeconds;
          httpWebRequest.AllowAutoRedirect = true;
          httpWebRequest.MaximumAutomaticRedirections = 1;
          httpWebRequest.Method = "POST";
          httpWebRequest.ContentType = "application/x-www-form-urlencoded";
          httpWebRequest.ContentLength = (long) bytes.Length;
          httpWebRequest.Timeout = TokenProviderHelper.ConvertToInt32(timeout);
          httpWebRequest.UseDefaultCredentials = true;
          AuthenticationManager.CustomTargetNameDictionary[requestUri.AbsoluteUri] = "HTTP\\";
          using (Stream requestStream = httpWebRequest.GetRequestStream())
            requestStream.Write(bytes, 0, bytes.Length);
          using (HttpWebResponse response = httpWebRequest.GetResponse() as HttpWebResponse)
          {
            using (Stream responseStream = response.GetResponseStream())
            {
              using (StreamReader streamReader = new StreamReader(responseStream, Encoding.UTF8))
              {
                simpleWebToken = streamReader.ReadToEnd();
                TokenProviderHelper.ExtractExpiresInAndAudience(simpleWebToken, out expiresIn, out audience);
              }
            }
          }
          flag = false;
        }
        catch (ArgumentException ex)
        {
          TokenProviderHelper.ThrowException(requestUri, (Exception) ex);
        }
        catch (WebException ex)
        {
          flag = stsUris.MoveNext();
          if (!flag)
            TokenProviderHelper.ThrowException(requestUri, ex);
        }
      }
      return simpleWebToken;
    }

    public static string GetOAuthAccessTokenCore(
      IEnumerator<Uri> stsUris,
      Func<Uri, Uri> uriBuilder,
      string requestToken,
      TimeSpan timeout,
      out DateTime expiresIn,
      out string audience)
    {
      byte[] bytes = Encoding.UTF8.GetBytes(requestToken);
      string simpleWebToken = (string) null;
      expiresIn = DateTime.MinValue;
      audience = (string) null;
      bool flag = stsUris.MoveNext();
      while (flag)
      {
        Uri requestUri = uriBuilder(stsUris.Current);
        try
        {
          HttpWebRequest httpWebRequest = WebRequest.Create(requestUri) as HttpWebRequest;
          if (ServiceBusEnvironment.Proxy != null)
            httpWebRequest.Proxy = ServiceBusEnvironment.Proxy;
          httpWebRequest.ServicePoint.MaxIdleTime = Constants.ServicePointMaxIdleTimeMilliSeconds;
          httpWebRequest.AllowAutoRedirect = true;
          httpWebRequest.MaximumAutomaticRedirections = 1;
          httpWebRequest.Method = "POST";
          httpWebRequest.ContentType = "application/x-www-form-urlencoded";
          httpWebRequest.ContentLength = (long) bytes.Length;
          httpWebRequest.Timeout = TokenProviderHelper.ConvertToInt32(timeout);
          using (Stream requestStream = httpWebRequest.GetRequestStream())
            requestStream.Write(bytes, 0, bytes.Length);
          using (HttpWebResponse response = httpWebRequest.GetResponse() as HttpWebResponse)
          {
            using (Stream responseStream = response.GetResponseStream())
            {
              using (StreamReader streamReader = new StreamReader(responseStream, Encoding.UTF8))
              {
                simpleWebToken = streamReader.ReadToEnd();
                TokenProviderHelper.ExtractExpiresInAndAudience(simpleWebToken, out expiresIn, out audience);
              }
            }
          }
          flag = false;
        }
        catch (ArgumentException ex)
        {
          TokenProviderHelper.ThrowException(requestUri, (Exception) ex);
        }
        catch (WebException ex)
        {
          flag = stsUris.MoveNext();
          if (!flag)
            TokenProviderHelper.ThrowException(requestUri, ex);
        }
      }
      return simpleWebToken;
    }

    private static int ConvertToInt32(TimeSpan timeout)
    {
      try
      {
        return Convert.ToInt32((object) timeout.TotalMilliseconds, (IFormatProvider) CultureInfo.InvariantCulture);
      }
      catch (OverflowException ex)
      {
        throw new ArgumentException(SRClient.TimeoutExceeded, (Exception) ex);
      }
    }

    private static void ThrowException(string message)
    {
      SecurityTokenException securityTokenException = new SecurityTokenException(message);
      if (DiagnosticUtility.ShouldTraceInformation)
        DiagnosticUtility.ExceptionUtility.TraceHandledException((Exception) securityTokenException, TraceEventType.Information);
      throw securityTokenException;
    }

    private static void ThrowException(Uri requestUri, WebException exception) => throw TokenProviderHelper.ConvertWebException(requestUri, exception);

    private static void ThrowException(Uri requestUri, Exception innerException) => throw TokenProviderHelper.ConvertException(requestUri, innerException);

    private static Exception ConvertWebException(Uri requestUri, WebException exception)
    {
      string str = string.Empty;
      if (exception != null && exception.Response != null)
      {
        using (exception.Response)
        {
          using (StreamReader streamReader = new StreamReader(exception.Response.GetResponseStream()))
            str = streamReader.ReadToEnd();
          if (exception.Response is HttpWebResponse response)
          {
            if (response.StatusCode != HttpStatusCode.OK)
            {
              if (response.StatusCode != HttpStatusCode.Continue)
              {
                Exception exception1 = (Exception) new TokenProviderHelper.InternalSecurityTokenException(SR.GetString(Resources.TokenProviderFailedSecurityToken, (object) requestUri.AbsoluteUri, (object) str), response.StatusCode, (Exception) exception);
                switch (response.StatusCode)
                {
                  case HttpStatusCode.NotFound:
                  case HttpStatusCode.BadGateway:
                  case HttpStatusCode.ServiceUnavailable:
                    exception1 = (Exception) new TokenProviderException(SR.GetString(Resources.TokenProviderServiceUnavailable, (object) requestUri.AbsoluteUri), (Exception) exception);
                    break;
                  case HttpStatusCode.RequestTimeout:
                  case HttpStatusCode.GatewayTimeout:
                    exception1 = (Exception) new TimeoutException(SR.GetString(Resources.TokenProviderTimeout, (object) requestUri.AbsoluteUri), (Exception) exception);
                    break;
                }
                if (DiagnosticUtility.ShouldTraceInformation)
                  DiagnosticUtility.ExceptionUtility.TraceHandledException(exception1, TraceEventType.Information);
                return exception1;
              }
            }
          }
        }
      }
      else
        str = exception == null ? "Unknown" : exception.Message;
      Exception exception2 = (Exception) new SecurityTokenException(SR.GetString(Resources.TokenProviderFailedSecurityToken, (object) requestUri.AbsoluteUri, (object) str), (Exception) exception);
      if (exception != null && (exception.Status == WebExceptionStatus.Timeout || exception.Status == WebExceptionStatus.RequestCanceled))
        exception2 = (Exception) new TimeoutException(SR.GetString(Resources.TokenProviderTimeout, (object) requestUri.AbsoluteUri), exception2);
      if (DiagnosticUtility.ShouldTraceInformation)
        DiagnosticUtility.ExceptionUtility.TraceHandledException(exception2, TraceEventType.Information);
      return exception2;
    }

    private static Exception ConvertException(Uri requestUri, Exception innerException)
    {
      SecurityTokenException securityTokenException = new SecurityTokenException(SR.GetString(Resources.TokenProviderFailedSecurityToken, (object) requestUri.AbsoluteUri, innerException == null ? (object) "Unknown" : (object) innerException.Message), innerException);
      if (DiagnosticUtility.ShouldTraceInformation)
        DiagnosticUtility.ExceptionUtility.TraceHandledException((Exception) securityTokenException, TraceEventType.Information);
      return (Exception) securityTokenException;
    }

    internal class TokenResult<TToken>
    {
      public DateTime CacheUntil { get; set; }

      public TToken Token { get; set; }
    }

    [Serializable]
    internal class InternalSecurityTokenException : SecurityTokenException
    {
      public InternalSecurityTokenException(
        string message,
        HttpStatusCode statusCode,
        Exception innerException)
        : base(message, innerException)
      {
        this.StatusCode = statusCode;
      }

      public HttpStatusCode StatusCode { get; private set; }
    }

    private sealed class TokenRequestAsyncResult : 
      IteratorAsyncResult<TokenProviderHelper.TokenRequestAsyncResult>
    {
      private static readonly Action<object> onCancelTimer = new Action<object>(TokenProviderHelper.TokenRequestAsyncResult.OnCancelTimer);
      private readonly Uri requestUri;
      private readonly string appliesTo;
      private readonly string requestToken;
      private readonly string simpleAuthAssertionFormat;
      private Stream requestStream;
      private HttpWebRequest request;
      private HttpWebResponse response;
      private string expiresIn;
      private string audience;
      private string accessToken;
      private byte[] body;
      private Stream sourceStream;
      private MemoryStream tmpStream;
      private IOThreadTimer requestCancelTimer;
      private volatile bool requestTimedOut;

      public TokenRequestAsyncResult(
        Uri requestUri,
        string appliesTo,
        string requestToken,
        string simpleAuthAssertionFormat,
        TimeSpan timeout,
        AsyncCallback callback,
        object state)
        : base(timeout, callback, state)
      {
        this.requestUri = requestUri;
        this.appliesTo = appliesTo;
        this.requestToken = requestToken;
        this.simpleAuthAssertionFormat = simpleAuthAssertionFormat;
        this.Start();
      }

      public string ExpiresIn => this.expiresIn;

      public string Audience => this.audience;

      public string AccessToken => this.accessToken;

      protected override IEnumerator<IteratorAsyncResult<TokenProviderHelper.TokenRequestAsyncResult>.AsyncStep> GetAsyncSteps()
      {
        try
        {
          StringBuilder stringBuilder = new StringBuilder();
          stringBuilder.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, "{0}={1}", new object[2]
          {
            (object) "wrap_scope",
            (object) HttpUtility.UrlEncode(this.appliesTo)
          });
          stringBuilder.Append('&');
          stringBuilder.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, "{0}={1}", new object[2]
          {
            (object) "wrap_assertion_format",
            (object) this.simpleAuthAssertionFormat
          });
          stringBuilder.Append('&');
          stringBuilder.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, "{0}={1}", new object[2]
          {
            (object) "wrap_assertion",
            (object) HttpUtility.UrlEncode(this.requestToken)
          });
          this.body = Encoding.UTF8.GetBytes(stringBuilder.ToString());
          this.request = (HttpWebRequest) WebRequest.Create(this.requestUri);
          if (ServiceBusEnvironment.Proxy != null)
            this.request.Proxy = ServiceBusEnvironment.Proxy;
          this.request.ServicePoint.MaxIdleTime = Constants.ServicePointMaxIdleTimeMilliSeconds;
          this.request.AllowAutoRedirect = true;
          this.request.MaximumAutomaticRedirections = 1;
          this.request.Method = "POST";
          this.request.ContentType = "application/x-www-form-urlencoded";
          this.request.ContentLength = (long) this.body.Length;
          try
          {
            this.request.Timeout = Convert.ToInt32((object) this.RemainingTime().TotalMilliseconds, (IFormatProvider) CultureInfo.InvariantCulture);
          }
          catch (OverflowException ex)
          {
            throw new ArgumentException(SRClient.TimeoutExceeded, (Exception) ex);
          }
        }
        catch (ArgumentException ex)
        {
          this.Complete(this.ConvertException((Exception) ex));
          yield break;
        }
        bool asyncSteps;
        try
        {
          this.requestCancelTimer = new IOThreadTimer(TokenProviderHelper.TokenRequestAsyncResult.onCancelTimer, (object) this, true);
          try
          {
            TimeSpan timeToCancelRequest = this.RemainingTime();
            if (timeToCancelRequest <= TimeSpan.Zero)
            {
              this.Complete((Exception) new TimeoutException(SRClient.OperationRequestTimedOut((object) this.OriginalTimeout)));
              asyncSteps = false;
            }
            else
            {
              yield return this.CallAsync((IteratorAsyncResult<TokenProviderHelper.TokenRequestAsyncResult>.BeginCall) ((thisPtr, t, c, s) =>
              {
                IAsyncResult requestStream = thisPtr.request.BeginGetRequestStream(c, s);
                thisPtr.requestCancelTimer.Set(timeToCancelRequest);
                return requestStream;
              }), (IteratorAsyncResult<TokenProviderHelper.TokenRequestAsyncResult>.EndCall) ((thisPtr, r) =>
              {
                TokenProviderHelper.TokenRequestAsyncResult requestAsyncResult = thisPtr;
                requestAsyncResult.requestStream = requestAsyncResult.request.EndGetRequestStream(r);
              }), IteratorAsyncResult<TokenProviderHelper.TokenRequestAsyncResult>.ExceptionPolicy.Continue);
              if (this.LastAsyncStepException != null)
              {
                this.Complete(this.ConvertException(this.LastAsyncStepException));
                asyncSteps = false;
              }
              else
              {
                yield return this.CallAsync((IteratorAsyncResult<TokenProviderHelper.TokenRequestAsyncResult>.BeginCall) ((thisPtr, t, c, s) => thisPtr.requestStream.BeginWrite(thisPtr.body, 0, thisPtr.body.Length, c, s)), (IteratorAsyncResult<TokenProviderHelper.TokenRequestAsyncResult>.EndCall) ((thisPtr, r) => thisPtr.requestStream.EndWrite(r)), IteratorAsyncResult<TokenProviderHelper.TokenRequestAsyncResult>.ExceptionPolicy.Continue);
                if (this.LastAsyncStepException != null)
                {
                  this.Complete(this.ConvertException(this.LastAsyncStepException));
                  asyncSteps = false;
                }
                else
                  goto label_13;
              }
            }
            goto label_36;
          }
          finally
          {
            if (this.requestStream != null)
              this.requestStream.Dispose();
            this.requestCancelTimer.Cancel();
          }
label_13:
          TimeSpan timeToCancelResponse = this.RemainingTime();
          if (timeToCancelResponse <= TimeSpan.Zero)
          {
            this.Complete((Exception) new TimeoutException(SRClient.OperationRequestTimedOut((object) this.OriginalTimeout)));
            asyncSteps = false;
          }
          else
          {
            yield return this.CallAsync((IteratorAsyncResult<TokenProviderHelper.TokenRequestAsyncResult>.BeginCall) ((thisPtr, t, c, s) =>
            {
              IAsyncResult response = thisPtr.request.BeginGetResponse(c, s);
              thisPtr.requestCancelTimer.Set(timeToCancelResponse);
              return response;
            }), (IteratorAsyncResult<TokenProviderHelper.TokenRequestAsyncResult>.EndCall) ((thisPtr, r) =>
            {
              TokenProviderHelper.TokenRequestAsyncResult requestAsyncResult = thisPtr;
              requestAsyncResult.response = (HttpWebResponse) requestAsyncResult.request.EndGetResponse(r);
            }), IteratorAsyncResult<TokenProviderHelper.TokenRequestAsyncResult>.ExceptionPolicy.Continue);
            if (this.LastAsyncStepException != null)
            {
              this.Complete(this.ConvertException(this.LastAsyncStepException));
              asyncSteps = false;
            }
            else
            {
              using (this.sourceStream = this.response.GetResponseStream())
              {
                MemoryStream memoryStream = this.tmpStream = new MemoryStream();
                try
                {
                  yield return this.CallAsync((IteratorAsyncResult<TokenProviderHelper.TokenRequestAsyncResult>.BeginCall) ((thisPtr, t, c, s) => (IAsyncResult) new TokenProviderHelper.StreamReaderAsyncResult(thisPtr.sourceStream, thisPtr.tmpStream, t, c, s)), (IteratorAsyncResult<TokenProviderHelper.TokenRequestAsyncResult>.EndCall) ((thisPtr, r) => AsyncResult<TokenProviderHelper.StreamReaderAsyncResult>.End(r)), IteratorAsyncResult<TokenProviderHelper.TokenRequestAsyncResult>.ExceptionPolicy.Continue);
                  if (this.LastAsyncStepException != null)
                  {
                    this.Complete(this.ConvertException(this.LastAsyncStepException));
                    asyncSteps = false;
                  }
                  else
                  {
                    try
                    {
                      this.tmpStream.Position = 0L;
                      using (StreamReader streamReader = new StreamReader((Stream) this.tmpStream, Encoding.UTF8))
                      {
                        TokenProviderHelper.ExtractAccessToken(streamReader.ReadToEnd(), out this.accessToken, out this.expiresIn, out this.audience);
                        goto label_12;
                      }
                    }
                    catch (Exception ex)
                    {
                      if (Fx.IsFatal(ex))
                      {
                        throw;
                      }
                      else
                      {
                        this.Complete(this.ConvertException(ex));
                        asyncSteps = false;
                      }
                    }
                  }
                  goto label_36;
                }
                finally
                {
                  memoryStream?.Dispose();
                }
label_12:
                memoryStream = (MemoryStream) null;
              }
              yield break;
            }
          }
label_36:;
        }
        finally
        {
          if (this.requestCancelTimer != null)
            this.requestCancelTimer.Cancel();
        }
        return asyncSteps;
      }

      private static void OnCancelTimer(object state)
      {
        TokenProviderHelper.TokenRequestAsyncResult requestAsyncResult = (TokenProviderHelper.TokenRequestAsyncResult) state;
        requestAsyncResult.requestTimedOut = true;
        requestAsyncResult.request.Abort();
      }

      private Exception ConvertException(Exception exception)
      {
        switch (exception)
        {
          case ArgumentException _:
            return TokenProviderHelper.ConvertException(this.requestUri, exception);
          case WebException _:
            return TokenProviderHelper.ConvertWebException(this.requestUri, (WebException) exception);
          case IOException _:
            if (this.requestTimedOut)
              return (Exception) new TimeoutException(SRClient.OperationRequestTimedOut((object) this.OriginalTimeout), exception);
            break;
        }
        return exception;
      }
    }

    private sealed class StreamReaderAsyncResult : 
      IteratorAsyncResult<TokenProviderHelper.StreamReaderAsyncResult>
    {
      private const int ReadBufferSize = 1024;
      private readonly Stream inputStream;
      private readonly MemoryStream outputStream;
      private readonly byte[] buffer;
      private int bytesRead;

      public StreamReaderAsyncResult(
        Stream inputStream,
        MemoryStream outputStream,
        TimeSpan timeout,
        AsyncCallback callback,
        object state)
        : base(timeout, callback, state)
      {
        this.inputStream = inputStream;
        this.outputStream = outputStream;
        this.buffer = new byte[1024];
        this.bytesRead = -1;
        this.Start();
      }

      protected override IEnumerator<IteratorAsyncResult<TokenProviderHelper.StreamReaderAsyncResult>.AsyncStep> GetAsyncSteps()
      {
        while (this.bytesRead != 0)
        {
          yield return this.CallAsync((IteratorAsyncResult<TokenProviderHelper.StreamReaderAsyncResult>.BeginCall) ((thisPtr, t, c, s) => thisPtr.inputStream.BeginRead(thisPtr.buffer, 0, 1024, c, s)), (IteratorAsyncResult<TokenProviderHelper.StreamReaderAsyncResult>.EndCall) ((thisPtr, r) =>
          {
            TokenProviderHelper.StreamReaderAsyncResult readerAsyncResult = thisPtr;
            readerAsyncResult.bytesRead = readerAsyncResult.inputStream.EndRead(r);
          }), IteratorAsyncResult<TokenProviderHelper.StreamReaderAsyncResult>.ExceptionPolicy.Transfer);
          this.outputStream.Write(this.buffer, 0, this.bytesRead);
        }
      }
    }
  }
}
