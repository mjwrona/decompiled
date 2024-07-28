// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.TransportException
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using Microsoft.Azure.Cosmos.Core.Trace;
using Microsoft.Azure.Documents.Rntbd;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Net.Sockets;
using System.Threading;

namespace Microsoft.Azure.Documents
{
  [Serializable]
  internal sealed class TransportException : Exception
  {
    private static readonly Lazy<Dictionary<TransportErrorCode, string>> lazyMessageMap = new Lazy<Dictionary<TransportErrorCode, string>>(new Func<Dictionary<TransportErrorCode, string>>(TransportException.GetErrorTextMap), LazyThreadSafetyMode.ExecutionAndPublication);
    private static TransportExceptionCounters transportExceptionCounters = new TransportExceptionCounters();
    private readonly object mutex = new object();
    private CpuLoadHistory cpuHistory;

    public TransportException(
      TransportErrorCode errorCode,
      Exception innerException,
      Guid activityId,
      Uri requestUri,
      string sourceDescription,
      bool userPayload,
      bool payloadSent)
      : base(TransportException.LoadMessage(errorCode), innerException)
    {
      this.Timestamp = DateTime.UtcNow;
      this.ErrorCode = errorCode;
      this.ActivityId = activityId;
      this.RequestUri = requestUri;
      this.Source = sourceDescription;
      this.UserRequestSent = TransportException.IsUserRequestSent(errorCode, userPayload, payloadSent);
      TransportException.UpdateCounters(requestUri, innerException);
    }

    public override string Message
    {
      get
      {
        Exception baseException = this.GetBaseException();
        string str1;
        switch (baseException)
        {
          case SocketException socketException:
            str1 = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "socket error {0} [0x{1:X8}]", (object) socketException.SocketErrorCode, (object) (int) socketException.SocketErrorCode);
            break;
          case Win32Exception win32Exception:
            str1 = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Windows error 0x{0:X8}", (object) win32Exception.NativeErrorCode);
            break;
          default:
            str1 = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "HRESULT 0x{0:X8}", (object) baseException.HResult);
            break;
        }
        string str2 = "not available";
        CpuLoadHistory cpuHistory = this.CpuHistory;
        if (cpuHistory != null)
          str2 = cpuHistory.ToString();
        return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0} (Time: {1:o}, activity ID: {2}, error code: {3} [0x{4:X4}], base error: {5}, URI: {6}, connection: {7}, payload sent: {8}, CPU history: {9}, CPU count: {10})", (object) base.Message, (object) this.Timestamp, (object) this.ActivityId, (object) this.ErrorCode, (object) (int) this.ErrorCode, (object) str1, (object) this.RequestUri, (object) this.Source, (object) this.UserRequestSent, (object) str2, (object) Environment.ProcessorCount);
      }
    }

    public DateTime Timestamp { get; private set; }

    public DateTime? RequestStartTime { get; set; }

    public DateTime? RequestEndTime { get; set; }

    public ResourceType ResourceType { get; set; }

    public OperationType OperationType { get; set; }

    public TransportErrorCode ErrorCode { get; private set; }

    public Guid ActivityId { get; private set; }

    public Uri RequestUri { get; private set; }

    public bool UserRequestSent { get; private set; }

    public bool IsClientCpuOverloaded
    {
      get
      {
        CpuLoadHistory cpuHistory = this.CpuHistory;
        return cpuHistory != null && cpuHistory.IsCpuOverloaded;
      }
    }

    public static bool IsTimeout(TransportErrorCode errorCode) => errorCode == TransportErrorCode.ChannelOpenTimeout || errorCode == TransportErrorCode.DnsResolutionTimeout || errorCode == TransportErrorCode.ConnectTimeout || errorCode == TransportErrorCode.SslNegotiationTimeout || errorCode == TransportErrorCode.TransportNegotiationTimeout || errorCode == TransportErrorCode.RequestTimeout || errorCode == TransportErrorCode.SendLockTimeout || errorCode == TransportErrorCode.SendTimeout || errorCode == TransportErrorCode.ReceiveTimeout;

    internal void SetCpuLoad(CpuLoadHistory cpuHistory)
    {
      lock (this.mutex)
      {
        if (this.cpuHistory != null)
          return;
        this.cpuHistory = cpuHistory;
      }
    }

    private CpuLoadHistory CpuHistory
    {
      get
      {
        lock (this.mutex)
          return this.cpuHistory;
      }
    }

    private static bool IsUserRequestSent(
      TransportErrorCode errorCode,
      bool userPayload,
      bool payloadSent)
    {
      if (!userPayload)
        return false;
      return payloadSent || TransportException.IsTimeout(errorCode);
    }

    private static string LoadMessage(TransportErrorCode errorCode) => string.Format((IFormatProvider) CultureInfo.CurrentUICulture, RMResources.TransportExceptionMessage, (object) TransportException.GetErrorText(errorCode));

    private static string GetErrorText(TransportErrorCode errorCode)
    {
      string str;
      return TransportException.lazyMessageMap.Value.TryGetValue(errorCode, out str) ? str : string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}", (object) errorCode);
    }

    private static Dictionary<TransportErrorCode, string> GetErrorTextMap() => new Dictionary<TransportErrorCode, string>()
    {
      {
        TransportErrorCode.ChannelMultiplexerClosed,
        RMResources.ChannelMultiplexerClosedTransportError
      },
      {
        TransportErrorCode.ChannelOpenFailed,
        RMResources.ChannelOpenFailedTransportError
      },
      {
        TransportErrorCode.ChannelOpenTimeout,
        RMResources.ChannelOpenTimeoutTransportError
      },
      {
        TransportErrorCode.ConnectFailed,
        RMResources.ConnectFailedTransportError
      },
      {
        TransportErrorCode.ConnectTimeout,
        RMResources.ConnectTimeoutTransportError
      },
      {
        TransportErrorCode.ConnectionBroken,
        RMResources.ConnectionBrokenTransportError
      },
      {
        TransportErrorCode.DnsResolutionFailed,
        RMResources.DnsResolutionFailedTransportError
      },
      {
        TransportErrorCode.DnsResolutionTimeout,
        RMResources.DnsResolutionTimeoutTransportError
      },
      {
        TransportErrorCode.ReceiveFailed,
        RMResources.ReceiveFailedTransportError
      },
      {
        TransportErrorCode.ReceiveStreamClosed,
        RMResources.ReceiveStreamClosedTransportError
      },
      {
        TransportErrorCode.ReceiveTimeout,
        RMResources.ReceiveTimeoutTransportError
      },
      {
        TransportErrorCode.RequestTimeout,
        RMResources.RequestTimeoutTransportError
      },
      {
        TransportErrorCode.SendFailed,
        RMResources.SendFailedTransportError
      },
      {
        TransportErrorCode.SendLockTimeout,
        RMResources.SendLockTimeoutTransportError
      },
      {
        TransportErrorCode.SendTimeout,
        RMResources.SendTimeoutTransportError
      },
      {
        TransportErrorCode.SslNegotiationFailed,
        RMResources.SslNegotiationFailedTransportError
      },
      {
        TransportErrorCode.SslNegotiationTimeout,
        RMResources.SslNegotiationTimeoutTransportError
      },
      {
        TransportErrorCode.TransportNegotiationTimeout,
        RMResources.TransportNegotiationTimeoutTransportError
      },
      {
        TransportErrorCode.Unknown,
        RMResources.UnknownTransportError
      }
    };

    private static void UpdateCounters(Uri requestUri, Exception innerException)
    {
      if (innerException == null || innerException is TransportException)
        return;
      innerException = innerException.GetBaseException();
      switch (innerException)
      {
        case SocketException socketException:
          if (socketException.SocketErrorCode != SocketError.NoBufferSpaceAvailable)
            break;
          TransportException.transportExceptionCounters.IncrementEphemeralPortExhaustion();
          break;
        case Win32Exception win32Exception when win32Exception.NativeErrorCode == -2146893008:
          DefaultTrace.TraceWarning("Decryption failure. Exception text: {0}. Native error code: 0x{1:X8}. Remote endpoint: {2}", (object) win32Exception.Message, (object) win32Exception.NativeErrorCode, (object) string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}:{1}", (object) requestUri.DnsSafeHost, (object) requestUri.Port));
          TransportException.transportExceptionCounters.IncrementDecryptionFailures();
          break;
      }
    }

    internal static void SetCounters(
      TransportExceptionCounters transportExceptionCounters)
    {
      TransportException.transportExceptionCounters = transportExceptionCounters != null ? transportExceptionCounters : throw new ArgumentNullException(nameof (transportExceptionCounters));
    }
  }
}
