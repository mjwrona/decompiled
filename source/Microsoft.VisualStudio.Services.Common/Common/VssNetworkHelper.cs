// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Common.VssNetworkHelper
// Assembly: Microsoft.VisualStudio.Services.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9C46641-2F1F-44C4-9C2E-4328CD5FFC63
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Common.dll

using System;
using System.ComponentModel;
using System.Data.Services.Client;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace Microsoft.VisualStudio.Services.Common
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public static class VssNetworkHelper
  {
    public const HttpStatusCode TooManyRequests = (HttpStatusCode) 429;

    public static bool IsTransientNetworkException(Exception ex) => VssNetworkHelper.IsTransientNetworkException(ex, new VssHttpRetryOptions());

    public static bool IsTransientNetworkException(Exception ex, VssHttpRetryOptions options) => VssNetworkHelper.IsTransientNetworkException(ex, options, out HttpStatusCode? _, out WebExceptionStatus? _, out SocketError? _, out WinHttpErrorCode? _, out CurlErrorCode? _);

    public static bool IsTransientNetworkException(
      Exception ex,
      out HttpStatusCode? httpStatusCode,
      out WebExceptionStatus? webExceptionStatus,
      out SocketError? socketErrorCode,
      out WinHttpErrorCode? winHttpErrorCode,
      out CurlErrorCode? curlErrorCode)
    {
      return VssNetworkHelper.IsTransientNetworkException(ex, VssHttpRetryOptions.Default, out httpStatusCode, out webExceptionStatus, out socketErrorCode, out winHttpErrorCode, out curlErrorCode);
    }

    public static bool IsTransientNetworkException(
      Exception ex,
      VssHttpRetryOptions options,
      out HttpStatusCode? httpStatusCode,
      out WebExceptionStatus? webExceptionStatus,
      out SocketError? socketErrorCode,
      out WinHttpErrorCode? winHttpErrorCode,
      out CurlErrorCode? curlErrorCode)
    {
      httpStatusCode = new HttpStatusCode?();
      webExceptionStatus = new WebExceptionStatus?();
      socketErrorCode = new SocketError?();
      winHttpErrorCode = new WinHttpErrorCode?();
      curlErrorCode = new CurlErrorCode?();
      for (; ex != null; ex = ex.InnerException)
      {
        if (VssNetworkHelper.IsTransientNetworkExceptionHelper(ex, options, out httpStatusCode, out webExceptionStatus, out socketErrorCode, out winHttpErrorCode, out curlErrorCode))
          return true;
      }
      return false;
    }

    private static bool IsTransientNetworkExceptionHelper(
      Exception ex,
      VssHttpRetryOptions options,
      out HttpStatusCode? httpStatusCode,
      out WebExceptionStatus? webExceptionStatus,
      out SocketError? socketErrorCode,
      out WinHttpErrorCode? winHttpErrorCode,
      out CurlErrorCode? curlErrorCode)
    {
      ArgumentUtility.CheckForNull<Exception>(ex, nameof (ex));
      httpStatusCode = new HttpStatusCode?();
      webExceptionStatus = new WebExceptionStatus?();
      socketErrorCode = new SocketError?();
      winHttpErrorCode = new WinHttpErrorCode?();
      curlErrorCode = new CurlErrorCode?();
      switch (ex)
      {
        case WebException _:
          WebException webException = (WebException) ex;
          if (webException.Response != null && webException.Response is HttpWebResponse)
          {
            HttpWebResponse response = (HttpWebResponse) webException.Response;
            httpStatusCode = new HttpStatusCode?(response.StatusCode);
            if (options.RetryableStatusCodes.Contains(response.StatusCode))
              return true;
          }
          webExceptionStatus = new WebExceptionStatus?(webException.Status);
          if (webException.Status == WebExceptionStatus.ConnectFailure || webException.Status == WebExceptionStatus.ConnectionClosed || webException.Status == WebExceptionStatus.KeepAliveFailure || webException.Status == WebExceptionStatus.NameResolutionFailure || webException.Status == WebExceptionStatus.ReceiveFailure || webException.Status == WebExceptionStatus.SendFailure || webException.Status == WebExceptionStatus.Timeout)
            return true;
          break;
        case SocketException _:
          SocketException socketException = (SocketException) ex;
          socketErrorCode = new SocketError?(socketException.SocketErrorCode);
          if (socketException.SocketErrorCode == SocketError.Interrupted || socketException.SocketErrorCode == SocketError.NetworkDown || socketException.SocketErrorCode == SocketError.NetworkUnreachable || socketException.SocketErrorCode == SocketError.NetworkReset || socketException.SocketErrorCode == SocketError.ConnectionAborted || socketException.SocketErrorCode == SocketError.ConnectionReset || socketException.SocketErrorCode == SocketError.TimedOut || socketException.SocketErrorCode == SocketError.HostDown || socketException.SocketErrorCode == SocketError.HostUnreachable || socketException.SocketErrorCode == SocketError.TryAgain)
            return true;
          break;
        case Win32Exception _:
          int nativeErrorCode = ((Win32Exception) ex).NativeErrorCode;
          if (nativeErrorCode > 12000 && nativeErrorCode <= 12188)
          {
            winHttpErrorCode = new WinHttpErrorCode?((WinHttpErrorCode) nativeErrorCode);
            WinHttpErrorCode? nullable1 = winHttpErrorCode;
            WinHttpErrorCode winHttpErrorCode1 = WinHttpErrorCode.ERROR_WINHTTP_CANNOT_CONNECT;
            if (!(nullable1.GetValueOrDefault() == winHttpErrorCode1 & nullable1.HasValue))
            {
              WinHttpErrorCode? nullable2 = winHttpErrorCode;
              WinHttpErrorCode winHttpErrorCode2 = WinHttpErrorCode.ERROR_WINHTTP_CONNECTION_ERROR;
              if (!(nullable2.GetValueOrDefault() == winHttpErrorCode2 & nullable2.HasValue))
              {
                WinHttpErrorCode? nullable3 = winHttpErrorCode;
                WinHttpErrorCode winHttpErrorCode3 = WinHttpErrorCode.ERROR_WINHTTP_INTERNAL_ERROR;
                if (!(nullable3.GetValueOrDefault() == winHttpErrorCode3 & nullable3.HasValue))
                {
                  WinHttpErrorCode? nullable4 = winHttpErrorCode;
                  WinHttpErrorCode winHttpErrorCode4 = WinHttpErrorCode.ERROR_WINHTTP_NAME_NOT_RESOLVED;
                  if (!(nullable4.GetValueOrDefault() == winHttpErrorCode4 & nullable4.HasValue))
                  {
                    WinHttpErrorCode? nullable5 = winHttpErrorCode;
                    WinHttpErrorCode winHttpErrorCode5 = WinHttpErrorCode.ERROR_WINHTTP_SECURE_FAILURE;
                    if (!(nullable5.GetValueOrDefault() == winHttpErrorCode5 & nullable5.HasValue))
                    {
                      WinHttpErrorCode? nullable6 = winHttpErrorCode;
                      WinHttpErrorCode winHttpErrorCode6 = WinHttpErrorCode.ERROR_WINHTTP_TIMEOUT;
                      if (!(nullable6.GetValueOrDefault() == winHttpErrorCode6 & nullable6.HasValue))
                        break;
                    }
                  }
                }
              }
            }
            return true;
          }
          break;
        case IOException _:
          if (ex.InnerException != null && ex.InnerException is Win32Exception)
          {
            string stackTrace = ex.StackTrace;
            if (stackTrace != null && stackTrace.IndexOf("System.Net.Security._SslStream.StartWriting(", StringComparison.Ordinal) >= 0)
              return true;
            break;
          }
          if (ex.Message.Contains("Unable to read data from the transport connection: The connection was closed"))
            return true;
          break;
        default:
          if (ex.GetType().Name == "CurlException")
          {
            if (ex.HResult > 0 && ex.HResult < 94)
            {
              curlErrorCode = new CurlErrorCode?((CurlErrorCode) ex.HResult);
              CurlErrorCode? nullable = curlErrorCode;
              CurlErrorCode curlErrorCode1 = CurlErrorCode.CURLE_COULDNT_RESOLVE_PROXY;
              if (!(nullable.GetValueOrDefault() == curlErrorCode1 & nullable.HasValue))
              {
                nullable = curlErrorCode;
                CurlErrorCode curlErrorCode2 = CurlErrorCode.CURLE_COULDNT_RESOLVE_HOST;
                if (!(nullable.GetValueOrDefault() == curlErrorCode2 & nullable.HasValue))
                {
                  nullable = curlErrorCode;
                  CurlErrorCode curlErrorCode3 = CurlErrorCode.CURLE_COULDNT_CONNECT;
                  if (!(nullable.GetValueOrDefault() == curlErrorCode3 & nullable.HasValue))
                  {
                    nullable = curlErrorCode;
                    CurlErrorCode curlErrorCode4 = CurlErrorCode.CURLE_HTTP2;
                    if (!(nullable.GetValueOrDefault() == curlErrorCode4 & nullable.HasValue))
                    {
                      nullable = curlErrorCode;
                      CurlErrorCode curlErrorCode5 = CurlErrorCode.CURLE_PARTIAL_FILE;
                      if (!(nullable.GetValueOrDefault() == curlErrorCode5 & nullable.HasValue))
                      {
                        nullable = curlErrorCode;
                        CurlErrorCode curlErrorCode6 = CurlErrorCode.CURLE_WRITE_ERROR;
                        if (!(nullable.GetValueOrDefault() == curlErrorCode6 & nullable.HasValue))
                        {
                          nullable = curlErrorCode;
                          CurlErrorCode curlErrorCode7 = CurlErrorCode.CURLE_UPLOAD_FAILED;
                          if (!(nullable.GetValueOrDefault() == curlErrorCode7 & nullable.HasValue))
                          {
                            nullable = curlErrorCode;
                            CurlErrorCode curlErrorCode8 = CurlErrorCode.CURLE_READ_ERROR;
                            if (!(nullable.GetValueOrDefault() == curlErrorCode8 & nullable.HasValue))
                            {
                              nullable = curlErrorCode;
                              CurlErrorCode curlErrorCode9 = CurlErrorCode.CURLE_OPERATION_TIMEDOUT;
                              if (!(nullable.GetValueOrDefault() == curlErrorCode9 & nullable.HasValue))
                              {
                                nullable = curlErrorCode;
                                CurlErrorCode curlErrorCode10 = CurlErrorCode.CURLE_INTERFACE_FAILED;
                                if (!(nullable.GetValueOrDefault() == curlErrorCode10 & nullable.HasValue))
                                {
                                  nullable = curlErrorCode;
                                  CurlErrorCode curlErrorCode11 = CurlErrorCode.CURLE_GOT_NOTHING;
                                  if (!(nullable.GetValueOrDefault() == curlErrorCode11 & nullable.HasValue))
                                  {
                                    nullable = curlErrorCode;
                                    CurlErrorCode curlErrorCode12 = CurlErrorCode.CURLE_SEND_ERROR;
                                    if (!(nullable.GetValueOrDefault() == curlErrorCode12 & nullable.HasValue))
                                    {
                                      nullable = curlErrorCode;
                                      CurlErrorCode curlErrorCode13 = CurlErrorCode.CURLE_RECV_ERROR;
                                      if (!(nullable.GetValueOrDefault() == curlErrorCode13 & nullable.HasValue))
                                        break;
                                    }
                                  }
                                }
                              }
                            }
                          }
                        }
                      }
                    }
                  }
                }
              }
              return true;
            }
            break;
          }
          switch (ex)
          {
            case DataServiceRequestException _:
            case DataServiceClientException _:
              return true;
          }
          break;
      }
      return false;
    }
  }
}
